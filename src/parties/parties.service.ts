import { Injectable } from '@nestjs/common';
import { SchemeOwnerClient, SchemeOwnerParty } from '../scheme-owner/scheme-owner.client';

/**
 * Caches Scheme Owner party lookups for the lifetime of one request batch.
 * Answers: is this client an active iSHARE adherent, and is the JWT
 * certificate trusted by the Scheme Owner?
 */
@Injectable()
export class PartiesService {
  constructor(private readonly schemeOwner: SchemeOwnerClient) {}

  async assertActiveParty(
    clientId: string,
    accessTokenForSchemeOwner: string,
  ): Promise<SchemeOwnerParty> {
    const party = await this.schemeOwner.getParty(clientId, accessTokenForSchemeOwner);
    if (!party) {
      throw new Error(`Party ${clientId} not found at iSHARE Scheme Owner`);
    }
    if (party.adherence.status !== 'Active') {
      throw new Error(`Party ${clientId} is not Active (status=${party.adherence.status})`);
    }
    return party;
  }

  async assertCertTrusted(leafCertB64: string, accessToken: string): Promise<void> {
    const trusted = await this.schemeOwner.isCertTrusted(leafCertB64, accessToken);
    if (!trusted) {
      throw new Error('Counterparty certificate is not on the iSHARE trusted_list');
    }
  }
}
