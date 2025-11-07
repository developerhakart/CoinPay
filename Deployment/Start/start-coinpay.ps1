# CoinPay Startup Script with Automatic Restore
# Automatically restores database and Vault if needed

$ErrorActionPreference = "Stop"

# Change to project root directory
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
Set-Location $projectRoot

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CoinPay Startup Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Working directory: $projectRoot" -ForegroundColor Gray
Write-Host ""

# Step 0: Clean up existing containers if any
Write-Host "[0/5] Checking for existing containers..." -ForegroundColor Yellow

$existingContainers = docker ps -a --filter "name=coinpay-" --format "{{.Names}}" 2>$null

if ($existingContainers) {
    Write-Host "[0/5] Found existing containers - cleaning up..." -ForegroundColor Yellow

    # Temporarily allow errors for cleanup
    $prevErrorAction = $ErrorActionPreference
    $ErrorActionPreference = "Continue"

    # Stop and remove existing containers
    docker-compose down *>&1 | Out-Null

    # Force remove any remaining containers
    docker ps -a --filter "name=coinpay-" --format "{{.Names}}" 2>$null | ForEach-Object {
        docker rm -f $_ *>&1 | Out-Null
    }

    # Restore error preference
    $ErrorActionPreference = $prevErrorAction

    Write-Host "[0/5] Cleanup completed" -ForegroundColor Green
} else {
    Write-Host "[0/5] No existing containers found" -ForegroundColor Green
}

Write-Host ""

# Step 1: Start docker-compose services
Write-Host "[1/5] Starting docker-compose services..." -ForegroundColor Yellow
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to start docker-compose services" -ForegroundColor Red
    exit 1
}

Write-Host "[1/5] Services started" -ForegroundColor Green
Write-Host ""

# Wait for services to initialize
Write-Host "Waiting for services to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Step 2: Check and restore database if needed
Write-Host "[2/5] Checking database..." -ForegroundColor Yellow

# Suppress error output for database check
$prevErrorAction = $ErrorActionPreference
$ErrorActionPreference = "Continue"

$userCount = docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c 'SELECT COUNT(*) FROM "Users"' 2>$null

$ErrorActionPreference = $prevErrorAction

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($userCount)) {
    Write-Host "[2/5] Database is empty - checking for backups..." -ForegroundColor Yellow

    $backupDir = ".\Deployment\Start\backups"
    $backups = Get-ChildItem -Path $backupDir -Filter "database_*.sql" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending

    if ($backups.Count -gt 0) {
        Write-Host "[2/5] Found backup - restoring database..." -ForegroundColor Yellow
        $latestBackup = $backups[0].FullName

        # Drop and recreate database for clean restore
        $prevErrorAction = $ErrorActionPreference
        $ErrorActionPreference = "Continue"

        docker exec coinpay-postgres-compose psql -U postgres -d postgres -c 'DROP DATABASE IF EXISTS coinpay' *>&1 | Out-Null
        docker exec coinpay-postgres-compose psql -U postgres -d postgres -c 'CREATE DATABASE coinpay' *>&1 | Out-Null

        # Restore without confirmation prompt
        Get-Content $latestBackup | docker exec -i coinpay-postgres-compose psql -U postgres -d coinpay *>&1 | Out-Null

        $ErrorActionPreference = $prevErrorAction

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[2/5] Database restored successfully" -ForegroundColor Green
        } else {
            Write-Host "[2/5] Database restore failed - will seed test user later" -ForegroundColor Yellow
        }
    } else {
        Write-Host "[2/5] No backups found - will seed test user later" -ForegroundColor Yellow
    }
} else {
    $userCount = $userCount.Trim()
    Write-Host "[2/5] Database OK - found $userCount user(s)" -ForegroundColor Green
}

Write-Host ""

# Step 3: Check and restore Vault if needed
Write-Host "[3/5] Checking Vault secrets..." -ForegroundColor Yellow

# Suppress error output for vault check
$prevErrorAction = $ErrorActionPreference
$ErrorActionPreference = "Continue"

$vaultCheck = docker exec -e VAULT_TOKEN=dev-root-token coinpay-vault vault kv get secret/coinpay/circle 2>$null

$ErrorActionPreference = $prevErrorAction

if ($LASTEXITCODE -ne 0) {
    Write-Host "[3/5] Vault is empty - checking for backups..." -ForegroundColor Yellow

    $backupDir = ".\Deployment\Start\backups"
    $vaultBackups = Get-ChildItem -Path $backupDir -Filter "vault-circle_*.json" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending

    if ($vaultBackups.Count -gt 0) {
        Write-Host "[3/5] Found backup - restoring Vault..." -ForegroundColor Yellow

        # Extract timestamp from backup file
        $timestamp = $vaultBackups[0].Name -replace '.*_(\d{8}_\d{6})\.json', '$1'

        # Restore Vault secrets
        & ".\Deployment\Start\restore-vault.ps1" -Timestamp $timestamp

        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "[3/5] Vault restored successfully" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "[3/5] Vault restore failed - populating default secrets..." -ForegroundColor Yellow
            & ".\Deployment\populate-dev-secrets.ps1"
        }
    } else {
        Write-Host "[3/5] No backups found - populating default secrets..." -ForegroundColor Yellow
        & ".\Deployment\populate-dev-secrets.ps1"
    }
} else {
    Write-Host "[3/5] Vault OK - secrets already populated" -ForegroundColor Green
}

Write-Host ""

# Step 4: Ensure test user exists
Write-Host "[4/5] Checking test user..." -ForegroundColor Yellow

# Suppress error output for test user check
$prevErrorAction = $ErrorActionPreference
$ErrorActionPreference = "Continue"

$testUserCheck = docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c 'SELECT COUNT(*) FROM "Users" WHERE "Username" = ''testuser''' 2>$null

$ErrorActionPreference = $prevErrorAction

if ($LASTEXITCODE -eq 0) {
    $testUserCount = $testUserCheck.Trim()

    if ($testUserCount -eq "0") {
        Write-Host "[4/5] Creating test user..." -ForegroundColor Yellow

        # Create test user
        docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c 'INSERT INTO "Users" ("Username", "CircleUserId", "CredentialId", "CreatedAt", "LastLoginAt", "WalletAddress", "CircleWalletId") VALUES (''testuser'', ''test-circle-user-id-12345'', ''test-credential-id-67890'', NOW(), NOW(), ''0x1234567890123456789012345678901234567890'', ''test-wallet-id-abc123'') ON CONFLICT DO NOTHING;' 2>&1 | Out-Null

        # Create test wallet
        docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c 'INSERT INTO "Wallets" ("UserId", "Address", "CircleWalletId", "Blockchain", "WalletType", "Balance", "BalanceCurrency", "CreatedAt") VALUES (1, ''0x1234567890123456789012345678901234567890'', ''test-wallet-id-abc123'', ''POL-AMOY'', ''EOA'', 1000.00, ''USDC'', NOW()) ON CONFLICT ("Address") DO NOTHING;' 2>&1 | Out-Null

        Write-Host "[4/5] Test user created successfully" -ForegroundColor Green
    } else {
        Write-Host "[4/5] Test user already exists" -ForegroundColor Green
    }
} else {
    Write-Host "[4/5] Could not check test user - database may not be ready" -ForegroundColor Yellow
}

Write-Host ""

# Step 4.5: Initialize real Circle wallet IDs
Write-Host "[4.5/5] Initializing real Circle wallet IDs..." -ForegroundColor Yellow
& ".\Deployment\Start\init-real-wallet.ps1"
Write-Host ""

# Step 5: Restart API to load secrets
Write-Host "[5/5] Restarting API to load secrets..." -ForegroundColor Yellow
docker-compose restart api > $null

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restart API" -ForegroundColor Red
    exit 1
}

Write-Host "[5/5] API restarted" -ForegroundColor Green
Write-Host ""

# Wait for API to be ready
Write-Host "Verifying system health..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

$retries = 0
$maxRetries = 20

while ($retries -lt $maxRetries) {
    try {
        $response = Invoke-WebRequest -Uri 'http://localhost:7777/health' -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            Write-Host "System is healthy!" -ForegroundColor Green
            break
        }
    } catch {
        $retries++
        if ($retries -lt $maxRetries) {
            Write-Host "  Waiting for API... ($retries/$maxRetries)" -ForegroundColor Gray
            Start-Sleep -Seconds 1
        }
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  CoinPay Started Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Services:" -ForegroundColor White
Write-Host "  - API:        http://localhost:7777" -ForegroundColor Cyan
Write-Host "  - Swagger:    http://localhost:7777/swagger" -ForegroundColor Cyan
Write-Host "  - Gateway:    http://localhost:5000" -ForegroundColor Cyan
Write-Host "  - Web UI:     http://localhost:3000" -ForegroundColor Cyan
Write-Host "  - Docs:       http://localhost:8080" -ForegroundColor Cyan
Write-Host "  - Vault UI:   http://localhost:8200/ui" -ForegroundColor Cyan
Write-Host ""
Write-Host "Test User:" -ForegroundColor White
Write-Host "  - Username:   testuser" -ForegroundColor Cyan
Write-Host "  - Wallet:     0x1234567890123456789012345678901234567890" -ForegroundColor Cyan
Write-Host "  - Balance:    1000 USDC" -ForegroundColor Cyan
Write-Host ""
Write-Host "Vault:" -ForegroundColor White
Write-Host "  - Status:     Running (Dev Mode)" -ForegroundColor Yellow
Write-Host "  - Root Token: dev-root-token" -ForegroundColor Yellow
Write-Host "  - Secrets:    Populated" -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT:" -ForegroundColor Yellow
Write-Host "  - Vault is running in development mode" -ForegroundColor Yellow
Write-Host "  - Secrets are stored in-memory (will be lost on restart)" -ForegroundColor Yellow
Write-Host "  - Backups are created automatically on stop" -ForegroundColor Yellow
Write-Host ""
Write-Host "Commands:" -ForegroundColor Gray
Write-Host "  - Stop: .\Deployment\Start\stop-coinpay.ps1" -ForegroundColor Gray
Write-Host "  - Logs: docker-compose logs -f" -ForegroundColor Gray
Write-Host "  - Status: docker-compose ps" -ForegroundColor Gray
Write-Host ""
