import { Module } from '@nestjs/common';

import { CapabilitiesController } from './capabilities.controller';

/**
 * Placeholder users/capabilities module. The full SPA admin endpoints
 * (DelegationsController, UsersController, AccountController) are part of
 * the remaining modernization roadmap in MIGRATION.md.
 */
@Module({
  controllers: [CapabilitiesController],
})
export class UsersModule {}
