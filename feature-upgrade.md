# Cập nhật Tính năng Thương mại điện tử (Payment, Dashboard, Reviews)

## Goal
Tích hợp thanh toán VNPAY/MoMo, xây dựng hệ thống báo cáo doanh thu cho Admin và tính năng đánh giá sản phẩm (Review) trên cả Backend (.NET 8) và Frontend (Vue 3).

## Tasks
- [x] Task 1: Thiết kế Database & Schema -> Verify: Cập nhật Entities (`Order` thêm cổng thanh toán/trạng thái thanh toán, tạo Entity `ProductReview`), chạy migration thành công.
- [x] Task 2: Backend - API Đánh giá sản phẩm -> Verify: Thêm endpoint POST `/api/products/{id}/reviews` và GET `/api/products/{id}/reviews` test bằng Swagger.
- [x] Task 3: Backend - Tích hợp VNPAY/MoMo -> Verify: Thêm endpoint Redirect tới cổng thanh toán và endpoint webhook IPN xử lý kết quả trả về, update trạng thái thanh toán đơn hàng.
- [x] Task 4: Backend - API Báo cáo Doanh thu Admin -> Verify: Endpoint GET `/api/admin/dashboard/revenue` trả về dữ liệu doanh thu theo ngày/tháng/vùng.
- [ ] Task 5: Frontend - Giao diện Đánh giá Sản phẩm -> Verify: Trên trang chi tiết sản phẩm, hiển thị sao đánh giá, danh sách review và form gửi review.
- [ ] Task 6: Frontend - Tích hợp checkout thanh toán -> Verify: Thêm lựa chọn phương thức thanh toán tại giỏ hàng, chuyển hướng sang page VNPAY và xử lý màn hình kết quả thanh toán.
- [ ] Task 7: Frontend - Dashboard Admin Báo cáo -> Verify: Trang Admin Dashboard hiển thị biểu đồ thống kê doanh thu (dùng Chart.js/Recharts), số lượng đơn hàng và đánh giá.

## Done When
- [ ] Người dùng hoàn tất quá trình thanh toán qua VNPAY, đơn hàng được cập nhật "Đã thanh toán".
- [ ] Admin truy cập Dashboard xem được tổng quan doanh thu, số đơn ngay trên hình ảnh trực quan.
- [ ] Người dùng truy cập sản phẩm xem được các nhận xét kèm số sao trung bình trên Giao diện sản phẩm.
