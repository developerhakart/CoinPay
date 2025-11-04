# HashiCorp Vault Integration

This directory contains configuration and scripts for HashiCorp Vault integration with CoinPay.

## Overview

CoinPay uses HashiCorp Vault to securely manage sensitive configuration values such as:
- Database credentials
- API keys (Circle, JWT secrets)
- Encryption keys
- Blockchain private keys
- Redis connection strings

## Directory Structure

```
vault/
├── .gitignore                    # Prevents committing sensitive files
├── README.md                     # This file
├── config/
│   └── vault.hcl                 # Vault server configuration
├── data/                         # Persistent storage (not used in dev mode)
├── logs/                         # Vault logs
└── scripts/
    ├── populate-dev-secrets.ps1  # Populate secrets in dev mode (PowerShell)
    ├── init-and-unseal.ps1      # Production initialization (PowerShell)
    └── init-and-unseal.sh       # Production initialization (Bash)
```

## Quick Start

The easiest way to start CoinPay with Vault is using the provided startup script:

### Windows (PowerShell)
```powershell
.\start-coinpay.ps1
```

### Linux/Mac (Bash)
```bash
chmod +x start-coinpay.sh
./start-coinpay.sh
```

The script will:
1. Start all docker-compose services
2. Wait for Vault to be available
3. Populate Vault with development secrets
4. Restart the API to load secrets
5. Verify system health

### Manual Start (Advanced)

If you prefer to start services manually:

1. **Start All Services**:
   ```bash
   docker-compose up -d
   ```

2. **Populate Vault Secrets**:
   ```powershell
   # Windows
   .\vault\scripts\populate-dev-secrets.ps1

   # Linux/Mac
   chmod +x vault/scripts/populate-dev-secrets.sh
   ./vault/scripts/populate-dev-secrets.sh
   ```

3. **Restart API** (to load secrets):
   ```bash
   docker-compose restart api
   ```

## Configuration

### Vault Options (appsettings.Development.json)

```json
{
  "Vault": {
    "Address": "http://localhost:8200",
    "Token": "dev-root-token",
    "MountPoint": "secret",
    "BasePath": "coinpay",
    "TimeoutSeconds": 30,
    "RetryAttempts": 3,
    "RetryDelayMs": 1000
  }
}
```

### Docker Compose Configuration

```yaml
vault:
  image: hashicorp/vault:1.15
  container_name: coinpay-vault
  ports:
    - "8200:8200"
  environment:
    VAULT_DEV_ROOT_TOKEN_ID: dev-root-token
    VAULT_DEV_LISTEN_ADDRESS: 0.0.0.0:8200
```

## Secret Paths

All secrets are stored under the `secret/coinpay/` path:

| Path | Description | Fields |
|------|-------------|--------|
| `secret/coinpay/database` | PostgreSQL credentials | `host`, `port`, `database`, `username`, `password`, `connection_string` |
| `secret/coinpay/redis` | Redis connection | `connection_string` |
| `secret/coinpay/circle` | Circle API credentials | `api_key`, `entity_secret`, `webhook_secret`, `api_base_url`, `app_id` |
| `secret/coinpay/jwt` | JWT configuration | `secret_key`, `issuer`, `audience`, `expiration_minutes`, `refresh_token_expiration_days` |
| `secret/coinpay/gateway` | Gateway configuration | `webhook_secret` |
| `secret/coinpay/blockchain` | Blockchain test wallet | `test_wallet_private_key` |

## Managing Secrets

### View All Secrets

```bash
# List all secret paths
vault kv list secret/coinpay

# Get a specific secret
vault kv get secret/coinpay/database

# Get a specific field
vault kv get -field=password secret/coinpay/database
```

### Update a Secret

```bash
# Update a single field
vault kv patch secret/coinpay/database password=newpassword

# Update entire secret
vault kv put secret/coinpay/jwt \
  secret_key="NewSecretKey123" \
  issuer="CoinPay" \
  audience="CoinPay"
```

### Add a New Secret

```bash
vault kv put secret/coinpay/newsecret \
  key1="value1" \
  key2="value2"
```

## Application Integration

The CoinPay API loads secrets from Vault on startup via:

```csharp
// In Program.cs
await builder.LoadSecretsFromVaultAsync();
builder.Services.AddVaultConfiguration(builder.Configuration);
```

### How It Works

1. **Startup**: Application connects to Vault using configured address and token
2. **Health Check**: Verifies Vault is accessible and unsealed
3. **Secret Loading**: Retrieves all secrets from defined paths
4. **Configuration Mapping**: Maps Vault secrets to .NET configuration structure
5. **Service Registration**: Registers `IVaultService` for runtime secret access

### Error Handling

If Vault is unavailable on startup, the application will:
1. Log detailed error messages with troubleshooting steps
2. Throw `InvalidOperationException` to prevent startup
3. Display health check URL and initialization commands

Example error log:
```
[ERROR] CRITICAL: Failed to connect to Vault at http://vault:8200
[ERROR] Please ensure Vault is running: docker-compose up vault
[ERROR] Vault health check endpoint: http://vault:8200/v1/sys/health
```

### Logging

Vault operations are logged with different levels:

- **Information**: Successful connections and secret loading
- **Warning**: Retries, missing fields, empty secrets
- **Error**: Connection failures, secret retrieval errors
- **Debug**: Individual secret field retrievals

## Development vs Production

### Development Mode (Current)

- Uses `-dev` flag for easy setup
- Root token: `dev-root-token`
- Data stored in memory (not persisted)
- Auto-unsealed on startup
- Suitable for local development

### Production Mode

For production deployment:

1. **Remove `-dev` flag** from docker-compose.yml
2. **Configure persistent storage**:
   ```yaml
   volumes:
     - vault-data:/vault/data
   ```
3. **Use proper initialization**:
   ```bash
   vault operator init
   ```
4. **Store unseal keys securely**
5. **Use AppRole or Kubernetes auth** instead of root token
6. **Enable TLS/HTTPS**
7. **Configure backup and recovery**

## Troubleshooting

### Vault Container Won't Start

```bash
# Check container logs
docker logs coinpay-vault

# Verify port not in use
netstat -an | findstr :8200  # Windows
lsof -i :8200                # Linux/Mac
```

### Application Can't Connect to Vault

```bash
# Test Vault health
curl http://localhost:8200/v1/sys/health

# Check Vault status
docker exec coinpay-vault vault status

# Verify environment variables
docker exec coinpay-vault env | grep VAULT
```

### Secrets Not Loading

```bash
# Verify secrets exist
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv list secret/coinpay

# Re-populate secrets
.\vault\scripts\populate-dev-secrets.ps1  # Windows
./vault/scripts/populate-dev-secrets.sh   # Linux/Mac

# Restart API to reload secrets
docker-compose restart api

# Check API logs for Vault errors
docker logs coinpay-api | findstr Vault  # Windows
docker logs coinpay-api | grep Vault     # Linux/Mac
```

### Permission Denied Errors

```bash
# Verify token is correct
docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault token lookup

# Check KV engine is enabled
docker exec coinpay-vault vault secrets list
```

## Security Best Practices

1. **Never commit Vault tokens to git**
2. **Use environment-specific tokens** (dev, staging, prod)
3. **Rotate secrets regularly**
4. **Enable audit logging in production**
5. **Use least-privilege policies**
6. **Monitor Vault access logs**
7. **Backup Vault data regularly**
8. **Use TLS in production**

## Useful Commands

```bash
# Start Vault only
docker-compose up vault

# Stop Vault
docker-compose stop vault

# Restart Vault
docker-compose restart vault

# View Vault logs
docker logs -f coinpay-vault

# Access Vault CLI
docker exec -it coinpay-vault sh

# Inside container
export VAULT_ADDR='http://127.0.0.1:8200'
export VAULT_TOKEN='dev-root-token'
vault status
vault kv list secret/coinpay
```

## Further Reading

- [HashiCorp Vault Documentation](https://www.vaultproject.io/docs)
- [Vault KV Secrets Engine](https://www.vaultproject.io/docs/secrets/kv/kv-v2)
- [VaultSharp .NET Client](https://github.com/rajanadar/VaultSharp)
- [Docker Vault Image](https://hub.docker.com/_/vault)

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review application logs: `docker logs coinpay-api`
3. Review Vault logs: `docker logs coinpay-vault`
4. Consult HashiCorp Vault documentation
