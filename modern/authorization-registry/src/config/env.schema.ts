import { z } from 'zod';

const csv = (v: string | undefined) =>
  (v ?? '')
    .split(',')
    .map((s) => s.trim())
    .filter(Boolean);

export const envSchema = z.object({
  NODE_ENV: z.enum(['development', 'test', 'staging', 'production']).default('development'),
  PORT: z.coerce.number().int().positive().default(8080),
  LOG_LEVEL: z.enum(['fatal', 'error', 'warn', 'info', 'debug', 'trace']).default('info'),

  DATABASE_URL: z.string().min(1),

  PARTY_CLIENT_ID: z.string().min(1),
  PARTY_NAME: z.string().min(1),

  DIGITAL_SIGNER_PRIVATE_KEY_PEM: z.string().min(1),
  DIGITAL_SIGNER_CERT_X5C_LEAF: z.string().min(1),
  DIGITAL_SIGNER_CERT_X5C_CHAIN: z.string().optional().default(''),

  SCHEME_OWNER_BASE_URL: z.string().url(),
  SCHEME_OWNER_CLIENT_ID: z.string().min(1),
  SCHEME_OWNER_PUBLIC_KEY: z.string().min(1),

  ACCESS_TOKEN_TTL_SECONDS: z.coerce.number().int().positive().default(3600),
  RESPONSE_JWT_TTL_SECONDS: z.coerce.number().int().positive().default(30),

  ALLOW_DO_NOT_SIGN_HEADER: z
    .string()
    .optional()
    .default('false')
    .transform((v) => v === 'true'),
});

export type Env = z.infer<typeof envSchema>;

export function loadEnv(raw: NodeJS.ProcessEnv): Env & { x5cChain: string[] } {
  const parsed = envSchema.parse(raw);
  // The PEM may arrive with literal "\n" sequences when injected via single-line env var.
  parsed.DIGITAL_SIGNER_PRIVATE_KEY_PEM = parsed.DIGITAL_SIGNER_PRIVATE_KEY_PEM.replace(
    /\\n/g,
    '\n',
  );
  return { ...parsed, x5cChain: csv(parsed.DIGITAL_SIGNER_CERT_X5C_CHAIN) };
}
