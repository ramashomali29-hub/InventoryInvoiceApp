/*
  InventoryInvoiceApp - Database Security Setup
  ------------------------------------------------
  Run ONCE as the SQL Server admin (sa), AFTER `Update-Database` has created the tables.
  All passwords below are DEMO values for local development only.
  Change them before any real deployment.

  What this script does:
    1. Least privilege: creates a dedicated login for the app with read/write access only.
    2. Encryption at rest: enables Transparent Data Encryption (TDE, AES-256).
*/

/* ---------- 1. Least-privilege application account ---------- */
USE master;
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'inventory_app')
    CREATE LOGIN inventory_app WITH PASSWORD = 'AppUser@2026';
GO

USE InventoryInvoiceDatabase;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'inventory_app')
    CREATE USER inventory_app FOR LOGIN inventory_app;
ALTER ROLE db_datareader ADD MEMBER inventory_app;
ALTER ROLE db_datawriter ADD MEMBER inventory_app;
GO

/* ---------- 2. Transparent Data Encryption (TDE) ---------- */
USE master;
IF NOT EXISTS (SELECT 1 FROM sys.symmetric_keys WHERE name = '##MS_DatabaseMasterKey##')
    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'MasterKey@2026';
GO

IF NOT EXISTS (SELECT 1 FROM sys.certificates WHERE name = 'TDE_Cert')
BEGIN
    CREATE CERTIFICATE TDE_Cert WITH SUBJECT = 'TDE Certificate';

    -- IMPORTANT: without this backup, encrypted database backups cannot be restored.
    BACKUP CERTIFICATE TDE_Cert
    TO FILE = '/var/opt/mssql/data/TDE_Cert.cer'
    WITH PRIVATE KEY (
        FILE = '/var/opt/mssql/data/TDE_Cert.pvk',
        ENCRYPTION BY PASSWORD = 'CertBackup@2026'
    );
END
GO

USE InventoryInvoiceDatabase;
IF NOT EXISTS (SELECT 1 FROM sys.dm_database_encryption_keys WHERE database_id = DB_ID())
    CREATE DATABASE ENCRYPTION KEY
    WITH ALGORITHM = AES_256
    ENCRYPTION BY SERVER CERTIFICATE TDE_Cert;
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'InventoryInvoiceDatabase' AND is_encrypted = 1)
    ALTER DATABASE InventoryInvoiceDatabase SET ENCRYPTION ON;
GO

/* ---------- 3. Verification ---------- */
-- encryption_state = 3 means fully encrypted
SELECT DB_NAME(database_id) AS DatabaseName, encryption_state, key_algorithm, key_length
FROM sys.dm_database_encryption_keys;
GO
