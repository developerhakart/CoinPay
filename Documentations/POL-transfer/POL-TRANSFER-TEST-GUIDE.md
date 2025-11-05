# POL Transfer Testing Guide

This guide will help you test real POL blockchain transfers on Polygon Amoy testnet.

## What Was Implemented

### Backend Changes
1. **DirectTransferService** - New service for executing real blockchain transactions
2. **Updated WalletService** - Now supports both USDC and POL transfers using DirectTransferService
3. **Currency Support** - Transfer API now accepts `currency` parameter ("USDC" or "POL")
4. **Circle Transaction API** - Implemented for future frontend passkey integration

### Architecture
- **Test Mode**: Uses DirectTransferService with a test wallet private key
- **Production Mode**: Will use Circle's passkey-based transaction flow (frontend implementation pending)

---

## Prerequisites

1. ✅ CoinPay API running
2. ✅ MetaMask wallet installed
3. ✅ Polygon Amoy testnet configured in MetaMask
4. ⚠️ Test wallet with POL for gas fees

---

## Step 1: Create a Test Wallet

### Option A: Use Existing MetaMask Wallet

1. Open MetaMask
2. Switch to Polygon Amoy network
3. Click your account → Account Details → Export Private Key
4. Enter MetaMask password
5. Copy the private key (64 character hex string)

⚠️ **WARNING**: NEVER use your main wallet! Create a new test-only wallet.

### Option B: Create New Test Wallet in MetaMask

1. Open MetaMask
2. Click account icon → Create Account
3. Name it "CoinPay Test"
4. Export private key (follow Option A steps 3-5)

---

## Step 2: Fund Test Wallet with POL

Your test wallet needs POL (native currency) for gas fees.

### Get Free POL from Faucets

**Polygon Faucet**
1. Visit: https://faucet.polygon.technology/
2. Select "Polygon Amoy" network
3. Paste your test wallet address
4. Click "Submit"
5. Wait 1-2 minutes
6. Receive ~0.5 POL

**Alchemy Faucet**
1. Visit: https://www.alchemy.com/faucets/polygon-amoy
2. Create free account (if needed)
3. Paste your test wallet address
4. Click "Send Me MATIC"
5. Receive ~0.5 POL

### Verify POL Balance

Check your balance on PolygonScan:
```
https://amoy.polygonscan.com/address/YOUR_WALLET_ADDRESS
```

You should see POL balance (e.g., 0.5 POL).

---

## Step 3: Configure CoinPay API

### Set Private Key in Configuration

Edit `CoinPay.Api/appsettings.Development.json`:

```json
{
  "Blockchain": {
    "TestWallet": {
      "PrivateKey": "YOUR_64_CHARACTER_PRIVATE_KEY_HERE",
      "Note": "Test wallet for direct blockchain transfers"
    }
  }
}
```

**OR** Set Environment Variable (More Secure):

```bash
# Windows PowerShell
$env:TEST_WALLET_PRIVATE_KEY="YOUR_64_CHARACTER_PRIVATE_KEY_HERE"

# Windows CMD
set TEST_WALLET_PRIVATE_KEY=YOUR_64_CHARACTER_PRIVATE_KEY_HERE

# Linux/Mac
export TEST_WALLET_PRIVATE_KEY="YOUR_64_CHARACTER_PRIVATE_KEY_HERE"
```

⚠️ **IMPORTANT**:
- Never commit private keys to git
- Use test wallets only
- appsettings.Development.json is in .gitignore

---

## Step 4: Start the API

```bash
cd CoinPay.Api
dotnet run
```

Expected logs:
```
DirectTransferService initialized. Test wallet: 0x...
```

If you see this error:
```
Test wallet private key not configured
```
→ Go back to Step 3 and configure the private key correctly.

---

## Step 5: Test POL Transfer

### Using Swagger UI (Easiest)

1. Open browser: http://localhost:5100/swagger
2. Find endpoint: `POST /api/wallet/transfer`
3. Click "Try it out"
4. Enter request body:

```json
{
  "fromWalletAddress": "0xYourTestWalletAddress",
  "toWalletAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
  "amount": 0.001,
  "currency": "POL",
  "memo": "Test POL transfer"
}
```

5. Click "Execute"

### Using curl

```bash
curl -X POST http://localhost:5100/api/wallet/transfer \
  -H "Content-Type: application/json" \
  -d '{
    "fromWalletAddress": "0xYourTestWalletAddress",
    "toWalletAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
    "amount": 0.001,
    "currency": "POL",
    "memo": "Test POL transfer"
  }'
```

### Expected Response

```json
{
  "transactionId": "0x1234...abcd",
  "status": "Confirmed",
  "amount": 0.001,
  "fromAddress": "0xYourTestWalletAddress",
  "toAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
  "initiatedAt": "2025-11-03T10:30:00Z"
}
```

The `transactionId` is the blockchain transaction hash!

---

## Step 6: Verify on PolygonScan

1. Copy the transaction hash from the response
2. Visit PolygonScan:
   ```
   https://amoy.polygonscan.com/tx/YOUR_TRANSACTION_HASH
   ```

3. Verify transaction details:
   - ✅ Status: Success
   - ✅ From: Your test wallet address
   - ✅ To: `0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579`
   - ✅ Value: 0.001 POL
   - ✅ Gas fee paid

4. Check receiver's address:
   ```
   https://amoy.polygonscan.com/address/0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
   ```

---

## Testing USDC Transfers

To test USDC transfers, you need USDC in your test wallet.

### Get Testnet USDC

**Circle Faucet**
1. Visit: https://faucet.circle.com/
2. Select "Polygon Amoy"
3. Paste your test wallet address
4. Receive 10 USDC

### Transfer USDC

```json
{
  "fromWalletAddress": "0xYourTestWalletAddress",
  "toWalletAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
  "amount": 1.5,
  "currency": "USDC",
  "memo": "Test USDC transfer"
}
```

---

## Troubleshooting

### Error: "Test wallet private key not configured"

**Solution**: Set private key in appsettings.Development.json or as environment variable.

### Error: "Insufficient POL balance"

**Solution**:
- Get more POL from faucets (Step 2)
- Check your balance on PolygonScan
- Ensure you have at least 0.01 POL for gas

### Error: "Insufficient USDC balance"

**Solution**:
- Get testnet USDC from Circle faucet
- Check USDC balance on PolygonScan

### Transaction Pending Forever

**Possible Causes**:
- Network congestion
- Gas price too low
- RPC node issues

**Solution**:
1. Wait 2-5 minutes
2. Check transaction on PolygonScan
3. Check network status: https://status.polygon.technology/
4. Try again with higher gas

### Error: "Invalid recipient address format"

**Solution**:
- Ensure address starts with "0x"
- Must be 42 characters long (0x + 40 hex characters)
- Use checksummed address

### DirectTransferService Not Found

**Solution**:
- Ensure you ran `dotnet build` successfully
- Check Program.cs has `AddScoped<IDirectTransferService, DirectTransferService>()`
- Restart the API

---

## API Logs to Check

**Successful Transfer:**
```
[INFO] Sending 0.001 POL from 0x... to 0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
[INFO] Sending transaction with gas price: 1500000000 Wei, gas limit: 21000
[INFO] Transaction sent successfully. TxHash: 0x...
[INFO] Transaction receipt received after 3 attempts
[INFO] Real blockchain transfer completed. TxHash: 0x..., Status: Confirmed
```

**Failed Transfer:**
```
[ERROR] Failed to send POL transaction to 0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
[ERROR] Insufficient funds for gas * price + value
```

---

## Important Notes

### Gas Fees
- POL transfers: ~21,000 gas (~$0.001)
- USDC transfers: ~65,000 gas (~$0.003)
- Gas is paid from sender's POL balance

### Transaction Times
- Polygon Amoy: 2-5 seconds average
- Can take up to 30 seconds during congestion

### Security
- ⚠️ Never commit private keys to git
- ⚠️ Use test wallets only
- ⚠️ Don't use real funds on testnet
- ⚠️ Private keys in environment variables are more secure than config files

### Testnet vs Mainnet
- This guide is for TESTNET only (Polygon Amoy)
- Testnet POL/USDC has no real value
- For production, use Circle's passkey-based flow

---

## Next Steps

### Frontend Implementation

To enable end-users to sign their own transactions with passkeys:

1. Implement Circle passkey challenge flow
2. Add transaction signing UI
3. Submit signed transactions to backend
4. Backend uses Circle Transaction API

See: https://developers.circle.com/w3s/docs/user-controlled-initialization

### Production Deployment

1. Remove DirectTransferService (test-only)
2. Use Circle's custody solution
3. Implement proper passkey flow
4. Deploy to Polygon mainnet

---

## Quick Reference

### Polygon Amoy Testnet

```
Network Name: Polygon Amoy Testnet
RPC URL: https://rpc-amoy.polygon.technology/
Chain ID: 80002
Currency: POL (MATIC)
Block Explorer: https://amoy.polygonscan.com/
```

### USDC Contract Address

```
0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582
```

### Test Recipient Address

```
0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
```

### Useful Commands

```bash
# Build project
dotnet build

# Run API
dotnet run

# Check logs
dotnet run | grep "DirectTransfer"

# Set environment variable (Windows PowerShell)
$env:TEST_WALLET_PRIVATE_KEY="your_key_here"
```

---

## Support

- Polygon Discord: https://discord.gg/polygon
- Circle Discord: https://discord.gg/buildoncircle
- PolygonScan: https://amoy.polygonscan.com/

---

**Last Updated**: 2025-11-03
**Testnet**: Polygon Amoy (Chain ID: 80002)
**Status**: Ready for Testing ✅
