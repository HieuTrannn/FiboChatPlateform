using Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Course.Infrastructure.Persistence
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

        public DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("course");
            base.OnModelCreating(modelBuilder);
        }
    }

}
