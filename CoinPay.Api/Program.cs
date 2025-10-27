using Microsoft.EntityFrameworkCore;
using CoinPay.Api.Data;
using CoinPay.Api.Models;
using CoinPay.Api.Middleware;
using CoinPay.Api.HealthChecks;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Configure Serilog before building the application
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/coinpay-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting CoinPay API application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

// Add services to the container.
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

// Add DbContext with PostgreSQL database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "db", "ready" });

// Add CORS with environment-specific policies
builder.Services.AddCors(options =>
{
    // Development policy - allows local development servers
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",      // React default
                "http://localhost:5173",      // Vite default
                "http://localhost:5100",      // Custom frontend port
                "http://localhost:5174",      // Additional Vite port
                "http://localhost:4200")      // Angular default
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-Correlation-ID");
    });

    // Production policy - restrict to specific domains
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins(
                "https://app.coinpay.com",
                "https://www.coinpay.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-Correlation-ID");
    });
});

    var app = builder.Build();

    // Apply pending migrations automatically
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        Log.Information("Database migrations applied successfully");
    }

    // Configure the HTTP request pipeline.
    // IMPORTANT: Middleware order matters! Exception handler must be first.

    // 1. Global exception handler - catches all unhandled exceptions
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // 2. CorrelationId middleware - adds tracking to all requests
    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoinPay API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "CoinPay API Documentation";
});

app.UseHttpsRedirection();

// Use environment-specific CORS policy
var corsPolicy = app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy";
app.UseCors(corsPolicy);
Log.Information("Using CORS policy: {CorsPolicy}", corsPolicy);

// Health Check Endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => true,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // No checks, just responds if app is running
});

// API Endpoints

// GET: Get all transactions
app.MapGet("/api/transactions", async (AppDbContext db) =>
{
    var transactions = await db.Transactions.ToListAsync();
    return Results.Ok(transactions);
})
.WithName("GetAllTransactions")
.WithTags("Transactions")
.WithSummary("Get all transactions")
.WithDescription("Retrieves a list of all transactions in the system");

// GET: Get transaction by ID
app.MapGet("/api/transactions/{id}", async (int id, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
})
.WithName("GetTransactionById")
.WithTags("Transactions")
.WithSummary("Get transaction by ID")
.WithDescription("Retrieves a specific transaction by its unique identifier");

// GET: Get transactions by status
app.MapGet("/api/transactions/status/{status}", async (string status, AppDbContext db) =>
{
    var transactions = await db.Transactions
        .Where(t => t.Status.ToLower() == status.ToLower())
        .ToListAsync();
    return Results.Ok(transactions);
})
.WithName("GetTransactionsByStatus")
.WithTags("Transactions")
.WithSummary("Get transactions by status")
.WithDescription("Retrieves all transactions filtered by status (Pending, Completed, Failed)");

// POST: Create a new transaction
app.MapPost("/api/transactions", async (Transaction transaction, AppDbContext db) =>
{
    // Generate transaction ID if not provided
    if (string.IsNullOrEmpty(transaction.TransactionId))
    {
        transaction.TransactionId = $"TXN{DateTime.UtcNow.Ticks}";
    }

    transaction.CreatedAt = DateTime.UtcNow;

    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();

    return Results.Created($"/api/transactions/{transaction.Id}", transaction);
})
.WithName("CreateTransaction")
.WithTags("Transactions")
.WithSummary("Create a new transaction")
.WithDescription("Creates a new transaction with the provided details");

// PUT: Update transaction
app.MapPut("/api/transactions/{id}", async (int id, Transaction updatedTransaction, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null)
    {
        return Results.NotFound();
    }

    transaction.Amount = updatedTransaction.Amount;
    transaction.Currency = updatedTransaction.Currency;
    transaction.Type = updatedTransaction.Type;
    transaction.Status = updatedTransaction.Status;
    transaction.SenderName = updatedTransaction.SenderName;
    transaction.ReceiverName = updatedTransaction.ReceiverName;
    transaction.Description = updatedTransaction.Description;

    if (updatedTransaction.Status == "Completed" && transaction.CompletedAt == null)
    {
        transaction.CompletedAt = DateTime.UtcNow;
    }

    await db.SaveChangesAsync();

    return Results.Ok(transaction);
})
.WithName("UpdateTransaction")
.WithTags("Transactions")
.WithSummary("Update a transaction")
.WithDescription("Updates an existing transaction with new details");

// DELETE: Delete transaction
app.MapDelete("/api/transactions/{id}", async (int id, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null)
    {
        return Results.NotFound();
    }

    db.Transactions.Remove(transaction);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteTransaction")
.WithTags("Transactions")
.WithSummary("Delete a transaction")
.WithDescription("Deletes a transaction from the system");

// PATCH: Update transaction status
app.MapPatch("/api/transactions/{id}/status", async (int id, string status, AppDbContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null)
    {
        return Results.NotFound();
    }

    transaction.Status = status;
    if (status == "Completed" && transaction.CompletedAt == null)
    {
        transaction.CompletedAt = DateTime.UtcNow;
    }

    await db.SaveChangesAsync();

    return Results.Ok(transaction);
})
.WithName("UpdateTransactionStatus")
.WithTags("Transactions")
.WithSummary("Update transaction status")
.WithDescription("Updates the status of a transaction (Pending, Completed, Failed)");

    Log.Information("CoinPay API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
