namespace btl_myselt.Models.Enums
{
    public enum BookingStatus
    {
        Pending = 0,    // Chờ chủ xe duyệt
        Confirmed = 1,  // Chủ xe đã đồng ý
        Ongoing = 2,    // Đang trong thời gian giao nhận / đi xe
        Completed = 3,  // Đã trả xe thành công
        Cancelled = 4,  // Đã hủy
        Rejected = 5    // Chủ xe từ chối
    }
}