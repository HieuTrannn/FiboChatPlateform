using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gateway API", Version = "v1" });

    c.SwaggerDoc("auth", new() { Title = "Authentication API", Version = "v1" });
    c.SwaggerDoc("course", new() { Title = "Course API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");

        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Authentication API v1");
        c.SwaggerEndpoint("/course/swagger/v1/swagger.json", "Course API v1");

        c.RoutePrefix = "swagger"; 
    });
}

// Health check
app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// Reverse proxy
app.MapReverseProxy();

app.Run();