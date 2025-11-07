#!/bin/bash
# CoinPay Stop Script
# This script stops all CoinPay services

echo "====================================="
echo "  CoinPay Stop Script"
echo "====================================="
echo ""

# Navigate to project root (two levels up from Deployment/Start)
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$( cd "$SCRIPT_DIR/../.." && pwd )"
cd "$PROJECT_ROOT"

echo "[1/2] Stopping all containers..."
docker-compose down

if [ $? -ne 0 ]; then
    echo "ERROR: Failed to stop containers"
    exit 1
fi

echo "[1/2] Containers stopped"
echo ""

echo "[2/2] Checking container status..."
CONTAINERS=$(docker ps -a --filter "name=coinpay" --format "{{.Names}}")

if [ -z "$CONTAINERS" ]; then
    echo "[2/2] All CoinPay containers removed"
else
    echo "[2/2] Some containers still exist (stopped):"
    docker ps -a --filter "name=coinpay" --format "table {{.Names}}\t{{.Status}}"
fi

echo ""
echo "====================================="
echo "  CoinPay Stopped Successfully!"
echo "====================================="
echo ""
echo "Note:"
echo "  - All containers have been stopped and removed"
echo "  - Data volumes are preserved (postgres-data)"
echo "  - Vault data is lost (dev mode uses in-memory storage)"
echo ""
echo "To restart:"
echo "  ./Deployment/Start/start-coinpay.sh"
echo ""
