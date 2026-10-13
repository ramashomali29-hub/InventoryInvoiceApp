# InventoryInvoiceApp

An inventory and invoice management system built with ASP.NET Core MVC, Entity Framework Core, and SQL Server, with role-based access control and a security-focused configuration.

## Features

- Secure login with account lockout and rate limiting
- Role-based access control: **Admin**, **Warehouse**, and **Cashier**
- Admin dashboard for creating Warehouse and Cashier users
- Warehouse supply management (providers, materials, quantities, prices)
- Cashier sales with automatic stock deduction (transactional, FIFO)
- Stock movement history

## Technologies

- ASP.NET Core MVC (C#)
- Entity Framework Core
- SQL Server 2022 (running in Docker)
- ASP.NET Core Identity
- HTML, CSS, JavaScript, Bootstrap

## Security

This project was built and reviewed with security in mind:

| Area | Implementation |
|------|----------------|
| SQL Injection | All data access uses Entity Framework Core parameterized queries; no raw SQL. |
| Cross-Site Scripting (XSS) | Razor automatic output encoding; no raw HTML rendering; all JavaScript in external files, so the Content-Security-Policy blocks inline scripts (`script-src 'self'`). |
| CSRF | Anti-forgery tokens validated automatically on all POST requests. |
| Security headers | `Content-Security-Policy`, `X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`. |
| Authentication | Strong password policy, account lockout after 5 failed attempts, login rate limiting, secure `HttpOnly` / `SameSite=Strict` cookies. |
| Least privilege | The app connects with a dedicated database login (`inventory_app`) that has read/write access only and cannot alter or drop tables. |
| Encryption in transit | Encrypted connection between the app and the database (`Encrypt=True`). |
| Encryption at rest | Transparent Data Encryption (TDE, AES-256) enabled on the database. |
| Secrets management | No credentials in the base `appsettings.json`; local demo values live only in `appsettings.Development.json`. |
| Network exposure | The database container port is bound to `127.0.0.1` only. |

## Getting Started (Local)

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Visual Studio (or the `dotnet ef` CLI tools)

### 1. Start the database container

```bash
docker run -d --name mydb -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Test@12345" -p 127.0.0.1:1433:1433 -v sqldata:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest
```

Wait about a minute for SQL Server to start.

### 2. Create the database and tables

Run the migrations **using the admin (sa) account**, because the app account is not allowed to change the database schema. In Visual Studio's Package Manager Console:

```powershell
Update-Database -Connection "Server=localhost,1433;Database=InventoryInvoiceDatabase;User Id=sa;Password=Test@12345;TrustServerCertificate=True"
```

### 3. Apply the database security setup

Creates the least-privilege app account and enables TDE encryption. From the repository root:

```powershell
docker cp database\security-setup.sql mydb:/tmp/security-setup.sql
docker exec -it mydb /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Test@12345" -C -i /tmp/security-setup.sql
```

### 4. Run the app

Open the solution in Visual Studio and run it with the `https` profile.

## Demo Login

For local testing only. These credentials are defined in `appsettings.Development.json` and **must be changed in any real deployment**.

- Email: `admin@inventory.com`
- Password: `Admin@123`

Log in as Admin to create Warehouse and Cashier users.

## Project Structure

```
InventoryInvoiceApp/
├── database/
│   └── security-setup.sql      # Least-privilege account + TDE encryption
└── WebApplication3/
    ├── Controllers/            # Account, Admin, Warehouse, Cashier
    ├── Data/                   # DbContext and seed data
    ├── Migrations/             # EF Core migrations
    ├── Models/                 # Entities and view models
    ├── Views/                  # Razor views
    ├── wwwroot/js/             # External JavaScript (CSP-compliant)
    └── Program.cs              # Security headers, auth, rate limiting
```

## Note

All passwords in this repository are demo values for local development only.
