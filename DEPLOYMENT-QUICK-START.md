# CoinPay Deployment - Quick Start

## ⚡ Quick Commands

### Start CoinPay
```powershell
.\Deployment\Start\start-coinpay.ps1
```

### Stop CoinPay
```powershell
.\Deployment\Start\stop-coinpay.ps1
```

## 📦 What You Get

- ✅ **Automatic Backup** - Database & Vault backed up on stop
- ✅ **Automatic Restore** - Everything restored on start
- ✅ **Real Circle API** - Production-ready wallet integration
- ✅ **No Manual Setup** - Circle Wallet IDs configured automatically

## 🌐 Access Services

After starting, access:

| Service | URL |
|---------|-----|
| **Web UI** | http://localhost:3000 |
| **API** | http://localhost:7777 |
| **Swagger** | http://localhost:7777/swagger |
| **Gateway** | http://localhost:5000 |

## 📚 Documentation

- **Complete Guide:** `Deployment/DEPLOYMENT-GUIDE.md`
- **Cleanup Details:** `Deployment/CLEANUP-SUMMARY.md`
- **Wallet Management:** `Deployment/Start/AUTOMATIC-WALLET-INITIALIZATION.md`
- **Scripts Reference:** `Deployment/Start/README.md`

## 🔧 Troubleshooting

### Check Logs
```powershell
docker-compose logs -f
```

### Check Status
```powershell
docker-compose ps
```

### Manual Backup
```powershell
.\Deployment\Start\backup-database.ps1
.\Deployment\Start\backup-vault.ps1
```

### Manual Restore
```powershell
.\Deployment\Start\restore-database.ps1
.\Deployment\Start\restore-vault.ps1
```

## 💡 Key Features

### Automatic Circle Wallet Management
Your real Circle Wallet credentials are automatically configured:
- **Sender:** `fef70777-cb2d-5096-a0ea-15dba5662ce6`
- **Address:** `0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579`

No manual updates needed across stop/start cycles! ✅

### Automatic Backup/Restore
- **Stop:** Creates backups before stopping
- **Start:** Restores from latest backup if needed
- **Backups:** Stored in `Deployment/Start/backups/`

### Production Ready
- Real Circle API integration (testnet)
- Secure Vault secret management
- Persistent data across restarts
- Health checks and monitoring

## 🏗️ Clean Structure

```
CoinPay/
├── docker-compose.yml              # Main config
├── Deployment/
│   ├── docker-compose.yml         # Config copy
│   ├── populate-dev-secrets.ps1   # Vault setup
│   └── Start/                     # Deployment scripts
│       ├── start-coinpay.ps1     # ⭐ START HERE
│       ├── stop-coinpay.ps1      # ⭐ STOP HERE
│       ├── backup-*.ps1          # Backup scripts
│       ├── restore-*.ps1         # Restore scripts
│       └── backups/              # Backup storage
└── Archive/                       # Old files (preserved)
```

## 🎯 Common Tasks

### Full Restart
```powershell
.\Deployment\Start\stop-coinpay.ps1
.\Deployment\Start\start-coinpay.ps1
```

### Test Transfer (POL)
```bash
curl -X POST http://localhost:7777/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 0.05,
    "currency": "POL",
    "type": "Transfer",
    "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58"
  }'
```

### Switch Circle Mode
```powershell
# To Mock Mode
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='true'
docker-compose restart api

# To Real Mode
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='false'
docker-compose restart api
```

## ⚠️ Important Notes

### Current Configuration
- ✅ Circle API: **Real mode** (testnet)
- ✅ Wallet Balance: Check Circle console
- ✅ Auto-backup: Enabled
- ✅ Auto-restore: Enabled

### Vault (Development Mode)
- Mode: In-memory (dev mode)
- Token: `dev-root-token`
- Secrets: Auto-populated on start
- Persistence: Via backup/restore

### Test User
- Username: `testuser`
- Wallet: `0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579`
- Circle ID: `fef70777-cb2d-5096-a0ea-15dba5662ce6`

## 🚀 Next Steps

1. ✅ Start CoinPay: `.\Deployment\Start\start-coinpay.ps1`
2. ✅ Access Web UI: http://localhost:3000
3. ✅ Test transfers via UI or API
4. ✅ Stop when done: `.\Deployment\Start\stop-coinpay.ps1`

**Everything is automatic - just start and go!** 🎉
