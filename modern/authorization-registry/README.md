# iSHARE Authorization Registry — TypeScript / NestJS

A modern rewrite of the legacy .NET Core 3.1 implementation that lives at the
root of this repository. The two projects coexist during the migration — the
original C# code stays in place as a reference; this directory is the going-forward
codebase.

## Stack

| Concern              | Legacy (.NET Core 3.1)           | New (TypeScript / NestJS)              |
|----------------------|----------------------------------|----------------------------------------|
| Web framework        | ASP.NET Core MVC                 | NestJS 10 (Express adapter)            |
| OAuth2 / OIDC server | IdentityServer4 (abandoned)      | Custom `/connect/token` + `jose` JWT   |
| ORM                  | Entity Framework Core 3.1        | Prisma 5 (PostgreSQL)                  |
| JWT signing          | `Microsoft.IdentityModel.Tokens` | `jose` (RS256 + x5c chain)             |
| JSON Schema validate | `Newtonsoft.Json.Schema`         | `ajv` 8                                |
| Validation           | DataAnnotations                  | `class-validator` + `zod` (env)        |
| Logging              | `Microsoft.Extensions.Logging`   | `pino` (`nestjs-pino`)                 |
| API docs             | Swashbuckle                      | `@nestjs/swagger`                      |
| Tests                | xUnit + Moq                      | Jest                                   |

## Running locally

Prerequisites: Node 20+, PostgreSQL 14+ (or use the bundled `docker-compose`).

```bash
cp .env.example .env
# Fill in PARTY_CLIENT_ID, DIGITAL_SIGNER_PRIVATE_KEY_PEM, x5c chain, etc.

npm install
npx prisma migrate dev          # creates the schema in your dev DB
npm run start:dev               # API on :8080, Swagger at /swagger
```

Or, with Docker:

```bash
docker compose up --build
```

### Generating signing material

The legacy README uses `openssl pkcs12` to extract a PKCS#1 private key from a
`.p12`. `jose` needs PKCS#8 instead — run this once to convert:

```bash
openssl pkcs12 -in cert.p12 -nocerts -nodes -out leaf.key.pem
openssl pkcs8 -topk8 -in leaf.key.pem -nocrypt -out leaf.key.pkcs8.pem
openssl pkcs12 -in cert.p12 -clcerts -nokeys -out leaf.crt.pem
```

Then export the PEM body of `leaf.crt.pem` (no headers, no line breaks) as
`DIGITAL_SIGNER_CERT_X5C_LEAF`. The PKCS#8 PEM goes verbatim into
`DIGITAL_SIGNER_PRIVATE_KEY_PEM` (replace literal newlines with `\n` if you
must store it on a single line).

## API surface

| Method | Path              | Auth                | Description                                        |
|--------|-------------------|---------------------|----------------------------------------------------|
| POST   | `/connect/token`  | client_assertion JWT | iSHARE OAuth2 token endpoint (`client_credentials`)|
| POST   | `/delegation`     | Bearer JWT          | Translate a `delegation_mask` into signed evidence |
| POST   | `/policy`         | Bearer JWT          | Create or update a delegation policy               |
| GET    | `/capabilities`   | anonymous           | iSHARE capability advertisement                    |
| GET    | `/health`         | anonymous           | Liveness probe                                     |

All non-public-key responses are wrapped in a single signed JWT envelope (the
iSHARE convention), e.g. `{"delegation_token": "<JWT>"}`. The JWT header
embeds the full x5c certificate chain.

## What is and isn't ported (yet)

✅ Done

- Domain types (DelegationEvidence, DelegationMask, Policy, …)
- Translate-mask-to-evidence algorithm (`DelegationTranslateService`)
- Mask permit-rule validation
- `previous_steps` JWT validation (with the future-`iat` fix from `Differences.md`)
- Method-not-allowed (`405`) responses for non-POST/non-GET (also from `Differences.md`)
- RS256 JWT signing + x5c header chain (`DigitalSignerService`, `ResponseJwtService`)
- Bearer token guard for protected endpoints
- OAuth2 `/connect/token` endpoint
- `/delegation`, `/policy`, `/capabilities`
- Prisma schema for `Delegation`, `DelegationHistory`, `User`
- Health check, request logging, Swagger UI
- Unit tests for the translate + mask-validation services

⚠️ Stubbed / partial

- `SchemeOwnerClient.isCertTrusted` — needs a real trusted_list call (or
  hardcoded CA whitelist port from `iSHARE.IdentityServer/CertificatesAuthorities.cs`).
- `PartiesService.assertActiveParty` is wired but not invoked from `/token`
  yet (the legacy `PartyValidator` chain pulled an SO access token first).
- The hardcoded `__bootstrap__` access token in `TokenService.issueAccessToken`
  must be replaced with a real outbound iSHARE assertion before production use.

❌ Not yet ported (see `MIGRATION.md`)

- SPA admin endpoints (`/delegations`, `/users`, `/account/*`) — they belong
  to Phase 3 of the migration and the legacy versions hardcode partyId/userId
  anyway.
- Identity / 2FA / password reset flow (`iSHARE.Identity.Api`) — replace with
  a hosted IdP (Auth0, Keycloak, etc.) per the migration plan.
- Email + SendGrid client (used only for password reset/activation in admin UI).
- Azure Key Vault adapter — replace with Vault, AWS KMS, or Doppler at deploy
  time; the `DigitalSignerService` already takes a PEM string so any secret
  manager that produces one is sufficient.
- Test suite parity — only smoke unit tests so far. The C# project ships a
  golden corpus at `tests/iSHARE.IdentityServer.Tests/DelegationTestCases/*.json`
  that should be wired up as Jest fixtures.

## Project layout

```
src/
  config/        env schema (zod) + AppConfigService
  prisma/        PrismaService (singleton)
  crypto/        DigitalSigner, ResponseJwt builder, JwtBearerVerifier
  auth/          Bearer guard for protected endpoints
  common/        global pipes, interceptors, filters, decorators
  delegation/    /delegation controller + translation + mask validation
  policy/        /policy controller + issuer validation
  token/         /connect/token controller + service
  scheme-owner/  outbound iSHARE Scheme Owner client
  parties/      adherence + cert-trust facade
  users/         /capabilities (placeholder; SPA admin endpoints to come)
prisma/         Prisma schema
test/            E2E suite scaffold
```

## Migration roadmap

See [`MIGRATION.md`](./MIGRATION.md) for the full plan.
