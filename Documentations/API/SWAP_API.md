# Token Swap API

Complete reference for CoinPay Token Swap API - powered by 1inch DEX aggregator for best exchange rates.

## Overview

The Swap API enables users to exchange tokens at competitive rates through decentralized exchanges (DEX). CoinPay integrates with 1inch Protocol to find the best rates across multiple DEXs.

## Base URL

```
Production: https://api.coinpay.com
Testnet:    http://localhost:7777
```

## Authentication

All Swap API endpoints require JWT authentication.

## Endpoints

### Get Swap Quote

Get a quote for swapping one token to another.

**Endpoint:** `GET /api/swap/quote`

**Authentication:** Required (JWT)

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| fromToken | string | Yes | Source token symbol (USDC, POL, WETH, etc.) |
| toToken | string | Yes | Destination token symbol |
| amount | string | Yes | Amount to swap (in source token units) |
| slippage | string | No | Max slippage percentage (default: 0.5) |

**Request Example:**
```bash
curl -X GET "http://localhost:7777/api/swap/quote?fromToken=USDC&toToken=WETH&amount=100&slippage=0.5" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:** `200 OK`
```json
{
  "fromToken": "USDC",
  "toToken": "WETH",
  "fromAmount": "100.000000",
  "toAmount": "0.027123456789",
  "exchangeRate": "0.00027123456789",
  "slippage": "0.5",
  "minimumReceived": "0.026987654321",
  "priceImpact": "0.15",
  "platformFee": "0.50",
  "estimatedGas": "0.002",
  "route": [
    {
      "exchange": "QuickSwap",
      "percentage": 60,
      "path": ["USDC", "WMATIC", "WETH"]
    },
    {
      "exchange": "SushiSwap",
      "percentage": 40,
      "path": ["USDC", "WETH"]
    }
  ],
  "validUntil": "2025-11-07T14:35:00Z"
}
```

### Execute Swap

Execute a token swap transaction.

**Endpoint:** `POST /api/swap/execute`

**Authentication:** Required (JWT)

**Request Body:**
```json
{
  "fromToken": "USDC",
  "toToken": "WETH",
  "amount": "100",
  "slippage": "0.5",
  "quoteId": "quote_abc123xyz"
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| fromToken | string | Yes | Source token symbol |
| toToken | string | Yes | Destination token symbol |
| amount | string | Yes | Amount to swap |
| slippage | string | No | Max slippage (default: 0.5%) |
| quoteId | string | No | Quote ID from /quote endpoint |

**Response:** `201 Created`
```json
{
  "id": 456,
  "swapId": "swap_xyz789abc",
  "fromToken": "USDC",
  "toToken": "WETH",
  "fromAmount": "100.000000",
  "toAmount": "0.027123456789",
  "status": "Pending",
  "txHash": null,
  "platformFee": "0.50",
  "createdAt": "2025-11-07T14:30:00Z",
  "completedAt": null
}
```

### Get Swap History

Retrieve swap transaction history.

**Endpoint:** `GET /api/swap/history`

**Authentication:** Required (JWT)

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 20 | Items per page (max 100) |
| status | string | - | Filter by status (Pending, Completed, Failed) |
| fromToken | string | - | Filter by source token |
| toToken | string | - | Filter by destination token |

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": 456,
      "swapId": "swap_xyz789abc",
      "fromToken": "USDC",
      "toToken": "WETH",
      "fromAmount": "100.000000",
      "toAmount": "0.027123456789",
      "status": "Completed",
      "txHash": "0x1234567890abcdef...",
      "platformFee": "0.50",
      "createdAt": "2025-11-07T14:30:00Z",
      "completedAt": "2025-11-07T14:32:15Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "totalPages": 3,
    "totalItems": 45
  }
}
```

### Get Swap Details

Get details of a specific swap transaction.

**Endpoint:** `GET /api/swap/{id}`

**Authentication:** Required (JWT)

**Response:** `200 OK`
```json
{
  "id": 456,
  "swapId": "swap_xyz789abc",
  "fromToken": "USDC",
  "toToken": "WETH",
  "fromAmount": "100.000000",
  "toAmount": "0.027123456789",
  "status": "Completed",
  "txHash": "0x1234567890abcdef...",
  "platformFee": "0.50",
  "route": [
    {
      "exchange": "QuickSwap",
      "percentage": 60
    }
  ],
  "createdAt": "2025-11-07T14:30:00Z",
  "completedAt": "2025-11-07T14:32:15Z"
}
```

## Supported Tokens

All ERC-20 tokens on Polygon Amoy:

| Token | Symbol | Contract Address |
|-------|--------|------------------|
| USD Coin | USDC | 0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582 |
| Polygon | POL | 0x0000000000000000000000000000000000001010 |
| Dai Stablecoin | DAI | 0x001B3B4d0F3714Ca98ba10F6042DaEbF0B1B7b6F |
| Tether USD | USDT | 0xA02f6adc7926efeBBd59Fd43A84f4E0c0c91e832 |
| Wrapped BTC | WBTC | 0x97e8dE167322a3bCA28E8A49BC46F6Ce128FEC68 |
| Wrapped Ether | WETH | 0x7ceB23fD6bC0adD59E62ac25578270cFf1b9f619 |

## Code Examples

### Node.js - Get Quote and Execute Swap

```javascript
const axios = require('axios');

async function performSwap(token) {
  try {
    // 1. Get quote
    const quoteResponse = await axios.get(
      'http://localhost:7777/api/swap/quote',
      {
        params: {
          fromToken: 'USDC',
          toToken: 'WETH',
          amount: '100',
          slippage: '0.5'
        },
        headers: { 'Authorization': `Bearer ${token}` }
      }
    );

    const quote = quoteResponse.data;
    console.log(`Quote: ${quote.fromAmount} ${quote.fromToken} → ${quote.toAmount} ${quote.toToken}`);

    // 2. Execute swap
    const swapResponse = await axios.post(
      'http://localhost:7777/api/swap/execute',
      {
        fromToken: 'USDC',
        toToken: 'WETH',
        amount: '100',
        slippage: '0.5'
      },
      {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }
    );

    console.log('Swap executed:', swapResponse.data);
    return swapResponse.data;
  } catch (error) {
    console.error('Swap error:', error.response?.data || error.message);
    throw error;
  }
}
```

## Best Practices

1. **Always get a quote first** before executing swap
2. **Set appropriate slippage** (0.5-1% for stable pairs, 1-3% for volatile pairs)
3. **Check minimum received** amount matches expectations
4. **Monitor swap status** via webhooks or polling
5. **Handle failed swaps** gracefully with retry logic

## Rate Limits

| Endpoint | Rate Limit |
|----------|------------|
| GET /api/swap/quote | 30 requests/minute |
| POST /api/swap/execute | 10 requests/minute |
| GET /api/swap/history | 30 requests/minute |

## Troubleshooting

**Issue:** "Insufficient liquidity" error
**Solution:** Try smaller amount or different token pair

**Issue:** Swap pending for too long
**Solution:** Check blockchain explorer with txHash

---

**Last Updated:** November 7, 2025
