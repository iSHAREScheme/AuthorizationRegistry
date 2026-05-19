import { Injectable, Logger } from '@nestjs/common';
import axios, { AxiosInstance } from 'axios';

import { AppConfigService } from '../config/app-config.service';

export type AdherenceStatus = 'Active' | 'NotActive' | 'Pending' | 'Withdrawn';
export type CertificateStatus = 'Valid' | 'Revoked' | 'Expired';

export interface SchemeOwnerParty {
  partyId: string;
  partyName: string;
  adherence: { status: AdherenceStatus; startDate?: string; endDate?: string };
  certificates?: { x5c: string; status: CertificateStatus }[];
  capabilities?: { url: string };
}

/**
 * Calls the iSHARE Scheme Owner /parties endpoint to check that a
 * counterparty's iSHARE adherence is active. Certificate trust validation
 * is stubbed until /trusted_list integration is wired.
 */
@Injectable()
export class SchemeOwnerClient {
  private readonly logger = new Logger(SchemeOwnerClient.name);
  private readonly http: AxiosInstance;

  constructor(private readonly cfg: AppConfigService) {
    this.http = axios.create({
      baseURL: cfg.schemeOwnerBaseUrl,
      timeout: 5_000,
    });
  }

  async getParty(partyId: string, accessToken: string): Promise<SchemeOwnerParty | null> {
    try {
      const { data } = await this.http.get(`/parties/${encodeURIComponent(partyId)}`, {
        headers: { Authorization: `Bearer ${accessToken}` },
      });
      return data?.party_info ?? data ?? null;
    } catch (err) {
      this.logger.warn(
        `Scheme Owner /parties lookup failed for ${partyId}: ${(err as Error).message}`,
      );
      return null;
    }
  }

  /**
   * Stub: validate the JWT's leaf cert against the iSHARE trusted_list.
   * Hash the cert with SHA256 and query the Scheme Owner /trusted_list
   * endpoint before production use.
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  async isCertTrusted(_leafCertB64: string, _accessToken: string): Promise<boolean> {
    this.logger.warn('SchemeOwnerClient.isCertTrusted is not yet implemented; returning true');
    return true;
  }
}
