using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PBL3.Models;

namespace PBL3.Data
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                    .HasDiscriminator<string>("UserType")
                    .HasValue<AppUser>("AppUser")
                    .HasValue<Staff>("Staff")
                    .HasValue<Student>("Student");
            // Cấu hình cho Student và Ticket
            builder.Entity<Student>()
                .HasMany(s => s.Tickets) //Student có thể có nhiều hoặc không có Ticket
                .WithOne(t => t.Student)    //Mỗi Ticket chỉ thuộc về 1 Student
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho ParkingSlot và Ticket
            builder.Entity<ParkingSlot>()
                .HasMany(p => p.Tickets) //ParkingSlot có thể có nhiều hoặc không có Ticket
                .WithOne(t => t.ParkingSlot)   
                .HasForeignKey(t => t.ParkingSlotId)
                .IsRequired(false)  // Cho phép Ticket không có ParkingSlot
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);
        }
    }
}