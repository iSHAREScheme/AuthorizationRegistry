import { SetMetadata, UseInterceptors, applyDecorators } from '@nestjs/common';
import {
  SIGN_RESPONSE_KEY,
  SignResponseInterceptor,
  SignResponseOptions,
} from '../interceptors/sign-response.interceptor';

/**
 * Replaces [SignResponse(...)] from the .NET implementation. Apply to controller
 * routes that must return iSHARE-signed JWTs.
 */
export const SignResponse = (opts: SignResponseOptions) =>
  applyDecorators(SetMetadata(SIGN_RESPONSE_KEY, opts), UseInterceptors(SignResponseInterceptor));
