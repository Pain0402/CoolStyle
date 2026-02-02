# Hướng Dẫn Cấu Hình Cloudflare Tunnel

Để website của bạn có thể truy cập từ internet (có HTTPS, có domain riêng) mà không cần mở port trên modem hay thuê IP tĩnh, chúng ta dùng **Cloudflare Tunnel**.

## Bước 1: Chuẩn bị Domain
1.  Đăng ký một Domain (ví dụ: `myshop.com`) và thêm nó vào Cloudflare.
2.  Vào Dashboard của Cloudflare > **Zero Trust**.

## Bước 2: Tạo Tunnel
1.  Trong Zero Trust Dashboard, chọn **Networks** > **Tunnels**.
2.  Nhấn **Create a Tunnel**.
3.  Đặt tên (ví dụ: `fashion-ecommerce-prod`) -> Save.
4.  Chọn môi trường **Docker**.
5.  Bạn sẽ thấy một câu lệnh chạy docker, trong đó có đoạn token: `... --token eyJhIjoi...`.
6.  Copy chuỗi Token dài ngoằng đó (bắt đầu bằng `ey...`).

## Bước 3: Cấu hình Dự án
1.  Tại thư mục dự án `FashionEcommerce`, tạo file `.env` (nếu chưa có).
2.  Thêm dòng sau vào file `.env`:
    ```env
    TUNNEL_TOKEN=eyJhIt................................
    CLOUDFLARE_DOMAIN=shop.myshop.com
    ```
    *   `TUNNEL_TOKEN`: Token bạn vừa copy.
    *   `CLOUDFLARE_DOMAIN`: Domain chính xác mà khách hàng sẽ truy cập (ví dụ: `shop.myshop.com`). Điều này quan trọng để Backend cho phép trình duyệt truy cập (CORS).

## Bước 4: Cấu hình Public Hostname (Trong Cloudflare Dashboard)
Sau khi có Token, bạn cần trỏ Domain vào các Container.
Vẫn ở trang cấu hình Tunnel, chọn tab **Public Hostname** -> **Add a public hostname**.

### 4.1. Trỏ Frontend (Web Bán Hàng)
*   **Subdomain**: `shop` (hoặc để trống nếu muốn dùng domain chính).
*   **Domain**: `myshop.com`.
*   **Service**:
    *   Type: `HTTP`.
    *   URL: `frontend:80` (Lưu ý: dùng tên service `frontend`, không dùng localhost).

### 4.2. Trỏ Backend (API)
*   **Subdomain**: `api`.
*   **Domain**: `myshop.com`.
*   **Service**:
    *   Type: `HTTP`.
    *   URL: `api:8080`.

## Bước 5: Build lại Frontend (Quan trọng!)
Frontend VueJS chạy trên trình duyệt của khách hàng, nên nó cần biết địa chỉ API thật (Domain public) thay vì `localhost`.

1.  Mở file `src/Frontend/.env.production`.
2.  Sửa dòng `VITE_API_URL`:
    ```env
    VITE_API_URL=https://api.myshop.com/api
    ```
    *(Thay `api.myshop.com` bằng domain bạn vừa cấu hình ở bước 4.2)*

## Bước 6: Deploy lại
Chạy lại lệnh deploy để cập nhật thay đổi:
```powershell
.\deploy.ps1
```

Lúc này, Cloudflare Tunnel sẽ kết nối.
- Truy cập `https://shop.myshop.com` -> Ra web mua hàng.
- Web này sẽ gọi API tại `https://api.myshop.com` -> Backend xử lý -> Trả về kết quả.
