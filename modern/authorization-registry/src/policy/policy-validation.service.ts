import { Injectable } from '@nestjs/common';
import { DelegationService } from '../delegation/delegation.service';
import {
  DelegationPolicyJsonParser,
} from '../delegation/delegation-policy-parser';

export type ValidationResult = { ok: true } | { ok: false; error: string };

/**
 * Port of iSHARE.AuthorizationRegistry.Core.DelegationValidationService.
 * The party submitting the policy must be the policy issuer; (issuer,subject)
 * pairs must be unique on create; on edit they must remain stable.
 */
@Injectable()
export class PolicyValidationService {
  constructor(private readonly delegations: DelegationService) {}

  validateIssuer(
    callerPartyId: string,
    policyIssuer: string | undefined,
    accessSubject: string | undefined,
  ): ValidationResult {
    if (!policyIssuer || !accessSubject) {
      return { ok: false, error: 'Policy issuer and access subject are required.' };
    }
    if (callerPartyId !== policyIssuer) {
      return { ok: false, error: 'Policy issuer must be equal to your party id.' };
    }
    return { ok: true };
  }

  async validateCreate(policyJson: string, callerPartyId: string): Promise<ValidationResult> {
    const parsed = new DelegationPolicyJsonParser(policyJson);
    const issuerCheck = this.validateIssuer(callerPartyId, parsed.policyIssuer, parsed.accessSubject);
    if (!issuerCheck.ok) return issuerCheck;

    if (await this.delegations.exists(parsed.policyIssuer!, parsed.accessSubject!)) {
      return {
        ok: false,
        error: 'The combination policyIssuer - accessSubject already exists.',
      };
    }
    return { ok: true };
  }
}
