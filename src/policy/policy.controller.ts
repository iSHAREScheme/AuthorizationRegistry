import {
  All,
  BadRequestException,
  Body,
  Controller,
  HttpCode,
  HttpStatus,
  MethodNotAllowedException,
  Post,
  UseGuards,
  UsePipes,
} from '@nestjs/common';
import { ApiBearerAuth, ApiBody, ApiOperation, ApiTags } from '@nestjs/swagger';

import { CurrentParty, RequestingParty } from '../auth/current-party.decorator';
import { JwtBearerGuard } from '../auth/jwt-bearer.guard';
import { SignResponse } from '../common/decorators/sign-response.decorator';
import { JsonSchemaPipe } from '../common/pipes/json-schema.pipe';
import policySchema from '../delegation/schemas/policy.schema.json';
import { DelegationEvidenceEnvelope } from '../delegation/dto/delegation-evidence.dto';
import { DelegationService } from '../delegation/delegation.service';
import { PolicyValidationService } from './policy-validation.service';

/**
 * Accepts a delegation_evidence-shaped policy and persists it for the caller.
 */
@ApiTags('policy')
@ApiBearerAuth()
@Controller('policy')
@UseGuards(JwtBearerGuard)
export class PolicyController {
  constructor(
    private readonly delegations: DelegationService,
    private readonly validation: PolicyValidationService,
  ) {}

  @Post()
  @HttpCode(HttpStatus.OK)
  @UsePipes(new JsonSchemaPipe(policySchema))
  @SignResponse({ tokenName: 'policy_token', payloadClaim: 'delegationEvidence' })
  @ApiOperation({ summary: 'Create or update a delegation policy' })
  @ApiBody({ description: 'iSHARE delegation_evidence envelope' })
  async upsert(
    @Body() body: DelegationEvidenceEnvelope,
    @CurrentParty() party: RequestingParty,
  ): Promise<unknown> {
    const policyJson = JSON.stringify(body);
    const issuerCheck = this.validation.validateIssuer(
      party.clientId,
      body.delegationEvidence.policyIssuer,
      body.delegationEvidence.target.accessSubject,
    );
    if (!issuerCheck.ok) throw new BadRequestException({ error: issuerCheck.error });

    const stored = await this.delegations.upsertForParty(policyJson, null);
    return JSON.parse(stored.policy).delegationEvidence;
  }

  @All()
  methodNotAllowed(): never {
    throw new MethodNotAllowedException({ error: 'method_not_allowed' });
  }
}
