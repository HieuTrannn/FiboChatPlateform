using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.SharedLibrary.MiddleWare;
using Serilog;

namespace SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            //Db Context
            services.AddDbContext<TContext>(option => option.UseNpgsql(
                config
                .GetConnectionString(fileName), posgestserverOption =>
                posgestserverOption.EnableRetryOnFailure()));
            //config serilog 
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ff zzz } [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add JWT authentication
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //Use Global Exception
            app.UseMiddleware<GlobalException>();

            // Register middleware to block all outsiders API calls
            app.UseMiddleware<ListenToOnlyApiGateWay>();

            return app;
        }
    }
}
