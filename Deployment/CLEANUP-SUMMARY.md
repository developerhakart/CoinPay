# Deployment Cleanup Summary

## Overview
Cleaned up and reorganized deployment structure for simplicity and maintainability.

## What Was Done

### 1. Cleaned Deployment/Start Folder
**Kept (Essential Scripts):**
- ✅ `start-coinpay.ps1` - Main startup script with auto-restore
- ✅ `stop-coinpay.ps1` - Main stop script with auto-backup
- ✅ `backup-database.ps1` - Database backup
- ✅ `backup-vault.ps1` - Vault secrets backup
- ✅ `restore-database.ps1` - Database restore
- ✅ `restore-vault.ps1` - Vault secrets restore
- ✅ `init-real-wallet.ps1` - Circle wallet initialization
- ✅ `README.md` - Scripts documentation
- ✅ `backups/` - Backup storage directory

**Moved to Archive:**
- ❌ `backup-restore.sh` - Old bash script (PowerShell versions preferred)
- ❌ `build-coinpay.ps1` - Build script (not needed for deployment)
- ❌ `build-coinpay.sh` - Build script (not needed for deployment)
- ❌ `start-coinpay.sh` - Old bash version
- ❌ `stop-coinpay.sh` - Old bash version
- ❌ `DOCKER-COMPOSE-DEPLOYMENT.md` - Old documentation
- ❌ `SAFE-DEPLOYMENT-STRATEGY.md` - Old documentation
- ❌ `VAULT_ADMIN_GUIDE.md` - Old documentation

### 2. Cleaned Deployment Root Folder
**Kept:**
- ✅ `docker-compose.yml` - Main Docker Compose config (copied from root)
- ✅ `populate-dev-secrets.ps1` - Vault secrets population
- ✅ `README.md` - Deployment overview
- ✅ `DEPLOYMENT-GUIDE.md` - Complete deployment guide (NEW)
- ✅ `CLEANUP-SUMMARY.md` - This file (NEW)
- ✅ `Start/` - Start/stop scripts folder

**Moved to Archive:**
- ❌ `.github-workflows-docker-deploy.yml` - Old GitHub workflow
- ❌ `regression-test.sh` - Old test script
- ❌ `docker-compose.yml` (old version) - Replaced with root version
- ❌ `backups/` - Old backup folders

### 3. Vault Folder Reorganization
**Moved entire `vault/` folder to Archive:**
- ❌ `vault/config/` - Vault HCL config (using dev mode)
- ❌ `vault/data/` - Old data directory
- ❌ `vault/logs/` - Old logs directory
- ❌ `vault/scripts/init-and-unseal.ps1` - Production sealing (archived)
- ❌ `vault/scripts/init-secrets.ps1` - Old initialization
- ❌ `vault/README.md` - Old documentation
- ❌ `vault/VAULT-*.md` - Old documentation

**Kept (moved to Deployment):**
- ✅ `populate-dev-secrets.ps1` - Essential for dev mode Vault

**Why:** We use Vault in **dev mode** (in-memory), so complex sealing/unsealing isn't needed. The `populate-dev-secrets.ps1` script handles all our needs.

### 4. Docker Compose Consolidation
**Before:**
- `./docker-compose.yml` (root) - Main version
- `./Deployment/docker-compose.yml` - Old version
- `./Deployment/backups/*/docker-compose.yml` - Old backups

**After:**
- `./docker-compose.yml` (root) - **Primary version** (used by scripts)
- `./Deployment/docker-compose.yml` - **Copy for reference**
- Old versions moved to `Archive/Deployment/`

## New Folder Structure

```
D:\Projects\Test\Claude\CoinPay\
│
├── docker-compose.yml                    # MAIN Docker Compose config
│
├── Deployment/
│   ├── docker-compose.yml               # Copy of main config
│   ├── populate-dev-secrets.ps1         # Vault secrets population
│   ├── README.md                        # Overview
│   ├── DEPLOYMENT-GUIDE.md              # Complete guide (NEW)
│   ├── CLEANUP-SUMMARY.md               # This file (NEW)
│   │
│   └── Start/                           # Deployment scripts
│       ├── start-coinpay.ps1            # Start with auto-restore
│       ├── stop-coinpay.ps1             # Stop with auto-backup
│       ├── backup-database.ps1          # DB backup
│       ├── backup-vault.ps1             # Vault backup
│       ├── restore-database.ps1         # DB restore
│       ├── restore-vault.ps1            # Vault restore
│       ├── init-real-wallet.ps1         # Wallet initialization
│       ├── README.md                    # Scripts docs
│       ├── AUTOMATIC-WALLET-INITIALIZATION.md  # Wallet docs (NEW)
│       └── backups/                     # Active backups
│
└── Archive/
    ├── Deployment/                       # Old deployment files
    │   ├── Start/                       # Old scripts
    │   ├── backups/                     # Old backup folders
    │   ├── docker-compose.yml           # Old config
    │   └── *.sh, *.md                   # Old files
    │
    └── vault/                           # Old vault folder
        ├── config/                      # Old vault config
        ├── scripts/                     # Old vault scripts
        └── *.md                         # Old vault docs
```

## Benefits

### ✅ Simplified Structure
- Only essential files in `Deployment/Start/`
- Clear purpose for each file
- No confusion about which scripts to use

### ✅ Single Source of Truth
- One `docker-compose.yml` (root)
- One set of deployment scripts (PowerShell)
- One Vault population script

### ✅ Better Maintainability
- Fewer files to manage
- Clear documentation structure
- Old files preserved in Archive

### ✅ Automatic Operations
- Start: Auto-restore, auto-initialize
- Stop: Auto-backup
- No manual intervention needed

## Usage

### Daily Operations
```powershell
# Start everything
.\Deployment\Start\start-coinpay.ps1

# Stop everything
.\Deployment\Start\stop-coinpay.ps1
```

### Manual Operations
```powershell
# Backup only
.\Deployment\Start\backup-database.ps1
.\Deployment\Start\backup-vault.ps1

# Restore only
.\Deployment\Start\restore-database.ps1
.\Deployment\Start\restore-vault.ps1

# Populate Vault secrets
.\Deployment\populate-dev-secrets.ps1
```

## Path Updates

### Scripts Updated
**File:** `Deployment/Start/start-coinpay.ps1`

**Changed:**
```powershell
# Old path
& ".\vault\scripts\populate-dev-secrets.ps1"

# New path
& ".\Deployment\populate-dev-secrets.ps1"
```

**Why:** Vault folder moved to Archive, script moved to Deployment root.

## Verification

### Check Cleaned Structure
```powershell
# Essential files in Deployment/Start
ls Deployment/Start/

# Should show only: start/stop/backup/restore scripts + README + backups folder
```

### Test Functionality
```powershell
# Full stop-start cycle
.\Deployment\Start\stop-coinpay.ps1
.\Deployment\Start\start-coinpay.ps1

# Verify services running
docker ps

# Test transfer
curl -X POST http://localhost:7777/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 0.05,
    "currency": "POL",
    "type": "Transfer",
    "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58"
  }'
```

## Archive Contents

### What's in Archive
All non-essential files preserved for reference:
- Old bash scripts
- Old documentation
- Old vault configuration
- Old GitHub workflows
- Old test scripts
- Old backup folders

### When to Use Archive
- Reference old implementation
- Restore old scripts if needed
- Review historical documentation
- Compare old vs new approaches

## Summary

✅ **Deployment/Start**: Only essential start/stop/backup/restore scripts
✅ **Deployment**: Main config + Vault population
✅ **docker-compose.yml**: Single source of truth (root)
✅ **Archive**: All old files preserved
✅ **Documentation**: Clear, comprehensive guides
✅ **Automatic**: No manual intervention needed

**Result:** Clean, maintainable, production-ready deployment structure! 🎉
