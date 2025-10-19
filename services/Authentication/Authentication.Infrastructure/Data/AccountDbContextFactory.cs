//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace Authentication.Infrastructure.Data
//{
//    public class AccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
//    {
//        public AccountDbContext CreateDbContext(string[] args)
//        {
//            // Lấy thư mục API từ folder Infrastructure
//            var basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "API");

//            IConfigurationRoot configuration = new ConfigurationBuilder()
//                .SetBasePath(basePath)
//                .AddJsonFile("appsettings.json", optional: false)
//                .Build();

//            var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
//            var connectionString = configuration.GetConnectionString("DbConnection");

//            optionsBuilder.UseNpgsql(connectionString);

//            return new AccountDbContext(optionsBuilder.Options);
//        }
//    }
//}
