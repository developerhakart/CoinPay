# Vault Backup Script
# Backs up Vault secrets to Deployment/Start/backups folder

$ErrorActionPreference = "Stop"

$BACKUP_DIR = ".\Deployment\Start\backups"
$TIMESTAMP = Get-Date -Format "yyyyMMdd_HHmmss"
$VAULT_TOKEN = "dev-root-token"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Vault Backup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Create backup directory if it doesn't exist
if (-not (Test-Path $BACKUP_DIR)) {
    New-Item -ItemType Directory -Path $BACKUP_DIR -Force | Out-Null
    Write-Host "Created backup directory: $BACKUP_DIR" -ForegroundColor Green
}

# Check if vault container is running
Write-Host "[1/2] Checking Vault container..." -ForegroundColor Yellow
$vaultRunning = docker ps --filter "name=coinpay-vault" --format "{{.Names}}"

if (-not $vaultRunning) {
    Write-Host "ERROR: Vault container is not running" -ForegroundColor Red
    Write-Host "Cannot backup Vault when container is stopped" -ForegroundColor Red
    exit 1
}

Write-Host "[1/2] Vault container is running" -ForegroundColor Green
Write-Host ""

# Backup all Vault secrets
Write-Host "[2/2] Backing up Vault secrets..." -ForegroundColor Yellow

$secrets = @("database", "redis", "circle", "jwt", "gateway", "blockchain", "whitebit", "oneinch")
$backedUpCount = 0

foreach ($secret in $secrets) {
    $backupFile = "$BACKUP_DIR\vault-${secret}_$TIMESTAMP.json"

    try {
        docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv get -format=json secret/coinpay/$secret > $backupFile 2>$null

        if ($LASTEXITCODE -eq 0 -and (Test-Path $backupFile) -and (Get-Item $backupFile).Length -gt 10) {
            Write-Host "  Success: $secret" -ForegroundColor Green
            $backedUpCount++
        } else {
            Write-Host "  Skipped: $secret (not found or empty)" -ForegroundColor Gray
            if (Test-Path $backupFile) {
                Remove-Item $backupFile
            }
        }
    } catch {
        Write-Host "  Skipped: $secret (error)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "[2/2] Backed up $backedUpCount secret(s)" -ForegroundColor Green
Write-Host ""

if ($backedUpCount -eq 0) {
    Write-Host "=====================================" -ForegroundColor Yellow
    Write-Host "  No Secrets to Backup" -ForegroundColor Yellow
    Write-Host "=====================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Vault appears to be empty. This is normal for a fresh start." -ForegroundColor Yellow
    exit 0
}

Write-Host "=====================================" -ForegroundColor Green
Write-Host "  Backup Completed Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Backup location: $BACKUP_DIR" -ForegroundColor Cyan
Write-Host "Secrets backed up: $backedUpCount" -ForegroundColor Cyan
Write-Host ""
