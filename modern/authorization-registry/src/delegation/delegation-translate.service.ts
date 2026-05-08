import { Injectable } from '@nestjs/common';
import {
  DelegationEvidence,
  DelegationEvidenceEnvelope,
  Policy,
  PolicyRule,
  PolicySet,
} from './dto/delegation-evidence.dto';
import {
  DelegationMask,
  DelegationRequestPolicySet,
} from './dto/delegation-mask.dto';

/**
 * Port of iSHARE.IdentityServer.Delegation.DelegationTranslateService.
 *
 * Given a stored policy (as JSON, the canonical iSHARE delegation evidence
 * envelope) and a delegation mask, derive a fresh envelope where each mask
 * policy is converted to a single Permit/Deny rule based on whether it is
 * fully covered by the stored policy.
 */
@Injectable()
export class DelegationTranslateService {
  translate(mask: DelegationMask, storedPolicyJson: string): DelegationEvidenceEnvelope {
    const stored = JSON.parse(storedPolicyJson) as DelegationEvidenceEnvelope;
    const out = prepareResponse(stored.delegationEvidence);

    for (const maskSet of mask.delegationRequest.policySets) {
      for (const storedSet of stored.delegationEvidence.policySets) {
        out.delegationEvidence.policySets.push(buildResponsePolicySet(maskSet, storedSet));
      }
    }

    return out;
  }
}

function prepareResponse(src: DelegationEvidence): DelegationEvidenceEnvelope {
  return {
    delegationEvidence: {
      notBefore: src.notBefore,
      notOnOrAfter: src.notOnOrAfter,
      policyIssuer: src.policyIssuer,
      target: src.target,
      policySets: [],
    },
  };
}

function buildResponsePolicySet(
  maskSet: DelegationRequestPolicySet,
  storedSet: PolicySet,
): PolicySet {
  const responseSet: PolicySet = {
    maxDelegationDepth: storedSet.maxDelegationDepth,
    target: storedSet.target,
    policies: [],
  };

  for (const maskPolicy of maskSet.policies) {
    const matching = storedSet.policies.filter((p) => isMatchingPolicy(maskPolicy, p));

    const rule: PolicyRule =
      matching.length === 0 || matching.some((p) => accessDeniedToContainers(maskPolicy, p))
        ? { effect: 'Deny' }
        : { effect: 'Permit' };

    responseSet.policies.push({ target: maskPolicy.target, rules: [rule] });
  }

  return responseSet;
}

function isMatchingPolicy(maskPolicy: Policy, stored: Policy): boolean {
  return (
    isTypeMatch(maskPolicy, stored) &&
    isProviderMatch(maskPolicy, stored) &&
    hasAllIdentifiers(maskPolicy, stored) &&
    hasAllAttributes(maskPolicy, stored) &&
    hasAllActions(maskPolicy, stored)
  );
}

function isTypeMatch(maskPolicy: Policy, stored: Policy): boolean {
  return maskPolicy.target.resource.type === stored.target.resource.type;
}

function hasAllIdentifiers(maskPolicy: Policy, stored: Policy): boolean {
  const storedIds = stored.target.resource.identifiers;
  return maskPolicy.target.resource.identifiers.every(
    (mId) => (storedIds.length === 1 && storedIds.includes('*')) || storedIds.includes(mId),
  );
}

function hasAllAttributes(maskPolicy: Policy, stored: Policy): boolean {
  const storedAttrs = stored.target.resource.attributes;
  return maskPolicy.target.resource.attributes.every(
    (mAtt) => (storedAttrs.length === 1 && storedAttrs.includes('*')) || storedAttrs.includes(mAtt),
  );
}

function hasAllActions(maskPolicy: Policy, stored: Policy): boolean {
  const maskActions = maskPolicy.target.actions ?? [];
  const storedActions = stored.target.actions ?? [];
  if (maskActions.length === 0) return false;
  return maskActions.every(
    (mAct) =>
      (storedActions.length === 1 && storedActions.includes('*')) || storedActions.includes(mAct),
  );
}

function isProviderMatch(maskPolicy: Policy, stored: Policy): boolean {
  const storedEnv = stored.target.environment;
  if (!storedEnv) return true;
  const maskEnv = maskPolicy.target.environment;
  if (!maskEnv) return false;
  return maskEnv.serviceProviders.every((sp) => storedEnv.serviceProviders.includes(sp));
}

/**
 * iSHARE rule: even if a Permit rule covers the request, a later Deny rule
 * with overlapping resource/action terms blocks the permit. The legacy
 * implementation iterates the rule list reversed, but logically it just
 * checks "any Deny rule that overlaps".
 */
function accessDeniedToContainers(maskPolicy: Policy, stored: Policy): boolean {
  return stored.rules.some((rule) => {
    if (rule.effect !== 'Deny' || !rule.target?.resource) return false;
    const r = rule.target.resource;
    const m = maskPolicy.target.resource;

    if (r.type !== m.type) return false;
    if (!hasAnyOverlap(r.identifiers, m.identifiers)) return false;
    if (r.attributes.length > 0 && !hasAnyOverlap(r.attributes, m.attributes)) return false;

    const maskActions = maskPolicy.target.actions ?? [];
    const ruleActions = rule.target.actions ?? [];
    if (ruleActions.length > 0 && !hasAnyOverlap(ruleActions, maskActions)) return false;

    return true;
  });
}

function hasAnyOverlap(a: string[], b: string[]): boolean {
  if (a.length === 0 || b.length === 0) return true; // legacy "permit when unset"
  return a.some((x) => b.includes(x));
}

function _silence(_p: PolicyRule) {
  // keep PolicyRule referenced even if narrowed away
}
void _silence;
