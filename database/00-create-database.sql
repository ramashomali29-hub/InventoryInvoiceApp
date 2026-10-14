-- Creates the application database if it does not exist yet.
IF DB_ID(N'InventoryInvoiceDatabase') IS NULL
    CREATE DATABASE InventoryInvoiceDatabase;
GO
