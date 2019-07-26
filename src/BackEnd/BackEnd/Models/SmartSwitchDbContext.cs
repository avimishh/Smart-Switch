using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BackEnd.Models
{
    public class SmartSwitchDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Plug> Plugs { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<PowerUsageSample> PowerUsageSamples { get; set; }

        public SmartSwitchDbContext(DbContextOptions<SmartSwitchDbContext> options)
            : base(options)
        { }
    }
}
