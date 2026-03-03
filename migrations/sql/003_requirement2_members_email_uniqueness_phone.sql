BEGIN;
--I just add a new collumn to the members table without any constraints for now
ALTER TABLE "Members"
    ADD COLUMN IF NOT EXISTS "PhoneNumber" character varying(64);

DROP INDEX IF EXISTS "IX_Members_Email";
CREATE INDEX "IX_Members_Email" ON "Members" ("Email");


COMMIT;
