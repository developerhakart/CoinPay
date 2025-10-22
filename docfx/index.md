# CoinPay API Documentation

Welcome to the CoinPay API documentation. This documentation provides comprehensive information about the CoinPay cryptocurrency payment platform API.

## Overview

CoinPay is a modern .NET 9.0 Minimal API that provides RESTful endpoints for managing cryptocurrency payment transactions. The API is designed to be lightweight, fast, and easy to integrate with frontend applications.

## Features

- **RESTful API Design** - Standard HTTP methods for CRUD operations
- **Swagger/OpenAPI** - Interactive API documentation and testing
- **CORS Enabled** - Ready for frontend integration
- **In-Memory Database** - Fast development and testing
- **Minimal API Pattern** - Lightweight and performant

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- Any HTTP client (curl, Postman, or use Swagger UI)

### Running the API

```bash
cd CoinPay.Api
dotnet restore
dotnet run --launch-profile http
```

The API will be available at: **http://localhost:7777**

### Swagger UI

Access the interactive API documentation at: **http://localhost:7777/swagger**

## API Endpoints

The CoinPay API provides the following endpoints:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/transactions` | Get all transactions |
| GET | `/api/transactions/{id}` | Get transaction by ID |
| GET | `/api/transactions/status/{status}` | Get transactions by status |
| POST | `/api/transactions` | Create a new transaction |
| PUT | `/api/transactions/{id}` | Update a transaction |
| PATCH | `/api/transactions/{id}/status` | Update transaction status |
| DELETE | `/api/transactions/{id}` | Delete a transaction |

## Transaction Model

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

## Getting Started

1. [API Reference](api/index.md) - Detailed API documentation
2. [Articles](articles/toc.yml) - Guides and tutorials
3. [GitHub Repository](https://github.com/developerhakart/CoinPay) - Source code

## Support

For questions or issues, please visit our [GitHub Issues](https://github.com/developerhakart/CoinPay/issues) page.

## License

This project is licensed under the MIT License.
