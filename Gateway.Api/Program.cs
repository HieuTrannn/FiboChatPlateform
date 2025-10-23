using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);



// Đọc config ReverseProxy từ appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
var app = builder.Build();

// bật swagger
//if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
//{
//    //app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        if (builder.Environment.IsDevelopment())
//        {
//            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API");
//            c.RoutePrefix = string.Empty;
//        }
//        else
//        {
//            c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Authentication API");
//            c.SwaggerEndpoint("/course/swagger/v1/swagger.json", "Course API");
//            c.RoutePrefix = "gateway";
//        }
//    });
//}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwaggerUI(c =>
    {
        // Các swagger document của service
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Authentication API");
        c.SwaggerEndpoint("/course/swagger/v1/swagger.json", "Course API");

        // Giao diện Swagger UI sẽ hiển thị tại /gateway
        c.RoutePrefix = "gateway";
    });
}


// health check
app.MapGet("/healthz", () => Results.Ok(new { ok = true }));

// Reverse proxy - Forward TẤT CẢ requests
app.MapReverseProxy();

app.Run();