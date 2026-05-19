import { Injectable } from '@nestjs/common';
import { Env, loadEnv } from './env.schema';

@Injectable()
export class AppConfigService {
  private readonly env: Env & { x5cChain: string[] };

  constructor() {
    this.env = loadEnv(process.env);
  }

  get nodeEnv() {
    return this.env.NODE_ENV;
  }
  get port() {
    return this.env.PORT;
  }
  get isProduction() {
    return this.env.NODE_ENV === 'production' || this.env.NODE_ENV === 'staging';
  }

  get partyClientId() {
    return this.env.PARTY_CLIENT_ID;
  }
  get partyName() {
    return this.env.PARTY_NAME;
  }

  get privateKeyPem() {
    return this.env.DIGITAL_SIGNER_PRIVATE_KEY_PEM;
  }
  /** Full x5c chain, leaf first, then intermediates/root. */
  get x5cChain(): string[] {
    return [this.env.DIGITAL_SIGNER_CERT_X5C_LEAF, ...this.env.x5cChain];
  }

  get schemeOwnerBaseUrl() {
    return this.env.SCHEME_OWNER_BASE_URL;
  }
  get schemeOwnerClientId() {
    return this.env.SCHEME_OWNER_CLIENT_ID;
  }
  get schemeOwnerPublicKey() {
    return this.env.SCHEME_OWNER_PUBLIC_KEY;
  }

  get accessTokenTtlSeconds() {
    return this.env.ACCESS_TOKEN_TTL_SECONDS;
  }
  get responseJwtTtlSeconds() {
    return this.env.RESPONSE_JWT_TTL_SECONDS;
  }
  get allowDoNotSignHeader() {
    return this.env.ALLOW_DO_NOT_SIGN_HEADER;
  }
}
