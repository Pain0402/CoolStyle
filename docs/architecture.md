# System Architecture: FashionEcommerce (ASP.NET Core + Vue.js)

## 1. Overview
This document defines the architectural standards for the "FashionEcommerce" project. We aim to build a **scable, high-performance, and maintainable** modular monolith that can evolve into microservices if needed.

## 2. Technology Stack

### Backend (The Core)
- **Framework**: ASP.NET Core 8 Web API
- **Language**: C# 12
- **Database**: SQL Server 2022
- **ORM**: Entity Framework Core 8 (Code-First)
- **Caching**: Redis (Distributed Cache)
- **Background Jobs**: Hangfire (Order processing, Emails)
- **Object Mapping**: AutoMapper
- **Validation**: FluentValidation
- **Logging**: Serilog + Seq/ELK

### Frontend (The Experience)
- **Framework**: Vue.js 3 (Composition API, Script Setup)
- **Build Tool**: Vite 5
- **Language**: TypeScript 5
- **State Management**: Pinia
- **Styling**: Tailwind CSS v4 (Base) + Custom SCSS (Premium UI)
- **UI Library**: Headless UI / PrimeVue (Unstyled) + Custom Components
- **Animations**: GSAP / Framer Motion (VueUse Motion)

### DevOps & Infrastructure
- **Containerization**: Docker & Docker Compose
- **CI/CD**: GitHub Actions
- **Hosting**: Windows Server (IIS) or Linux (K8s/Docker Swarm) - TBD
- **Reverse Proxy**: Nginx (if Linux) or YARP

## 3. High-Level Architecture (Clean Architecture)

```
src/
├── Backend/
│   ├── Core/                           # Inner Circle (No dependencies)
│   │   ├── Domain/                     # Entities, Value Objects, Aggregates, Domain Events
│   │   └── Application/                # Use Cases, DTOs, Interfaces, CQRS Handlers
│   ├── Infrastructure/                 # Outer Circle
│   │   ├── Persistence/                # EF Core DbContext, Repositories
│   │   ├── Identity/                   # Auth logic (JWT, IdentityServer)
│   │   └── Shared/                     # Mail, FileStorage, Payment Integrations
│   └── API/                            # Entry Point
│       ├── Controllers/                # REST Endpoints
│       ├── Middlewares/                # Error Handling, Logging
│       └── Program.cs                  # DI Container
├── Frontend/                           # Vue.js SPA
│   ├── src/
│   │   ├── components/                 # Reusable Atoms/Molecules
│   │   ├── features/                   # Vertical Slices (Cart, Product, Checkout)
│   │   ├── stores/                     # Pinia State
│   │   └── ...
└── DevOps/                             # Docker, K8s, Nginx configs
```

## 4. Key Architectural Patterns

### 4.1. Domain-Driven Design (DDD)
- **Rich Domain Models**: Business logic resides in Entities, not Services.
- **Aggregates**: Ensure consistency boundaries (e.g., `Order` executes integrity checks on `OrderItems`).
- **Domain Events**: Decouple side effects (e.g., `OrderPlacedEvent` -> `SendEmailHandler`).

### 4.2. CQRS (Command Query Responsibility Segregation)
- **Reads**: Optimize for speed. Dapper (optional) or EF Core AsNoTracking. Projections.
- **Writes**: Ensure consistency. EF Core within a Transaction.

### 4.3. Vertical Slash (Frontend)
- Organize frontend by **Features** (e.g., `features/checkout/`) rather than technical type (e.g., `components/`, `views/`).

## 5. Security & Performance
- **Auth**: JWT (Access Token + Refresh Token).
- **Rate Limiting**: IP-based rate limiting in middleware.
- **Security**: CSP, XSS protection, SQL Injection prevention (via ORM).
- **Performance**:
  - Image Optimization (WebP).
  - Lazy Loading components.
  - Redis output caching for "Product List" endpoints.

