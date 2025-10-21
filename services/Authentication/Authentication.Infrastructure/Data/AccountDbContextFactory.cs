using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Authentication.Infrastructure
{
    public class AccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
    {
        public AccountDbContext CreateDbContext(string[] args)
        {
            // Xác định môi trường hiện tại (nếu không có thì mặc định là Development)
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Tìm đường dẫn tới API project (vì appsettings nằm ở đó)
            var basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "Authentication.API");

            // Build configuration theo môi trường
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
            var connectionString = configuration.GetConnectionString("DbConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new AccountDbContext(optionsBuilder.Options);
        }
    }
}
