using Microsoft.EntityFrameworkCore;
using CoinPay.Api.Data;
using CoinPay.Api.Models;
using CoinPay.Api.Middleware;
using CoinPay.Api.HealthChecks;
using CoinPay.Api.Services.Auth;
using CoinPay.Api.Services.Circle;
using CoinPay.Api.Services.Wallet;
using CoinPay.Api.Services.Blockchain;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Caching;
using CoinPay.Api.Services.BackgroundWorkers;
using CoinPay.Api.Services.Transaction;
using CoinPay.Api.Services.Encryption;
using CoinPay.Api.Services.BankAccount;
using CoinPay.Api.Services.FiatGateway;
using CoinPay.Api.Services.ExchangeRate;
using CoinPay.Api.Services.Fees;
using StackExchange.Redis;
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

    // Load configuration settings
    var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
    var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();

    Log.Information("API Settings: BaseUrl={BaseUrl}, Port={Port}", apiSettings.BaseUrl, apiSettings.Port);
    Log.Information("CORS Settings: AllowedOrigins Count={Count}", corsSettings.AllowedOrigins.Length);
    if (corsSettings.AllowedOrigins.Length > 0)
    {
        Log.Information("CORS Allowed Origins: {Origins}", string.Join(", ", corsSettings.AllowedOrigins));
    }

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

// Add Redis caching
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString);
            configuration.AbortOnConnectFail = false; // Don't throw if Redis is unavailable
            configuration.ConnectTimeout = 5000; // 5 second timeout
            return ConnectionMultiplexer.Connect(configuration);
        });
        builder.Services.AddScoped<ICachingService, RedisCachingService>();
        Log.Information("Redis caching configured: {RedisConnection}", redisConnectionString);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to configure Redis. Caching will be disabled.");
    }
}
else
{
    Log.Warning("Redis connection string not found. Caching will be disabled.");
}

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "db", "ready" });

// Add CORS with environment-specific policies (read from configuration)
builder.Services.AddCors(options =>
{
    // Development policy - read from configuration
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        var origins = corsSettings.AllowedOrigins;
        if (origins.Length > 0)
        {
            policy.WithOrigins(origins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Correlation-ID");
            Log.Information("CORS DevelopmentPolicy configured with {Count} allowed origins", origins.Length);
        }
        else
        {
            // Fallback: Allow all origins in development if not configured
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("X-Correlation-ID");
            Log.Warning("CORS DevelopmentPolicy configured to allow ANY origin (no origins specified in configuration)");
        }
    });

    // Production policy - read from configuration
    options.AddPolicy("ProductionPolicy", policy =>
    {
        var origins = corsSettings.AllowedOrigins;
        if (origins.Length > 0)
        {
            policy.WithOrigins(origins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Correlation-ID");
            Log.Information("CORS ProductionPolicy configured with {Count} allowed origins", origins.Length);
        }
        else
        {
            // Production MUST have origins configured
            Log.Error("CORS AllowedOrigins must be configured in Production environment");
            throw new InvalidOperationException("CORS AllowedOrigins must be configured in Production. Please update appsettings.Production.json.");
        }
    });
});

// Configure Circle SDK options
builder.Services.Configure<CircleOptions>(
    builder.Configuration.GetSection("Circle"));

// Configure JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Register services
// Use MockCircleService for MVP testing (no real Circle API calls)
builder.Services.AddScoped<ICircleService, MockCircleService>();
// For production with real Circle API:
// builder.Services.AddScoped<ICircleService, CircleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IWalletService, WalletService>();

// Use MockBlockchainRpcService for MVP testing
builder.Services.AddScoped<IBlockchainRpcService, MockBlockchainRpcService>();
// For production with real blockchain RPC:
// builder.Services.AddScoped<IBlockchainRpcService, BlockchainRpcService>();

// Register repositories
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, CoinPay.Api.Repositories.TransactionRepository>();
builder.Services.AddScoped<CoinPay.Api.Repositories.IWebhookRepository, CoinPay.Api.Repositories.WebhookRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IPayoutRepository, PayoutRepository>();

// Register encryption service (Phase 3)
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();

// Register bank account validation service (Phase 3)
builder.Services.AddScoped<IBankAccountValidationService, BankAccountValidationService>();

// Register fiat gateway services (Phase 3)
// Use MockFiatGatewayService for MVP testing
builder.Services.AddScoped<IFiatGatewayService, MockFiatGatewayService>();
// For production with real gateway:
// builder.Services.AddScoped<IFiatGatewayService, FiatGatewayService>();

// Register memory cache for exchange rate service (Phase 3)
builder.Services.AddMemoryCache();

// Register new exchange rate service with memory cache (Phase 3)
builder.Services.AddScoped<CoinPay.Api.Services.ExchangeRate.IExchangeRateService, CoinPay.Api.Services.ExchangeRate.ExchangeRateService>();

// Register legacy exchange rate service for fiat gateway (Phase 3)
// This uses Redis caching and delegates to IFiatGatewayService
builder.Services.AddScoped<CoinPay.Api.Services.FiatGateway.IExchangeRateService, CoinPay.Api.Services.FiatGateway.ExchangeRateService>();

// Register conversion fee calculator (Phase 3)
builder.Services.AddScoped<CoinPay.Api.Services.Fees.IConversionFeeCalculator, CoinPay.Api.Services.Fees.ConversionFeeCalculator>();

// Register transaction services
builder.Services.AddScoped<ITransactionStatusService, TransactionStatusService>();

// Register webhook service
builder.Services.AddScoped<CoinPay.Api.Services.Webhook.IWebhookService, CoinPay.Api.Services.Webhook.WebhookService>();

// Register UserOperation and Paymaster services
builder.Services.AddScoped<CoinPay.Api.Services.UserOperation.IUserOperationService, CoinPay.Api.Services.UserOperation.UserOperationService>();

// Use MockPaymasterService for MVP testing (no real paymaster calls)
builder.Services.AddScoped<CoinPay.Api.Services.Paymaster.IPaymasterService, CoinPay.Api.Services.Paymaster.MockPaymasterService>();
// For production with real Circle Paymaster:
// builder.Services.AddScoped<CoinPay.Api.Services.Paymaster.IPaymasterService, CoinPay.Api.Services.Paymaster.PaymasterService>();

// Configure HTTP clients for Circle services
builder.Services.AddHttpClient("CircleBundler", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Circle:BundlerUrl"] ?? "https://bundler.circle.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Circle:ApiKey"]}");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("CirclePaymaster", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Circle:PaymasterUrl"] ?? "https://paymaster.circle.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Circle:ApiKey"]}");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add MVC Controllers for new endpoints
builder.Services.AddControllers();

// Register background services
builder.Services.AddHostedService<TransactionMonitoringService>();
Log.Information("Transaction Monitoring background service registered");

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

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

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

// Map controllers for transaction endpoints
app.MapControllers();

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

// ============================================================================
// USER ENDPOINTS (Test/Development)
// ============================================================================

// GET: Get all users
app.MapGet("/api/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
})
.WithName("GetAllUsers")
.WithTags("Users")
.WithSummary("Get all users")
.WithDescription("Retrieves a list of all users in the system");

// GET: Get user by ID
app.MapGet("/api/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUserById")
.WithTags("Users")
.WithSummary("Get user by ID")
.WithDescription("Retrieves a specific user by their unique identifier");

// GET: Get user by username
app.MapGet("/api/users/username/{username}", async (string username, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
    return user is not null ? Results.Ok(user) : Results.NotFound();
})
.WithName("GetUserByUsername")
.WithTags("Users")
.WithSummary("Get user by username")
.WithDescription("Retrieves a specific user by their username");

// POST: Create a new user
app.MapPost("/api/users", async (User user, AppDbContext db) =>
{
    // Set creation timestamp
    user.CreatedAt = DateTime.UtcNow;

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", user);
})
.WithName("CreateUser")
.WithTags("Users")
.WithSummary("Create a new user")
.WithDescription("Creates a new user with the provided details");

// PUT: Update user
app.MapPut("/api/users/{id}", async (int id, User updatedUser, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null)
    {
        return Results.NotFound();
    }

    user.Username = updatedUser.Username;
    user.CircleUserId = updatedUser.CircleUserId;
    user.CredentialId = updatedUser.CredentialId;
    user.WalletAddress = updatedUser.WalletAddress;
    user.LastLoginAt = updatedUser.LastLoginAt;

    await db.SaveChangesAsync();

    return Results.Ok(user);
})
.WithName("UpdateUser")
.WithTags("Users")
.WithSummary("Update a user")
.WithDescription("Updates an existing user with new details");

// DELETE: Delete user
app.MapDelete("/api/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null)
    {
        return Results.NotFound();
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteUser")
.WithTags("Users")
.WithSummary("Delete a user")
.WithDescription("Deletes a user from the system");

// ============================================================================
// AUTHENTICATION ENDPOINTS (Passkey-based)
// ============================================================================

// POST: Check if username exists
app.MapPost("/api/auth/check-username", async (UsernameCheckRequest request, IAuthService authService) =>
{
    var exists = await authService.UsernameExistsAsync(request.Username);
    return Results.Ok(new { exists });
})
.WithName("CheckUsername")
.WithTags("Authentication")
.WithSummary("Check if username exists")
.WithDescription("Verifies if a username is already taken");

// POST: Initiate user registration
app.MapPost("/api/auth/register/initiate", async (InitiateRegistrationRequest request, IAuthService authService) =>
{
    try
    {
        var challenge = await authService.InitiateRegistrationAsync(request.Username);
        return Results.Ok(challenge);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error initiating registration for username: {Username}", request.Username);
        return Results.Problem("An error occurred during registration initiation");
    }
})
.WithName("InitiateRegistration")
.WithTags("Authentication")
.WithSummary("Initiate user registration")
.WithDescription("Starts the passkey-based registration process and returns a challenge");

// POST: Complete user registration
app.MapPost("/api/auth/register/complete", async (CompleteRegistrationRequest request, IAuthService authService) =>
{
    try
    {
        var result = await authService.CompleteRegistrationAsync(request);
        return Results.Created($"/api/users/{result.UserId}", result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error completing registration for username: {Username}", request.Username);
        return Results.Problem("An error occurred during registration completion");
    }
})
.WithName("CompleteRegistration")
.WithTags("Authentication")
.WithSummary("Complete user registration")
.WithDescription("Completes the passkey-based registration process after passkey creation");

// POST: Initiate user login
app.MapPost("/api/auth/login/initiate", async (InitiateLoginRequest request, IAuthService authService) =>
{
    try
    {
        var challenge = await authService.InitiateLoginAsync(request.Username);
        return Results.Ok(challenge);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error initiating login for username: {Username}", request.Username);
        return Results.Problem("An error occurred during login initiation");
    }
})
.WithName("InitiateLogin")
.WithTags("Authentication")
.WithSummary("Initiate user login")
.WithDescription("Starts the passkey-based login process and returns a challenge");

// POST: Complete user login
app.MapPost("/api/auth/login/complete", async (CompleteLoginRequest request, IAuthService authService) =>
{
    try
    {
        var result = await authService.CompleteLoginAsync(request);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error completing login for username: {Username}", request.Username);
        return Results.Problem("An error occurred during login completion");
    }
})
.WithName("CompleteLogin")
.WithTags("Authentication")
.WithSummary("Complete user login")
.WithDescription("Completes the passkey-based login process after passkey verification");

// POST: Development/Test login (bypasses passkey for testing)
// WARNING: This endpoint should be disabled in production!
app.MapPost("/api/auth/login/dev", async (DevLoginRequest request, AppDbContext db, JwtTokenService jwtService) =>
{
    Log.Warning("Development login endpoint used for username: {Username}", request.Username);

    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

    if (user == null)
    {
        return Results.NotFound(new { error = "User not found" });
    }

    // Update last login
    user.LastLoginAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    // Generate JWT token
    var token = jwtService.GenerateToken(user);
    var expiresAt = DateTime.UtcNow.AddMinutes(1440); // 24 hours

    Log.Information("Development login successful for user {UserId} ({Username})", user.Id, user.Username);

    return Results.Ok(new
    {
        token,
        username = user.Username,
        walletAddress = user.WalletAddress,
        expiresAt
    });
})
.WithName("DevLogin")
.WithTags("Authentication - Development")
.WithSummary("Development login (bypasses passkey)")
.WithDescription("⚠️ FOR TESTING ONLY - Logs in with just username, no passkey required. Should be disabled in production!");

// ============================================================================
// PROTECTED ENDPOINTS (Require Authentication)
// ============================================================================

// GET: Get current user profile
app.MapGet("/api/me", async (HttpContext context, AppDbContext db) =>
{
    var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
        return Results.Unauthorized();

    var userId = int.Parse(userIdClaim.Value);
    var user = await db.Users.FindAsync(userId);

    if (user == null)
        return Results.NotFound();

    return Results.Ok(new
    {
        user.Id,
        user.Username,
        user.CircleUserId,
        user.WalletAddress,
        user.CreatedAt,
        user.LastLoginAt
    });
})
.RequireAuthorization()
.WithName("GetCurrentUser")
.WithTags("Protected")
.WithSummary("Get current user profile")
.WithDescription("Returns the authenticated user's profile information");

// ============================================================================
// WALLET ENDPOINTS (Require Authentication)
// ============================================================================

// POST: Create wallet for current user
app.MapPost("/api/wallet/create", async (HttpContext context, IWalletService walletService) =>
{
    try
    {
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Results.Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var result = await walletService.CreateWalletAsync(userId);

        return Results.Created($"/api/wallet/balance/{result.WalletAddress}", result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error creating wallet for user");
        return Results.Problem("An error occurred during wallet creation");
    }
})
.RequireAuthorization()
.WithName("CreateWallet")
.WithTags("Wallet")
.WithSummary("Create wallet for current user")
.WithDescription("Creates a new Circle Web3 Services wallet for the authenticated user");

// GET: Get wallet balance
app.MapGet("/api/wallet/balance/{walletAddress}", async (
    string walletAddress,
    IWalletService walletService,
    bool refresh = false) =>
{
    try
    {
        var result = await walletService.GetWalletBalanceAsync(walletAddress, refresh);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error getting wallet balance for {WalletAddress}", walletAddress);
        return Results.Problem("An error occurred while fetching wallet balance");
    }
})
.RequireAuthorization()
.WithName("GetWalletBalance")
.WithTags("Wallet")
.WithSummary("Get wallet balance")
.WithDescription("Retrieves the USDC balance for a wallet address. Use refresh=true to bypass cache and get fresh balance.");

// POST: Transfer USDC
app.MapPost("/api/wallet/transfer", async (WalletTransferRequest request, IWalletService walletService) =>
{
    try
    {
        var result = await walletService.TransferUSDCAsync(request);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error transferring USDC from {From} to {To}", request.FromWalletAddress, request.ToWalletAddress);
        return Results.Problem("An error occurred during USDC transfer");
    }
})
.RequireAuthorization()
.WithName("TransferUSDC")
.WithTags("Wallet")
.WithSummary("Transfer USDC")
.WithDescription("Initiates a USDC transfer from one wallet to another");

// GET: Get transaction status
app.MapGet("/api/wallet/transaction/{transactionId}", async (string transactionId, IWalletService walletService) =>
{
    try
    {
        var result = await walletService.GetTransactionStatusAsync(transactionId);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error getting transaction status for {TransactionId}", transactionId);
        return Results.Problem("An error occurred while fetching transaction status");
    }
})
.RequireAuthorization()
.WithName("GetTransactionStatus")
.WithTags("Wallet")
.WithSummary("Get transaction status")
.WithDescription("Retrieves the status of a blockchain transaction");

// GET: Get transaction history
app.MapGet("/api/wallet/history/{walletAddress}", async (string walletAddress, IWalletService walletService, int? limit) =>
{
    try
    {
        var result = await walletService.GetTransactionHistoryAsync(walletAddress, limit ?? 20);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error getting transaction history for {WalletAddress}", walletAddress);
        return Results.Problem("An error occurred while fetching transaction history");
    }
})
.RequireAuthorization()
.WithName("GetTransactionHistory")
.WithTags("Wallet")
.WithSummary("Get transaction history")
.WithDescription("Retrieves the transaction history for a wallet address");

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

// ============================================================================
// REQUEST DTOs for API endpoints
// ============================================================================

public record UsernameCheckRequest(string Username);
public record InitiateRegistrationRequest(string Username);
public record InitiateLoginRequest(string Username);
public record DevLoginRequest(string Username);

// ============================================================================
// CONFIGURATION MODELS
// ============================================================================

public class ApiSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5100";
    public int Port { get; set; } = 5100;
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
