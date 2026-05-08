import { Injectable } from '@nestjs/common';
import { DelegationMask } from './dto/delegation-mask.dto';

export type ValidationResult = { ok: true } | { ok: false; error: string };

/**
 * Port of iSHARE.IdentityServer.Delegation.DelegationMaskValidationService.
 * The mask is only valid if every policy in every policy set asks for at
 * least one rule with effect="Permit" — otherwise no evidence can be issued.
 */
@Injectable()
export class DelegationMaskValidationService {
  validate(mask: DelegationMask): ValidationResult {
    const sets = mask.delegationRequest.policySets;
    for (let i = 0; i < sets.length; i++) {
      const policies = sets[i].policies;
      for (let j = 0; j < policies.length; j++) {
        if (!policies[j].rules.some((r) => r.effect === 'Permit')) {
          return {
            ok: false,
            error: `delegationRequest.policySets[${i}].policies[${j}] does not contain a rule with the Effect 'Permit'`,
          };
        }
      }
    }
    return { ok: true };
  }
}
