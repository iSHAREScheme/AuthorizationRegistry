# Migration plan: .NET Core 3.1 → TypeScript / NestJS

This document tracks the staged migration of the iSHARE Authorization
Registry from the legacy .NET stack to NestJS. The legacy code remains in
the repository root and continues to work during the migration; the new
codebase under `modern/authorization-registry/` is the going-forward target.

## Why we're rewriting

- **.NET Core 3.1 is end-of-life** (December 2022). Every NuGet upgrade,
  CVE patch, or hosting decision is gated on a runtime that no longer
  receives security updates.
- **IdentityServer4 was abandoned** and re-licensed commercially as Duende.
  Staying on the OSS fork means no new security work.
- The current repository is composed of **20+ tightly-coupled C# projects**
  with circular-feeling dependencies that make incremental refactoring
  expensive.
- The team no longer has C# expertise; TypeScript is the lingua franca.

## Stack decisions

| Concern               | Choice                  | Rationale |
|-----------------------|-------------------------|-----------|
| Language              | TypeScript 5            | Team consensus; type-safety on top of JS ecosystem. |
| Framework             | NestJS 10               | Decorator-driven controllers + DI map cleanly from ASP.NET MVC. |
| ORM                   | Prisma 5                | Best DX in TS; first-class migrations; supports Postgres + MSSQL. |
| Database              | PostgreSQL 16           | Drop the SQL Server dependency. `Delegation` schema is portable. |
| OAuth2 endpoint       | Hand-rolled `/connect/token` with `jose` | Avoids re-introducing IS4-equivalent baggage. We only need `client_credentials` + `urn:ietf:params:oauth:client-assertion-type:jwt-bearer`. |
| Identity / login      | Defer to hosted IdP (Auth0, Keycloak, Azure AD B2C) | Removes the entire `iSHARE.Identity*` family of projects. |
| JWT crypto            | `jose` 5                | Modern, widely-used, supports RS256 / PS256 / x5c. |
| JSON Schema validation| `ajv` 8                 | Re-uses the unmodified iSHARE schemas. |
| Logging               | `pino` (`nestjs-pino`)  | Structured JSON logs, low overhead. |
| Tests                 | Jest                    | NestJS default; mature TS support. |

## Phases

### Phase 1 — Core iSHARE protocol *(this PR)*

Goal: the new service can act as an iSHARE Authorization Registry for the
core delegation flow, end-to-end, without the SPA or identity admin.

- [x] NestJS project scaffold (`package.json`, `tsconfig`, `nest-cli`, ESLint).
- [x] Prisma schema mirroring `Delegation` / `DelegationHistory` / `User`.
- [x] Domain types ported as TS interfaces (`DelegationEvidence`,
  `DelegationMask`, `Policy`, …) — keeps the iSHARE wire shapes verbatim.
- [x] `DelegationTranslateService` — byte-equivalent port of the legacy
  algorithm, including wildcard matching and reverse-Deny-rule overrides.
- [x] `DelegationMaskValidationService` — the "every policy must have a
  Permit rule" check.
- [x] `DigitalSignerService` + `ResponseJwtService` — RS256 + x5c chain.
- [x] `JwtBearerGuard` — validates inbound bearer tokens.
- [x] `/connect/token` endpoint (client_credentials with JWT bearer assertion).
- [x] `/delegation`, `/policy`, `/capabilities`, `/health` controllers.
- [x] Three `Differences.md` fixes:
  - Reject future `iat` on client assertions and `previous_steps` JWTs (400 vs 200).
  - 405 instead of 404 for non-POST/non-GET on iSHARE endpoints.
  - 400 instead of 401 for missing/non-Bearer Authorization on `/delegation`
    *(handled by the guard's specific error message — pending verification
    against the iSHARE Conformance Test Tool).*
- [x] Unit tests for translate + mask validation.

### Phase 2 — iSHARE Scheme Owner integration *(next)*

- [ ] Wire `SchemeOwnerClient.getParty` into `TokenService.issueAccessToken`
  (replace the `__bootstrap__` shortcut). Cache responses per-party for the
  configured TTL.
- [ ] Implement `SchemeOwnerClient.isCertTrusted` against the Scheme Owner
  `/trusted_list` endpoint (returns the SHA256 fingerprints of trusted
  intermediates).
- [ ] Port `iSHARE.IdentityServer/CertificatesAuthorities.cs` as a fallback
  hardcoded CA whitelist for offline / preprod environments.
- [ ] Re-implement the iSHARE outbound assertion service so this AR can
  request its own access token from the Scheme Owner.
- [ ] Run the iSHARE Conformance Test Tool against the new service; track
  any deviations as test fixtures so future regressions fail loudly.
- [ ] Port the golden test corpus at
  `tests/iSHARE.IdentityServer.Tests/DelegationTestCases/*.json` into Jest
  fixtures.

### Phase 3 — Admin UI / SPA backend

- [ ] Decide between (a) keeping the existing Angular SPA and pointing it at
  the new API, or (b) replacing it with a fresh React/Vue frontend. Most of
  the SPA endpoints' value comes from a hosted IdP and a few CRUD endpoints,
  so option (b) is likely cheaper.
- [ ] Replace `iSHARE.Identity.Api` with the chosen hosted IdP (Auth0,
  Keycloak, Azure AD B2C, …). User records in the new DB only need to map
  IdP `sub` → `partyId`.
- [ ] Port the SPA admin endpoints:
  - `GET /delegations` (paged), `GET /delegations/:id` (with history),
    `POST /delegations`, `PUT /delegations/:id`, `DELETE /delegations/:id`,
    download endpoints, `POST /delegations/test`.
  - `GET /users`, plus the role-aware user CRUD from
    `iSHARE.Identity.Api.Controllers.UsersController`.
- [ ] Replace SendGrid usage with whatever the hosted IdP provides for
  password resets / activation.

### Phase 4 — Cutover and decommission

- [ ] Run both stacks side-by-side behind a reverse-proxy with traffic
  mirroring on `/delegation` and `/policy` to spot behavioural drift.
- [ ] Promote the NestJS service to primary; keep the legacy stack as a
  fallback for one release.
- [ ] Delete the legacy C# projects and the `Differences.md` file (now
  encoded as Jest tests).

## Risk register

| Risk                                              | Mitigation |
|---------------------------------------------------|-----------|
| Behavioural drift in `DelegationTranslateService` | Drive Jest tests from the legacy `DelegationTestCases/*.json` corpus before cutover. |
| Subtle JWT differences (claim ordering, base64 padding) | Use the iSHARE Conformance Test Tool as the gate before promoting. |
| RSA key format mismatch (PKCS#1 vs PKCS#8)        | `DigitalSignerService` rejects PKCS#1 with an actionable error message; README documents the conversion. |
| Database migration                                | Schema is small. Write a one-off ETL that reads the legacy SQL Server tables and writes Prisma-compatible rows. |
| Loss of IdentityServer4 features we didn't port   | Audit which IS4 features were actually used by clients; the iSHARE flow only requires `client_credentials` + JWT bearer assertion. SPA login moves to a hosted IdP. |
