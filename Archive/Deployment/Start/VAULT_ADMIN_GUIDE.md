# Vault Configuration Admin Guide

## Overview

All CoinPay API configuration is now centralized in HashiCorp Vault. This allows administrators to change settings (including mock modes for external APIs) without rebuilding Docker containers - just restart the API service.

## Quick Reference

**Vault Access:**
- Address: http://localhost:8200/ui
- Root Token: `dev-root-token`
- Secret Mount: `secret/coinpay/`

**Available Configuration Paths:**
1. `secret/coinpay/circle` - Circle API configuration
2. `secret/coinpay/whitebit` - WhiteBit Exchange configuration
3. `secret/coinpay/oneinch` - 1inch DEX Aggregator configuration
4. `secret/coinpay/database` - PostgreSQL connection
5. `secret/coinpay/redis` - Redis connection
6. `secret/coinpay/jwt` - JWT authentication settings
7. `secret/coinpay/gateway` - API Gateway configuration
8. `secret/coinpay/blockchain` - Blockchain wallet settings

---

## Switching Between Mock Mode and Real Mode

### Using Vault UI (Recommended)

1. **Access Vault UI:**
   ```
   http://localhost:8200/ui
   ```

2. **Login:**
   - Method: Token
   - Token: `dev-root-token`

3. **Navigate to Secret:**
   - Click "secret/" → "coinpay/" → Choose service (e.g., "circle")

4. **Edit Secret:**
   - Click "Create new version" button
   - Modify `use_mock_mode` value:
     - `true` = Mock mode (development/testing)
     - `false` = Real mode (production with real APIs)
   - Click "Save"

5. **Restart API:**
   ```powershell
   docker-compose restart api
   ```

6. **Verify:**
   ```powershell
   docker logs coinpay-api --tail 50 | grep "UseMockMode"
   ```

### Using Vault CLI

#### Enable Mock Mode (Development):

```bash
# Circle API
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/circle \
  api_key='TEST_API_KEY:...' \
  entity_secret='...' \
  webhook_secret='...' \
  api_base_url='https://api.circle.com/v1/w3s' \
  app_id='...' \
  use_mock_mode='true'

# WhiteBit Exchange
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/whitebit \
  api_url='https://whitebit.com/api/v4' \
  base_url='https://whitebit.com' \
  use_mock_mode='true'

# OneInch DEX
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/oneinch \
  api_url='https://api.1inch.dev/swap/v6.0' \
  api_key='YOUR_1INCH_API_KEY_HERE' \
  use_mock_mode='true'

# Restart API
docker-compose restart api
```

#### Disable Mock Mode (Production):

```bash
# Circle API - IMPORTANT: Replace with REAL credentials!
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/circle \
  api_key='REAL_API_KEY:your_key_id:your_secret' \
  entity_secret='your_real_entity_secret' \
  webhook_secret='your_webhook_secret' \
  api_base_url='https://api.circle.com/v1/w3s' \
  app_id='your_real_app_id' \
  use_mock_mode='false'

# WhiteBit Exchange
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/whitebit \
  api_url='https://whitebit.com/api/v4' \
  base_url='https://whitebit.com' \
  use_mock_mode='false'

# OneInch DEX - IMPORTANT: Add real API key!
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/oneinch \
  api_url='https://api.1inch.dev/swap/v6.0' \
  api_key='YOUR_REAL_1INCH_API_KEY' \
  use_mock_mode='false'

# Restart API
docker-compose restart api
```

---

## Complete Configuration Reference

### Circle API Configuration

**Path:** `secret/coinpay/circle`

**Fields:**
- `api_key` (string) - Circle API key (format: `ENVIRONMENT:KEY_ID:SECRET`)
- `entity_secret` (string) - Circle entity secret for encryption
- `webhook_secret` (string) - Webhook signature verification secret
- `api_base_url` (string) - Circle API base URL
- `app_id` (string) - Circle application ID
- `use_mock_mode` (string) - `"true"` or `"false"`

**Example (Mock):**
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/circle \
  api_key='TEST_API_KEY:abc123:xyz789' \
  entity_secret='test_entity_secret' \
  webhook_secret='test_webhook_secret' \
  api_base_url='https://api.circle.com/v1/w3s' \
  app_id='test-app-id' \
  use_mock_mode='true'
```

---

### WhiteBit Exchange Configuration

**Path:** `secret/coinpay/whitebit`

**Fields:**
- `api_url` (string) - WhiteBit API base URL
- `base_url` (string) - WhiteBit website base URL
- `use_mock_mode` (string) - `"true"` or `"false"`

**Example (Mock):**
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/whitebit \
  api_url='https://whitebit.com/api/v4' \
  base_url='https://whitebit.com' \
  use_mock_mode='true'
```

---

### OneInch DEX Configuration

**Path:** `secret/coinpay/oneinch`

**Fields:**
- `api_url` (string) - 1inch API base URL
- `api_key` (string) - 1inch API key
- `use_mock_mode` (string) - `"true"` or `"false"`

**Example (Mock):**
```bash
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv put secret/coinpay/oneinch \
  api_url='https://api.1inch.dev/swap/v6.0' \
  api_key='placeholder_key' \
  use_mock_mode='true'
```

---

## Verification Commands

### Check Current Configuration:

```bash
# View Circle config
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/circle

# View WhiteBit config
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/whitebit

# View OneInch config
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/oneinch

# List all secret paths
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay/
```

### Check API Logs After Restart:

```bash
# Check if mock mode is loaded correctly
docker logs coinpay-api --tail 50 | grep "UseMockMode"

# Should show:
# Circle API configuration loaded from Vault (UseMockMode: true)
# WhiteBit configuration loaded from Vault (UseMockMode: true)
# OneInch configuration loaded from Vault (UseMockMode: true)
```

### Test API Health:

```bash
# Check API health
curl http://localhost:7777/health

# Check Swagger documentation
# http://localhost:7777/swagger
```

---

## Common Operations

### Repopulate All Secrets (Development Defaults):

```powershell
# Run the populate script
.\vault\scripts\populate-dev-secrets.ps1

# Restart API to load new secrets
docker-compose restart api
```

### Backup Current Configuration:

```bash
# Export all secrets to JSON files
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get -format=json secret/coinpay/circle > circle-backup.json
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get -format=json secret/coinpay/whitebit > whitebit-backup.json
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get -format=json secret/coinpay/oneinch > oneinch-backup.json
```

### View Secret History:

```bash
# View all versions of a secret
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv metadata secret/coinpay/circle

# Read a specific version
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get -version=2 secret/coinpay/circle
```

---

## Troubleshooting

### Issue: API Can't Connect to Vault

**Symptoms:**
```
CRITICAL: Failed to connect to Vault
```

**Solution:**
1. Check Vault is running:
   ```bash
   docker ps | grep vault
   ```

2. Check Vault health:
   ```bash
   curl http://localhost:8200/v1/sys/health
   ```

3. Restart Vault:
   ```bash
   docker-compose restart vault
   ```

### Issue: Secrets Not Found

**Symptoms:**
```
no handler for route "coinpay/data/circle". route entry not found.
```

**Solution:**
1. Repopulate secrets:
   ```powershell
   .\vault\scripts\populate-dev-secrets.ps1
   ```

2. Verify secrets exist:
   ```bash
   docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay/
   ```

### Issue: Still Using Real API After Enabling Mock Mode

**Symptoms:**
```
Circle API request failed with status Unauthorized
```

**Solution:**
1. Verify mock mode is set in Vault:
   ```bash
   docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/circle | grep use_mock_mode
   ```

2. **Rebuild the API container** (required if changing appsettings.json):
   ```bash
   docker-compose build api
   docker-compose up -d api
   ```

3. Check API logs to confirm:
   ```bash
   docker logs coinpay-api --tail 20 | grep "UseMockMode"
   ```

---

## Production Deployment Notes

**IMPORTANT:** Before deploying to production:

1. **Obtain Real API Credentials:**
   - Circle: https://console.circle.com
   - WhiteBit: User-provided (per-user API keys)
   - 1inch: https://portal.1inch.dev

2. **Update Vault Secrets with Real Credentials**

3. **Set Mock Mode to False:**
   ```bash
   use_mock_mode='false'
   ```

4. **Use Production Vault Instance:**
   - NOT dev mode (in-memory)
   - Proper authentication (AppRole, Kubernetes, etc.)
   - Sealed storage with encryption

5. **Restart API Service:**
   ```bash
   docker-compose restart api
   ```

6. **Monitor Logs for Errors:**
   ```bash
   docker logs -f coinpay-api
   ```

---

## Security Best Practices

1. **Never Commit Vault Tokens to Git**
2. **Use Strong Tokens in Production** (not `dev-root-token`)
3. **Enable Vault Audit Logging**
4. **Rotate API Keys Regularly**
5. **Use Vault Policies for Access Control**
6. **Back Up Vault Data** (production only)
7. **Monitor Vault Access Logs**

---

## Quick Commands Cheat Sheet

```bash
# View all secrets
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay/

# Get specific secret
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/circle

# Update single field (patches existing secret)
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv patch secret/coinpay/circle use_mock_mode='false'

# Restart API
docker-compose restart api

# Check API logs
docker logs coinpay-api --tail 50

# Check mock mode status
docker logs coinpay-api --tail 50 | grep "UseMockMode"
```

---

## Support

For issues or questions:
- Check API logs: `docker logs coinpay-api`
- Check Vault logs: `docker logs coinpay-vault`
- Review this guide
- Check `vault/scripts/populate-dev-secrets.ps1` for default values
