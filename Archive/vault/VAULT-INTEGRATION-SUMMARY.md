# HashiCorp Vault Integration - Summary & Quick Start

## ✅ Integration Complete

HashiCorp Vault has been successfully integrated into CoinPay for secure configuration management.

## 🚀 Quick Start

### Starting CoinPay (Recommended Method)

**Windows:**
```powershell
.\start-coinpay.ps1
```

**Linux/Mac:**
```bash
chmod +x start-coinpay.sh
./start-coinpay.sh
```

This script will:
1. Start all services
2. Initialize Vault secrets
3. Restart API to load secrets
4. Verify system health

### Stopping CoinPay

```bash
docker-compose down
```

## ⚠️ Important: Vault Dev Mode Behavior

**Vault is running in development mode with in-memory storage.**

This means:
- ✅ Easy setup, perfect for development
- ❌ **Secrets are lost when Vault restarts**
- ❌ Root token used (not secure for production)
- ❌ No TLS encryption

**Every time you restart docker-compose, you MUST re-initialize Vault secrets.**

Use the `start-coinpay` script to make this easy!

## 📝 What Was Integrated

### Services
- **Vault Container**: HashiCorp Vault 1.15 running on port 8200
- **VaultSharp**: .NET client library v1.17.5.1
- **VaultService**: Custom service for loading secrets

### Secrets Managed
All sensitive configuration moved to Vault:
- Database credentials (PostgreSQL)
- Redis connection string
- Circle API keys and secrets
- JWT signing keys
- Gateway webhook secrets
- Blockchain test wallet private keys

### Features
- ✅ Automatic secret loading at startup
- ✅ Retry logic (3 attempts with 1s delay)
- ✅ Comprehensive error logging
- ✅ Health check dependencies
- ✅ Fail-safe startup (won't start without Vault)

## 🔧 Troubleshooting

### Problem: 502 Bad Gateway / Swagger Not Loading

**Cause**: Vault secrets lost after restart

**Solution**: Run the startup script
```powershell
.\start-coinpay.ps1  # Windows
./start-coinpay.sh   # Linux/Mac
```

**OR manually:**
1. Re-initialize secrets: `.\vault\scripts\init-secrets.ps1`
2. Restart API: `docker-compose restart api`

### Problem: API Won't Start

**Check logs:**
```bash
docker logs coinpay-api 2>&1 | tail -50
```

**Look for** "Failed to retrieve secret from Vault"

**Solution:**
1. Verify Vault is running: `docker ps | grep vault`
2. Check Vault health: `docker exec coinpay-vault vault status`
3. Re-initialize secrets (see above)

### Problem: Vault Container Not Starting

**Check container:**
```bash
docker ps -a | grep vault
docker logs coinpay-vault
```

**Solution:**
```bash
docker-compose restart vault
```

## 📁 Key Files

| File | Purpose |
|------|---------|
| `start-coinpay.ps1` | **START HERE** - PowerShell startup script |
| `start-coinpay.sh` | **START HERE** - Bash startup script |
| `VAULT-RESTART-GUIDE.md` | Detailed restart & troubleshooting guide |
| `VAULT-INTEGRATION-TEST-RESULTS.md` | Full integration test results |
| `vault/README.md` | Complete Vault documentation |
| `vault/scripts/init-secrets.ps1` | PowerShell secret initialization |
| `vault/scripts/init-secrets.sh` | Bash secret initialization |

## 🌐 Service URLs

After starting with the startup script:

| Service | URL | Description |
|---------|-----|-------------|
| API | http://localhost:7777 | Backend API |
| Swagger | http://localhost:7777/swagger | API documentation |
| Health Check | http://localhost:7777/health | API health status |
| Gateway | http://localhost:5000 | API Gateway |
| Web UI | http://localhost:3000 | Frontend application |
| Docs | http://localhost:8080 | DocFX documentation |
| Vault UI | http://localhost:8200/ui | Vault web interface |

### Vault Access

- **Address**: http://localhost:8200
- **Token**: `dev-root-token`
- **UI**: http://localhost:8200/ui (login with token)

## 📚 Documentation

### For Daily Use
1. **VAULT-INTEGRATION-SUMMARY.md** (this file) - Quick reference
2. **start-coinpay.ps1/.sh** - Startup script

### For Troubleshooting
3. **VAULT-RESTART-GUIDE.md** - Restart procedures & troubleshooting
4. **vault/README.md** - Complete Vault documentation

### For Reference
5. **VAULT-INTEGRATION-TEST-RESULTS.md** - Test results & technical details

## 🔐 Security Notes

### Development (Current Setup)
- Uses dev mode Vault (in-memory storage)
- Root token authentication
- No TLS/HTTPS
- Secrets in docker-compose files for easy setup
- ✅ **Suitable for local development only**

### Production (Future)
Before deploying to production, you MUST:
1. Use persistent storage backend (Consul, filesystem, etc.)
2. Enable TLS/HTTPS
3. Use AppRole or Kubernetes authentication
4. Remove dev mode flag
5. Enable audit logging
6. Implement secret rotation
7. Deploy in HA mode

See `VAULT-RESTART-GUIDE.md` section "Production Migration Steps" for details.

## 🎯 Common Commands

```bash
# Start everything (recommended)
.\start-coinpay.ps1  # Windows
./start-coinpay.sh   # Linux/Mac

# Stop everything
docker-compose down

# View all logs
docker-compose logs -f

# View API logs only
docker logs -f coinpay-api

# View Vault logs
docker logs -f coinpay-vault

# Check system status
docker-compose ps

# Check API health
curl http://localhost:7777/health

# List Vault secrets
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay

# View specific secret
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/database

# Restart API only (after re-initializing secrets)
docker-compose restart api

# Restart Vault only
docker-compose restart vault
```

## ✨ Benefits

1. **Centralized Secrets**: All sensitive config in one secure place
2. **No Hardcoded Secrets**: Config files are now safe to commit
3. **Access Control**: Vault tokens control who can access secrets
4. **Audit Ready**: Can enable audit logging for compliance
5. **Rotation Ready**: Secrets can be rotated without code changes
6. **Production Path**: Clear upgrade path to production-grade setup

## 🎓 Learning Resources

- [HashiCorp Vault Documentation](https://www.vaultproject.io/docs)
- [Vault KV Secrets Engine](https://www.vaultproject.io/docs/secrets/kv/kv-v2)
- [VaultSharp .NET Client](https://github.com/rajanadar/VaultSharp)
- [Vault Production Hardening](https://learn.hashicorp.com/tutorials/vault/production-hardening)

## 💡 Tips

1. **Always use the startup script** - It handles everything automatically
2. **Don't commit Vault tokens** - They're for development only
3. **Check logs if issues occur** - Most problems show clear error messages
4. **Vault health check** - Ensure Vault is healthy before debugging API
5. **Dev mode is temporary** - Plan for production Vault deployment

## 📞 Support

If you encounter issues:
1. Try the startup script: `.\start-coinpay.ps1`
2. Check `VAULT-RESTART-GUIDE.md` for troubleshooting
3. Review logs: `docker logs coinpay-api` and `docker logs coinpay-vault`
4. Verify Vault status: `docker exec coinpay-vault vault status`
5. Check if secrets exist: `docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay`

---

**Quick Reminder**: Use `.\start-coinpay.ps1` (Windows) or `./start-coinpay.sh` (Linux/Mac) to start the system!

This ensures Vault secrets are properly initialized every time.
