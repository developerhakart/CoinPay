# Configuration Guide

This guide covers configuration options for the CoinPay API.

## Application Settings

The API uses standard ASP.NET Core configuration files:

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

Environment-specific settings for development.

## Launch Settings

Configure ports and launch profiles in `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:7777",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## CORS Configuration

CORS is configured in `Program.cs` to allow all origins for development:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Production CORS

For production, restrict to specific origins:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## Database Configuration

### Current: In-Memory Database

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CoinPayDb"));
```

### SQL Server

1. Install package:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

2. Update `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

3. Add connection string to `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CoinPayDb;Trusted_Connection=true"
  }
}
```

4. Create and apply migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### PostgreSQL

1. Install package:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

2. Update `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

3. Add connection string to `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=coinpaydb;Username=postgres;Password=yourpassword"
  }
}
```

## Swagger Configuration

Swagger is configured in `Program.cs`:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "CoinPay API",
        Version = "v1",
        Description = "A simple transactions API for CoinPay application",
        Contact = new()
        {
            Name = "CoinPay Team",
            Email = "developerhakart@yahoo.com"
        }
    });
});
```

Access Swagger UI at: `/swagger`

## Logging

### Console Logging (Default)

Configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### File Logging with Serilog

1. Install packages:
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

2. Update `Program.cs`:
```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/coinpay-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

## Environment Variables

Set environment variables for different configurations:

### Development
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

### Production
```bash
export ASPNETCORE_ENVIRONMENT=Production
```

## Port Configuration

Change the default port by updating `launchSettings.json`:

```json
"applicationUrl": "http://localhost:YOUR_PORT"
```

Or use command line:
```bash
dotnet run --urls="http://localhost:8080"
```

## HTTPS Configuration

Enable HTTPS in `launchSettings.json`:

```json
"applicationUrl": "https://localhost:7778;http://localhost:7777"
```

Generate development certificate:
```bash
dotnet dev-certs https --trust
```

## Next Steps

- Review [Getting Started](getting-started.md) guide
- Explore [API Usage Examples](api-examples.md)
- Check the [API Reference](../api/index.md)
