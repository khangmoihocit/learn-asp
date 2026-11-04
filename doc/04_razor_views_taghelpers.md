# 04. GIAO DIỆN RAZOR VIEWS, TAG HELPERS & LAYOUT

---

### 1. Khi muốn định nghĩa kiểu dữ liệu chính nhận từ Controller vào View
$\rightarrow$ **Dùng:** Cú pháp `@model TênKiểu` ở dòng đầu tiên của file `.cshtml`.

* Nếu View nhận về 1 danh sách:
  ```razor
  @model IEnumerable<btl_webnc_G08.Models.Entities.Vehicle>
  ```
* Nếu View nhận về 1 đối tượng duy nhất (trang Details/Edit):
  ```razor
  @model btl_webnc_G08.Models.Entities.Vehicle
  ```

---

### 2. Khi muốn tạo thẻ link bấm chuyển sang Controller / Action khác
$\rightarrow$ **Dùng:** Tag Helpers `asp-controller`, `asp-action`, `asp-route-[paramName]`.
```razor
<!-- Link xem chi tiết: /Vehicle/Details/5 -->
<a asp-controller="Vehicle" asp-action="Details" asp-route-id="@item.Id" class="btn btn-info">
    Xem chi tiết
</a>

<!-- Link quay về trang chủ: /Home/Index -->
<a asp-controller="Home" asp-action="Index">Về trang chủ</a>
```

---

### 3. Khi muốn tạo Form nhập liệu tự động liên kết với Model
$\rightarrow$ **Dùng:** Thẻ `<form asp-action="...">` kết hợp `asp-for` trên thẻ `<input>`, `<label>`, `<span asp-validation-for>`.
```razor
<form asp-action="Create" method="post" enctype="multipart/form-data">
    <!-- Hiển thị tổng hợp lỗi của toàn form -->
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>

    <div class="mb-3">
        <!-- Tự động lấy tên hiển thị từ [Display(Name="...")] trong Model -->
        <label asp-for="Name" class="form-label"></label>
        <!-- Tự động map thuộc tính Name và render thuộc tính name/id HTML -->
        <input asp-for="Name" class="form-control" />
        <!-- Tự động hiển thị lỗi màu đỏ nếu vi phạm [Required] -->
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>

    <!-- Ô upload file ảnh -->
    <div class="mb-3">
        <label class="form-label">Ảnh đại diện</label>
        <input type="file" name="imageFile" class="form-control" accept="image/*" />
    </div>

    <button type="submit" class="btn btn-success">Lưu lại</button>
</form>

<!-- Bật kiểm tra lỗi trực tiếp ở Client bằng Javascript (không reload trang) -->
@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```
> ⚠️ **Lưu ý quan trọng:** Bắt buộc phải có `enctype="multipart/form-data"` nếu form có ô chọn file/ảnh!

---

### 4. Khi muốn hiển thị Dropdown danh mục chọn lựa
$\rightarrow$ **Dùng:** Thẻ `<select asp-for="..." asp-items="ViewBag.CategoryId">`.
```razor
<div class="mb-3">
    <label asp-for="CategoryId" class="form-label">Chọn danh mục</label>
    <select asp-for="CategoryId" class="form-select" asp-items="ViewBag.CategoryId">
        <option value="">-- Chọn một danh mục --</option>
    </select>
    <span asp-validation-for="CategoryId" class="text-danger"></span>
</div>
```

---

### 5. Khi muốn hiển thị thông báo một lần (Flash Alert) sau khi Redirect
$\rightarrow$ **Dùng:** `TempData["..."]`.

* Ở Controller trước khi Redirect:
  ```csharp
  TempData["SuccessMessage"] = "Đã lưu phương tiện thành công!";
  return RedirectToAction(nameof(Index));
  ```
* Ở View (`Index.cshtml` hoặc `_Layout.cshtml`):
  ```razor
  @if (TempData["SuccessMessage"] != null)
  {
      <div class="alert alert-success alert-dismissible fade show" role="alert">
          @TempData["SuccessMessage"]
          <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      </div>
  }
  ```

---

### 6. Khi muốn tái sử dụng một đoạn HTML nhiều lần (Card xe, Khung thông tin)
$\rightarrow$ **Dùng:** Partial View (`_PartialName.cshtml`) và gọi bằng `<partial name="..." model="..." />`.

* Tạo file `Views/Shared/_VehicleCard.cshtml`:
  ```razor
  @model btl_webnc_G08.Models.Entities.Vehicle

  <div class="card shadow-sm mb-3">
      <img src="@model.ImageUrl" class="card-img-top" alt="@model.Name" style="height: 180px; object-fit: cover;">
      <div class="card-body">
          <h5 class="card-title">@model.Name</h5>
          <p class="text-danger fw-bold">@model.DailyRate.ToString("N0") VNĐ / ngày</p>
          <a asp-action="Details" asp-route-id="@model.Id" class="btn btn-sm btn-primary">Xem chi tiết</a>
      </div>
  </div>
  ```
* Nhúng vào View chính (`Index.cshtml`):
  ```razor
  <div class="row">
      @foreach (var item in Model)
      {
          <div class="col-md-4">
              <partial name="_VehicleCard" model="item" />
          </div>
      }
  </div>
  ```

---

### 7. Khi muốn truyền mã Javascript hoặc CSS riêng cho từng trang con vào Layout chung
$\rightarrow$ **Dùng:** `@section Scripts { ... }` ở View con và `@await RenderSectionAsync("Scripts", required: false)` ở `_Layout.cshtml`.
