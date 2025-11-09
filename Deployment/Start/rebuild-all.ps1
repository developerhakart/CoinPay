# CoinPay - Rebuild All Services
# This script rebuilds all Docker containers

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "CoinPay - Rebuild All Services" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Change to project root
$ProjectRoot = "D:\Projects\Test\Claude\CoinPay"
Set-Location $ProjectRoot

# Step 1: Stop all containers
Write-Host "[1/4] Stopping all containers..." -ForegroundColor Yellow
& ".\Deployment\Start\stop-coinpay.ps1"
Write-Host ""

# Step 2: Rebuild all services
Write-Host "[2/4] Rebuilding all services..." -ForegroundColor Yellow
Write-Host "This may take several minutes..." -ForegroundColor Gray
docker-compose build

if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] All services rebuilt successfully" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Failed to rebuild services" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 3: Start all containers
Write-Host "[3/4] Starting all services..." -ForegroundColor Yellow
& ".\Deployment\Start\start-coinpay.ps1"
Write-Host ""

# Step 4: Verify
Write-Host "[4/4] Verifying services..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
docker-compose ps
Write-Host ""

Write-Host "======================================" -ForegroundColor Green
Write-Host "All Services Rebuilt Successfully!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "Services:" -ForegroundColor Cyan
Write-Host "  Web UI:  http://localhost:3000" -ForegroundColor White
Write-Host "  API:     http://localhost:7777" -ForegroundColor White
Write-Host "  Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "  Swagger: http://localhost:7777/swagger" -ForegroundColor White
Write-Host ""
