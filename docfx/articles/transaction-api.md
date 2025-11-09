# Transaction API

## Overview

The Transaction API allows you to create, retrieve, and manage cryptocurrency transactions on the CoinPay platform.

## Endpoints

### Create Transaction

Create a new cryptocurrency transaction (transfer, payment, etc.).

**Endpoint:** `POST /api/transactions`

**Authentication:** Required (JWT)

**Request Body:**
```json
{
  "amount": 100.50,
  "currency": "USDC",
  "type": "Transfer",
  "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
  "description": "Payment for services",
  "metadata": {
    "orderId": "ORDER-123",
    "customerId": "CUST-456"
  }
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| amount | decimal | Yes | Transaction amount |
| currency | string | Yes | Currency code (USDC, POL, MATIC, DAI, USDT, WBTC, WETH) |
| type | string | Yes | Transaction type (Transfer, Payment, Refund) |
| receiverName | string | Yes | Recipient wallet address (0x...) |
| description | string | No | Transaction description |
| metadata | object | No | Custom metadata (key-value pairs) |

**Response:** `201 Created`
```json
{
  "id": 123,
  "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
  "amount": 100.50,
  "currency": "USDC",
  "type": "Transfer",
  "status": "Pending",
  "senderName": "testuser",
  "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
  "description": "Payment for services",
  "createdAt": "2025-11-07T14:30:00Z",
  "completedAt": null,
  "blockchainTxHash": null,
  "fee": "0.50",
  "metadata": {
    "orderId": "ORDER-123",
    "customerId": "CUST-456"
  }
}
```

**Error Responses:**

- `400 Bad Request` - Invalid parameters
- `401 Unauthorized` - Missing or invalid token
- `402 Payment Required` - Insufficient balance
- `500 Internal Server Error` - Server error

### Get Transaction by ID

Retrieve details of a specific transaction.

**Endpoint:** `GET /api/transactions/{id}`

**Authentication:** Required (JWT)

**Path Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| id | integer | Transaction ID |

**Response:** `200 OK`
```json
{
  "id": 123,
  "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
  "amount": 100.50,
  "currency": "USDC",
  "type": "Transfer",
  "status": "Completed",
  "senderName": "testuser",
  "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
  "description": "Payment for services",
  "createdAt": "2025-11-07T14:30:00Z",
  "completedAt": "2025-11-07T14:32:15Z",
  "blockchainTxHash": "0x1234567890abcdef...",
  "fee": "0.50",
  "confirmations": 12
}
```

**Error Responses:**

- `401 Unauthorized` - Missing or invalid token
- `404 Not Found` - Transaction not found

### List Transactions

Get a list of transactions for the authenticated user.

**Endpoint:** `GET /api/transactions`

**Authentication:** Required (JWT)

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number (1-based) |
| limit | integer | 20 | Items per page (max 100) |
| status | string | - | Filter by status (Pending, Completed, Failed) |
| currency | string | - | Filter by currency |
| type | string | - | Filter by type (Transfer, Payment, Refund) |
| startDate | string | - | Start date (ISO 8601) |
| endDate | string | - | End date (ISO 8601) |

**Example Request:**
```
GET /api/transactions?page=1&limit=20&status=Completed&currency=USDC
```

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": 123,
      "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
      "amount": 100.50,
      "currency": "USDC",
      "type": "Transfer",
      "status": "Completed",
      "senderName": "testuser",
      "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
      "createdAt": "2025-11-07T14:30:00Z",
      "completedAt": "2025-11-07T14:32:15Z"
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

### Get Transactions by Status

Get transactions filtered by status.

**Endpoint:** `GET /api/transactions/status/{status}`

**Authentication:** Required (JWT)

**Path Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| status | string | Status filter (Pending, Completed, Failed) |

**Response:** `200 OK`
```json
[
  {
    "id": 123,
    "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
    "amount": 100.50,
    "currency": "USDC",
    "status": "Pending",
    "createdAt": "2025-11-07T14:30:00Z"
  }
]
```

### Update Transaction Status (Admin only)

Update the status of a transaction.

**Endpoint:** `PATCH /api/transactions/{id}/status`

**Authentication:** Required (JWT) + Admin role

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| status | string | New status (Pending, Completed, Failed) |

**Example Request:**
```
PATCH /api/transactions/123/status?status=Completed
```

**Response:** `200 OK`
```json
{
  "id": 123,
  "transactionId": "660c8ba1-d392-558b-80ff-a7d3eaeca602",
  "status": "Completed",
  "completedAt": "2025-11-07T14:32:15Z"
}
```

### Delete Transaction (Admin only)

Delete a transaction (soft delete).

**Endpoint:** `DELETE /api/transactions/{id}`

**Authentication:** Required (JWT) + Admin role

**Response:** `204 No Content`

## Transaction Lifecycle

```
Created → Pending → Confirmed → Completed
         ↓
        Failed
```

1. **Created**: Transaction record created
2. **Pending**: Submitted to blockchain
3. **Confirmed**: Received confirmations (1+)
4. **Completed**: Fully confirmed (12+ confirmations)
5. **Failed**: Transaction reverted or failed

## Supported Currencies

| Currency | Network | Address Format |
|----------|---------|----------------|
| USDC | Polygon Amoy | 0x... (20 bytes) |
| POL | Polygon Amoy | 0x... (20 bytes) |
| MATIC | Polygon Amoy | 0x... (20 bytes) |
| DAI | Polygon Amoy | 0x... (20 bytes) |
| USDT | Polygon Amoy | 0x... (20 bytes) |
| WBTC | Polygon Amoy | 0x... (20 bytes) |
| WETH | Polygon Amoy | 0x... (20 bytes) |

## Transaction Types

| Type | Description |
|------|-------------|
| Transfer | Simple wallet-to-wallet transfer |
| Payment | Payment for goods/services |
| Refund | Refund of previous payment |

## Transaction Fees

| Transaction Type | Fee |
|------------------|-----|
| Transfer | 0.5% (min $0.50) |
| Payment | 0.5% (min $0.50) |
| Refund | No fee |

## Code Examples

### Node.js (axios)

```javascript
const axios = require('axios');

async function createTransaction() {
  try {
    const response = await axios.post(
      'http://localhost:7777/api/transactions',
      {
        amount: 100.50,
        currency: 'USDC',
        type: 'Transfer',
        receiverName: '0x76f9f32d75fe641c3d3992f0992ae46ed75cab58',
        description: 'Payment for services'
      },
      {
        headers: {
          'Authorization': 'Bearer YOUR_JWT_TOKEN',
          'Content-Type': 'application/json'
        }
      }
    );

    console.log('Transaction created:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error:', error.response?.data || error.message);
    throw error;
  }
}
```

### Python (requests)

```python
import requests

def create_transaction():
    url = "http://localhost:7777/api/transactions"
    headers = {
        "Authorization": "Bearer YOUR_JWT_TOKEN",
        "Content-Type": "application/json"
    }
    payload = {
        "amount": 100.50,
        "currency": "USDC",
        "type": "Transfer",
        "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
        "description": "Payment for services"
    }

    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 201:
        print("Transaction created:", response.json())
        return response.json()
    else:
        print("Error:", response.json())
        raise Exception("Transaction creation failed")
```

### C# (.NET)

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class TransactionService
{
    private readonly HttpClient _httpClient;

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Transaction> CreateTransactionAsync(string token)
    {
        var payload = new
        {
            amount = 100.50,
            currency = "USDC",
            type = "Transfer",
            receiverName = "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
            description = "Payment for services"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync(
            "http://localhost:7777/api/transactions",
            content
        );

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Transaction>(responseData);
    }
}
```

### cURL

```bash
curl -X POST http://localhost:7777/api/transactions \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 100.50,
    "currency": "USDC",
    "type": "Transfer",
    "receiverName": "0x76f9f32d75fe641c3d3992f0992ae46ed75cab58",
    "description": "Payment for services"
  }'
```

## Webhooks

Subscribe to transaction events:

- `transaction.created` - Transaction created
- `transaction.pending` - Submitted to blockchain
- `transaction.confirmed` - Received first confirmation
- `transaction.completed` - Fully confirmed
- `transaction.failed` - Transaction failed

See [Webhooks Documentation](./WEBHOOKS.md) for details.

## Best Practices

### Error Handling

Always handle these scenarios:

1. **Insufficient Balance** (402)
   - Check wallet balance before creating transaction
   - Display clear message to user

2. **Invalid Address** (400)
   - Validate address format before submitting
   - Use address validation library

3. **Network Issues** (500/503)
   - Implement retry logic with exponential backoff
   - Don't retry on 4xx errors

### Idempotency

Use the `transactionId` to implement idempotency:

```javascript
// Check if transaction already exists before creating
async function createIdempotentTransaction(data) {
  const existing = await getTransactionByMetadata(data.metadata.orderId);
  if (existing) {
    return existing; // Return existing transaction
  }

  return await createTransaction(data);
}
```

### Performance

1. **Use pagination** for listing transactions
2. **Cache transaction details** (TTL: 30 seconds)
3. **Use webhooks** instead of polling for status updates

## Rate Limits

- **Create Transaction:** 10 requests/minute
- **Get Transaction:** 60 requests/minute
- **List Transactions:** 30 requests/minute

## Troubleshooting

### Common Issues

**Issue:** "Insufficient balance" error

**Solution:** Check wallet balance via `/api/wallet/balance` before creating transaction

**Issue:** "Invalid address" error

**Solution:** Ensure recipient address is valid EVM address (0x... format, 42 characters)

**Issue:** Transaction stuck in "Pending" status

**Solution:** Check blockchain explorer for transaction status. May take 2-5 minutes for confirmation.

## Support

For issues or questions:

- Email: dev@coinpay.com
- Discord: [Join community](https://discord.gg/coinpay)
- GitHub: [Report issue](https://github.com/developerhakart/CoinPay/issues)

---

**Last Updated:** November 7, 2025
