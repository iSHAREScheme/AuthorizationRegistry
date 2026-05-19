# Modernization Plan: TypeScript / NestJS

This document tracks the remaining work after cutting the repository over to
the TypeScript / NestJS Authorization Registry. The service at the repository
root is the only build target.

## Decisions

| Concern | Choice | Rationale |
| --- | --- | --- |
| Language | TypeScript 5 | Matches the current team skill set and Node ecosystem. |
| Framework | NestJS 11 | Controller and dependency-injection model fits the iSHARE API shape. |
| ORM | Prisma 5 | Strong TypeScript DX, migrations, and PostgreSQL support. |
| Database | PostgreSQL 16 | Portable relational store for the delegation schema. |
| OAuth2 endpoint | Hand-rolled `/connect/token` with `jose` | The service only needs `client_credentials` plus JWT bearer assertions. |
| Identity / login | Hosted IdP | Keeps user lifecycle and MFA out of this service. |
| JWT crypto | `jose` 5 | Supports RS256, certificate chains, and modern JWT validation. |
| JSON Schema validation | `ajv` 8 | Reuses the iSHARE wire schemas directly. |
| Logging | `pino` through `nestjs-pino` | Structured JSON logs with low overhead. |
| Tests | Jest | Native NestJS test workflow. |

## Completed Cutover

- [x] Moved the NestJS service to the repository root.
- [x] Removed the previous server projects, solution file, project files, test
  projects, and SPA.
- [x] Kept only the reusable iSHARE delegation JSON fixtures under
  `test/fixtures/delegation/`.
- [x] Added a root-level Prisma schema and Docker Compose development setup.
- [x] Added a baseline Prisma migration, production Dockerfile, readiness
  probe, CI workflow, and test deployment notes.
- [x] Kept the existing `Differences.md` conformance notes as historical
  protocol context.

## Implemented Service Scope

- [x] Domain types for delegation evidence, delegation masks, policies, and
  policy sets.
- [x] `DelegationTranslateService` for mask-to-evidence translation.
- [x] `DelegationMaskValidationService` for Permit-rule validation.
- [x] `DigitalSignerService` and `ResponseJwtService` for RS256 signed
  response envelopes with x5c headers.
- [x] `JwtBearerGuard` for protected endpoints.
- [x] `/connect/token` endpoint for client credentials with JWT bearer
  assertion.
- [x] `/delegation`, `/policy`, `/capabilities`, and `/health` controllers.
- [x] `/health/ready` database readiness probe for deployment platforms.
- [x] Prisma schema for `Delegation`, `DelegationHistory`, and `User`.
- [x] Jest unit tests for translation and mask validation.

## Next Work

- [ ] Replace the temporary `__bootstrap__` access token shortcut in
  `TokenService.issueAccessToken`.
- [ ] Wire `SchemeOwnerClient.getParty` into token issuance and cache
  party lookups for the configured TTL.
- [ ] Implement `SchemeOwnerClient.isCertTrusted` against the Scheme Owner
  `/trusted_list` endpoint.
- [ ] Add outbound iSHARE assertion generation so this service can request
  its own Scheme Owner access token.
- [ ] Run the iSHARE Conformance Test Tool and turn deviations into Jest
  fixtures.
- [ ] Convert `test/fixtures/delegation/*.json` into Jest fixture-driven
  coverage for `DelegationTranslateService`.
- [ ] Add admin CRUD endpoints for delegations and users.
- [ ] Build or connect a new admin UI backed by a hosted IdP.

## Risk Register

| Risk | Mitigation |
| --- | --- |
| Behaviour drift in delegation translation | Drive Jest tests from the JSON fixture corpus before release. |
| Subtle JWT differences | Use the iSHARE Conformance Test Tool as a release gate. |
| RSA key format mismatch | `DigitalSignerService` rejects PKCS#1 keys with an actionable conversion command. |
| Database cutover | Write a one-off ETL from the previous database shape into the Prisma schema. |
| Missing identity features | Keep this service protocol-focused and delegate login, MFA, and password recovery to a hosted IdP. |
