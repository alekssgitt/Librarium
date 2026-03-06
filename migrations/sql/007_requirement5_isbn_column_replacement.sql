BEGIN;

-- i add replacement column (string + nullable while the whole transition is happening)
ALTER TABLE "Books"
    ADD COLUMN IF NOT EXISTS "IsbnText" character varying(32);

-- i treat all existing values as invalid 
UPDATE "Books"
SET "IsbnText" = NULL;

-- i remove old uniqueness index before replacing column
DROP INDEX IF EXISTS "IX_Books_ISBN";

-- then i drop legacy column and promote the replacement column
ALTER TABLE "Books"
    DROP COLUMN IF EXISTS "ISBN";

ALTER TABLE "Books"
    RENAME COLUMN "IsbnText" TO "ISBN";

--finally i  recreate uniqueness constraint for valid (non-null) ISBNs only
CREATE UNIQUE INDEX "IX_Books_ISBN"
    ON "Books" ("ISBN")
    WHERE "ISBN" IS NOT NULL;

COMMIT;
