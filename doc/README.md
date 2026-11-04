# SỔ TAY TRA CỨU NHANH ASP.NET CORE MVC (KHI CẦN GÌ THÌ DÙNG GÌ)

Tài liệu này tổng hợp toàn bộ các kỹ thuật, cú pháp và công cụ hay dùng nhất trong lập trình Web nâng cao ASP.NET Core MVC theo định dạng tra cứu: **"Khi cần [Làm việc gì] $\rightarrow$ Dùng [Kỹ thuật / Cú pháp gì]"**.

---

## 📌 MỤC LỤC TRA CỨU THEO CHỦ ĐỀ

| STT | File tài liệu | Nội dung tra cứu chính |
| :---: | :--- | :--- |
| **01** | [01_program_routing_middleware.md](./01_program_routing_middleware.md) | Cấu hình `Program.cs`, Chuỗi kết nối DB, Routing, Areas, Session, File tĩnh |
| **02** | [02_entity_framework_core_linq.md](./02_entity_framework_core_linq.md) | Data Annotations (Model), Khóa chính/ngoại, Migration, Truy vấn LINQ thực chiến |
| **03** | [03_controllers_actions_crud.md](./03_controllers_actions_crud.md) | Dependency Injection, CRUD Actions, ModelState, Upload File/Ảnh, JSON/AJAX |
| **04** | [04_razor_views_taghelpers.md](./04_razor_views_taghelpers.md) | Cú pháp Razor, Tag Helpers form/link, Dropdown, Partial View, Layout, Flash Message |
| **05** | [05_auth_session_phanquyen.md](./05_auth_session_phanquyen.md) | Phân quyền `[Authorize]`, Kiểm tra Role, Quản lý Session, Lấy User hiện tại |
| **06** | [06_bootstrap_hay_dung.md](./06_bootstrap_hay_dung.md) | Các Class Bootstrap hay dùng: Grid responsive, Form, Table, Card, Button, Modal, Badge, Pagination |

---

## 🚀 TÓM TẮT TRA CỨU CẤP TỐC (QUICK LOOKUP)

* **Muốn định nghĩa khóa chính tự tăng:** $\rightarrow$ Dùng `[Key]`
* **Muốn liên kết bảng 1-N (Khóa ngoại):** $\rightarrow$ Dùng `[ForeignKey("...")]` + Navigation property
* **Muốn lấy dữ liệu bảng quan hệ tránh bị null:** $\rightarrow$ Dùng `.Include(x => x.NavigationProp)`
* **Muốn phân trang dữ liệu:** $\rightarrow$ Dùng `.Skip((page - 1) * pageSize).Take(pageSize)`
* **Muốn tiêm DbContext vào Controller:** $\rightarrow$ Dùng Constructor Dependency Injection (`ctor`)
* **Muốn form HTML tự liên kết với Model:** $\rightarrow$ Dùng Tag Helper `asp-for="TenThuocTinh"`
* **Muốn upload file/ảnh qua form:** $\rightarrow$ Dùng `enctype="multipart/form-data"` ở View và `IFormFile` ở Controller
* **Muốn hiển thị Dropdown danh mục:** $\rightarrow$ Dùng `SelectList` ở Controller và `<select asp-items="...">` ở View
* **Muốn kiểm tra người dùng nhập đúng/sai quy tắc:** $\rightarrow$ Dùng `if (!ModelState.IsValid)`
* **Muốn bắt buộc đăng nhập mới được vào Action:** $\rightarrow$ Dùng `[Authorize]`
* **Muốn chỉ cho tài khoản Admin truy cập:** $\rightarrow$ Dùng `[Authorize(Roles = "Admin")]`
* **Muốn lưu dữ liệu tạm giữa các trang:** $\rightarrow$ Dùng `HttpContext.Session`
* **Muốn gửi thông báo 1 lần sau khi redirect:** $\rightarrow$ Dùng `TempData["ThongBao"]`
* **Muốn bảng dữ liệu kẻ sọc, có hover và cuộn mobile:** $\rightarrow$ Dùng `table table-striped table-hover align-middle` bọc trong `table-responsive`
* **Muốn nút bấm cạnh nhau dạt đều 2 bên:** $\rightarrow$ Dùng `d-flex justify-content-between align-items-center`
