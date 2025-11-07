#!/bin/sh
set -e

# Wait for Vault to be ready
sleep 2

# Configure Vault CLI
export VAULT_ADDR='http://127.0.0.1:8200'
export VAULT_TOKEN='dev-root-token'

echo "Initializing Vault secrets..."

# Enable KV v2 secrets engine at secret/ (usually enabled by default in dev mode)
vault secrets enable -version=2 -path=secret kv 2>/dev/null || echo "KV secrets engine already enabled"

# Database Configuration
echo "Setting up database secrets..."
vault kv put secret/coinpay/database \
  host="postgres" \
  port="5432" \
  database="coinpay" \
  username="postgres" \
  password="root" \
  connection_string="Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root"

# Redis Configuration
echo "Setting up Redis secrets..."
vault kv put secret/coinpay/redis \
  connection_string="localhost:6379"

# Circle API Configuration
echo "Setting up Circle API secrets..."
vault kv put secret/coinpay/circle \
  api_key="TEST_API_KEY:d93edad9d7011eae471468f01252bafa:8cc4aae56a478a0a313914a062be0445" \
  entity_secret="dc1ff0c795a9701035d45927a8cfc3dd54255f19e1ceebb8e50bafeaf2493d26" \
  webhook_secret="test_webhook_secret_def456ghi012jkl345mno678pqr901stu234" \
  api_base_url="https://api.circle.com/v1/w3s" \
  app_id="0f473fcf-335f-5e52-b1d2-ee9de5d43c9f"

# JWT Configuration
echo "Setting up JWT secrets..."
vault kv put secret/coinpay/jwt \
  secret_key="DevelopmentSecretKey_ChangeInProduction_MinimumLength32Characters" \
  issuer="CoinPay" \
  audience="CoinPay" \
  expiration_minutes="1440" \
  refresh_token_expiration_days="7"

# Gateway Configuration
echo "Setting up Gateway secrets..."
vault kv put secret/coinpay/gateway \
  webhook_secret="dev-webhook-secret-change-in-production"

# Blockchain Test Wallet Configuration
echo "Setting up Blockchain test wallet secrets..."
vault kv put secret/coinpay/blockchain \
  test_wallet_private_key="YOUR_TEST_WALLET_PRIVATE_KEY_HERE"

echo "Vault initialization complete!"
echo "Secrets stored at: secret/coinpay/*"
