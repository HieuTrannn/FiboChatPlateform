using Contracts.Common;
using Course.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.DependencyInjection;
using Course.Domain.Abstraction;
using Course.Infrastructure.Implements;
using Course.Application.Interfaces;
using Course.Application.Implements;

namespace Course.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add db connection
            SharedServiceContainer.AddSharedServices<CourseDbContext>(services, config, config["MySerilog:Filename"]);

            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped<IDomainService, DomainService>();
            services.AddScoped<IKeywordService, KeywordService>();
            services.AddScoped<IMasterTopicService, MasterTopicService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IMasterTopicService, MasterTopicService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IFirebaseService, FirebaseService>();

            services.AddHttpClient<IExternalApiService, ExternalApiService>();
            services.AddScoped<IExternalApiService, ExternalApiService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Lấy lỗi đầu tiên (theo thứ tự field)
                    var firstError = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Error = x.Value!.Errors.First().ErrorMessage })
                        .FirstOrDefault();

                    var errorData = firstError != null
                        ? new { Field = firstError.Field, Error = firstError.Error }
                        : null;

                    var response = new ApiResponse<object>(
                        statusCode: 400,
                        code: "ValidationError",
                        message: "Invalid input data",
                        data: errorData
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}