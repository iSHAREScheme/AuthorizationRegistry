import { Module } from '@nestjs/common';

import { PartiesModule } from '../parties/parties.module';
import { TokenController } from './token.controller';
import { TokenService } from './token.service';

@Module({
  imports: [PartiesModule],
  controllers: [TokenController],
  providers: [TokenService],
})
export class TokenModule {}
