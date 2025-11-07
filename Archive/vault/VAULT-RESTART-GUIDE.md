# HashiCorp Vault - Restart & Troubleshooting Guide

## Problem: Vault Secrets Lost on Restart

### Issue Description

When you stop and restart docker-compose, you may encounter the following errors:

1. **Swagger Error**: "Failed to fetch /swagger/v1/swagger.json"
2. **502 Bad Gateway**: API endpoints return 502 errors
3. **API Logs Show**: Vault secret retrieval failures

### Root Cause

Vault is currently running in **dev mode** with in-memory storage. This means:
- All secrets are stored in RAM, not on disk
- When the Vault container restarts, all secrets are lost
- The API fails to start because it cannot load secrets from Vault

This is **expected behavior for development mode** and is documented by HashiCorp.

## Quick Fix: Using the Startup Script

### Windows (PowerShell)

```powershell
.\start-coinpay.ps1
```

### Linux/Mac

```bash
chmod +x start-coinpay.sh
./start-coinpay.sh
```

The startup script will:
1. Start all docker-compose services
2. Wait for Vault to be healthy
3. Re-initialize all secrets
4. Restart the API to load secrets
5. Verify the system is working

## Manual Restart Procedure

If you prefer to do it manually:

### Step 1: Start Services

```bash
docker-compose up -d
```

### Step 2: Wait for Vault

```bash
# Wait about 5-10 seconds for Vault to be healthy
docker exec coinpay-vault vault status
```

### Step 3: Re-Initialize Secrets

**Option A: Using the PowerShell script**
```powershell
.\vault\scripts\init-secrets.ps1
```

**Option B: Using the Bash script**
```bash
./vault/scripts/init-secrets.sh
```

**Option C: Using docker exec**
```bash
docker exec -e VAULT_ADDR='http://127.0.0.1:8200' -e VAULT_TOKEN='dev-root-token' coinpay-vault sh -c "
vault kv put secret/coinpay/database \
  host='postgres' port='5432' database='coinpay' username='postgres' password='root' \
  connection_string='Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root'

vault kv put secret/coinpay/redis connection_string='localhost:6379'

vault kv put secret/coinpay/circle \
  api_key='TEST_API_KEY:d93edad9d7011eae471468f01252bafa:8cc4aae56a478a0a313914a062be0445' \
  entity_secret='dc1ff0c795a9701035d45927a8cfc3dd54255f19e1ceebb8e50bafeaf2493d26' \
  webhook_secret='test_webhook_secret_def456ghi012jkl345mno678pqr901stu234' \
  api_base_url='https://api.circle.com/v1/w3s' \
  app_id='0f473fcf-335f-5e52-b1d2-ee9de5d43c9f'

vault kv put secret/coinpay/jwt \
  secret_key='DevelopmentSecretKey_ChangeInProduction_MinimumLength32Characters' \
  issuer='CoinPay' audience='CoinPay' expiration_minutes='1440' \
  refresh_token_expiration_days='7'

vault kv put secret/coinpay/gateway webhook_secret='dev-webhook-secret-change-in-production'

vault kv put secret/coinpay/blockchain test_wallet_private_key='YOUR_TEST_WALLET_PRIVATE_KEY_HERE'
"
```

### Step 4: Restart API

```bash
docker-compose restart api
```

### Step 5: Verify

```bash
# Check API health
curl http://localhost:7777/health

# Check Swagger
curl http://localhost:7777/swagger/v1/swagger.json

# Check Gateway
curl http://localhost:5000/api/auth/login/dev -H "Content-Type: application/json" -d '{"username":"test"}'
```

## Troubleshooting

### Problem: API Still Not Working

**Check API Logs:**
```bash
docker logs coinpay-vault 2>&1 | tail -50
```

**Look for:**
- "Failed to retrieve secret from" errors
- "Vault connection successful" messages
- "Successfully loaded X configuration values from Vault"

### Problem: Vault Not Healthy

**Check Vault Status:**
```bash
docker exec coinpay-vault vault status
```

**Expected Output:**
```
Initialized    true
Sealed         false
```

**If Sealed = true:**
```bash
# Restart Vault container
docker-compose restart vault
```

### Problem: Secrets Not Persisting

This is **normal in dev mode**. Vault dev mode uses in-memory storage.

**Solution for Production:**
1. Remove `-dev` flag from docker-compose.yml
2. Configure persistent storage backend (Consul, filesystem, etc.)
3. Properly initialize and unseal Vault
4. Use AppRole authentication instead of root token

## Understanding Dev Mode vs Production

### Development Mode (Current)

**Pros:**
- Easy setup, no configuration needed
- Auto-unsealed on startup
- Perfect for local development

**Cons:**
- ❌ Secrets lost on restart
- ❌ Uses root token (insecure)
- ❌ No TLS/encryption
- ❌ Single instance only

### Production Mode (Future)

**Pros:**
- ✅ Persistent storage
- ✅ Proper authentication (AppRole, Kubernetes)
- ✅ TLS encryption
- ✅ High availability support
- ✅ Audit logging

**Cons:**
- Requires more configuration
- Manual unsealing (or auto-unseal with cloud KMS)
- More complex setup

## Production Migration Steps

When you're ready to move to production:

### 1. Update docker-compose.yml

```yaml
vault:
  image: hashicorp/vault:1.15
  command: server  # Remove -dev flag
  volumes:
    - ./vault/config/vault.hcl:/vault/config/vault.hcl
    - vault-data:/vault/data
  environment:
    VAULT_ADDR: https://vault:8200  # Use HTTPS
```

### 2. Create Vault Configuration

Create `vault/config/vault.hcl`:
```hcl
storage "file" {
  path = "/vault/data"
}

listener "tcp" {
  address     = "0.0.0.0:8200"
  tls_disable = 0
  tls_cert_file = "/vault/config/vault.crt"
  tls_key_file = "/vault/config/vault.key"
}

api_addr = "https://vault:8200"
cluster_addr = "https://vault:8201"
ui = true
```

### 3. Initialize Vault

```bash
docker exec -it coinpay-vault vault operator init
```

**Save the unseal keys and root token securely!**

### 4. Unseal Vault

```bash
docker exec -it coinpay-vault vault operator unseal <key1>
docker exec -it coinpay-vault vault operator unseal <key2>
docker exec -it coinpay-vault vault operator unseal <key3>
```

### 5. Set Up AppRole Authentication

```bash
# Enable AppRole
vault auth enable approle

# Create policy
vault policy write coinpay-api - <<EOF
path "secret/data/coinpay/*" {
  capabilities = ["read"]
}
EOF

# Create AppRole
vault write auth/approle/role/coinpay-api \
  token_policies="coinpay-api" \
  token_ttl=1h \
  token_max_ttl=4h

# Get RoleID and SecretID
vault read auth/approle/role/coinpay-api/role-id
vault write -f auth/approle/role/coinpay-api/secret-id
```

### 6. Update API Configuration

Update `appsettings.Production.json`:
```json
{
  "Vault": {
    "Address": "https://vault:8200",
    "AuthMethod": "AppRole",
    "RoleId": "<role-id>",
    "SecretId": "<secret-id>",
    "MountPoint": "secret",
    "BasePath": "coinpay"
  }
}
```

## Best Practices

### Development
1. ✅ Use the startup script (`start-coinpay.ps1` or `start-coinpay.sh`)
2. ✅ Accept that secrets will be lost on restart (this is expected)
3. ✅ Keep sensitive values out of git (use `.env.local` if needed)

### Production
1. ✅ Use persistent storage backend
2. ✅ Enable TLS/HTTPS
3. ✅ Use AppRole or Kubernetes auth
4. ✅ Enable audit logging
5. ✅ Implement secret rotation policies
6. ✅ Set up automated backups
7. ✅ Deploy in HA mode

## FAQ

**Q: Why do I need to re-initialize secrets every time?**
A: Because Vault is running in dev mode with in-memory storage. Secrets are not persisted to disk.

**Q: Can I make secrets persist in dev mode?**
A: No. Dev mode is designed for quick setup and testing. For persistence, use production mode with a storage backend.

**Q: Is this secure for production?**
A: No. Dev mode is NOT suitable for production. It uses a root token, no TLS, and in-memory storage. See "Production Migration Steps" above.

**Q: How do I know if secrets are loaded?**
A: Check the API logs for "Successfully loaded X configuration values from Vault" message.

**Q: What if the startup script fails?**
A: Follow the "Manual Restart Procedure" above and check troubleshooting steps.

## Quick Reference

```bash
# Start everything
.\start-coinpay.ps1  # Windows
./start-coinpay.sh   # Linux/Mac

# Stop everything
docker-compose down

# View logs
docker-compose logs -f api
docker logs coinpay-vault

# Check Vault status
docker exec coinpay-vault vault status

# List secrets
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay

# Get a specific secret
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/database

# Restart just the API
docker-compose restart api
```

## Support

If you continue to have issues:
1. Check this troubleshooting guide
2. Review the API logs: `docker logs coinpay-api`
3. Review the Vault logs: `docker logs coinpay-vault`
4. Ensure Vault is healthy: `docker exec coinpay-vault vault status`
5. Verify secrets exist: `docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay`

---

**Remember**: Dev mode is for development only. Plan for production Vault deployment before going live!
