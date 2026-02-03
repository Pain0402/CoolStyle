# Implementation Plan v2: CoolStyle Professional Edition 🚀

> **Goal**: Từ MVP đơn giản -> Hệ thống E-commerce chuyên nghiệp, giao diện Premium, dữ liệu phong phú.
> **Philosophy**: Mobile First, Animation-rich, Glassmorphism 2.0.

## Phase 6: Visual Upgrade & "Wow" Factor (Frontend First) (Completed)
**Mục tiêu:** Nâng cấp trải nghiệm nhìn (Visual) trước, làm cho web sống động ngay lập tức.
- [x] **6.1. UI & Motion Framework Setup**:
  - [x] Cài đặt thư viện Animation (AOS, VueUse Motion).
  - [x] Định nghĩa lại `style.css`: Bảng màu Premium hơn, Glassmorphism cấp độ 2.
- [x] **6.2. Page Expansion**:
  - [x] **Landing Page**: Thiết kế lại Home với Hero Banner 3D, Featured Categories.
- [ ] **6.3. Advanced Effects Integration**:
  - [ ] Thêm hiệu ứng Tilt (nghiêng 3D) cho Product Card khi hover.
  - [x] Page Transition: Hiệu ứng chuyển trang mượt mà (Fade).

## Phase 7: Data Richness (Content Strategy) (In Progress)
**Mục tiêu:** Thay máu toàn bộ dữ liệu mẫu bằng dữ liệu "thật".
- [ ] **7.1. Cloudinary Integration**:
  - [ ] Thiết lập Cloudinary để host ảnh chất lượng cao.
  - [ ] Xây dựng Component `AppImage` tự động tối ưu hóa size ảnh.
- [x] **7.2. Data Seeder v2 (The Big Data)**:
  - [x] Refactor `DataSeeder.cs` với danh mục đa dạng (Nam, Nữ, Phụ kiện).
  - [x] Nạp dữ liệu 50+ sản phẩm với ảnh Unsplash chất lượng cao.

## Phase 8: Advanced Product Logic (Backend Heavy)
**Mục tiêu:** Xử lý các logic phức tạp của thương mại điện tử.
- [ ] **8.1. Variant System (Size/Color)**:
  - [ ] Cập nhật DB Schema: `Product` -> `ProductVariant` (Mỗi size/màu là 1 dòng riêng có tồn kho riêng).
  - [ ] Cập nhật API `GetProductDetail` để trả về các biến thể.
- [ ] **8.2. User Features**:
  - [ ] Trang Profile (Hồ sơ cá nhân).
  - [ ] Quản lý Sổ địa chỉ (Address Book).
  - [ ] Wishlist (Sản phẩm yêu thích).

## Phase 9: Real Transactions (Operations)
**Mục tiêu:** Tiền thật, Đơn hàng thật.
- [ ] **9.1. Payment Gateway (VNPay)**:
  - [ ] Đăng ký Sandbox VNPay.
  - [ ] Tích hợp luồng Redirect thanh toán.
  - [ ] Xử lý IPN (Instant Payment Notification) để cập nhật trạng thái đơn hàng tự động.
- [ ] **9.2. Real Email System**:
  - [ ] Gửi email chào mừng và xác nhận đơn hàng qua SMTP/SendGrid.

## Phase 10: Security & Final Polish
- [ ] **10.1. RBAC**: Phân quyền Admin chặt chẽ.
- [ ] **10.2. SEO Optimization**: Meta tags động cho từng sản phẩm.
- [ ] **10.3. Performance Tuning**: Lazy load ảnh, Cache Redis.
