using Course.Infrastructure.Implements;
using Course.Infrastructure.Interfaces;
using Course.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ====== 1) Connection string (appsettings.json hoặc ENV) ======
var cs = builder.Configuration.GetConnectionString("CourseDb")
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

// ====== 2) Infrastructure & DI ======
builder.Services.AddDbContext<CourseDbContext>(opt => opt.UseNpgsql(cs));

// Repository & Service DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var authBase = builder.Configuration["Services:AuthBaseUrl"];


// ====== 3) API plumbing ======
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// (tuỳ chọn) CORS dev
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ====== 4) Auto-migrate (Dev only) ======
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
}

// ====== 5) Middlewares ======
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("dev");

app.MapControllers();

app.Run();
