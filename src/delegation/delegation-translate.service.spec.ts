import { DelegationTranslateService } from './delegation-translate.service';
import { DelegationMask } from './dto/delegation-mask.dto';
import { DelegationEvidence } from './dto/delegation-evidence.dto';

const buildStored = (storedActions: string[], storedIdentifiers: string[]): string =>
  JSON.stringify({
    delegationEvidence: {
      notBefore: 0,
      notOnOrAfter: 9999999999,
      policyIssuer: 'EU.EORI.NL000000001',
      target: { accessSubject: 'EU.EORI.NL000000002' },
      policySets: [
        {
          maxDelegationDepth: 1,
          target: { environment: { licenses: ['ISHARE.0001'] } },
          policies: [
            {
              target: {
                resource: { type: 'CONTAINER', identifiers: storedIdentifiers, attributes: ['*'] },
                actions: storedActions,
              },
              rules: [{ effect: 'Permit' }],
            },
          ],
        },
      ],
    },
  } satisfies { delegationEvidence: DelegationEvidence });

const buildMask = (action: string, identifier: string): DelegationMask => ({
  delegationRequest: {
    policyIssuer: 'EU.EORI.NL000000001',
    target: { accessSubject: 'EU.EORI.NL000000002' },
    policySets: [
      {
        policies: [
          {
            target: {
              resource: { type: 'CONTAINER', identifiers: [identifier], attributes: ['*'] },
              actions: [action],
            },
            rules: [{ effect: 'Permit' }],
          },
        ],
      },
    ],
  },
});

describe('DelegationTranslateService', () => {
  const svc = new DelegationTranslateService();

  it('returns Permit when stored policy covers the requested action and identifier', () => {
    const out = svc.translate(buildMask('READ', 'C1'), buildStored(['READ', 'WRITE'], ['C1']));
    expect(out.delegationEvidence.policySets[0].policies[0].rules[0].effect).toBe('Permit');
  });

  it('returns Deny when the requested action is not in the stored policy', () => {
    const out = svc.translate(buildMask('DELETE', 'C1'), buildStored(['READ'], ['C1']));
    expect(out.delegationEvidence.policySets[0].policies[0].rules[0].effect).toBe('Deny');
  });

  it("treats stored actions=['*'] as wildcard match", () => {
    const out = svc.translate(buildMask('DELETE', 'C1'), buildStored(['*'], ['C1']));
    expect(out.delegationEvidence.policySets[0].policies[0].rules[0].effect).toBe('Permit');
  });

  it("treats stored identifiers=['*'] as wildcard match", () => {
    const out = svc.translate(buildMask('READ', 'C42'), buildStored(['READ'], ['*']));
    expect(out.delegationEvidence.policySets[0].policies[0].rules[0].effect).toBe('Permit');
  });

  it('preserves notBefore/notOnOrAfter and policyIssuer from the stored policy', () => {
    const out = svc.translate(buildMask('READ', 'C1'), buildStored(['READ'], ['C1']));
    expect(out.delegationEvidence.policyIssuer).toBe('EU.EORI.NL000000001');
    expect(out.delegationEvidence.notOnOrAfter).toBe(9999999999);
  });
});
