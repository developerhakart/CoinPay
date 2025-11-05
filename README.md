# CoinPay

A modern cryptocurrency payment platform with a RESTful API backend and React frontend.

## Project Structure

```
CoinPay/
├── CoinPay.Api/          # .NET Core Minimal API Backend
│   ├── Models/           # Data models
│   ├── Data/             # Database context
│   └── Program.cs        # API endpoints and configuration
├── CoinPay.Gateway/      # YARP API Gateway
│   ├── Program.cs        # Gateway configuration
│   └── appsettings.json  # Route configuration
├── CoinPay.Web/          # React Frontend Application
│   └── src/              # React source files
└── docfx/                # API Documentation
    └── _site/            # Generated documentation site
```

## Technologies Used

### Secret Management
- **HashiCorp Vault 1.15** - Secure secret storage and management
- **VaultSharp** - .NET client library for Vault integration

### Gateway (CoinPay.Gateway)
- **.NET 9.0** - Latest .NET framework
- **YARP** - Yet Another Reverse Proxy for routing
- **CORS** - Cross-Origin Resource Sharing enabled

### Backend (CoinPay.Api)
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Minimal API** - Lightweight API architecture
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Production-grade relational database
- **Swagger/OpenAPI** - API documentation and testing interface
- **CORS** - Cross-Origin Resource Sharing enabled
- **Circle API** - Web3 Services integration

### Frontend (CoinPay.Web)
- **React 18** - Modern React framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **Zustand** - State management
- **React Router** - Client-side routing

### Documentation
- **DocFX** - Static documentation site generator

### Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

## Getting Started

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (required for all services)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (optional, for local development)
- [Node.js](https://nodejs.org/) (v18 or higher) (optional, for local development)
- Git

## Quick Reference

### Most Common Commands

```bash
# START APPLICATION
.\start-coinpay.ps1              # Windows - Start all services with Vault
./start-coinpay.sh               # Linux/Mac - Start all services with Vault

# STOP APPLICATION
docker-compose down              # Stop all services (keeps data)
docker-compose down -v           # Stop and remove all data

# VIEW LOGS
docker-compose logs -f           # All services
docker-compose logs -f api       # API only
docker-compose logs -f vault     # Vault only

# RESTART SERVICES
docker-compose restart api       # Restart API
docker-compose restart           # Restart all

# CHECK STATUS
docker-compose ps                # Service status
curl http://localhost:7777/health  # API health
docker exec coinpay-vault vault status  # Vault status

# VAULT SECRETS
.\vault\scripts\populate-dev-secrets.ps1  # Re-populate secrets (Windows)
./vault/scripts/populate-dev-secrets.sh   # Re-populate secrets (Linux/Mac)

# REBUILD
docker-compose up -d --build     # Rebuild and restart all
docker-compose build api         # Rebuild API only
```

### Service URLs

| Service | URL | Description |
|---------|-----|-------------|
| Frontend | http://localhost:3000 | React Web Application |
| API | http://localhost:7777 | Backend REST API |
| Swagger | http://localhost:7777/swagger | API Documentation |
| Gateway | http://localhost:5000 | API Gateway |
| Docs | http://localhost:8080 | DocFX Documentation |
| Vault UI | http://localhost:8200/ui | Vault Management (Token: `dev-root-token`) |

### Quick Start (Recommended - Using Docker Compose)

The entire application runs in Docker containers with all dependencies included.

#### Start All Services

**Windows (PowerShell):**
```powershell
.\start-coinpay.ps1
```

**Linux/Mac (Bash):**
```bash
chmod +x start-coinpay.sh
./start-coinpay.sh
```

The startup script will automatically:
1. Start all docker-compose services (Vault, API, Gateway, Web, Docs)
2. Initialize HashiCorp Vault with development secrets
3. Restart the API to load secrets from Vault
4. Verify system health

**Manual Start (Alternative):**
```bash
# Start all services
docker-compose up -d

# Populate Vault secrets
.\vault\scripts\populate-dev-secrets.ps1  # Windows
./vault/scripts/populate-dev-secrets.sh   # Linux/Mac

# Restart API to load secrets
docker-compose restart api
```

#### Stop All Services

```bash
# Stop all containers (keeps data)
docker-compose down

# Stop and remove all data (including database)
docker-compose down -v
```

#### View Service Logs

```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f api
docker-compose logs -f vault
docker-compose logs -f gateway
docker-compose logs -f web
```

### Access URLs

Once all services are running, access the application:

- **Frontend Application**: http://localhost:3000
- **API**: http://localhost:7777
- **Swagger UI**: http://localhost:7777/swagger
- **Gateway**: http://localhost:5000
- **API Documentation**: http://localhost:8080
- **Vault UI**: http://localhost:8200/ui (Token: `dev-root-token`)

**Through Gateway (Recommended):**
- **API Endpoints**: http://localhost:5000/api/transactions
- **Swagger UI**: http://localhost:5000/swagger/
- **API Documentation**: http://localhost:5000/docs/

## API Documentation

### Base URL (via Gateway)
```
http://localhost:5000/api
```

### Direct API URL (for development)
```
http://localhost:7777/api
```

### Endpoints

#### Transactions

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/transactions` | Get all transactions |
| GET | `/api/transactions/{id}` | Get transaction by ID |
| GET | `/api/transactions/status/{status}` | Get transactions by status |
| POST | `/api/transactions` | Create a new transaction |
| PUT | `/api/transactions/{id}` | Update a transaction |
| PATCH | `/api/transactions/{id}/status` | Update transaction status |
| DELETE | `/api/transactions/{id}` | Delete a transaction |

### Transaction Model

```json
{
  "id": 1,
  "transactionId": "TXN001",
  "amount": 100.50,
  "currency": "USD",
  "type": "Payment",
  "status": "Completed",
  "senderName": "John Doe",
  "receiverName": "Jane Smith",
  "description": "Payment for services",
  "createdAt": "2025-10-22T00:00:00Z",
  "completedAt": "2025-10-22T00:00:00Z"
}
```

### Transaction Types
- `Payment` - Standard payment transaction
- `Transfer` - Money transfer between accounts
- `Refund` - Refund transaction

### Transaction Statuses
- `Pending` - Transaction is pending
- `Completed` - Transaction completed successfully
- `Failed` - Transaction failed

## API Examples

### Get All Transactions
```bash
curl http://localhost:7777/api/transactions
```

### Get Transaction by ID
```bash
curl http://localhost:7777/api/transactions/1
```

### Create New Transaction
```bash
curl -X POST http://localhost:7777/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 150.00,
    "currency": "USD",
    "type": "Payment",
    "status": "Pending",
    "senderName": "Alice",
    "receiverName": "Bob",
    "description": "Service payment"
  }'
```

### Update Transaction Status
```bash
curl -X PATCH "http://localhost:7777/api/transactions/1/status?status=Completed"
```

### Delete Transaction
```bash
curl -X DELETE http://localhost:7777/api/transactions/1
```

## Docker Compose Setup

The project includes a `docker-compose.yml` file that runs all services in containers.

### Services Included

1. **HashiCorp Vault** - Secret Management
   - Image: `hashicorp/vault:1.15`
   - Port: `8200`
   - Mode: Development (in-memory storage)
   - Root Token: `dev-root-token`
   - UI: http://localhost:8200/ui
   - Stores: Database credentials, API keys, JWT secrets, Circle API credentials

2. **CoinPay API** - Backend Service
   - Built from: `./CoinPay.Api/Dockerfile`
   - Port: `7777`
   - Swagger UI: http://localhost:7777/swagger
   - Depends on: Vault (waits for healthy status)
   - Loads secrets from Vault on startup

3. **CoinPay Gateway** - API Gateway (YARP)
   - Built from: `./CoinPay.Gateway/Dockerfile`
   - Port: `5000`
   - Routes requests to API and Docs
   - Depends on: API

4. **CoinPay Web** - React Frontend
   - Built from: `./CoinPay.Web/Dockerfile`
   - Port: `3000`
   - UI: http://localhost:3000
   - Depends on: Gateway

5. **Documentation** - DocFX Site
   - Built from: `./docfx/Dockerfile`
   - Port: `8080`
   - Docs: http://localhost:8080

### Docker Compose Commands

#### Starting the Application

**Recommended (with automatic Vault setup):**
```powershell
# Windows
.\start-coinpay.ps1

# Linux/Mac
./start-coinpay.sh
```

**Manual start:**
```bash
# Start all services
docker-compose up -d

# Populate Vault with secrets
.\vault\scripts\populate-dev-secrets.ps1  # Windows
./vault/scripts/populate-dev-secrets.sh   # Linux/Mac

# Restart API to load secrets
docker-compose restart api

# Verify all services are running
docker-compose ps
```

#### Stopping the Application

```bash
# Stop all containers (keeps volumes/data)
docker-compose down

# Stop and remove all data (including database)
docker-compose down -v

# Stop specific service
docker-compose stop api
docker-compose stop vault
```

#### Restarting Services

```bash
# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart api
docker-compose restart vault
docker-compose restart gateway
docker-compose restart web
```

#### Viewing Logs

```bash
# View all logs (follow mode)
docker-compose logs -f

# View logs for specific service
docker-compose logs -f api
docker-compose logs -f vault
docker-compose logs -f gateway

# View last 100 lines of logs
docker-compose logs --tail=100 api

# View logs since specific time
docker-compose logs --since 30m api
```

#### Service Status and Health

```bash
# View running services and status
docker-compose ps

# Check Vault status
docker exec coinpay-vault vault status

# Check API health
curl http://localhost:7777/health

# View container resource usage
docker stats
```

#### Rebuilding Services

```bash
# Rebuild all services
docker-compose build

# Rebuild specific service
docker-compose build api
docker-compose build web

# Rebuild and restart
docker-compose up -d --build

# Force rebuild (no cache)
docker-compose build --no-cache api
```

#### Cleanup

```bash
# Remove stopped containers
docker-compose rm

# Remove all unused images, containers, networks
docker system prune

# Remove everything including volumes (WARNING: deletes all data)
docker-compose down -v
docker system prune -a --volumes
```

### Troubleshooting Docker Compose

#### Services Not Starting

```bash
# Check container logs
docker-compose logs api
docker-compose logs vault

# Check container status
docker-compose ps

# Restart specific service
docker-compose restart api
```

#### Port Already in Use

```bash
# Windows - Find process using port
netstat -ano | findstr :8200
taskkill /PID <process_id> /F

# Linux/Mac - Find and kill process
lsof -ti:8200 | xargs kill -9
```

#### Vault Secrets Not Loading

```bash
# Re-populate Vault secrets
.\vault\scripts\populate-dev-secrets.ps1

# Restart API
docker-compose restart api

# Verify secrets exist
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay
```

#### Database Connection Issues

The application uses Vault to store database credentials. If you see connection errors:

1. Check Vault is running: `docker-compose ps vault`
2. Verify Vault is healthy: `docker exec coinpay-vault vault status`
3. Re-populate secrets: `.\vault\scripts\populate-dev-secrets.ps1`
4. Restart API: `docker-compose restart api`

### Secret Management with Vault

All sensitive configuration is stored in HashiCorp Vault. On first start, you must populate Vault with secrets:

```powershell
# Windows
.\vault\scripts\populate-dev-secrets.ps1

# Linux/Mac
./vault/scripts/populate-dev-secrets.sh
```

**Important Notes:**
- Vault runs in **development mode** with in-memory storage
- Secrets are **lost on restart** and must be re-populated
- Root token: `dev-root-token`
- See `vault/README.md` for detailed Vault documentation

## Features

- **RESTful API** with full CRUD operations
- **HashiCorp Vault Integration** for secure secret management
- **PostgreSQL Database** with EF Core migrations
- **Circle API Integration** for Web3 cryptocurrency operations
- **Swagger UI** for interactive API documentation
- **YARP API Gateway** for unified service routing
- **React Frontend** with TypeScript and Tailwind CSS
- **CORS Enabled** for frontend integration
- **Auto-generated Transaction IDs**
- **Timestamp Tracking** for creation and completion
- **Status Management** with automatic completion timestamps
- **Docker Compose** for complete containerized deployment
- **Comprehensive Logging** with Serilog and observability
- **Health Checks** for all services

## Development

### API Architecture
The API follows the Minimal API pattern introduced in .NET 6+, providing a lightweight and performant approach to building APIs with minimal boilerplate code.

### Database
The application uses **PostgreSQL** as its database, running in Docker. Entity Framework Core manages the database schema through code-first migrations.

#### Database Configuration
Database credentials are securely stored in **HashiCorp Vault** and loaded at application startup.

**Vault Path:** `secret/coinpay/database`

**Default Development Values:**
- **Host**: postgres (container name)
- **Port**: 5432
- **Database**: coinpay
- **User**: postgres
- **Password**: root (stored in Vault)

To update database credentials:
```bash
# Update in Vault
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault \
  vault kv put secret/coinpay/database \
  host=postgres port=5432 database=coinpay \
  username=postgres password=newpassword \
  connection_string='Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=newpassword'

# Restart API to load new credentials
docker-compose restart api
```

#### Database Migrations
Migrations are applied automatically when the API starts. To manage migrations manually:

```bash
# Create a new migration
cd CoinPay.Api
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Rollback to a previous migration
dotnet ef database update PreviousMigrationName

# Remove last migration
dotnet ef migrations remove
```

### Sample Data
The database is pre-seeded with 3 sample transactions through EF Core seed data configuration.

## Sprint N06: Performance & Monitoring

This sprint introduces critical performance optimizations and monitoring capabilities to ensure API reliability and optimal user experience.

### Response Caching (BE-604)

Response caching reduces database load and improves response times for frequently accessed endpoints.

#### Configured Endpoints
- **GET /api/wallet/balance/{walletAddress}** - Cached for 30 seconds
- **GET /api/transactions/history** - Cached for 60 seconds with query parameter variations
- **GET /api/swap/quote** - Cached for 30 seconds with token, amount, and slippage variations

#### How It Works
- Responses are cached on the server using HTTP Cache-Control headers
- Cache is automatically validated on subsequent requests
- Uses `VaryByQueryKeys` to cache different results based on query parameters
- Clients receive cached responses when available, reducing API computation

#### Configuration
No additional configuration needed. Response caching is enabled by default in `Program.cs`:
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

### Application Insights Integration (BE-608)

Azure Application Insights provides comprehensive monitoring and diagnostics for the API.

#### Features
- Request/response tracking and performance metrics
- Dependency tracking (database, external API calls)
- Exception and error tracking
- Custom event logging
- Performance anomaly detection
- Real-time analytics dashboard

#### Configuration

**1. In appsettings.json:**
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key-here"
  }
}
```

**2. For Development (appsettings.Development.json):**
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "dev-instrumentation-key-placeholder"
  }
}
```

**3. For Production:**
1. Create an Application Insights resource in Azure Portal
2. Copy the Instrumentation Key
3. Store securely in HashiCorp Vault or Azure Key Vault
4. Update production configuration with the real key

#### Accessing Insights
- Azure Portal: https://portal.azure.com
- Application Insights Workspace: View performance metrics, failures, dependencies
- Live Metrics Stream: Real-time request and performance data

### Rate Limiting (BE-612)

Rate limiting protects the API from abuse and ensures fair resource distribution across users.

#### Configured Rules

**Development Environment (appsettings.Development.json):**
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 1000,
        "Message": "Max 1000 requests per minute"
      }
    ],
    "EndpointRules": [
      {
        "Endpoint": "POST:/api/auth/*",
        "Period": "1m",
        "Limit": 5,
        "Message": "Max 5 auth attempts per minute"
      },
      {
        "Endpoint": "POST:/api/swap/execute",
        "Period": "1m",
        "Limit": 10,
        "Message": "Max 10 swaps per minute"
      },
      {
        "Endpoint": "GET:/api/swap/quote",
        "Period": "1m",
        "Limit": 30,
        "Message": "Max 30 quote requests per minute"
      }
    ]
  }
}
```

**Production Environment (appsettings.Production.json):**
- More restrictive limits to prevent abuse
- Should be updated based on actual usage patterns
- Consider IP whitelisting for trusted services

#### HTTP Responses
When a client exceeds the rate limit:
- **Status Code:** 429 (Too Many Requests)
- **Response Body:**
```json
{
  "message": "API call quota exceeded. Maximum X requests per minute."
}
```

#### Configuring Rate Limits
1. Edit relevant `appsettings.*.json` file
2. Modify `IpRateLimiting` settings:
   - `GeneralRules`: Default rate limit for all endpoints
   - `EndpointRules`: Specific limits for sensitive endpoints
   - `Period`: Time window (s=seconds, m=minutes, h=hours)
   - `Limit`: Maximum requests allowed in the period

3. For exempting IPs (trusted services):
```json
{
  "IpRateLimiting": {
    "IpWhitelist": ["127.0.0.1", "192.168.1.100"],
    "EndpointWhitelist": ["/health", "/swagger"]
  }
}
```

### Performance Monitoring Best Practices

1. **Monitor Response Times**
   - Ideal: < 200ms for GET requests
   - Acceptable: < 500ms for POST requests
   - Check Application Insights for baseline metrics

2. **Cache Hit Ratios**
   - Aim for > 80% cache hit rate on GET endpoints
   - Monitor via browser Developer Tools (Cache-Control headers)

3. **Rate Limit Monitoring**
   - Watch for 429 responses in Application Insights
   - Adjust limits if legitimate users are being blocked
   - Review traffic patterns during peak usage

4. **Database Query Performance**
   - Monitor slow queries in PostgreSQL logs
   - Ensure indexes are created on frequently filtered columns
   - Use EXPLAIN ANALYZE to optimize query plans

5. **API Health Checks**
   - GET /health - Overall health status
   - GET /health/ready - Ready to serve traffic
   - GET /health/live - Liveness probe for orchestration

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is open source and available under the [MIT License](LICENSE).

## Contact

Developer: developerhakart@yahoo.com

## Project Status

This project is currently in active development. The API backend is functional with full CRUD operations, and the frontend integration is in progress.
