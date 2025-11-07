# Database Backup Script
# Backs up PostgreSQL database to Deployment/Start/backups folder

$ErrorActionPreference = "Stop"

$BACKUP_DIR = ".\Deployment\Start\backups"
$TIMESTAMP = Get-Date -Format "yyyyMMdd_HHmmss"
$BACKUP_FILE = "$BACKUP_DIR\database_$TIMESTAMP.sql"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Database Backup" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Create backup directory if it doesn't exist
if (-not (Test-Path $BACKUP_DIR)) {
    New-Item -ItemType Directory -Path $BACKUP_DIR -Force | Out-Null
    Write-Host "Created backup directory: $BACKUP_DIR" -ForegroundColor Green
}

# Check if postgres container is running
Write-Host "[1/3] Checking PostgreSQL container..." -ForegroundColor Yellow
$postgresRunning = docker ps --filter "name=coinpay-postgres-compose" --format "{{.Names}}"

if (-not $postgresRunning) {
    Write-Host "ERROR: PostgreSQL container is not running" -ForegroundColor Red
    Write-Host "Cannot backup database when container is stopped" -ForegroundColor Red
    exit 1
}

Write-Host "[1/3] PostgreSQL container is running" -ForegroundColor Green
Write-Host ""

# Create database backup
Write-Host "[2/3] Creating database backup..." -ForegroundColor Yellow
docker exec coinpay-postgres-compose pg_dump -U postgres -d coinpay > $BACKUP_FILE

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Database backup failed" -ForegroundColor Red
    exit 1
}

$backupSize = (Get-Item $BACKUP_FILE).Length
$backupSizeMB = [math]::Round($backupSize / 1MB, 2)

Write-Host "[2/3] Database backup created: $backupSizeMB MB" -ForegroundColor Green
Write-Host ""

# Verify backup
Write-Host "[3/3] Verifying backup..." -ForegroundColor Yellow
$lineCount = (Get-Content $BACKUP_FILE | Measure-Object -Line).Lines

if ($lineCount -lt 10) {
    Write-Host "ERROR: Backup appears to be empty or corrupted" -ForegroundColor Red
    Remove-Item $BACKUP_FILE
    exit 1
}

Write-Host "[3/3] Backup verified: $lineCount lines" -ForegroundColor Green
Write-Host ""

Write-Host "=====================================" -ForegroundColor Green
Write-Host "  Backup Completed Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Backup file: $BACKUP_FILE" -ForegroundColor Cyan
Write-Host "Size: $backupSizeMB MB" -ForegroundColor Cyan
Write-Host "Lines: $lineCount" -ForegroundColor Cyan
Write-Host ""
