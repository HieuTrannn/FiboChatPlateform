using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Đọc config ReverseProxy từ appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gateway API", Version = "v1" });
});

var app = builder.Build();

// Bật swagger (chỉ trong dev và staging)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // 👇 Swagger JSON thật vẫn ở /swagger/v1/swagger.json
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");

        // 👇 Nhưng Swagger UI hiển thị tại /gateway/swagger
        c.RoutePrefix = "gateway/swagger";
    });
}

// Health check
app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// Reverse proxy
app.MapReverseProxy();

app.Run();
