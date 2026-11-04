# 03. CONTROLLER, ACTION METHODS, CRUD & VALIDATION

---

### 1. Khi muốn lấy DbContext hoặc Environment vào Controller
$\rightarrow$ **Dùng:** Constructor Dependency Injection (khởi tạo qua hàm khởi tạo `ctor`).
```csharp
public class VehicleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public VehicleController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
}
```

---

### 2. Khi muốn phân biệt action mở giao diện form nhập và action nhận dữ liệu gửi lên
$\rightarrow$ **Dùng:** Cặp thuộc tính `[HttpGet]` và `[HttpPost]`.
```csharp
// 1. Mở Form nhập:
[HttpGet]
public IActionResult Create()
{
    return View();
}

// 2. Tiếp nhận dữ liệu Form khi bấm nút Submit:
[HttpPost]
[ValidateAntiForgeryToken] // Chống tấn công giả mạo CSRF
public async Task<IActionResult> Create(Vehicle model)
{
    // Xử lý lưu dữ liệu
}
```

---

### 3. Khi muốn kiểm tra dữ liệu người dùng nhập có thỏa mãn quy tắc Data Annotations không
$\rightarrow$ **Dùng:** `if (ModelState.IsValid)` và `ModelState.AddModelError(...)`.
```csharp
[HttpPost]
public async Task<IActionResult> Create(Vehicle vehicle)
{
    // Tự thêm lỗi nghiệp vụ nếu cần:
    if (vehicle.DailyRate <= 0)
    {
        ModelState.AddModelError("DailyRate", "Giá thuê phải lớn hơn 0");
    }

    if (ModelState.IsValid)
    {
        _context.Add(vehicle);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); // Chuyển trang thành công
    }

    // Nếu form sai, nạp lại View cùng model để hiển thị thông báo lỗi đỏ
    return View(vehicle);
}
```

---

### 4. Khi muốn upload file/ảnh từ máy tính của người dùng lên Server
$\rightarrow$ **Dùng:** Kiểu tham số `IFormFile` + thư viện `System.IO` + lưu vào `wwwroot/uploads`.
```csharp
[HttpPost]
public async Task<IActionResult> Create(Vehicle vehicle, IFormFile? imageFile)
{
    if (imageFile != null && imageFile.Length > 0)
    {
        // 1. Định vị thư mục wwwroot/uploads
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        // 2. Tạo tên file ngẫu nhiên tránh trùng lặp
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // 3. Ghi file vào ổ cứng server
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // 4. Lưu đường dẫn tương đối vào database
        vehicle.ImageUrl = "/uploads/" + uniqueFileName;
    }

    // Tiến hành lưu vehicle vào DB...
}
```

---

### 5. Khi muốn truyền danh sách danh mục để đổ vào thẻ `<select>` Dropdown trên View
$\rightarrow$ **Dùng:** `SelectList` kết hợp với `ViewBag`.
```csharp
// Ở Controller:
ViewBag.CategoryId = new SelectList(await _context.VehicleCategories.ToListAsync(), "Id", "Name");

// Nếu ở trang Edit (cần chọn sẵn danh mục cũ của đối tượng):
ViewBag.CategoryId = new SelectList(await _context.VehicleCategories.ToListAsync(), "Id", "Name", vehicle.CategoryId);
```

---

### 6. Khi muốn chuyển hướng người dùng sang trang khác sau khi xử lý thành công
$\rightarrow$ **Dùng:** `return RedirectToAction("ActionName", "ControllerName")`.
```csharp
// Chuyển về Index của cùng Controller:
return RedirectToAction(nameof(Index));

// Chuyển về Action Index của Controller Home:
return RedirectToAction("Index", "Home");

// Chuyển sang Action khác có kèm tham số id:
return RedirectToAction(nameof(Details), new { id = vehicle.Id });
```

---

### 7. Khi không tìm thấy dữ liệu theo ID được yêu cầu
$\rightarrow$ **Dùng:** `return NotFound()`.
```csharp
public async Task<IActionResult> Details(int? id)
{
    if (id == null) return NotFound();

    var vehicle = await _context.Vehicles.FindAsync(id);
    if (vehicle == null) return NotFound();

    return View(vehicle);
}
```

---

### 8. Khi muốn viết hàm phục vụ AJAX hoặc trả về dữ liệu cho Javascript
$\rightarrow$ **Dùng:** `return Json(...)`.
```csharp
[HttpGet]
public async Task<IActionResult> CheckAvailability(int vehicleId, DateTime date)
{
    bool isAvailable = !await _context.Bookings.AnyAsync(b => b.VehicleId == vehicleId && b.StartDate.Date == date.Date);
    return Json(new { success = true, available = isAvailable });
}
```
