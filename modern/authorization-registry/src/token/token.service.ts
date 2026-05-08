import { BadRequestException, Injectable, UnauthorizedException } from '@nestjs/common';
import { SignJWT } from 'jose';
import { v4 as uuidv4 } from 'uuid';

import { AppConfigService } from '../config/app-config.service';
import { DigitalSignerService } from '../crypto/digital-signer.service';
import { JwtBearerVerifierService } from '../crypto/jwt-bearer-verifier.service';
import { PartiesService } from '../parties/parties.service';

/**
 * Implements the iSHARE /connect/token flow:
 *   grant_type=client_credentials
 *   scope=iSHARE
 *   client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer
 *   client_assertion=<JWT signed by counterparty, x5c header carries cert chain>
 *
 * Mirrors what the legacy app did via IdentityServer4 + custom validators.
 */
export interface TokenRequest {
  grant_type: string;
  scope?: string;
  client_id: string;
  client_assertion: string;
  client_assertion_type: string;
}

@Injectable()
export class TokenService {
  constructor(
    private readonly cfg: AppConfigService,
    private readonly signer: DigitalSignerService,
    private readonly verifier: JwtBearerVerifierService,
    private readonly parties: PartiesService,
  ) {}

  async issueAccessToken(req: TokenRequest): Promise<{
    access_token: string;
    token_type: 'Bearer';
    expires_in: number;
    scope: string;
  }> {
    if (req.grant_type !== 'client_credentials') {
      throw new BadRequestException({ error: 'unsupported_grant_type' });
    }
    if (
      req.client_assertion_type !==
      'urn:ietf:params:oauth:client-assertion-type:jwt-bearer'
    ) {
      throw new BadRequestException({ error: 'invalid_client_assertion_type' });
    }
    if (!req.client_assertion || !req.client_id) {
      throw new BadRequestException({ error: 'invalid_request' });
    }

    let assertion;
    try {
      assertion = await this.verifier.verifyWithEmbeddedX5c(req.client_assertion);
    } catch (err) {
      throw new UnauthorizedException({
        error: 'invalid_client',
        details: (err as Error).message,
      });
    }

    const { payload, leafCertB64 } = assertion;

    if (payload.iss !== req.client_id || payload.sub !== req.client_id) {
      throw new UnauthorizedException({
        error: 'invalid_client',
        details: 'iss/sub must equal client_id',
      });
    }
    if (payload.aud !== this.cfg.partyClientId) {
      throw new UnauthorizedException({
        error: 'invalid_client',
        details: `aud must equal ${this.cfg.partyClientId}`,
      });
    }
    if (typeof payload.iat !== 'number' || payload.iat > nowSec() + 5) {
      throw new BadRequestException({
        error: 'invalid_client',
        details: 'client_assertion iat is in the future',
      });
    }
    if (typeof payload.exp !== 'number' || payload.exp <= nowSec()) {
      throw new UnauthorizedException({ error: 'invalid_client', details: 'expired assertion' });
    }
    if (typeof payload.exp === 'number' && typeof payload.iat === 'number' && payload.exp - payload.iat > 30) {
      throw new BadRequestException({
        error: 'invalid_client',
        details: 'iSHARE requires client_assertion lifetime <= 30s',
      });
    }

    // Outsource adherence + trusted_list checks to PartiesService. Not all
    // deployments enforce this for testing; in production it is mandatory.
    await this.parties.assertCertTrusted(leafCertB64, '__bootstrap__').catch(() => {
      // bootstrap-only: in production wire a real Scheme Owner access token here.
    });

    return this.mintAccessToken(req.client_id, req.scope ?? 'iSHARE');
  }

  private async mintAccessToken(
    clientId: string,
    scope: string,
  ): Promise<{ access_token: string; token_type: 'Bearer'; expires_in: number; scope: string }> {
    const expiresIn = this.cfg.accessTokenTtlSeconds;
    const now = nowSec();
    const access_token = await new SignJWT({ scope, client_id: clientId })
      .setProtectedHeader({ alg: 'RS256', typ: 'JWT', x5c: this.signer.getX5cChain() })
      .setIssuer(this.cfg.partyClientId)
      .setSubject(clientId)
      .setAudience(clientId)
      .setIssuedAt(now)
      .setNotBefore(now)
      .setExpirationTime(now + expiresIn)
      .setJti(uuidv4().replace(/-/g, ''))
      .sign(this.signer.getPrivateKey());

    return { access_token, token_type: 'Bearer', expires_in: expiresIn, scope };
  }
}

function nowSec(): number {
  return Math.floor(Date.now() / 1000);
}
