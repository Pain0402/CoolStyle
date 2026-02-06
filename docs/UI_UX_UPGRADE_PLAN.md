# 🎨 CoolStyle 2025 UI/UX Upgrade Plan

> "Transforming a basic e-commerce site into a visual masterpiece."

## 1. Design Philosophy: "Neo-Glass & Dynamic Motion"
We will move away from standard flat design and adopt 2025 trends:
-   **Glassmorphism 2.0:** Subtle, frosted glass effects with noise textures, not just blur.
-   **Micro-Interactions:** Buttons that breathe, cards that lift, images that parallax.
-   **Bento Grid Layouts:** Asymmetrical but organized content presentation.
-   **Typography:** Big, bold, editorial fonts (Outfit/Clash Display) mixed with clean sans-serif (Inter).
-   **Dark Mode First:** Deep, rich background colors (#0a0a0a) with neon accents.

## 2. New Page Architecture
We will expand the current 2-page app to a full suite:

### A. Current Pages (To Upgrade)
1.  **Home Page (`/`)**:
    *   *Current:* Basic list.
    *   *Upgrade:* Hero slider (video/3D), "Trending Now" bento grid, Infinite scroll marquee of brands.
2.  **Admin Dashboard (`/admin`)**:
    *   *Current:* Functional.
    *   *Upgrade:* Analytics charts, real-time stock updates, drag-and-drop management.

### B. New Pages (To Create)
3.  **Shop / Catalog (`/shop`)**:
    *   Advanced filtering (sidebar with accordion).
    *   Sort by Price, Newest, Best Selling.
    *   Pagination / Infinite Scroll.
4.  **Product Detail Page (`/product/:slug`)**:
    *   Sticky image gallery.
    *   "Add to Cart" sticky bar on mobile.
    *   Related products carousel.
    *   Reviews section with breakdown.
5.  **Cart & Checkout (`/cart`)**:
    *   Slide-out drawer for "Quick Cart".
    *   Full checkout page with step-by-step progress.
6.  **User Profile (`/profile`)**:
    *   Order history visually presented.
    *   Wishlist.
7.  **Auth Pages (`/login`, `/register`)**:
    *   Split screen design (Image + Form).

## 3. Component Upgrades (The "Wow" Factor)

### 💎 The "Hero" Product Card
*   **Hover:** Image swap (Front -> Back/Model).
*   **Quick Actions:** "Quick View" eye icon appears on hover.
*   **Badges:** "New", "-20%", "Sold Out" with pulsing effects.
*   **Loading:** Skeleton screens (shimmer effect) instead of spinners.

## 4. Implementation Strategy

### Phase 1: Foundation (The "Vibe" Check) 👈 START HERE
*   [ ] Setup `2025-design-system` (Tailwind Config).
*   [ ] Define Color Palette (Neon Cyan + Deep Violet + Noir Black).
*   [ ] Create `animations.css` for custom keyframes.

### Phase 2: Core Components
*   [ ] Build `ProductCard.vue` (The Atomic Unit).
*   [ ] Build `Navbar.vue` (Glassmorphism + Mega Menu).
*   [ ] Build `Footer.vue` (Big typography).

### Phase 3: Page Construction
*   [ ] Implement **Home Page** (Hero + Bento).
*   [ ] Implement **Shop Page** (Grid + Filters).
*   [ ] Implement **Product Detail** (Gallery + Info).

### Phase 4: Functionality & Flow
*   [ ] Cart State Management (Pinia).
*   [ ] Checkout Flow.

## 5. Technology Stack
*   **Vue 3 (Composition API)**
*   **Tailwind CSS v4** (Latest)
*   **VueUse/Motion** (For animations)
*   **Lucide Icons** (Clean vector icons)
