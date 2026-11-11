using System;
using System.Collections.Generic;
using System.Linq;
using btl_myselt.Helpers;
using btl_myselt.Models.Entities;
using btl_myselt.Models.Enums;

namespace btl_myselt.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Đảm bảo Database đã được tạo
            context.Database.EnsureCreated();

            // 1. SEED COMMUNITIES (10 khu dân cư lớn)
            if (!context.Communities.Any())
            {
                var communities = new List<Community>
                {
                    new Community { Name = "Vinhomes Ocean Park 1", Address = "Gia Lâm, Hà Nội", IsActive = true },
                    new Community { Name = "Vinhomes Smart City", Address = "Tây Mỗ, Nam Từ Liêm, Hà Nội", IsActive = true },
                    new Community { Name = "Ecopark Hưng Yên", Address = "Văn Giang, Hưng Yên", IsActive = true },
                    new Community { Name = "Times City", Address = "458 Minh Khai, Hai Bà Trưng, Hà Nội", IsActive = true },
                    new Community { Name = "Royal City", Address = "72A Nguyễn Trãi, Thanh Xuân, Hà Nội", IsActive = true },
                    new Community { Name = "Ciputra Hà Nội", Address = "Xuân Đỉnh, Bắc Từ Liêm, Hà Nội", IsActive = true },
                    new Community { Name = "Chung cư Ngoại Giao Đoàn", Address = "Xuân Tảo, Bắc Từ Liêm, Hà Nội", IsActive = true },
                    new Community { Name = "Goldmark City", Address = "136 Hồ Tùng Mậu, Bắc Từ Liêm, Hà Nội", IsActive = true },
                    new Community { Name = "Imperia Smart City", Address = "Tây Mỗ, Nam Từ Liêm, Hà Nội", IsActive = true },
                    new Community { Name = "Starlake Tây Hồ Tây", Address = "Xuân Tảo, Bắc Từ Liêm, Hà Nội", IsActive = true }
                };
                context.Communities.AddRange(communities);
                context.SaveChanges();
            }

            // 2. SEED VEHICLE CATEGORIES (6 loại xe)
            if (!context.VehicleCategories.Any())
            {
                var categories = new List<VehicleCategory>
                {
                    new VehicleCategory { Name = "Xe máy số", Description = "Tiết kiệm xăng, thích hợp đi làm hàng ngày" },
                    new VehicleCategory { Name = "Xe máy tay ga", Description = "Êm ái, cốp rộng, phù hợp dạo phố" },
                    new VehicleCategory { Name = "Xe máy điện", Description = "Thân thiện môi trường, êm ái tiện lợi" },
                    new VehicleCategory { Name = "Ô tô sedan 4-5 chỗ", Description = "Gia đình nhỏ, đi công việc tiện nghi" },
                    new VehicleCategory { Name = "Ô tô SUV 7 chỗ", Description = "Gầm cao, rộng rãi cho gia đình đi xa" },
                    new VehicleCategory { Name = "Xe thể thao / Sang trọng", Description = "Trải nghiệm lái cao cấp, sự kiện đặc biệt" }
                };
                context.VehicleCategories.AddRange(categories);
                context.SaveChanges();
            }

            if (context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com") == null)
            {
                var defaultPass = PasswordHelper.HashPassword("123456");
                var comIds = context.Communities.Select(c => c.Id).ToList();
                var user = new User
                {
                    FullName = "Quản trị viên",
                    Email = "admin@gmail.com",
                    PhoneNumber = "0901234567",
                    Password = defaultPass,
                    Role = "ADMIN",
                    MemberStatus = MemberStatus.Active,
                    CommunityId = comIds[1]
                };
                context.Users.Add(user);
                context.SaveChanges();
            }

            // 3. SEED USERS (12 người dùng)
            if (!context.Users.Any())
            {
                var comIds = context.Communities.Select(c => c.Id).ToList();
                var users = new List<User>
                {
                    new User { FullName = "Nguyễn Văn An", Email = "an.nguyen@gmail.com", PhoneNumber = "0981112221", CommunityId = comIds[0], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Trần Thị Bình", Email = "binh.tran@gmail.com", PhoneNumber = "0981112222", CommunityId = comIds[1], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Lê Hoàng Cường", Email = "cuong.le@gmail.com", PhoneNumber = "0981112223", CommunityId = comIds[2], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Phạm Thị Dung", Email = "dung.pham@gmail.com", PhoneNumber = "0981112224", CommunityId = comIds[3], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Hoàng Văn Em", Email = "em.hoang@gmail.com", PhoneNumber = "0981112225", CommunityId = comIds[4], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Vũ Đình Giang", Email = "giang.vu@gmail.com", PhoneNumber = "0981112226", CommunityId = comIds[5], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Đặng Thị Hoa", Email = "hoa.dang@gmail.com", PhoneNumber = "0981112227", CommunityId = comIds[6], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Bùi Quốc Hùng", Email = "hung.bui@gmail.com", PhoneNumber = "0981112228", CommunityId = comIds[7], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Ngô Mai Lan", Email = "lan.ngo@gmail.com", PhoneNumber = "0981112229", CommunityId = comIds[8], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Đỗ Minh Khang", Email = "khang.do@gmail.com", PhoneNumber = "0981112230", CommunityId = comIds[9], MemberStatus = MemberStatus.Active },
                    new User { FullName = "Trịnh Thu Nga", Email = "nga.trinh@gmail.com", PhoneNumber = "0981112231", CommunityId = comIds[0], MemberStatus = MemberStatus.Pending },
                    new User { FullName = "Phan Văn Phong", Email = "phong.phan@gmail.com", PhoneNumber = "0981112232", CommunityId = comIds[1], MemberStatus = MemberStatus.Active }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // 4. SEED VEHICLES & VEHICLE IMAGES (12 xe kèm đúng ảnh từ wwwroot/images/vehicles/)
            if (!context.Vehicles.Any())
            {
                var coms = context.Communities.ToList();
                var cats = context.VehicleCategories.ToList();
                var owners = context.Users.ToList();

                var vehicleData = new[]
                {
                    new { Name = "Honda Vision 2023 Smartkey", Cat = cats[1].Id, Com = coms[0].Id, Owner = owners[0].Id, Price = 120000m, Img = "/images/vehicles/honda-vision.jpg", Cond = "Xe mới 98%, phanh ABS, bảo dưỡng định kỳ", Share = ShareType.Rental },
                    new { Name = "Yamaha NVX 155 VVA", Cat = cats[1].Id, Com = coms[1].Id, Owner = owners[1].Id, Price = 150000m, Img = "/images/vehicles/yamaha-nvx.jpg", Cond = "Máy bốc, lốp mới thay, cốp to", Share = ShareType.Rental },
                    new { Name = "Yamaha Janus 125cc", Cat = cats[1].Id, Com = coms[2].Id, Owner = owners[2].Id, Price = 100000m, Img = "/images/vehicles/yamaha-janus.png", Cond = "Xe nữ đi giữ gìn, êm ái", Share = ShareType.Rental },
                    new { Name = "Honda Super Cub C125", Cat = cats[0].Id, Com = coms[3].Id, Owner = owners[3].Id, Price = 80000m, Img = "/images/vehicles/honda-cube.png", Cond = "Cổ điển sang trọng, cực kỳ tiết kiệm xăng", Share = ShareType.Rental },
                    new { Name = "Xe máy điện Honda e-Scooter", Cat = cats[2].Id, Com = coms[4].Id, Owner = owners[4].Id, Price = 70000m, Img = "/images/vehicles/honda-electric.png", Cond = "Pin đi được 80km/lần sạc, kèm sạc zin", Share = ShareType.Free },
                    new { Name = "Toyota Vios 1.5G 2022", Cat = cats[3].Id, Com = coms[0].Id, Owner = owners[5].Id, Price = 600000m, Img = "/images/vehicles/toyota-vios.jpg", Cond = "Số tự động, camera lùi, nội thất da sạch sẽ", Share = ShareType.Rental },
                    new { Name = "Hyundai Elantra 2.0 AT", Cat = cats[3].Id, Com = coms[1].Id, Owner = owners[6].Id, Price = 700000m, Img = "/images/vehicles/hyundai-elantra.png", Cond = "Cửa sổ trời, màn hình Apple CarPlay, cách âm tốt", Share = ShareType.Rental },
                    new { Name = "Mazda CX-5 2.0 Luxury", Cat = cats[4].Id, Com = coms[2].Id, Owner = owners[7].Id, Price = 900000m, Img = "/images/vehicles/mazda-cx5.jpg", Cond = "Gầm cao 5 chỗ, loa Bose, cốp điện", Share = ShareType.Rental },
                    new { Name = "VinFast VF e34 Điện Thông Minh", Cat = cats[4].Id, Com = coms[3].Id, Owner = owners[8].Id, Price = 500000m, Img = "/images/vehicles/vinfast-vfe34.png", Cond = "Trợ lý ảo thông minh, sạc miễn phí tại chung cư", Share = ShareType.Rental },
                    new { Name = "Toyota GT86 Thể Thao", Cat = cats[5].Id, Com = coms[4].Id, Owner = owners[9].Id, Price = 1500000m, Img = "/images/vehicles/toyota-gt86.png", Cond = "Dẫn động cầu sau, pô thể thao phấn khích", Share = ShareType.Rental },
                    new { Name = "Ferrari 488 GTB Spider", Cat = cats[5].Id, Com = coms[0].Id, Owner = owners[0].Id, Price = 5000000m, Img = "/images/vehicles/ferrari-488.png", Cond = "Siêu xe phục vụ quay MV / Đám cưới sang trọng", Share = ShareType.Rental },
                    new { Name = "McLaren 720S Performance", Cat = cats[5].Id, Com = coms[1].Id, Owner = owners[1].Id, Price = 6000000m, Img = "/images/vehicles/mclaren-720s.png", Cond = "Cửa mở cánh bướm, mui trần thời thượng", Share = ShareType.Rental }
                };

                foreach (var item in vehicleData)
                {
                    var vehicle = new Vehicle
                    {
                        Name = item.Name,
                        CategoryId = item.Cat,
                        CommunityId = item.Com,
                        OwnerId = item.Owner,
                        Price = item.Price,
                        Condition = item.Cond,
                        ShareType = item.Share,
                        Status = VehicleStatus.Available,
                        Description = $"Phương tiện chính chủ tại khu dân cư, thủ tục nhận xe nhanh gọn 5 phút."
                    };
                    context.Vehicles.Add(vehicle);
                    context.SaveChanges();

                    // Thêm ảnh tương ứng vào bảng VehicleImages
                    context.VehicleImages.Add(new VehicleImage
                    {
                        VehicleId = vehicle.Id,
                        ImageUrl = item.Img,
                        IsPrimary = true
                    });
                }
                context.SaveChanges();
            }

            // 5. SEED BOOKINGS (10 giao dịch thuê xe)
            if (!context.Bookings.Any())
            {
                var vehicles = context.Vehicles.ToList();
                var users = context.Users.ToList();

                var bookings = new List<Booking>
                {
                    new Booking { VehicleId = vehicles[0].Id, BorrowerId = users[1].Id, StartAt = DateTime.Now.AddDays(-5), EndAt = DateTime.Now.AddDays(-4), EstimatedFee = 120000m, Status = BookingStatus.Completed, CreatedAt = DateTime.Now.AddDays(-6) },
                    new Booking { VehicleId = vehicles[1].Id, BorrowerId = users[2].Id, StartAt = DateTime.Now.AddDays(-3), EndAt = DateTime.Now.AddDays(-2), EstimatedFee = 150000m, Status = BookingStatus.Completed, CreatedAt = DateTime.Now.AddDays(-4) },
                    new Booking { VehicleId = vehicles[5].Id, BorrowerId = users[3].Id, StartAt = DateTime.Now.AddDays(-2), EndAt = DateTime.Now.AddDays(-1), EstimatedFee = 600000m, Status = BookingStatus.Completed, CreatedAt = DateTime.Now.AddDays(-3) },
                    new Booking { VehicleId = vehicles[7].Id, BorrowerId = users[4].Id, StartAt = DateTime.Now.AddDays(-1), EndAt = DateTime.Now.AddDays(1), EstimatedFee = 1800000m, Status = BookingStatus.Ongoing, CreatedAt = DateTime.Now.AddDays(-2) },
                    new Booking { VehicleId = vehicles[2].Id, BorrowerId = users[5].Id, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddDays(2), EstimatedFee = 100000m, Status = BookingStatus.Confirmed, CreatedAt = DateTime.Now.AddDays(-1) },
                    new Booking { VehicleId = vehicles[3].Id, BorrowerId = users[6].Id, StartAt = DateTime.Now.AddDays(2), EndAt = DateTime.Now.AddDays(3), EstimatedFee = 80000m, Status = BookingStatus.Pending, CreatedAt = DateTime.Now },
                    new Booking { VehicleId = vehicles[6].Id, BorrowerId = users[7].Id, StartAt = DateTime.Now.AddDays(3), EndAt = DateTime.Now.AddDays(5), EstimatedFee = 1400000m, Status = BookingStatus.Pending, CreatedAt = DateTime.Now },
                    new Booking { VehicleId = vehicles[8].Id, BorrowerId = users[0].Id, StartAt = DateTime.Now.AddDays(-10), EndAt = DateTime.Now.AddDays(-9), EstimatedFee = 500000m, Status = BookingStatus.Completed, CreatedAt = DateTime.Now.AddDays(-11) },
                    new Booking { VehicleId = vehicles[4].Id, BorrowerId = users[8].Id, StartAt = DateTime.Now.AddDays(1), EndAt = DateTime.Now.AddDays(1).AddHours(4), EstimatedFee = 0m, Status = BookingStatus.Confirmed, CreatedAt = DateTime.Now },
                    new Booking { VehicleId = vehicles[9].Id, BorrowerId = users[2].Id, StartAt = DateTime.Now.AddDays(-7), EndAt = DateTime.Now.AddDays(-6), EstimatedFee = 1500000m, Status = BookingStatus.Completed, CreatedAt = DateTime.Now.AddDays(-8) }
                };
                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }

            // 6. SEED HANDOVER RECORDS (Biên bản bàn giao cho các đơn Completed & Ongoing)
            if (!context.HandoverRecords.Any())
            {
                var bookings = context.Bookings.Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Ongoing).ToList();

                var handovers = new List<HandoverRecord>();
                foreach (var b in bookings)
                {
                    handovers.Add(new HandoverRecord
                    {
                        BookingId = b.Id,
                        HandedOverAt = b.StartAt,
                        ReturnedAt = b.Status == BookingStatus.Completed ? b.EndAt : null,
                        ConditionBefore = "Xe sạch sẽ, đầy bình xăng, lốp căng, công tơ mét chuẩn.",
                        ConditionAfter = b.Status == BookingStatus.Completed ? "Đã kiểm tra xe nguyên vẹn, không có trầy xước phát sinh." : null
                    });
                }
                context.HandoverRecords.AddRange(handovers);
                context.SaveChanges();
            }

            // 7. SEED REVIEWS (Đánh giá sau khi hoàn thành đơn thuê)
            if (!context.Reviews.Any())
            {
                var completedBookings = context.Bookings
                    .Where(b => b.Status == BookingStatus.Completed)
                    .Select(b => new { b.Id, b.BorrowerId, b.VehicleId })
                    .ToList();

                var reviews = new List<Review>();
                int count = 1;
                foreach (var b in completedBookings)
                {
                    var vehicle = context.Vehicles.FirstOrDefault(v => v.Id == b.VehicleId);
                    if (vehicle != null)
                    {
                        reviews.Add(new Review
                        {
                            BookingId = b.Id,
                            ReviewerId = b.BorrowerId,       // Người thuê đánh giá
                            RevieweeId = vehicle.OwnerId,     // Chủ xe được nhận đánh giá
                            Rating = count % 2 == 0 ? 5 : 4,
                            Comment = count % 2 == 0
                                ? "Chủ xe siêu nhiệt tình, xe chạy rất êm và tiết kiệm xăng. Lần sau sẽ ủng hộ tiếp!"
                                : "Xe sạch sẽ, giao nhận xe đúng giờ, thủ tục rất nhanh gọn lẹ.",
                            CreatedAt = DateTime.Now.AddDays(-count)
                        });
                        count++;
                    }
                }
                context.Reviews.AddRange(reviews);
                context.SaveChanges();
            }

            // 8. SEED NOTIFICATIONS (Thông báo người dùng)
            if (!context.Notifications.Any())
            {
                var users = context.Users.ToList();
                var notifications = new List<Notification>
                {
                    new Notification { UserId = users[0].Id, Content = "Chào mừng bạn đến với mạng lưới thuê xe láng giềng NeighborRide!", Type = "System", IsRead = true, CreatedAt = DateTime.Now.AddDays(-10) },
                    new Notification { UserId = users[0].Id, Content = "Xe Honda Vision của bạn đã có người đặt thuê thành công.", Type = "Booking", IsRead = true, CreatedAt = DateTime.Now.AddDays(-5) },
                    new Notification { UserId = users[1].Id, Content = "Yêu cầu thuê xe của bạn đã được chủ xe chấp thuận.", Type = "Booking", IsRead = false, CreatedAt = DateTime.Now.AddDays(-1) },
                    new Notification { UserId = users[2].Id, Content = "Bạn có 1 đánh giá 5 sao mới từ người hàng xóm cùng khu dân cư.", Type = "Review", IsRead = false, CreatedAt = DateTime.Now.AddHours(-3) },
                    new Notification { UserId = users[3].Id, Content = "Đơn thuê xe Toyota Vios của bạn đã hoàn thành xuất sắc.", Type = "Booking", IsRead = true, CreatedAt = DateTime.Now.AddDays(-2) },
                    new Notification { UserId = users[4].Id, Content = "Nhắc nhở: Lịch bàn giao xe Mazda CX-5 sắp diễn ra lúc 14:00.", Type = "Reminder", IsRead = false, CreatedAt = DateTime.Now.AddHours(-1) },
                    new Notification { UserId = users[5].Id, Content = "Tài khoản của bạn đã được xác minh cư dân thành công.", Type = "System", IsRead = true, CreatedAt = DateTime.Now.AddDays(-7) },
                    new Notification { UserId = users[6].Id, Content = "Chủ xe đã cập nhật tình trạng bàn giao xe.", Type = "Booking", IsRead = true, CreatedAt = DateTime.Now.AddHours(-12) },
                    new Notification { UserId = users[7].Id, Content = "Cảm ơn bạn đã tham gia cộng đồng chia sẻ xe văn minh!", Type = "System", IsRead = true, CreatedAt = DateTime.Now.AddDays(-4) },
                    new Notification { UserId = users[8].Id, Content = "Yêu cầu thuê xe điện VinFast đang chờ bạn phê duyệt.", Type = "Booking", IsRead = false, CreatedAt = DateTime.Now.AddMinutes(-30) }
                };
                context.Notifications.AddRange(notifications);
                context.SaveChanges();
            }

            // 9. SEED VIOLATION REPORTS (Báo cáo vi phạm mẫu)
            if (!context.ViolationReports.Any())
            {
                var users = context.Users.ToList();
                var reports = new List<ViolationReport>
                {
                    new ViolationReport
                    {
                        ReporterId = users[0].Id,
                        TargetType = "User",
                        TargetId = users[10].Id,
                        Reason = "Trễ hẹn bàn giao xe hơn 2 tiếng không báo trước",
                        Description = "Người thuê hẹn 8h sáng trả xe nhưng đến 10h30 mới tới và không nghe máy.",
                        Status = "Pending",
                        AdminNote = "Đang liên hệ nhắc nhở thành viên."
                    },
                    new ViolationReport
                    {
                        ReporterId = users[2].Id,
                        TargetType = "Vehicle",
                        TargetId = 1,
                        Reason = "Thông tin mô tả xe chưa chính xác",
                        Description = "Xe ghi đời 2023 nhưng thực tế là đăng ký năm 2021.",
                        Status = "Resolved",
                        AdminNote = "Đã yêu cầu chủ xe chụp lại cavet xe và sửa lại mô tả."
                    }
                };
                context.ViolationReports.AddRange(reports);
                context.SaveChanges();
            }
        }
    }
}
