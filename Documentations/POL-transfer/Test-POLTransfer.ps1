# CoinPay POL Transfer Test Script
# This script tests the 0.001 POL transfer functionality

param(
    [Parameter(Mandatory=$false)]
    [string]$TestWalletPrivateKey,

    [Parameter(Mandatory=$false)]
    [string]$RecipientAddress = "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",

    [Parameter(Mandatory=$false)]
    [decimal]$Amount = 0.001,

    [Parameter(Mandatory=$false)]
    [string]$ApiUrl = "http://localhost:5100"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   CoinPay POL Transfer Test Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check if private key is provided
if ([string]::IsNullOrEmpty($TestWalletPrivateKey)) {
    Write-Host "[WARNING] No private key provided!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "You have 3 options to provide a test wallet private key:" -ForegroundColor White
    Write-Host "1. Set environment variable: `$env:TEST_WALLET_PRIVATE_KEY='your_key'" -ForegroundColor Gray
    Write-Host "2. Add to appsettings.Development.json: Blockchain:TestWallet:PrivateKey" -ForegroundColor Gray
    Write-Host "3. Pass as parameter: .\Test-POLTransfer.ps1 -TestWalletPrivateKey 'your_key'" -ForegroundColor Gray
    Write-Host ""

    $TestWalletPrivateKey = $env:TEST_WALLET_PRIVATE_KEY
    if ([string]::IsNullOrEmpty($TestWalletPrivateKey)) {
        Write-Host "[INFO] API will return mock response without real blockchain transfer" -ForegroundColor Yellow
        Write-Host ""
    } else {
        Write-Host "[OK] Found private key in environment variable" -ForegroundColor Green
    }
} else {
    Write-Host "[OK] Private key provided via parameter" -ForegroundColor Green
}

# Step 2: Set environment variable if needed
if (-not [string]::IsNullOrEmpty($TestWalletPrivateKey)) {
    $env:TEST_WALLET_PRIVATE_KEY = $TestWalletPrivateKey
    Write-Host "[INFO] Set TEST_WALLET_PRIVATE_KEY environment variable" -ForegroundColor Cyan
}

# Step 3: Check if API is running
Write-Host ""
Write-Host "Checking if API is running on $ApiUrl..." -ForegroundColor Cyan
try {
    $healthCheck = Invoke-WebRequest -Uri "$ApiUrl/health" -Method GET -UseBasicParsing -ErrorAction Stop
    Write-Host "[OK] API is running (Health: $($healthCheck.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] API is not running!" -ForegroundColor Red
    Write-Host "Please start the API first: cd CoinPay.Api && dotnet run" -ForegroundColor Yellow
    exit 1
}

# Step 4: Login to get JWT token
Write-Host ""
Write-Host "Logging in as test user..." -ForegroundColor Cyan
$loginBody = @{
    email = "testuser@coinpay.com"
    password = "Test@123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$ApiUrl/api/auth/login" -Method POST `
        -Body $loginBody -ContentType "application/json" -ErrorAction Stop

    $token = $loginResponse.token
    Write-Host "[OK] Logged in successfully" -ForegroundColor Green
    Write-Host "     Token: $($token.Substring(0, 20))..." -ForegroundColor Gray
} catch {
    Write-Host "[ERROR] Login failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure test user exists. Create one:" -ForegroundColor Yellow
    Write-Host "POST $ApiUrl/api/auth/register" -ForegroundColor Gray
    Write-Host '@{email="testuser@coinpay.com"; password="Test@123"; username="testuser"}' -ForegroundColor Gray
    exit 1
}

# Step 5: Get wallet address from user profile
Write-Host ""
Write-Host "Getting wallet address..." -ForegroundColor Cyan
try {
    $headers = @{ "Authorization" = "Bearer $token" }
    $profile = Invoke-RestMethod -Uri "$ApiUrl/api/auth/me" -Method GET -Headers $headers -ErrorAction Stop

    $fromAddress = $profile.walletAddress
    if ([string]::IsNullOrEmpty($fromAddress)) {
        Write-Host "[ERROR] User does not have a wallet address!" -ForegroundColor Red
        Write-Host "Please create a wallet for this user first" -ForegroundColor Yellow
        exit 1
    }

    Write-Host "[OK] Wallet address: $fromAddress" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] Failed to get user profile: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 6: Check POL balance
Write-Host ""
Write-Host "Checking POL balance..." -ForegroundColor Cyan
try {
    $balance = Invoke-RestMethod -Uri "$ApiUrl/api/wallet/balance/$fromAddress" -Method GET -ErrorAction Stop

    $polBalance = $balance.nativeBalance
    Write-Host "[INFO] Current POL balance: $polBalance POL" -ForegroundColor Cyan

    if ($polBalance -lt $Amount) {
        Write-Host "[WARNING] Insufficient POL balance for transfer!" -ForegroundColor Yellow
        Write-Host "          Required: $Amount POL" -ForegroundColor Yellow
        Write-Host "          Available: $polBalance POL" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Get free testnet POL from: https://faucet.polygon.technology/" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "[WARNING] Could not check balance: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Step 7: Initiate POL transfer
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Initiating POL Transfer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "From:     $fromAddress" -ForegroundColor White
Write-Host "To:       $RecipientAddress" -ForegroundColor White
Write-Host "Amount:   $Amount POL" -ForegroundColor White
Write-Host ""

$transferBody = @{
    fromWalletAddress = $fromAddress
    toWalletAddress = $RecipientAddress
    amount = $Amount
    currency = "POL"
    memo = "Test POL transfer - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
} | ConvertTo-Json

try {
    $transferResponse = Invoke-RestMethod -Uri "$ApiUrl/api/wallet/transfer" -Method POST `
        -Body $transferBody -ContentType "application/json" -Headers $headers -ErrorAction Stop

    Write-Host "[SUCCESS] Transfer initiated!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Transaction Details:" -ForegroundColor Cyan
    Write-Host "-------------------" -ForegroundColor Cyan
    Write-Host "Transaction ID: $($transferResponse.transactionId)" -ForegroundColor White
    Write-Host "Status:         $($transferResponse.status)" -ForegroundColor White
    Write-Host "Amount:         $($transferResponse.amount) POL" -ForegroundColor White
    Write-Host "From:           $($transferResponse.fromAddress)" -ForegroundColor White
    Write-Host "To:             $($transferResponse.toAddress)" -ForegroundColor White
    Write-Host "Initiated At:   $($transferResponse.initiatedAt)" -ForegroundColor White
    Write-Host ""

    # Check if it's a real transaction or mock
    if ($transferResponse.transactionId -match "^0x[a-fA-F0-9]{64}$") {
        Write-Host "[OK] Real blockchain transaction!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Verify on PolygonScan:" -ForegroundColor Cyan
        Write-Host "https://amoy.polygonscan.com/tx/$($transferResponse.transactionId)" -ForegroundColor Blue
        Write-Host ""
        Write-Host "Check recipient balance:" -ForegroundColor Cyan
        Write-Host "https://amoy.polygonscan.com/address/$RecipientAddress" -ForegroundColor Blue

        # Open PolygonScan in browser
        $openBrowser = Read-Host "`nOpen PolygonScan in browser? (Y/N)"
        if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
            Start-Process "https://amoy.polygonscan.com/tx/$($transferResponse.transactionId)"
        }
    } else {
        Write-Host "[INFO] Mock transaction (no real blockchain transfer)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "To enable real transfers:" -ForegroundColor Yellow
        Write-Host "1. Set TEST_WALLET_PRIVATE_KEY environment variable" -ForegroundColor Gray
        Write-Host "2. OR add private key to appsettings.Development.json" -ForegroundColor Gray
        Write-Host "3. Restart the API" -ForegroundColor Gray
    }

} catch {
    Write-Host "[ERROR] Transfer failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red

    if ($_.ErrorDetails) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }

    exit 1
}

# Step 8: Wait and check transaction status
if ($transferResponse.transactionId -match "^0x[a-fA-F0-9]{64}$") {
    Write-Host ""
    Write-Host "Waiting 10 seconds for transaction confirmation..." -ForegroundColor Cyan
    Start-Sleep -Seconds 10

    Write-Host "Checking transaction status..." -ForegroundColor Cyan
    try {
        $txStatus = Invoke-RestMethod -Uri "$ApiUrl/api/wallet/transaction/$($transferResponse.transactionId)" `
            -Method GET -Headers $headers -ErrorAction Stop

        Write-Host "[OK] Transaction Status: $($txStatus.status)" -ForegroundColor Green

        if ($txStatus.txHash) {
            Write-Host "     TxHash: $($txStatus.txHash)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "[WARNING] Could not check transaction status" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Test Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

exit 0
