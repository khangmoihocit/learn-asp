# 05. XÁC THỰC, PHÂN QUYỀN (AUTH) & QUẢN LÝ TRẠNG THÁI (SESSION)

---

### 1. Khi muốn bắt buộc người dùng phải đăng nhập mới được vào trang / Action
$\rightarrow$ **Dùng:** Thuộc tính `[Authorize]` đặt trên đầu Controller hoặc Action.
```csharp
[Authorize] // Phải đăng nhập mới được vào bất kỳ Action nào trong Controller này
public class BookingController : Controller
{
    // ...
}
```

* Nếu Controller có `[Authorize]` nhưng muốn mở công khai 1 action (ví dụ trang giới thiệu):
  ```csharp
  [AllowAnonymous]
  public IActionResult About()
  {
      return View();
  }
  ```

---

### 2. Khi muốn chỉ cho phép một vai trò cụ thể (ví dụ Admin) được truy cập
$\rightarrow$ **Dùng:** `[Authorize(Roles = "Admin")]`.
```csharp
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}

// Nếu cho phép nhiều Role cùng truy cập:
[Authorize(Roles = "Admin,Moderator")]
public IActionResult Reports() => View();
```

---

### 3. Khi muốn lấy thông tin của người dùng đang đăng nhập hiện tại
$\rightarrow$ **Dùng:** Đối tượng `User` có sẵn trong Controller và Razor View.

* Kiểm tra đã đăng nhập chưa:
  ```csharp
  bool isLoggedIn = User.Identity?.IsAuthenticated ?? false;
  ```
* Lấy tên đăng nhập / Email:
  ```csharp
  string? userName = User.Identity?.Name;
  ```
* Lấy Id của tài khoản (ClaimTypes.NameIdentifier):
  ```csharp
  using System.Security.Claims;

  string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
  ```
* Kiểm tra quyền trong code C# hoặc Razor:
  ```csharp
  if (User.IsInRole("Admin"))
  {
      // Xử lý riêng cho Admin
  }
  ```

---

### 4. Khi muốn ẩn/hiện nút bấm trên giao diện View theo trạng thái đăng nhập
$\rightarrow$ **Dùng:** `@if (User.Identity.IsAuthenticated)` trong Razor View.
```razor
@if (User.Identity != null && User.Identity.IsAuthenticated)
{
    <li class="nav-item">
        <span class="nav-link text-dark">Xin chào, @User.Identity.Name!</span>
    </li>
    @if (User.IsInRole("Admin"))
    {
        <li class="nav-item">
            <a class="nav-link text-danger" asp-area="Admin" asp-controller="Dashboard" asp-action="Index">Quản trị</a>
        </li>
    }
    <li class="nav-item">
        <form asp-controller="Account" asp-action="Logout" method="post">
            <button type="submit" class="btn btn-link nav-link">Đăng xuất</button>
        </form>
    </li>
}
else
{
    <li class="nav-item">
        <a class="nav-link" asp-controller="Account" asp-action="Login">Đăng nhập</a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-controller="Account" asp-action="Register">Đăng ký</a>
    </li>
}
```

---

### 5. Khi muốn lưu và lấy dữ liệu trong Session (Giỏ hàng, trạng thái tạm)
$\rightarrow$ **Dùng:** `HttpContext.Session.SetString` / `SetInt32` và `GetString` / `GetInt32`.

* Lưu dữ liệu vào Session:
  ```csharp
  // Lưu chuỗi:
  HttpContext.Session.SetString("UserRole", "Admin");

  // Lưu số nguyên:
  HttpContext.Session.SetInt32("CartCount", 5);

  // Lưu một đối tượng phức tạp (chuyển sang chuỗi JSON):
  string jsonString = JsonSerializer.Serialize(myCartObject);
  HttpContext.Session.SetString("MyCart", jsonString);
  ```

* Đọc dữ liệu từ Session:
  ```csharp
  string? role = HttpContext.Session.GetString("UserRole");
  int? count = HttpContext.Session.GetInt32("CartCount");

  // Đọc lại đối tượng từ JSON:
  string? cartJson = HttpContext.Session.GetString("MyCart");
  if (cartJson != null)
  {
      var cart = JsonSerializer.Deserialize<CartViewModel>(cartJson);
  }
  ```

* Xóa dữ liệu Session khi đăng xuất:
  ```csharp
  HttpContext.Session.Remove("MyCart"); // Xóa 1 khóa
  HttpContext.Session.Clear();          // Xóa toàn bộ session
  ```
