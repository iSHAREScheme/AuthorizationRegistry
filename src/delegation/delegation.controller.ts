import {
  All,
  Body,
  Controller,
  HttpCode,
  HttpStatus,
  MethodNotAllowedException,
  NotFoundException,
  Post,
  UnauthorizedException,
  UseGuards,
  UsePipes,
} from '@nestjs/common';
import { ApiBearerAuth, ApiBody, ApiOperation, ApiTags } from '@nestjs/swagger';

import { CurrentParty, RequestingParty } from '../auth/current-party.decorator';
import { JwtBearerGuard } from '../auth/jwt-bearer.guard';
import { SignResponse } from '../common/decorators/sign-response.decorator';
import { JsonSchemaPipe } from '../common/pipes/json-schema.pipe';
import delegationMaskSchema from './schemas/delegation-mask.schema.json';
import { DelegationMaskValidationService } from './delegation-mask-validation.service';
import { DelegationService } from './delegation.service';
import { DelegationTranslateService } from './delegation-translate.service';
import { PreviousStepsValidationService } from './previous-steps-validation.service';
import { DelegationMask } from './dto/delegation-mask.dto';

@ApiTags('delegation')
@ApiBearerAuth()
@Controller('delegation')
@UseGuards(JwtBearerGuard)
export class DelegationController {
  constructor(
    private readonly delegations: DelegationService,
    private readonly translate: DelegationTranslateService,
    private readonly maskValidation: DelegationMaskValidationService,
    private readonly previousSteps: PreviousStepsValidationService,
  ) {}

  @Post()
  @HttpCode(HttpStatus.OK)
  @UsePipes(new JsonSchemaPipe(delegationMaskSchema))
  @SignResponse({ tokenName: 'delegation_token', payloadClaim: 'delegationEvidence' })
  @ApiOperation({
    summary: 'Obtain delegation evidence',
    description:
      'Used to obtain delegation evidence from an Authorization Registry. ' +
      'The response is a signed JSON Web Token (RS256 + x5c).',
  })
  @ApiBody({ description: 'iSHARE delegation_mask' })
  async translateMask(
    @Body() mask: DelegationMask,
    @CurrentParty() _party: RequestingParty,
  ): Promise<unknown> {
    const stepsResult = await this.previousSteps.validate(mask);
    if (!stepsResult.ok) {
      throw new UnauthorizedException({
        error: 'invalid_request',
        details: stepsResult.errors,
      });
    }

    const maskResult = this.maskValidation.validate(mask);
    if (!maskResult.ok) {
      throw new UnauthorizedException({ error: 'invalid_request', details: maskResult.error });
    }

    const stored = await this.delegations.getBySubject(
      mask.delegationRequest.target.accessSubject,
      mask.delegationRequest.policyIssuer,
    );
    if (!stored) {
      throw new NotFoundException({ error: 'delegation_not_found' });
    }

    const envelope = this.translate.translate(mask, stored.policy);
    return envelope.delegationEvidence;
  }

  /**
   * Differences.md fix: return 405 (not 404) for non-POST methods.
   */
  @All()
  methodNotAllowed(): never {
    throw new MethodNotAllowedException({ error: 'method_not_allowed' });
  }
}
