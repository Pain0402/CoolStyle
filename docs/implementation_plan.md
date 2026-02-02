# Implementation Plan: FashionEcommerce Project

> **Status**: **Phase 4 Implementation Complete** (Admin Dashboard)
> **Deployment**: VPS + Docker
> **Payment**: Mock
> **Admin**: Simple CRUD

## Phase 1: Foundation & Infrastructure (Completed)
**Goal:** Setup the "Walking Skeleton" - a runnable app with zero features but full pipeline.

- [x] **1.1. Project Scaffolding**
  - [x] Initialize Git repository.
  - [x] Setup Solution (`.sln`) with Clean Architecture layers.
  - [x] Setup `docker-compose.yml` (SQL Server, Redis, Seq).
  - [x] Verify Environment: Dotnet 8, Node 20+, Docker.
- [x] **1.2. Shared Kernel & Core**
  - [x] Implement JSend Envelope Pattern (`ApiResponse<T>`).
  - [x] Implement Global Exception Middleware.
  - [x] Setup Serilog logging to Console & Seq.
- [x] **1.3. Infrastructure Setup**
  - [x] EF Core configuration (SQL Server).
  - [x] **Data Seeder**: `DataSeeder` service creates mock products/categories on startup.
- [x] **1.4. Frontend Foundation**
  - [x] Vue 3 + Vite + TypeScript.
  - [x] Tailwind CSS v4 setup.
  - [x] Initial Layout (Coolstyle).

## Phase 2: Core Domain - Identity & Catalog (Completed)
**Goal:** Users can register and browse products.

- [x] **2.1. Domain Modeling & Seeding**
  - [x] Entities: `Product`, `Category`, `Variant`, `Stock`.
  - [x] Database Migration (`InitialCreate`).
  - [x] Auto-Seeding logic in `Program.cs`.

- [x] **2.2. Product Public API**
  - [x] Implement `GetProducts` Query (Pagination, Filtering).
  - [x] Implement `GetProductDetail` Query.
  - [x] **Frontend**: 
    - [x] Integrate API Client (`axios` instance).
    - [x] Product List Component (Grid + Filters).
    - [x] Product Detail View (Basic).

- [x] **2.3. Authentication Module**
  - [x] Register/Login API (Identity + JWT).
  - [x] Frontend: Auth Store (Pinia) + Login Modal.
  - [x] User Menu & Logout Logic.

## Phase 3: Sales - Cart, Checkout & Orders (Completed)
**Goal:** Mocked Revenue generation flow.

- [x] **3.1. Shopping Cart**
  - [x] Store: Pinia Cart Store (LocalStorage persistence).
  - [x] UI: Cart Drawer (Slide-over).
  - [x] Logic: Add to Cart, Remove Item, Update Quantity.
- [x] **3.2. Checkout Process (Mock)**
  - [x] Address Form & Shipping Fee logic.
  - [x] Payment Selection.
- [x] **3.3. Order Management**
  - [x] Backend: Order Entities (`Order`, `OrderItem`).
  - [x] API: `CreateOrder` endpoint.
  - [x] Frontend: Checkout Modal & API Integration.

## Phase 4: Admin & Operations (Completed)
**Goal:** Simple Back-office.

- [x] **4.1. Routing & Layout**
  - [x] Install `vue-router`.
  - [x] Split `App.vue` into `HomeView` and `AdminView`.
- [x] **4.2. Admin Dashboard**
  - [x] Secure `/admin` route (Basic URL routing).
  - [x] Admin Order List (Table View).
  - [x] Update Order Status API.

## Phase 5: Production Prep (In Progress)
**Goal:** Deploy to VPS.

- [ ] **5.1. Docker Production**
  - [ ] Backend Dockerfile (Multi-stage build).
  - [ ] Frontend Dockerfile (Nginx).
  - [ ] `docker-compose.prod.yml`.
- [ ] **5.2. Deploy**
  - [ ] Script to build and run in production mode.
