# CoinPay Build Script
# This script builds all Docker images for CoinPay services

$ErrorActionPreference = "Stop"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CoinPay Build Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to project root (two levels up from Deployment/Start)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptPath)
Set-Location $projectRoot

Write-Host "Project Root: $projectRoot" -ForegroundColor Cyan
Write-Host ""

# Check if docker-compose.yml exists
if (-not (Test-Path "docker-compose.yml")) {
    Write-Host "ERROR: docker-compose.yml not found in $projectRoot" -ForegroundColor Red
    exit 1
}

Write-Host "[1/3] Building all Docker images..." -ForegroundColor Yellow
Write-Host "This may take several minutes on first build..." -ForegroundColor Gray
Write-Host ""

docker-compose build

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: Failed to build Docker images" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[1/3] Docker images built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "[2/3] Listing built images..." -ForegroundColor Yellow
docker images | Select-String "coinpay"

Write-Host ""
Write-Host "[2/3] Images listed" -ForegroundColor Green
Write-Host ""

Write-Host "[3/3] Verifying image sizes..." -ForegroundColor Yellow
$images = docker images --filter "reference=coinpay*" --format "{{.Repository}}:{{.Tag}} - {{.Size}}"
foreach ($image in $images) {
    Write-Host "  $image" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "[3/3] Verification complete" -ForegroundColor Green
Write-Host ""

Write-Host "=====================================" -ForegroundColor Green
Write-Host "  Build Complete!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Images built:" -ForegroundColor White
Write-Host "  - coinpay-api        (Backend API)" -ForegroundColor Cyan
Write-Host "  - coinpay-gateway    (API Gateway)" -ForegroundColor Cyan
Write-Host "  - coinpay-web        (Frontend)" -ForegroundColor Cyan
Write-Host "  - coinpay-docfx      (Documentation)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Start services:  .\Deployment\Start\start-coinpay.ps1" -ForegroundColor Gray
Write-Host "  2. Stop services:   .\Deployment\Start\stop-coinpay.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "Note: PostgreSQL and Vault use official images (no build needed)" -ForegroundColor Gray
Write-Host ""
