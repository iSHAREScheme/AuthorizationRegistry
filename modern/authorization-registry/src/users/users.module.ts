import { Module } from '@nestjs/common';

import { CapabilitiesController } from './capabilities.controller';

/**
 * Placeholder users/capabilities module. The full SPA admin endpoints
 * (DelegationsController, UsersController, AccountController) are part of
 * Phase 3 of the migration roadmap — see modern/MIGRATION.md.
 */
@Module({
  controllers: [CapabilitiesController],
})
export class UsersModule {}
