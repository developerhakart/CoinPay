# Vault Restore Script
# Restores Vault secrets from most recent backup

param(
    [string]$Timestamp = ""
)

$ErrorActionPreference = "Stop"

$BACKUP_DIR = ".\Deployment\Start\backups"
$VAULT_TOKEN = "dev-root-token"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Vault Restore" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if backup directory exists
if (-not (Test-Path $BACKUP_DIR)) {
    Write-Host "ERROR: Backup directory not found: $BACKUP_DIR" -ForegroundColor Red
    Write-Host "No backups available to restore" -ForegroundColor Red
    exit 1
}

# Find timestamp if not specified
if ($Timestamp -eq "") {
    Write-Host "[1/3] Finding most recent backup..." -ForegroundColor Yellow
    $backups = Get-ChildItem -Path $BACKUP_DIR -Filter "vault-*_*.json" | Sort-Object LastWriteTime -Descending

    if ($backups.Count -eq 0) {
        Write-Host "ERROR: No Vault backups found in $BACKUP_DIR" -ForegroundColor Red
        exit 1
    }

    # Extract timestamp from first backup file
    $Timestamp = $backups[0].Name -replace '.*_(\d{8}_\d{6})\.json', '$1'
    Write-Host "[1/3] Found backup timestamp: $Timestamp" -ForegroundColor Green
}

Write-Host ""

# Check if vault container is running
Write-Host "[2/3] Checking Vault container..." -ForegroundColor Yellow
$vaultRunning = docker ps --filter "name=coinpay-vault" --format "{{.Names}}"

if (-not $vaultRunning) {
    Write-Host "ERROR: Vault container is not running" -ForegroundColor Red
    Write-Host "Start the container first: docker-compose up -d vault" -ForegroundColor Yellow
    exit 1
}

Write-Host "[2/3] Vault container is running" -ForegroundColor Green
Write-Host ""

# Wait for Vault to be ready
Write-Host "Waiting for Vault to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# Restore all Vault secrets
Write-Host "[3/3] Restoring Vault secrets..." -ForegroundColor Yellow

$secrets = @("database", "redis", "circle", "jwt", "gateway", "blockchain", "whitebit", "oneinch")
$restoredCount = 0

foreach ($secret in $secrets) {
    $backupFile = "$BACKUP_DIR\vault-${secret}_$Timestamp.json"

    if (-not (Test-Path $backupFile)) {
        Write-Host "  - Skipped: $secret (backup file not found)" -ForegroundColor Gray
        continue
    }

    try {
        # Read backup file
        $backupContent = Get-Content $backupFile -Raw | ConvertFrom-Json

        # Extract secret data
        $secretData = $backupContent.data.data

        # Build key-value pairs for vault
        $kvPairs = @()
        foreach ($key in $secretData.PSObject.Properties.Name) {
            $value = $secretData.$key
            $kvPairs += "${key}='$value'"
        }

        # Restore to Vault
        $kvString = $kvPairs -join " "
        $cmd = "docker exec -e VAULT_TOKEN=$VAULT_TOKEN coinpay-vault vault kv put secret/coinpay/$secret $kvString"
        Invoke-Expression $cmd 2>&1 | Out-Null

        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] Restored: $secret" -ForegroundColor Green
            $restoredCount++
        } else {
            Write-Host "  [FAILED] Failed: $secret" -ForegroundColor Red
        }
    } catch {
        Write-Host "  [FAILED] Failed: $secret - $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "[3/3] Restored $restoredCount secret(s)" -ForegroundColor Green
Write-Host ""

if ($restoredCount -eq 0) {
    Write-Host "=====================================" -ForegroundColor Yellow
    Write-Host "  No Secrets Restored" -ForegroundColor Yellow
    Write-Host "=====================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "No secrets were restored from backup" -ForegroundColor Yellow
    exit 1
}

Write-Host "=====================================" -ForegroundColor Green
Write-Host "  Restore Completed Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Secrets restored: $restoredCount" -ForegroundColor Cyan
Write-Host ""
