# Automatic Circle Wallet Initialization

## Overview
The deployment system now automatically manages your real Circle Wallet IDs across stop/start cycles.

## How It Works

### 1. Wallet Configuration
The system uses your real Circle Wallet credentials from Circle Console:

**Sender Wallet:**
- Circle Wallet ID: `fef70777-cb2d-5096-a0ea-15dba5662ce6`
- Blockchain Address: `0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579`

**Recipient Wallet:**
- Circle Wallet ID: `40b623f4-5ccd-5006-b4d2-c8b22e61681d`
- Blockchain Address: `0x76f9f32d75fe641c3d3992f0992ae46ed75cab58`

### 2. Initialization Script
`init-real-wallet.ps1` automatically:
- Updates Users table with real Circle Wallet ID
- Updates Wallets table with real blockchain address
- Runs automatically during startup

### 3. Backup & Restore
- **On Stop:** Database backed up with real wallet IDs
- **On Start:** Database restored with real wallet IDs
- **Initialization:** Ensures wallet IDs are correct after restore

## No Manual Updates Required! ✅

Your real Circle Wallet IDs are:
1. ✅ Stored in database backups
2. ✅ Restored automatically on start
3. ✅ Verified by initialization script

**Result:** Stop/start cycles work seamlessly without manual intervention!

## Testing
```powershell
# Stop
.\Deployment\Start\stop-coinpay.ps1

# Start (automatically restores real wallet IDs)
.\Deployment\Start\start-coinpay.ps1

# Test transfer
curl -X POST http://localhost:7777/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 0.05,
    "currency": "POL",
    "type": "Transfer",
    "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58"
  }'
```

## Updating Wallet IDs
If you need to change wallet IDs, edit `init-real-wallet.ps1`:

```powershell
$SENDER_WALLET_ID = "your-new-wallet-id-here"
$SENDER_ADDRESS = "0xYourNewAddressHere"
```

Then run:
```powershell
.\Deployment\Start\stop-coinpay.ps1
.\Deployment\Start\start-coinpay.ps1
```
