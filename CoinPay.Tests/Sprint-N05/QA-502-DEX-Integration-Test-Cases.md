# QA-502: DEX Integration Test Cases
# 1inch API Integration Testing

**Test Suite**: DEX Integration
**Priority**: P0 (Critical Path)
**Estimated Effort**: 3 days
**Status**: BLOCKED - Phase 5 not implemented
**Dependencies**: BE-501, BE-502 (1inch API Client)

---

## Test Objective

Validate 1inch DEX aggregator integration on Polygon Amoy testnet, including API connectivity, quote retrieval, swap transaction data, and error handling.

---

## Test Environment

```
DEX Provider: 1inch API v5.0
Network: Polygon Amoy Testnet (Chain ID: 80002)
Base URL: https://api.1inch.io/v5.0/80002
Authentication: API Key (stored in HashiCorp Vault)
Rate Limit: 10 requests/second
```

**Test Tokens**:
- USDC: `0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582`
- WETH: `TBD (testnet address)`
- WMATIC: `TBD (testnet address)`

---

## TC-502-001: Verify 1inch API Connection

**Priority**: P0
**Type**: Integration Test
**Preconditions**:
- Backend API running
- 1inch API key configured in Vault
- Internet connectivity

**Test Steps**:
1. Configure 1inch API endpoint in `appsettings.json`:
   ```json
   "DexAggregator": {
     "Provider": "1inch",
     "BaseUrl": "https://api.1inch.io/v5.0",
     "ChainId": 80002,
     "ApiKey": "vault:dex/1inch/apikey"
   }
   ```

2. Send GET request to 1inch health endpoint:
   ```bash
   curl -X GET "https://api.1inch.io/v5.0/80002/healthcheck" \
     -H "Authorization: Bearer {API_KEY}"
   ```

3. Verify backend can establish connection:
   ```http
   GET {{baseUrl}}/api/swap/quote?fromToken=0x41e94eb...&toToken=0x...&amount=100
   ```

**Expected Result**:
- ✅ 1inch API responds with 200 OK
- ✅ Backend successfully connects to 1inch
- ✅ Response time < 2 seconds
- ✅ No authentication errors (401)

**Actual Result**: ⏸️  BLOCKED - Endpoint not implemented

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-002: Verify API Authentication

**Priority**: P0
**Type**: Security Test
**Preconditions**:
- Valid API key stored in Vault
- Backend service configured

**Test Steps**:
1. Test with valid API key:
   ```http
   GET /api/swap/quote?fromToken=USDC&toToken=WETH&amount=100
   Authorization: Bearer {valid_jwt}
   ```

2. Test with invalid API key (backend should handle):
   - Temporarily set invalid 1inch API key in Vault
   - Attempt to get quote
   - Verify proper error handling

3. Test with missing API key:
   - Remove API key from configuration
   - Attempt to get quote
   - Verify proper error handling

**Expected Result**:
- ✅ Valid key: 200 OK with quote data
- ✅ Invalid key: 500 Internal Server Error with message "1inch API authentication failed"
- ✅ Missing key: 500 Internal Server Error with message "1inch API key not configured"
- ✅ Error logged with correlation ID

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-003: Verify API Rate Limiting

**Priority**: P1
**Type**: Performance Test
**Preconditions**:
- Backend implements rate limiting
- 1inch API rate limit: 10 req/sec

**Test Steps**:
1. Send 20 concurrent requests for quotes:
   ```bash
   for i in {1..20}; do
     curl -X GET "{{baseUrl}}/api/swap/quote?fromToken=USDC&toToken=WETH&amount=100" &
   done
   wait
   ```

2. Verify backend implements request queuing:
   - Check response times
   - Verify no 429 (Too Many Requests) from 1inch
   - Verify requests processed sequentially if needed

3. Monitor logs for rate limiting logic:
   ```
   [INFO] Rate limit: 8/10 requests in current second
   [WARN] Rate limit reached, queuing request
   ```

**Expected Result**:
- ✅ All 20 requests eventually succeed
- ✅ No 429 errors from 1inch API
- ✅ Backend implements rate limiting (max 10 req/sec to 1inch)
- ✅ Requests queued and processed within 3 seconds total

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-004: Get USDC → WETH Quote

**Priority**: P0
**Type**: Functional Test
**Test Data**:
```json
{
  "fromToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
  "toToken": "0x...",
  "amount": 100,
  "slippage": 1.0
}
```

**Test Steps**:
1. Send GET request to backend quote API:
   ```http
   GET {{baseUrl}}/api/swap/quote
     ?fromToken=0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582
     &toToken=0x...
     &amount=100
     &slippage=1
   Authorization: Bearer {{jwt_token}}
   ```

2. Verify response structure:
   ```json
   {
     "fromToken": "0x41e94eb...",
     "fromTokenSymbol": "USDC",
     "toToken": "0x...",
     "toTokenSymbol": "WETH",
     "fromAmount": 100.0,
     "toAmount": 0.0285,
     "exchangeRate": 0.000285,
     "platformFee": 0.50,
     "platformFeePercentage": 0.5,
     "estimatedGas": "150000",
     "estimatedGasCost": "0.015",
     "priceImpact": 0.15,
     "slippageTolerance": 1.0,
     "minimumReceived": 0.02821,
     "quoteValidUntil": "2025-11-05T10:00:30Z",
     "provider": "1inch"
   }
   ```

3. Validate calculations:
   - Exchange rate is reasonable (compare with market rate)
   - Platform fee = 100 * 0.005 = 0.50 USDC
   - Minimum received = 0.0285 * 0.99 = 0.02821 WETH (1% slippage)
   - Price impact < 5% for this amount

**Expected Result**:
- ✅ 200 OK response
- ✅ Response time < 2 seconds
- ✅ All required fields present
- ✅ Exchange rate within reasonable range
- ✅ Platform fee calculated correctly (0.5%)
- ✅ Minimum received calculated correctly
- ✅ Gas estimate present and reasonable (100k-300k)

**Validation Criteria**:
- Exchange rate > 0 and < 1 (USDC to WETH)
- Platform fee = fromAmount * 0.005 (accurate to 8 decimals)
- Minimum received = toAmount * (1 - slippageTolerance/100)
- Quote valid for 30 seconds
- Gas estimate between 100,000 and 300,000

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-005: Get USDC → WMATIC Quote

**Priority**: P0
**Type**: Functional Test
**Test Data**:
```json
{
  "fromToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
  "toToken": "0x...",
  "amount": 100,
  "slippage": 1.0
}
```

**Test Steps**:
1. Request quote for USDC → WMATIC swap
2. Verify quote returned with proper decimal precision
3. Validate WMATIC-specific parameters

**Expected Result**:
- ✅ Quote returned successfully
- ✅ Exchange rate shows correct USDC/WMATIC ratio
- ✅ Decimal precision: 18 decimals for WMATIC amount
- ✅ All validations pass

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-006: Get WETH → USDC Quote

**Priority**: P0
**Type**: Functional Test
**Test Data**:
```json
{
  "fromToken": "0x...",
  "toToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
  "amount": 0.05,
  "slippage": 1.0
}
```

**Test Steps**:
1. Request quote for WETH → USDC (reverse swap)
2. Verify exchange rate is inverse of USDC → WETH
3. Validate platform fee in WETH

**Expected Result**:
- ✅ Quote returned successfully
- ✅ Exchange rate is reciprocal (if USDC→WETH rate is 0.000285, then WETH→USDC ≈ 3508)
- ✅ Platform fee = 0.05 * 0.005 = 0.00025 WETH
- ✅ All validations pass

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-007: Quote with Different Amounts

**Priority**: P1
**Type**: Functional Test
**Test Data**:
```
Test amounts: 1, 10, 100, 1000 USDC
```

**Test Steps**:
1. Request quotes for each amount:
   - 1 USDC → WETH
   - 10 USDC → WETH
   - 100 USDC → WETH
   - 1000 USDC → WETH

2. Compare exchange rates and price impact:
   | Amount | Expected Exchange Rate | Expected Price Impact |
   |--------|------------------------|----------------------|
   | 1 | Similar to market | < 0.5% |
   | 10 | Similar to market | < 0.5% |
   | 100 | Slightly worse | 0.5% - 1% |
   | 1000 | Noticeably worse | 1% - 3% |

3. Verify price impact increases with amount:
   - price_impact(1000) > price_impact(100) > price_impact(10) > price_impact(1)

**Expected Result**:
- ✅ All quotes return successfully
- ✅ Exchange rates reasonable for all amounts
- ✅ Price impact increases with trade size
- ✅ Large trades (1000 USDC) show warning for high price impact
- ✅ Response times < 2 seconds for all

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-008: Quote with Invalid Token

**Priority**: P0
**Type**: Negative Test
**Test Data**:
```json
{
  "fromToken": "0xinvalidaddress",
  "toToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
  "amount": 100,
  "slippage": 1.0
}
```

**Test Steps**:
1. Send request with invalid token address
2. Verify error handling
3. Check logs for proper error logging

**Expected Result**:
- ✅ 400 Bad Request
- ✅ Error message: "Invalid token address"
- ✅ Response structure:
   ```json
   {
     "error": "Invalid token address",
     "field": "fromToken",
     "providedValue": "0xinvalidaddress"
   }
   ```
- ✅ Error logged with correlation ID

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-009: Quote with Zero Amount

**Priority**: P0
**Type**: Negative Test
**Test Data**:
```json
{
  "fromToken": "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582",
  "toToken": "0x...",
  "amount": 0,
  "slippage": 1.0
}
```

**Test Steps**:
1. Send request with amount = 0
2. Verify validation error
3. Test with negative amount (-10)

**Expected Result**:
- ✅ 400 Bad Request for amount = 0
- ✅ Error message: "Amount must be greater than 0"
- ✅ 400 Bad Request for amount = -10
- ✅ Error message: "Amount must be positive"

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-010: Get Swap Transaction for USDC → WETH

**Priority**: P0
**Type**: Integration Test
**Preconditions**:
- Valid quote obtained (from TC-502-004)

**Test Steps**:
1. Get quote for 100 USDC → WETH
2. Use quote to request swap transaction data:
   ```http
   POST {{baseUrl}}/api/swap/execute
   Content-Type: application/json
   Authorization: Bearer {{jwt_token}}

   {
     "fromToken": "0x41e94eb...",
     "toToken": "0x...",
     "fromAmount": 100,
     "slippageTolerance": 1.0
   }
   ```
   NOTE: This actually EXECUTES the swap, so for testing:
   - Use testnet only
   - Or implement GET /api/swap/transaction endpoint for dry-run

3. Verify response contains transaction object:
   ```json
   {
     "swapId": "uuid",
     "transactionHash": "0x...",
     "status": "pending",
     "fromToken": "0x41e94eb...",
     "toToken": "0x...",
     "fromAmount": 100,
     "expectedToAmount": 0.0285,
     "minimumReceived": 0.02821,
     "platformFee": 0.50,
     "message": "Swap transaction submitted successfully"
   }
   ```

4. Verify transaction data structure includes:
   - `to`: Contract address to send transaction to
   - `data`: Encoded function call data
   - `value`: Amount of native token (0 for USDC swap)
   - `gas`: Gas limit estimate

**Expected Result**:
- ✅ Transaction object returned
- ✅ Contains `to`, `data`, `value`, `gas` fields
- ✅ Gas estimate between 150,000 and 300,000
- ✅ Transaction data is valid hex string
- ✅ Swap ID is valid UUID
- ✅ Status is "pending"

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-011: Swap Transaction with Slippage

**Priority**: P0
**Type**: Functional Test
**Test Data**:
```json
{
  "fromToken": "0x41e94eb...",
  "toToken": "0x...",
  "fromAmount": 100,
  "slippageTolerance": 2.5
}
```

**Test Steps**:
1. Request quote with 2.5% slippage
2. Verify minimum received calculated with 2.5% slippage:
   - If expected amount = 0.0285 WETH
   - Minimum received = 0.0285 * (1 - 0.025) = 0.0277875 WETH
3. Verify slippage applied correctly in transaction data

**Expected Result**:
- ✅ Minimum received = expectedAmount * (1 - slippage/100)
- ✅ Slippage tolerance reflected in transaction data
- ✅ Accurate to 8 decimal places

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## TC-502-012: Verify Gas Estimation

**Priority**: P1
**Type**: Functional Test

**Test Steps**:
1. Request swap transaction for various token pairs
2. Verify gas estimates are reasonable:
   - USDC → WETH: 150,000 - 250,000 gas
   - USDC → WMATIC: 150,000 - 250,000 gas
   - WETH → USDC: 150,000 - 250,000 gas

3. Verify gas includes buffer (safety margin):
   - Actual gas + 10-20% buffer

**Expected Result**:
- ✅ Gas estimates within reasonable range
- ✅ Gas estimate includes safety buffer
- ✅ Higher gas for complex swaps (multi-hop routes)
- ✅ Gas cost calculated: gasEstimate * gasPrice * maticPrice

**Actual Result**: ⏸️  BLOCKED

**Pass/Fail**: ⏸️  BLOCKED

---

## Performance Acceptance Criteria

All tests must meet the following performance targets:

| Metric | Target | Critical Threshold |
|--------|--------|-------------------|
| Quote API Response Time (P95) | < 2s | < 3s |
| Quote API Response Time (P99) | < 3s | < 5s |
| Success Rate | > 95% | > 90% |
| 1inch API Errors | < 5% | < 10% |
| Rate Limit Compliance | 100% | 100% |

---

## Test Summary

**Total Test Cases**: 12
**Executed**: 0 (BLOCKED)
**Passed**: 0
**Failed**: 0
**Blocked**: 12

**Blocking Issue**: BLOCKER-001 - Phase 5 not implemented

**Priority Breakdown**:
- P0 (Critical): 10 test cases
- P1 (High): 2 test cases

**Coverage**:
- API Connectivity: ✅ Planned
- Authentication: ✅ Planned
- Quote Retrieval: ✅ Planned
- Transaction Data: ✅ Planned
- Error Handling: ✅ Planned
- Performance: ✅ Planned

---

## Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| 1inch testnet API unavailable | High | Use mainnet fork or mock server |
| Rate limiting issues | Medium | Implement request queuing |
| Testnet liquidity low | Medium | Use small test amounts |
| Gas price volatility | Low | Use gas price oracles |

---

## Test Environment Setup

**Prerequisites**:
1. ✅ 1inch API key obtained
2. ⏸️  Backend DEX service implemented
3. ⏸️  Polygon Amoy RPC accessible
4. ⏸️  Test wallet funded with tokens
5. ⏸️  HashiCorp Vault configured

**Test Tools**:
- Postman or REST Client
- curl for command-line testing
- Backend API logs for debugging

---

## Next Steps

1. ⏸️  Wait for BE-502 (1inch API Client) implementation
2. ⏸️  Setup test environment (Polygon Amoy)
3. ⏸️  Obtain 1inch API key
4. ⏸️  Fund test wallets with tokens
5. ⏸️  Execute test cases
6. ⏸️  Document results

---

**Status**: READY TO EXECUTE (pending development)
**Owner**: QA Engineer 1
**Estimated Duration**: 3 days (after unblocked)
**Current Progress**: 0% (blocked by BLOCKER-001)

---

**END OF TEST CASES**
