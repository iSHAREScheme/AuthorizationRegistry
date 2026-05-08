import {
  CallHandler,
  ExecutionContext,
  Injectable,
  Logger,
  NestInterceptor,
} from '@nestjs/common';
import { Reflector } from '@nestjs/core';
import { Observable } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

import { AppConfigService } from '../../config/app-config.service';
import { ResponseJwtService } from '../../crypto/response-jwt.service';
import { DelegationEvidence } from '../../delegation/dto/delegation-evidence.dto';

export interface SignResponseOptions {
  /** Top-level wrapper claim e.g. "delegation_token", "policy_token". */
  tokenName: string;
  /** Inner claim that holds the response object e.g. "delegationEvidence". */
  payloadClaim: string;
  /** Set true for endpoints that may be called unauthenticated (skip aud claim). */
  anonymousUsage?: boolean;
}

export const SIGN_RESPONSE_KEY = 'iSHARE:signResponse';

/**
 * Wraps the response body in `{ [tokenName]: <signed JWT> }` — port of
 * iSHARE.Api.Filters.SignResponseAttribute. Honours a `Do-Not-Sign: true`
 * request header in non-production environments to ease local debugging.
 */
@Injectable()
export class SignResponseInterceptor implements NestInterceptor {
  private readonly logger = new Logger(SignResponseInterceptor.name);

  constructor(
    private readonly reflector: Reflector,
    private readonly cfg: AppConfigService,
    private readonly jwt: ResponseJwtService,
  ) {}

  intercept(context: ExecutionContext, next: CallHandler): Observable<unknown> {
    const opts = this.reflector.get<SignResponseOptions>(
      SIGN_RESPONSE_KEY,
      context.getHandler(),
    );
    if (!opts) {
      return next.handle();
    }

    const req = context.switchToHttp().getRequest();
    const skipHeader =
      this.cfg.allowDoNotSignHeader &&
      String(req.headers['do-not-sign'] ?? '').toLowerCase() === 'true';

    if (skipHeader && !this.cfg.isProduction) {
      this.logger.debug('Bypassing response signing (Do-Not-Sign header)');
      return next.handle();
    }

    return next.handle().pipe(
      mergeMap(async (body: unknown) => {
        if (body == null) return body;

        const subject = pickSubject(body, this.cfg.partyClientId);
        const audience = opts.anonymousUsage
          ? undefined
          : (req.user?.clientId as string | undefined);

        const jwt = await this.jwt.create({
          payload: body,
          payloadClaim: opts.payloadClaim,
          subject,
          audience,
        });
        return { [opts.tokenName]: jwt };
      }),
    );
  }
}

function pickSubject(body: unknown, fallback: string): string {
  if (
    body &&
    typeof body === 'object' &&
    'target' in body &&
    typeof (body as DelegationEvidence).target?.accessSubject === 'string'
  ) {
    return (body as DelegationEvidence).target.accessSubject;
  }
  return fallback;
}
