using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using btl_myselt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace btl_myselt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
        {

        }

        public DbSet<Community> Communities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VehicleCategory> VehicleCategories { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<VehicleAvailability> VehicleAvailabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<HandoverRecord> HandoverRecords { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ViolationReport> ViolationReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict); // Giữ lại review kể cả khi user bị xóa
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany()
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Khi xóa User hoặc Vehicle -> KHÔNG ĐƯỢC xóa lịch sử Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Borrower)
                .WithMany()
                .HasForeignKey(b => b.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vehicle)
                .WithMany()
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Khi xóa Vehicle -> KHÔNG ĐƯỢC xóa lịch sử của Chủ xe (Owner)
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Owner)
                .WithMany()
                .HasForeignKey(v => v.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            // Khi xóa Community -> KHÔNG ĐƯỢC xóa User và Vehicle đang thuộc về nó
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Community)
                .WithMany()
                .HasForeignKey(v => v.CommunityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Community)
                .WithMany()
                .HasForeignKey(u => u.CommunityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ViolationReport>()
                .HasOne(vr => vr.Reporter)
                .WithMany()
                .HasForeignKey(vr => vr.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); //xóa User thì xóa thông báo theo
        }
    }
}