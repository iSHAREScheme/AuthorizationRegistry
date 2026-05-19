import { Module } from '@nestjs/common';

import { AuthModule } from '../auth/auth.module';
import { DelegationController } from './delegation.controller';
import { DelegationMaskValidationService } from './delegation-mask-validation.service';
import { DelegationService } from './delegation.service';
import { DelegationTranslateService } from './delegation-translate.service';
import { PreviousStepsValidationService } from './previous-steps-validation.service';

@Module({
  imports: [AuthModule],
  controllers: [DelegationController],
  providers: [
    DelegationService,
    DelegationTranslateService,
    DelegationMaskValidationService,
    PreviousStepsValidationService,
  ],
  exports: [DelegationService, DelegationTranslateService],
})
export class DelegationModule {}
