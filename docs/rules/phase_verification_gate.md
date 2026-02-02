# Phase Verification Gatekeeper

## 1. Protocol
Before moving from **Phase X** to **Phase Y**, the Developer and AI must complete this checklist.
**Rule**: If any item is unchecked, the Phase is NOT done.

## 2. General Integrity Check
- [ ] **Clean Console**: No errors in Browser Console or Backend Terminal on startup.
- [ ] **Data Integrity**: Database is seeded/connected and data persists after restart.
- [ ] **Code Quality**: No dead code commented out (unless explicitly marked `// TODO`).
- [ ] **Plan Status**: `implementation_plan.md` is updated.

## 3. Phase-Specific Gates

### Gate: Phase 1 -> Phase 2 (Foundation)
- [ ] Backend runs on `http://localhost:5xxx`.
- [ ] Swagger UI loads.
- [ ] Frontend runs on `http://localhost:5173`.
- [ ] Docker containers (SQL, Redis) are healthy.

### Gate: Phase 2 -> Phase 3 (Core Domain)
- [ ] Product API returns JSON data.
- [ ] Frontend displays dynamic products (not hardcoded HTML).
- [ ] User can Login/Register (JWT received).
- [ ] Images load correctly (No broken links).

### Gate: Phase 3 -> Phase 4 (Sales)
- [ ] Add to Cart persists on refresh.
- [ ] Checkout API creates an Order record in DB.
- [ ] Mock Payment flow completes without crashing.

### Gate: Phase 4 -> Phase 5 (Production)
- [ ] Build succeeds in Docker (`docker build`).
- [ ] No secrets in `appsettings.json`.
- [ ] Lighthouse Performance Score > 80.

## 4. Sign-off
**Current Phase**: Phase 2 (In Progress)
**Next Phase**: Phase 3
**Approved By**: [User Name]
**Date**: [YYYY-MM-DD]
