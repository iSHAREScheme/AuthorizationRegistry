# iSHARE Authorization Registry

TypeScript / NestJS implementation of the iSHARE Authorization Registry.
This branch is now a single Node service at the repository root.

## Stack

| Concern | Current choice |
| --- | --- |
| Web framework | NestJS 11 with the Express adapter |
| OAuth2 token endpoint | Custom `/connect/token` using `jose` |
| Persistence | Prisma 5 with PostgreSQL |
| JWT signing | `jose` with RS256 and x5c headers |
| JSON Schema validation | `ajv` 8 |
| Request validation | `class-validator` and `zod` |
| Logging | `pino` through `nestjs-pino` |
| API docs | `@nestjs/swagger` |
| Tests | Jest |

## Running Locally

Prerequisites:

- Node.js 20.11 or newer
- PostgreSQL 14 or newer, unless you use the bundled Docker Compose setup

```bash
cp .env.example .env
# Fill in PARTY_CLIENT_ID, DIGITAL_SIGNER_PRIVATE_KEY_PEM, x5c chain, etc.

npm install
npx prisma migrate dev
npm run start:dev
```

The API listens on `http://localhost:8080`. Swagger is available at
`http://localhost:8080/swagger`.

With Docker:

```bash
docker compose up --build
```

The container runs `prisma migrate deploy` before starting the API. Use
`/health/ready` as the readiness probe because it verifies database
connectivity.

## Release Gate

```bash
npm ci
npm run check
docker build -t ishare-authorization-registry:modernize .
```

The GitHub Actions workflow runs the same Node checks, applies the Prisma
migration against PostgreSQL 16, and fails on production dependency audit
findings.

## Signing Material

The service expects a PKCS#8 private key and a PEM-less x5c certificate chain.
If your certificate bundle contains a PKCS#1 private key, convert it once:

```bash
openssl pkcs12 -in cert.p12 -nocerts -nodes -out leaf.key.pem
openssl pkcs8 -topk8 -in leaf.key.pem -nocrypt -out leaf.key.pkcs8.pem
openssl pkcs12 -in cert.p12 -clcerts -nokeys -out leaf.crt.pem
```

Use the PKCS#8 PEM as `DIGITAL_SIGNER_PRIVATE_KEY_PEM`. Export the certificate
body without headers or line breaks as `DIGITAL_SIGNER_CERT_X5C_LEAF`.

## API Surface

| Method | Path | Auth | Description |
| --- | --- | --- | --- |
| POST | `/connect/token` | client assertion JWT | iSHARE OAuth2 token endpoint |
| POST | `/delegation` | Bearer JWT | Translate a `delegation_mask` into signed evidence |
| POST | `/policy` | Bearer JWT | Create or update a delegation policy |
| GET | `/capabilities` | Anonymous | iSHARE capability advertisement |
| GET | `/health` | Anonymous | Liveness probe |
| GET | `/health/ready` | Anonymous | Database readiness probe |

Signed responses use the iSHARE JWT envelope convention, for example:

```json
{ "delegation_token": "<JWT>" }
```

## Current Status

Implemented:

- Core delegation evidence, delegation mask, policy, and policy set types
- Mask-to-evidence translation
- Permit-rule validation
- `previous_steps` JWT validation, including future `iat` rejection
- Method-not-allowed responses for iSHARE endpoints
- RS256 JWT signing with x5c headers
- Bearer token guard for protected endpoints
- `/connect/token`, `/delegation`, `/policy`, `/capabilities`, and `/health`
- Prisma schema for `Delegation`, `DelegationHistory`, and `User`
- Baseline Prisma migration and deploy-time migration command
- Docker production image, Docker Compose test stack, and CI workflow
- Health checks, request logging, Swagger UI, and Jest unit tests

Still pending:

- Real Scheme Owner `/trusted_list` certificate validation
- Replacing the temporary `__bootstrap__` token shortcut in `TokenService`
- Full Scheme Owner token acquisition for outbound iSHARE calls
- Admin UI and admin CRUD endpoints
- Wider Jest fixture coverage using `test/fixtures/delegation/*.json`

## Project Layout

```text
src/
  auth/          Bearer guard and current-party decorator
  common/        filters, interceptors, pipes, health endpoint
  config/        zod env schema and AppConfigService
  crypto/        DigitalSigner, ResponseJwt builder, JwtBearerVerifier
  delegation/    /delegation controller, translation, validation, schemas
  parties/       adherence and certificate-trust facade
  policy/        /policy controller and issuer validation
  prisma/        PrismaService
  scheme-owner/  outbound Scheme Owner client
  token/         /connect/token controller and service
  users/         /capabilities controller
prisma/          Prisma schema
deploy/          Test deployment notes and platform example
test/fixtures/   iSHARE delegation JSON fixtures
```

See [MIGRATION.md](./MIGRATION.md) for the remaining modernization work.
