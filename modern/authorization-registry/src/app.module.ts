import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { LoggerModule } from 'nestjs-pino';
import { TerminusModule } from '@nestjs/terminus';
import { ThrottlerModule, ThrottlerGuard } from '@nestjs/throttler';
import { APP_GUARD } from '@nestjs/core';

import { AppConfigModule } from './config/config.module';
import { PrismaModule } from './prisma/prisma.module';
import { CryptoModule } from './crypto/crypto.module';
import { AuthModule } from './auth/auth.module';
import { DelegationModule } from './delegation/delegation.module';
import { PolicyModule } from './policy/policy.module';
import { TokenModule } from './token/token.module';
import { SchemeOwnerModule } from './scheme-owner/scheme-owner.module';
import { PartiesModule } from './parties/parties.module';
import { UsersModule } from './users/users.module';
import { HealthController } from './common/health.controller';

@Module({
  imports: [
    ConfigModule.forRoot({ isGlobal: true, cache: true }),
    LoggerModule.forRoot({
      pinoHttp: {
        level: process.env.LOG_LEVEL ?? 'info',
        transport:
          process.env.NODE_ENV === 'development' ? { target: 'pino-pretty' } : undefined,
      },
    }),
    ThrottlerModule.forRoot([{ ttl: 60_000, limit: 120 }]),
    TerminusModule,
    AppConfigModule,
    PrismaModule,
    CryptoModule,
    SchemeOwnerModule,
    PartiesModule,
    AuthModule,
    UsersModule,
    DelegationModule,
    PolicyModule,
    TokenModule,
  ],
  controllers: [HealthController],
  providers: [{ provide: APP_GUARD, useClass: ThrottlerGuard }],
})
export class AppModule {}
