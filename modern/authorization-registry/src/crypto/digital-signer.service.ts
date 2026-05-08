import { Injectable, OnModuleInit } from '@nestjs/common';
import { importPKCS8, importX509, KeyLike } from 'jose';

import { AppConfigService } from '../config/app-config.service';

/**
 * Loads and caches the RSA private key used to sign JWTs and the leaf
 * certificate used to verify our own signatures. Replaces the legacy
 * iSHARE.IdentityServer.Services.DigitalSigner + HardcodedPrivateKeyVault.
 */
@Injectable()
export class DigitalSignerService implements OnModuleInit {
  private privateKey!: KeyLike;
  private leafPublicKey!: KeyLike;

  constructor(private readonly cfg: AppConfigService) {}

  async onModuleInit(): Promise<void> {
    this.privateKey = await loadPrivateKey(this.cfg.privateKeyPem);
    this.leafPublicKey = await loadPublicKeyFromBase64Cert(this.cfg.x5cChain[0]);
  }

  getPrivateKey(): KeyLike {
    return this.privateKey;
  }

  getLeafPublicKey(): KeyLike {
    return this.leafPublicKey;
  }

  /** PEM-less, base64-encoded DER certificates — leaf first. */
  getX5cChain(): string[] {
    return this.cfg.x5cChain;
  }
}

async function loadPrivateKey(pem: string): Promise<KeyLike> {
  // jose supports both PKCS#8 and PKCS#1 once normalised to PEM-PKCS8.
  // The legacy app stored PKCS#1 ("BEGIN RSA PRIVATE KEY"); jose@5 only
  // imports PKCS#8 directly, so callers should provide PKCS#8 PEMs (the
  // README documents the openssl conversion). Attempting both gives a
  // friendlier error message if someone supplies the wrong format.
  if (pem.includes('BEGIN RSA PRIVATE KEY')) {
    throw new Error(
      'DIGITAL_SIGNER_PRIVATE_KEY_PEM is in PKCS#1 format. ' +
        'Convert it once with: openssl pkcs8 -topk8 -in old.pem -out new.pem -nocrypt',
    );
  }
  return importPKCS8(pem, 'RS256');
}

async function loadPublicKeyFromBase64Cert(b64: string): Promise<KeyLike> {
  const pem = `-----BEGIN CERTIFICATE-----\n${chunk64(b64)}\n-----END CERTIFICATE-----\n`;
  return importX509(pem, 'RS256');
}

function chunk64(s: string): string {
  return s.match(/.{1,64}/g)?.join('\n') ?? s;
}
