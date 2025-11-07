# Wallet API

Complete reference for CoinPay Wallet API - manage crypto wallets, check balances, and handle wallet operations.

## Overview

The Wallet API provides endpoints for creating and managing cryptocurrency wallets on the Polygon network. Each user has a Circle-powered wallet that supports multiple ERC-20 tokens.

## Base URL

```
Production: https://api.coinpay.com
Testnet:    http://localhost:7777
```

## Authentication

All Wallet API endpoints require JWT authentication. Include the token in the `Authorization` header:

```
Authorization: Bearer YOUR_JWT_TOKEN
```

## Endpoints

### Get Wallet Details

Retrieve complete wallet information for the authenticated user.

**Endpoint:** `GET /api/wallet`

**Authentication:** Required (JWT)

**Request Example:**
```bash
curl -X GET http://localhost:7777/api/wallet \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "address": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
  "circleWalletId": "fef70777-cb2d-5096-a0ea-15dba5662ce6",
  "blockchain": "MATIC-AMOY",
  "balance": {
    "USDC": "1250.50",
    "POL": "125.75",
    "MATIC": "125.75",
    "DAI": "500.00",
    "USDT": "750.25",
    "WETH": "0.5",
    "WBTC": "0.025"
  },
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-11-07T14:30:00Z"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| id | integer | Internal wallet ID |
| userId | string (UUID) | User ID (GUID format) |
| address | string | Ethereum-compatible wallet address (0x...) |
| circleWalletId | string (UUID) | Circle W3S wallet identifier |
| blockchain | string | Blockchain network (MATIC-AMOY for testnet) |
| balance | object | Token balances (key: currency, value: amount) |
| createdAt | string (ISO 8601) | Wallet creation timestamp |
| updatedAt | string (ISO 8601) | Last update timestamp |

**Error Responses:**

| Status Code | Description |
|-------------|-------------|
| 401 | Unauthorized - Invalid or missing JWT token |
| 404 | Not Found - Wallet not found for user |
| 500 | Internal Server Error - Server error |

### Get Wallet Balance

Get current balance for all supported tokens.

**Endpoint:** `GET /api/wallet/balance`

**Authentication:** Required (JWT)

**Request Example:**
```bash
curl -X GET http://localhost:7777/api/wallet/balance \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:** `200 OK`
```json
{
  "address": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
  "balances": [
    {
      "token": "USDC",
      "amount": "1250.50",
      "usdValue": "1250.50",
      "symbol": "USDC",
      "decimals": 6,
      "contractAddress": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582"
    },
    {
      "token": "POL",
      "amount": "125.75",
      "usdValue": "63.50",
      "symbol": "POL",
      "decimals": 18,
      "contractAddress": "0x0000000000000000000000000000000000001010"
    },
    {
      "token": "WETH",
      "amount": "0.5",
      "usdValue": "1850.00",
      "symbol": "WETH",
      "decimals": 18,
      "contractAddress": "0x7ceB23fD6bC0adD59E62ac25578270cFf1b9f619"
    }
  ],
  "totalUsdValue": "3164.00",
  "lastUpdated": "2025-11-07T14:30:00Z"
}
```

**Query Parameters (Optional):**

| Parameter | Type | Description |
|-----------|------|-------------|
| currency | string | Filter by specific currency (USDC, POL, etc.) |

**Example with Filter:**
```bash
GET /api/wallet/balance?currency=USDC
```

### Create Wallet (Admin/System)

Create a new wallet for a user. This endpoint is typically called automatically during user registration.

**Endpoint:** `POST /api/wallet`

**Authentication:** Required (JWT) + Admin role

**Request Body:**
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "blockchain": "MATIC-AMOY"
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| userId | string (UUID) | Yes | User ID to associate wallet with |
| blockchain | string | No | Blockchain network (default: MATIC-AMOY) |

**Response:** `201 Created`
```json
{
  "id": 1,
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "address": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
  "circleWalletId": "fef70777-cb2d-5096-a0ea-15dba5662ce6",
  "blockchain": "MATIC-AMOY",
  "createdAt": "2025-11-07T14:30:00Z"
}
```

### Get Wallet Transactions

Get transaction history for the wallet.

**Endpoint:** `GET /api/wallet/transactions`

**Authentication:** Required (JWT)

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number (1-based) |
| limit | integer | 20 | Items per page (max 100) |
| currency | string | - | Filter by currency |
| type | string | - | Filter by type (sent, received, all) |
| startDate | string | - | Start date (ISO 8601) |
| endDate | string | - | End date (ISO 8601) |

**Request Example:**
```bash
curl -X GET "http://localhost:7777/api/wallet/transactions?page=1&limit=20&currency=USDC" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": 123,
      "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
      "type": "sent",
      "amount": "100.50",
      "currency": "USDC",
      "status": "Completed",
      "recipient": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
      "txHash": "0x1234567890abcdef...",
      "fee": "0.50",
      "createdAt": "2025-11-07T14:30:00Z",
      "completedAt": "2025-11-07T14:32:15Z"
    },
    {
      "id": 122,
      "transactionId": "550c8ba1-d392-558b-80ff-a7d3eaeca601",
      "type": "received",
      "amount": "250.00",
      "currency": "USDC",
      "status": "Completed",
      "sender": "0x1234567890abcdef1234567890abcdef12345678",
      "txHash": "0xabcdef1234567890...",
      "fee": "0.00",
      "createdAt": "2025-11-06T10:15:00Z",
      "completedAt": "2025-11-06T10:17:30Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "totalPages": 5,
    "totalItems": 95
  }
}
```

## Supported Tokens

| Token | Symbol | Network | Contract Address | Decimals |
|-------|--------|---------|------------------|----------|
| USD Coin | USDC | Polygon Amoy | 0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582 | 6 |
| Polygon | POL | Polygon Amoy | 0x0000000000000000000000000000000000001010 | 18 |
| Matic | MATIC | Polygon Amoy | 0x0000000000000000000000000000000000001010 | 18 |
| Dai Stablecoin | DAI | Polygon Amoy | 0x001B3B4d0F3714Ca98ba10F6042DaEbF0B1B7b6F | 18 |
| Tether USD | USDT | Polygon Amoy | 0xA02f6adc7926efeBBd59Fd43A84f4E0c0c91e832 | 6 |
| Wrapped BTC | WBTC | Polygon Amoy | 0x97e8dE167322a3bCA28E8A49BC46F6Ce128FEC68 | 8 |
| Wrapped Ether | WETH | Polygon Amoy | 0x7ceB23fD6bC0adD59E62ac25578270cFf1b9f619 | 18 |

## Testnet Faucets

Get free testnet tokens:

| Token | Faucet URL |
|-------|------------|
| MATIC/POL | https://faucet.polygon.technology/ |
| USDC | https://faucet.circle.com/ |

## Code Examples

### Node.js - Get Wallet Balance

```javascript
const axios = require('axios');

async function getWalletBalance(token) {
  try {
    const response = await axios.get(
      'http://localhost:7777/api/wallet/balance',
      {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      }
    );

    console.log('Wallet Balance:', response.data);
    console.log(`Total USD Value: $${response.data.totalUsdValue}`);

    // Display each token balance
    response.data.balances.forEach(balance => {
      console.log(`${balance.token}: ${balance.amount} ($${balance.usdValue})`);
    });

    return response.data;
  } catch (error) {
    console.error('Error fetching balance:', error.response?.data || error.message);
    throw error;
  }
}
```

### Python - Get Wallet Details

```python
import requests

def get_wallet_details(token):
    url = "http://localhost:7777/api/wallet"
    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.get(url, headers=headers)

    if response.status_code == 200:
        wallet = response.json()
        print(f"Wallet Address: {wallet['address']}")
        print(f"Circle Wallet ID: {wallet['circleWalletId']}")
        print(f"Balances: {wallet['balance']}")
        return wallet
    else:
        print(f"Error: {response.json()}")
        raise Exception("Failed to fetch wallet details")
```

### C# - Get Wallet Balance

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class WalletService
{
    private readonly HttpClient _httpClient;

    public WalletService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WalletBalance> GetBalanceAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(
            "http://localhost:7777/api/wallet/balance"
        );

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var balance = JsonSerializer.Deserialize<WalletBalance>(content);

        Console.WriteLine($"Total USD Value: ${balance.TotalUsdValue}");

        foreach (var tokenBalance in balance.Balances)
        {
            Console.WriteLine($"{tokenBalance.Token}: {tokenBalance.Amount}");
        }

        return balance;
    }
}
```

### cURL - Get Wallet

```bash
# Get wallet details
curl -X GET http://localhost:7777/api/wallet \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get wallet balance
curl -X GET http://localhost:7777/api/wallet/balance \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get wallet balance for specific currency
curl -X GET "http://localhost:7777/api/wallet/balance?currency=USDC" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Webhooks

Subscribe to wallet-related events:

| Event | Description |
|-------|-------------|
| wallet.created | New wallet created |
| wallet.updated | Wallet information updated |
| wallet.balance_changed | Balance updated (incoming/outgoing transaction) |
| wallet.low_balance | Balance below threshold (configurable) |

**Webhook Payload Example:**
```json
{
  "event": "wallet.balance_changed",
  "timestamp": "2025-11-07T14:30:00Z",
  "data": {
    "walletId": 1,
    "address": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579",
    "currency": "USDC",
    "previousBalance": "1150.00",
    "newBalance": "1250.50",
    "change": "+100.50",
    "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602"
  }
}
```

## Best Practices

### 1. Balance Checks

Always check wallet balance before initiating transactions:

```javascript
async function canAffordTransaction(token, amount, currency) {
  const balance = await getWalletBalance(token);
  const tokenBalance = balance.balances.find(b => b.token === currency);

  if (!tokenBalance || parseFloat(tokenBalance.amount) < amount) {
    throw new Error('Insufficient balance');
  }

  return true;
}
```

### 2. Caching

Cache wallet balance for short periods to reduce API calls:

```javascript
const cache = new Map();
const CACHE_TTL = 30000; // 30 seconds

async function getCachedBalance(token) {
  const cached = cache.get('balance');

  if (cached && Date.now() - cached.timestamp < CACHE_TTL) {
    return cached.data;
  }

  const balance = await getWalletBalance(token);
  cache.set('balance', { data: balance, timestamp: Date.now() });

  return balance;
}
```

### 3. Error Handling

Handle common wallet errors gracefully:

```javascript
async function safeGetWallet(token) {
  try {
    return await getWallet(token);
  } catch (error) {
    if (error.response?.status === 404) {
      console.log('Wallet not found - creating new wallet');
      return await createWallet(token);
    }

    if (error.response?.status === 401) {
      console.log('Token expired - re-authenticating');
      const newToken = await refreshToken();
      return await getWallet(newToken);
    }

    throw error; // Re-throw unexpected errors
  }
}
```

### 4. Real-time Updates

Use webhooks for real-time balance updates instead of polling:

```javascript
// ❌ Bad: Polling every second
setInterval(() => getWalletBalance(token), 1000);

// ✅ Good: Use webhooks
app.post('/webhooks/coinpay', (req, res) => {
  if (req.body.event === 'wallet.balance_changed') {
    updateLocalBalance(req.body.data);
  }
  res.status(200).send('OK');
});
```

## Rate Limits

| Endpoint | Rate Limit |
|----------|------------|
| GET /api/wallet | 60 requests/minute |
| GET /api/wallet/balance | 60 requests/minute |
| GET /api/wallet/transactions | 30 requests/minute |
| POST /api/wallet | 5 requests/minute (admin only) |

## Security

### Address Validation

Always validate Ethereum addresses before using them:

```javascript
function isValidAddress(address) {
  return /^0x[a-fA-F0-9]{40}$/.test(address);
}
```

### Private Key Management

**⚠️ CRITICAL:** Never expose or transmit private keys through the API. CoinPay uses Circle's secure key management system. Private keys are never exposed via the API.

## Troubleshooting

### Issue: "Wallet not found" error

**Solution:** Wallet is created automatically during user registration. If missing, contact support or use the create wallet endpoint (admin only).

### Issue: Balance shows zero after receiving funds

**Solution:**
1. Check transaction status on blockchain explorer
2. Wait for blockchain confirmations (2-5 minutes)
3. Try refreshing balance after 30 seconds

### Issue: Incorrect balance display

**Solution:**
1. Clear cache
2. Check for pending transactions
3. Verify token decimal places in your display logic

## Support

For issues or questions:

- **Email:** dev@coinpay.com
- **Discord:** [Join community](https://discord.gg/coinpay)
- **GitHub:** [Report issue](https://github.com/developerhakart/CoinPay/issues)
- **Swagger Documentation:** http://localhost:7777/swagger

---

**Last Updated:** November 7, 2025
**API Version:** 1.0.0
