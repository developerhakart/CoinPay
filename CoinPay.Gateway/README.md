# CoinPay Gateway

YARP (Yet Another Reverse Proxy) based API Gateway for the CoinPay platform.

## Overview

This gateway acts as a single entry point for all CoinPay services, routing requests to:
- **CoinPay API** (port 7777)
- **DocFX Documentation** (port 8080)

## Configuration

The gateway runs on **port 5000** and uses YARP for reverse proxy functionality.

### Routes

| Path | Destination | Description |
|------|-------------|-------------|
| `/api/**` | http://localhost:7777/api/** | Transaction API endpoints |
| `/swagger/**` | http://localhost:7777/swagger/** | Swagger UI documentation |
| `/docs/**` | http://localhost:8080/** | DocFX API documentation |
| `/` | Gateway Info | Gateway service information |

## Running the Gateway

```bash
cd CoinPay.Gateway
dotnet run --launch-profile http
```

The gateway will start on: **http://localhost:5000**

## Service URLs

Once the gateway is running, access services through:

- **API**: http://localhost:5000/api/transactions
- **Swagger UI**: http://localhost:5000/swagger/
- **Documentation**: http://localhost:5000/docs/

## Prerequisites

Before starting the gateway, ensure these services are running:

1. **CoinPay API** on port 7777
```bash
cd CoinPay.Api
dotnet run --launch-profile http
```

2. **DocFX Server** on port 8080
```bash
cd docfx
docfx serve _site --port 8080
```

## CORS

The gateway has CORS enabled for all origins to support frontend development. For production, update the CORS policy in `Program.cs` to restrict to specific domains.

## Dependencies

- Yarp.ReverseProxy (v2.3.0)
- .NET 9.0

## Architecture

```
┌─────────────┐
│   Frontend  │
│ (port 3000) │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│  YARP Gateway   │
│  (port 5000)    │
└────────┬────────┘
         │
    ┌────┴─────────────────┐
    │                      │
    ▼                      ▼
┌─────────┐         ┌──────────┐
│   API   │         │  DocFX   │
│ (7777)  │         │  (8080)  │
└─────────┘         └──────────┘
```

## Configuration Files

- `appsettings.json` - YARP route and cluster configuration
- `Program.cs` - Gateway setup and middleware configuration
- `Properties/launchSettings.json` - Launch profiles and port configuration
