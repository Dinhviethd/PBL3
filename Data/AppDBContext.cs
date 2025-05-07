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
        public AppDBContext (DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<PBL3.Models.Student> Students { get; set; } = default!;
        public DbSet<PBL3.Models.Staff> Staffs { get; set; } = default!;
        public DbSet<PBL3.Models.Ticket> Tickets { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>()
                .HasKey(s => s.MSSV);
        }
    }
}
