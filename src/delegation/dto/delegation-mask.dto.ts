import { DelegationTarget, Policy } from './delegation-evidence.dto';

export interface DelegationRequestPolicySet {
  policies: Policy[];
}

export interface DelegationRequest {
  policyIssuer: string;
  target: DelegationTarget;
  policySets: DelegationRequestPolicySet[];
  notBefore?: number;
  notOnOrAfter?: number;
}

/** Maps to iSHARE.Models.DelegationMask.DelegationMask. */
export interface DelegationMask {
  delegationRequest: DelegationRequest;
  // Spec uses snake_case for these two fields on the wire.
  previous_steps?: string[];
  delegation_path?: string[];
}
