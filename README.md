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
- SQL Server 2022
- Docker and Docker Compose
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
| Network exposure | The database is reachable only inside the Docker network; the web app is published on `127.0.0.1` only. |

## Quick Start (Docker only)

The only thing you need is **[Docker Desktop](https://www.docker.com/products/docker-desktop/)**. No source code, .NET SDK, Visual Studio, or SQL Server installation required.

### 1. Install and open Docker Desktop

Install Docker Desktop, open it, and wait until it shows **Engine running**.

### 2. Download the compose file

Create an empty folder (for example `C:\inventory-app`), open a terminal inside it, and run:

```powershell
curl.exe -O https://raw.githubusercontent.com/ramashomali29-hub/InventoryInvoiceApp/master/docker-compose.hub.yml
```

Or open [`docker-compose.hub.yml`](docker-compose.hub.yml) on GitHub, click **Download raw file**, and save it in that folder.

### 3. Start the application

In the same folder, run:

```bash
docker compose -f docker-compose.hub.yml up -d
```

The first run downloads the images and sets up the database, so it can take a few minutes.

### 4. Check that everything started

```bash
docker compose -f docker-compose.hub.yml ps -a
```

You should see:

- `db` — **Up (healthy)**
- `db-init` — **Exited (0)** (the one-time setup finished successfully)
- `web` — **Up**

### 5. Open the app

Go to **http://localhost:8080** and log in with the [demo account](#demo-login).

### Stop / reset

```bash
# Stop the app (data is kept)
docker compose -f docker-compose.hub.yml down

# Stop and delete all data
docker compose -f docker-compose.hub.yml down -v
```

### Troubleshooting

- **`port is already allocated`** — another program is using port 8080. Stop it, or edit `docker-compose.hub.yml` and change `"127.0.0.1:8080:8080"` to another port such as `"127.0.0.1:8090:8080"`, then open http://localhost:8090.
- **The page does not open** — view the logs with `docker compose -f docker-compose.hub.yml logs web` and `docker compose -f docker-compose.hub.yml logs db-init`.
- **Start fresh** — run `docker compose -f docker-compose.hub.yml down -v`, then `up -d` again.

### What runs

| Container | Image | Purpose |
|-----------|-------|---------|
| `db` | `mcr.microsoft.com/mssql/server:2022-latest` | SQL Server 2022 database (not exposed to the host) |
| `db-init` | `ramaalshomali/inventoryinvoiceapp-dbinit` | One-time job: creates the database, tables, least-privilege login, and TDE encryption, then exits |
| `web` | `ramaalshomali/inventoryinvoiceapp-web` | The ASP.NET Core application, connecting with the least-privilege account |

### Build from source (optional)

To build the images yourself instead of downloading them, clone the repository and run from its root:

```bash
docker compose up -d --build
```

## Demo Login

For local testing only. These credentials are defined in `appsettings.Development.json` and **must be changed in any real deployment**.

- Email: `admin@inventory.com`
- Password: `Admin@123`

Log in as Admin to create Warehouse and Cashier users.

## Project Structure

```
InventoryInvoiceApp/
├── docker-compose.hub.yml      # Run the app from Docker Hub images
├── docker-compose.yml          # Build and run the app from source
├── database/
│   ├── Dockerfile              # Image for the one-time database setup job
│   ├── 00-create-database.sql  # Creates the database
│   ├── 01-schema.sql           # Tables (generated from EF Core migrations)
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
