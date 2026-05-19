import { CanActivate, ExecutionContext, Injectable, UnauthorizedException } from '@nestjs/common';
import { jwtVerify } from 'jose';

import { DigitalSignerService } from '../crypto/digital-signer.service';
import { AppConfigService } from '../config/app-config.service';

/**
 * Validates the `Authorization: Bearer <jwt>` access token issued by this
 * Authorization Registry's /connect/token endpoint. The token is signed by
 * our own RSA key, so we verify against the leaf certificate.
 */
@Injectable()
export class JwtBearerGuard implements CanActivate {
  constructor(
    private readonly cfg: AppConfigService,
    private readonly signer: DigitalSignerService,
  ) {}

  async canActivate(context: ExecutionContext): Promise<boolean> {
    const req = context.switchToHttp().getRequest();
    const header = req.headers['authorization'] ?? '';
    if (!header.startsWith('Bearer ')) {
      throw new UnauthorizedException('Missing or malformed Authorization header');
    }
    const token = header.slice('Bearer '.length).trim();
    if (!token) {
      throw new UnauthorizedException('Empty bearer token');
    }

    try {
      const { payload } = await jwtVerify(token, this.signer.getLeafPublicKey(), {
        algorithms: ['RS256'],
        issuer: this.cfg.partyClientId,
      });
      req.user = {
        clientId: payload.sub,
        scope: typeof payload.scope === 'string' ? payload.scope.split(' ') : [],
        token: payload,
      };
      return true;
    } catch (err) {
      throw new UnauthorizedException(`Invalid bearer token: ${(err as Error).message}`);
    }
  }
}
