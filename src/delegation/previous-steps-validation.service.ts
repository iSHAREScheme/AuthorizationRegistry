import { Injectable } from '@nestjs/common';

import { JwtBearerVerifierService } from '../crypto/jwt-bearer-verifier.service';
import { DelegationMask } from './dto/delegation-mask.dto';

export type PreviousStepsResult = { ok: true } | { ok: false; errors: string[] };

/**
 * `previous_steps` carries a chain of signed JWTs proving each upstream
 * delegation hop. Each JWT must be valid (RS256 + x5c) and its `iat` must
 * not be in the future; see Differences.md for the conformance note.
 */
@Injectable()
export class PreviousStepsValidationService {
  constructor(private readonly verifier: JwtBearerVerifierService) {}

  async validate(mask: DelegationMask): Promise<PreviousStepsResult> {
    const steps = mask.previous_steps ?? [];
    if (steps.length === 0) return { ok: true };

    const errors: string[] = [];
    const now = Math.floor(Date.now() / 1000);

    for (let i = 0; i < steps.length; i++) {
      try {
        const { payload } = await this.verifier.verifyWithEmbeddedX5c(steps[i]);
        if (typeof payload.iat !== 'number') {
          errors.push(`previous_steps[${i}] missing iat`);
          continue;
        }
        if (payload.iat > now + 5) {
          errors.push(`previous_steps[${i}] iat is in the future`);
        }
        if (typeof payload.exp === 'number' && payload.exp < now) {
          errors.push(`previous_steps[${i}] is expired`);
        }
      } catch (err) {
        errors.push(`previous_steps[${i}] invalid: ${(err as Error).message}`);
      }
    }

    return errors.length === 0 ? { ok: true } : { ok: false, errors };
  }
}
