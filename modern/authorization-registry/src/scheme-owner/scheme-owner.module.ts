import { Module } from '@nestjs/common';
import { SchemeOwnerClient } from './scheme-owner.client';

@Module({
  providers: [SchemeOwnerClient],
  exports: [SchemeOwnerClient],
})
export class SchemeOwnerModule {}
