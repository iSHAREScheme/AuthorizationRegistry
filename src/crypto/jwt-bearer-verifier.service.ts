import { Injectable } from '@nestjs/common';
import { decodeProtectedHeader, importX509, jwtVerify, JWTPayload, KeyLike } from 'jose';

/**
 * Verifies an iSHARE-signed JWT (used for both the /token client_assertion and
 * any nested previous_steps tokens). The signing certificate is taken from the
 * JWT's own x5c header. The trust chain check (against the iSHARE Scheme Owner
 * trusted_list) is left to the caller because it requires an HTTP round-trip.
 */
@Injectable()
export class JwtBearerVerifierService {
  /**
   * Verifies a JWT against the leaf certificate it embeds in its x5c header
   * and returns the decoded payload + the leaf cert (base64 DER).
   *
   * Throws if the signature is invalid, the JWT has no x5c, or the algorithm
   * differs from RS256/PS256.
   */
  async verifyWithEmbeddedX5c(token: string): Promise<{
    payload: JWTPayload;
    leafCertB64: string;
    x5c: string[];
  }> {
    const header = decodeProtectedHeader(token);
    if (!header.x5c || header.x5c.length === 0) {
      throw new Error('JWT is missing the x5c header');
    }
    if (header.alg !== 'RS256' && header.alg !== 'PS256') {
      throw new Error(`Unsupported JWT alg: ${String(header.alg)}`);
    }

    const leafCertB64 = header.x5c[0];
    const key = await this.loadX509(leafCertB64);

    const { payload } = await jwtVerify(token, key, {
      algorithms: [header.alg],
    });

    return { payload, leafCertB64, x5c: header.x5c };
  }

  async verifyWithKey(token: string, key: KeyLike): Promise<JWTPayload> {
    const { payload } = await jwtVerify(token, key, { algorithms: ['RS256', 'PS256'] });
    return payload;
  }

  loadX509(certB64: string): Promise<KeyLike> {
    const pem = `-----BEGIN CERTIFICATE-----\n${chunk64(certB64)}\n-----END CERTIFICATE-----\n`;
    return importX509(pem, 'RS256');
  }
}

function chunk64(s: string): string {
  return s.match(/.{1,64}/g)?.join('\n') ?? s;
}
