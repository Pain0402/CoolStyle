# Backend Development Rules & Guidelines

## 1. Architectural Style: Clean Architecture (Onion)
Strict dependency flow: **API -> Infrastructure -> Application -> Domain**.

### 1.1. Domain Layer (The Heart)
- **Pure C#**. No references to EF Core, ASP.NET Core, or third-party libraries.
- **Entities**: Rich models with logic (e.g., `order.AddLast()` validates state). Private setters.
- **Value Objects**: Use for complex types (Address, Money).
- **Exceptions**: Domain-specific exceptions (`OrderAlreadyShippedException`).

### 1.2. Application Layer (The Orchestrator)
- **CQRS**: Use MediatR. Separate `Commands` (Write) and `Queries` (Read).
- **Validation**: FluentValidation pipeline for all commands.
- **DTOs**: Map Entities to DTOs here (using AutoMapper or manual mapping).
- **Interfaces**: Define `IRepository`, `IEmailService` here. Implement in Infra.

### 1.3. Infrastructure Layer (The Side Effects)
- **Persistence**: EF Core DbContext.
- **External Services**: Implementations for Email, Payment, Storage.
- **Identity**: ASP.NET Identity configuration.

### 1.4. API Layer (The Entry Point)
- **Controllers**: Thin controllers. They only call MediatR and return `IActionResult`.
- **Global Error Handling**: Middleware to catch Domain Exceptions -> 400 Bad Request.

## 2. Coding Standards
- **Async/Await**: All I/O operations must be asynchronous.
- **Nullability**: `Nullable<enable>` is ON. Handle possible nulls.
- **REST Principles**:
  - GET /resource (200 OK)
  - POST /resource (201 Created + Location Header)
  - PUT /resource/{id} (204 No Content for Updates)
  - DELETE /resource/{id} (204 No Content)
- **Result Pattern**: Use a `Result<T>` wrapper for standardized API responses (Success/Failure).

## 3. Database Guidelines
- **Naming**: Snake_case for tables/columns is preferred in SQL, but CamelCase in C# mapping (standard EF Core).
- **Indexes**: Always index Foreign Keys and high-cardinality query columns.
- **No IQueryable**: Do NOT return `IQueryable` from Repositories. Leaky abstraction.

## 4. Testing
- **Unit Tests**: Domain and Application layers (xUnit + Moq).
- **Integration Tests**: Test with real In-Memory DB or Testcontainers.
