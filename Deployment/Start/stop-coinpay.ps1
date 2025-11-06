# CoinPay Stop Script
# This script stops all CoinPay services

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CoinPay Stop Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to project root (one level up from Deployment/Start)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptPath)
Set-Location $projectRoot

Write-Host "[1/2] Stopping all containers..." -ForegroundColor Yellow
docker-compose down

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to stop containers" -ForegroundColor Red
    exit 1
}

Write-Host "[1/2] Containers stopped" -ForegroundColor Green
Write-Host ""

Write-Host "[2/2] Checking container status..." -ForegroundColor Yellow
$containers = docker ps -a --filter "name=coinpay" --format "{{.Names}}"

if ([string]::IsNullOrWhiteSpace($containers)) {
    Write-Host "[2/2] All CoinPay containers removed" -ForegroundColor Green
} else {
    Write-Host "[2/2] Some containers still exist (stopped):" -ForegroundColor Yellow
    docker ps -a --filter "name=coinpay" --format "table {{.Names}}\t{{.Status}}"
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  CoinPay Stopped Successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Note:" -ForegroundColor Yellow
Write-Host "  - All containers have been stopped and removed" -ForegroundColor Yellow
Write-Host "  - Data volumes are preserved (postgres-data)" -ForegroundColor Yellow
Write-Host "  - Vault data is lost (dev mode uses in-memory storage)" -ForegroundColor Yellow
Write-Host ""
Write-Host "To restart:" -ForegroundColor Gray
Write-Host "  .\Deployment\Start\start-coinpay.ps1" -ForegroundColor Gray
Write-Host ""
