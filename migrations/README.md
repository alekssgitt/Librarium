# Migrations

## Migration 001 - Initial Schema migration

**Description:** This is the initial live schema for books, members, and loans.

### Type of change
Additive (non-breaking).

### API impact
This migration created the initial API contract (`GET /api/books`, `GET /api/members`, `POST /api/loans`, `GET /api/loans/{memberId}`), so there was no previous client behavior to break. No versioning was needed no changes were necessary.

### Deployment notes
You should apply this migration before first deployment so the application has all required tables and constraints at startup. There is no coexistence window concern here because this is the baseline initial schema of the application.

### Decisions and tradeoffs
I kept the initial model small and explicit so later migrations can evolve it cleanly without hidden assumptions. I  used restrictive loan foreign keys to avoid accidental cascaded deletes of historical loan records. The tradeoff is that some deletes may require manual handling in application logic, but that is safer for audit-like history data.

---

## Migration 002 - Books Need Authors

**Description:** Adds authors and a many-to-many `BookAuthors` relationship, and updates books API responses to include author data in them.

### Type of change
Additive (potentially breaking).

### API impact
`GET /api/books` now includes an additional `authors` field while keeping all existing fields unchanged. I did not version the  api endpoint because this is an additive response change and most clients tolerate unknown fields. To preserve compatibility for legacy db data, books with no linked authors return `authors: []` instead of `null`.

### Deployment notes
You should apply the migration before or at the same time as deploying the new application version. During the coexistence window, the old app still works because existing tables are untouched, and the new app also works because the new tables are available after migration. Existing book rows remain valid even without author relationship, so there is no forced downtime or blocking any data backfill step.

### Decisions and tradeoffs
I treated the initial schema as live production, so I avoided enforcing a strict "every book must already have an author" rule at the database level in this step. Enforcing it immediately would be bad because it would either fail migration for existing data or force fake author records, both of which are risky and misleading for production environment. Instead, I chose a more transitional model: im allowing legacy rows without authors, but support real author links going forward. This way im making sure that the rollout is safe and contract stable while it is still moving toward the product requirement of author links. The downside is a temporary  gap between data until legacy rows are backfilled and then the stricter enforcement of authors books relationship can be introduced.

---

## Migration 003 - Member Profile Preparation(Adding phone numbers)

**Description:** Adds `PhoneNumber` as nullable and keeps email non-unique while preparing data for cleanup before i enforce anything.

### Type of change
Requires coordination.

### API impact
`GET /api/members` can now include `phoneNumber` as part of the expanded member model profile, but constraints are not enforced yet. No endpoint versioning was needed right now because the response shape change is additive and existing fields were not changed.

### Deployment notes
Apply this migration before enforcing constraints so existing data and old and new app versions can coexist safely. During the coexistence window, members with missing phone numbers and duplicate emails can still be allowed, which helps avoid deployment failure. You should run the duplicate detection query from the SQL file and clean up the data before running migration number 004.

### Decisions and tradeoffs
I split Requirement 2 into two migrations to avoid risky one-shot operations on live data. This phase is intentionally permissive: it introduces the schema and entity shape needed by the app while keeping the production stable. The tradeoff is that data quality is still inconsistent after this migration because duplicate emails and empty phone numbers still exist. However, this creates a controlled window for cleanup rather than silently deleting and operating on user data in migration scripts. It is easier to explain and is just so much safer operationally.

---

## Migration 004 - Member Constraint Enforcement

**Description:** Enforces required `PhoneNumber` and unique member email after precondition operations pass.

### Type of change
Requires coordination.

### API impact
No endpoint changes were required and no API versioning was done. The API behavior is tightened indirectly because writes that would violate required phone or unique email are not accepted at the database layer.

### Deployment notes
Run this migration only after data cleanup is complete. The script fails fast if duplicate emails or blank phone number values still exist, which prevents partial or unsafe enforcement. During rollout, apply this migration before enabling any new registration flows that assume hard DB constraints, so application and schema remain aligned.

### Decisions and tradeoffs
I chose precondition checks instead of auto-fixing data with sql scripts because automatic rewrites can be messy and surprising to users and data managers. Failing fast makes coordination completely visible and keeps ownership of cleanup of the data. The downside is that extra operational effort is required by the developers and one more deployment step is required. The benefit is correctness, traceability, and clearer production behavior.I believe this is a more clean way to do it since the application is treated as a live production software.

---

## Migration 005 - Loan Status addition

**Description:** Adds loan `Status` values and introduces a versioned loans endpoint so that already existing clients can continue to work unchanged.

### Type of change
Requires coordination.

### API impact
The existing `GET /api/loans/{memberId}` endpoint is kept stable and unchanged for the current frontend and still returns `loanId`, `bookTitle`, `loanDate`, and `returnDate` all the same. A new endpoint, `GET /api/v2/loans/{memberId}`, was created to include the new `status` field. I versioned here because the existing client cannot update yet and currently depends on `returnDate == null` to decide if the loan is open or not.

### Deployment notes
You should apply this migration before deploying the new API code. The artifact adds `Status`, backfills existing rows (`Returned` when `ReturnDate` is not null, otherwise `Active`), then enforces `NOT NULL`, a default of `Active`, and a status check constraint. This ensures that the old application keeps functioning during coexistence because inserts that do not specify status still receive the default value. Once the frontend is migrated, everybody should move to the v2 endpoint for status feature.

### Decisions and tradeoffs
I chose endpoint versioning for this requirement because this is not just adding some metadata, because the frontend has logic tied to the old contract and cannot change immediately. Running v1 and v2 apis in parallel helps to avoid a forced client change and protects ongoing frontend work. For historical rows, I only inferred `Active` and `Returned` because there is yet no due date or book loss data available to use `Overdue` or `Lost` in the db safely. The tradeoff is temporary maintenance cost for two endpoints, but that is acceptable compared to breaking an active frontend. I believe it strongly aligns with the live system rule of minimizing disruption first.

---

## Migration 006 - Book Retirement (Soft Delete)

**Description:** This migration adds a soft retire flag for books and also updates application behavior so retired books are hidden from catalogue results and cannot receive new loans on themselves while keeping historical loan records as they are.

### Type of change
Requires coordination.

### API impact
`GET /api/books` behavior changed to exclude retired books from results. `POST /api/loans` now checks if the book is flagged as retired or not and rejects new loans for the books with a clear validation error. Loan history endpoints are intentionally kept as they were and still return existing loan and book relationships, so retiring a book does not make old loan responses lose any book details.

### Deployment notes
You should apply this migration before deploying the new code so the `IsRetired` column exists when updated queries run on the db. During coexistence, old application code continues to work because the new column has a default value and does not remove existing columns or relationships. After new deployment, catalogue reads filter out retired books and loan creation checks retirement status before inserting.

### Decisions and tradeoffs
I have reviewed the teammate migration proposal and i accepted the main idea which is to have a boolean flag instead of hard deletion, because that preserves the history and also avoids breaking any foreign key links from `Loans`. I also accepted filtering catalogue reads so retired books disappear from normal discovery. I decided to change the implementation in three ways: I used `IsRetired` (instead of isDeleted), I added a safe guard on loan creation to block new loans for retired books, and I avoided applying global query filters that might accidentally null book data inside historical loan responses. The tradeoff is more application logic than "just a WHERE clause" which he proposed, but the way i did it prevents any confusions and also keeps all the clients stable and doesnt risk anything.

---

## Migration 007 - ISBN Column Replacement

**Description:** Replaces the legacy ISBN column with a string column and marks existing ISBN values as invalid (`NULL`) because they are truncated and corrupted and cannot be recovered.

### Type of change
Requires coordination.

### API impact
`GET /api/books` keeps the same `isbn` field name, but its now "nullable string ISBN". During and after transition, old rows return `isbn: null` until corrected values are received and filled out. I did not version the endpoint and instead I preserved field presence and documented a contract update so everybody can handle null/string safely.

### Deployment notes
You should apply this migration before deploying the updated API model to keep schema behavior and everything else aligned. The SQL artifact performs the full transition: adds replacement string column, clears legacy data as invalid, drops the old column, renames replacement, and recreates unique index as partial (`WHERE ISBN IS NOT NULL`). During coexistence, old code that is still reading `isbn` gets the same column name, but downstream clients must tolerate null values for it.

### Decisions and tradeoffs
I decided to treat existing ISBN values as faulty as per requirement. I accepted that this is a complex contract change: not in endpoint shape, but in value guarantees and type expectations across integrations. Keeping the field name stable minimizes payload churn, while returning null makes the invalid-state explicit instead of silently passing corrupted numbers. The downside is some clients may need parsing updates and null-handling immediately. The upside is correctness and a clean path for later backfill from trusted catalog sources.
