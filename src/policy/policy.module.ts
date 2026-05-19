import { Module } from '@nestjs/common';

import { AuthModule } from '../auth/auth.module';
import { DelegationModule } from '../delegation/delegation.module';
import { PolicyController } from './policy.controller';
import { PolicyValidationService } from './policy-validation.service';

@Module({
  imports: [AuthModule, DelegationModule],
  controllers: [PolicyController],
  providers: [PolicyValidationService],
})
export class PolicyModule {}
