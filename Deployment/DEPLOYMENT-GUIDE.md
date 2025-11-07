# CoinPay Deployment Guide

## Quick Start

### Start CoinPay
```powershell
.\Deployment\Start\start-coinpay.ps1
```

### Stop CoinPay
```powershell
.\Deployment\Start\stop-coinpay.ps1
```

## Folder Structure

```
Deployment/
├── docker-compose.yml           # Main Docker Compose configuration
├── populate-dev-secrets.ps1     # Vault secrets population script
├── README.md                    # Deployment overview
├── DEPLOYMENT-GUIDE.md          # This file
└── Start/                       # Start/Stop/Backup/Restore scripts
    ├── start-coinpay.ps1       # Main start script
    ├── stop-coinpay.ps1        # Main stop script (auto-backup)
    ├── backup-database.ps1     # Manual database backup
    ├── backup-vault.ps1        # Manual Vault backup
    ├── restore-database.ps1    # Manual database restore
    ├── restore-vault.ps1       # Manual Vault restore
    ├── init-real-wallet.ps1    # Circle wallet initialization
    ├── backups/                # Backup storage directory
    └── README.md               # Scripts documentation
```

## Features

### ✅ Automatic Backup on Stop
When you run `stop-coinpay.ps1`:
1. Backs up PostgreSQL database
2. Backs up Vault secrets (8 secrets)
3. Stops all containers
4. Removes containers

### ✅ Automatic Restore on Start
When you run `start-coinpay.ps1`:
1. Cleans up any existing containers
2. Starts all services
3. Restores database from backup (if empty)
4. Restores Vault secrets from backup (if empty)
5. Initializes real Circle Wallet IDs
6. Restarts API to load secrets
7. Verifies system health

### ✅ Circle Wallet Management
- Real Circle Wallet IDs automatically configured
- Persists across stop/start cycles
- No manual updates required
- See: `Start/AUTOMATIC-WALLET-INITIALIZATION.md`

## Configuration

### Circle API
**Mode:** Real API (use_mock_mode: false)
**Credentials:** Stored in Vault `secret/coinpay/circle`

To switch to mock mode:
```powershell
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='true'
docker-compose restart api
```

### Vault Secrets
All secrets stored in Vault under `secret/coinpay/`:
- `database` - PostgreSQL connection
- `redis` - Redis connection
- `circle` - Circle API credentials
- `jwt` - JWT signing keys
- `gateway` - Gateway webhook secret
- `blockchain` - Blockchain private keys
- `whitebit` - WhiteBit exchange API
- `oneinch` - 1inch DEX API

## Manual Operations

### Manual Backup
```powershell
# Backup database only
.\Deployment\Start\backup-database.ps1

# Backup Vault only
.\Deployment\Start\backup-vault.ps1
```

### Manual Restore
```powershell
# Restore latest database backup
.\Deployment\Start\restore-database.ps1

# Restore specific backup
.\Deployment\Start\restore-database.ps1 -BackupFile ".\Deployment\Start\backups\database_20251107_125318.sql"

# Restore latest Vault backup
.\Deployment\Start\restore-vault.ps1

# Restore specific Vault backup
.\Deployment\Start\restore-vault.ps1 -Timestamp "20251107_125318"
```

### Populate Vault Secrets
```powershell
.\Deployment\populate-dev-secrets.ps1
```

## Services

After starting, the following services are available:

| Service | URL | Description |
|---------|-----|-------------|
| API | http://localhost:7777 | Backend API |
| Swagger | http://localhost:7777/swagger | API Documentation |
| Gateway | http://localhost:5000 | API Gateway |
| Web UI | http://localhost:3000 | Frontend Application |
| Docs | http://localhost:8080 | MkDocs Documentation |
| Vault UI | http://localhost:8200/ui | Vault Management |

**Test User:**
- Username: testuser
- Wallet: 0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
- Circle Wallet ID: fef70777-cb2d-5096-a0ea-15dba5662ce6

## Troubleshooting

### Check Logs
```powershell
# All services
docker-compose logs -f

# Specific service
docker logs coinpay-api -f
```

### Check Container Status
```powershell
docker-compose ps
```

### Check Vault Secrets
```powershell
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay
```

### Verify Database
```powershell
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "SELECT \"Id\", \"Username\", \"CircleWalletId\" FROM \"Users\";"
```

## Important Notes

### Development Mode
- Vault runs in **dev mode** (in-memory storage)
- Secrets lost on Vault container restart
- Automatic backup/restore handles this

### Production Deployment
For production:
1. Enable Vault production mode (persistent storage)
2. Use real SSL certificates
3. Update all credentials
4. Set `use_mock_mode='false'` for all services
5. Configure backup retention policies

## Support Files

- `Start/FIXES-SUMMARY.md` - Complete fix history
- `Start/AUTOMATIC-WALLET-INITIALIZATION.md` - Wallet management details
- `Start/IMPLEMENTATION-SUMMARY.md` - Backup/restore implementation
- `Start/AUTOMATIC-CLEANUP-IMPLEMENTATION.md` - Container cleanup details

## Archive

Old files and documentation moved to `Archive/` folder:
- `Archive/Deployment/` - Old deployment files
- `Archive/vault/` - Old vault configuration
- `Archive/Documentation/` - Old documentation
