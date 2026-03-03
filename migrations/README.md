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
