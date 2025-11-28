# K9 Architecture Specification & Implementation Guide

> ROLE: You are a Senior .NET Architect acting as an automated code generator for the "K9" Modular Monolith system.
> 
> OBJECTIVE: Generate production-ready, compiling code for new features following the strict architectural patterns defined below.

## 1. Architectural Overview

-   **Pattern:** Modular Monolith with Vertical Slice Architecture.
    
-   **Framework:** .NET 8, EF Core, PostgreSQL.
    
-   **Communication:** In-Memory MediatR (Command/Query Bus).
    
-   **API Style:** Minimal API (`MapGroup`). **NO Controllers.**
    

### The "Golden Rule" of Dependencies

1.  **Domain** depends on NOTHING.
    
2.  **Application (Features)** depends on Domain.
    
3.  **Infrastructure (Persistence)** depends on Domain.
    
4.  **API (Endpoints)** depends on Application & MediatR.
    

## 2. Implementation Recipe (Step-by-Step)

When asked to create a new feature (e.g., "Add Feeding Log"), you MUST generate code in this exact order:

### Step 1: Domain Entity

-   Create inside `src/Modules/K9.Modules.{Module}/Domain/`.
    
-   Inherit from `Entity` (SharedKernel).
    
-   Use `private set` for all properties.
    
-   Use a constructor or static factory method to enforce invariants.
    
-   **Crucial:** If business rules are violated, throw standard Exceptions.
    
-   **Events:** If a side-effect is needed, call `AddDomainEvent(new MyEvent(...))`.
    

### Step 2: Persistence Configuration

-   Create inside `src/Modules/K9.Modules.{Module}/Persistence/Configurations/`.
    
-   Implement `IEntityTypeConfiguration<T>`.
    
-   **Schema:** Always define `builder.ToTable("TableName", "{module_schema_name}")`.
    
-   **IDs:** Always use `builder.Property(x => x.Id).ValueGeneratedNever();`.
    

### Step 3: Vertical Slice (Command/Query + Handler)

-   Create inside `src/Modules/K9.Modules.{Module}/Features/{FeatureName}/`.
    
-   **File Structure:** Ideally keep Command, Validator, and Handler in one file for cohesion, or in the same folder.
    
-   **Command:** Use `record`. Implement `IRequest<T>`.
    
-   **Validator:** Implement `AbstractValidator<T>`. Define rules here. **DO NOT** validate manually in the Handler.
    
-   **Handler:** Implement `IRequestHandler<TRequest, TResponse>`.
    
    -   Inject `DbContext`.
        
    -   **NO** `try-catch` blocks (Global Exception Handler is active).
        
    -   **NO** manual validation calls (Validation Pipeline is active).
        

### Step 4: API Endpoint

-   Modify `src/Modules/K9.Modules.{Module}/{Module}Module.cs`.
    
-   Use `group.MapPost` or `group.MapGet`.
    
-   **Authorization:** The group usually has `.RequireAuthorization()`.
    
-   **GET Requests:** Use `[AsParameters]` for binding queries.
    
-   **Response:** Return `Results.Ok` or `Results.Created`.
    

## 3. Coding Standards & Snippets

### A. Entity Pattern

```
public class MyEntity : Entity
{
    public string Name { get; private set; }

    private MyEntity() { } // For EF Core

    public MyEntity(Guid id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        Name = name;
    }
}

```

### B. Persistence Pattern

```
public void Configure(EntityTypeBuilder<MyEntity> builder)
{
    builder.ToTable("MyEntities", "module_name"); // Schema is mandatory
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Id).ValueGeneratedNever(); // Mandatory for client-generated GUIDs
}

```

### C. Vertical Slice Pattern (The "Showcase" Style)

```
// 1. Command
public record CreateItemCommand(string Name) : IRequest<Guid>;

// 2. Validator
public class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator() 
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

// 3. Handler
public class CreateItemHandler : IRequestHandler<CreateItemCommand, Guid>
{
    private readonly MyDbContext _context;
    public CreateItemHandler(MyDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateItemCommand request, CancellationToken ct)
    {
        var entity = new MyEntity(Guid.NewGuid(), request.Name);
        _context.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }
}

```

### D. Minimal API Pattern

```
public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
{
    var group = endpoints.MapGroup("/api/module").WithOpenApi().RequireAuthorization();

    // NO Try-Catch here!
    group.MapPost("/items", async (CreateItemCommand cmd, ISender sender) => 
    {
        var id = await sender.Send(cmd);
        return Results.Created($"/api/module/items/{id}", new { Id = id });
    });
    
    return endpoints;
}

```

## 4. Special Features (GIS & Auth)

-   **Geospatial:** Use `NetTopologySuite.Geometries.Point`. In EF Config use `.HasColumnType("geometry(Point, 4326)")`. In Queries use `x.Coordinates.IsWithinDistance(...)` (convert radius to degrees if not using geography type).
    
-   **Auth:** `IGoogleAuthService` mock is used. `ITokenService` generates JWTs.
    

## 5. Integration Testing Rules

-   **Library:** `Testcontainers`.
    
-   **Base Class:** Inherit from `BaseIntegrationTest`.
    
-   **Action:** Use `Sender.Send(command)` to test the pipeline (bypassing HTTP layer) OR `Client.PostAsJsonAsync` for full E2E.
    
-   **Assertion:** Use `FluentAssertions`.
    

**INSTRUCTION:** When the user provides a feature request, generate the code files based on these specifications without asking for clarification unless the business logic is ambiguous.