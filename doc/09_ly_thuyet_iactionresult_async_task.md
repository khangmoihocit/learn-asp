# 09. GIẢI THÍCH LÝ THUYẾT: IACTIONRESULT, ASYNC/AWAIT, TASK & CÁC ATTRIBUTE

Tài liệu này giải thích bản chất kiến trúc và lý thuyết nền tảng của các từ khóa thường gặp nhất trong Controller của ASP.NET Core MVC.

---

## 1. `IActionResult` LÀ GÌ?

### Định nghĩa:
`IActionResult` là một **Interface** trong namespace `Microsoft.AspNetCore.Mvc`. Nó đóng vai trò là kiểu trả về tổng quát (hợp đồng chung) đại diện cho kết quả của một Action gửi về cho trình duyệt (Client).

### Tại sao lại dùng `IActionResult` thay vì kiểu cụ thể?
Vì một Action có thể kết thúc bằng nhiều kịch bản phản hồi HTTP khác nhau:

| Lệnh trả về | Mã HTTP | Ý nghĩa bản chất |
| :--- | :--- | :--- |
| `return View();` | 200 OK | Render file giao diện `.cshtml` thành mã HTML gửi về trình duyệt |
| `return View(model);` | 200 OK | Gửi kèm đối tượng dữ liệu sang View để hiển thị |
| `return NotFound();` | 404 Not Found | Báo cho trình duyệt biết tài nguyên/bản ghi ID này không tồn tại |
| `return RedirectToAction("Index");` | 302 Found | Yêu cầu trình duyệt tự động chuyển hướng sang URL khác |
| `return Json(data);` | 200 OK | Trả về dữ liệu dạng JSON (thường dùng cho gọi AJAX / API) |
| `return BadRequest();` | 400 Bad Request | Báo lỗi dữ liệu gửi lên không đúng định dạng |
| `return Forbid();` | 403 Forbidden | Người dùng đã đăng nhập nhưng không đủ quyền truy cập |

👉 Nhờ có `IActionResult`, bạn có thể viết:
```csharp
public IActionResult Details(int? id)
{
    if (id == null) 
        return NotFound(); // Trả về NotFoundResult

    var item = _context.Categories.Find(id);
    if (item == null) 
        return NotFound(); // Trả về NotFoundResult

    return View(item); // Trả về ViewResult
}
```
Cả `NotFoundResult` và `ViewResult` đều kế thừa từ `IActionResult`, giúp mã nguồn linh hoạt và tuân thủ nguyên lý Đa hình (Polymorphism) trong OOP.

---

## 2. `Task` VÀ `async / await` (LẬP TRÌNH BẤT ĐỒNG BỘ)

### Bản chất:
* Trong môi trường Web Server, mỗi khi có người dùng truy cập, máy chủ sẽ cấp phát một Luồng (Thread) để xử lý.
* Nếu xử lý **Đồng bộ (Synchronous)**: Khi code gọi truy vấn Database (`_context.SaveChanges()`), Thread này sẽ bị "treo" (Block) để ngồi đợi Database phản hồi. Nếu có 1.000 người cùng bấm lưu, 1.000 Thread bị treo $\rightarrow$ Web bị chậm, đơ.
* Nếu xử lý **Bất đồng bộ (Asynchronous)**: Khi gặp từ khóa `await`, Thread hiện tại được giải phóng để đi phục vụ người dùng khác. Database làm việc xong sẽ báo lại, máy chủ cấp 1 Thread rảnh để tiếp tục chạy nốt đoạn code còn lại.

### `Task` là gì?
`Task` đại diện cho một công việc đang được thực thi trong tương lai (tương tự như `Promise` trong JavaScript). 
* `Task`: Thực hiện công việc nhưng không trả về giá trị (tương đương `void`).
* `Task<IActionResult>`: Thực hiện công việc bất đồng bộ, khi hoàn thành sẽ trả về một đối tượng kiểu `IActionResult`.

### Quy tắc sử dụng:
1. Khi nào hàm có từ khóa **`await`** $\rightarrow$ Tên hàm bắt buộc phải có bổ từ **`async`** và kiểu trả về bọc trong **`Task<...>`**.
2. Khi nào thao tác gọi I/O (Database, đọc file ổ cứng, gọi API bên thứ 3) $\rightarrow$ Luôn ưu tiên dùng hàm Async đi kèm `await` (ví dụ: `ToListAsync()`, `SaveChangesAsync()`, `FindAsync()`).

```csharp
// ĐÚNG: Có await -> Phải có async Task<IActionResult>
public async Task<IActionResult> Index()
{
    var data = await _context.VehicleCategories.ToListAsync();
    return View(data);
}

// ĐÚNG: Không có await (chỉ mở View trắng) -> Dùng IActionResult thường
[HttpGet]
public IActionResult Create()
{
    return View();
}
```

---

## 3. CÁC THUỘC TÍNH (ATTRIBUTES) TRÊN ACTION

### a) `[HttpGet]` và `[HttpPost]`
Quy định phương thức HTTP mà Action đó chấp nhận:
* **`[HttpGet]`**: Mặc định. Dùng khi muốn lấy dữ liệu để xem (tải trang, chuyển trang). Tham số truyền trên URL (`/Admin/VehicleCategory/Edit?id=5`).
* **`[HttpPost]`**: Dùng khi gửi dữ liệu lên Server để thay đổi trạng thái hệ thống (Thêm mới, Cập nhật, Xóa). Dữ liệu được giấu trong Body của HTTP Request.

### b) `[ValidateAntiForgeryToken]`
* **Mục đích:** Ngăn chặn tấn công CSRF (Cross-Site Request Forgery - Tấn công giả mạo yêu cầu từ trang web khác).
* **Cơ chế:** Khi render Form ở View, ASP.NET Core tự sinh một mã xác thực (Token) bí mật vào cookie và một ô `<input type="hidden">`. Khi bấm Submit, thuộc tính này sẽ đối chiếu xem hai mã Token có khớp nhau không. Nếu kẻ xấu dẫn dụ người dùng bấm vào một link độc từ bên ngoài để xóa dữ liệu, request đó sẽ bị chặn đứng vì không có Token hợp lệ.
* **Quy tắc:** Luôn đặt trên các Action nhận `[HttpPost]`.

### c) `ModelState.IsValid`
* `ModelState` là một từ điển chứa dữ liệu người dùng gửi lên và các lỗi phát sinh trong quá trình Model Binding và Validation.
* Trước khi Action thực thi, ASP.NET Core tự động đối chiếu dữ liệu gửi lên với các Data Annotations trong Entity Model (như `[Required]`, `[StringLength]`, `[Range]`).
* `ModelState.IsValid == true`: Dữ liệu hoàn toàn sạch và hợp lệ $\rightarrow$ An toàn để lưu vào Database.
* `ModelState.IsValid == false`: Có lỗi nhập liệu $\rightarrow$ Không lưu vào DB, trả lại View kèm thông báo lỗi cho người dùng.
