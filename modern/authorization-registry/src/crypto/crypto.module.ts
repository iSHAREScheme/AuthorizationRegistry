import { Global, Module } from '@nestjs/common';
import { DigitalSignerService } from './digital-signer.service';
import { ResponseJwtService } from './response-jwt.service';
import { JwtBearerVerifierService } from './jwt-bearer-verifier.service';

@Global()
@Module({
  providers: [DigitalSignerService, ResponseJwtService, JwtBearerVerifierService],
  exports: [DigitalSignerService, ResponseJwtService, JwtBearerVerifierService],
})
export class CryptoModule {}
