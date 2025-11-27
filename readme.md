
# K9 - Modular Monolith Architecture 🐕

> A robust, production-ready .NET 8 Modular Monolith reference architecture, showcasing Domain-Driven Design (DDD), CQRS, and advanced Geospatial features with PostGIS.

**K9** is a specialized veterinary and activity tracking system designed for dogs (specifically inspired by Lola, a Golden Retriever from Rijeka). It demonstrates how to build scalable systems that are logically separated into modules but physically deployed as a single unit, avoiding the complexity of Microservices until absolutely necessary.

## 🏗 Architecture Overview

The solution follows the **Modular Monolith** pattern with **Vertical Slice Architecture** inside each module.

### 1. High-Level Structure

-   **K9.Bootstrapper**: The entry point (API Host). It knows about all modules but contains no business logic. It handles cross-cutting concerns (Auth, Logging, Global Exception Handling).
    
-   **K9.SharedKernel**: Contains building blocks shared across modules (Base Entities, Domain Events, Common Behaviors).
    
-   **Modules**:
    
    -   **Health**: The Core domain. Handles medical records, vaccinations, and strictly audited weight logs.
        
    -   **Activity**: Geospatial domain. Handles locations and pathfinding using PostGIS.
        
    -   **Identity**: Authentication & Authorization domain.
        

### 2. Module Internals (Clean Architecture)

Each module is self-contained and follows strict encapsulation:

-   **Domain**: Rich Domain Models, Aggregates, and Domain Events. No external dependencies.
    
-   **Persistence**: EF Core configurations, Migrations, and Repositories (isolated schemas per module).
    
-   **Features (Application)**: CQRS pattern using MediatR. Commands, Queries, Validators, and Handlers are grouped by feature (Vertical Slices), not by technical layers.
    

## 🚀 Tech Stack & Key Features

### Core

-   **.NET 8** (LTS)
    
-   **PostgreSQL 16** (Database)
    
-   **Entity Framework Core** (ORM)
    

### "The Cool Stuff"

-   **🌍 PostGIS & NetTopologySuite**: Advanced geospatial queries (e.g., _"Find suitable lakes within 50km radius"_). Geometry mapping directly in EF Core.
    
-   **🏗 Testcontainers**: Zero-mock integration testing. Tests spin up real Docker containers with PostgreSQL/PostGIS to ensure reliability.
    
-   **📡 Event-Driven Architecture (In-Memory)**: Decoupling modules via MediatR Domain Events (e.g., Weight changes trigger alerts without coupling the API).
    
-   **🛡 MediatR Pipeline Behaviors**: Automatic validation and logging pipeline. No `try-catch` blocks or manual validation calls in handlers.
    
-   **📝 Structured Logging**: Serilog integration with rich context (machine name, thread id, correlation).
    
-   **🔐 JWT Authentication**: Custom token issuance and validation with granular authorization policies.
    
-   **🛑 Global Exception Handling**: Centralized `IExceptionHandler` implementation translating exceptions to RFC 7807 Problem Details.
    

## 📂 Project Structure

```
src/
├── K9.Bootstrapper         # API Host, DI Composition, Swagger, Serilog setup
├── K9.SharedKernel         # BaseEntity, IDomainEvent, ValidationBehavior
└── Modules/
    ├── K9.Modules.Activity # GIS Logic (PostGIS), Locations
    ├── K9.Modules.Health   # Core Logic (Vaccinations, Weight Alerts)
    └── K9.Modules.Identity # Auth Logic (Google Sign-In Mock, JWT)

tests/
├── K9.Tests.Integration    # Testcontainers based tests (End-to-End flows)
└── K9.Modules.Health.Tests.Unit # Pure Domain Logic tests


```

## 🛠 Prerequisites

-   [Docker Desktop](https://www.docker.com/products/docker-desktop/ "null") (Required for DB and Tests)
    
-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0 "null")
    
-   IDE of choice (Visual Studio 2022 / JetBrains Rider / VS Code)
    

## 🔐 Configuration & Secrets (Important!)

This project avoids storing secrets in `appsettings.json`. Before running the project, you must configure your local environment.

### 1. Runtime Secrets (User Secrets)

For running the application (`dotnet run`), we use .NET User Secrets to store the database connection string and JWT secret securely.

Run these commands in your terminal (from the root folder):

```
# Initialize secrets for the Bootstrapper project
dotnet user-secrets init --project K9.Bootstrapper/K9.Bootstrapper.csproj

# Set the Database Connection String (matches docker-compose)
dotnet user-secrets set "ConnectionStrings:Database" "Host=localhost;Port=5432;Database=k9_db;Username=postgres;Password=lola_password_123" --project K9.Bootstrapper/K9.Bootstrapper.csproj

# Set the JWT Secret Key
dotnet user-secrets set "Jwt:Secret" "your_secret_key_minimum_32_chars_in_length" --project K9.Bootstrapper/K9.Bootstrapper.csproj

```

### 2. Design-Time Configuration (For EF Core Tools)

The `dotnet ef` tools (Migrations) require a physical configuration file to instantiate the DbContexts via Factories. User Secrets are not always visible in this design-time context.

1.  Create a file named `appsettings.Development.json` in `K9.Bootstrapper/`.
    
2.  **Ensure this file is in your `.gitignore`!**
    
3.  Add the following content:
    

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=k9_db;Username=postgres;Password=lola_password_123"
  },
  "Jwt": {
    "Secret": "your_secret_key_minimum_32_chars_in_length"
  }
}

```

## 🚦 Getting Started

### 1. Start Infrastructure

The project uses `docker-compose` to spin up a PostgreSQL instance with the PostGIS extension pre-installed.

```
docker-compose up -d

```

### 2. Run the API

Navigate to the Bootstrapper and run the project. EF Core migrations will be applied automatically (via instructions or manual update).

```
dotnet run --project K9.Bootstrapper

```

The API will be available at https://localhost:7015 (or similar port).

Swagger UI: https://localhost:7015/swagger

### 3. Authentication Flow (How to use)

The API is secured. To interact with protected endpoints:

1.  Call `POST /api/identity/login` with `{"idToken": "valid_token_123"}` (Mocked for dev).
    
2.  Copy the `token` from the response.
    
3.  In Swagger, click **Authorize** and paste the token (Bearer scheme).
    
4.  You can now access endpoints like `POST /api/health/weight` or `GET /api/activity/locations/nearby`.
    

## 🧪 Testing Strategy

We follow a strict testing pyramid:

1.  **Unit Tests**: Focused strictly on **Domain Logic** (e.g., calculating vaccination dates, weight fluctuation algorithms). No database mocking allowed here.
    
2.  **Integration Tests**: Powered by **Testcontainers**.
    
    -   Spins up a real ephemeral PostgreSQL+PostGIS container.
        
    -   Migrates the database schema on the fly.
        
    -   Tests the full pipeline: `API Request -> Mediator -> Validation -> EF Core -> DB -> Logic -> Response`.
        
    -   _Why?_ Because mocking geospatial queries is unreliable. We test against the real thing.
        

Run tests via CLI:

```
dotnet test

```

## 📝 Design Decisions (Why?)

-   Why PostgreSQL?
    
    SQL Server's spatial support is decent, but PostGIS is the industry standard for GIS. It offers superior indexing and functions (e.g., ST_DWithin, ProjectToGeography) crucial for the Activity module.
    
-   Why Modular Monolith?
    
    Microservices introduce network latency and distributed transaction complexity. A Modular Monolith offers the same logical separation (impossible to create spaghetti code due to strict project references) but with zero deployment overhead. It can be split into microservices in hours, not days.
    
-   Why Vertical Slices?
    
    Grouping code by technical layers (Services, Controllers, Repositories) leads to jumping between folders. Grouping by Features (RegisterDog) keeps high cohesion and makes code navigation instant.
    

Made with ❤️ in Rijeka by Zoran.

Dedicated to my Golden Retriever Lola 🐾 (who hates waves but loves lakes).