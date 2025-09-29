using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<Class> Classes => Set<Class>();
        public DbSet<ClassEnrollment> ClassEnrollments => Set<ClassEnrollment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).IsRequired();
                e.Property(x => x.Name).IsRequired();
                e.Property(x => x.Role).IsRequired();
                e.Property(x => x.Status).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
            });

            // Semester
            modelBuilder.Entity<Semester>(e =>
            {
                e.ToTable("semesters");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Code).IsUnique();
                e.Property(x => x.Code).IsRequired();
                e.Property(x => x.Term).IsRequired();
                e.Property(x => x.Year).IsRequired();
                e.Property(x => x.Status).HasDefaultValue("active");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
            });

            // Class
            modelBuilder.Entity<Class>(e =>
            {
                e.ToTable("classes");
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.Code, x.SemesterId }).IsUnique();
                e.Property(x => x.Code).IsRequired();
                e.Property(x => x.Status).HasDefaultValue("active");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
            });

            // ClassEnrollment
            modelBuilder.Entity<ClassEnrollment>(e =>
            {
                e.ToTable("class_enrollments");
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.ClassId, x.UserId }).IsUnique();
                e.Property(x => x.RoleInClass).HasDefaultValue("student");
                e.Property(x => x.Status).HasDefaultValue("active");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}
