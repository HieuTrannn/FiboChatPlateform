using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Infrastructure.Persistence
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            // Ưu tiên lấy từ ENV để không hardcode (nếu không có thì fallback)
            var cs = Environment.GetEnvironmentVariable("Postgres");
            var opt = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseNpgsql(cs)
                .Options;

            return new IdentityDbContext(opt);
        }
    }
}
