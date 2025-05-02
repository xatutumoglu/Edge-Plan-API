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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Config'ler burada: Fluent API, Relations vs.
        }
    }
}