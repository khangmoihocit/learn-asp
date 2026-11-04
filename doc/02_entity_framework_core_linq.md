# 02. ENTITY FRAMEWORK CORE, DATA ANNOTATIONS & TRUY VẤN LINQ

---

## PHẦN A: KHAI BÁO MODEL VÀ RÀNG BUỘC DỮ LIỆU (DATA ANNOTATIONS)

### 1. Khi muốn chỉ định khóa chính (Primary Key)
$\rightarrow$ **Dùng:** `[Key]`.
```csharp
[Key]
public int Id { get; set; }
```

### 2. Khi muốn trường dữ liệu bắt buộc phải nhập và hiển thị thông báo lỗi tùy chỉnh
$\rightarrow$ **Dùng:** `[Required(ErrorMessage = "...")]`.
```csharp
[Required(ErrorMessage = "Vui lòng nhập tên phương tiện")]
public string Name { get; set; } = string.Empty;
```

### 3. Khi muốn giới hạn số ký tự tối đa / tối thiểu
$\rightarrow$ **Dùng:** `[StringLength(max, MinimumLength = min)]` hoặc `[MaxLength(max)]`.
```csharp
[StringLength(150, MinimumLength = 3, ErrorMessage = "Độ dài từ 3 đến 150 ký tự")]
public string Title { get; set; } = string.Empty;
```

### 4. Khi muốn giới hạn khoảng giá trị số (giá tiền, số lượng, tuổi)
$\rightarrow$ **Dùng:** `[Range(min, max)]`.
```csharp
[Range(1000, 50000000, ErrorMessage = "Giá thuê phải từ 1,000đ đến 50,000,000đ")]
public decimal Price { get; set; }
```

### 5. Khi muốn đổi tên hiển thị tiếng Việt trên nhãn Form của View
$\rightarrow$ **Dùng:** `[Display(Name = "...")]`.
```csharp
[Display(Name = "Biển số xe")]
public string LicensePlate { get; set; } = string.Empty;
```

### 6. Khi muốn thiết lập quan hệ 1 - Nhiều (1 Category có nhiều Vehicle)
$\rightarrow$ **Dùng:** `[ForeignKey]` trên khóa ngoại + Navigation property.
```csharp
// Trong Entity Vehicle:
public int CategoryId { get; set; } // Cột khóa ngoại trong Database

[ForeignKey("CategoryId")]
public virtual VehicleCategory? Category { get; set; } // Thuộc tính điều hướng bảng cha

// Trong Entity VehicleCategory:
public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>(); // Danh sách bảng con
```

### 7. Khi muốn một thuộc tính KHÔNG lưu vào Database (chỉ dùng tính toán trên web)
$\rightarrow$ **Dùng:** `[NotMapped]`.
```csharp
[NotMapped]
public string FullInfo => $"{Name} - {LicensePlate}";
```

---

## PHẦN B: TRUY VẤN LINQ THỰC CHIẾN TRONG CONTROLLER

### 1. Khi muốn lấy toàn bộ danh sách kèm theo bảng quan hệ (tránh lỗi Object Null)
$\rightarrow$ **Dùng:** `.Include(...)` (Eager Loading).
```csharp
var vehicles = await _context.Vehicles
    .Include(v => v.Category)
    .Include(v => v.Owner)
    .ToListAsync();
```

### 2. Khi muốn tìm kiếm bản ghi theo từ khóa (LIKE %keyword%)
$\rightarrow$ **Dùng:** `.Where(x => x.Prop.Contains(keyword))`.
```csharp
if (!string.IsNullOrEmpty(keyword))
{
    query = query.Where(v => v.Name.Contains(keyword) || v.LicensePlate.Contains(keyword));
}
```

### 3. Khi muốn tìm một bản ghi duy nhất theo Khóa chính (Id)
$\rightarrow$ **Dùng:** `_context.Vehicles.FindAsync(id)` (nếu không cần Include) hoặc `.FirstOrDefaultAsync(x => x.Id == id)` (nếu cần Include).
```csharp
var vehicle = await _context.Vehicles
    .Include(v => v.Category)
    .FirstOrDefaultAsync(v => v.Id == id);
```

### 4. Khi muốn sắp xếp dữ liệu (mới nhất lên đầu, giá từ cao xuống thấp)
$\rightarrow$ **Dùng:** `.OrderByDescending(...)` (giảm dần) hoặc `.OrderBy(...)` (tăng dần).
```csharp
var sortedVehicles = await query
    .OrderByDescending(v => v.CreatedAt) // Mới nhất lên đầu
    .ThenBy(v => v.Price)                // Sau đó xếp theo giá rẻ đến đắt
    .ToListAsync();
```

### 5. Khi muốn phân trang dữ liệu (Pagination)
$\rightarrow$ **Dùng:** `.Skip((pageIndex - 1) * pageSize).Take(pageSize)`.
```csharp
int pageSize = 10;   // Số phần tử trên 1 trang
int pageNumber = 1; // Trang muốn xem

var pagedData = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 6. Khi muốn kiểm tra dữ liệu đã tồn tại trong DB hay chưa (Check trùng lặp)
$\rightarrow$ **Dùng:** `.AnyAsync(...)` (trả về `true` hoặc `false`).
```csharp
bool isPlateExists = await _context.Vehicles.AnyAsync(v => v.LicensePlate == plateInput);
if (isPlateExists)
{
    ModelState.AddModelError("LicensePlate", "Biển số xe này đã tồn tại trên hệ thống!");
}
```

### 7. Khi muốn thêm mới, sửa, xóa dữ liệu vào Database
$\rightarrow$ **Dùng:** `_context.Add()`, `_context.Update()`, `_context.Remove()` kết hợp `await _context.SaveChangesAsync()`.
```csharp
// Thêm:
_context.Vehicles.Add(newVehicle);
await _context.SaveChangesAsync();

// Sửa:
_context.Vehicles.Update(vehicleToEdit);
await _context.SaveChangesAsync();

// Xóa:
_context.Vehicles.Remove(vehicleToDelete);
await _context.SaveChangesAsync();
```
