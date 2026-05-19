import { DelegationMaskValidationService } from './delegation-mask-validation.service';
import { DelegationMask } from './dto/delegation-mask.dto';

const mask = (effects: ('Permit' | 'Deny')[]): DelegationMask => ({
  delegationRequest: {
    policyIssuer: 'X',
    target: { accessSubject: 'Y' },
    policySets: [
      {
        policies: [
          {
            target: {
              resource: { type: 'T', identifiers: ['I'], attributes: ['*'] },
              actions: ['READ'],
            },
            rules: effects.map((e) => ({ effect: e })),
          },
        ],
      },
    ],
  },
});

describe('DelegationMaskValidationService', () => {
  const svc = new DelegationMaskValidationService();

  it('accepts a mask with at least one Permit rule per policy', () => {
    expect(svc.validate(mask(['Permit']))).toEqual({ ok: true });
    expect(svc.validate(mask(['Deny', 'Permit']))).toEqual({ ok: true });
  });

  it('rejects a mask whose policy has no Permit rule', () => {
    const result = svc.validate(mask(['Deny']));
    expect(result.ok).toBe(false);
    if (!result.ok) {
      expect(result.error).toContain("does not contain a rule with the Effect 'Permit'");
    }
  });
});
