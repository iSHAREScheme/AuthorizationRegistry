# `modern/` — TypeScript / NestJS rewrite

This folder contains the in-progress modern rewrite of the iSHARE
Authorization Registry. The legacy .NET Core 3.1 implementation continues
to live in the repository root and is the reference for this port.

- **`authorization-registry/`** — NestJS application.
  - See its [`README.md`](authorization-registry/README.md) for setup.
  - See [`MIGRATION.md`](authorization-registry/MIGRATION.md) for the staged
    migration plan and what is/isn't ported yet.
