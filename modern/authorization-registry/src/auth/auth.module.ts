import { Module } from '@nestjs/common';
import { JwtBearerGuard } from './jwt-bearer.guard';

@Module({
  providers: [JwtBearerGuard],
  exports: [JwtBearerGuard],
})
export class AuthModule {}
