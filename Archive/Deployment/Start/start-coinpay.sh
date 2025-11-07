#!/bin/bash
# CoinPay Startup Script with Persistent Vault
# This script starts all services with persistent Vault storage

set -e

echo "====================================="
echo "  CoinPay Startup Script"
echo "  (with Persistent Vault)"
echo "====================================="
echo ""

# Step 1: Start docker-compose services
echo "[1/4] Starting docker-compose services..."
docker-compose up -d

echo "[1/4] Services started"
echo ""

# Step 2: Initialize and Unseal Vault
echo "[2/4] Initializing and unsealing Vault..."
echo ""

chmod +x ./vault/scripts/init-and-unseal.sh
./vault/scripts/init-and-unseal.sh

echo ""
echo "[2/4] Vault initialized and unsealed"
echo ""

# Step 3: Update API configuration with root token
echo "[3/4] Updating API configuration..."

ROOT_TOKEN_FILE="./vault/.root-token"
if [ -f "$ROOT_TOKEN_FILE" ]; then
    ROOT_TOKEN=$(cat "$ROOT_TOKEN_FILE" | tr -d '[:space:]')
    echo "[3/4] Using root token from: $ROOT_TOKEN_FILE"

    # Update appsettings.Development.json with the actual root token
    appsettingsPath="./CoinPay.Api/appsettings.Development.json"
    if [ -f "$appsettingsPath" ]; then
        # Update using jq if available, otherwise use python
        if command -v jq &> /dev/null; then
            jq --arg token "$ROOT_TOKEN" '.Vault.Token = $token' "$appsettingsPath" > "${appsettingsPath}.tmp"
            mv "${appsettingsPath}.tmp" "$appsettingsPath"
        elif command -v python3 &> /dev/null; then
            python3 -c "
import json
with open('$appsettingsPath', 'r') as f:
    config = json.load(f)
config['Vault']['Token'] = '$ROOT_TOKEN'
with open('$appsettingsPath', 'w') as f:
    json.dump(config, f, indent=2)
"
        fi
        echo "[3/4] API configuration updated with root token"
    else
        echo "WARNING: appsettings.Development.json not found"
    fi
else
    echo "[3/4] No root token file found, using default"
fi

echo ""

# Step 4: Restart API to load secrets
echo "[4/4] Restarting API to load secrets..."
docker-compose restart api > /dev/null

echo "[4/4] API restarted"
echo ""

# Wait for API to be ready
echo "Verifying system health..."
sleep 5

retries=0
maxRetries=20

while [ $retries -lt $maxRetries ]; do
    if curl -s http://localhost:7777/health > /dev/null 2>&1; then
        echo "System is healthy!"
        break
    fi
    retries=$((retries + 1))
    if [ $retries -lt $maxRetries ]; then
        echo "  Waiting for API... ($retries/$maxRetries)"
        sleep 1
    fi
done

echo ""
echo "====================================="
echo "  CoinPay Started Successfully!"
echo "====================================="
echo ""
echo "Services:"
echo "  - API:        http://localhost:7777"
echo "  - Swagger:    http://localhost:7777/swagger"
echo "  - Gateway:    http://localhost:5000"
echo "  - Web UI:     http://localhost:3000"
echo "  - Docs:       http://localhost:8080"
echo "  - Vault UI:   http://localhost:8200/ui"
echo ""
echo "Vault:"
echo "  - Status:     Unsealed & Ready"
echo "  - Storage:    Persistent (survives restarts!)"
echo "  - Secrets:    Populated"
echo ""
echo "IMPORTANT:"
echo "  - Vault data is now PERSISTENT"
echo "  - Secrets will survive restarts!"
echo "  - Unseal keys: ./vault/unseal-keys.json"
echo "  - Root token: ./vault/.root-token"
echo "  - Backup these files securely!"
echo ""
echo "Commands:"
echo "  - Stop: docker-compose down"
echo "  - Logs: docker-compose logs -f"
echo "  - Status: docker-compose ps"
echo ""
