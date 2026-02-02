# Functional Requirements Specification

## 1. User Roles
- **Guest (Visitor)**: Browse, Search, View Product, Add to Cart (Redis/Local), Checkout (Guest).
- **Customer (Auth)**: Profile management, Order History, Address Book, Wishlist.
- **Admin**: Product Management, Order Status Management, Basic Dashboard.

## 2. Core Modules

### 2.1. Authentication & Identity
- Register/Login (Email/Password).
- Social Login (Google/Facebook) - *Phase 2*.
- Forgot Password flow.
- Email Verification.

### 2.2. Product Catalog (Realism Focus)
- **Categories**: Multi-level (e.g., Men -> Tops -> T-Shirts).
- **Collections**: Seasonal collections (Summer 2025, Essentials).
- **Product Details**:
  - Multiple images (Gallery + Zoom).
  - Rich text description (Material, Care instructions).
  - **Variants**: SKU combinations (Color + Size).
  - Real-time Stock check (Visual: "Only 3 left!").
- **Filtering**:
  - Filter by Price Range, Size (S, M, L), Color (Hex visual).
  - Sort by: Newest, Price (Low/High), Best Selling.

### 2.3. Shopping Cart & Checkout
- **Cart**:
  - Persistent cart for logged-in users.
  - "Mini-cart" drawer on the right.
  - Update quantity, Remove item.
- **Checkout Flow**:
  1. **Info**: Email + Shipping Address (District/Ward Selector).
  2. **Shipping**: Standard Delivery (Mock calculation based on region).
  3. **Payment**:
     - COD (Cash on Delivery).
     - Banking QR (Static QR for MVP).
  4. **Confirmation**: Thank you page + Email receipt.

### 2.4. Order Management
- **Customer View**:
  - Order Status tracking timeline (Placed -> Confirmed -> Shipping -> Delivered).
  - Cancel Order (logic: only if status == Pending).
- **Admin View**:
  - List all orders with filters (Status, Date, Customer).
  - Update Order Status (Trigger mock lifecycle).

## 3. Data Strategy (Seeding)
> **Goal**: The site must look ALIVE.

- **Data Source**: We will create a `SeedData/` directory with robust JSON files.
- **Images**: Use unsplash source / or placeholder image services with specific "fashion" keywords, or host a curated set of 20-30 high-quality images on Cloudinary specifically for this demo.
- **Content**:
  - 50+ Real Products (e.g., "Cotton Compact T-Shirt", "Daily Shorts").
  - 5+ Categories.
  - 20+ Real Customer Reviews (Mocked).

## 4. Non-Functional Requirements
- **Performance**:
  - Homepage First Contentful Paint < 1.5s.
  - API Response Time < 200ms (95th percentile).
- **SEO**:
  - Dynamic Meta Tags for Product Pages.
  - Canonical URLs.
  - Sitemap.xml generation.
