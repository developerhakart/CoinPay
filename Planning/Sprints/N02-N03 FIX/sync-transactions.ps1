# PowerShell script to call Circle API and sync transaction statuses

$walletId = "fef70777-cb2d-5096-a0ea-15dba5662ce6"
$apiKey = $env:CIRCLE_API_KEY
$baseUrl = "https://api.circle.com/v1/w3s"

Write-Host "Fetching transactions for wallet: $walletId" -ForegroundColor Cyan

# Get wallet transactions from Circle API
$headers = @{
    "Authorization" = "Bearer $apiKey"
    "X-Correlation-Id" = [Guid]::NewGuid().ToString()
}

$url = "$baseUrl/developer/wallets/$walletId/transactions"
Write-Host "Calling Circle API: $url" -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
    Write-Host "Successfully fetched transactions" -ForegroundColor Green
    Write-Host "Response:" -ForegroundColor Yellow
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Host "Error fetching transactions:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host $_.Exception.Response -ForegroundColor Red
}
