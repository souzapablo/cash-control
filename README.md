# Cash Control

A simple personal finance app (work in progress) to monitor cards, incomes, and outcomes.

### Purpose
- Showcase my portfolio with a clean, focused finance tracker.
- Start small and evolve features over time.

### Tech Stack
- .NET 8.0
- PostgreSQL
- Entity Framework Core
- Vertical Slice Architecture
- Testcontainers (for integration tests)

### Requirements

- .NET 8.0
- EF Core Tools 8.x (`dotnet-ef`)
- Docker (for local PostgreSQL and running tests)

### Project Structure

```
src/
  CashControl.Api/            # Minimal API + Swagger, endpoint grouping under /api
  CashControl.Domain/         # Entities, value objects, domain primitives
  CashControl.Infrastructure/ # EF Core DbContext, configurations, migrations
```

### Configuration

- Connection string key: `ConnectionStrings:CashControlDatabase`
- Default development value lives in `src/CashControl.Api/appsettings.Development.json` and points to `localhost:5432`.

Example (already present):

```json
{
  "ConnectionStrings": {
    "CashControlDatabase": "Server=localhost;Port=5432;Database=cash-control-dev;User Id=postgres;Password=<password>;"
  }
}
```

Update the password (and other fields) to match your local PostgreSQL.

### Run PostgreSQL locally

If you do not have PostgreSQL installed, you can spin up one with Docker:

```bash
docker run -d \
  --name cash-control-postgres \
  -e POSTGRES_PASSWORD=<password> \
  -e POSTGRES_DB=cash-control-dev \
  -p 5432:5432 \
  postgres:16
```

Alternatively, there is a minimal Docker Compose snippet at the repository that you can use localy.


### How to Run

1. Install EF Core tools (once):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. Apply database migrations:
   ```bash
   dotnet ef database update \
     --project src/CashControl.Infrastructure \
     --startup-project src/CashControl.Api
   ```

3. Run the API:
   ```bash
   dotnet run --project src/CashControl.Api
   ```

   Development profile URLs (from `launchSettings.json`):
   - HTTP: `http://localhost:5193`
   - HTTPS: `https://localhost:7039`

4. Open Swagger UI (Development only):
   - `http://localhost:5193/swagger`


### Testing

Integration tests use Testcontainers, so Docker must be running. Execute from the repository root:

```bash
dotnet test
```