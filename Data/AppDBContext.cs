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
        public AppDBContext (DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<PBL3.Models.Student> Student { get; set; } = default!;
        public DbSet<PBL3.Models.Staff> Staff { get; set; } = default!;
        public DbSet<PBL3.Models.Ticket> Tickets { get; set; } = default!;
    }
}
