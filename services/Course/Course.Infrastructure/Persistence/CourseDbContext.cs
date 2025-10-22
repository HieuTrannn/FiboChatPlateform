using Course.Domain.Entities;
using Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace Course.Infrastructure.Persistence
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<Embedding> Embeddings { get; set; }
        public DbSet<Domain.Entities.Domain> Domains { get; set; }
        public DbSet<MasterTopic> MasterTopics { get; set; }
        public DbSet<LecturerMasterTopic> LecturerMasterTopics { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<MasterTopicKeyword> MasterTopicKeywords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Topic>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasMany(x => x.Documents)
                 .WithOne(d => d.Topic)
                 .HasForeignKey(d => d.TopicId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DocumentType>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasMany(x => x.Documents)
                 .WithOne(d => d.DocumentType)
                 .HasForeignKey(d => d.DocumentTypeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Domain.Entities.File>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FileName).IsRequired();
                e.Property(x => x.FilePath).IsRequired();
                e.Property(x => x.FileContentType).IsRequired();
                e.Property(x => x.FileUrl).IsRequired();
                e.Property(x => x.FileKey).IsRequired();
                e.Property(x => x.FileBucket).IsRequired();
                e.Property(x => x.FileRegion).IsRequired();
                e.Property(x => x.FileAcl).IsRequired();
            });

            modelBuilder.Entity<Document>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).IsRequired();
                e.HasOne(x => x.File)
                 .WithMany()
                 .HasForeignKey(x => x.FileId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasMany(x => x.Embeddings)
                 .WithOne(em => em.Document!)
                 .HasForeignKey(em => em.DocumentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Embedding>(e =>
            {
                e.HasKey(x => x.Id);
                // Vector is float[]; for PostgreSQL this maps to array; ensure provider supports it
            });

            modelBuilder.Entity<Domain.Entities.Domain>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.Property(x => x.Description).IsRequired();
                e.Property(x => x.Status)
                    .HasConversion(
                        v => v.ToString(),
                        v => (StaticEnum.StatusEnum)Enum.Parse(typeof(StaticEnum.StatusEnum), v))
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<MasterTopic>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.Property(x => x.Description).IsRequired();

                // Configure Status enum to be stored as string
                e.Property(x => x.Status)
                    .HasConversion(
                        v => v.ToString(),
                        v => (StaticEnum.StatusEnum)Enum.Parse(typeof(StaticEnum.StatusEnum), v))
                    .HasMaxLength(50);

                e.HasOne(x => x.Domain)
                 .WithMany()
                 .HasForeignKey(x => x.DomainId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Remove Semester foreign key constraint - SemesterId is just a reference
                // e.HasOne(x => x.Semester)  // Remove this if it exists
                //  .WithMany()
                //  .HasForeignKey(x => x.SemesterId)
                //  .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Topics)
                 .WithOne()
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.MasterTopicKeywords)
                 .WithOne(mtk => mtk.MasterTopic)
                 .HasForeignKey(mtk => mtk.MasterTopicId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<LecturerMasterTopic>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.LecturerId)
                      .IsRequired();

                e.HasOne(x => x.MasterTopic)
                      .WithMany(mt => mt.LecturerMasterTopics)
                      .HasForeignKey(x => x.MasterTopicId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Keyword>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasMany(x => x.MasterTopicKeywords)
                 .WithOne(mtk => mtk.Keyword)
                 .HasForeignKey(mtk => mtk.KeywordId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MasterTopicKeyword>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.MasterTopicId, x.KeywordId }).IsUnique();
            });
        }


        // // # Từ thư mục gốc FiboChatPlatform
        // dotnet ef migrations add UpdateCourseModel -p services/Course/Course.Infrastructure -s services/Course/Course.Api

        // // Apply migration
        // dotnet ef database update -p services/Course/Course.Infrastructure -s services/Course/Course.Api
    }
}