using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Course.Infrastructure.Persistence
{
    public class CourseDbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
    {
        public CourseDbContext CreateDbContext(string[] args)
        {
            // Point to Course.Api for appsettings
            var basePath = Path.Combine(
                Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
                "Course.Api"
            );

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CourseDbContext>();
            var connectionString = configuration.GetConnectionString("DBConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new CourseDbContext(optionsBuilder.Options);
        }
    }
}