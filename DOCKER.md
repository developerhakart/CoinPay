# Docker Setup Guide

This guide explains how to run the entire CoinPay platform using Docker Compose.

## Services Included

The docker-compose configuration includes the following services:

1. **postgres** - PostgreSQL 15 database (Port 5432)
2. **api** - CoinPay.Api backend service (Port 7777)
3. **gateway** - CoinPay.Gateway API gateway (Port 5000)
4. **web** - CoinPay.Web React frontend (Port 3000)
5. **docfx** - API documentation site (Port 8080)
6. **pgadmin** - PostgreSQL admin interface (Port 5050)

## Prerequisites

- Docker Desktop installed and running
- Docker Compose (included with Docker Desktop)

## Quick Start

### Start All Services

```bash
docker-compose up -d
```

This will:
- Build all images (first time only)
- Start all containers in detached mode
- Create the coinpay-network for inter-service communication
- Initialize the PostgreSQL database with persistent volume

### Check Service Status

```bash
docker-compose ps
```

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f web
docker-compose logs -f gateway
```

### Stop All Services

```bash
docker-compose down
```

### Stop and Remove Volumes (Clean Reset)

```bash
docker-compose down -v
```

## Service URLs

Once all services are running:

- **Frontend**: http://localhost:3000
- **API Gateway**: http://localhost:5000
- **API Direct**: http://localhost:7777
- **API Documentation**: http://localhost:8080
- **PgAdmin**: http://localhost:5050
  - Email: admin@coinpay.com
  - Password: admin
- **PostgreSQL**: localhost:5432
  - Username: postgres
  - Password: root
  - Database: coinpay

## Development Workflow

### Rebuild Specific Service

If you make changes to code:

```bash
# Rebuild and restart API
docker-compose up -d --build api

# Rebuild and restart Web frontend
docker-compose up -d --build web

# Rebuild and restart Gateway
docker-compose up -d --build gateway
```

### Access Database

#### Using PgAdmin (Web UI)
1. Open http://localhost:5050
2. Login with admin@coinpay.com / admin
3. Add server:
   - Name: CoinPay
   - Host: postgres (use service name, not localhost)
   - Port: 5432
   - Username: postgres
   - Password: root

#### Using psql Command Line
```bash
docker exec -it coinpay-postgres psql -U postgres -d coinpay
```

### Execute Commands in Containers

```bash
# Run migrations in API container
docker exec -it coinpay-api dotnet ef database update

# Access API container shell
docker exec -it coinpay-api /bin/bash

# Access frontend container shell
docker exec -it coinpay-web /bin/sh
```

## Troubleshooting

### Port Already in Use

If you get "port already in use" errors:

1. Check what's using the port:
   ```bash
   # Windows
   netstat -ano | findstr :5432

   # Linux/Mac
   lsof -i :5432
   ```

2. Stop the conflicting service or change the port in docker-compose.yml

### Container Won't Start

Check logs for the specific container:
```bash
docker-compose logs api
```

### Database Connection Issues

Ensure postgres is healthy before other services start:
```bash
docker-compose ps
```

Look for "healthy" status for postgres service.

### Rebuild Everything from Scratch

```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

## Environment Variables

Environment variables are configured in docker-compose.yml for each service:

- **API Service**: Database connection, JWT secrets, Circle API credentials
- **Gateway Service**: ASPNETCORE settings
- **Web Service**: VITE_API_BASE_URL (build-time variable)

To override, create a `.env` file or use docker-compose.override.yml

## Production Considerations

For production deployment:

1. Change default passwords in docker-compose.yml
2. Use environment-specific compose files
3. Configure proper secrets management
4. Set up SSL/TLS certificates
5. Configure reverse proxy (nginx/traefik)
6. Set up monitoring and logging
7. Use production-grade database with backups

## Architecture

```
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │
       ▼
┌─────────────┐     ┌──────────────┐     ┌──────────────┐
│  Web (3000) │────▶│Gateway (5000)│────▶│ API (7777)   │
└─────────────┘     └──────────────┘     └──────┬───────┘
                                                 │
       ┌─────────────────────────────────────────┘
       │
       ▼
┌──────────────┐
│Postgres (5432)│
└──────────────┘

Additional Services:
- DocFx (8080): API Documentation
- PgAdmin (5050): Database Admin UI
```

## Network

All services run on the `coinpay-network` bridge network, allowing them to communicate using service names:

- API connects to postgres using `Host=postgres`
- Gateway connects to API using `http://api:8080`
- Services are isolated from other Docker networks

## Volumes

- `postgres-data`: Persistent storage for PostgreSQL database

Data persists between container restarts unless explicitly removed with `-v` flag.
