using Course.Infrastructure.Implements;
using Course.Infrastructure.Interfaces;
using Course.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Course.Application.Interfaces;
using Course.Application.Implements;

var builder = WebApplication.CreateBuilder(args);

// ====== 1) Connection string (appsettings.json hoặc ENV) ======
var cs = builder.Configuration.GetConnectionString("CourseDb")
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

// ====== 2) Infrastructure & DI ======
builder.Services.AddDbContext<CourseDbContext>(opt => opt.UseNpgsql(cs));

// Repository & Service DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IClassService, ClassService>();

var authBase = builder.Configuration["Services:AuthBaseUrl"];
builder.Services.AddHttpClient<IAccountsClient, AccountsClient>(c =>
{
	if (!Uri.TryCreate(authBase, UriKind.Absolute, out var uri))
		throw new InvalidOperationException("Invalid Services:AuthBaseUrl (e.g., https://localhost:7047)");
	c.BaseAddress = uri;
});

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
