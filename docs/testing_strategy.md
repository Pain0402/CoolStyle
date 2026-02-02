# Testing Strategy

## 1. Overview
We follow the **Testing Pyramid**:
1. **Unit Tests (High Volume)**: Domain logic, Validation rules.
2. **Integration Tests (Medium Volume)**: API Endpoints + Database.
3. **E2E Tests (Low Volume)**: Critical User Flows (Checkout).

## 2. Backend Testing (.NET)
- **Framework**: xUnit.
- **Mocking**: Moq.
- **Assertions**: FluentAssertions.
- **Integration**: `Microsoft.AspNetCore.Mvc.Testing` (TestServer).

### Naming Convention
`MethodName_StateUnderTesting_ExpectedBehavior`
- Example: `AddToCart_ItemOutOfStock_ThrowsException`

## 3. Frontend Testing (Vue)
- **Unit**: Vitest + Vue Test Utils.
- **E2E**: Playwright (Phase 5).

## 4. Manual Testing Checklist (Before Commit)
- [ ] API returns 200 OK.
- [ ] No Console Errors.
- [ ] Layout is responsive on Mobile view (DevTools).
