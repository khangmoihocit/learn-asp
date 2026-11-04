# 01. CẤU HÌNH HỆ THỐNG, ROUTING & MIDDLEWARE (PROGRAM.CS)

Tập hợp các trường hợp cấu hình hay gặp nhất trong file `Program.cs`.

---

### 1. Khi muốn kết nối Database SQL Server qua Entity Framework Core
$\rightarrow$ **Dùng:** Cấu hình chuỗi kết nối trong `appsettings.json` và gọi `builder.Services.AddDbContext<...>()` trong `Program.cs`.

* Trong `appsettings.json`:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyProjectDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
  ```
* Trong `Program.cs`:
  ```csharp
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  ```

---

### 2. Khi muốn chia dự án thành phân hệ quản trị riêng biệt (Admin Area)
$\rightarrow$ **Dùng:** Thư mục `Areas/Admin` và cấu hình route cho Area trong `Program.cs` **trước** route mặc định.

* Trong `Program.cs`:
  ```csharp
  // Route cho Area (phải đặt trước route default)
  app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

  // Route mặc định
  app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
  ```
* Trên Controller thuộc Area:
  ```csharp
  [Area("Admin")]
  public class DashboardController : Controller { ... }
  ```

---

### 3. Khi muốn sử dụng Session (lưu giỏ hàng, thông tin đăng nhập tạm)
$\rightarrow$ **Dùng:** Đăng ký dịch vụ `AddSession` và kích hoạt middleware `UseSession`.

* Trong `Program.cs`:
  ```csharp
  // 1. Đăng ký Service
  builder.Services.AddDistributedMemoryCache();
  builder.Services.AddSession(options =>
  {
      options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn
      options.Cookie.HttpOnly = true;
      options.Cookie.IsEssential = true;
  });

  // 2. Kích hoạt Middleware (Đặt GIỮA UseRouting và UseAuthorization)
  app.UseRouting();
  app.UseSession(); // <-- Đặt ở đây
  app.UseAuthorization();
  ```

---

### 4. Khi muốn cho phép hiển thị ảnh / CSS / JS từ thư mục `wwwroot`
$\rightarrow$ **Dùng:** Middleware `app.UseStaticFiles()` hoặc `app.MapStaticAssets()`.

```csharp
app.UseStaticFiles(); // Cho phép truy cập file tĩnh trong wwwroot
```

---

### 5. Khi muốn đổi trang khởi động mặc định của ứng dụng
$\rightarrow$ **Dùng:** Sửa giá trị mặc định của `controller` và `action` trong `pattern`.

* Ví dụ: Muốn mở web lên là vào ngay `Vehicle/Index`:
  ```csharp
  app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Vehicle}/{action=Index}/{id?}");
  ```

---

### 6. Khi muốn tự động cập nhật Migration và dữ liệu mẫu (Seed Data) khi ứng dụng khởi chạy
$\rightarrow$ **Dùng:** Tạo Scope từ `app.Services` sau khi `builder.Build()`.

```csharp
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // Tự động chạy migration nếu DB chưa có
    // DbInitializer.Seed(context); // Nạp dữ liệu mẫu
}
```
