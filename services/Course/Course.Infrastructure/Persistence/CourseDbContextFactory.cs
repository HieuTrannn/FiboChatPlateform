using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Course.Infrastructure.Persistence
{
    public class CourseDbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
    {
        public CourseDbContext CreateDbContext(string[] args)
        {
            // Xác định môi trường hiện tại (nếu không có thì mặc định là Development)
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Tìm đường dẫn tới API project (vì appsettings nằm ở đó)
            var basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "Course.API");

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