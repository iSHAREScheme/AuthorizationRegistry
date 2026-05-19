import {
  All,
  Controller,
  Get,
  HttpCode,
  HttpStatus,
  MethodNotAllowedException,
} from '@nestjs/common';
import { ApiOperation, ApiTags } from '@nestjs/swagger';

import { SignResponse } from '../common/decorators/sign-response.decorator';
import { AppConfigService } from '../config/app-config.service';

interface CapabilitiesInfo {
  party_id: string;
  ishare_roles: { role: string }[];
  supported_versions: { version: string; supported_features: { public: unknown[] }[] }[];
}

/**
 * Anonymous endpoint: anyone may discover what this AR supports. Response is signed.
 */
@ApiTags('capabilities')
@Controller('capabilities')
export class CapabilitiesController {
  constructor(private readonly cfg: AppConfigService) {}

  @Get()
  @HttpCode(HttpStatus.OK)
  @SignResponse({
    tokenName: 'capabilities_token',
    payloadClaim: 'capabilities_info',
    anonymousUsage: true,
  })
  @ApiOperation({ summary: 'Public iSHARE capabilities advertisement' })
  capabilities(): CapabilitiesInfo {
    return {
      party_id: this.cfg.partyClientId,
      ishare_roles: [{ role: 'AuthorizationRegistry' }],
      supported_versions: [
        {
          version: '1.10',
          supported_features: [
            {
              public: [
                {
                  capabilities: { id: 'capabilities', feature: 'capabilities' },
                  delegation: { id: 'delegation', feature: 'delegation' },
                  policy: { id: 'policy', feature: 'policy' },
                },
              ],
            },
          ],
        },
      ],
    };
  }

  @All()
  methodNotAllowed(): never {
    throw new MethodNotAllowedException({ error: 'method_not_allowed' });
  }
}
