using Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Course.Infrastructure.Persistence
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("course");

            modelBuilder.Entity<Semester>(e =>
            {
                e.Property(x => x.Term).HasConversion<string>();
                e.Property(x => x.Status).HasConversion<string>();
            });
            modelBuilder.Entity<Class>(e =>
            {
                e.Property(x => x.Status).HasConversion<string>();
            });

            base.OnModelCreating(modelBuilder);
        }
    }

}
