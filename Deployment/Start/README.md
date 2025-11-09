# CoinPay Deployment Scripts & Guides

This folder contains all scripts and guides needed to manually build, start, and stop CoinPay Docker containers.

## Quick Start

### 1. Build Docker Images

**PowerShell:**
```powershell
.\Deployment\Start\build-coinpay.ps1
```

**Bash:**
```bash
./Deployment/Start/build-coinpay.sh
```

### 2. Start All Services

**PowerShell:**
```powershell
.\Deployment\Start\start-coinpay.ps1
```

**Bash:**
```bash
./Deployment/Start/start-coinpay.sh
```

### 3. Stop All Services

**PowerShell:**
```powershell
.\Deployment\Start\stop-coinpay.ps1
```

**Bash:**
```bash
./Deployment/Start/stop-coinpay.sh
```

---

## Available Scripts

### Build Scripts

| Script | Description |
|--------|-------------|
| `build-coinpay.ps1` | Build all Docker images (PowerShell) |
| `build-coinpay.sh` | Build all Docker images (Bash) |

**What it does:**
- Builds API, Gateway, Web, and Docs Docker images
- Verifies all images were created successfully
- Shows image sizes
- PostgreSQL and Vault use official images (no build needed)

**When to use:**
- First time setup
- After code changes in backend or frontend
- After updating Dockerfile configurations

---

### Start Scripts

| Script | Description |
|--------|-------------|
| `start-coinpay.ps1` | Start all CoinPay services (PowerShell) |
| `start-coinpay.sh` | Start all CoinPay services (Bash) |

**What it does:**
1. Starts all docker-compose services
2. Populates Vault with dev secrets
3. Restarts API to load Vault secrets
4. Verifies system health

**Services Started:**
- **API**: http://localhost:7777 (Swagger: /swagger)
- **Gateway**: http://localhost:5000
- **Web UI**: http://localhost:3000
- **Docs**: http://localhost:8080
- **Vault**: http://localhost:8200/ui (Token: `dev-root-token`)
- **PostgreSQL**: localhost:5432

**Vault Secrets Populated:**
- Database connection strings
- Redis connection
- Circle API configuration (mock mode enabled)
- WhiteBit API configuration (mock mode enabled)
- OneInch DEX configuration (mock mode enabled)
- JWT authentication settings
- Gateway webhook secrets
- Blockchain wallet configuration

---

### Rebuild Scripts

| Script | Description |
|--------|-------------|
| `rebuild-web.ps1` | Rebuild only the web (frontend) service |
| `rebuild-all.ps1` | Rebuild all services (API, Gateway, Web, Docs) |

**rebuild-web.ps1:**
- Stops the web container
- Rebuilds the web Docker image
- Starts the web container
- **Use after frontend code changes**

**rebuild-all.ps1:**
- Stops all containers (with backup)
- Rebuilds all Docker images
- Starts all containers (with restore)
- **Use after backend or frontend code changes**

**When to use:**
- After pulling new code from Git
- After modifying React/TypeScript frontend code
- After modifying .NET backend code
- After updating Dockerfiles

**Example - After Git Pull:**
```powershell
# Pull latest code
git pull origin development

# Rebuild and restart
.\Deployment\Start\rebuild-all.ps1
```

**Example - After Frontend Changes:**
```powershell
# Rebuild just the web service (faster)
.\Deployment\Start\rebuild-web.ps1
```

---

### Stop Scripts

| Script | Description |
|--------|-------------|
| `stop-coinpay.ps1` | Stop all CoinPay services (PowerShell) |
| `stop-coinpay.sh` | Stop all CoinPay services (Bash) |

**What it does:**
- Stops all running containers
- Removes containers (preserves volumes)
- Verifies all containers stopped

**Data Preservation:**
- ✅ PostgreSQL data volume preserved
- ❌ Vault data lost (dev mode uses in-memory storage)

**Note:** After stopping, you'll need to re-run `start-coinpay` to repopulate Vault secrets.

---

### Backup & Restore Script

| Script | Description |
|--------|-------------|
| `backup-restore.sh` | Backup and restore PostgreSQL database |

**Usage:**

```bash
# Create backup
./Deployment/Start/backup-restore.sh backup

# List backups
./Deployment/Start/backup-restore.sh list

# Restore from backup
./Deployment/Start/backup-restore.sh restore backup-2025-11-06.sql
```

---

## Deployment Guides

### Core Documentation

| Guide | Description |
|-------|-------------|
| `VAULT_ADMIN_GUIDE.md` | Complete Vault configuration management guide |
| `DOCKER-COMPOSE-DEPLOYMENT.md` | Docker Compose deployment instructions |
| `SAFE-DEPLOYMENT-STRATEGY.md` | Safe deployment strategy and best practices |

### Vault Admin Guide

**Key Topics:**
- Switching between Mock Mode and Real Mode
- Managing API configurations (Circle, WhiteBit, OneInch)
- Vault UI and CLI usage
- Configuration reference for all services
- Troubleshooting common issues
- Production deployment checklist

**Quick Example:**
```bash
# Switch Circle API to Real Mode
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='false'

# Restart API to load changes
docker-compose restart api
```

---

## Typical Workflows

### First Time Setup

```bash
# 1. Build Docker images
./Deployment/Start/build-coinpay.sh

# 2. Start all services (includes Vault population)
./Deployment/Start/start-coinpay.sh

# 3. Verify services are running
curl http://localhost:7777/health
```

### Daily Development

```bash
# Start services
./Deployment/Start/start-coinpay.sh

# ... do development work ...

# Stop services when done
./Deployment/Start/stop-coinpay.sh
```

### After Code Changes

```bash
# 1. Stop current services
./Deployment/Start/stop-coinpay.sh

# 2. Rebuild affected images
docker-compose build api    # If backend changed
docker-compose build web    # If frontend changed

# 3. Start services again
./Deployment/Start/start-coinpay.sh
```

### Switching API Modes (Mock ↔ Real)

**Option 1: Via Vault UI**
1. Go to http://localhost:8200/ui
2. Login with token: `dev-root-token`
3. Navigate: secret/ → coinpay/ → circle
4. Edit `use_mock_mode` field
5. Restart: `docker-compose restart api`

**Option 2: Via Vault CLI**
```bash
# Enable Mock Mode
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='true'
docker-compose restart api

# Enable Real Mode (requires real credentials!)
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='false'
docker-compose restart api
```

See `VAULT_ADMIN_GUIDE.md` for complete details.

---

## Environment-Specific Notes

### Development Environment

- **Mock Mode**: Enabled by default (Circle, WhiteBit, OneInch)
- **Vault**: Dev mode (in-memory, secrets lost on restart)
- **Database**: Persistent volume (data preserved)
- **Ports**: Standard development ports (3000, 5000, 7777, etc.)

### Production Environment

⚠️ **Before deploying to production:**

1. **Build Production Images:**
   ```bash
   docker-compose -f docker-compose.prod.yml build
   ```

2. **Configure Real API Credentials in Vault:**
   - Obtain real Circle API keys
   - Obtain 1inch API keys
   - Update Vault secrets with real credentials
   - Set `use_mock_mode='false'`

3. **Use Production Vault:**
   - NOT dev mode
   - Sealed storage with encryption
   - Proper authentication (AppRole/Kubernetes)

4. **Review Security:**
   - See `SAFE-DEPLOYMENT-STRATEGY.md`
   - Enable TLS/HTTPS
   - Use strong secrets
   - Enable audit logging

---

## Troubleshooting

### Services Won't Start

**Check Docker is running:**
```bash
docker ps
```

**Check ports aren't in use:**
```bash
# Windows
netstat -ano | findstr :7777
netstat -ano | findstr :3000
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :7777
lsof -i :3000
lsof -i :5000
```

### Vault Secrets Not Loading

**Repopulate Vault:**
```bash
./vault/scripts/populate-dev-secrets.ps1
docker-compose restart api
```

**Check Vault is running:**
```bash
curl http://localhost:8200/v1/sys/health
```

### API Returns 500 Errors

**Check if mock mode is enabled:**
```bash
docker logs coinpay-api --tail 50 | grep "UseMockMode"
```

Should show:
```
Circle API configuration loaded from Vault (UseMockMode: true)
```

**If showing false but you want mock mode:**
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='true'
docker-compose restart api
```

### Database Connection Issues

**Check PostgreSQL is running:**
```bash
docker ps | grep postgres
```

**Test connection:**
```bash
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT 1"
```

---

## Additional Resources

- **Root README**: `../../README.md` - Project overview
- **API Documentation**: http://localhost:7777/swagger (when running)
- **Vault UI**: http://localhost:8200/ui (token: `dev-root-token`)
- **Docker Compose File**: `../../docker-compose.yml`

---

## Support

For issues:
1. Check service logs: `docker logs coinpay-api`
2. Review relevant guide in this folder
3. Check troubleshooting sections
4. Verify all prerequisites are met

---

## File Structure

```
Deployment/Start/
├── README.md                           # This file
├── build-coinpay.ps1                   # Build script (PowerShell)
├── build-coinpay.sh                    # Build script (Bash)
├── start-coinpay.ps1                   # Start script (PowerShell)
├── start-coinpay.sh                    # Start script (Bash)
├── stop-coinpay.ps1                    # Stop script (PowerShell)
├── stop-coinpay.sh                     # Stop script (Bash)
├── backup-restore.sh                   # Database backup/restore
├── VAULT_ADMIN_GUIDE.md               # Vault configuration guide
├── DOCKER-COMPOSE-DEPLOYMENT.md       # Docker Compose guide
└── SAFE-DEPLOYMENT-STRATEGY.md        # Deployment best practices
```

---

## Quick Command Reference

```bash
# Build
./Deployment/Start/build-coinpay.sh

# Start
./Deployment/Start/start-coinpay.sh

# Stop
./Deployment/Start/stop-coinpay.sh

# Check status
docker-compose ps

# View logs
docker logs -f coinpay-api
docker logs -f coinpay-web

# Restart specific service
docker-compose restart api

# Access containers
docker exec -it coinpay-api bash
docker exec -it coinpay-postgres-compose psql -U postgres -d coinpay

# Backup database
./Deployment/Start/backup-restore.sh backup

# Health check
curl http://localhost:7777/health
```
