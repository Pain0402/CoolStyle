# Hướng Dẫn Deploy (Triển Khai) - Fashion Ecommerce

Tài liệu này hướng dẫn cách đưa ứng dụng từ môi trường Code (Development) lên môi trường Chạy Thật (Production) sử dụng Docker.

## 1. Kiến Trúc (Architecture)

Hệ thống được đóng gói hoàn toàn trong các Container, giúp đảm bảo "chạy được ở máy tôi thì cũng chạy được ở server".

*   **Frontend (Container: `fashion_frontend_prod`)**: 
    *   Chạy VueJS App đã được build thành file tĩnh (HTML/CSS/JS).
    *   Sử dụng **Nginx** làm Web Server siêu nhẹ để phục vụ các file này.
    *   Chạy ở Port **80**.
*   **Backend (Container: `fashion_api_prod`)**:
    *   Chạy .NET 8 API ở chế độ Release (Tối ưu hiệu năng).
    *   Chạy ở Port **8080** (Internal) nhưng được map ra ngoài để Frontend gọi.
*   **Database (Container: `fashion_sql_prod`)**:
    *   SQL Server 2022 lưu trữ dữ liệu bền vững (Volume mapping).
*   **Caching (Container: `fashion_redis_prod`)**:
    *   Redis dùng để cache dữ liệu, giảm tải cho SQL (Phase nâng cao).
*   **Logging (Container: `fashion_seq_prod`)**:
    *   Seq server để tập trung log từ Backend, giúp debug lỗi trên Prod dễ dàng.

## 2. Quy trình Deploy (1-Click)

Chúng tôi đã chuẩn bị script để tự động hóa toàn bộ quy trình.

### Trên Windows (PowerShell)
Chỉ cần chạy file script tại thư mục gốc dự án:
```powershell
.\deploy.ps1
```

### Chạy Thủ Công (Manual)
Nếu bạn muốn hiểu từng bước hoặc chạy trên Linux/Mac:

1.  **Stop & Clean**: Dọn dẹp phiên bản cũ (nếu có).
    ```bash
    docker compose -f docker-compose.prod.yml down -v
    ```
    *(Lưu ý: `-v` sẽ xóa sạch database cũ. Nếu muốn giữ dữ liệu, bỏ cờ `-v` đi)*

2.  **Build & Up**: Tự động build lại code mới nhất và khởi động.
    ```bash
    docker compose -f docker-compose.prod.yml up -d --build
    ```

## 3. Cách deploy lên VPS (Server thật)

Để đưa dự án này lên một VPS (ví dụ Ubuntu Server của DigitalOcean, AWS, Google Cloud):

1.  **Chuẩn bị Server**:
    *   Cài đặt Docker và Docker Compose trên VPS.
2.  **Copy các file cần thiết**:
    *   Bạn không cần copy toàn bộ code. Chỉ cần copy:
        *   `src/Backend/` (Source Backend)
        *   `src/Frontend/` (Source Frontend)
        *   `docker-compose.prod.yml`
        *   `deploy.ps1` (hoặc tạo file .sh tương tự)
3.  **Chạy lệnh**:
    *   SSH vào VPS.
    *   Chạy lệnh `docker compose -f docker-compose.prod.yml up -d --build`.
4.  **Cấu hình Domain (Optional)**:
    *   Trỏ domain về IP của VPS.
    *   Hiện tại Nginx ở Frontend đang đón Port 80, nên truy cập domain là vào được ngay.

## 4. Các sự cố thường gặp (Troubleshooting)

*   **Lỗi "Port already allocated"**:
    *   Kiểm tra xem máy bạn có phần mềm nào đang chiếm Port 80 (Skype, IIS) hoặc 1433 (SQL Server cài sẵn) không. Tắt chúng đi hoặc đổi port mapping trong file `docker-compose.prod.yml`.
*   **Lỗi Kết nối Database (Connection Refused)**:
    *   Đợi khoảng 10-20 giây sau khi khởi động. SQL Server khởi động chậm hơn API một chút. API có cơ chế thử lại (Retry), nhưng đôi khi cần restart container API.
*   **Frontend không gọi được API**:
    *   Kiểm tra tab Network (F12). Nếu thấy lỗi CORS (đỏ), đảm bảo Backend đã add `http://domain-cua-ban` vào file `Program.cs`.

## 5. Môi trường (Environment)

Các biến môi trường quan trọng nằm trong `docker-compose.prod.yml`:
*   `JwtSettings__Secret`: Khóa bí mật mã hóa Token. **PHẢI ĐỔI** khi lên Production thật.
*   `SA_PASSWORD`: Mật khẩu Database.

---
> **Chúc mừng!** Bạn đã sở hữu một quy trình deploy chuyên nghiệp theo chuẩn CI/CD cơ bản.
