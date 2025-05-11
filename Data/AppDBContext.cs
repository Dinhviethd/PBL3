using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Cấu hình cho Ticket
            builder.Entity<Ticket>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.ParkingSlot)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);

            // Cấu hình cho ParkingSlot
            builder.Entity<ParkingSlot>()
                .HasMany(p => p.Tickets)
                .WithOne(t => t.ParkingSlot)
                .HasForeignKey(t => t.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}