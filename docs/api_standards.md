# API Guidelines & Standards

## 1. JSend Specification
We adopt a modified **JSend** standard to ensure consistent JSON responses.

### 1.1. Response Format
Every API response must follow this envelope:

```json
{
  "status": "success" | "fail" | "error",
  "data": { ... } | null,
  "message": "Human readable message (mostly for error/fail)",
  "code": "INTERNAL_CODE_123" // Optional custom error code
}
```

- **Success** (HTTP 200-299):
  ```json
  {
    "status": "success",
    "data": {
      "product": { "id": 1, "name": "T-Shirt" }
    }
  }
  ```
- **Fail** (HTTP 400-499) - Validation errors or invalid state:
  ```json
  {
    "status": "fail",
    "data": {
      "email": "Email format is invalid",
      "password": "Too short"
    },
    "message": "Validation failed"
  }
  ```
- **Error** (HTTP 500) - Server crash/exception:
  ```json
  {
    "status": "error",
    "message": "Database connection failed",
    "code": "DB_CONN_ERR",
    "data": null // Stack trace only in Dev
  }
  ```

## 2. URL Naming Conventions
- **Nouns, not verbs**: `GET /products` (Correct), `GET /getProducts` (Wrong).
- **Plural resources**: `GET /users`, `GET /users/{id}`.
- **Hyphens for spacing**: `/product-categories` (Correct), `/productCategories` (Avoid).
- **Filtering/Pagination**:
  `GET /products?page=1&limit=20&sort=-createdAt`

## 3. Swagger / OpenAPI
- **Library**: `Swashbuckle.AspNetCore`.
- **Annotations**:
  - Use `[ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]` to explicitly document return types.
  - Document all error codes.
  - Enable **XML Comments** so summary/remarks in C# code show up in Swagger UI.

## 4. HTTP Methods & Status Codes
- `GET`: 200 OK.
- `POST`: 201 Created (return created resource in `data`).
- `PUT`: 200 OK (return updated resource) or 204 No Content.
- `PATCH`: 200 OK (return updated resource).
- `DELETE`: 204 No Content.

## 5. Security Headers
All APIs must support:
- `Authorization: Bearer <token>`
- `X-Correlation-ID`: For request tracking in logs.
