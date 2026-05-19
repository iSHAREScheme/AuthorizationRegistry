/**
 * Wire-format types for iSHARE delegation evidence.
 * Mirror of iSHARE.Models.DelegationEvidence.* with camelCase JSON serialisation.
 *
 * These are intentionally plain interfaces (not class-validator classes). JSON
 * Schema validation is done explicitly in the request pipeline so that the
 * iSHARE Conformance Test Tool messages remain consistent with the spec.
 */

export interface PolicyTargetResource {
  type: string;
  identifiers: string[];
  attributes: string[];
}

export interface PolicyTargetEnvironment {
  serviceProviders: string[];
}

export interface PolicyTarget {
  resource: PolicyTargetResource;
  actions?: string[];
  environment?: PolicyTargetEnvironment;
}

export interface PolicyRuleTarget {
  resource?: PolicyTargetResource;
  actions?: string[];
}

export type PolicyEffect = 'Permit' | 'Deny';

export interface PolicyRule {
  effect: PolicyEffect;
  target?: PolicyRuleTarget;
}

export interface Policy {
  target: PolicyTarget;
  rules: PolicyRule[];
}

export interface PolicySetTargetEnvironment {
  licenses: string[];
}

export interface PolicySetTarget {
  environment: PolicySetTargetEnvironment;
}

export interface PolicySet {
  maxDelegationDepth?: number;
  target: PolicySetTarget;
  policies: Policy[];
}

export interface DelegationTarget {
  accessSubject: string;
}

export interface DelegationEvidence {
  notBefore: number;
  notOnOrAfter: number;
  policyIssuer: string;
  target: DelegationTarget;
  policySets: PolicySet[];
}

/** Wrapper used when the evidence is round-tripped as a `delegationEvidence` JSON property. */
export interface DelegationEvidenceEnvelope {
  delegationEvidence: DelegationEvidence;
}
