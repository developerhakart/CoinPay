# Populate Vault with development secrets (Dev Mode)
# This script populates a Vault dev instance with all required secrets

$ErrorActionPreference = "Stop"

$VAULT_ADDR = "http://localhost:8200"
$VAULT_TOKEN = "dev-root-token"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Populating Vault Dev Secrets" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Wait for Vault to be ready
Write-Host "[1/2] Waiting for Vault to be available..." -ForegroundColor Yellow
$retries = 0
$maxRetries = 30

while ($retries -lt $maxRetries) {
    try {
        $response = Invoke-WebRequest -Uri "$VAULT_ADDR/v1/sys/health" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            break
        }
    } catch {
        $retries++
        if ($retries -lt $maxRetries) {
            Write-Host "  Waiting for Vault... ($retries/$maxRetries)" -ForegroundColor Gray
            Start-Sleep -Seconds 1
        }
    }
}

if ($retries -ge $maxRetries) {
    Write-Host "ERROR: Vault is not accessible" -ForegroundColor Red
    exit 1
}

Write-Host "[1/2] Vault is accessible" -ForegroundColor Green
Write-Host ""

# Populate secrets
Write-Host "[2/2] Populating secrets..." -ForegroundColor Yellow

# Note: In dev mode, the KV secrets engine is already enabled at 'secret/'
# No need to enable it again

# Populate all secrets
docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/database `
  host='postgres' port='5432' database='coinpay' username='postgres' password='root' `
  connection_string='Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/redis `
  connection_string='localhost:6379' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/circle `
  api_key='TEST_API_KEY:d93edad9d7011eae471468f01252bafa:8cc4aae56a478a0a313914a062be0445' `
  entity_secret='dc1ff0c795a9701035d45927a8cfc3dd54255f19e1ceebb8e50bafeaf2493d26' `
  webhook_secret='test_webhook_secret_def456ghi012jkl345mno678pqr901stu234' `
  api_base_url='https://api.circle.com/v1/w3s' `
  app_id='0f473fcf-335f-5e52-b1d2-ee9de5d43c9f' `
  use_mock_mode='true' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/jwt `
  secret_key='DevelopmentSecretKey_ChangeInProduction_MinimumLength32Characters' `
  issuer='CoinPay' audience='CoinPay' expiration_minutes='1440' `
  refresh_token_expiration_days='7' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/gateway `
  webhook_secret='dev-webhook-secret-change-in-production' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/blockchain `
  test_wallet_private_key='YOUR_TEST_WALLET_PRIVATE_KEY_HERE' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/whitebit `
  api_url='https://whitebit.com/api/v4' `
  base_url='https://whitebit.com' `
  use_mock_mode='true' > $null

docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/oneinch `
  api_url='https://api.1inch.dev/swap/v6.0' `
  api_key='YOUR_1INCH_API_KEY_HERE' `
  use_mock_mode='true' > $null

Write-Host "[2/2] All secrets populated successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Vault Secrets Ready!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Vault Configuration:" -ForegroundColor White
Write-Host "  - Mode: Development (in-memory)" -ForegroundColor Yellow
Write-Host "  - Address: $VAULT_ADDR" -ForegroundColor Cyan
Write-Host "  - Root Token: $VAULT_TOKEN" -ForegroundColor Yellow
Write-Host "  - Secrets: Populated" -ForegroundColor Green
Write-Host ""
Write-Host "Note: Dev mode uses in-memory storage." -ForegroundColor Yellow
Write-Host "Secrets will be lost on restart and need repopulation." -ForegroundColor Yellow
Write-Host ""
