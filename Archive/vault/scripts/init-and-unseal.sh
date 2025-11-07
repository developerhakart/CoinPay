#!/bin/bash
# Vault Initialization and Auto-Unseal Script
# This script initializes Vault (if needed), unseals it, and populates secrets

set -e

VAULT_ADDR="http://localhost:8200"
KEYS_FILE="./vault/unseal-keys.json"
ROOT_TOKEN_FILE="./vault/.root-token"

echo "========================================"
echo "  Vault Auto-Initialization & Unseal"
echo "========================================"
echo ""

# Step 1: Wait for Vault to be available
echo "[1/5] Waiting for Vault to be available..."
retries=0
maxRetries=30

while [ $retries -lt $maxRetries ]; do
    if curl -s "$VAULT_ADDR/v1/sys/health" > /dev/null 2>&1; then
        break
    fi
    retries=$((retries + 1))
    if [ $retries -lt $maxRetries ]; then
        echo "  Waiting for Vault... ($retries/$maxRetries)"
        sleep 1
    fi
done

if [ $retries -ge $maxRetries ]; then
    echo "ERROR: Vault is not accessible"
    exit 1
fi

echo "[1/5] Vault is accessible"
echo ""

# Step 2: Check if Vault is initialized
echo "[2/5] Checking Vault initialization status..."

init_status=$(docker exec coinpay-vault vault status -format=json 2>/dev/null | jq -r '.initialized')

if [ "$init_status" != "true" ]; then
    echo "[2/5] Vault is not initialized. Initializing now..."

    # Initialize Vault
    init_output=$(docker exec coinpay-vault vault operator init -key-shares=3 -key-threshold=2 -format=json)

    # Save unseal keys and root token
    echo "$init_output" > "$KEYS_FILE"
    echo "$init_output" | jq -r '.root_token' > "$ROOT_TOKEN_FILE"

    echo "[2/5] Vault initialized successfully!"
    echo "      Unseal keys saved to: $KEYS_FILE"
    echo "      Root token saved to: $ROOT_TOKEN_FILE"
    echo "      IMPORTANT: Backup these files securely!"
else
    echo "[2/5] Vault is already initialized"
fi

echo ""

# Step 3: Unseal Vault if sealed
echo "[3/5] Checking Vault seal status..."

sealed=$(docker exec coinpay-vault vault status -format=json 2>/dev/null | jq -r '.sealed')

if [ "$sealed" = "true" ]; then
    echo "[3/5] Vault is sealed. Unsealing..."

    if [ ! -f "$KEYS_FILE" ]; then
        echo "ERROR: Unseal keys file not found: $KEYS_FILE"
        echo "Cannot unseal Vault without keys."
        exit 1
    fi

    # Unseal with threshold number of keys (2 out of 3)
    key1=$(jq -r '.unseal_keys_b64[0]' "$KEYS_FILE")
    key2=$(jq -r '.unseal_keys_b64[1]' "$KEYS_FILE")

    docker exec coinpay-vault vault operator unseal "$key1" > /dev/null
    docker exec coinpay-vault vault operator unseal "$key2" > /dev/null

    echo "[3/5] Vault unsealed successfully!"
else
    echo "[3/5] Vault is already unsealed"
fi

echo ""

# Step 4: Get root token
echo "[4/5] Retrieving root token..."

if [ -f "$ROOT_TOKEN_FILE" ]; then
    ROOT_TOKEN=$(cat "$ROOT_TOKEN_FILE" | tr -d '[:space:]')
    echo "[4/5] Root token loaded from file"
elif [ -f "$KEYS_FILE" ]; then
    ROOT_TOKEN=$(jq -r '.root_token' "$KEYS_FILE")
    echo "$ROOT_TOKEN" > "$ROOT_TOKEN_FILE"
    echo "[4/5] Root token extracted from keys file"
else
    echo "WARNING: Cannot find root token. Using fallback."
    ROOT_TOKEN="dev-root-token"
fi

echo ""

# Step 5: Populate secrets (if they don't exist)
echo "[5/5] Populating secrets..."

# Check if secrets already exist
if docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv list secret/coinpay 2>/dev/null | grep -q "database"; then
    echo "[5/5] Secrets already exist. Skipping population."
else
    echo "[5/5] Secrets not found. Populating now..."

    # Enable KV v2 secrets engine
    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault secrets enable -version=2 -path=secret kv 2>/dev/null || true

    # Populate all secrets
    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv put secret/coinpay/database \
      host='postgres' port='5432' database='coinpay' username='postgres' password='root' \
      connection_string='Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root' > /dev/null

    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv put secret/coinpay/redis \
      connection_string='localhost:6379' > /dev/null

    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv put secret/coinpay/circle \
      api_key='TEST_API_KEY:d93edad9d7011eae471468f01252bafa:8cc4aae56a478a0a313914a062be0445' \
      entity_secret='dc1ff0c795a9701035d45927a8cfc3dd54255f19e1ceebb8e50bafeaf2493d26' \
      webhook_secret='test_webhook_secret_def456ghi012jkl345mno678pqr901stu234' \
      api_base_url='https://api.circle.com/v1/w3s' \
      app_id='0f473fcf-335f-5e52-b1d2-ee9de5d43c9f' > /dev/null

    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv put secret/coinpay/jwt \
      secret_key='DevelopmentSecretKey_ChangeInProduction_MinimumLength32Characters' \
      issuer='CoinPay' audience='CoinPay' expiration_minutes='1440' \
      refresh_token_expiration_days='7' > /dev/null

    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv put secret/coinpay/gateway \
      webhook_secret='dev-webhook-secret-change-in-production' > /dev/null

    docker exec -e VAULT_TOKEN="$ROOT_TOKEN" coinpay-vault vault kv put secret/coinpay/blockchain \
      test_wallet_private_key='YOUR_TEST_WALLET_PRIVATE_KEY_HERE' > /dev/null

    echo "[5/5] All secrets populated successfully!"
fi

echo ""
echo "========================================"
echo "  Vault Ready!"
echo "========================================"
echo ""
echo "Status:"
echo "  - Initialized: Yes"
echo "  - Sealed: No"
echo "  - Secrets: Populated"
echo "  - Root Token: $ROOT_TOKEN"
echo ""
echo "IMPORTANT NOTES:"
echo "  - Unseal keys are saved in: $KEYS_FILE"
echo "  - Root token is saved in: $ROOT_TOKEN_FILE"
echo "  - DO NOT commit these files to git!"
echo "  - Backup these files securely!"
echo ""
