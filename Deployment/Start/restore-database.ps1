# Database Restore Script
# Restores PostgreSQL database from most recent backup or specified file

param(
    [string]$BackupFile = ""
)

$ErrorActionPreference = "Stop"

$BACKUP_DIR = ".\Deployment\Start\backups"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Database Restore" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if backup directory exists
if (-not (Test-Path $BACKUP_DIR)) {
    Write-Host "ERROR: Backup directory not found: $BACKUP_DIR" -ForegroundColor Red
    Write-Host "No backups available to restore" -ForegroundColor Red
    exit 1
}

# Find backup file
if ($BackupFile -eq "") {
    Write-Host "[1/4] Finding most recent backup..." -ForegroundColor Yellow
    $backups = Get-ChildItem -Path $BACKUP_DIR -Filter "database_*.sql" | Sort-Object LastWriteTime -Descending

    if ($backups.Count -eq 0) {
        Write-Host "ERROR: No database backups found in $BACKUP_DIR" -ForegroundColor Red
        exit 1
    }

    $BackupFile = $backups[0].FullName
    Write-Host "[1/4] Found backup: $($backups[0].Name)" -ForegroundColor Green
} else {
    Write-Host "[1/4] Using specified backup: $BackupFile" -ForegroundColor Yellow
    if (-not (Test-Path $BackupFile)) {
        Write-Host "ERROR: Backup file not found: $BackupFile" -ForegroundColor Red
        exit 1
    }
}

$backupSize = (Get-Item $BackupFile).Length / 1MB
$backupSizeMB = [math]::Round($backupSize, 2)
Write-Host "    Backup size: $backupSizeMB MB" -ForegroundColor Cyan
Write-Host ""

# Check if postgres container is running
Write-Host "[2/4] Checking PostgreSQL container..." -ForegroundColor Yellow
$postgresRunning = docker ps --filter "name=coinpay-postgres-compose" --format "{{.Names}}"

if (-not $postgresRunning) {
    Write-Host "ERROR: PostgreSQL container is not running" -ForegroundColor Red
    Write-Host "Start the container first: docker-compose up -d postgres" -ForegroundColor Yellow
    exit 1
}

Write-Host "[2/4] PostgreSQL container is running" -ForegroundColor Green
Write-Host ""

# Confirm restoration
Write-Host "[3/4] Ready to restore database..." -ForegroundColor Yellow
Write-Host ""
Write-Host "WARNING: This will overwrite the current database!" -ForegroundColor Red
$confirmation = Read-Host "Type 'yes' to continue"

if ($confirmation -ne "yes") {
    Write-Host "Restore cancelled" -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# Restore database
Write-Host "[4/4] Restoring database..." -ForegroundColor Yellow

# Drop and recreate database
docker exec coinpay-postgres-compose psql -U postgres -c "DROP DATABASE IF EXISTS coinpay" 2>&1 | Out-Null
docker exec coinpay-postgres-compose psql -U postgres -c "CREATE DATABASE coinpay" 2>&1 | Out-Null

# Restore from backup
Get-Content $BackupFile | docker exec -i coinpay-postgres-compose psql -U postgres -d coinpay 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Database restore failed" -ForegroundColor Red
    exit 1
}

Write-Host "[4/4] Database restored successfully" -ForegroundColor Green
Write-Host ""

# Verify restoration
Write-Host "Verifying restoration..." -ForegroundColor Yellow
$userCount = docker exec coinpay-postgres-compose psql -U postgres -d coinpay -t -c "SELECT COUNT(*) FROM \"Users\"" 2>$null

if ($LASTEXITCODE -eq 0) {
    $userCount = $userCount.Trim()
    Write-Host "  Users in database: $userCount" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  Restore Completed Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
