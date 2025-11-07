# Vault Initialization and Auto-Unseal Script
# This script initializes Vault (if needed), unseals it, and populates secrets

$ErrorActionPreference = "Stop"

$VAULT_ADDR = "http://localhost:8200"
$KEYS_FILE = "./vault/unseal-keys.json"
$ROOT_TOKEN_FILE = "./vault/.root-token"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Vault Auto-Initialization & Unseal" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Wait for Vault to be available
Write-Host "[1/5] Waiting for Vault to be available..." -ForegroundColor Yellow
$retries = 0
$maxRetries = 30

while ($retries -lt $maxRetries) {
    try {
        $null = Invoke-WebRequest -Uri "$VAULT_ADDR/v1/sys/health" -Method GET -UseBasicParsing -TimeoutSec 2 2>$null
        break
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

Write-Host "[1/5] Vault is accessible" -ForegroundColor Green
Write-Host ""

# Step 2: Check if Vault is initialized
Write-Host "[2/5] Checking Vault initialization status..." -ForegroundColor Yellow

$initStatus = docker exec coinpay-vault vault status -format=json 2>$null | ConvertFrom-Json

if (-not $initStatus.initialized) {
    Write-Host "[2/5] Vault is not initialized. Initializing now..." -ForegroundColor Yellow

    # Initialize Vault
    $initOutput = docker exec coinpay-vault vault operator init -key-shares=3 -key-threshold=2 -format=json | ConvertFrom-Json

    # Save unseal keys and root token
    $keysData = @{
        unseal_keys_b64 = $initOutput.unseal_keys_b64
        unseal_keys_hex = $initOutput.unseal_keys_hex
        root_token = $initOutput.root_token
        recovery_keys_b64 = $initOutput.recovery_keys_b64
        recovery_keys_hex = $initOutput.recovery_keys_hex
    }

    $keysData | ConvertTo-Json -Depth 10 | Set-Content $KEYS_FILE
    $initOutput.root_token | Set-Content $ROOT_TOKEN_FILE

    Write-Host "[2/5] Vault initialized successfully!" -ForegroundColor Green
    Write-Host "      Unseal keys saved to: $KEYS_FILE" -ForegroundColor Yellow
    Write-Host "      Root token saved to: $ROOT_TOKEN_FILE" -ForegroundColor Yellow
    Write-Host "      IMPORTANT: Backup these files securely!" -ForegroundColor Red
} else {
    Write-Host "[2/5] Vault is already initialized" -ForegroundColor Green
}

Write-Host ""

# Step 3: Unseal Vault if sealed
Write-Host "[3/5] Checking Vault seal status..." -ForegroundColor Yellow

$status = docker exec coinpay-vault vault status -format=json | ConvertFrom-Json

if ($status.sealed) {
    Write-Host "[3/5] Vault is sealed. Unsealing..." -ForegroundColor Yellow

    if (-not (Test-Path $KEYS_FILE)) {
        Write-Host "ERROR: Unseal keys file not found: $KEYS_FILE" -ForegroundColor Red
        Write-Host "Cannot unseal Vault without keys." -ForegroundColor Red
        exit 1
    }

    $keys = Get-Content $KEYS_FILE | ConvertFrom-Json

    # Unseal with threshold number of keys (2 out of 3)
    foreach ($key in $keys.unseal_keys_b64[0..1]) {
        docker exec coinpay-vault vault operator unseal $key | Out-Null
    }

    Write-Host "[3/5] Vault unsealed successfully!" -ForegroundColor Green
} else {
    Write-Host "[3/5] Vault is already unsealed" -ForegroundColor Green
}

Write-Host ""

# Step 4: Get root token
Write-Host "[4/5] Retrieving root token..." -ForegroundColor Yellow

$ROOT_TOKEN = ""
if (Test-Path $ROOT_TOKEN_FILE) {
    $ROOT_TOKEN = Get-Content $ROOT_TOKEN_FILE -Raw | ForEach-Object { $_.Trim() }
    Write-Host "[4/5] Root token loaded from file" -ForegroundColor Green
} elseif (Test-Path $KEYS_FILE) {
    $keys = Get-Content $KEYS_FILE | ConvertFrom-Json
    $ROOT_TOKEN = $keys.root_token
    $ROOT_TOKEN | Set-Content $ROOT_TOKEN_FILE
    Write-Host "[4/5] Root token extracted from keys file" -ForegroundColor Green
} else {
    Write-Host "WARNING: Cannot find root token. Secrets may not be populated." -ForegroundColor Yellow
    $ROOT_TOKEN = "dev-root-token"  # Fallback
}

Write-Host ""

# Step 5: Populate secrets (if they don't exist)
Write-Host "[5/5] Populating secrets..." -ForegroundColor Yellow

$env:VAULT_TOKEN = $ROOT_TOKEN
$env:VAULT_ADDR = $VAULT_ADDR

# Check if secrets already exist
try {
    $secretsList = docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv list secret/coinpay 2>$null

    if ($secretsList -match "database") {
        Write-Host "[5/5] Secrets already exist. Skipping population." -ForegroundColor Green
    } else {
        throw "Secrets not found"
    }
} catch {
    Write-Host "[5/5] Secrets not found. Populating now..." -ForegroundColor Yellow

    # Enable KV v2 secrets engine
    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault secrets enable -version=2 -path=secret kv 2>$null

    # Populate all secrets
    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv put secret/coinpay/database `
      host='postgres' port='5432' database='coinpay' username='postgres' password='root' `
      connection_string='Host=postgres;Port=5432;Database=coinpay;Username=postgres;Password=root' > $null

    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv put secret/coinpay/redis `
      connection_string='localhost:6379' > $null

    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv put secret/coinpay/circle `
      api_key='TEST_API_KEY:d93edad9d7011eae471468f01252bafa:8cc4aae56a478a0a313914a062be0445' `
      entity_secret='dc1ff0c795a9701035d45927a8cfc3dd54255f19e1ceebb8e50bafeaf2493d26' `
      webhook_secret='test_webhook_secret_def456ghi012jkl345mno678pqr901stu234' `
      api_base_url='https://api.circle.com/v1/w3s' `
      app_id='0f473fcf-335f-5e52-b1d2-ee9de5d43c9f' > $null

    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv put secret/coinpay/jwt `
      secret_key='DevelopmentSecretKey_ChangeInProduction_MinimumLength32Characters' `
      issuer='CoinPay' audience='CoinPay' expiration_minutes='1440' `
      refresh_token_expiration_days='7' > $null

    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv put secret/coinpay/gateway `
      webhook_secret='dev-webhook-secret-change-in-production' > $null

    docker exec -e VAULT_TOKEN=$ROOT_TOKEN coinpay-vault vault kv put secret/coinpay/blockchain `
      test_wallet_private_key='YOUR_TEST_WALLET_PRIVATE_KEY_HERE' > $null

    Write-Host "[5/5] All secrets populated successfully!" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Vault Ready!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Status:" -ForegroundColor White
Write-Host "  - Initialized: Yes" -ForegroundColor Green
Write-Host "  - Sealed: No" -ForegroundColor Green
Write-Host "  - Secrets: Populated" -ForegroundColor Green
Write-Host "  - Root Token: $ROOT_TOKEN" -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANT NOTES:" -ForegroundColor Red
Write-Host "  - Unseal keys are saved in: $KEYS_FILE" -ForegroundColor Yellow
Write-Host "  - Root token is saved in: $ROOT_TOKEN_FILE" -ForegroundColor Yellow
Write-Host "  - DO NOT commit these files to git!" -ForegroundColor Red
Write-Host "  - Backup these files securely!" -ForegroundColor Red
Write-Host ""
