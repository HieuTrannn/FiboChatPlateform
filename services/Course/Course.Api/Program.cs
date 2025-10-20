using Course.Infrastructure.DependencyInjection;
using Course.Infrastructure.Persistence;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ====== 1) Connection string (appsettings.json hoặc ENV) ======
var cs = builder.Configuration.GetConnectionString("DBConnection")
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

// ====== 2) Infrastructure & DI ======
builder.Services.AddDbContext<CourseDbContext>(opt => opt.UseNpgsql(cs));

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
builder.Services.AddSwaggerGen();

var firebaseConfig = builder.Configuration.GetSection("Firebase");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseConfig["PrivateKeyPath"])
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
app.UseSwaggerUI();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
