# CoinPay - Rebuild Web Service
# This script rebuilds the web container to pick up frontend changes

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "CoinPay - Rebuild Web Service" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Change to project root
$ProjectRoot = "D:\Projects\Test\Claude\CoinPay"
Set-Location $ProjectRoot

# Stop web container
Write-Host "[1/3] Stopping web container..." -ForegroundColor Yellow
docker stop coinpay-web 2>$null
docker rm coinpay-web 2>$null
Write-Host "[OK] Web container stopped" -ForegroundColor Green
Write-Host ""

# Rebuild web service
Write-Host "[2/3] Rebuilding web service..." -ForegroundColor Yellow
docker-compose build web

if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] Web service rebuilt successfully" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Failed to rebuild web service" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Start web container
Write-Host "[3/3] Starting web container..." -ForegroundColor Yellow
docker-compose up -d web

if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] Web service started" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Failed to start web service" -ForegroundColor Red
    exit 1
}
Write-Host ""

Write-Host "======================================" -ForegroundColor Green
Write-Host "Web Service Rebuilt Successfully!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "Access the web app at: http://localhost:3000" -ForegroundColor Cyan
Write-Host ""
