# 08. HƯỚNG DẪN CHI TIẾT CÁCH VIẾT CONTROLLER CRUD VÀ KẾT NỐI VỚI VIEWS

Tài liệu này dành cho người mới bắt đầu, giải thích cặn kẽ từng dòng code trong Controller và cách Controller bắt tay với các View `.cshtml`.

---

## PHẦN 1: BẢN CHẤT CỦA CÁC ACTION TRONG 1 CONTROLLER CRUD

Mỗi chức năng quản lý dữ liệu (như Quản lý loại xe `VehicleCategory`) luôn bao gồm **5 hành động cơ bản**:

```
1. INDEX   : Lấy danh sách từ DB -> Gửi sang Index.cshtml để hiển thị bảng
2. CREATE  : 
   - [HttpGet]  : Mở trang Create.cshtml (Form rỗng)
   - [HttpPost] : Nhận dữ liệu người dùng gửi lên -> Lưu vào DB -> Quay về Index
3. EDIT    : 
   - [HttpGet]  : Tìm bản ghi theo ID -> Gửi sang Edit.cshtml (Form có sẵn dữ liệu cũ)
   - [HttpPost] : Nhận dữ liệu đã sửa -> Cập nhật vào DB -> Quay về Index
4. DELETE  : Nhận ID -> Tìm và xóa khỏi DB -> Quay về Index
```

---

## PHẦN 2: GIẢI THÍCH CHI TIẾT TỪNG ĐOẠN CODE CONTROLLER

### 1. Khởi tạo và Dependency Injection (DI)
```csharp
private readonly ApplicationDbContext _context;

public VehicleCategoryController(ApplicationDbContext context)
{
    _context = context;
}
```
* **Tại sao phải viết?** Controller không thể tự kết nối xuống Database. ASP.NET Core có cơ chế Dependency Injection (DI): hệ thống tự động đưa `ApplicationDbContext` vào hàm khởi tạo. Ta lưu vào biến `_context` để các hàm bên dưới dùng để thêm/sửa/xóa.

---

### 2. Action `Index` (Xem danh sách)
```csharp
// GET: /Admin/VehicleCategory
public async Task<IActionResult> Index()
{
    // 1. Dùng LINQ truy vấn bảng VehicleCategories, lấy toàn bộ về dạng danh sách
    var categories = await _context.VehicleCategories.ToListAsync();

    // 2. Truyền danh sách này vào hàm View() để file Index.cshtml nhận được
    return View(categories);
}
```
* **Luồng chạy:** Khi người dùng vào link `/Admin/VehicleCategory` $\rightarrow$ Hàm `Index` chạy $\rightarrow$ Lấy dữ liệu từ SQL $\rightarrow$ Bắn sang `Index.cshtml`.

---

### 3. Cặp Action `Create` (Thêm mới)
Tại sao thêm mới lại cần **2 hàm** cùng tên `Create`?

#### Hàm 1: `[HttpGet] Create()` - Mở Form
```csharp
[HttpGet]
public IActionResult Create()
{
    // Chỉ trả về trang giao diện form trắng tinh cho người dùng gõ
    return View();
}
```

#### Hàm 2: `[HttpPost] Create(...)` - Lưu dữ liệu
```csharp
[HttpPost]
[ValidateAntiForgeryToken] // Chống giả mạo request từ trang web khác
public async Task<IActionResult> Create(VehicleCategory category)
{
    // 1. Kiểm tra dữ liệu gửi lên có vi phạm [Required], [StringLength] trong Model không
    if (ModelState.IsValid)
    {
        // 2. Thêm đối tượng mới vào DbContext
        _context.Add(category);

        // 3. Thực thi câu lệnh INSERT INTO xuống SQL Server
        await _context.SaveChangesAsync();

        // 4. Lưu xong thì chuyển hướng về trang danh sách (Index)
        return RedirectToAction(nameof(Index));
    }

    // Nếu người dùng nhập sai/thiếu: trả lại trang Create kèm dữ liệu cũ để họ sửa lại
    return View(category);
}
```

---

### 4. Cặp Action `Edit` (Chỉnh sửa)

#### Hàm 1: `[HttpGet] Edit(int? id)` - Mở Form có sẵn dữ liệu cũ
```csharp
[HttpGet]
public async Task<IActionResult> Edit(int? id)
{
    // 1. Kiểm tra xem người dùng có truyền ID lên không
    if (id == null) return NotFound();

    // 2. Tìm bản ghi trong Database theo ID
    var category = await _context.VehicleCategories.FindAsync(id);

    // 3. Nếu không tìm thấy xe nào có ID này -> Báo lỗi 404
    if (category == null) return NotFound();

    // 4. Nếu thấy, truyền dữ liệu cũ sang Edit.cshtml để điền sẵn vào các ô input
    return View(category);
}
```

#### Hàm 2: `[HttpPost] Edit(...)` - Lưu dữ liệu vừa sửa
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, VehicleCategory category)
{
    // Kiểm tra tính hợp lệ: ID trên URL phải khớp với ID của đối tượng
    if (id != category.Id) return NotFound();

    if (ModelState.IsValid)
    {
        // Đánh dấu bản ghi này đã bị thay đổi để EF Core sinh lệnh UPDATE
        _context.Update(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    return View(category);
}
```

---

### 5. Action `Delete` (Xóa)
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    // 1. Tìm bản ghi cần xóa
    var category = await _context.VehicleCategories.FindAsync(id);

    if (category != null)
    {
        // 2. Đánh dấu xóa và thực thi lệnh DELETE FROM xuống SQL
        _context.VehicleCategories.Remove(category);
        await _context.SaveChangesAsync();
    }

    // 3. Xóa xong load lại trang Index
    return RedirectToAction(nameof(Index));
}
```

---

## PHẦN 3: GIẢI THÍCH CHI TIẾT TỪNG VIEW (KẾT NỐI VỚI CONTROLLER)

### 1. View `Index.cshtml` (Nhận danh sách và hiển thị)
```razor
@* Khai báo kiểu dữ liệu mà Controller gửi sang qua hàm return View(categories) *@
@model IEnumerable<btl_myselt.Models.Entities.VehicleCategory>

@{
    ViewData["Title"] = "Danh sách loại xe";
}

<div class="d-flex justify-content-between align-items-center mb-3">
    <h2>Quản lý loại xe</h2>
    <!-- Nút bấm chuyển sang trang Create -->
    <a asp-action="Create" class="btn btn-primary">Thêm loại xe mới</a>
</div>

<table class="table table-bordered table-striped">
    <thead>
        <tr>
            <th>ID</th>
            <th>Tên loại xe</th>
            <th>Mô tả</th>
            <th class="text-center" style="width: 150px;">Thao tác</th>
        </tr>
    </thead>
    <tbody>
    @foreach (var item in Model)
    {
        <tr>
            <td>@item.Id</td>
            <td>@item.Name</td>
            <td>@item.Description</td>
            <td class="text-center">
                <!-- Nút Sửa: truyền Id sang hàm Edit(int id) -->
                <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-warning btn-sm">Sửa</a>
                
                <!-- Nút Xóa: submit form gửi POST tới action Delete -->
                <form asp-action="Delete" asp-route-id="@item.Id" method="post" class="d-inline" onsubmit="return confirm('Bạn có chắc muốn xóa loại xe này?');">
                    <button type="submit" class="btn btn-danger btn-sm">Xóa</button>
                </form>
            </td>
        </tr>
    }
    </tbody>
</table>
```

* **Điểm mấu chốt:**
  * `@model IEnumerable<...>`: Bắt buộc để View hiểu biến `@Model` là một danh sách.
  * `asp-route-id="@item.Id"`: Tự động gắn `?id=...` hoặc `/Edit/1` vào đường link.

---

### 2. View `Create.cshtml` (Form nhập liệu)
```razor
@model btl_myselt.Models.Entities.VehicleCategory

@{
    ViewData["Title"] = "Thêm loại xe mới";
}

<h2>Thêm loại xe mới</h2>
<hr />

<div class="row">
    <div class="col-md-6">
        <form asp-action="Create" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>

            <div class="mb-3">
                <label asp-for="Name" class="form-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="Description" class="form-label"></label>
                <textarea asp-for="Description" class="form-control" rows="3"></textarea>
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>

            <button type="submit" class="btn btn-success">Lưu lại</button>
            <a asp-action="Index" class="btn btn-secondary">Quay lại</a>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

* **Điểm mấu chốt:**
  * `<input asp-for="Name" />`: Giúp ô nhập tự động nhận tên và tự động bind dữ liệu vào thuộc tính `Name` của `VehicleCategory category` bên Controller khi bấm Submit.
  * `<span asp-validation-for="Name"></span>`: Nơi dòng chữ *"Tên danh mục không được để trống"* sẽ xuất hiện nếu người dùng không nhập gì.
  * `_ValidationScriptsPartial`: File script có sẵn của template, giúp kiểm tra lỗi ngay lập tức mà không cần load lại trang.
