# CoolStyle - Kế hoạch Cải tiến Hệ thống

> **Ngày phân tích**: 28/03/2026  
> **Phiên bản hiện tại**: MVP Complete (last commit: 25/02/2026)  
> **Mục tiêu**: Nâng cấp từ MVP lên Production-Ready

---

## Tổng quan

Sau khi phân tích toàn bộ codebase (Backend .NET 8 + Frontend Vue 3), hệ thống hiện tại đã hoàn thành MVP với các tính năng cơ bản. Tuy nhiên còn nhiều vấn đề nghiêm trọng về bảo mật, tính năng còn thiếu và chất lượng code cần được giải quyết trước khi có thể coi là production-ready.

---

## 🔴 NHÓM 1: Thiếu hoàn toàn (Critical Missing)

### 1.1. Order History cho User

**Vấn đề:**  
Tab "Mission Logs (Orders)" trong `UserProfileView.vue` chỉ là placeholder hardcode, không có dữ liệu thực:

```html
<!-- UserProfileView.vue - dòng ~180 -->
<div v-if="activeTab === 'orders'" class="animate-fade-in-up">
    <h2 class="text-2xl font-bold mb-6 font-display">Mission Logs</h2>
    <div class="text-center py-20 text-gray-500 bg-white/5 rounded-2xl">
        No missions recorded yet.  <!-- hardcoded, không gọi API -->
    </div>
</div>
```

Backend không có endpoint `GET /api/orders/my-orders` cho user xem lịch sử đơn hàng của chính mình. `OrdersController` chỉ có endpoint admin.

**Cần làm:**
- Backend: Thêm `GET /api/orders/my-orders` — trả về danh sách đơn hàng của user đang đăng nhập
- Backend: Thêm `GET /api/orders/{id}` — xem chi tiết 1 đơn hàng
- Frontend: Implement tab Orders trong `UserProfileView.vue` với timeline trạng thái
- Frontend: Hiển thị danh sách đơn, trạng thái, tổng tiền, sản phẩm đã mua

---

### 1.2. RBAC Backend thực sự

**Vấn đề:**  
Tất cả admin endpoints đều bị comment `[Authorize(Roles = "Admin")]`:

```csharp
// OrdersController.cs
[HttpGet("admin")]
// [Authorize(Roles = "Admin")] // Uncomment when Roles are set up
public async Task<IActionResult> GetAllOrders() { ... }

[HttpPut("admin/{id}/status")]
// [Authorize(Roles = "Admin")] // Uncomment when Roles are set up
public async Task<IActionResult> UpdateOrderStatus(...) { ... }

// DashboardController.cs
[Route("api/admin/dashboard")]
// [Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase { ... }
```

Bất kỳ ai cũng có thể gọi `GET /api/orders/admin`, `PUT /api/orders/admin/{id}/status`, `GET /api/admin/dashboard/revenue` mà không cần đăng nhập. Đây là lỗ hổng bảo mật nghiêm trọng nhất trong hệ thống.

Ngoài ra, logic phân quyền Admin hiện tại dựa trên email:
```csharp
// AuthService.cs
Role = user.Email!.Contains("admin") ? "Admin" : "User"
```
Không dùng ASP.NET Identity Roles thực sự, nên `[Authorize(Roles = "Admin")]` sẽ không hoạt động ngay cả khi bỏ comment.

**Cần làm:**
- Seed Admin role vào Identity khi startup
- Gán role "Admin" cho user có email chứa "admin" trong `DataSeeder`
- Thêm role claim vào JWT token trong `AuthService.GenerateJwtToken()`
- Bỏ comment tất cả `[Authorize(Roles = "Admin")]` trên các controller admin
- Kiểm tra lại navigation guard ở frontend (`router/index.ts`)

---

### 1.3. Refresh Token trên Frontend

**Vấn đề:**  
Backend đã implement Refresh Token (commit 26/02/2026) nhưng Frontend hoàn toàn bỏ qua:

```typescript
// auth.ts - setAuth() không lưu refreshToken
const setAuth = (newToken: string, newUser: User) => {
    token.value = newToken;
    // refreshToken không được lưu ở đây
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify(newUser));
};

// api.ts - không có interceptor xử lý 401
apiClient.interceptors.response.use(
    (response) => { ... },
    (error) => {
        // Chỉ log error, không tự động refresh token
        console.error('[API Error]:', message);
        return Promise.reject(error);
    }
);
```

Khi access token hết hạn (60 phút), user bị đăng xuất đột ngột mà không có thông báo.

**Cần làm:**
- Lưu `refreshToken` vào `localStorage` trong `setAuth()`
- Thêm axios response interceptor: khi nhận 401, tự động gọi `POST /api/auth/refresh-token`
- Nếu refresh thành công → retry request gốc với token mới
- Nếu refresh thất bại → logout và redirect về `/login`
- Xử lý race condition khi nhiều request cùng lúc nhận 401

---

### 1.4. Pagination API

**Vấn đề:**  
`GET /api/products` trả về toàn bộ ~300+ sản phẩm một lần:

```csharp
// ProductService.cs
public async Task<List<ProductDto>> GetProductsAsync(string? categorySlug = null)
{
    var query = _context.Products
        .Include(p => p.Category)
        .Include(p => p.Images)
        .Include(p => p.Variants)
        .AsNoTracking()
        .AsQueryable();
    // Không có .Skip() .Take()
    var products = await query.ToListAsync(); // Load tất cả
    return products.Select(...).ToList();
}
```

Frontend nhận hết rồi filter/sort client-side. Với 300+ sản phẩm, response payload lớn, thời gian load chậm, memory usage cao.

**Cần làm:**
- Backend: Thêm `page` (default: 1) và `pageSize` (default: 20) vào query params
- Backend: Response trả về `PagedResult<T>` với `{ items, totalCount, page, pageSize, totalPages }`
- Backend: Thêm filter `minPrice`, `maxPrice`, `sortBy` vào query params (hiện tại filter ở client)
- Frontend: Implement infinite scroll hoặc pagination UI
- Frontend: Gọi API với params thay vì filter client-side

---

### 1.5. Product CRUD cho Admin

**Vấn đề:**  
Sidebar admin có dòng `Sản phẩm (Sắp ra mắt)` nhưng không có implementation:

```html
<!-- AdminDashboard.vue -->
<a href="#" class="flex items-center gap-3 px-4 py-3 text-gray-400 ...">
    <Package :size="20" /> Sản phẩm (Sắp ra mắt)
</a>
```

Admin không thể thêm/sửa/xóa sản phẩm, quản lý stock, upload ảnh qua giao diện.

**Cần làm:**
- Backend: `POST /api/admin/products` — tạo sản phẩm mới
- Backend: `PUT /api/admin/products/{id}` — cập nhật sản phẩm
- Backend: `DELETE /api/admin/products/{id}` — soft delete
- Backend: `PUT /api/admin/products/{id}/variants/{sku}/stock` — cập nhật tồn kho
- Frontend: Trang quản lý sản phẩm với bảng danh sách, form thêm/sửa

---

## 🟡 NHÓM 2: Có nhưng chưa đúng (Broken/Incomplete)

### 2.1. Rating hiển thị hardcode

**Vấn đề:**  
Điểm đánh giá và số lượng review trên trang chi tiết sản phẩm là hardcode:

```html
<!-- ProductDetailView.vue -->
<div class="flex items-center gap-1 text-yellow-400 text-sm bg-yellow-400/10 px-2 py-1 rounded">
    <Star :size="14" fill="currentColor" /> 4.8 (120 reviews)
    <!-- Hardcode, không tính từ DB -->
</div>
```

Trong khi đó, backend đã có `ProductReviews` table và API `GET /api/products/{id}/reviews`.

**Cần làm:**
- Backend: Thêm `AverageRating` và `ReviewCount` vào `ProductDetailDto`
- Backend: Tính toán trong `GetProductBySlugAsync()` từ `ProductReviews`
- Frontend: Hiển thị rating động từ API response
- Frontend: Component `StarRating` tái sử dụng được

---

### 2.2. Stock Validation khi Add to Cart

**Vấn đề:**  
Không kiểm tra tồn kho khi thêm vào giỏ hàng:

```typescript
// ProductDetailView.vue
const addToCart = () => {
    if (!product.value) return;
    const variant = product.value.variants?.find(...);
    cartStore.addToCart(product.value, quantity.value, variant);
    // Không check variant.stock > 0
    // Không check quantity <= variant.stock
    toast.success(`Đã thêm ${quantity.value} sản phẩm vào giỏ!`);
};
```

User có thể thêm sản phẩm hết hàng (`StockQuantity = 0`) vào giỏ, dẫn đến lỗi khi checkout.

**Cần làm:**
- Frontend: Disable nút "Add to Cart" khi `variant.stock === 0`
- Frontend: Giới hạn `quantity` không vượt quá `variant.stock`
- Frontend: Hiển thị badge "Hết hàng" trên variant selector
- Backend: Validate stock trong `CreateOrderAsync()` trước khi tạo đơn

---

### 2.3. Admin Dashboard — Double API Call

**Vấn đề:**  
`AdminDashboard.vue` gọi 2 API riêng biệt và dữ liệu bị overwrite:

```typescript
// AdminDashboard.vue
onMounted(async () => {
    await fetchDashboardStats(); // Gọi /admin/dashboard/revenue → set orders = recentOrders (5 đơn)
    loading.value = false;
    // fetchOrders() không được gọi ở đây nữa
    // Nhưng nếu gọi thì: orders.value = data (tất cả đơn) → overwrite recentOrders
});
```

Hiện tại chỉ hiển thị 5 đơn gần nhất từ dashboard stats, không phải toàn bộ danh sách đơn hàng.

**Cần làm:**
- Tách rõ 2 section: "Stats Overview" (dùng dashboard API) và "Order Management" (dùng orders API)
- Thêm pagination cho bảng đơn hàng trong admin
- Thêm filter theo trạng thái, ngày tháng

---

### 2.4. VNPAY là Mock hoàn toàn

**Vấn đề:**  
Implementation VNPAY không thực sự tích hợp với cổng thanh toán:

```csharp
// OrdersController.cs
[HttpGet("{id}/pay")]
public async Task<IActionResult> GetPaymentUrl(int id)
{
    // Không có VNPAY hash signature
    // Không có merchant code, terminal code
    var returnUrl = $"...vnp_ResponseCode=00"; // Luôn trả về 00 (thành công)
    var mockVnPayUrl = returnUrl; // Tự redirect về luôn
    return Ok(ApiResponse<string>.Success(mockVnPayUrl, "Payment URL generated"));
}
```

Mọi thanh toán VNPAY đều "thành công" ngay lập tức mà không qua cổng thật.

**Cần làm:**
- Đăng ký VNPAY Sandbox account
- Implement đúng VNPAY signature (HMAC-SHA512)
- Thêm `VnPaySettings` vào `appsettings.json`
- Implement `IVnPayService` với `CreatePaymentUrl()` và `ValidateCallback()`
- Xử lý IPN webhook thực sự

---

### 2.5. Search không có Debounce

**Vấn đề:**  
`ShopView.vue` filter client-side theo `searchQuery` nhưng không có debounce:

```typescript
// ShopView.vue - filteredProducts computed
const filteredProducts = computed(() => {
    let p = [...products.value];
    if (searchQuery.value) {
        p = p.filter(prod => 
            prod.name.toLowerCase().includes(searchQuery.value.toLowerCase()) || ...
        );
    }
    // Mỗi keystroke trigger re-compute toàn bộ 300+ sản phẩm
    return p;
});
```

**Cần làm:**
- Thêm debounce 300ms cho search input (dùng `@vueuse/core` `useDebounceFn`)
- Khi có pagination: search gọi API thay vì filter client-side
- Highlight từ khóa tìm kiếm trong kết quả

---

## 🟢 NHÓM 3: Cần cải thiện (Quality & DevOps)

### 3.1. Redis Caching chưa được dùng

**Vấn đề:**  
Redis có trong `docker-compose.yml` và `docker-compose.prod.yml` nhưng không có dòng code nào sử dụng:

```csharp
// Program.cs - không có AddStackExchangeRedisCache()
// ProductService.cs - không có IDistributedCache injection
// Tất cả query đều hit thẳng SQL Server
```

**Cần làm:**
- Thêm `Microsoft.Extensions.Caching.StackExchangeRedis` package
- Register `AddStackExchangeRedisCache()` trong `Program.cs`
- Cache `GET /api/products` với TTL 5 phút
- Cache `GET /api/categories` với TTL 30 phút
- Invalidate cache khi có thay đổi sản phẩm/danh mục

---

### 3.2. Input Validation Backend thiếu

**Vấn đề:**  
Chỉ có `[Required]` annotation cơ bản, không có FluentValidation:

```csharp
// OrderDtos.cs
public class CreateOrderRequest
{
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    // Không validate độ dài, format
    // Không validate Items không rỗng
    // Không validate Quantity > 0
}
```

Có thể tạo order với `Items = []` (giỏ rỗng) hoặc `Quantity = -1`.

**Cần làm:**
- Thêm `FluentValidation.AspNetCore` package
- Viết validators cho `CreateOrderRequest`, `RegisterDto`, `CreateReviewDto`
- Register validators trong `Program.cs`
- Trả về validation errors theo format chuẩn

---

### 3.3. Error Handling Frontend không nhất quán

**Vấn đề:**  
Một số chỗ dùng `toast.error()`, một số chỉ `console.error()`, một số không xử lý:

```typescript
// UserProfileView.vue
} catch (e) {
    console.error(e); // Không thông báo cho user
    toast.error("Failed to load profile data");
}

// WishlistStore.ts
} catch (e) {
    console.error("Wishlist toggle error", e); // Chỉ log, không toast
    return isItemInWishlist;
}
```

`api.ts` interceptor không có global error handling cho các HTTP status codes phổ biến (401, 403, 500).

**Cần làm:**
- Thêm global error handler trong `api.ts` interceptor
- Xử lý 401 → trigger refresh token flow
- Xử lý 403 → toast "Bạn không có quyền thực hiện thao tác này"
- Xử lý 500 → toast "Lỗi hệ thống, vui lòng thử lại"
- Chuẩn hóa error messages

---

### 3.4. SEO / Meta Tags

**Vấn đề:**  
Tất cả trang đều dùng title mặc định, không có meta description động:

```typescript
// ProductDetailView.vue - không có useHead() hay document.title
// Tất cả trang đều hiển thị "Fashion Ecommerce" trong tab browser
```

**Cần làm:**
- Thêm `@vueuse/head` hoặc `@unhead/vue` package
- Set dynamic `<title>` và `<meta description>` cho từng trang
- Thêm Open Graph tags cho product pages (chia sẻ mạng xã hội)
- Thêm canonical URLs

---

### 3.5. GitHub Actions CI/CD

**Vấn đề:**  
Không có `.github/workflows/` — không có automated build/test khi push code. Không có badge "build passing" trên README.

**Cần làm:**
- Tạo `.github/workflows/ci.yml`
- Job 1: Build & Test .NET (dotnet build + dotnet test)
- Job 2: Build Frontend (npm install + npm run build)
- Trigger: push to main, pull request to main
- Thêm build status badge vào README

---

### 3.6. CORS quá rộng

**Vấn đề:**  
```csharp
// Program.cs
policy.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
```

`AllowAnyOrigin()` cho phép bất kỳ domain nào gọi API. Trong production cần restrict về các domain cụ thể.

**Cần làm:**
- Đọc `AllowedOrigins` từ `appsettings.json` (đã có config sẵn)
- Thay `AllowAnyOrigin()` bằng `WithOrigins(allowedOrigins)`
- Giữ `AllowAnyOrigin()` chỉ cho môi trường Development

---

### 3.7. Loading States không đồng nhất

**Vấn đề:**  
- `ShopView.vue`: Có skeleton loading đẹp
- `ProductDetailView.vue`: Chỉ có spinner đơn giản
- `UserProfileView.vue`: Không có loading state cho từng tab
- `AdminDashboard.vue`: Có skeleton nhưng chỉ cho lần load đầu

**Cần làm:**
- Tạo component `SkeletonLoader.vue` tái sử dụng
- Áp dụng nhất quán cho tất cả views
- Thêm loading state cho các action buttons (submit, update)

---

## 📊 Bảng Tổng hợp & Ưu tiên

| # | Vấn đề | Nhóm | Độ khó | Impact | Sprint |
|---|--------|------|--------|--------|--------|
| 1 | RBAC Backend (bỏ comment + seed roles) | 🔴 Critical | Thấp | Bảo mật | 1 |
| 2 | Refresh Token Frontend (interceptor + store) | 🔴 Critical | Trung bình | Auth UX | 1 |
| 3 | Order History API + UI | 🔴 Critical | Trung bình | Core Feature | 1 |
| 4 | Stock Validation Cart | 🟡 Broken | Thấp | UX/Logic | 1 |
| 5 | Rating tính từ DB | 🟡 Broken | Thấp | Data Integrity | 2 |
| 6 | Pagination API + UI | 🔴 Critical | Trung bình | Performance | 2 |
| 7 | Redis Caching | 🟢 Quality | Trung bình | Performance | 2 |
| 8 | FluentValidation Backend | 🟢 Quality | Thấp | Robustness | 2 |
| 9 | CORS restrict | 🟢 Quality | Thấp | Bảo mật | 2 |
| 10 | GitHub Actions CI | 🟢 Quality | Thấp | DevOps | 3 |
| 11 | Admin Dashboard fix | 🟡 Broken | Thấp | Admin UX | 3 |
| 12 | Search Debounce | 🟡 Broken | Thấp | Performance | 3 |
| 13 | Error Handling chuẩn hóa | 🟢 Quality | Trung bình | UX | 3 |
| 14 | SEO Meta Tags | 🟢 Quality | Thấp | SEO | 3 |
| 15 | Product CRUD Admin | 🔴 Critical | Cao | Admin Feature | 4 |
| 16 | VNPAY thực sự | 🟡 Broken | Cao | Payment | 4 |
| 17 | Loading States đồng nhất | 🟢 Quality | Thấp | UX | 4 |

---

## 🗓️ Lộ trình thực hiện

### Sprint 1 (27/02 - 07/03/2026) — Security & Core Auth
- RBAC Backend: Seed roles, gán role vào JWT, bỏ comment Authorize
- Refresh Token Frontend: Lưu token, axios interceptor 401
- Order History: API endpoint + UI tab

### Sprint 2 (08/03 - 18/03/2026) — Performance & Data
- Pagination: API + Frontend UI
- Redis Caching: Product catalog
- Rating từ DB: Tính toán và hiển thị động
- FluentValidation + CORS fix
- Stock validation

### Sprint 3 (19/03 - 25/03/2026) — Quality & DevOps
- GitHub Actions CI workflow
- Admin Dashboard fix
- Search debounce
- Error handling chuẩn hóa
- SEO meta tags

### Sprint 4 (26/03 - 28/03/2026) — Advanced Features
- Product CRUD Admin UI
- VNPAY Sandbox thực sự
- Loading states đồng nhất
- Final polish & documentation

---

*Tài liệu này được tạo dựa trên phân tích code thực tế ngày 28/03/2026.*  
*Cập nhật khi có thay đổi lớn trong architecture hoặc requirements.*
