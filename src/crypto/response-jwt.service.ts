import { Injectable } from '@nestjs/common';
import { SignJWT } from 'jose';
import { v4 as uuidv4 } from 'uuid';

import { AppConfigService } from '../config/app-config.service';
import { DigitalSignerService } from './digital-signer.service';

/**
 * Builds a signed iSHARE response JWT (RS256 + x5c header chain).
 * The payload object is wrapped in the iSHARE convention: a single named
 * claim such as `delegationEvidence` holding the original response body.
 */
@Injectable()
export class ResponseJwtService {
  constructor(
    private readonly cfg: AppConfigService,
    private readonly signer: DigitalSignerService,
  ) {}

  async create<T>(args: {
    payload: T;
    payloadClaim: string;
    subject: string;
    audience?: string;
  }): Promise<string> {
    const now = Math.floor(Date.now() / 1000);
    const exp = now + this.cfg.responseJwtTtlSeconds;

    const builder = new SignJWT({ [args.payloadClaim]: args.payload })
      .setProtectedHeader({
        alg: 'RS256',
        typ: 'JWT',
        x5c: this.signer.getX5cChain(),
      })
      .setIssuer(this.cfg.partyClientId)
      .setSubject(args.subject)
      .setIssuedAt(now)
      .setNotBefore(now)
      .setExpirationTime(exp)
      .setJti(uuidv4().replace(/-/g, ''));

    if (args.audience) {
      builder.setAudience(args.audience);
    }

    return builder.sign(this.signer.getPrivateKey());
  }
}
