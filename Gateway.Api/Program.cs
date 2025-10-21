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

// bật swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API");
        c.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "Identity API");
        c.SwaggerEndpoint("/kb/swagger/v1/swagger.json", "Knowledge Base API");
    });
}


// health check
app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// map reverse proxy
app.MapReverseProxy();

app.Run();
