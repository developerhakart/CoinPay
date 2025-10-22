# CoinPay

A modern cryptocurrency payment platform with a RESTful API backend and React frontend.

## Project Structure

```
CoinPay/
├── CoinPay.Api/          # .NET Core Minimal API Backend
│   └── CoinPay.Api/      # API Project
│       ├── Models/       # Data models
│       ├── Data/         # Database context
│       └── Program.cs    # API endpoints and configuration
└── CoinPay.Web/          # React Frontend Application
    └── src/              # React source files
```

## Technologies Used

### Backend (CoinPay.Api)
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Minimal API** - Lightweight API architecture
- **Entity Framework Core InMemory** - In-memory database for development
- **Swagger/OpenAPI** - API documentation and testing interface
- **CORS** - Cross-Origin Resource Sharing enabled

### Frontend (CoinPay.Web)
- **React 18** - Modern React framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework

## Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- Git

### Backend Setup

1. Navigate to the API directory:
```bash
cd CoinPay.Api/CoinPay.Api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the API:
```bash
dotnet run --launch-profile http
```

The API will start on **http://localhost:7777**

5. Access Swagger Documentation:
Open your browser and navigate to: **http://localhost:7777/swagger**

### Frontend Setup

1. Navigate to the Web directory:
```bash
cd CoinPay.Web
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The web application will typically start on **http://localhost:5173**

## API Documentation

### Base URL
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

## Features

- **RESTful API** with full CRUD operations
- **In-Memory Database** for fast development and testing
- **Swagger UI** for interactive API documentation
- **CORS Enabled** for frontend integration
- **Auto-generated Transaction IDs**
- **Timestamp Tracking** for creation and completion
- **Status Management** with automatic completion timestamps

## Development

### API Architecture
The API follows the Minimal API pattern introduced in .NET 6+, providing a lightweight and performant approach to building APIs with minimal boilerplate code.

### Database
Currently using Entity Framework Core InMemory database for development. This can be easily switched to SQL Server, PostgreSQL, or other databases by updating the DbContext configuration in `Program.cs`.

### Sample Data
The API comes pre-seeded with 3 sample transactions for testing purposes.

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
