using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Implements;
using Identity.Application.Implements;
using Identity.Infrastructure.Interfaces;
using Identity.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// ====== 1) Connection string (appsettings.json hoặc ENV) ======
var cs = builder.Configuration.GetConnectionString("Postgres")
         ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
         ?? "Host=localhost;Port=5433;Database=identitydb;Username=user;Password=pass"; // fallback dev

// ====== 2) Infrastructure & DI ======
builder.Services.AddDbContext<IdentityDbContext>(opt => opt.UseNpgsql(cs));

// Repository & Service DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();

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
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
}

// ====== 5) Middlewares ======
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("dev");
}

app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

app.MapControllers();

app.Run();
