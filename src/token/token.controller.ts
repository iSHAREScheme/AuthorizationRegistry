import {
  All,
  Body,
  Controller,
  HttpCode,
  HttpStatus,
  MethodNotAllowedException,
  Post,
} from '@nestjs/common';
import { ApiOperation, ApiTags } from '@nestjs/swagger';

import { TokenRequest, TokenService } from './token.service';

/**
 * iSHARE OAuth2 token endpoint. Mounted at the IS4-compatible path
 * `/connect/token` so existing clients work unchanged.
 */
@ApiTags('token')
@Controller('connect')
export class TokenController {
  constructor(private readonly tokens: TokenService) {}

  @Post('token')
  @HttpCode(HttpStatus.OK)
  @ApiOperation({
    summary: 'Issue an iSHARE access token',
    description:
      'OAuth2 client_credentials with grant_type=client_credentials, ' +
      'client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer.',
  })
  async issue(@Body() body: TokenRequest) {
    return this.tokens.issueAccessToken(body);
  }

  @All('token')
  methodNotAllowed(): never {
    throw new MethodNotAllowedException({ error: 'method_not_allowed' });
  }
}
