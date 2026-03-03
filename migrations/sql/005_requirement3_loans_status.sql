BEGIN;
--first i create a new Status collumn in the Loans table
ALTER TABLE "Loans"
    ADD COLUMN IF NOT EXISTS "Status" character varying(16);

--then i set the status collumn for all loan entries to either Returned or Active based on the 
--old logic where if return date is null then the loan is active
UPDATE "Loans"
SET "Status" = CASE
    WHEN "ReturnDate" IS NOT NULL THEN 'Returned'
    ELSE 'Active'
END
WHERE "Status" IS NULL;

--enforcing NOT NULL constraint on the status collumn
ALTER TABLE "Loans"
    ALTER COLUMN "Status" SET NOT NULL;

--setting default expression to be Active so that on loan creation the status is automatically active
ALTER TABLE "Loans"
    ALTER COLUMN "Status" SET DEFAULT 'Active';

ALTER TABLE "Loans"
    DROP CONSTRAINT IF EXISTS "CK_Loans_Status";

ALTER TABLE "Loans"
    ADD CONSTRAINT "CK_Loans_Status"
    CHECK ("Status" IN ('Active', 'Returned', 'Overdue', 'Lost'));

COMMIT;
