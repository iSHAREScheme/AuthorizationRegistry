# Deploying the TypeScript Authorization Registry

The modernized branch is a single containerized Node service. It needs a
PostgreSQL database and the runtime environment variables from `.env.example`.

## Release Gate

Run the local gate before promoting this branch:

```bash
npm ci
npm run check
docker build -t ishare-authorization-registry:modernize .
```

For a clean database smoke test:

```bash
cp .env.example .env
# Fill in real signing material and Scheme Owner settings.
docker compose up --build
curl -fsS http://localhost:8080/health/ready
```

## Runtime Contract

The image starts with:

```bash
npm run start:migrate
```

That runs `prisma migrate deploy` and then starts `node dist/main.js`. Prisma's
database lock keeps this safe for normal rolling deploys, but large production
migrations should still be reviewed before rollout.

Health endpoints:

- `/health` is a lightweight liveness check.
- `/health/ready` verifies database connectivity and should be used by the
  platform load balancer.

## Required Runtime Environment

Set these values in the deployment platform:

- `NODE_ENV=production`
- `PORT=8080`
- `DATABASE_URL`
- `PARTY_CLIENT_ID`
- `PARTY_NAME`
- `DIGITAL_SIGNER_PRIVATE_KEY_PEM`
- `DIGITAL_SIGNER_CERT_X5C_LEAF`
- `DIGITAL_SIGNER_CERT_X5C_CHAIN`
- `SCHEME_OWNER_BASE_URL`
- `SCHEME_OWNER_CLIENT_ID`
- `SCHEME_OWNER_PUBLIC_KEY`
- `ACCESS_TOKEN_TTL_SECONDS`
- `RESPONSE_JWT_TTL_SECONDS`
- `ALLOW_DO_NOT_SIGN_HEADER=false`

## Promotion Flow

1. Deploy `modernize/typescript` to the test environment.
2. Run the iSHARE Conformance Test Tool and API smoke tests against that URL.
3. Fix deviations on this branch and keep rerunning `npm run check`.
4. Merge or fast-forward the tested branch to `main` only after the test
   environment is green.
