# CoinPay API Quick Reference

Quick reference for common CoinPay API operations.

## Base URLs

```
Production: https://api.coinpay.com
Testnet:    http://localhost:7777
Swagger:    http://localhost:7777/swagger
```

## Authentication

```bash
# Login
curl -X POST http://localhost:7777/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"your-password"}'

# Use token in subsequent requests
curl -X GET http://localhost:7777/api/wallet \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Common Operations

### Wallet

```bash
# Get wallet details
GET /api/wallet

# Get balance
GET /api/wallet/balance
```

### Transactions

```bash
# Create transaction
POST /api/transactions
{
  "amount": 100.50,
  "currency": "USDC",
  "type": "Transfer",
  "receiverName": "0x..."
}

# Get transaction
GET /api/transactions/{id}

# List transactions
GET /api/transactions?page=1&limit=20&status=Completed
```

### Token Swap

```bash
# Get swap quote
GET /api/swap/quote?fromToken=USDC&toToken=WETH&amount=100

# Execute swap
POST /api/swap/execute
{
  "fromToken": "USDC",
  "toToken": "WETH",
  "amount": "100",
  "slippage": "0.5"
}

# Swap history
GET /api/swap/history
```

### Withdrawals (Fiat Off-Ramp)

```bash
# Create payout
POST /api/payouts
{
  "amount": 1000,
  "currency": "USD",
  "bankAccountId": "123"
}

# Get payout status
GET /api/payouts/{id}

# List payouts
GET /api/payouts
```

### Bank Accounts

```bash
# List bank accounts
GET /api/bank-accounts

# Add bank account
POST /api/bank-accounts
{
  "accountHolderName": "John Doe",
  "accountNumber": "1234567890",
  "routingNumber": "987654321",
  "bankName": "Test Bank"
}

# Delete bank account
DELETE /api/bank-accounts/{id}
```

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 204 | No Content |
| 400 | Bad Request |
| 401 | Unauthorized |
| 402 | Payment Required (insufficient balance) |
| 404 | Not Found |
| 429 | Too Many Requests |
| 500 | Internal Server Error |

## Error Response Format

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

## Supported Currencies

| Currency | Network | Testnet Faucet |
|----------|---------|----------------|
| USDC | Polygon Amoy | https://faucet.circle.com/ |
| POL | Polygon Amoy | https://faucet.polygon.technology/ |
| MATIC | Polygon Amoy | https://faucet.polygon.technology/ |
| DAI | Polygon Amoy | - |
| USDT | Polygon Amoy | - |
| WBTC | Polygon Amoy | - |
| WETH | Polygon Amoy | - |

## Rate Limits

| Endpoint | Limit |
|----------|-------|
| Authentication | 5/min |
| Transaction Create | 10/min |
| Transaction Read | 60/min |
| Swap Quote | 30/min |
| Swap Execute | 10/min |
| General API | 1000/min |

## Webhook Events

| Event | Trigger |
|-------|---------|
| transaction.created | Transaction initiated |
| transaction.completed | Transaction confirmed |
| transaction.failed | Transaction failed |
| payout.created | Payout initiated |
| payout.completed | Payout processed |
| payout.failed | Payout failed |
| wallet.updated | Balance changed |

## Code Examples

### Node.js

```javascript
const axios = require('axios');

// Login
const auth = await axios.post('http://localhost:7777/api/auth/login', {
  username: 'testuser',
  password: 'password'
});

const token = auth.data.token;

// Create transaction
const tx = await axios.post(
  'http://localhost:7777/api/transactions',
  {
    amount: 100.50,
    currency: 'USDC',
    type: 'Transfer',
    receiverName: '0x76f9f32d75fe641c3d3992f0992ae46ed75cab58'
  },
  {
    headers: { 'Authorization': `Bearer ${token}` }
  }
);
```

### Python

```python
import requests

# Login
auth = requests.post(
    'http://localhost:7777/api/auth/login',
    json={'username': 'testuser', 'password': 'password'}
)

token = auth.json()['token']

# Create transaction
tx = requests.post(
    'http://localhost:7777/api/transactions',
    json={
        'amount': 100.50,
        'currency': 'USDC',
        'type': 'Transfer',
        'receiverName': '0x76f9f32d75fe641c3d3992f0992ae46ed75cab58'
    },
    headers={'Authorization': f'Bearer {token}'}
)
```

### C#

```csharp
using System.Net.Http;
using System.Text.Json;

var client = new HttpClient();

// Login
var authResponse = await client.PostAsync(
    "http://localhost:7777/api/auth/login",
    new StringContent(
        JsonSerializer.Serialize(new { username = "testuser", password = "password" }),
        Encoding.UTF8,
        "application/json"
    )
);

var authData = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();
var token = authData.Token;

// Create transaction
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var txResponse = await client.PostAsync(
    "http://localhost:7777/api/transactions",
    new StringContent(
        JsonSerializer.Serialize(new {
            amount = 100.50,
            currency = "USDC",
            type = "Transfer",
            receiverName = "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58"
        }),
        Encoding.UTF8,
        "application/json"
    )
);
```

## Test Data

### Test User
```
Username: testuser
Password: Test123!
Wallet: 0xac5f9e0b3b87a0a5ca0ff0fc169db6bb653fe579
Circle Wallet ID: fef70777-cb2d-5096-a0ea-15dba5662ce6
```

### Test Recipient
```
Address: 0x76f9f32d75fe641c3d3992f0992ae46ed75cab58
Circle Wallet ID: 40b623f4-5ccd-5006-b4d2-c8b22e61681d
```

## Useful Links

- **Swagger UI:** http://localhost:7777/swagger
- **GitHub:** https://github.com/developerhakart/CoinPay
- **Full Documentation:** [API Documentation](./README.md)
- **B2B Integration Guide:** [B2B Guide](./guides/B2B_INTEGRATION_GUIDE.md)
- **Transaction API:** [Transaction Docs](./TRANSACTION_API.md)

## Support

- **Email:** dev@coinpay.com
- **Discord:** https://discord.gg/coinpay
- **GitHub Issues:** https://github.com/developerhakart/CoinPay/issues

---

**Last Updated:** November 7, 2025
