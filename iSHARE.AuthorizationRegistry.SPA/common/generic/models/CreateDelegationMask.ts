export class CreateDelegationMask {
  delegationRequest: DelegationRequest;
  constructor() {
    this.delegationRequest = new DelegationRequest();
  }
}
export class DelegationRequest {
  policyIssuer: string;
  target: Target;
  policySets: Array<PolicySet>;
  constructor() {
    this.policyIssuer = '';
    this.target = new Target();
    this.policySets = [new PolicySet()];
  }
}
export class Target {
  accessSubject: string;
  constructor() {
    this.accessSubject = '';
  }
}
export class PolicySet {
  policies: Array<Policy>;
  constructor() {
    this.policies = [new Policy()];
  }
}
export class Policy {
  target: PolicyTarget;
  rules: Array<PolicyRule>;
  constructor() {
    this.target = new PolicyTarget();
    this.rules = [new PolicyRule()];
  }
}
export class PolicyTarget {
  resource: PolicyTargetResource;
  actions: Array<string>;
  constructor() {
    this.resource = new PolicyTargetResource();
    this.actions = [];
  }
}
export class PolicyTargetResource {
  type: string;
  identifiers: Array<string>;
  attributes: Array<string>;
  constructor() {
    this.type = '';
    this.identifiers = [''];
    this.attributes = [''];
  }
}
export class PolicyRule {
  effect: string;
  constructor() {
    this.effect = 'Permit';
  }
}
