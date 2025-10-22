using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Đọc config ReverseProxy từ appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Health check
app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// Reverse proxy - Forward TẤT CẢ requests
app.MapReverseProxy();

app.Run();