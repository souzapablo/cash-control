# Cash Control

A simple personal finance app (work in progress) to monitor my cards, incomes, and outcomes.

### Purpose
- Showcase my portfolio with a clean, focused finance tracker.
- Start small and evolve features over time.

### Tech Stack
- .NET 8.0
- PostgreSQL 18.0
- Entity Framework Core
- Vertical Slice Architecture

### Requirements

- .NET 8.0 
- Entity Framework Core tools 8.0
- Docker and Docker Compose


### How to Run

1. **Start the database:**
   ```bash
   docker compose up -d
   ```
   This starts PostgreSQL on port 5432.

2. **Install Entity Framework Core tools:**
   
   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. **Run database migrations:**
   ```bash
   dotnet ef database update --project src/CashControl.Infrastructure --startup-project src/CashControl.Api
   ```

4. **Run the API:**
   ```bash
   dotnet run --project src/CashControl.Api
   ```
   
   The API will be available at `http://localhost:5000` or the port configured in `launchSettings.json`.