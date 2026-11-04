# 06. TẬP HỢP CÁC COMPONENT VÀ CLASS BOOTSTRAP HAY DÙNG TRONG ASP.NET CORE MVC

Tài liệu tra cứu nhanh các thành phần giao diện Bootstrap 5 thường xuyên sử dụng trong các View của ASP.NET Core MVC theo quy tắc: **"Khi cần [Làm giao diện gì] $\rightarrow$ Dùng [Class Bootstrap gì]"**.

---

### 1. BỐ CỤC CHIA CỘT RESPONSIVE (GRID SYSTEM)

#### Khi muốn chia danh sách dạng lưới (Mobile 1 cột, Tablet 2 cột, PC 3-4 cột)
$\rightarrow$ **Dùng:** `container` $\rightarrow$ `row` $\rightarrow$ `col-12 col-md-6 col-lg-4` kết hợp khoảng cách `g-3` (gutter).
```html
<div class="container my-4">
    <div class="row g-3">
        @foreach (var item in Model)
        {
            <div class="col-12 col-md-6 col-lg-4">
                <!-- Nội dung card xe / sản phẩm ở đây -->
            </div>
        }
    </div>
</div>
```

---

### 2. FORM NHẬP LIỆU (FORM CONTROLS)

#### a) Khi muốn ô nhập văn bản, số, mật khẩu chuẩn Bootstrap
$\rightarrow$ **Dùng:** `class="form-control"`, nhãn dùng `class="form-label"`, gom nhóm bằng `mb-3`.
```html
<div class="mb-3">
    <label asp-for="Name" class="form-label"></label>
    <input asp-for="Name" class="form-control" placeholder="Nhập tên..." />
    <span asp-validation-for="Name" class="text-danger small"></span>
</div>
```

#### b) Khi muốn làm Dropdown danh mục chọn lựa
$\rightarrow$ **Dùng:** `class="form-select"`.
```html
<div class="mb-3">
    <label asp-for="CategoryId" class="form-label">Danh mục</label>
    <select asp-for="CategoryId" class="form-select" asp-items="ViewBag.CategoryId">
        <option value="">-- Chọn danh mục --</option>
    </select>
</div>
```

#### c) Khi muốn làm Checkbox (Ví dụ: "Còn hoạt động", "Ghi nhớ đăng nhập")
$\rightarrow$ **Dùng:** `form-check`, `form-check-input`, `form-check-label`.
```html
<div class="form-check mb-3">
    <input asp-for="IsAvailable" class="form-check-input" type="checkbox" />
    <label asp-for="IsAvailable" class="form-check-label">Xe đang sẵn sàng cho thuê</label>
</div>
```

#### d) Khi muốn làm thanh Tìm kiếm liền nút bấm (Search Bar)
$\rightarrow$ **Dùng:** `input-group`.
```html
<form method="get" asp-action="Index">
    <div class="input-group mb-3">
        <input type="text" name="search" value="@ViewBag.Search" class="form-control" placeholder="Tìm theo tên xe hoặc biển số..." />
        <button class="btn btn-primary" type="submit">
            <i class="bi bi-search"></i> Tìm kiếm
        </button>
        <a asp-action="Index" class="btn btn-outline-secondary">Đặt lại</a>
    </div>
</form>
```

---

### 3. NÚT BẤM (BUTTONS) & HÀNH ĐỘNG

| Nhu cầu hành động | Dùng Class Bootstrap | Ví dụ minh họa |
| :--- | :--- | :--- |
| **Lưu / Gửi form chính** | `btn btn-primary` (Xanh dương) | `<button type="submit" class="btn btn-primary">Lưu dữ liệu</button>` |
| **Thêm mới / Xác nhận thành công** | `btn btn-success` (Xanh lá) | `<a asp-action="Create" class="btn btn-success">Thêm xe mới</a>` |
| **Xóa / Hủy bỏ / Nguy hiểm** | `btn btn-danger` (Đỏ) | `<button type="submit" class="btn btn-danger">Xóa bản ghi</button>` |
| **Chỉnh sửa dữ liệu** | `btn btn-warning` (Vàng) | `<a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-warning">Sửa</a>` |
| **Xem chi tiết** | `btn btn-info` (Xanh ngọc) | `<a asp-action="Details" asp-route-id="@item.Id" class="btn btn-info">Chi tiết</a>` |
| **Quay lại / Đóng form** | `btn btn-secondary` (Xám) | `<a asp-action="Index" class="btn btn-secondary">Quay lại</a>` |
| **Nút nhỏ dùng trong bảng dữ liệu** | Thêm class `btn-sm` | `class="btn btn-warning btn-sm"` |
| **Nút viền trong suốt** | Dùng tiền tố `btn-outline-*` | `class="btn btn-outline-danger"` |

---

### 4. BẢNG DỮ LIỆU DANH SÁCH (TABLES)

#### Khi muốn làm bảng hiển thị dữ liệu đẹp, có kẻ sọc, hover chuột và cuộn ngang trên điện thoại
$\rightarrow$ **Dùng:** Bọc thẻ `<table>` trong thẻ `<div class="table-responsive">` và dùng các class `table table-striped table-hover table-bordered align-middle`.
```html
<div class="table-responsive shadow-sm rounded">
    <table class="table table-bordered table-striped table-hover align-middle mb-0">
        <thead class="table-dark">
            <tr>
                <th>Ảnh</th>
                <th>Tên phương tiện</th>
                <th>Giá thuê</th>
                <th>Trạng thái</th>
                <th class="text-center" style="width: 180px;">Hành động</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model)
            {
                <tr>
                    <td style="width: 80px;">
                        <img src="@item.ImageUrl" class="img-thumbnail" style="width: 60px; height: 60px; object-fit: cover;" />
                    </td>
                    <td class="fw-bold">@item.Name</td>
                    <td class="text-danger fw-semibold">@item.DailyRate.ToString("N0") đ/ngày</td>
                    <td><span class="badge bg-success">Đang rảnh</span></td>
                    <td class="text-center">
                        <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-warning btn-sm">Sửa</a>
                        <a asp-action="Details" asp-route-id="@item.Id" class="btn btn-info btn-sm">Xem</a>
                        <form asp-action="Delete" asp-route-id="@item.Id" method="post" class="d-inline" onsubmit="return confirm('Bạn có chắc muốn xóa?');">
                            <button type="submit" class="btn btn-danger btn-sm">Xóa</button>
                        </form>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

---

### 5. KHUNG THẺ THÔNG TIN (CARDS)

#### Khi muốn làm card hiển thị xe / phòng / thẻ thông số Dashboard
$\rightarrow$ **Dùng:** `card`, `card-img-top`, `card-body`, `card-footer`, `shadow-sm`.
```html
<div class="card h-100 shadow-sm border-0">
    <img src="@item.ImageUrl" class="card-img-top" alt="@item.Name" style="height: 200px; object-fit: cover;">
    <div class="card-body">
        <div class="d-flex justify-content-between align-items-center mb-2">
            <span class="badge bg-primary">@item.Category?.Name</span>
            <small class="text-muted">Biển số: @item.LicensePlate</small>
        </div>
        <h5 class="card-title text-truncate">@item.Name</h5>
        <p class="card-text text-danger fs-5 fw-bold mb-0">
            @item.DailyRate.ToString("N0") VNĐ <small class="text-muted fs-6 fw-normal">/ ngày</small>
        </p>
    </div>
    <div class="card-footer bg-white border-0 pt-0">
        <a asp-action="Details" asp-route-id="@item.Id" class="btn btn-outline-primary w-100">Xem chi tiết</a>
    </div>
</div>
```

---

### 6. HUY HIỆU TRẠNG THÁI (BADGES)

#### Khi muốn gắn nhãn trạng thái (Đã duyệt, Chờ xử lý, Đã hủy)
$\rightarrow$ **Dùng:** `badge` kết hợp màu nền `bg-*`.
```html
<!-- Trạng thái hoàn thành / Sẵn sàng -->
<span class="badge bg-success">Sẵn sàng</span>

<!-- Trạng thái chờ xử lý / Chú ý -->
<span class="badge bg-warning text-dark">Chờ duyệt</span>

<!-- Trạng thái lỗi / Đã hủy / Đã khóa -->
<span class="badge bg-danger">Đã hủy</span>

<!-- Trạng thái phụ / Không hoạt động -->
<span class="badge bg-secondary">Bảo trì</span>

<!-- Dạng bo tròn hình viên thuốc -->
<span class="badge rounded-pill bg-info text-dark">Xe số</span>
```

---

### 7. HỘP THOẠI POPUP XÁC NHẬN (MODAL)

#### Khi muốn hiển thị Popup xác nhận Xóa mà không bị thô như alert mặc định
$\rightarrow$ **Dùng:** `modal`, `modal-dialog`, `modal-content`.
```html
<!-- Nút kích hoạt Modal mở lên -->
<button type="button" class="btn btn-danger btn-sm" data-bs-toggle="modal" data-bs-target="#deleteModal-@item.Id">
    Xóa
</button>

<!-- Khung Modal -->
<div class="modal fade" id="deleteModal-@item.Id" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Xác nhận xóa</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                Bạn có chắc chắn muốn xóa phương tiện <strong>@item.Name</strong> không?
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                <form asp-action="Delete" asp-route-id="@item.Id" method="post">
                    <button type="submit" class="btn btn-danger">Đồng ý Xóa</button>
                </form>
            </div>
        </div>
    </div>
</div>
```

---

### 8. THANH PHÂN TRANG (PAGINATION)

#### Khi muốn hiển thị thanh phân trang đẹp phía dưới bảng danh sách
$\rightarrow$ **Dùng:** Thẻ `<ul>` với class `pagination` kết hợp `page-item`, `page-link`.
```html
<nav class="mt-4">
    <ul class="pagination justify-content-center">
        <!-- Nút Trang trước -->
        <li class="page-item @(ViewBag.CurrentPage == 1 ? "disabled" : "")">
            <a class="page-link" asp-action="Index" asp-route-page="@(ViewBag.CurrentPage - 1)" asp-route-search="@ViewBag.Search">Trước</a>
        </li>

        <!-- Các số trang -->
        @for (int i = 1; i <= ViewBag.TotalPages; i++)
        {
            <li class="page-item @(i == ViewBag.CurrentPage ? "active" : "")">
                <a class="page-link" asp-action="Index" asp-route-page="@i" asp-route-search="@ViewBag.Search">@i</a>
            </li>
        }

        <!-- Nút Trang sau -->
        <li class="page-item @(ViewBag.CurrentPage == ViewBag.TotalPages ? "disabled" : "")">
            <a class="page-link" asp-action="Index" asp-route-page="@(ViewBag.CurrentPage + 1)" asp-route-search="@ViewBag.Search">Sau</a>
        </li>
    </ul>
</nav>
```

---

### 9. CÁC CLASS TIỆN ÍCH CỰC KỲ TIẾT KIỆM THỜI GIAN (UTILITIES)

* **Khoảng cách (Margin / Padding):**
  - `mb-3`: Cách dưới 1rem (`margin-bottom`).
  - `my-4`: Cách trên và dưới (`margin-top & bottom`).
  - `p-3`: Đệm trong 4 phía (`padding`).
  - `mx-auto`: Căn giữa khối block ra giữa màn hình.
* **Căn chỉnh Flexbox:**
  - `d-flex justify-content-between align-items-center`: Hai phần tử dạt sang hai bên mép và căn giữa theo chiều dọc.
  - `d-flex gap-2`: Các nút bấm nằm ngang cạnh nhau và cách đều nhau 1 khoảng nhỏ.
* **Định dạng chữ (Text):**
  - `text-center`, `text-end`: Căn giữa, căn phải chữ.
  - `text-danger`, `text-success`, `text-muted`: Màu đỏ, xanh lá, xám mờ.
  - `fw-bold` (in đậm), `fst-italic` (in nghiêng), `fs-4` (cỡ chữ to).
  - `text-truncate`: Chữ quá dài tự hiện dấu 3 chấm `...`.
* **Hiển thị / Ẩn phần tử:**
  - `d-none d-md-block`: Ẩn trên điện thoại, chỉ hiện trên màn hình máy tính (tablet/PC).
