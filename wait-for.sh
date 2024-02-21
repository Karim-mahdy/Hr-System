#!/bin/sh
# wait-for.sh

set -e

host="$1"
shift
cmd="$@"

echo "Waiting for SQL Server at $host:1433..."

until /opt/mssql-tools/bin/sqlcmd -S "$host" -U sa -P "$SA_PASSWORD" -Q "SELECT 1;"; do
  >&2 echo "SQL Server is unavailable - sleeping"
  sleep 1
done

echo "SQL Server is up - executing command: $cmd"
exec $cmd
