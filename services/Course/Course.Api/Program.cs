using System.Reflection;
using Course.Infrastructure.DependencyInjection;
using Course.Infrastructure.Persistence;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ====== 3) API plumbing ======
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "COURSE API",
        Version = "v1",
        Description = "API for Course Service"
    });
    
    // Add JWT Bearer authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var firebaseConfig = builder.Configuration.GetSection("Firebase");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseConfig["PrivateKey"])
});

var app = builder.Build();

// ====== 4) Auto-migrate (Dev only) ======
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
}

// ====== 5) Middlewares ======
app.UseInfrastructurePolicy();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    if (builder.Environment.IsDevelopment())
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course API v1");
        c.RoutePrefix = string.Empty;
    }
    else
    {
        c.SwaggerEndpoint("/course/swagger/v1/swagger.json", "Course API v1");
        c.RoutePrefix = "course";
    }
});

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
