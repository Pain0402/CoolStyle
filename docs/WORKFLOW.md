# Quy trình Phát triển tính năng (Dev Workflow)

Đây là quy trình chuẩn để bạn phát triển tính năng mới từ môi trường Dev và đưa lên Prod.

## 1. Môi trường Development (Dev)
Ở môi trường này, bạn chạy code trực tiếp để debug và sửa lỗi nhanh chóng.

### Bước 1: Khởi động Hạ tầng Dev (Database, Redis)
Nếu bạn chưa bật database, hãy chạy lệnh này (chỉ cần chạy 1 lần khi bắt đầu làm việc):
```powershell
# Chạy file docker-compose.yml (File này chỉ chứa SQL, Redis, Seq - KHÔNG chứa App)
docker compose up -d
```
*Lưu ý: Đừng nhầm với `docker-compose.prod.yml` (Prod).*

### Bước 2: Chạy Backend (API)
Mở một cửa sổ Terminal (hoặc dùng Visual Studio):
```powershell
cd src/Backend
dotnet watch run --project FashionEcommerce.API/FashionEcommerce.API.csproj
```
*   `dotnet watch`: Chế độ này giúp Backend tự khởi động lại mỗi khi bạn sửa code C#.
*   API sẽ chạy tại: `http://localhost:5058` (hoặc port bạn cấu hình).

### Bước 3: Chạy Frontend (Web)
Mở một cửa sổ Terminal khác:
```powershell
cd src/Frontend
npm run dev
```
*   Web sẽ chạy tại: `http://localhost:5173`.
*   Mọi thay đổi UI sẽ cập nhật tức thì (Hot Reload).

### Bước 4: Code & Test
Bây giờ bạn code tính năng mới, ví dụ "Thêm màn hình Quản lý Sản phẩm".
1.  Sửa Backend -> Tạo API mới.
2.  Sửa Frontend -> Tạo trang Vue mới gọi API đó.
3.  Test trên `http://localhost:5173`.

---

## 2. Đưa lên Production (Cập nhật)
Sau khi bạn đã test ngon lành ở môi trường Dev, giờ là lúc đưa nó sang môi trường "Thật" (Docker Production).

### Bước 1: Kiểm tra config (Nếu cần)
Nếu bạn có thêm Environment Variable mới (ví dụ Key thanh toán), hãy nhớ thêm vào `docker-compose.prod.yml`.

### Bước 2: Chạy Script Update
Chạy lệnh duy nhất:
```powershell
.\update.ps1
```

### Điều gì sẽ xảy ra?
1.  Script sẽ build lại Backend (Release mode).
2.  Script sẽ build lại Frontend (thành file tĩnh HTML/CSS).
3.  Docker sẽ thay thế container cũ bằng container mới chứa code vừa sửa.
4.  **Database Prod được giữ nguyên** (không bị mất dữ liệu).

### Bước 3: Tận hưởng kết quả
Truy cập `http://localhost` (Port 80) để xem tính năng mới trên môi trường Production.

---

## Tóm tắt lệnh

| Hành động | Lệnh |
| :--- | :--- |
| **Bắt đầu ngày làm việc** | `docker compose up -d` (Dev Database) |
| **Code Backend** | `dotnet watch run ...` |
| **Code Frontend** | `npm run dev` |
| **Cập nhật lên Web chính** | `.\update.ps1` |
