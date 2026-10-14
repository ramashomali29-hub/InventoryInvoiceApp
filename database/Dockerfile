# One-time database setup image.
# Contains the SQL scripts, runs them against the database container, then exits.
FROM mcr.microsoft.com/mssql/server:2022-latest

COPY *.sql /scripts/

ENTRYPOINT ["/bin/bash", "-c", "set -e; SQL=\"/opt/mssql-tools18/bin/sqlcmd -S db -U sa -P $SA_PASSWORD -C -b -I\"; $SQL -i /scripts/00-create-database.sql; $SQL -d InventoryInvoiceDatabase -i /scripts/01-schema.sql; $SQL -i /scripts/security-setup.sql; echo 'Database setup complete.'"]
