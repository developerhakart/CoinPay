# CoinPay Startup Script
# This script starts all services with Vault in dev mode

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CoinPay Startup Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Start docker-compose services
Write-Host "[1/3] Starting docker-compose services..." -ForegroundColor Yellow
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to start docker-compose services" -ForegroundColor Red
    exit 1
}

Write-Host "[1/3] Services started" -ForegroundColor Green
Write-Host ""

# Step 2: Populate Vault secrets
Write-Host "[2/3] Populating Vault secrets..." -ForegroundColor Yellow
Write-Host ""

& ".\vault\scripts\populate-dev-secrets.ps1"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: Failed to populate Vault secrets" -ForegroundColor Red
    exit 1
}

Write-Host "[2/3] Vault secrets populated" -ForegroundColor Green
Write-Host ""

# Step 3: Restart API to load secrets
Write-Host "[3/3] Restarting API to load secrets..." -ForegroundColor Yellow
docker-compose restart api > $null

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restart API" -ForegroundColor Red
    exit 1
}

Write-Host "[3/3] API restarted" -ForegroundColor Green
Write-Host ""

# Wait for API to be ready
Write-Host "Verifying system health...  " -ForegroundColor Yellow
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
Write-Host "Vault:" -ForegroundColor White
Write-Host "  - Status:     Running (Dev Mode)" -ForegroundColor Yellow
Write-Host "  - Root Token: dev-root-token" -ForegroundColor Yellow
Write-Host "  - Secrets:    Populated" -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT:" -ForegroundColor Yellow
Write-Host "  - Vault is running in development mode" -ForegroundColor Yellow
Write-Host "  - Secrets are stored in-memory (will be lost on restart)" -ForegroundColor Yellow
Write-Host "  - Re-run this script after stopping to repopulate secrets" -ForegroundColor Yellow
Write-Host ""
Write-Host "Commands:" -ForegroundColor Gray
Write-Host "  - Stop: docker-compose down" -ForegroundColor Gray
Write-Host "  - Logs: docker-compose logs -f" -ForegroundColor Gray
Write-Host "  - Status: docker-compose ps" -ForegroundColor Gray
Write-Host ""
