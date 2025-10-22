# API Usage Examples

This guide provides practical examples for using the CoinPay API.

## Base URL

All API endpoints are relative to: `http://localhost:7777/api`

## Authentication

Currently, the API does not require authentication. This will be added in future versions.

## Examples

### Get All Transactions

**Request:**
```bash
curl -X GET http://localhost:7777/api/transactions
```

**Response:**
```json
[
  {
    "id": 1,
    "transactionId": "TXN001",
    "amount": 100.50,
    "currency": "USD",
    "type": "Payment",
    "status": "Completed",
    "senderName": "John Doe",
    "receiverName": "Jane Smith",
    "description": "Payment for services",
    "createdAt": "2025-10-20T21:43:21.5528677Z",
    "completedAt": "2025-10-20T21:43:21.552907Z"
  }
]
```

### Get Transaction by ID

**Request:**
```bash
curl -X GET http://localhost:7777/api/transactions/1
```

**Response:**
```json
{
  "id": 1,
  "transactionId": "TXN001",
  "amount": 100.50,
  "currency": "USD",
  "type": "Payment",
  "status": "Completed",
  "senderName": "John Doe",
  "receiverName": "Jane Smith",
  "description": "Payment for services",
  "createdAt": "2025-10-20T21:43:21.5528677Z",
  "completedAt": "2025-10-20T21:43:21.552907Z"
}
```

### Get Transactions by Status

**Request:**
```bash
curl -X GET http://localhost:7777/api/transactions/status/Pending
```

**Response:**
```json
[
  {
    "id": 3,
    "transactionId": "TXN003",
    "amount": 75.25,
    "currency": "USD",
    "type": "Payment",
    "status": "Pending",
    "senderName": "Charlie Brown",
    "receiverName": "David Lee",
    "description": "Pending payment",
    "createdAt": "2025-10-22T21:43:21.5529498Z",
    "completedAt": null
  }
]
```

### Create a New Transaction

**Request:**
```bash
curl -X POST http://localhost:7777/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 150.00,
    "currency": "USD",
    "type": "Payment",
    "status": "Pending",
    "senderName": "Alice",
    "receiverName": "Bob",
    "description": "Service payment"
  }'
```

**Response:**
```json
{
  "id": 4,
  "transactionId": "TXN638654321234567890",
  "amount": 150.00,
  "currency": "USD",
  "type": "Payment",
  "status": "Pending",
  "senderName": "Alice",
  "receiverName": "Bob",
  "description": "Service payment",
  "createdAt": "2025-10-22T22:00:00Z",
  "completedAt": null
}
```

### Update a Transaction

**Request:**
```bash
curl -X PUT http://localhost:7777/api/transactions/1 \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 200.00,
    "currency": "USD",
    "type": "Payment",
    "status": "Completed",
    "senderName": "John Doe",
    "receiverName": "Jane Smith",
    "description": "Updated payment"
  }'
```

**Response:**
```json
{
  "id": 1,
  "transactionId": "TXN001",
  "amount": 200.00,
  "currency": "USD",
  "type": "Payment",
  "status": "Completed",
  "senderName": "John Doe",
  "receiverName": "Jane Smith",
  "description": "Updated payment",
  "createdAt": "2025-10-20T21:43:21.5528677Z",
  "completedAt": "2025-10-22T22:05:00Z"
}
```

### Update Transaction Status

**Request:**
```bash
curl -X PATCH "http://localhost:7777/api/transactions/3/status?status=Completed"
```

**Response:**
```json
{
  "id": 3,
  "transactionId": "TXN003",
  "amount": 75.25,
  "currency": "USD",
  "type": "Payment",
  "status": "Completed",
  "senderName": "Charlie Brown",
  "receiverName": "David Lee",
  "description": "Pending payment",
  "createdAt": "2025-10-22T21:43:21.5529498Z",
  "completedAt": "2025-10-22T22:10:00Z"
}
```

### Delete a Transaction

**Request:**
```bash
curl -X DELETE http://localhost:7777/api/transactions/1
```

**Response:**
```
204 No Content
```

## Status Codes

The API uses standard HTTP status codes:

- `200 OK` - Request succeeded
- `201 Created` - Resource created successfully
- `204 No Content` - Request succeeded with no response body
- `404 Not Found` - Resource not found
- `400 Bad Request` - Invalid request data

## Transaction Types

- `Payment` - Standard payment transaction
- `Transfer` - Money transfer between accounts
- `Refund` - Refund transaction

## Transaction Statuses

- `Pending` - Transaction is pending
- `Completed` - Transaction completed successfully
- `Failed` - Transaction failed

## Error Handling

When an error occurs, the API returns an appropriate HTTP status code and error details.

**Example Error Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-xxx"
}
```

## Next Steps

- Explore the [API Reference](../api/index.md) for detailed documentation
- Check the [Configuration Guide](configuration.md) for advanced setup
