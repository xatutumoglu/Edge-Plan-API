using Microsoft.EntityFrameworkCore;
using EdgePlan.Data.Entity;

namespace EdgePlan.Data.Postgre
{
    public class ApplicationPostgreContext : DbContext
    {
        public ApplicationPostgreContext(DbContextOptions<ApplicationPostgreContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Target> Targets { get; set; }
        
    }
}