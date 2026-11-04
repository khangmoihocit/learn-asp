# 07. TỔNG HỢP CÚ PHÁP LINQ HAY DÙNG NHẤT TRONG ASP.NET CORE MVC

Tài liệu tra cứu nhanh các câu lệnh LINQ (kết hợp với Entity Framework Core) được sử dụng nhiều nhất khi viết Controller và làm thống kê Dashboard theo quy tắc: **"Khi cần [Xử lý dữ liệu gì] $\rightarrow$ Dùng [Hàm LINQ gì]"**.

---

### 1. LẤY DỮ LIỆU ĐƠN LẺ HOẶC DANH SÁCH

| Mục đích | Hàm LINQ | Code mẫu | Lưu ý |
| :--- | :--- | :--- | :--- |
| **Lấy toàn bộ danh sách** | `.ToListAsync()` | `await _context.Vehicles.ToListAsync();` | Luôn dùng `await` và hàm Async |
| **Tìm theo Khóa chính (Id)** | `FindAsync(id)` | `await _context.Vehicles.FindAsync(id);` | Nhanh nhất vì kiểm tra bộ nhớ Cache trước |
| **Tìm phần tử đầu tiên thỏa mãn** | `.FirstOrDefaultAsync(...)` | `await _context.Vehicles.FirstOrDefaultAsync(v => v.Name == name);` | Trả về `null` nếu không tìm thấy |
| **Tìm duy nhất 1 phần tử** | `.SingleOrDefaultAsync(...)` | `await _context.Users.SingleOrDefaultAsync(u => u.Email == email);` | Ném ra Exception nếu có từ 2 bản ghi trùng |

---

### 2. LỌC DỮ LIỆU (FILTERING & SEARCH)

#### a) Khi muốn tìm kiếm từ khóa (tương đương LIKE %keyword%)
$\rightarrow$ **Dùng:** `.Where(x => x.ThuocTinh.Contains(keyword))`.
```csharp
var result = await _context.Vehicles
    .Where(v => v.Name.Contains(searchString) || v.LicensePlate.Contains(searchString))
    .ToListAsync();
```

#### b) Khi muốn lọc theo khoảng giá hoặc ngày tháng
$\rightarrow$ **Dùng:** `.Where(...)` với toán tử `>=`, `<=`.
```csharp
var result = await _context.Vehicles
    .Where(v => v.DailyRate >= minPrice && v.DailyRate <= maxPrice)
    .ToListAsync();
```

#### c) Khi muốn lọc theo danh sách ID (tương đương IN (1, 2, 3))
$\rightarrow$ **Dùng:** `danhSachIds.Contains(x.Id)`.
```csharp
List<int> selectedCategoryIds = new List<int> { 1, 2, 5 };

var result = await _context.Vehicles
    .Where(v => selectedCategoryIds.Contains(v.CategoryId))
    .ToListAsync();
```

---

### 3. SẮP XẾP DỮ LIỆU (SORTING)

#### a) Khi muốn sắp xếp từ mới đến cũ / giá từ cao xuống thấp (Giảm dần)
$\rightarrow$ **Dùng:** `.OrderByDescending(...)`.
```csharp
var list = await _context.Vehicles
    .OrderByDescending(v => v.Id) // Hoặc v.CreatedAt
    .ToListAsync();
```

#### b) Khi muốn sắp xếp nhiều tiêu chí lồng nhau
$\rightarrow$ **Dùng:** `.OrderBy(...)` kết hợp với `.ThenBy(...)` hoặc `.ThenByDescending(...)`.
```csharp
// Sắp xếp loại xe theo Tên (A-Z), nếu trùng tên thì xếp theo Giá rẻ nhất lên trước:
var list = await _context.Vehicles
    .OrderBy(v => v.Category.Name)
    .ThenBy(v => v.DailyRate)
    .ToListAsync();
```

---

### 4. PHÂN TRANG (PAGINATION)

#### Khi muốn lấy dữ liệu cho trang số X (mỗi trang N phần tử)
$\rightarrow$ **Dùng:** `.Skip(...)` và `.Take(...)`.
```csharp
int pageSize = 10;   // 10 bản ghi mỗi trang
int pageNumber = 2; // Đang xem trang 2

var pagedVehicles = await _context.Vehicles
    .OrderBy(v => v.Id)                  // Bắt buộc phải OrderBy trước khi phân trang
    .Skip((pageNumber - 1) * pageSize)   // Bỏ qua 10 bản ghi đầu của trang 1
    .Take(pageSize)                      // Lấy 10 bản ghi tiếp theo
    .ToListAsync();
```

---

### 5. NỐI BẢNG / LẤY DỮ LIỆU BẢNG LIÊN QUAN (JOIN & INCLUDE)

#### a) Khi muốn lấy bảng cha hoặc bảng con (Eager Loading)
$\rightarrow$ **Dùng:** `.Include(...)`.
```csharp
// Lấy xe kèm thông tin Danh mục của xe đó:
var vehicles = await _context.Vehicles
    .Include(v => v.Category)
    .ToListAsync();
```

#### b) Khi muốn lấy sâu 2 cấp liên tiếp (Bảng A $\rightarrow$ Bảng B $\rightarrow$ Bảng C)
$\rightarrow$ **Dùng:** `.ThenInclude(...)`.
```csharp
// Lấy Đơn thuê (Booking) -> Kèm theo Xe (Vehicle) -> Kèm theo Danh mục của xe (Category):
var bookings = await _context.Bookings
    .Include(b => b.Vehicle)
        .ThenInclude(v => v.Category)
    .ToListAsync();
```

---

### 6. KIỂM TRA ĐIỀU KIỆN & THỐNG KÊ (AGGREGATIONS & DASHBOARD)

Rất hay dùng khi làm trang **Admin Dashboard** để vẽ biểu đồ và hiển thị thẻ con số thống kê.

#### a) Kiểm tra tồn tại (Trả về `true`/`false`)
$\rightarrow$ **Dùng:** `.AnyAsync(...)`.
```csharp
// Kiểm tra biển số xe có bị trùng không:
bool isDuplicate = await _context.Vehicles.AnyAsync(v => v.LicensePlate == "29A-12345");

// Kiểm tra danh mục này hiện tại có xe nào không (trước khi xóa):
bool hasVehicles = await _context.Vehicles.AnyAsync(v => v.CategoryId == categoryId);
```

#### b) Đếm tổng số lượng bản ghi
$\rightarrow$ **Dùng:** `.CountAsync(...)`.
```csharp
int totalVehicles = await _context.Vehicles.CountAsync();
int pendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Pending");
```

#### c) Tính tổng tiền / Giá trị trung bình
$\rightarrow$ **Dùng:** `.SumAsync(...)` và `.AverageAsync(...)`.
```csharp
// Tổng doanh thu từ các đơn đã hoàn thành:
decimal totalRevenue = await _context.Bookings
    .Where(b => b.Status == "Completed")
    .SumAsync(b => b.TotalPrice);

// Giá thuê xe trung bình:
decimal avgRate = await _context.Vehicles.AverageAsync(v => v.DailyRate);
```

---

### 7. CHỌN LỌC CỘT / ÁNH XẠ SANG VIEWMODEL (PROJECTION)

#### Khi chỉ muốn lấy 2-3 cột cần thiết (Giúp câu SQL chạy siêu nhẹ, không select *)
$\rightarrow$ **Dùng:** `.Select(...)`.
```csharp
// Lấy danh sách gọn chỉ gồm Id và Tên để đổ vào Dropdown:
var categoryDropdown = await _context.VehicleCategories
    .Select(c => new {
        Value = c.Id,
        Text = c.Name
    })
    .ToListAsync();
```

---

### 8. GOM NHÓM DỮ LIỆU (GROUP BY)

#### Khi muốn thống kê số lượng xe theo từng danh mục
$\rightarrow$ **Dùng:** `.GroupBy(...)` kết hợp `.Select(...)`.
```csharp
var stats = await _context.Vehicles
    .GroupBy(v => v.Category.Name)
    .Select(g => new {
        CategoryName = g.Key,
        VehicleCount = g.Count(),
        AvgPrice = g.Average(v => v.DailyRate)
    })
    .ToListAsync();
```

---

### 9. KỸ THUẬT XÂY DỰNG QUERY ĐỘNG (DYNAMIC FILTER)

#### Khi trang có nhiều ô lọc tùy chọn (Người dùng nhập ô nào thì mới lọc ô đó)
$\rightarrow$ **Dùng:** `AsQueryable()` kết hợp cộng dồn các lệnh `if`.
```csharp
public async Task<IActionResult> Index(string? search, int? categoryId, decimal? maxPrice)
{
    // 1. Tạo Query rỗng (chưa chạy xuống SQL)
    var query = _context.Vehicles.Include(v => v.Category).AsQueryable();

    // 2. Kiểm tra có từ khóa thì mới Where
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(v => v.Name.Contains(search));
    }

    // 3. Có chọn danh mục thì mới Where
    if (categoryId.HasValue)
    {
        query = query.Where(v => v.CategoryId == categoryId.Value);
    }

    // 4. Có nhập giá tối đa thì mới Where
    if (maxPrice.HasValue)
    {
        query = query.Where(v => v.DailyRate <= maxPrice.Value);
    }

    // 5. Đến bước này LINQ mới chính thức thực thi lệnh SQL xuống Database:
    var result = await query.OrderByDescending(v => v.Id).ToListAsync();

    return View(result);
}
```

---

### 10. TỐI ƯU HIỆU NĂNG VỚI `AsNoTracking()`

#### Khi truy vấn dữ liệu CHỈ ĐỂ HIỂN THỊ (Xem danh sách, chi tiết) mà KHÔNG SỬA/XÓA
$\rightarrow$ **Dùng:** `.AsNoTracking()`.
```csharp
var list = await _context.Vehicles
    .AsNoTracking()
    .Include(v => v.Category)
    .ToListAsync();
```
* **Lợi ích:** Bỏ qua cơ chế theo dõi thay đổi (Change Tracker) của Entity Framework Core, giúp ứng dụng tiết kiệm RAM và tốc độ truy vấn nhanh hơn 30% - 50%.
