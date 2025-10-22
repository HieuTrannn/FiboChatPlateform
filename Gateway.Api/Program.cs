using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Đọc config ReverseProxy từ appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// bật swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API");
        c.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "Identity API");
        c.SwaggerEndpoint("/kb/swagger/v1/swagger.json", "Knowledge Base API");

        c.RoutePrefix = string.Empty;
    });
}


// health check
app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// Reverse proxy - Forward TẤT CẢ requests
app.MapReverseProxy();

app.Run();