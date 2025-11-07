# Initialize Real Circle Wallet ID
# This script ensures the database has real Circle wallet IDs after restore

$ErrorActionPreference = "Continue"

Write-Host "Initializing real Circle wallet IDs..." -ForegroundColor Yellow

# Real Circle wallet credentials
$SENDER_WALLET_ID = "fef70777-cb2d-5096-a0ea-15dba5662ce6"
$SENDER_ADDRESS = "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579"

# Update database with real Circle wallet ID
docker exec coinpay-postgres-compose psql -U postgres -d coinpay -c "
UPDATE \"Users\"
SET \"CircleWalletId\" = '$SENDER_WALLET_ID',
    \"WalletAddress\" = '$SENDER_ADDRESS'
WHERE \"Username\" = 'testuser';

UPDATE \"Wallets\"
SET \"CircleWalletId\" = '$SENDER_WALLET_ID',
    \"Address\" = '$SENDER_ADDRESS'
WHERE \"UserId\" = 1;
" 2>&1 | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] Real Circle wallet ID initialized successfully" -ForegroundColor Green
} else {
    Write-Host "[WARNING] Could not initialize wallet ID - database may not be ready" -ForegroundColor Yellow
}
