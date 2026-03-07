#!/usr/bin/env bash
set -euo pipefail

docker compose -f docker-compose.yml up -d postgres

echo "starting up postgres"
until docker compose -f docker-compose.yml exec -T postgres pg_isready -U libraryuser -d LibraryDb >/dev/null 2>&1; do
  sleep 1
done

echo "applying all migrations from migrations/sql in order"
ls migrations/sql/*.sql | sort | xargs cat | docker compose -f docker-compose.yml exec -T postgres \
  psql -v ON_ERROR_STOP=1 -U libraryuser -d LibraryDb

echo "database started and all migrations applied"
