using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PBL3.Models;

namespace PBL3.Data
{
    public class PBL3Context : DbContext
    {
        public PBL3Context (DbContextOptions<PBL3Context> options)
            : base(options)
        {
        }

        public DbSet<PBL3.Models.Student> Student { get; set; } = default!;
        public DbSet<PBL3.Models.Registered_Student> Registered_Students { get; set; } = default!;
        public DbSet<PBL3.Models.Staff> Staff { get; set; } = default!;
        public DbSet<PBL3.Models.Ticket> Tickets { get; set; } = default!;
    }
}
