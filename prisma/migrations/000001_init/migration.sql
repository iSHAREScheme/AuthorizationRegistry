-- CreateTable
CREATE TABLE "users" (
    "id" UUID NOT NULL,
    "identity_user_id" TEXT,
    "email" TEXT,
    "party_id" TEXT,
    "party_name" TEXT,
    "active" BOOLEAN NOT NULL DEFAULT true,
    "created_at" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMP(3) NOT NULL,

    CONSTRAINT "users_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "delegations" (
    "id" UUID NOT NULL,
    "authorization_registry_id" TEXT NOT NULL,
    "policy_issuer" TEXT NOT NULL,
    "access_subject" TEXT NOT NULL,
    "policy" TEXT NOT NULL,
    "deleted" BOOLEAN NOT NULL DEFAULT false,
    "created_at" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" TIMESTAMP(3) NOT NULL,
    "created_by_id" UUID,
    "updated_by_id" UUID,

    CONSTRAINT "delegations_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "delegations_histories" (
    "id" UUID NOT NULL,
    "delegation_id" UUID NOT NULL,
    "policy" TEXT NOT NULL,
    "created_at" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "created_by_id" UUID,

    CONSTRAINT "delegations_histories_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE UNIQUE INDEX "delegations_authorization_registry_id_key" ON "delegations"("authorization_registry_id");

-- CreateIndex
CREATE INDEX "delegations_policy_issuer_access_subject_idx" ON "delegations"("policy_issuer", "access_subject");

-- CreateIndex
CREATE INDEX "delegations_deleted_idx" ON "delegations"("deleted");

-- CreateIndex
CREATE INDEX "delegations_histories_delegation_id_idx" ON "delegations_histories"("delegation_id");

-- AddForeignKey
ALTER TABLE "delegations" ADD CONSTRAINT "delegations_created_by_id_fkey" FOREIGN KEY ("created_by_id") REFERENCES "users"("id") ON DELETE RESTRICT ON UPDATE RESTRICT;

-- AddForeignKey
ALTER TABLE "delegations" ADD CONSTRAINT "delegations_updated_by_id_fkey" FOREIGN KEY ("updated_by_id") REFERENCES "users"("id") ON DELETE RESTRICT ON UPDATE RESTRICT;

-- AddForeignKey
ALTER TABLE "delegations_histories" ADD CONSTRAINT "delegations_histories_delegation_id_fkey" FOREIGN KEY ("delegation_id") REFERENCES "delegations"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "delegations_histories" ADD CONSTRAINT "delegations_histories_created_by_id_fkey" FOREIGN KEY ("created_by_id") REFERENCES "users"("id") ON DELETE RESTRICT ON UPDATE RESTRICT;
