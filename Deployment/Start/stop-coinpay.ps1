# CoinPay Stop Script with Automatic Backup
# Backs up database and Vault before stopping containers

# Change to project root directory
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
Set-Location $projectRoot

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CoinPay Stop Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Working directory: $projectRoot" -ForegroundColor Gray
Write-Host ""

# Check if containers are running
$runningContainers = docker ps --filter "name=coinpay-" --format "{{.Names}}"

if (-not $runningContainers) {
    Write-Host "No CoinPay containers are currently running" -ForegroundColor Yellow
    exit 0
}

Write-Host "Running containers:" -ForegroundColor White
$runningContainers | ForEach-Object { Write-Host "  - $_" -ForegroundColor Cyan }
Write-Host ""

# Step 1: Backup Database
Write-Host "[1/3] Backing up database..." -ForegroundColor Yellow

$postgresRunning = docker ps --filter "name=coinpay-postgres-compose" --format "{{.Names}}"
if ($postgresRunning) {
    try {
        & ".\Deployment\Start\backup-database.ps1"
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "[1/3] Database backup completed" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "[1/3] Database backup failed (continuing with stop)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host ""
        Write-Host "[1/3] Database backup failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "[1/3] PostgreSQL container not running - skipping database backup" -ForegroundColor Gray
}

Write-Host ""

# Step 2: Backup Vault
Write-Host "[2/3] Backing up Vault secrets..." -ForegroundColor Yellow

$vaultRunning = docker ps --filter "name=coinpay-vault" --format "{{.Names}}"
if ($vaultRunning) {
    try {
        & ".\Deployment\Start\backup-vault.ps1"
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "[2/3] Vault backup completed" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "[2/3] Vault backup skipped (empty or failed)" -ForegroundColor Gray
        }
    } catch {
        Write-Host ""
        Write-Host "[2/3] Vault backup failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "[2/3] Vault container not running - skipping Vault backup" -ForegroundColor Gray
}

Write-Host ""

# Step 3: Stop all containers
Write-Host "[3/3] Stopping all CoinPay containers..." -ForegroundColor Yellow

# Try docker-compose down first
docker-compose down 2>&1 | Out-Null

# Force stop and remove all coinpay containers (in case they're from a different compose project)
$allContainers = docker ps -a --filter "name=coinpay-" --format "{{.Names}}"
if ($allContainers) {
    $allContainers | ForEach-Object {
        docker stop $_ 2>&1 | Out-Null
        docker rm $_ 2>&1 | Out-Null
    }
}

Write-Host "[3/3] All containers stopped and removed" -ForegroundColor Green
Write-Host ""

# Verify all containers are stopped
$stillRunning = docker ps --filter "name=coinpay-" --format "{{.Names}}"
if ($stillRunning) {
    Write-Host "WARNING: Some containers are still running:" -ForegroundColor Yellow
    $stillRunning | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
} else {
    Write-Host "Verified: All CoinPay containers stopped and removed" -ForegroundColor Green
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  CoinPay Stopped Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Backups saved to: .\Deployment\Start\backups\" -ForegroundColor Cyan
Write-Host ""
Write-Host "To start again:" -ForegroundColor White
Write-Host "  .\Deployment\Start\start-coinpay.ps1" -ForegroundColor Cyan
Write-Host ""
