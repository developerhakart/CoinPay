# CoinPay API Documentation

## Overview

CoinPay provides a comprehensive RESTful API for B2B cryptocurrency payment processing, wallet management, token swaps, and fiat off-ramp capabilities. This documentation is designed for developers integrating CoinPay into their platforms.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Authentication](#authentication)
3. [API Reference](#api-reference)
4. [Integration Guides](#integration-guides)
5. [Webhooks](#webhooks)
6. [SDKs & Libraries](#sdks--libraries)
7. [Support](#support)

## Quick Links

- **API Base URL (Production):** `https://api.coinpay.com`
- **API Base URL (Testnet):** `http://localhost:7777`
- **Swagger Documentation:** `http://localhost:7777/swagger`
- **GitHub Repository:** https://github.com/developerhakart/CoinPay

## Getting Started

### Prerequisites

- API credentials (username/password or API key)
- Basic understanding of REST APIs
- Development environment (Node.js, Python, .NET, or your preferred language)

### Quick Start Guide

1. **Obtain API Credentials**
   - Register for a CoinPay account
   - Navigate to Settings → API Credentials
   - Generate an API key

2. **Authentication**
   ```bash
   curl -X POST http://localhost:7777/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"your-username","password":"your-password"}'
   ```

3. **Make Your First API Call**
   ```bash
   curl -X GET http://localhost:7777/api/wallet \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"
   ```

## Authentication

CoinPay uses **JWT (JSON Web Token)** authentication for all API requests.

### Obtaining a Token

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "username": "testuser",
  "password": "your-password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 1440,
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "testuser",
    "walletAddress": "0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579"
  }
}
```

### Using the Token

Include the JWT token in the `Authorization` header for all subsequent requests:

```
Authorization: Bearer YOUR_JWT_TOKEN
```

### Token Expiration

- Default expiration: **24 hours (1440 minutes)**
- Refresh token expiration: **7 days**

## API Reference

### Base URL

```
Production: https://api.coinpay.com
Testnet:    http://localhost:7777
```

### Content Type

All requests and responses use JSON:

```
Content-Type: application/json
```

### Endpoints Overview

| Category | Endpoint | Description |
|----------|----------|-------------|
| **Authentication** | POST /api/auth/login | Obtain JWT token |
| **Authentication** | POST /api/auth/register | Create new account |
| **Wallet** | GET /api/wallet | Get wallet details |
| **Wallet** | GET /api/wallet/balance | Get wallet balance |
| **Transactions** | POST /api/transactions | Create transaction |
| **Transactions** | GET /api/transactions | List transactions |
| **Transactions** | GET /api/transactions/{id} | Get transaction details |
| **Swap** | GET /api/swap/quote | Get swap quote |
| **Swap** | POST /api/swap/execute | Execute token swap |
| **Swap** | GET /api/swap/history | Swap history |
| **Payouts** | POST /api/payouts | Initiate fiat withdrawal |
| **Payouts** | GET /api/payouts/{id} | Get payout status |
| **Payouts** | GET /api/payouts | List payouts |
| **Bank Accounts** | GET /api/bank-accounts | List bank accounts |
| **Bank Accounts** | POST /api/bank-accounts | Add bank account |
| **Bank Accounts** | DELETE /api/bank-accounts/{id} | Remove bank account |
| **Investments** | GET /api/investment/plans | List investment plans |
| **Investments** | POST /api/investment/positions | Create position |
| **Investments** | GET /api/investment/positions | List positions |

### Detailed API Documentation

See individual guides:

- [Authentication API](./AUTHENTICATION.md)
- [Wallet API](./WALLET_API.md)
- [Transaction API](./TRANSACTION_API.md)
- [Swap API](./SWAP_API.md)
- [Payout API](./PAYOUT_API.md)
- [Bank Account API](./BANK_ACCOUNT_API.md)
- [Investment API](./INVESTMENT_API.md)

## Integration Guides

### Common Use Cases

1. **[Payment Processing Integration](./guides/PAYMENT_PROCESSING.md)**
   - Accept crypto payments
   - Handle transaction webhooks
   - Display payment status

2. **[Token Swap Integration](./guides/TOKEN_SWAP_INTEGRATION.md)**
   - Get real-time quotes
   - Execute swaps
   - Track swap history

3. **[Fiat Off-Ramp Integration](./guides/FIAT_OFFRAMP.md)**
   - Link bank accounts
   - Process withdrawals
   - Monitor payout status

4. **[Wallet Management](./guides/WALLET_MANAGEMENT.md)**
   - Create wallets
   - Check balances
   - Monitor transactions

## Webhooks

CoinPay supports webhooks for real-time event notifications.

### Available Events

| Event | Description |
|-------|-------------|
| `transaction.created` | New transaction initiated |
| `transaction.completed` | Transaction confirmed on blockchain |
| `transaction.failed` | Transaction failed |
| `payout.created` | Fiat payout initiated |
| `payout.completed` | Payout processed successfully |
| `payout.failed` | Payout failed |
| `wallet.updated` | Wallet balance changed |

### Webhook Configuration

Configure webhook endpoints in your account settings:

```
Settings → Webhooks → Add Endpoint
```

### Webhook Payload Example

```json
{
  "event": "transaction.completed",
  "timestamp": "2025-11-07T14:30:00Z",
  "data": {
    "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
    "amount": "0.05",
    "currency": "POL",
    "status": "Completed",
    "blockchainTxHash": "0x1234...abcd"
  }
}
```

### Webhook Security

All webhooks include an HMAC-SHA256 signature in the `X-Signature` header:

```javascript
const crypto = require('crypto');

function verifyWebhook(payload, signature, secret) {
  const hash = crypto
    .createHmac('sha256', secret)
    .update(JSON.stringify(payload))
    .digest('hex');

  return hash === signature;
}
```

## Rate Limits

| Endpoint Type | Rate Limit |
|---------------|------------|
| Authentication | 5 requests/minute |
| General API | 1000 requests/minute |
| Swap Quote | 30 requests/minute |
| Swap Execute | 10 requests/minute |
| Transaction Read | 60 requests/minute |

Rate limit headers:
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1609459200
```

## Error Handling

### HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (successful deletion) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid/missing token) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 429 | Too Many Requests (rate limit exceeded) |
| 500 | Internal Server Error |

### Error Response Format

```json
{
  "status": 400,
  "title": "Validation Error",
  "detail": "Amount must be greater than 0",
  "errors": [
    {
      "field": "amount",
      "message": "Amount must be greater than 0"
    }
  ]
}
```

## SDKs & Libraries

### Official SDKs

- **Node.js/TypeScript:** Coming soon
- **.NET/C#:** Coming soon
- **Python:** Coming soon

### Community SDKs

- Submit your SDK via GitHub PR

## Testing

### Testnet Environment

- **API URL:** `http://localhost:7777`
- **Testnet Faucets:**
  - MATIC: https://faucet.polygon.technology/
  - USDC: https://faucet.circle.com/

### Test Credentials

```
Username: testuser
Password: Test123!
Wallet: 0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
```

### Test Cards (for bank account simulation)

Not applicable - use real testnet wallet addresses

## Best Practices

### Security

1. **Never expose API keys in client-side code**
2. **Use HTTPS in production**
3. **Validate webhook signatures**
4. **Implement rate limiting on your end**
5. **Log all API interactions for debugging**

### Performance

1. **Cache responses when appropriate**
2. **Use pagination for list endpoints**
3. **Implement exponential backoff for retries**
4. **Use webhooks instead of polling**

### Error Handling

1. **Handle all HTTP status codes**
2. **Implement retry logic for 5xx errors**
3. **Display user-friendly error messages**
4. **Log errors for monitoring**

## Support

### Resources

- **Email:** dev@coinpay.com
- **Discord:** [Join our community](https://discord.gg/coinpay)
- **GitHub Issues:** [Report bugs](https://github.com/developerhakart/CoinPay/issues)
- **Stack Overflow:** Tag your questions with `coinpay-api`

### SLA & Uptime

- Production: 99.9% uptime guarantee
- Testnet: Best effort

### Changelog

See [CHANGELOG.md](./CHANGELOG.md) for API version history and breaking changes.

## Legal

- [Terms of Service](https://coinpay.com/terms)
- [Privacy Policy](https://coinpay.com/privacy)
- [API License Agreement](https://coinpay.com/api-license)

---

**Version:** 1.0.0
**Last Updated:** November 7, 2025
**API Status:** ✅ Operational
