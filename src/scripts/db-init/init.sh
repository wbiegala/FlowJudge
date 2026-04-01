#!/bin/sh
set -euo pipefail
set -x

: "${POSTGRES_HOST:?POSTGRES_HOST is required}"
: "${POSTGRES_PORT:=5432}"
: "${POSTGRES_SUPERUSER:=postgres}"
: "${POSTGRES_SUPERPASS:?POSTGRES_SUPERPASS is required}"

export PGPASSWORD="$POSTGRES_SUPERPASS"

psql_base() {
  psql -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -U "$POSTGRES_SUPERUSER" -d postgres -v ON_ERROR_STOP=1 "$@"
}

create_db_and_user() {
  DB_NAME="$1"
  DB_USER="$2"
  DB_PASS="$3"

  echo ">> Init DB=$DB_NAME USER=$DB_USER"

  # rola (OK w DO)
  psql_base <<SQL
DO \$\$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = '${DB_USER}') THEN
    CREATE ROLE "${DB_USER}" LOGIN PASSWORD '${DB_PASS}';
  END IF;
END
\$\$;
SQL

  # baza (poza DO)
  if ! psql_base -tAc "SELECT 1 FROM pg_database WHERE datname='${DB_NAME}'" | grep -q 1; then
    createdb -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -U "$POSTGRES_SUPERUSER" -O "$DB_USER" "$DB_NAME"
  fi
}

create_db_and_user "$AUTH_DB_NAME"     	"$AUTH_DB_USER"     	"$AUTH_DB_PASSWORD"
create_db_and_user "$FLOWJUDGE_DB_NAME" "$FLOWJUDGE_DB_USER"  	"$FLOWJUDGE_DB_PASSWORD"

echo ">> Done."