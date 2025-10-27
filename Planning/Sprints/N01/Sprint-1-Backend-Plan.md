# CoinPay Wallet MVP - Sprint 1 Backend Plan

**Sprint Duration**: 2 weeks (10 working days)
**Sprint Period**: January 6 - January 17, 2025
**Team Composition**: 2-3 Backend Engineers
**Available Capacity**: 20-30 engineering days
**Sprint Type**: Foundation Sprint

---

## Sprint Goal

**Establish core backend infrastructure and integrate Circle SDK for passkey-based wallet creation and gasless USDC transfers on Polygon Amoy testnet.**

By the end of Sprint 1, we will have:
- A working ASP.NET Core API with clean architecture
- PostgreSQL database with EF Core migrations
- Hashicorp Vault integration for secrets management
- Circle SDK integration with passkey authentication
- Wallet creation endpoint operational
- Basic USDC transfer capability (gasless)
- YARP API Gateway configured
- Health checks and structured logging
- Comprehensive API documentation (Swagger)

---

## Selected Tasks & Effort Distribution

### Phase 0: Infrastructure Setup (9.88 days)
- Backend project structure and clean architecture
- PostgreSQL database setup with EF Core
- Hashicorp Vault integration
- Structured logging with Serilog
- Health check endpoints
- YARP gateway configuration
- CORS policies
- Global exception handling middleware

### Phase 1: Core Wallet Foundation - START (19.08 days out of 29 total)
- Circle SDK configuration
- Passkey registration and verification APIs
- Session management with JWT
- Wallet creation endpoint
- Basic wallet repository
- Transaction submission endpoint (POST /api/transactions/transfer)
- Transaction status endpoint (GET /api/transactions/{id}/status)
- Transaction repository
- Blockchain RPC service (basic)
- UserOperation service
- Paymaster integration

**Total Sprint 1 Effort**: ~29 days of tasks (fits 20-30 day capacity with 2-3 engineers)

---

## Task Breakdown with Details

### Epic 0.1: Backend Infrastructure Setup (9.88 days)

#### BE-001: Initialize ASP.NET Core Project (0.54 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: None

**Description**:
Create ASP.NET Core Web API project with clean architecture pattern.

**Technical Requirements**:
- .NET 8.0 SDK
- Solution structure:
  - `CoinPay.API` - API layer (controllers, middleware)
  - `CoinPay.Application` - Application logic (services, DTOs, interfaces)
  - `CoinPay.Domain` - Domain models and business logic
  - `CoinPay.Infrastructure` - Data access, external services
- NuGet packages:
  - Microsoft.EntityFrameworkCore.Design
  - Swashbuckle.AspNetCore
  - FluentValidation.AspNetCore
  - AutoMapper.Extensions.Microsoft.DependencyInjection

**Acceptance Criteria**:
- [x] Solution builds without errors
- [x] All 4 projects reference each other correctly
- [x] Project follows clean architecture principles
- [x] Basic dependency injection configured in Program.cs
- [x] Project runs and displays Swagger UI at /swagger

**Definition of Done**:
- Code reviewed and approved
- Solution structure documented in README
- All team members can clone and build locally

---

#### BE-002: Configure Development Environment (0.54 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-001

**Description**:
Setup Docker Compose for local PostgreSQL and configure environment variables.

**Technical Requirements**:
- Docker Desktop installed
- docker-compose.yml with:
  - PostgreSQL 14+ container
  - pgAdmin (optional for dev convenience)
- .env.development file structure:
```env
DATABASE_CONNECTION_STRING=Host=localhost;Database=coinpay_dev;Username=postgres;Password=dev_password
VAULT_ADDRESS=http://localhost:8200
VAULT_TOKEN=dev_token
ASPNETCORE_ENVIRONMENT=Development
```

**Acceptance Criteria**:
- [x] Docker Compose starts PostgreSQL successfully
- [x] Application connects to local PostgreSQL
- [x] .env.development file loaded correctly
- [x] Environment variables accessible via IConfiguration
- [x] Connection string tested with simple query

**Definition of Done**:
- docker-compose.yml committed to repo
- .env.example template documented
- Team setup guide in docs/SETUP.md

---

#### BE-003: Setup PostgreSQL Database (1.08 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-002

**Description**:
Configure Entity Framework Core with Code-First migrations and database context.

**Technical Requirements**:
- Install NuGet packages:
  - Microsoft.EntityFrameworkCore.Design
  - Npgsql.EntityFrameworkCore.PostgreSQL
- Create `CoinPayDbContext` with:
  - DbSet for Wallets
  - DbSet for Transactions
  - DbSet for Users (basic)
- Initial migration with core tables
- Connection pooling configuration
- Retry policy for transient failures

**Domain Models (Initial)**:
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasskeyCredentialId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public class Wallet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Address { get; set; }
    public int ChainId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public User User { get; set; }
}

public class Transaction
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public string UserOpHash { get; set; }
    public string TransactionHash { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string TokenAddress { get; set; }
    public string Amount { get; set; } // Store as string to preserve precision
    public TransactionStatus Status { get; set; }
    public int ChainId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public Wallet Wallet { get; set; }
}

public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed
}
```

**Acceptance Criteria**:
- [x] EF Core context configured in DI container
- [x] Initial migration created successfully
- [x] Database tables created (Users, Wallets, Transactions)
- [x] Connection pooling configured (min 5, max 100)
- [x] Indexes on WalletAddress, UserOpHash, TransactionHash
- [x] Retry policy handles transient SQL errors

**Definition of Done**:
- Migrations run successfully on local environment
- Database schema documented
- Repository pattern scaffolded

---

#### BE-004: Configure Hashicorp Vault (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-002

**Description**:
Integrate Hashicorp Vault for secure secrets management.

**Technical Requirements**:
- Install VaultSharp NuGet package
- Vault setup (dev mode for local):
  - Run Vault in Docker or install locally
  - Configure KV v2 secrets engine
- Create VaultService abstraction:
```csharp
public interface IVaultService
{
    Task<string> GetSecretAsync(string path, string key);
    Task SetSecretAsync(string path, string key, string value);
    Task<Dictionary<string, string>> GetSecretsAsync(string path);
}
```
- Store test secrets:
  - Circle API keys (CLIENT_KEY, CLIENT_URL)
  - Database credentials (future rotation)
  - JWT signing key

**Acceptance Criteria**:
- [x] Vault client authenticates successfully
- [x] Secrets written to Vault persist correctly
- [x] Secrets retrieved from Vault successfully
- [x] VaultService registered in DI container
- [x] Error handling for Vault unavailability
- [x] Unit tests for VaultService methods

**Definition of Done**:
- Code reviewed and approved
- Vault setup documented in docs/VAULT.md
- Integration tests pass

---

#### BE-005: Setup Structured Logging with Serilog (1.00 day)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-001

**Description**:
Configure Serilog for structured logging with correlation IDs.

**Technical Requirements**:
- Install Serilog packages:
  - Serilog.AspNetCore
  - Serilog.Sinks.Console
  - Serilog.Sinks.File
  - Serilog.Enrichers.Environment
  - Serilog.Enrichers.Thread
- Configuration:
  - Minimum level: Debug (Development), Information (Production)
  - Output template with timestamp, correlation ID, level, message
  - JSON formatting for production
- Middleware for correlation ID injection:
```csharp
public class CorrelationIdMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.Headers.Add("X-Correlation-ID", correlationId);
            await next(context);
        }
    }
}
```

**Acceptance Criteria**:
- [x] Serilog configured in Program.cs
- [x] Logs written to console and file (logs/coinpay-.log)
- [x] Correlation IDs appear in all log entries
- [x] Request/response logging enabled
- [x] Sensitive data (passwords, tokens) redacted from logs
- [x] Log levels configurable via appsettings.json

**Definition of Done**:
- Logging tested with sample requests
- Log format documented
- Team can trace requests using correlation ID

---

#### BE-006: Implement Health Check Endpoints (1.00 day)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-003, BE-004

**Description**:
Create health check endpoints for monitoring system dependencies.

**Technical Requirements**:
- Install Microsoft.Extensions.Diagnostics.HealthChecks
- Implement checks:
  - Database connectivity (SELECT 1)
  - Vault connectivity (status check)
  - Circle API reachability (GET /health - if available)
- Endpoints:
  - GET /health - Basic liveness check
  - GET /health/ready - Readiness check (all dependencies)
  - GET /health/startup - Startup check

**Implementation**:
```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly CoinPayDbContext _context;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
        }
    }
}
```

**Acceptance Criteria**:
- [x] /health returns 200 OK when service is running
- [x] /health/ready returns 200 if all dependencies healthy, 503 otherwise
- [x] Health checks test database, Vault, external APIs
- [x] Response includes status for each dependency
- [x] Health checks timeout after 5 seconds
- [x] Documentation for adding new health checks

**Definition of Done**:
- Health endpoints tested manually
- Integrated into deployment scripts (future)
- Monitoring team notified of endpoints

---

#### BE-007: Configure YARP Gateway (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-001

**Description**:
Configure YARP (Yet Another Reverse Proxy) API Gateway based on existing infrastructure.

**Technical Requirements**:
- Review existing YARP configuration at `D:\Projects\Test\Claude\CoinPay`
- Extend configuration for new endpoints:
  - /api/auth/** -> Auth service
  - /api/wallet/** -> Wallet service
  - /api/transactions/** -> Transaction service
- Configure:
  - Load balancing (Round Robin)
  - Retry policies (3 attempts with exponential backoff)
  - Timeout (30 seconds for external APIs)
  - Rate limiting (100 req/min per IP for unauthenticated, 500 for authenticated)

**Configuration Example**:
```json
{
  "ReverseProxy": {
    "Routes": {
      "auth-route": {
        "ClusterId": "auth-cluster",
        "Match": {
          "Path": "/api/auth/{**catch-all}"
        }
      },
      "wallet-route": {
        "ClusterId": "wallet-cluster",
        "Match": {
          "Path": "/api/wallet/{**catch-all}"
        },
        "AuthorizationPolicy": "authenticated"
      }
    },
    "Clusters": {
      "auth-cluster": {
        "Destinations": {
          "auth-api": {
            "Address": "http://localhost:5001/"
          }
        }
      }
    }
  }
}
```

**Acceptance Criteria**:
- [x] YARP routes requests to backend APIs correctly
- [x] Load balancing works with multiple instances (tested locally)
- [x] Retry policies handle transient failures
- [x] Rate limiting prevents abuse
- [x] CORS configured for frontend origin
- [x] Gateway responds with proper error codes

**Definition of Done**:
- Gateway configuration documented
- Integration tested with Postman
- Frontend team notified of gateway URL

---

#### BE-008: Configure CORS Policies (0.54 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-007

**Description**:
Configure Cross-Origin Resource Sharing for frontend communication.

**Technical Requirements**:
- CORS policy configuration:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-Correlation-ID");
    });

    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://app.coinpay.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("X-Correlation-ID");
    });
});

app.UseCors(builder.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy");
```

**Acceptance Criteria**:
- [x] Frontend can make requests without CORS errors
- [x] Development origins include Vite dev server (5173) and CRA (3000)
- [x] Production origin restricted to production domain
- [x] Credentials allowed for cookie/token transmission
- [x] Custom headers (X-Correlation-ID) exposed
- [x] Preflight requests handled correctly

**Definition of Done**:
- CORS tested with frontend app
- Configuration documented
- Security review passed

---

#### BE-009: Global Exception Handling Middleware (1.08 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-005

**Description**:
Implement global exception handling middleware for consistent error responses.

**Technical Requirements**:
- Create exception handling middleware:
```csharp
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                StatusCode = 400,
                Message = "Validation failed",
                Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList(),
                CorrelationId = context.Response.Headers["X-Correlation-ID"]
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                StatusCode = 401,
                Message = "Unauthorized access",
                CorrelationId = context.Response.Headers["X-Correlation-ID"]
            },
            NotFoundException notFoundEx => new ErrorResponse
            {
                StatusCode = 404,
                Message = notFoundEx.Message,
                CorrelationId = context.Response.Headers["X-Correlation-ID"]
            },
            _ => new ErrorResponse
            {
                StatusCode = 500,
                Message = "An internal server error occurred",
                CorrelationId = context.Response.Headers["X-Correlation-ID"]
            }
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public string CorrelationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

**Acceptance Criteria**:
- [x] All unhandled exceptions caught by middleware
- [x] Error responses follow consistent format
- [x] Correlation ID included in error response
- [x] Different exception types map to correct HTTP status codes
- [x] Stack traces hidden in production
- [x] Errors logged with full details

**Definition of Done**:
- Middleware tested with various exception types
- Error response format documented for frontend
- Security review passed (no sensitive data leakage)

---

### Epic 1.1: Circle SDK Integration (19.08 days)

#### BE-101: Configure Circle Modular Wallets SDK in .NET (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-004 (Vault)

**Description**:
Create HTTP client wrapper for Circle APIs with retry policies.

**Technical Requirements**:
- HTTP client with:
  - Base URL: https://api.circle.com (or testnet URL)
  - Authentication header injection
  - Retry policy (3 attempts with exponential backoff)
  - Timeout: 30 seconds
  - Circuit breaker pattern
- Service interface:
```csharp
public interface ICircleApiClient
{
    Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);
    Task<WalletBalanceResponse> GetBalanceAsync(string address, string tokenAddress);
    Task<SubmitUserOperationResponse> SubmitUserOperationAsync(UserOperationRequest request);
    Task<UserOperationReceiptResponse> GetUserOperationReceiptAsync(string userOpHash);
}
```

**Acceptance Criteria**:
- [x] HTTP client communicates with Circle API
- [x] Authentication headers injected correctly
- [x] Retry policy handles transient failures (429, 503)
- [x] Circuit breaker opens after 5 consecutive failures
- [x] Timeout exceptions handled gracefully
- [x] Unit tests with mocked HTTP responses

**Definition of Done**:
- Service registered in DI container
- Integration tests with Circle testnet pass
- Error handling documented

---

#### BE-102: Environment Configuration Service (1.00 day)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-004, BE-101

**Description**:
Create service to load Circle client keys from Vault and validate on startup.

**Technical Requirements**:
- Configuration service:
```csharp
public interface ICircleConfiguration
{
    string ClientKey { get; }
    string ClientUrl { get; }
    string BundlerUrl { get; }
    string PaymasterUrl { get; }
    int ChainId { get; }
}

public class CircleConfiguration : ICircleConfiguration
{
    private readonly IVaultService _vaultService;

    public async Task LoadConfigurationAsync()
    {
        var secrets = await _vaultService.GetSecretsAsync("circle/config");
        ClientKey = secrets["CLIENT_KEY"];
        ClientUrl = secrets["CLIENT_URL"];
        BundlerUrl = secrets["BUNDLER_URL"];
        PaymasterUrl = secrets["PAYMASTER_URL"];
        ChainId = int.Parse(secrets["CHAIN_ID"]); // 80002 for Polygon Amoy
    }

    public void Validate()
    {
        if (string.IsNullOrEmpty(ClientKey))
            throw new InvalidOperationException("Circle CLIENT_KEY not configured");
        // ... validate other fields
    }
}
```

**Acceptance Criteria**:
- [x] Configuration loads from Vault on startup
- [x] Validation throws exception if keys missing
- [x] Configuration accessible via DI
- [x] Supports environment-specific configuration (dev/staging/prod)
- [x] Unit tests verify validation logic

**Definition of Done**:
- Configuration tested with valid and invalid data
- Startup failure documented
- Team setup guide updated

---

#### BE-103: POST /api/auth/register Endpoint (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-102

**Description**:
Implement passkey registration endpoint for new users.

**Technical Requirements**:
- Endpoint: POST /api/auth/register
- Request DTO:
```csharp
public class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }

    [Required]
    public string PasskeyCredentialId { get; set; }

    [Required]
    public string PublicKey { get; set; }
}
```
- Response DTO:
```csharp
public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}
```
- Business logic:
  - Validate username uniqueness
  - Validate passkey credential format
  - Store user in database
  - Link passkey credential to user account
  - Generate WebAuthn challenge (if needed)

**Acceptance Criteria**:
- [x] Endpoint returns 201 Created on success
- [x] Username uniqueness enforced
- [x] Passkey credential stored securely
- [x] User record created in database
- [x] Returns 400 Bad Request for validation errors
- [x] Integration tests with valid/invalid requests

**Definition of Done**:
- API documented in Swagger
- Integration tests pass
- Frontend team can integrate

---

#### BE-104: POST /api/auth/verify Endpoint (2.17 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-103, BE-105

**Description**:
Implement passkey verification endpoint for user login.

**Technical Requirements**:
- Endpoint: POST /api/auth/verify
- Request DTO:
```csharp
public class VerifyRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string PasskeyCredentialId { get; set; }

    [Required]
    public string Signature { get; set; }

    [Required]
    public string Challenge { get; set; }
}
```
- Response DTO:
```csharp
public class VerifyResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; }
}
```
- Business logic:
  - Validate passkey credential assertion
  - Verify WebAuthn signature
  - Create JWT session token on success
  - Update last login timestamp

**Acceptance Criteria**:
- [x] Endpoint validates passkey signature correctly
- [x] Returns JWT tokens on successful authentication
- [x] Returns 401 Unauthorized for invalid credentials
- [x] Session created with expiration time
- [x] Last login timestamp updated
- [x] Integration tests with mock passkey verification

**Definition of Done**:
- API documented in Swagger
- Security review passed
- Frontend team can authenticate

---

#### BE-105: Session Management (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-001

**Description**:
Implement JWT token generation and validation for user sessions.

**Technical Requirements**:
- JWT configuration:
```csharp
public class JwtSettings
{
    public string SecretKey { get; set; } // From Vault
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
```
- Token service:
```csharp
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal ValidateToken(string token);
    Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken);
}
```
- JWT claims:
  - sub: UserId
  - username: Username
  - iat: Issued at timestamp
  - exp: Expiration timestamp
- Optional: Redis for session storage (can defer to Sprint 2)

**Acceptance Criteria**:
- [x] JWT tokens generated with correct claims
- [x] Access token expires after 60 minutes
- [x] Refresh token expires after 7 days
- [x] Token validation middleware rejects expired tokens
- [x] Token validation middleware rejects tampered tokens
- [x] Refresh token endpoint implemented (POST /api/auth/refresh)
- [x] Unit tests for token generation and validation

**Definition of Done**:
- JWT middleware configured
- Protected endpoints require valid token
- Token refresh flow tested

---

#### BE-106: POST /api/wallet/create Endpoint (3.08 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-101, BE-105

**Description**:
Create Circle Smart Account with passkey owner and store in database.

**Technical Requirements**:
- Endpoint: POST /api/wallet/create
- Request DTO:
```csharp
public class CreateWalletRequest
{
    // User authenticated via JWT, no additional fields needed
}
```
- Response DTO:
```csharp
public class CreateWalletResponse
{
    public string Address { get; set; }
    public int ChainId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```
- Business logic:
  - Get authenticated user from JWT
  - Call Circle API to create Smart Account
  - Use passkey credential as account owner
  - Generate deterministic address
  - Store wallet in database
  - Wallet deploys on first transaction (lazy deployment)

**Acceptance Criteria**:
- [x] Endpoint creates Circle Smart Account successfully
- [x] Returns deterministic wallet address
- [x] Wallet record stored in database with user association
- [x] Returns 409 Conflict if user already has wallet
- [x] Returns 401 Unauthorized if not authenticated
- [x] Integration tests with Circle testnet

**Definition of Done**:
- API documented in Swagger
- Integration tests pass
- Frontend can create wallets

---

#### BE-107: Wallet Repository (1.08 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-003, BE-106

**Description**:
Implement data access layer for wallet CRUD operations.

**Technical Requirements**:
- Repository interface:
```csharp
public interface IWalletRepository
{
    Task<Wallet> CreateAsync(Wallet wallet);
    Task<Wallet> GetByIdAsync(Guid id);
    Task<Wallet> GetByAddressAsync(string address);
    Task<Wallet> GetByUserIdAsync(Guid userId);
    Task UpdateLastActivityAsync(Guid walletId, DateTime activityTime);
    Task<bool> ExistsAsync(string address);
}
```
- Repository implementation using EF Core
- Unit of Work pattern (optional, can defer)

**Acceptance Criteria**:
- [x] Repository performs CRUD operations correctly
- [x] GetByAddress uses indexed query
- [x] GetByUserId retrieves correct wallet
- [x] Repository methods async and cancellation token support
- [x] Unit tests with in-memory database

**Definition of Done**:
- Repository tested with unit tests
- Registered in DI container
- Code reviewed

---

#### BE-108: GET /api/wallet/{address}/balance Endpoint (2.00 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-112 (RPC service), BE-107

**Description**:
Fetch USDC balance from Polygon RPC and cache with TTL.

**Technical Requirements**:
- Endpoint: GET /api/wallet/{address}/balance
- Response DTO:
```csharp
public class BalanceResponse
{
    public string Address { get; set; }
    public string TokenAddress { get; set; }
    public string Balance { get; set; } // Raw balance (with 6 decimals)
    public decimal FormattedBalance { get; set; } // Human-readable
    public DateTime LastUpdated { get; set; }
}
```
- Business logic:
  - Validate Ethereum address format
  - Query USDC contract balanceOf(address)
  - Cache balance with 30-second TTL
  - Return cached value if fresh

**Acceptance Criteria**:
- [x] Endpoint returns current USDC balance
- [x] Balance cached for 30 seconds
- [x] Cache invalidated on transaction completion
- [x] Returns 404 if wallet not found
- [x] Handles RPC errors gracefully
- [x] Integration tests with testnet wallet

**Definition of Done**:
- API documented in Swagger
- Caching strategy documented
- Performance tested (sub-second response)

---

#### BE-109: POST /api/transactions/transfer Endpoint (4.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-113 (UserOperation service), BE-114 (Paymaster)

**Description**:
Submit gasless USDC transfer using Circle bundler and paymaster.

**Technical Requirements**:
- Endpoint: POST /api/transactions/transfer
- Request DTO:
```csharp
public class TransferRequest
{
    [Required]
    [EthereumAddress]
    public string ToAddress { get; set; }

    [Required]
    [Range(0.000001, 1000000)]
    public decimal Amount { get; set; }

    public string TokenAddress { get; set; } = "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582"; // USDC on Amoy
}
```
- Response DTO:
```csharp
public class TransferResponse
{
    public string UserOpHash { get; set; }
    public string Status { get; set; } // "pending"
    public DateTime SubmittedAt { get; set; }
}
```
- Business logic:
  - Get authenticated user's wallet
  - Validate balance sufficient for transfer
  - Encode ERC-20 transfer call
  - Construct UserOperation with paymaster data
  - Submit to Circle bundler
  - Store transaction in database with "pending" status
  - Return userOpHash for tracking

**Acceptance Criteria**:
- [x] Endpoint submits gasless USDC transfer successfully
- [x] UserOperation includes paymaster sponsorship
- [x] Transaction stored in database
- [x] Returns userOpHash for status tracking
- [x] Returns 400 if insufficient balance
- [x] Returns 401 if not authenticated
- [x] Integration tests with testnet transfer

**Definition of Done**:
- API documented in Swagger
- Gasless transfer verified on testnet
- Frontend can submit transfers

---

#### BE-110: GET /api/transactions/{id}/status Endpoint (2.00 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-109, BE-111

**Description**:
Poll transaction status from bundler and update database.

**Technical Requirements**:
- Endpoint: GET /api/transactions/{id}/status
- Response DTO:
```csharp
public class TransactionStatusResponse
{
    public string UserOpHash { get; set; }
    public string TransactionHash { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public int Confirmations { get; set; }
}
```
- Business logic:
  - Query bundler for UserOperation receipt
  - Parse receipt for transaction hash and status
  - Update database with status
  - Return current status

**Acceptance Criteria**:
- [x] Endpoint returns current transaction status
- [x] Status updates from pending -> confirmed/failed
- [x] Transaction hash populated when confirmed
- [x] ConfirmedAt timestamp set on confirmation
- [x] Returns 404 if transaction not found
- [x] Integration tests with mock bundler responses

**Definition of Done**:
- API documented in Swagger
- Status polling tested
- Frontend can track transactions

---

#### BE-111: Transaction Repository (1.00 day)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-003

**Description**:
Implement data access layer for transaction records.

**Technical Requirements**:
- Repository interface:
```csharp
public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> GetByIdAsync(Guid id);
    Task<Transaction> GetByUserOpHashAsync(string userOpHash);
    Task<List<Transaction>> GetByWalletIdAsync(Guid walletId, int limit = 20);
    Task UpdateStatusAsync(Guid transactionId, TransactionStatus status, string txHash = null);
    Task<bool> ExistsAsync(string userOpHash);
}
```
- Repository implementation using EF Core

**Acceptance Criteria**:
- [x] Repository performs CRUD operations correctly
- [x] GetByUserOpHash uses indexed query
- [x] GetByWalletId returns last 20 transactions
- [x] UpdateStatus atomic operation
- [x] Unit tests with in-memory database

**Definition of Done**:
- Repository tested
- Registered in DI container
- Code reviewed

---

#### BE-112: Blockchain RPC Service (2.17 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-102

**Description**:
Create service for interacting with Polygon Amoy testnet using RPC.

**Technical Requirements**:
- Install Nethereum NuGet package (or use raw HTTP calls)
- Service interface:
```csharp
public interface IBlockchainRpcService
{
    Task<string> GetBalanceAsync(string address, string tokenAddress);
    Task<string> GetTransactionReceiptAsync(string txHash);
    Task<long> GetBlockNumberAsync();
    Task<string> CallContractAsync(string contractAddress, string data);
}
```
- RPC endpoint: https://rpc-amoy.polygon.technology/
- Implement balance queries via ERC-20 balanceOf call
- Implement transaction monitoring

**Acceptance Criteria**:
- [x] Service reads USDC balance correctly
- [x] Service fetches transaction receipts
- [x] RPC calls handle network errors gracefully
- [x] Retry policy for failed RPC calls
- [x] Unit tests with mocked RPC responses

**Definition of Done**:
- Service tested with testnet
- Registered in DI container
- Documentation for supported methods

---

#### BE-113: UserOperation Service (3.17 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-112, BE-101

**Description**:
Construct ERC-4337 UserOperations and submit to Circle bundler.

**Technical Requirements**:
- Service interface:
```csharp
public interface IUserOperationService
{
    Task<UserOperation> ConstructTransferOperationAsync(
        string fromAddress,
        string toAddress,
        string tokenAddress,
        decimal amount);

    Task<string> SubmitUserOperationAsync(UserOperation userOp);
    Task<UserOperationReceipt> GetReceiptAsync(string userOpHash);
}
```
- UserOperation structure:
```csharp
public class UserOperation
{
    public string Sender { get; set; }
    public string Nonce { get; set; }
    public string InitCode { get; set; }
    public string CallData { get; set; }
    public string CallGasLimit { get; set; }
    public string VerificationGasLimit { get; set; }
    public string PreVerificationGas { get; set; }
    public string MaxFeePerGas { get; set; }
    public string MaxPriorityFeePerGas { get; set; }
    public string PaymasterAndData { get; set; }
    public string Signature { get; set; }
}
```
- Business logic:
  - Encode ERC-20 transfer as callData
  - Fetch nonce from entry point
  - Estimate gas limits
  - Include paymaster data
  - Submit to bundler endpoint

**Acceptance Criteria**:
- [x] Service constructs valid UserOperations
- [x] CallData encodes transfer correctly
- [x] Nonce management prevents conflicts
- [x] Gas estimation accurate
- [x] UserOperations submit successfully to bundler
- [x] Integration tests with Circle bundler

**Definition of Done**:
- Service tested with testnet
- Code reviewed
- Documentation for UserOperation structure

---

#### BE-114: Paymaster Integration (2.17 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-113

**Description**:
Configure Circle paymaster for gas sponsorship in UserOperations.

**Technical Requirements**:
- Paymaster configuration from Circle
- Service method:
```csharp
public interface IPaymasterService
{
    Task<string> GetPaymasterDataAsync(UserOperation userOp);
    Task<bool> VerifySponsorship(string userOpHash);
}
```
- Include paymaster data in UserOperation:
  - paymasterAndData field format: `<paymaster_address><paymaster_verification_data>`
- Verify all transactions are gasless (user pays 0 gas)

**Acceptance Criteria**:
- [x] Paymaster data included in UserOperations
- [x] All transactions sponsored (0 gas paid by user)
- [x] Paymaster verification successful
- [x] Handles paymaster unavailability gracefully
- [x] Integration tests verify gasless transactions

**Definition of Done**:
- Gasless transfers verified on testnet
- Code reviewed
- 100% gas sponsorship confirmed

---

## Daily Milestone Plan

### Days 1-2 (Sprint Start)
**Focus**: Infrastructure foundation
- BE-001: Project structure ✅
- BE-002: Development environment ✅
- BE-003: Database setup ✅
- BE-005: Logging (started)

**Deliverable**: Team can run the API locally with database connectivity

---

### Days 3-4
**Focus**: Security and observability
- BE-004: Vault integration ✅
- BE-005: Logging (completed) ✅
- BE-006: Health checks ✅
- BE-008: CORS ✅

**Deliverable**: Secrets managed securely, logs structured, health checks operational

---

### Days 5-6 (Mid-Sprint)
**Focus**: Gateway and error handling
- BE-007: YARP gateway ✅
- BE-009: Exception handling ✅
- BE-101: Circle SDK client (started)
- BE-102: Circle configuration ✅

**Checkpoint Meeting**: Review infrastructure, demo health checks and logging

**Deliverable**: API gateway routes requests, error handling consistent

---

### Days 7-8
**Focus**: Authentication and wallet creation
- BE-101: Circle SDK client (completed) ✅
- BE-103: Passkey registration ✅
- BE-104: Passkey verification ✅
- BE-105: JWT session management ✅
- BE-106: Wallet creation (started)

**Deliverable**: Users can register and login with passkeys

---

### Days 9-10 (Sprint End)
**Focus**: Transaction capability
- BE-106: Wallet creation (completed) ✅
- BE-107: Wallet repository ✅
- BE-112: Blockchain RPC service ✅
- BE-113: UserOperation service ✅
- BE-114: Paymaster integration ✅
- BE-109: Transfer endpoint (started)
- BE-110: Status endpoint (stretch goal)
- BE-111: Transaction repository (stretch goal)

**Sprint Review**: Demo wallet creation and transaction submission

**Deliverable**: Gasless USDC transfer works end-to-end

---

## Risks, Dependencies & Mitigation

### External Dependencies

| Dependency | Risk Level | Impact | Mitigation |
|------------|------------|--------|------------|
| Circle Console Access | HIGH | Blocker - Cannot create Client Key | **Action**: Request access Day 1, escalate to PM if delayed |
| Circle API Keys | HIGH | Blocker - SDK won't work | **Action**: Obtain test keys from Circle support immediately |
| Polygon Amoy RPC | MEDIUM | Degraded - Balance queries fail | **Mitigation**: Implement retry logic, use multiple RPC endpoints |
| Vault Setup | MEDIUM | Delayed - Store secrets in appsettings temporarily | **Mitigation**: Setup Vault locally, defer production Vault to Sprint 2 |
| WhiteBit API Access | LOW | Not needed in Sprint 1 | **Action**: Request access in Sprint 1 for Sprint 3 preparation |

### Technical Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Circle SDK complexity | MEDIUM | Sprint delay 1-2 days | **Mitigation**: Allocate senior engineer, pair program on BE-101 |
| WebAuthn passkey integration unclear | MEDIUM | Authentication blocked | **Mitigation**: Research WebAuthn .NET libraries Day 1, consider frontend-first approach |
| ERC-4337 UserOperation construction difficult | HIGH | Transaction submission fails | **Mitigation**: Reference Circle SDK examples, allocate extra time (4 days), consult Circle support |
| Gas estimation errors | MEDIUM | Transactions fail | **Mitigation**: Use Circle's gas estimation API, implement retry with higher gas |
| Database migration issues in team | LOW | Dev environment setup delays | **Mitigation**: Document migration steps, provide docker-compose setup |

### Team Capacity Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Team member unavailable | LOW | 2-4 days lost capacity | **Mitigation**: Cross-train on critical tasks, pair programming |
| Onboarding delays (new team member) | MEDIUM | 1-2 days ramp-up | **Mitigation**: Provide detailed setup guide, assign mentor |
| Underestimation of Circle SDK integration | HIGH | Sprint scope creep | **Mitigation**: Cut BE-110, BE-111 to Sprint 2 if needed |

---

## Success Criteria

### Functional Success Metrics

- [ ] **Infrastructure**: API runs locally and in dev environment
- [ ] **Database**: PostgreSQL connects, migrations execute successfully
- [ ] **Secrets**: Vault stores and retrieves Circle API keys
- [ ] **Authentication**: Users can register with passkeys
- [ ] **Authentication**: Users can login with passkeys
- [ ] **Wallet**: POST /api/wallet/create returns deterministic address
- [ ] **Transaction**: POST /api/transactions/transfer submits gasless USDC transfer
- [ ] **Monitoring**: Health checks return status of dependencies
- [ ] **Logging**: Correlation IDs trace requests through logs

### Quality Gates

- [ ] All API endpoints documented in Swagger
- [ ] Unit test coverage >60% for services and repositories
- [ ] Integration tests pass for Circle SDK calls
- [ ] Code review completed for all critical components (BE-101, BE-106, BE-109, BE-113, BE-114)
- [ ] Security review passed for authentication flow
- [ ] Performance: API response time <3 seconds (P95)

### Sprint Review Demo Checklist

1. Show project structure and clean architecture
2. Demonstrate health check endpoints (/health, /health/ready)
3. Show structured logging with correlation IDs
4. Register new user with passkey (POST /api/auth/register)
5. Login with passkey (POST /api/auth/verify)
6. Create wallet (POST /api/wallet/create)
7. Check USDC balance (GET /api/wallet/{address}/balance)
8. Submit gasless USDC transfer (POST /api/transactions/transfer)
9. Query transaction status (GET /api/transactions/{id}/status)
10. Show Swagger documentation for all endpoints

---

## Handoff Points to Frontend and QA

### Frontend Team Handoff (Day 6-7)

**Deliverables**:
- Swagger documentation URL: `http://localhost:5000/swagger`
- API Gateway base URL: `http://localhost:5100` (YARP)
- Authentication flow documentation:
  - POST /api/auth/register - Register with passkey
  - POST /api/auth/verify - Login and get JWT
  - Authorization header format: `Bearer <jwt_token>`
- Postman collection with example requests
- WebAuthn integration notes (frontend handles passkey creation)
- CORS configuration (Vite port 5173 allowed)

**Coordination Points**:
- Passkey credential ID format agreed upon
- JWT token format and claims documented
- Error response format standardized

### QA Team Handoff (Day 8-9)

**Deliverables**:
- API documentation with acceptance criteria
- Test data:
  - Test user accounts with passkeys
  - Test wallet addresses with USDC balance on Polygon Amoy
  - Test USDC tokens for transfers
- Test environment URL: `http://dev.coinpay.local:5100`
- Database access for test data verification
- Logs location for debugging: `logs/coinpay-*.log`
- Health check endpoints for monitoring

**Testing Scenarios**:
1. **Authentication Tests**:
   - Register new user with valid passkey
   - Register with duplicate username (should fail)
   - Login with valid passkey
   - Login with invalid passkey (should fail)
   - Access protected endpoint without token (should return 401)
   - Access protected endpoint with expired token (should return 401)

2. **Wallet Tests**:
   - Create wallet for authenticated user
   - Create wallet for user who already has wallet (should fail)
   - Get balance for existing wallet
   - Get balance for non-existent wallet (should return 404)

3. **Transaction Tests**:
   - Submit USDC transfer with sufficient balance
   - Submit transfer with insufficient balance (should fail)
   - Submit transfer to invalid address (should fail)
   - Query transaction status by userOpHash
   - Verify transaction is gasless (0 gas paid by user)

---

## Definition of Done (Sprint 1)

### Code Quality
- [x] All code follows C# coding conventions
- [x] SOLID principles applied
- [x] Async/await used consistently
- [x] Nullable reference types enabled
- [x] XML documentation comments for public APIs
- [x] No critical SonarQube issues
- [x] Code reviewed and approved by senior engineer

### Testing
- [x] Unit tests for service layer (>60% coverage)
- [x] Integration tests for Circle SDK calls
- [x] Repository tests with in-memory database
- [x] Postman collection with happy path tests
- [x] Manual testing of authentication flow
- [x] Manual testing of wallet creation and transfer

### Documentation
- [x] Swagger documentation complete and accurate
- [x] README.md with setup instructions
- [x] Architecture decision records (ADRs) for key decisions
- [x] API integration guide for frontend
- [x] Environment setup guide for team

### Deployment
- [x] Docker Compose runs all dependencies
- [x] Database migrations executable
- [x] Health checks operational
- [x] Logging configured
- [x] Environment variables documented

### Security
- [x] Secrets stored in Vault (not in code)
- [x] JWT tokens validated correctly
- [x] Input validation on all endpoints
- [x] CORS configured properly
- [x] HTTPS enforced (production ready)
- [x] Error messages don't leak sensitive data

---

## Sprint Retrospective Topics

### What Went Well
- Infrastructure setup smooth?
- Circle SDK integration clear?
- Team collaboration effective?

### What Could Be Improved
- Estimation accuracy?
- Dependency management?
- Communication with frontend/QA?

### Action Items for Sprint 2
- Refine estimates based on actual time
- Identify additional Circle SDK needs
- Plan transaction monitoring service
- Prepare for Phase 2 (Transaction History)

---

## Appendix

### Useful Commands

**Database Migrations**:
```bash
# Create new migration
dotnet ef migrations add InitialCreate --project CoinPay.Infrastructure --startup-project CoinPay.API

# Apply migrations
dotnet ef database update --project CoinPay.Infrastructure --startup-project CoinPay.API

# Rollback migration
dotnet ef database update PreviousMigration --project CoinPay.Infrastructure --startup-project CoinPay.API
```

**Docker Commands**:
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Reset database
docker-compose down -v
docker-compose up -d
```

**Run API**:
```bash
dotnet run --project CoinPay.API
dotnet watch run --project CoinPay.API  # Hot reload
```

### Reference Links

- **Circle Modular Wallets Docs**: https://developers.circle.com/w3s/docs/modular-wallets-overview
- **ERC-4337 Account Abstraction**: https://eips.ethereum.org/EIPS/eip-4337
- **Polygon Amoy Testnet**: https://polygon.technology/blog/introducing-the-amoy-testnet-for-polygon-pos
- **WebAuthn .NET**: https://github.com/passwordless-lib/fido2-net-lib
- **YARP Documentation**: https://microsoft.github.io/reverse-proxy/

---

**Sprint 1 Plan Version**: 1.0
**Last Updated**: 2025-01-06
**Status**: Ready for Execution
**Next Steps**: Daily standup at 9 AM, start with BE-001, BE-002, BE-003 in parallel

---

**End of Sprint 1 Backend Plan**
