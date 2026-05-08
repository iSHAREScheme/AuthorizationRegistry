import { Module } from '@nestjs/common';
import { PartiesService } from './parties.service';

@Module({
  providers: [PartiesService],
  exports: [PartiesService],
})
export class PartiesModule {}
