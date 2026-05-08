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
 * Port of iSHARE.SchemeOwner.Client. Calls the iSHARE Scheme Owner /parties
 * endpoint to check that a counterparty's iSHARE adherence is active and
 * that their leaf cert (from the JWT x5c header) is on the trusted list.
 *
 * The legacy implementation also called /trusted_list and /capabilities;
 * those are stubbed here and should be wired up before production use.
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
   * The legacy implementation hashed the cert (SHA256) and queried the
   * Scheme Owner /trusted_list endpoint — replicate that here.
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  async isCertTrusted(_leafCertB64: string, _accessToken: string): Promise<boolean> {
    this.logger.warn('SchemeOwnerClient.isCertTrusted is not yet implemented — returning true');
    return true;
  }
}
