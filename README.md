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

### Frontend (CoinPay.Web)
- **React 18** - Modern React framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework

### Documentation
- **DocFX** - Static documentation site generator

## Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for running PostgreSQL)
- [DocFX](https://dotnet.github.io/docfx/) (for documentation)
- Git

### Quick Start (Using Gateway - Recommended)

The gateway provides a single entry point for all services.

#### Step 1: Start Database Services

Start PostgreSQL and pgAdmin using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- **PostgreSQL**: localhost:5432 (Database: coinpay, User: postgres, Password: root)
- **pgAdmin**: http://localhost:5050 (Email: admin@coinpay.com, Password: admin)

To stop the database services:
```bash
docker-compose down
```

To stop and remove all data:
```bash
docker-compose down -v
```

#### Step 2: Start Backend Services

1. **Start the API** (Terminal 1):
```bash
cd CoinPay.Api
dotnet run --launch-profile http
```
API runs on: **http://localhost:7777**
- Database migrations are applied automatically on startup

2. **Start DocFX** (Terminal 2):
```bash
cd docfx
docfx serve _site --port 8080
```
Docs run on: **http://localhost:8080**

#### Step 3: Start Gateway and Frontend

3. **Start the Gateway** (Terminal 3):
```bash
cd CoinPay.Gateway
dotnet run --launch-profile http
```
Gateway runs on: **http://localhost:5000**

4. **Start the Frontend** (Terminal 4):
```bash
cd CoinPay.Web
npm install
npm run dev
```
Frontend runs on: **http://localhost:3000**

### Access URLs (Through Gateway)

Once all services are running, access everything through the gateway:

- **Frontend Application**: http://localhost:3000
- **API Endpoints**: http://localhost:5000/api/transactions
- **Swagger UI**: http://localhost:5000/swagger/
- **API Documentation**: http://localhost:5000/docs/
- **Gateway Info**: http://localhost:5000/

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

The project includes a `docker-compose.yml` file for easy local development setup.

### Services Included

1. **PostgreSQL Database**
   - Image: `postgres:15-alpine`
   - Port: `5432`
   - Database: `coinpay`
   - Username: `postgres`
   - Password: `root`
   - Health checks enabled
   - Persistent volume: `postgres-data`

2. **pgAdmin (Database Management UI)**
   - Image: `dpage/pgadmin4:latest`
   - Port: `5050`
   - URL: http://localhost:5050
   - Email: `admin@coinpay.com`
   - Password: `admin`

### Docker Compose Commands

```bash
# Start all services (detached mode)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services (keeps data)
docker-compose stop

# Stop and remove containers (keeps data)
docker-compose down

# Stop and remove containers + volumes (removes all data)
docker-compose down -v

# Restart services
docker-compose restart

# View running services
docker-compose ps
```

### Connecting to PostgreSQL via pgAdmin

1. Open http://localhost:5050 in your browser
2. Login with `admin@coinpay.com` / `admin`
3. Right-click on "Servers" and select "Create" > "Server"
4. Configure connection:
   - **General Tab**: Name: `CoinPay Local`
   - **Connection Tab**:
     - Host: `postgres` (use container name)
     - Port: `5432`
     - Maintenance database: `coinpay`
     - Username: `postgres`
     - Password: `root`
5. Click "Save"

## Features

- **RESTful API** with full CRUD operations
- **PostgreSQL Database** with EF Core migrations
- **Swagger UI** for interactive API documentation
- **CORS Enabled** for frontend integration
- **Auto-generated Transaction IDs**
- **Timestamp Tracking** for creation and completion
- **Status Management** with automatic completion timestamps
- **Docker Compose** for easy local development setup

## Development

### API Architecture
The API follows the Minimal API pattern introduced in .NET 6+, providing a lightweight and performant approach to building APIs with minimal boilerplate code.

### Database
The application uses **PostgreSQL** as its database, running in Docker. Entity Framework Core manages the database schema through code-first migrations.

#### Database Configuration
- **Connection String**: Configured in `appsettings.Development.json`
- **Host**: localhost:5432
- **Database**: coinpay
- **User**: postgres
- **Password**: root

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
