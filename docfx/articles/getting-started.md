# Getting Started with CoinPay API

This guide will help you get started with the CoinPay API.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A code editor (Visual Studio, VS Code, or Rider)
- A REST client (Postman, curl, or use Swagger UI)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/developerhakart/CoinPay.git
cd CoinPay
```

### 2. Restore Dependencies

```bash
cd CoinPay.Api
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the API

```bash
dotnet run --launch-profile http
```

The API will start on **http://localhost:7777**

## Verify Installation

### Using Swagger UI

1. Open your browser
2. Navigate to: http://localhost:7777/swagger
3. You should see the Swagger documentation interface

### Using curl

```bash
curl http://localhost:7777/api/transactions
```

You should receive a JSON response with sample transactions.

## Next Steps

- Read the [API Usage Examples](api-examples.md) to learn how to use the API
- Check the [Configuration Guide](configuration.md) for advanced setup
- Explore the [API Reference](../api/index.md) for detailed endpoint information

## Troubleshooting

### Port Already in Use

If port 7777 is already in use, you can change it in `Properties/launchSettings.json`:

```json
"applicationUrl": "http://localhost:YOUR_PORT"
```

### Build Errors

Make sure you have .NET 9.0 SDK installed:

```bash
dotnet --version
```

Should return version 9.0.x or higher.
