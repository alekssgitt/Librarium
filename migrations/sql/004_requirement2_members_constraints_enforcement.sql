BEGIN;
--first i have precondition check that checks if the duplicate emails still exist in the database
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM "Members"
        GROUP BY lower("Email")
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'Cannot make  all emails unique: duplicate member emails still exist.';
    END IF;
END $$;

--second i have another precondition check that checks if there are any phone number fields in the database that are not filled out and are null
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM "Members"
        WHERE "PhoneNumber" IS NULL OR btrim("PhoneNumber") = ''
    ) THEN
        RAISE EXCEPTION 'Cannot enforce required phone number: blank phone values still exist in the database.';
    END IF;
END $$;

--then if both precondition checks succeed, i enforce constraints such as setting phone numebr field as not null and making an email field unique.
ALTER TABLE "Members"
    ALTER COLUMN "PhoneNumber" SET NOT NULL;

DROP INDEX IF EXISTS "IX_Members_Email";
CREATE UNIQUE INDEX "IX_Members_Email" ON "Members" (lower("Email"));

COMMIT;
