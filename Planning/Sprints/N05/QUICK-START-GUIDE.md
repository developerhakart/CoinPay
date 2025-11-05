# Sprint N05 - Quick Start Guide
## Basic Swap (DEX Integration) - Backend

**For:** Frontend Developers, QA Engineers, DevOps

---

## Quick Setup

### 1. Apply Database Migration
```bash
cd CoinPay.Api
dotnet ef database update
```

### 2. Configuration (Already in appsettings.json)
```json
{
  "OneInch": {
    "UseMockMode": true  // Set to false for real API
  },
  "Swap": {
    "PlatformFeePercentage": 0.5  // 0.5% platform fee
  }
}
```

### 3. Start the API
```bash
dotnet run
```

API will be available at: `http://localhost:5100`
Swagger UI: `http://localhost:5100/swagger`

---

## API Endpoints

### 1. Get Swap Quote (Anonymous)
```bash
GET /api/swap/quote?fromToken=0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582&toToken=0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9&amount=100&slippage=1.0

Response:
{
  "fromTokenSymbol": "USDC",
  "toTokenSymbol": "WETH",
  "fromAmount": 100.0,
  "toAmount": 0.0285,
  "exchangeRate": 0.000285,
  "platformFee": 0.50,
  "platformFeePercentage": 0.5,
  "estimatedGas": "150000",
  "priceImpact": 0.15,
  "slippageTolerance": 1.0,
  "minimumReceived": 0.02821,
  "quoteValidUntil": "2025-11-05T10:30:00Z",
  "provider": "1inch"
}
```

### 2. Execute Swap (Requires Auth)
```bash
POST /api/swap/execute
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "fromToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
  "toToken": "0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9",
  "fromAmount": 100.0,
  "slippageTolerance": 1.0
}

Response:
{
  "swapId": "guid",
  "transactionHash": "0x...",
  "status": "pending",
  "expectedToAmount": 0.0285,
  "message": "Swap transaction submitted successfully"
}
```

### 3. Get Swap History (Requires Auth)
```bash
GET /api/swap/history?status=all&page=1&pageSize=20
Authorization: Bearer {jwt_token}

Response:
{
  "swaps": [...],
  "totalItems": 15,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

### 4. Get Swap Details (Requires Auth)
```bash
GET /api/swap/{swapId}/details
Authorization: Bearer {jwt_token}

Response:
{
  "id": "guid",
  "fromTokenSymbol": "USDC",
  "toTokenSymbol": "WETH",
  "fromAmount": 100.0,
  "toAmount": 0.0285,
  "status": "confirmed",
  "transactionHash": "0x...",
  ...
}
```

---

## Supported Tokens (Polygon Amoy Testnet)

| Symbol | Address | Decimals |
|--------|---------|----------|
| USDC   | 0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582 | 6 |
| WETH   | 0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9 | 18 |
| WMATIC | 0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270 | 18 |

---

## Testing Scenarios

### Happy Path
1. Get quote for USDC -> WETH
2. Execute swap with valid balance
3. Check swap history
4. Get swap details

### Error Scenarios
1. **Insufficient Balance**
   - Try to swap more than available balance
   - Expect: 400 Bad Request with shortfall details

2. **Invalid Token Pair**
   - Use same token for from/to
   - Expect: 400 Bad Request

3. **Invalid Slippage**
   - Use slippage < 0.1% or > 50%
   - Expect: 400 Bad Request

4. **Unauthorized Access**
   - Try execute/history without JWT token
   - Expect: 401 Unauthorized

---

## Mock Mode Behavior

When `"OneInch:UseMockMode": true`:

1. **Mock Exchange Rates:**
   - USDC/WETH: 0.000285 (~$3500 ETH)
   - USDC/WMATIC: 1.25 (~$0.80 MATIC)
   - WETH/WMATIC: 4375

2. **Mock Balances:**
   - All users have 1000 units of each token
   - Sufficient for testing all scenarios

3. **Mock Transaction Hash:**
   - Generated as: `0x{guid}{timestamp}`
   - Not a real blockchain transaction

---

## Frontend Integration Tips

### Quote Widget
```javascript
// Fetch quote
const response = await fetch(
  '/api/swap/quote?fromToken=USDC&toToken=WETH&amount=100&slippage=1.0'
);
const quote = await response.json();

// Display
console.log(`Rate: ${quote.exchangeRate}`);
console.log(`Fee: ${quote.platformFee} ${quote.fromTokenSymbol}`);
console.log(`You'll receive: ${quote.toAmount} ${quote.toTokenSymbol}`);
console.log(`Min received: ${quote.minimumReceived} ${quote.toTokenSymbol}`);
```

### Execute Swap
```javascript
const response = await fetch('/api/swap/execute', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${jwtToken}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    fromToken: '0x41e94eb...',
    toToken: '0x360ad4f...',
    fromAmount: 100.0,
    slippageTolerance: 1.0
  })
});

const result = await response.json();
console.log(`Swap ID: ${result.swapId}`);
console.log(`TX Hash: ${result.transactionHash}`);
```

---

## QA Test Cases

### Test Case 1: Basic Swap Quote
- **Input:** 100 USDC -> WETH, 1% slippage
- **Expected:** Quote with exchange rate, fees, price impact
- **Verify:** Fee = 0.5 USDC, Price impact < 1%

### Test Case 2: Execute Swap
- **Precondition:** User logged in with wallet
- **Input:** Execute swap for 50 USDC -> WETH
- **Expected:** Transaction hash returned, swap record created
- **Verify:** Swap appears in history with status "pending"

### Test Case 3: Insufficient Balance
- **Input:** Try to swap 10,000 USDC (more than mock balance)
- **Expected:** 400 error with shortfall amount
- **Verify:** Error message is clear and actionable

### Test Case 4: Swap History
- **Precondition:** Execute 3 swaps
- **Input:** GET /api/swap/history
- **Expected:** List of 3 swaps with pagination
- **Verify:** Swaps are sorted by created_at desc

### Test Case 5: Invalid Slippage
- **Input:** Slippage = 60% (exceeds max 50%)
- **Expected:** 400 error
- **Verify:** Error mentions valid range (0.1% - 50%)

---

## Troubleshooting

### Issue: "Wallet not found"
**Solution:** User needs to create a wallet first via `/api/wallet/create`

### Issue: "Insufficient balance"
**Solution:** In mock mode, balance is 1000. Reduce swap amount or check mock balance configuration.

### Issue: "Quote expired"
**Solution:** Quotes are valid for 30 seconds. Get a fresh quote before executing.

### Issue: Redis cache errors
**Solution:** Redis is optional. If unavailable, caching is disabled gracefully.

### Issue: Database migration errors
**Solution:** Ensure connection string is correct and run `dotnet ef database update`

---

## Performance Expectations

- **Quote API:** < 2 seconds (first call), < 500ms (cached)
- **Execute API:** < 3 seconds
- **History API:** < 1 second
- **Cache Hit Rate:** > 80% for quote requests

---

## Support Token Addresses Quick Reference

Copy-paste ready for testing:

```javascript
const tokens = {
  USDC: '0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582',
  WETH: '0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9',
  WMATIC: '0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270'
};
```

---

## Next Steps

1. **Frontend:** Integrate swap widget and history page
2. **QA:** Execute test cases and report issues
3. **DevOps:** Plan deployment and monitoring setup
4. **Backend:** Implement unit tests and real 1inch integration

---

**Questions?** Check the detailed implementation summary or Swagger documentation at `/swagger`
