# CoinPay.Api - Claude Development Guide

## Project Overview
This is a .NET 9.0 Minimal API project for managing cryptocurrency payment transactions. The API provides RESTful endpoints for CRUD operations on transactions with Swagger/OpenAPI documentation.

## Technology Stack
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Minimal API** - Lightweight API pattern
- **Entity Framework Core InMemory** - In-memory database provider
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI implementation
- **C# 12** - Latest C# language features

## Project Structure

```
CoinPay.Api/
├── Models/
│   └── Transaction.cs          # Transaction entity model
├── Data/
│   └── AppDbContext.cs         # EF Core DbContext with seed data
├── Properties/
│   └── launchSettings.json     # Development server configuration
├── Program.cs                  # Application entry point & API endpoints
├── appsettings.json            # Application configuration
├── appsettings.Development.json # Development-specific settings
└── CoinPay.Api.csproj          # Project file with dependencies
```

## Key Components

### 1. Transaction Model (`Models/Transaction.cs`)
**Purpose:** Defines the transaction entity structure

**Properties:**
- `Id` (int) - Primary key, auto-generated
- `TransactionId` (string) - Unique transaction identifier
- `Amount` (decimal) - Transaction amount
- `Currency` (string) - Currency code (default: "USD")
- `Type` (string) - Transaction type: "Payment", "Refund", "Transfer"
- `Status` (string) - Status: "Pending", "Completed", "Failed"
- `SenderName` (string) - Sender's name
- `ReceiverName` (string) - Receiver's name
- `Description` (string) - Transaction description
- `CreatedAt` (DateTime) - Creation timestamp (UTC)
- `CompletedAt` (DateTime?) - Completion timestamp (nullable)

### 2. Database Context (`Data/AppDbContext.cs`)
**Purpose:** EF Core context for database operations

**Features:**
- Uses InMemory database provider (name: "CoinPayDb")
- Includes seed data with 3 sample transactions
- DbSet<Transaction> for transaction operations

**Seed Data:**
1. Transaction ID: TXN001 - Completed payment ($100.50)
2. Transaction ID: TXN002 - Completed transfer ($250.00)
3. Transaction ID: TXN003 - Pending payment ($75.25)

### 3. API Endpoints (`Program.cs`)

#### Configuration
- **Port:** localhost:7777 (HTTP)
- **Swagger UI:** http://localhost:7777/swagger
- **CORS:** Enabled for all origins (AllowAll policy)

#### Endpoints

| Method | Route | Description | Returns |
|--------|-------|-------------|---------|
| GET | `/api/transactions` | Get all transactions | 200 + Transaction[] |
| GET | `/api/transactions/{id}` | Get by ID | 200 + Transaction / 404 |
| GET | `/api/transactions/status/{status}` | Get by status | 200 + Transaction[] |
| POST | `/api/transactions` | Create new | 201 + Transaction |
| PUT | `/api/transactions/{id}` | Update existing | 200 + Transaction / 404 |
| PATCH | `/api/transactions/{id}/status?status={status}` | Update status | 200 + Transaction / 404 |
| DELETE | `/api/transactions/{id}` | Delete transaction | 204 / 404 |

## Development Commands

### Build & Run
```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run API (HTTP on port 7777)
dotnet run --launch-profile http

# Run with HTTPS
dotnet run --launch-profile https

# Watch mode (auto-reload on changes)
dotnet watch run
```

### Package Management
```bash
# Add new package
dotnet add package PackageName

# Update package
dotnet add package PackageName --version x.x.x

# Remove package
dotnet remove package PackageName

# List packages
dotnet list package
```

## Current Dependencies

```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.10" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="9.0.6" />
```

## Common Development Tasks

### Adding a New Entity Model
1. Create model class in `Models/` folder
2. Add DbSet<YourModel> to `AppDbContext.cs`
3. Update `OnModelCreating` if seed data needed
4. Create API endpoints in `Program.cs`

### Adding a New API Endpoint
```csharp
app.MapGet("/api/yourroute", async (AppDbContext db) =>
{
    // Your logic here
    return Results.Ok(data);
})
.WithName("EndpointName")
.WithTags("YourTag")
.WithSummary("Short description")
.WithDescription("Detailed description");
```

### Switching to Real Database (e.g., SQL Server)
1. Install package: `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
2. Update `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
3. Add connection string to `appsettings.json`
4. Run migrations: `dotnet ef migrations add InitialCreate`
5. Update database: `dotnet ef database update`

### Adding Authentication/Authorization
```bash
# Add JWT authentication package
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Update Program.cs with authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* config */ });

# Add authorization to endpoints
.RequireAuthorization();
```

## API Testing

### Using Swagger UI
1. Navigate to: http://localhost:7777/swagger
2. Click "Try it out" on any endpoint
3. Fill in parameters
4. Click "Execute"

### Using curl
```bash
# Get all transactions
curl http://localhost:7777/api/transactions

# Get by ID
curl http://localhost:7777/api/transactions/1

# Create new transaction
curl -X POST http://localhost:7777/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 150.00,
    "currency": "USD",
    "type": "Payment",
    "status": "Pending",
    "senderName": "Alice",
    "receiverName": "Bob",
    "description": "Test payment"
  }'

# Update status
curl -X PATCH "http://localhost:7777/api/transactions/1/status?status=Completed"

# Delete transaction
curl -X DELETE http://localhost:7777/api/transactions/1
```

## Business Logic Notes

### Transaction ID Generation
- If `TransactionId` is not provided during creation, it's auto-generated
- Format: `TXN{DateTime.UtcNow.Ticks}`
- Example: `TXN638654321234567890`

### Status Management
- When status changes to "Completed", `CompletedAt` is automatically set to UTC now
- Status changes are tracked in PATCH endpoint

### Timestamp Handling
- All timestamps use UTC (`DateTime.UtcNow`)
- `CreatedAt` is set automatically on creation
- `CompletedAt` is nullable and set when status becomes "Completed"

## CORS Configuration
Currently set to allow all origins for development:
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

**For Production:** Restrict to specific domains:
```csharp
policy.WithOrigins("https://yourdomain.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

## Configuration Files

### appsettings.json
- Logging configuration
- AllowedHosts setting

### launchSettings.json
- HTTP profile: localhost:7777
- HTTPS profile: localhost:7778 (HTTPS) + localhost:7777 (HTTP fallback)
- Swagger launch URL configured

## Troubleshooting

### API won't start on port 7777
- Check if port is in use: `netstat -ano | findstr :7777`
- Change port in `launchSettings.json`

### CORS errors from frontend
- Verify CORS policy is applied: `app.UseCors("AllowAll");`
- Check browser console for specific error
- Ensure frontend origin matches CORS policy

### Database seed data not appearing
- Check `OnModelCreating` in `AppDbContext.cs`
- Ensure `EnsureCreated()` is called in `Program.cs`

### Swagger not loading
- Verify Swashbuckle package is installed
- Check `UseSwagger()` and `UseSwaggerUI()` are called
- Navigate to correct URL: `/swagger` not `/swagger/index.html`

## Best Practices for This Project

1. **Keep endpoints in Program.cs organized** - Group by resource type
2. **Use meaningful endpoint names** - `.WithName("GetAllTransactions")`
3. **Add descriptions to all endpoints** - For Swagger documentation
4. **Validate input** - Add validation attributes to models
5. **Use async/await** - All database operations should be async
6. **Return proper status codes** - 200, 201, 204, 404, 400, etc.
7. **Handle exceptions** - Add global exception handling middleware
8. **Log important operations** - Use ILogger for debugging

## Future Enhancements to Consider

- [ ] Add input validation with FluentValidation
- [ ] Implement pagination for GET all transactions
- [ ] Add filtering and sorting capabilities
- [ ] Add authentication and authorization
- [ ] Implement rate limiting
- [ ] Add health check endpoints
- [ ] Switch to real database (PostgreSQL/SQL Server)
- [ ] Add unit and integration tests
- [ ] Implement DTOs for request/response mapping
- [ ] Add API versioning
- [ ] Implement caching with Redis
- [ ] Add transaction search functionality
- [ ] Implement audit logging

## Related Files
- Root README.md - Overall project documentation
- CoinPay.Web/CLAUDE.md - Frontend development guide

## Quick Reference Links
- [.NET Minimal APIs Documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Swagger/Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
