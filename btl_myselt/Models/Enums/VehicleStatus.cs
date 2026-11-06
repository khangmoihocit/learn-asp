namespace btl_myselt.Models.Enums
{
    public enum VehicleStatus
    {
        Available = 0,   // Đang rảnh sẵn sàng cho thuê
        Rented = 1,      // Đang có người thuê
        Maintenance = 2, // Đang bảo trì / hỏng
        Hidden = 3       // Chủ xe tạm ẩn
    }
}