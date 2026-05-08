import { createParamDecorator, ExecutionContext } from '@nestjs/common';

export interface RequestingParty {
  clientId: string;
  scope: string[];
}

/** Convenience decorator: `@CurrentParty() party: RequestingParty`. */
export const CurrentParty = createParamDecorator(
  (_data: unknown, ctx: ExecutionContext): RequestingParty => {
    const req = ctx.switchToHttp().getRequest();
    return req.user as RequestingParty;
  },
);
