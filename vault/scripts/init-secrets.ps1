# HashiCorp Vault Initialization Script for CoinPay
# This script populates Vault with all sensitive configuration values

$ErrorActionPreference = "Stop"

# Vault configuration
$env:VAULT_ADDR = "http://localhost:8200"
$env:VAULT_TOKEN = "dev-root-token"

Write-Host "Waiting for Vault to be ready..." -ForegroundColor Cyan
Start-Sleep -Seconds 3

# Test Vault connectivity
try {
    vault status | Out-Null
    Write-Host "Vault is ready!" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Vault is not accessible at $env:VAULT_ADDR" -ForegroundColor Red
    Write-Host "Please ensure Vault container is running: docker-compose up vault" -ForegroundColor Yellow
    exit 1
}

Write-Host "`nInitializing Vault secrets..." -ForegroundColor Cyan

# Enable KV v2 secrets engine (ignore if already enabled)
Write-Host "Enabling KV secrets engine..." -ForegroundColor White
vault secrets enable -version=2 -path=secret kv 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "KV secrets engine already enabled (this is OK)" -ForegroundColor Yellow
}

# Database Configuration
Write-Host "`nSetting up database secrets..." -ForegroundColor White
vault kv put secret/coinpay/database `
  host="postgres" `
  port="5432" `
  database="coinpay" `
  username="postgres" `
  password="root" `
  connection_string="Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  Database secrets stored successfully" -ForegroundColor Green
}

# Redis Configuration
Write-Host "`nSetting up Redis secrets..." -ForegroundColor White
vault kv put secret/coinpay/redis `
  connection_string="localhost:6379"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  Redis secrets stored successfully" -ForegroundColor Green
}

# Circle API Configuration
Write-Host "`nSetting up Circle API secrets..." -ForegroundColor White
vault kv put secret/coinpay/circle `
  api_key="TEST_API_KEY:d93edad9d7011eae471468f01252bafa:8cc4aae56a478a0a313914a062be0445" `
  entity_secret="dc1ff0c795a9701035d45927a8cfc3dd54255f19e1ceebb8e50bafeaf2493d26" `
  webhook_secret="test_webhook_secret_def456ghi012jkl345mno678pqr901stu234" `
  api_base_url="https://api.circle.com/v1/w3s" `
  app_id="0f473fcf-335f-5e52-b1d2-ee9de5d43c9f"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  Circle API secrets stored successfully" -ForegroundColor Green
}

# JWT Configuration
Write-Host "`nSetting up JWT secrets..." -ForegroundColor White
vault kv put secret/coinpay/jwt `
  secret_key="DevelopmentSecretKey_ChangeInProduction_MinimumLength32Characters" `
  issuer="CoinPay" `
  audience="CoinPay" `
  expiration_minutes="1440" `
  refresh_token_expiration_days="7"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  JWT secrets stored successfully" -ForegroundColor Green
}

# Gateway Configuration
Write-Host "`nSetting up Gateway secrets..." -ForegroundColor White
vault kv put secret/coinpay/gateway `
  webhook_secret="dev-webhook-secret-change-in-production"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  Gateway secrets stored successfully" -ForegroundColor Green
}

# Blockchain Test Wallet Configuration
Write-Host "`nSetting up Blockchain test wallet secrets..." -ForegroundColor White
vault kv put secret/coinpay/blockchain `
  test_wallet_private_key="YOUR_TEST_WALLET_PRIVATE_KEY_HERE"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  Blockchain secrets stored successfully" -ForegroundColor Green
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Vault initialization complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nSecrets are stored at the following paths:" -ForegroundColor White
Write-Host "  - secret/coinpay/database" -ForegroundColor Yellow
Write-Host "  - secret/coinpay/redis" -ForegroundColor Yellow
Write-Host "  - secret/coinpay/circle" -ForegroundColor Yellow
Write-Host "  - secret/coinpay/jwt" -ForegroundColor Yellow
Write-Host "  - secret/coinpay/gateway" -ForegroundColor Yellow
Write-Host "  - secret/coinpay/blockchain" -ForegroundColor Yellow

Write-Host "`nYou can verify secrets with:" -ForegroundColor Cyan
Write-Host "  vault kv get secret/coinpay/database" -ForegroundColor White

Write-Host "`nTo view all secrets:" -ForegroundColor Cyan
Write-Host "  vault kv list secret/coinpay" -ForegroundColor White
