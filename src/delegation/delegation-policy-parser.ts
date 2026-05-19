/**
 * Used by validation services to introspect a stored policy without
 * inflating the whole DTO graph.
 */
export class DelegationPolicyJsonParser {
  private readonly tree: unknown;

  constructor(json: string) {
    let parsed: unknown;
    try {
      parsed = JSON.parse(json);
    } catch (err) {
      throw new DelegationPolicyFormatError('JSON format is incorrect.', err);
    }
    if (!parsed || typeof parsed !== 'object' || Object.keys(parsed).length === 0) {
      throw new DelegationPolicyFormatError("The delegation policy doesn't have contents.");
    }
    this.tree = parsed;
  }

  get policyIssuer(): string | undefined {
    return this.pick<string>('delegationEvidence.policyIssuer');
  }

  get accessSubject(): string | undefined {
    return this.pick<string>('delegationEvidence.target.accessSubject');
  }

  private pick<T>(path: string): T | undefined {
    let cursor: unknown = this.tree;
    for (const segment of path.split('.')) {
      if (cursor && typeof cursor === 'object' && segment in (cursor as Record<string, unknown>)) {
        cursor = (cursor as Record<string, unknown>)[segment];
      } else {
        return undefined;
      }
    }
    return cursor as T | undefined;
  }
}

export class DelegationPolicyFormatError extends Error {
  constructor(
    message: string,
    readonly cause?: unknown,
  ) {
    super(message);
    this.name = 'DelegationPolicyFormatError';
  }
}
