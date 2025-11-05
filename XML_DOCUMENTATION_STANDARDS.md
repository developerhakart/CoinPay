# API Controller XML Documentation Standards
## Sprint N06 - BE-607 Task Implementation

---

## Overview

This document defines the XML documentation standards for all CoinPay API controller endpoints. All public methods must include comprehensive XML documentation that will be included in the Swagger/OpenAPI specification.

---

## Standard Documentation Template

### Basic Format
```csharp
/// <summary>
/// Brief one-line description of what the endpoint does
/// </summary>
/// <param name="paramName">Description of parameter</param>
/// <param name="cancellationToken">Cancellation token for async operations</param>
/// <returns>Description of return value/response</returns>
/// <response code="200">Success - entity created/updated/retrieved</response>
/// <response code="201">Created - new resource created</response>
/// <response code="204">No Content - operation successful, no response body</response>
/// <response code="400">Bad Request - validation error or invalid input</response>
/// <response code="401">Unauthorized - authentication required or invalid token</response>
/// <response code="403">Forbidden - user lacks permission for this resource</response>
/// <response code="404">Not Found - resource does not exist</response>
/// <response code="409">Conflict - resource already exists or conflict detected</response>
/// <response code="500">Internal Server Error - unexpected server error</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ResponseDto>> MethodName(int id, CancellationToken cancellationToken)
{
    // Implementation
}
```

---

## Detailed Guidelines

### 1. Summary Tags
**Purpose:** Provide a concise description of the endpoint's functionality

**Requirements:**
- Start with a capital letter
- Keep to one line (max 100 characters)
- Use active voice (e.g., "Retrieves", "Creates", "Updates")
- Do NOT include parameter list or response codes

**Examples:**
```csharp
/// <summary>
/// Retrieves a wallet by ID for the authenticated user
/// </summary>

/// <summary>
/// Initiates a payout transaction to a bank account
/// </summary>

/// <summary>
/// Updates webhook delivery preferences and event filters
/// </summary>

/// <summary>
/// Cancels a pending payout request
/// </summary>
```

**Bad Examples:**
```csharp
/// <summary>
/// Gets wallet (too vague)
/// </summary>

/// <summary>
/// Wallet endpoint that retrieves wallet data and returns it
/// </summary>

/// <summary>
/// This method retrieves wallet information for users
/// </summary>
```

---

### 2. Parameter Tags
**Purpose:** Document method parameters and their constraints

**Format:**
```csharp
/// <param name="paramName">
/// Description including:
/// - What the parameter represents
/// - Valid value ranges (if applicable)
/// - Format requirements (e.g., "Ethereum address must start with 0x")
/// - Required vs optional
/// </param>
```

**Examples:**
```csharp
/// <param name="id">
/// The wallet ID (integer, must be positive)
/// </param>

/// <param name="address">
/// Ethereum wallet address in checksum format (42 characters, starts with 0x)
/// </param>

/// <param name="amount">
/// Transfer amount in USDC (decimal, must be greater than 0, max 2 decimals)
/// </param>

/// <param name="startDate">
/// Optional. Filter transactions starting from this date (inclusive). Defaults to 30 days ago.
/// </param>

/// <param name="pageSize">
/// Number of items per page (1-100, default: 20)
/// </param>

/// <param name="cancellationToken">
/// Cancellation token for async operations
/// </param>
```

**Special Handling for CancellationToken:**
Always include it:
```csharp
/// <param name="cancellationToken">
/// Cancellation token for graceful shutdown
/// </param>
```

---

### 3. Returns Tag
**Purpose:** Document the response data type and structure

**Format:**
```csharp
/// <returns>
/// {ResponseType} containing:
/// - Field1: Description
/// - Field2: Description
/// - Nested objects: Describe structure
/// </returns>
```

**Examples:**
```csharp
/// <returns>
/// WalletResponse containing wallet address, balances for USDC, ETH, and MATIC
/// </returns>

/// <returns>
/// List of TransactionResponse objects with pagination metadata
/// </returns>

/// <returns>
/// No content (HTTP 204) on successful deletion
/// </returns>

/// <returns>
/// PayoutResponse containing payout ID, status, and estimated completion time
/// </returns>
```

---

### 4. Response Code Tags
**Purpose:** Document all possible HTTP response codes and scenarios

**Format:**
```csharp
/// <response code="statusCode">
/// Description of when this response is returned and what it indicates
/// </response>
```

**Guidelines:**
- Document all status codes returned by the endpoint
- Match `[ProducesResponseType(...)]` attributes
- Include practical scenarios that trigger each response

**Examples:**
```csharp
/// <response code="200">
/// Transaction retrieved successfully. Returns transaction details including status, hash, and confirmations.
/// </response>
/// <response code="400">
/// Bad request. Invalid address format, insufficient balance, or invalid amount.
/// </response>
/// <response code="401">
/// Unauthorized. User authentication failed or JWT token is invalid/expired.
/// </response>
/// <response code="404">
/// Transaction not found. The specified transaction ID does not exist for this user.
/// </response>
/// <response code="500">
/// Internal server error. Blockchain RPC service unavailable or unexpected error occurred.
/// </response>
```

**Complete Example Set:**
```csharp
/// <response code="201">
/// Webhook registered successfully. Returns WebhookRegistrationResponse with secret.
/// </response>
/// <response code="400">
/// Bad request. Invalid URL format, invalid event names, or missing required fields.
/// </response>
/// <response code="401">
/// Unauthorized. User not authenticated or JWT token is missing/invalid.
/// </response>
/// <response code="500">
/// Internal server error. Database operation failed or unexpected error.
/// </response>
```

---

### 5. Remarks Tags (Optional but Recommended)
**Purpose:** Provide additional context, examples, or usage notes

**Format:**
```csharp
/// <remarks>
/// This endpoint supports advanced filtering and pagination.
///
/// Example usage:
/// GET /api/transactions/history?page=1&pageSize=20&status=Confirmed&sortBy=CreatedAt
///
/// Default values:
/// - page: 1
/// - pageSize: 20
/// - sortBy: CreatedAt
/// - sortDescending: true
/// </remarks>
```

---

## Complete Controller Documentation Examples

### Example 1: Simple GET by ID
```csharp
/// <summary>
/// Retrieves wallet balance for the authenticated user
/// </summary>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>
/// WalletBalanceResponse containing total balance and individual token balances (USDC, ETH, MATIC)
/// </returns>
/// <response code="200">Balance retrieved successfully</response>
/// <response code="401">User not authenticated or token invalid</response>
/// <response code="500">Failed to fetch balance from blockchain</response>
[HttpGet("balance")]
[ProducesResponseType(typeof(WalletBalanceResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<WalletBalanceResponse>> GetBalance(CancellationToken cancellationToken)
{
    // Implementation
}
```

### Example 2: POST with Request Body
```csharp
/// <summary>
/// Initiates a gasless USDC transfer to another wallet
/// </summary>
/// <param name="request">
/// TransferRequest containing:
/// - ToAddress: Destination Ethereum address (42 chars, starts with 0x)
/// - Amount: Amount in USDC (decimal, must be > 0)
/// - TokenAddress: Optional token contract address (defaults to USDC)
/// </param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>
/// TransferResponse containing transaction ID, user operation hash, and submission timestamp
/// </returns>
/// <response code="200">Transfer submitted successfully for processing</response>
/// <response code="400">
/// Bad request. Invalid recipient address, insufficient balance, or invalid amount.
/// </response>
/// <response code="401">User not authenticated</response>
/// <response code="404">Wallet not found. User must create wallet first.</response>
/// <response code="500">Failed to submit transfer to bundler service</response>
/// <remarks>
/// The transfer is submitted to the bundler as a UserOperation and processed asynchronously.
/// Use GET /api/transaction/{id}/status to monitor completion.
/// </remarks>
[HttpPost("transfer")]
[ProducesResponseType(typeof(TransferResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<TransferResponse>> SubmitTransfer(
    [FromBody] TransferRequest request,
    CancellationToken cancellationToken)
{
    // Implementation
}
```

### Example 3: Complex GET with Query Parameters
```csharp
/// <summary>
/// Retrieves transaction history for authenticated user with advanced filtering and pagination
/// </summary>
/// <param name="page">
/// Page number (1-based, default: 1). Must be >= 1.
/// </param>
/// <param name="pageSize">
/// Number of transactions per page (1-100, default: 20). Capped at 100 for performance.
/// </param>
/// <param name="status">
/// Optional. Filter by transaction status: Pending, Confirmed, Failed
/// </param>
/// <param name="startDate">
/// Optional. Include only transactions after this date (inclusive)
/// </param>
/// <param name="endDate">
/// Optional. Include only transactions before this date (inclusive)
/// </param>
/// <param name="minAmount">
/// Optional. Minimum transaction amount in USDC
/// </param>
/// <param name="maxAmount">
/// Optional. Maximum transaction amount in USDC
/// </param>
/// <param name="sortBy">
/// Field to sort by: CreatedAt (default), Amount, Status, ConfirmedAt
/// </param>
/// <param name="sortDescending">
/// Sort direction (default: true for descending). Set to false for ascending.
/// </param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>
/// TransactionHistoryResponse containing paginated list of TransactionStatusResponse objects with:
/// - Transaction details (hash, status, amounts)
/// - Pagination metadata (total count, current page, page size)
/// - Block explorer URLs for verification
/// </returns>
/// <response code="200">Transactions retrieved successfully</response>
/// <response code="401">User not authenticated</response>
/// <response code="404">User wallet not found</response>
/// <response code="500">Database query failed</response>
/// <remarks>
/// All date filters use UTC time. Empty date range defaults to showing last 90 days.
///
/// Example: GET /api/transaction/history?page=1&pageSize=20&status=Confirmed&sortBy=CreatedAt&sortDescending=true
///
/// Response includes JiffyScan and block explorer URLs for external transaction verification.
/// </remarks>
[HttpGet("history")]
[ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<TransactionHistoryResponse>> GetTransactionHistory(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? status = null,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] decimal? minAmount = null,
    [FromQuery] decimal? maxAmount = null,
    [FromQuery] string sortBy = "CreatedAt",
    [FromQuery] bool sortDescending = true,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### Example 4: DELETE Operation
```csharp
/// <summary>
/// Deletes a webhook registration
/// </summary>
/// <param name="id">Webhook ID (must be positive integer)</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>No content (HTTP 204) on successful deletion</returns>
/// <response code="204">Webhook deleted successfully</response>
/// <response code="401">User not authenticated</response>
/// <response code="404">Webhook not found or user does not own this webhook</response>
/// <response code="500">Database deletion failed</response>
/// <remarks>
/// This operation is permanent and cannot be undone.
/// All delivery logs for this webhook are also deleted.
/// </remarks>
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult> DeleteWebhook(int id, CancellationToken cancellationToken)
{
    // Implementation
}
```

---

## ProducesResponseType Attributes

**Always pair XML docs with ProducesResponseType attributes:**

```csharp
// For successful response with body
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]

// For created response with body
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]

// For no content response
[ProducesResponseType(StatusCodes.Status204NoContent)]

// For error responses (no need for typeof, just status code)
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
```

---

## Common Patterns

### Authenticated Endpoint with Resource Access
```csharp
/// <summary>
/// Retrieves bank account details for authenticated user
/// </summary>
/// <param name="id">Bank account ID</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>
/// BankAccountResponse with masked account number and routing number
/// </returns>
/// <response code="200">Bank account retrieved successfully</response>
/// <response code="401">User not authenticated</response>
/// <response code="404">Bank account not found or user does not own this account</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(BankAccountResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<BankAccountResponse>> GetBankAccountById(Guid id, CancellationToken cancellationToken)
```

### Public Endpoint (No Authentication)
```csharp
/// <summary>
/// Gets current USDC to USD exchange rate
/// </summary>
/// <returns>
/// ExchangeRateResponse containing current rate, timestamp, and cache metadata
/// </returns>
/// <response code="200">Exchange rate retrieved successfully</response>
/// <response code="503">Exchange rate service unavailable, try again later</response>
[HttpGet("usdc-usd")]
[AllowAnonymous]
[ProducesResponseType(typeof(ExchangeRateResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
public async Task<ActionResult<ExchangeRateResponse>> GetUsdcToUsdRate()
```

---

## Checklist for Documentation

For each controller endpoint, verify:

- [ ] **Summary:** One-line description of what endpoint does
- [ ] **Parameters:** All parameters documented with constraints
- [ ] **Returns:** Response type and structure described
- [ ] **Response Codes:** All possible HTTP status codes documented
- [ ] **Remarks:** Additional context or examples (if needed)
- [ ] **ProducesResponseType:** Matches documented response codes
- [ ] **Error Scenarios:** 400, 401, 404, 500 documented if applicable
- [ ] **Security Notes:** Authorization requirements documented
- [ ] **Examples:** Usage examples in remarks for complex endpoints
- [ ] **Links:** References to related endpoints or documentation

---

## Tools and Validation

### Swagger/OpenAPI Generation
The XML documentation is automatically included in Swagger/OpenAPI specs when using Swashbuckle:

```bash
# The generated swagger.json will include all <summary>, <param>, <returns>, <response> tags
# View at: http://localhost:port/swagger/v1/swagger.json
```

### Validation
Check documentation completeness:
```bash
cd D:/Projects/Test/Claude/CoinPay
# Build will generate warnings if XML docs reference non-existent members
dotnet build
```

---

## Best Practices

1. **Be Specific:** Instead of "Gets data", say "Retrieves wallet address and token balances"
2. **Include Constraints:** Document valid ranges, formats, and required vs optional
3. **Use Active Voice:** "Initiates transfer", not "Transfer is initiated"
4. **Document Error Causes:** Explain why 400 occurs (validation error), 401 (auth), 404 (not found)
5. **Provide Examples:** For complex queries, include example URLs
6. **Keep Current:** Update docs when endpoint behavior changes
7. **Be Consistent:** Use same terminology across all endpoints
8. **Include Links:** Reference related endpoints where relevant

---

## Controller-Specific Guidelines

### Authentication Controllers
- Document JWT token format required
- Document claim fields expected
- Document token refresh endpoints

### Webhook Endpoints
- Document signature validation format
- Document event types available
- Document delivery retry logic

### Payment/Transaction Controllers
- Document amount precision (e.g., "6 decimals for USDC")
- Document supported currencies/tokens
- Document status transitions

### Admin Endpoints
- Mark with [Authorize(Roles = "Admin")]
- Document permission requirements
- Warn about system-wide impact

---

## Swagger UI Tips

To make your API easier to use:
1. Use descriptive OperationId values
2. Add examples to request/response models
3. Group endpoints with consistent Tags
4. Use consistent status code patterns
5. Include example requests/responses in remarks

---

**Last Updated:** 2025-11-06
**Standard Version:** 1.0
**Author:** Architecture Team
