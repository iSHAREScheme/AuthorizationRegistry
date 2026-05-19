import { Injectable, NotFoundException } from '@nestjs/common';
import { Delegation, Prisma } from '@prisma/client';
import { v4 as uuidv4 } from 'uuid';

import { PrismaService } from '../prisma/prisma.service';
import { DelegationPolicyJsonParser } from './delegation-policy-parser';

export const SCHEME_OWNER_PARTY_ID = 'EU.EORI.NL000000000';

export interface DelegationListQuery {
  partyId: string;
  filter?: string;
  page?: number;
  pageSize?: number;
  sortBy?: 'authorizationRegistryId' | 'policyIssuer' | 'accessSubject';
  sortOrder?: 'asc' | 'desc';
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

const FRIENDLY_ID_PREFIX = 'AR';

/** Delegation persistence and query service backed by Prisma. */
@Injectable()
export class DelegationService {
  constructor(private readonly prisma: PrismaService) {}

  list(query: DelegationListQuery): Promise<PagedResult<Delegation>> {
    const where: Prisma.DelegationWhereInput = {
      deleted: false,
      ...(query.partyId === SCHEME_OWNER_PARTY_ID ? {} : { policyIssuer: query.partyId }),
      ...(query.filter
        ? {
            OR: [
              { authorizationRegistryId: { contains: query.filter, mode: 'insensitive' } },
              { policyIssuer: { contains: query.filter, mode: 'insensitive' } },
              { accessSubject: { contains: query.filter, mode: 'insensitive' } },
            ],
          }
        : {}),
    };

    const sortColumn = query.sortBy ?? 'authorizationRegistryId';
    const orderBy: Prisma.DelegationOrderByWithRelationInput = {
      [sortColumn]: query.sortOrder ?? 'asc',
    };

    const page = Math.max(query.page ?? 1, 1);
    const pageSize = Math.min(Math.max(query.pageSize ?? 25, 1), 200);

    return this.prisma.$transaction(async (tx) => {
      const [items, totalCount] = await Promise.all([
        tx.delegation.findMany({
          where,
          orderBy,
          skip: (page - 1) * pageSize,
          take: pageSize,
        }),
        tx.delegation.count({ where }),
      ]);
      return { items, totalCount, page, pageSize };
    });
  }

  getByPolicyId(arId: string, partyId: string): Promise<Delegation | null> {
    return this.prisma.delegation.findFirst({
      where: {
        authorizationRegistryId: arId,
        deleted: false,
        ...(partyId === SCHEME_OWNER_PARTY_ID ? {} : { policyIssuer: partyId }),
      },
    });
  }

  getBySubject(subject: string, partyId: string): Promise<Delegation | null> {
    return this.prisma.delegation.findFirst({
      where: {
        accessSubject: subject,
        deleted: false,
        ...(partyId === SCHEME_OWNER_PARTY_ID ? {} : { policyIssuer: partyId }),
      },
    });
  }

  exists(partyId: string, subject: string): Promise<boolean> {
    return this.prisma.delegation
      .count({
        where: { policyIssuer: partyId, accessSubject: subject, deleted: false },
      })
      .then((n) => n > 0);
  }

  /**
   * Stores the canonical iSHARE delegation evidence envelope (camelCase JSON).
   */
  async upsertForParty(policyJson: string, actorUserId: string | null): Promise<Delegation> {
    const parsed = new DelegationPolicyJsonParser(policyJson);
    const policyIssuer = required(parsed.policyIssuer, 'delegationEvidence.policyIssuer');
    const accessSubject = required(parsed.accessSubject, 'delegationEvidence.target.accessSubject');

    const existing = await this.prisma.delegation.findFirst({
      where: { policyIssuer, accessSubject, deleted: false },
    });

    if (!existing) {
      return this.prisma.delegation.create({
        data: {
          authorizationRegistryId: await this.generateFriendlyId(),
          policyIssuer,
          accessSubject,
          policy: policyJson,
          createdById: actorUserId,
          updatedById: actorUserId,
        },
      });
    }

    return this.prisma.$transaction(async (tx) => {
      await tx.delegationHistory.create({
        data: {
          delegationId: existing.id,
          policy: existing.policy,
          createdById: actorUserId,
        },
      });
      return tx.delegation.update({
        where: { id: existing.id },
        data: {
          policy: policyJson,
          updatedById: actorUserId,
        },
      });
    });
  }

  async makeInactive(arId: string, actorUserId: string | null, partyId: string): Promise<void> {
    const existing = await this.getByPolicyId(arId, partyId);
    if (!existing) {
      throw new NotFoundException(`Delegation ${arId} not found for party ${partyId}`);
    }
    await this.prisma.$transaction(async (tx) => {
      await tx.delegationHistory.create({
        data: {
          delegationId: existing.id,
          policy: existing.policy,
          createdById: actorUserId,
        },
      });
      await tx.delegation.update({ where: { id: existing.id }, data: { deleted: true } });
    });
  }

  private async generateFriendlyId(): Promise<string> {
    for (let i = 0; i < 5; i++) {
      const candidate = `${FRIENDLY_ID_PREFIX}.${randomFriendly()}`;
      const taken = await this.prisma.delegation.count({
        where: { authorizationRegistryId: candidate },
      });
      if (taken === 0) return candidate;
    }
    return `${FRIENDLY_ID_PREFIX}.${uuidv4().replace(/-/g, '').slice(0, 12).toUpperCase()}`;
  }
}

function randomFriendly(): string {
  // 8-char base32 chunk: short enough to read in URLs, large enough to avoid collisions.
  const chars = 'ABCDEFGHJKMNPQRSTUVWXYZ23456789';
  let out = '';
  for (let i = 0; i < 8; i++) {
    out += chars[Math.floor(Math.random() * chars.length)];
  }
  return out;
}

function required<T>(v: T | undefined, fieldPath: string): T {
  if (v === undefined || v === null || v === '') {
    throw new Error(`Required field missing: ${fieldPath}`);
  }
  return v;
}
