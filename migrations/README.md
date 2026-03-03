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
