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
        public DbSet<Status> Statuses { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Target>()
                .HasOne(t => t.User)
                .WithMany(u => u.Targets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Target>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Targets)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}