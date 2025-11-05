# CoinPay Testnet Testing Guide

Complete guide for testing cryptocurrency send/receive functionality on testnet using Circle's Programmable Wallets and Polygon Amoy testnet.

## ✅ Latest Update (2025-11-03)

**DirectTransferService is now optional!**

- ✅ **Balance checks work** without any private key configuration
- ✅ **API endpoints work** in read-only mode by default
- ✅ **Mock transfers** return simulated responses for testing
- ⏳ **Real blockchain transfers** require test wallet configuration (see Step 2.3 below)

**Previous Issue Fixed:**
- 409 Conflict error when checking balances - RESOLVED
- DirectTransferService no longer blocks wallet operations
- All read operations work immediately after startup

## Table of Contents
- [Prerequisites](#prerequisites)
- [1. Circle API Setup (Testnet)](#1-circle-api-setup-testnet)
- [2. Configure Application](#2-configure-application)
  - [2.3 Configure Test Wallet for Real Transfers (Optional)](#step-23-configure-test-wallet-for-real-transfers-optional)
- [3. Get Testnet Tokens](#3-get-testnet-tokens)
- [4. Testing Send/Receive in CoinPay](#4-testing-sendreceive-in-coinpay)
- [5. Using MetaMask for External Testing](#5-using-metamask-for-external-testing)
- [6. Testing Checklist](#6-testing-checklist)
- [7. Troubleshooting](#7-troubleshooting)

---

## Prerequisites

- Running CoinPay application (all containers up)
- Circle developer account (free)
- MetaMask wallet (optional, for external testing)
- Basic understanding of blockchain wallets

---

## 1. Circle API Setup (Testnet)

### Step 1.1: Create Circle Developer Account

1. Go to https://console.circle.com/
2. Sign up for a free developer account
3. Verify your email address
4. Log in to Circle Console

### Step 1.2: Create Testnet Application

1. In Circle Console, click **"Create New App"**
2. Choose **"Web3 Services"**
3. Name your app: `CoinPay Testnet`
4. Select environment: **"Testnet"** (important!)
5. Click **"Create Application"**

### Step 1.3: Get API Credentials

After creating the app, you'll see three important credentials:

```
App ID: e.g., "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"
API Key: e.g., "TEST_API_KEY:12345abcde..."
Entity Secret: (encryption key for sensitive operations)
```

**Important**: Copy these immediately - the Entity Secret is only shown once!

### Step 1.4: Enable Required Features

In Circle Console, ensure these features are enabled:
- ✅ Programmable Wallets
- ✅ Smart Contract Accounts (SCA)
- ✅ Polygon Amoy Network
- ✅ USDC Token

---

## 2. Configure Application

### Step 2.1: Update API Configuration

Edit `CoinPay.Api/appsettings.Development.json`:

```json
{
  "Circle": {
    "ApiUrl": "https://api.circle.com/v1/w3s",
    "BundlerUrl": "https://bundler.circle.com",
    "PaymasterUrl": "https://paymaster.circle.com",
    "ApiKey": "TEST_API_KEY:your_actual_api_key_here",
    "EntitySecret": "your_actual_entity_secret_here",
    "AppId": "your_actual_app_id_here"
  }
}
```

Replace the placeholder values with your actual Circle credentials from Step 1.3.

### Step 2.2: Verify Configuration

```bash
# Restart the API container to load new configuration
docker-compose restart api

# Check logs to ensure Circle API is configured
docker-compose logs -f api
```

You should see logs indicating Circle API client initialized successfully.

### Step 2.3: Configure Test Wallet for Real Transfers (Optional)

**Note**: This step is OPTIONAL. Balance checks and mock transfers work without this configuration.

**When you need this**:
- Testing real POL/USDC blockchain transfers
- Verifying transactions on PolygonScan
- End-to-end blockchain testing

**Step 2.3.1: Create Test Wallet**

Create a new wallet in MetaMask:
1. Install MetaMask browser extension
2. Create new account → Name it "CoinPay Test"
3. Add Polygon Amoy network (see Step 5.1 for details)
4. Get free testnet POL: https://faucet.polygon.technology/
5. Export private key: Account Details → Export Private Key

**Step 2.3.2: Configure Private Key**

**Method 1: Environment Variable (Recommended)**

```powershell
# Windows PowerShell
$env:TEST_WALLET_PRIVATE_KEY = "your_64_character_private_key_here"
```

```bash
# Linux/Mac
export TEST_WALLET_PRIVATE_KEY="your_64_character_private_key_here"
```

**Method 2: Docker Environment**

Edit `docker-compose.yml`:

```yaml
services:
  api:
    environment:
      - TEST_WALLET_PRIVATE_KEY=your_private_key_here
```

**Method 3: Configuration File**

Edit `CoinPay.Api/appsettings.Development.json`:

```json
{
  "Blockchain": {
    "TestWallet": {
      "PrivateKey": "your_private_key_here"
    }
  }
}
```

⚠️ **SECURITY**: Never commit private keys to git! Use environment variables for sensitive data.

**Step 2.3.3: Restart and Verify**

```bash
# Rebuild and restart
docker-compose down
docker-compose build api
docker-compose up -d

# Check if DirectTransferService registered
docker logs coinpay-api | grep DirectTransferService
```

**Expected output (with valid key):**
```
[INFO] DirectTransferService registered with test wallet
```

**Expected output (without key - this is OK):**
```
[INFO] DirectTransferService NOT registered - no valid private key configured
[INFO] Wallet transfers will use mock responses
```

---

## 3. Get Testnet Tokens

### Step 3.1: Understanding Polygon Amoy Testnet

- **Network Name**: Polygon Amoy
- **Chain ID**: 80002
- **RPC URL**: https://rpc-amoy.polygon.technology/
- **Block Explorer**: https://amoy.polygonscan.com/
- **Currency**: MATIC (for gas fees)
- **Token**: USDC (Circle's testnet USDC)

### Step 3.2: Get Testnet MATIC (Gas Fees)

Testnet MATIC is required for transaction gas fees.

**Option 1: Polygon Faucet**
1. Go to https://faucet.polygon.technology/
2. Select **"Polygon Amoy"** network
3. Enter your wallet address (from CoinPay app or MetaMask)
4. Complete CAPTCHA
5. Click **"Submit"**
6. Wait 1-2 minutes for MATIC to arrive

**Option 2: Alchemy Faucet**
1. Go to https://www.alchemy.com/faucets/polygon-amoy
2. Create free Alchemy account (if needed)
3. Enter your wallet address
4. Click **"Send Me MATIC"**
5. Receive 0.5 MATIC (enough for ~100 transactions)

### Step 3.3: Get Testnet USDC

**Option 1: Circle Testnet Faucet**
1. Go to https://faucet.circle.com/
2. Select **"Polygon Amoy"** network
3. Enter your wallet address
4. Click **"Receive Test USDC"**
5. You'll receive 10 USDC (testnet)

**Option 2: Aave Faucet**
1. Go to https://staging.aave.com/faucet/
2. Connect your wallet (MetaMask)
3. Select **"Polygon Amoy"** network
4. Click **"Faucet"** for USDC
5. Approve transaction in MetaMask

### Step 3.4: Verify Token Receipt

Check your tokens on Polygon Amoy explorer:
```
https://amoy.polygonscan.com/address/YOUR_WALLET_ADDRESS
```

You should see:
- MATIC balance (e.g., 0.5 MATIC)
- USDC token balance (e.g., 10 USDC)

---

## 4. Testing Send/Receive in CoinPay

### Step 4.1: Create Test Users

1. Open CoinPay frontend: http://localhost:3000
2. Register two test accounts:
   - User A: `alice@test.com` / `password123`
   - User B: `bob@test.com` / `password123`

### Step 4.2: Initialize Wallets

For each user:
1. Log in
2. Navigate to **Wallet** page
3. Click **"Create Wallet"** or **"Initialize Wallet"**
4. Complete Circle's wallet creation flow
5. Copy your wallet address (starts with `0x...`)

### Step 4.3: Fund Wallets

Use the faucets from Step 3 to send tokens to each wallet:
- Alice's wallet: Get 10 USDC + 0.5 MATIC
- Bob's wallet: Get 10 USDC + 0.5 MATIC

### Step 4.4: Test Receiving (Deposit)

**Scenario**: Send USDC to your CoinPay wallet from external source

1. Log in as Alice
2. Go to **Wallet** page
3. Copy Alice's wallet address
4. Use MetaMask or another wallet to send 5 USDC to Alice's address
5. Wait for transaction confirmation (~30 seconds)
6. Refresh CoinPay wallet - balance should update

### Step 4.5: Test Sending (Withdrawal)

**Scenario**: Send USDC from CoinPay to external wallet

1. Log in as Alice (sender)
2. Ensure Alice has USDC balance > 0
3. Go to **Wallet** → **Withdraw**
4. Enter Bob's wallet address as recipient
5. Enter amount: `2.5 USDC`
6. Review fee breakdown
7. Confirm transaction
8. Wait for confirmation
9. Check transaction on Polygon Amoy explorer

### Step 4.6: Test Peer-to-Peer Transfer

**Scenario**: Transfer between CoinPay users

1. Log in as Alice
2. Go to **Transactions** → **Send**
3. Enter Bob's email or wallet address
4. Amount: `1 USDC`
5. Add note: "Test P2P transfer"
6. Confirm transaction
7. Log in as Bob
8. Check transaction history
9. Verify balance updated

### Step 4.7: Test Fiat Off-Ramp (Withdrawal to Bank)

**Scenario**: Convert USDC to fiat and withdraw

1. Log in as Alice
2. Go to **Wallet** → **Withdraw to Bank**
3. Add bank account (use test bank details)
4. Enter withdrawal amount: `5 USDC`
5. Review fee breakdown:
   - Conversion fee (1.5%)
   - Payout fee ($1.00)
   - Net amount received
6. Confirm withdrawal
7. Check payout status
8. Verify payout appears in history

---

## 5. Using MetaMask for External Testing

### Step 5.1: Add Polygon Amoy to MetaMask

1. Open MetaMask browser extension
2. Click network dropdown (top)
3. Click **"Add Network"** → **"Add a network manually"**
4. Enter network details:

```
Network Name: Polygon Amoy Testnet
New RPC URL: https://rpc-amoy.polygon.technology/
Chain ID: 80002
Currency Symbol: MATIC
Block Explorer URL: https://amoy.polygonscan.com/
```

5. Click **"Save"**

### Step 5.2: Import Test Account

**Option A: Use generated wallet from CoinPay**
1. Export private key from CoinPay (if feature exists)
2. MetaMask → Click account icon → Import Account
3. Paste private key
4. Click **"Import"**

**Option B: Use MetaMask wallet as external sender**
1. Keep your MetaMask wallet separate
2. Use faucets to fund it
3. Send to CoinPay wallet addresses

### Step 5.3: Add USDC Token to MetaMask

1. In MetaMask on Polygon Amoy network
2. Scroll down → Click **"Import tokens"**
3. Click **"Custom token"**
4. Enter USDC contract address (Polygon Amoy):
   ```
   0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582
   ```
5. Token Symbol: `USDC`
6. Decimals: `6`
7. Click **"Add custom token"**
8. Click **"Import tokens"**

Now you can see your USDC balance in MetaMask!

### Step 5.4: Send Test Transaction from MetaMask

1. Open MetaMask
2. Ensure you're on **Polygon Amoy** network
3. Click **"Send"**
4. Recipient: Enter your CoinPay wallet address
5. Asset: Select **"USDC"**
6. Amount: `5 USDC`
7. Click **"Next"** → **"Confirm"**
8. Wait for confirmation (check on https://amoy.polygonscan.com/)

---

## 6. Testing Checklist

Use this checklist to ensure comprehensive testing:

### Wallet Operations
- [ ] Create new wallet via Circle API
- [ ] Display wallet address
- [ ] Check balance (USDC)
- [ ] Check balance (MATIC for gas)
- [ ] Refresh balance manually
- [ ] Auto-refresh balance (polling)

### Receiving Transactions
- [ ] Receive USDC from external wallet (MetaMask)
- [ ] Receive USDC from another CoinPay user
- [ ] Transaction appears in history
- [ ] Balance updates correctly
- [ ] Transaction status updates (pending → completed)
- [ ] Blockchain transaction hash displayed

### Sending Transactions
- [ ] Send USDC to external wallet address
- [ ] Send USDC to another CoinPay user (P2P)
- [ ] Send with memo/note
- [ ] Insufficient balance error handling
- [ ] Invalid address error handling
- [ ] Gas fee estimation
- [ ] Transaction confirmation flow

### Fiat Off-Ramp (Bank Withdrawal)
- [ ] Add bank account
- [ ] Verify bank account
- [ ] Initiate USDC → USD conversion
- [ ] See fee breakdown (conversion + payout fee)
- [ ] Confirm effective fee rate
- [ ] Payout status tracking
- [ ] Cancel pending payout
- [ ] View payout history
- [ ] Payout audit trail

### Transaction Monitoring
- [ ] View transaction list
- [ ] Filter by status (pending/completed/failed)
- [ ] Filter by type (payment/transfer/withdrawal)
- [ ] View transaction details
- [ ] See blockchain explorer link
- [ ] Transaction timestamp displayed

### Error Handling
- [ ] Network error handling
- [ ] Insufficient gas (MATIC)
- [ ] Insufficient balance (USDC)
- [ ] Invalid recipient address
- [ ] Transaction timeout
- [ ] Failed transaction status

---

## 7. Troubleshooting

### Problem: 409 Conflict - "Invalid private key configuration" (FIXED)

**Symptom:**
```json
{
  "status": 409,
  "title": "Invalid operation",
  "developerMessage": "Invalid private key configuration"
}
```

**Status:** ✅ **FIXED as of 2025-11-03**

**What was wrong:**
DirectTransferService was required but tried to initialize with placeholder private key, breaking all wallet operations including balance checks.

**How it's fixed:**
DirectTransferService is now optional and only registers when a valid private key is configured. Balance checks and read operations work immediately without configuration.

**Verification:**
```bash
# Check API logs
docker logs coinpay-api | grep DirectTransferService

# Should see one of:
# [INFO] DirectTransferService registered with test wallet (if configured)
# [INFO] DirectTransferService NOT registered - no valid private key configured (default)
```

**If you still see this error:**
1. Pull latest code
2. Rebuild containers: `docker-compose build api`
3. Restart: `docker-compose up -d`

### Problem: "Circle API Key Invalid"

**Solution:**
1. Verify API key in `appsettings.Development.json`
2. Ensure you're using **Testnet** credentials (not production)
3. Check Circle Console - key should start with `TEST_API_KEY:`
4. Restart API container: `docker-compose restart api`

### Problem: "Insufficient Funds" when sending

**Possible causes:**
1. **Not enough USDC**: Get more from USDC faucet
2. **Not enough MATIC for gas**: Get MATIC from Polygon faucet
3. **Balance not synced**: Click refresh balance

**Solution:**
```bash
# Check wallet balance on blockchain
# Visit: https://amoy.polygonscan.com/address/YOUR_ADDRESS

# Get more MATIC
https://faucet.polygon.technology/

# Get more USDC
https://faucet.circle.com/
```

### Problem: "Transaction Pending Forever"

**Causes:**
- Network congestion
- Gas price too low
- RPC node issues

**Solution:**
1. Check transaction on explorer: https://amoy.polygonscan.com/
2. Wait 2-5 minutes for testnet confirmation
3. If still pending after 10 minutes, check Circle status: https://status.circle.com/
4. Restart transaction monitoring service:
   ```bash
   docker-compose restart api
   ```

### Problem: "Wallet Creation Failed"

**Causes:**
- Circle API credentials incorrect
- Entity secret missing or wrong
- Network connectivity issues

**Solution:**
1. Check API logs:
   ```bash
   docker-compose logs -f api | grep Circle
   ```
2. Verify all three Circle credentials are set:
   - ApiKey
   - EntitySecret
   - AppId
3. Test Circle API manually:
   ```bash
   curl -X GET https://api.circle.com/v1/w3s/config/entity \
     -H "Authorization: Bearer YOUR_API_KEY"
   ```

### Problem: "Transaction Not Appearing in History"

**Causes:**
- Background monitoring service not running
- Database sync issue
- Transaction not confirmed on blockchain

**Solution:**
1. Check monitoring service:
   ```bash
   docker-compose logs -f api | grep TransactionMonitoring
   ```
2. Manually refresh balance (triggers sync)
3. Check database:
   ```bash
   docker exec -it coinpay-postgres psql -U coinpay -d coinpay_db
   SELECT * FROM "Transactions" ORDER BY "CreatedAt" DESC LIMIT 10;
   ```

### Problem: "RPC Error" or "Network Error"

**Causes:**
- Polygon Amoy RPC node down
- Network connectivity

**Solution:**
1. Check Polygon network status: https://status.polygon.technology/
2. Try alternative RPC:
   - Primary: `https://rpc-amoy.polygon.technology/`
   - Backup: `https://polygon-amoy.g.alchemy.com/v2/demo`
   - Backup: `https://polygon-amoy-bor.publicnode.com`
3. Update RPC in Circle configuration if needed

### Problem: "Cannot Add USDC Token to MetaMask"

**Solution:**
Use correct USDC contract address for Polygon Amoy:
```
Contract: 0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582
Symbol: USDC
Decimals: 6
```

### Problem: "Balance Shows 0 After Receiving Tokens"

**Solution:**
1. Verify transaction confirmed: https://amoy.polygonscan.com/
2. Click **Refresh Balance** in CoinPay
3. Check API logs for sync errors
4. Manually query balance:
   ```bash
   # Check via API
   curl http://localhost:7777/api/wallet/balance \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"
   ```

---

## Quick Reference

### Testnet Resources

| Resource | URL |
|----------|-----|
| Circle Console | https://console.circle.com/ |
| Circle Docs | https://developers.circle.com/w3s/docs |
| Polygon Faucet | https://faucet.polygon.technology/ |
| USDC Faucet | https://faucet.circle.com/ |
| Block Explorer | https://amoy.polygonscan.com/ |
| Network Status | https://status.polygon.technology/ |
| Circle Status | https://status.circle.com/ |

### Network Details

```
Network: Polygon Amoy Testnet
Chain ID: 80002
RPC: https://rpc-amoy.polygon.technology/
Currency: MATIC
USDC Contract: 0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582
```

### Useful Commands

```bash
# Check container status
docker-compose ps

# View API logs
docker-compose logs -f api

# Restart services
docker-compose restart

# Check database
docker exec -it coinpay-postgres psql -U coinpay -d coinpay_db

# Check wallet balance
SELECT * FROM "Wallets";

# Check transactions
SELECT * FROM "Transactions" ORDER BY "CreatedAt" DESC;

# Check blockchain transactions
SELECT * FROM "BlockchainTransactions" ORDER BY "CreatedAt" DESC;
```

---

## Next Steps

After completing testnet testing:

1. **Document Issues**: Keep a log of any bugs or unexpected behavior
2. **Test Edge Cases**: Try invalid inputs, network failures, etc.
3. **Performance Testing**: Send multiple transactions, test concurrent users
4. **Security Testing**: Test authentication, authorization, input validation
5. **Production Planning**: Update configuration for mainnet when ready

---

## Need Help?

- Circle Developer Discord: https://discord.gg/buildoncircle
- Polygon Discord: https://discord.gg/polygon
- CoinPay Issues: https://github.com/your-repo/issues
- Developer Email: developerhakart@yahoo.com

---

**Last Updated**: 2025-11-03
**Testnet**: Polygon Amoy (Chain ID: 80002)
**Circle Environment**: Testnet
**Status**: ✅ DirectTransferService optional, balance checks work immediately
