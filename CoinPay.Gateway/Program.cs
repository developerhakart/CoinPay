var builder = WebApplication.CreateBuilder(args);

// Add YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Welcome page
app.MapGet("/", () => Results.Json(new
{
    service = "CoinPay Gateway",
    version = "1.0.0",
    endpoints = new
    {
        api = "/api/transactions",
        swagger = "/swagger",
        documentation = "/docs"
    }
}));

// Map reverse proxy
app.MapReverseProxy();

app.Run();
