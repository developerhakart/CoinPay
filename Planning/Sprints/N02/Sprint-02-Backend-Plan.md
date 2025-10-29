# CoinPay Wallet MVP - Sprint N02 Backend Plan

**Sprint Duration**: 2 weeks (10 working days)
**Sprint Period**: January 20 - January 31, 2025
**Team Composition**: 2-3 Backend Engineers
**Available Capacity**: 20-30 engineering days
**Planned Effort**: ~26 days (87% utilization)
**Sprint Type**: Feature Enhancement Sprint

---

## Sprint Goal

**Complete Phase 1 (Core Wallet Foundation) and implement Phase 2 (Transaction History & Monitoring) to deliver production-ready transaction management with real-time status updates and comprehensive history APIs.**

By the end of Sprint N02, we will have:
- Phase 1 fully complete (balance caching, transaction status, repositories)
- Background worker for automatic transaction status monitoring
- Transaction history endpoint with pagination, sorting, and filtering
- Transaction detail endpoint with blockchain information
- Webhook support for external status notifications
- Blockchain event listener for real-time updates (stretch goal)
- Performance optimizations (< 1s API response time)
- Comprehensive API documentation

---

## Selected Tasks & Effort Distribution

### Phase 1: Core Wallet Foundation - COMPLETION (4.08 days)
- Balance caching enhancement
- Transaction status endpoint
- Transaction repository completion

### Phase 2: Transaction History & Monitoring (17.00 days)
- Background worker for transaction monitoring
- Transaction status update service
- Transaction history endpoint with pagination
- Transaction history sorting and filtering
- Transaction detail endpoint
- Webhook endpoint for status notifications
- Blockchain event listener
- Performance optimizations

**Total Sprint N02 Effort**: ~21.08 days (within 20-30 day capacity)

---

## Task Breakdown with Details

### Epic 1.4: Phase 1 Completion (4.08 days)

#### BE-108: GET /api/wallet/{address}/balance Enhancement (1.00 day)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-112 (Blockchain RPC Service from Sprint N01)

**Description**:
Enhance wallet balance endpoint with caching and refresh mechanisms.

**Technical Requirements**:
- Implement Redis caching for balance data (30-second TTL)
- Add manual refresh capability
- Add cache invalidation on transaction completion
- Optimize RPC calls to minimize latency

**Implementation**:
```csharp
public class BalanceService : IBalanceService
{
    private readonly IDistributedCache _cache;
    private readonly IBlockchainService _blockchainService;

    public async Task<BalanceResponse> GetBalanceAsync(string address, bool forceRefresh = false)
    {
        var cacheKey = $"balance:{address}";

        if (!forceRefresh)
        {
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<BalanceResponse>(cached);
            }
        }

        // Fetch from blockchain
        var balance = await _blockchainService.GetBalanceAsync(address);

        // Cache for 30 seconds
        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(balance),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

        return balance;
    }

    public async Task InvalidateCacheAsync(string address)
    {
        await _cache.RemoveAsync($"balance:{address}");
    }
}
```

**API Enhancements**:
- GET `/api/wallet/{address}/balance?forceRefresh=true` - Force balance refresh
- POST `/api/wallet/{address}/balance/invalidate` - Invalidate cache

**Acceptance Criteria**:
- [x] Balance cached with 30-second TTL
- [x] Cache invalidated on transaction completion
- [x] Force refresh parameter works
- [x] Response time < 200ms (cached), < 1s (fresh)
- [x] Unit tests for caching logic

**Definition of Done**:
- API tested with high traffic
- Cache hit rate > 80%
- Documentation updated

---

#### BE-110: GET /api/transactions/{id}/status Endpoint (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-109 (Transfer endpoint from Sprint N01), BE-111

**Description**:
Implement transaction status tracking endpoint.

**Technical Requirements**:
- Endpoint: GET `/api/transactions/{id}/status`
- Query bundler for UserOperation receipt
- Parse receipt for transaction hash and status
- Update database with confirmed status
- Return current status with confirmations

**Response DTO**:
```csharp
public class TransactionStatusResponse
{
    public string UserOpHash { get; set; }
    public string TransactionHash { get; set; }
    public TransactionStatus Status { get; set; }  // Pending, Confirmed, Failed
    public DateTime SubmittedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public int Confirmations { get; set; }
    public string BlockNumber { get; set; }
    public string BlockExplorerUrl { get; set; }
}
```

**Business Logic**:
```csharp
public async Task<TransactionStatusResponse> GetTransactionStatusAsync(Guid transactionId)
{
    var transaction = await _transactionRepository.GetByIdAsync(transactionId);
    if (transaction == null)
        throw new NotFoundException("Transaction not found");

    // If already confirmed, return cached status
    if (transaction.Status == TransactionStatus.Confirmed)
        return MapToResponse(transaction);

    // Query bundler for latest status
    var receipt = await _userOperationService.GetReceiptAsync(transaction.UserOpHash);

    if (receipt.Success)
    {
        transaction.TransactionHash = receipt.TransactionHash;
        transaction.Status = TransactionStatus.Confirmed;
        transaction.ConfirmedAt = DateTime.UtcNow;

        await _transactionRepository.UpdateAsync(transaction);

        // Invalidate balance cache
        await _balanceService.InvalidateCacheAsync(transaction.FromAddress);
    }

    return MapToResponse(transaction);
}
```

**Acceptance Criteria**:
- [x] Endpoint returns current transaction status
- [x] Status updates from pending → confirmed/failed
- [x] Transaction hash populated when confirmed
- [x] ConfirmedAt timestamp set on confirmation
- [x] Returns 404 if transaction not found
- [x] Block explorer URL included in response
- [x] Integration tests with mock bundler

**Definition of Done**:
- API documented in Swagger
- Status polling tested
- Frontend can track transactions

---

#### BE-111: Transaction Repository Completion (1.08 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-003 (Database from Sprint N01)

**Description**:
Complete transaction repository with all CRUD operations and queries.

**Repository Interface**:
```csharp
public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> GetByIdAsync(Guid id);
    Task<Transaction> GetByUserOpHashAsync(string userOpHash);
    Task<List<Transaction>> GetByWalletIdAsync(Guid walletId, int limit = 20);
    Task<List<Transaction>> GetByStatusAsync(TransactionStatus status);
    Task<List<Transaction>> GetHistoryAsync(Guid walletId,
        int page, int pageSize, string sortBy, bool ascending,
        TransactionStatus? statusFilter = null,
        DateTime? fromDate = null, DateTime? toDate = null);
    Task<int> GetCountAsync(Guid walletId, TransactionStatus? statusFilter = null);
    Task UpdateAsync(Transaction transaction);
    Task UpdateStatusAsync(Guid transactionId, TransactionStatus status, string txHash = null);
    Task<bool> ExistsAsync(string userOpHash);
}
```

**Implementation Highlights**:
```csharp
public async Task<List<Transaction>> GetHistoryAsync(
    Guid walletId,
    int page,
    int pageSize,
    string sortBy,
    bool ascending,
    TransactionStatus? statusFilter = null,
    DateTime? fromDate = null,
    DateTime? toDate = null)
{
    var query = _context.Transactions
        .Where(t => t.WalletId == walletId);

    // Apply filters
    if (statusFilter.HasValue)
        query = query.Where(t => t.Status == statusFilter.Value);

    if (fromDate.HasValue)
        query = query.Where(t => t.CreatedAt >= fromDate.Value);

    if (toDate.HasValue)
        query = query.Where(t => t.CreatedAt <= toDate.Value);

    // Apply sorting
    query = sortBy.ToLower() switch
    {
        "amount" => ascending
            ? query.OrderBy(t => t.Amount)
            : query.OrderByDescending(t => t.Amount),
        "date" => ascending
            ? query.OrderBy(t => t.CreatedAt)
            : query.OrderByDescending(t => t.CreatedAt),
        _ => query.OrderByDescending(t => t.CreatedAt)  // Default: newest first
    };

    // Apply pagination
    return await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

**Database Indexes**:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Transaction>()
        .HasIndex(t => t.UserOpHash)
        .IsUnique();

    modelBuilder.Entity<Transaction>()
        .HasIndex(t => t.WalletId);

    modelBuilder.Entity<Transaction>()
        .HasIndex(t => new { t.WalletId, t.Status });

    modelBuilder.Entity<Transaction>()
        .HasIndex(t => new { t.WalletId, t.CreatedAt });
}
```

**Acceptance Criteria**:
- [x] Repository performs all CRUD operations
- [x] GetHistoryAsync supports pagination, sorting, filtering
- [x] Database indexes optimize queries
- [x] Unit tests with in-memory database
- [x] Integration tests with PostgreSQL

**Definition of Done**:
- Repository tested with 1000+ records
- Query performance < 100ms
- Code reviewed

---

### Epic 2.1: Transaction Monitoring (5.00 days)

#### BE-201: Background Worker for Transaction Monitoring (3.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-111, BE-113 (UserOperation Service from Sprint N01)

**Description**:
Create background service to automatically monitor and update transaction statuses.

**Technical Requirements**:
- Implement .NET BackgroundService
- Poll pending transactions every 30 seconds
- Query bundler for UserOperation receipts
- Update transaction status in database
- Handle failures and retries (Polly)
- Implement circuit breaker pattern
- Log monitoring activity

**Implementation**:
```csharp
public class TransactionMonitoringService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TransactionMonitoringService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Transaction Monitoring Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorPendingTransactionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in transaction monitoring");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Transaction Monitoring Service stopped");
    }

    private async Task MonitorPendingTransactionsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var userOperationService = scope.ServiceProvider.GetRequiredService<IUserOperationService>();

        // Get all pending transactions
        var pendingTransactions = await transactionRepository.GetByStatusAsync(TransactionStatus.Pending);

        _logger.LogInformation("Monitoring {Count} pending transactions", pendingTransactions.Count);

        foreach (var transaction in pendingTransactions)
        {
            try
            {
                // Query bundler for receipt
                var receipt = await userOperationService.GetReceiptAsync(transaction.UserOpHash);

                if (receipt != null)
                {
                    if (receipt.Success)
                    {
                        await transactionRepository.UpdateStatusAsync(
                            transaction.Id,
                            TransactionStatus.Confirmed,
                            receipt.TransactionHash);

                        _logger.LogInformation(
                            "Transaction {UserOpHash} confirmed with txHash {TxHash}",
                            transaction.UserOpHash,
                            receipt.TransactionHash);
                    }
                    else
                    {
                        await transactionRepository.UpdateStatusAsync(
                            transaction.Id,
                            TransactionStatus.Failed);

                        _logger.LogWarning(
                            "Transaction {UserOpHash} failed",
                            transaction.UserOpHash);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error monitoring transaction {UserOpHash}",
                    transaction.UserOpHash);
            }
        }
    }
}
```

**Configuration**:
```csharp
// Program.cs
builder.Services.AddHostedService<TransactionMonitoringService>();
```

**Acceptance Criteria**:
- [x] Background worker starts on application startup
- [x] Polls pending transactions every 30 seconds
- [x] Updates transaction status correctly
- [x] Handles errors gracefully (retry logic)
- [x] Logs monitoring activity
- [x] Can be stopped gracefully
- [x] Integration tests with test transactions

**Definition of Done**:
- Worker tested with 100+ pending transactions
- Monitoring logs reviewed
- Performance acceptable (< 5s per cycle)
- Code reviewed

---

#### BE-202: Transaction Status Update Service (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-201

**Description**:
Create service to handle transaction status updates with side effects.

**Technical Requirements**:
- Service to update transaction status
- Trigger balance cache invalidation
- Send notifications (webhook, future: email/push)
- Emit events for audit logging
- Handle concurrent updates safely

**Implementation**:
```csharp
public class TransactionStatusUpdateService : ITransactionStatusUpdateService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBalanceService _balanceService;
    private readonly IWebhookService _webhookService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<TransactionStatusUpdateService> _logger;

    public async Task UpdateTransactionStatusAsync(
        Guid transactionId,
        TransactionStatus newStatus,
        string transactionHash = null)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null)
            throw new NotFoundException("Transaction not found");

        // Validate state transition
        if (!IsValidTransition(transaction.Status, newStatus))
            throw new InvalidOperationException(
                $"Invalid status transition from {transaction.Status} to {newStatus}");

        var oldStatus = transaction.Status;

        // Update status
        transaction.Status = newStatus;
        if (newStatus == TransactionStatus.Confirmed)
        {
            transaction.TransactionHash = transactionHash;
            transaction.ConfirmedAt = DateTime.UtcNow;
        }

        await _transactionRepository.UpdateAsync(transaction);

        _logger.LogInformation(
            "Transaction {Id} status updated from {OldStatus} to {NewStatus}",
            transactionId, oldStatus, newStatus);

        // Side effects
        await ExecuteSideEffectsAsync(transaction, oldStatus, newStatus);
    }

    private async Task ExecuteSideEffectsAsync(
        Transaction transaction,
        TransactionStatus oldStatus,
        TransactionStatus newStatus)
    {
        // Invalidate balance cache
        if (newStatus == TransactionStatus.Confirmed)
        {
            await _balanceService.InvalidateCacheAsync(transaction.FromAddress);
            await _balanceService.InvalidateCacheAsync(transaction.ToAddress);
        }

        // Send webhook notification
        await _webhookService.NotifyTransactionStatusChangeAsync(
            transaction, oldStatus, newStatus);

        // Emit event for audit log
        await _eventBus.PublishAsync(new TransactionStatusChangedEvent
        {
            TransactionId = transaction.Id,
            UserOpHash = transaction.UserOpHash,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Timestamp = DateTime.UtcNow
        });
    }

    private bool IsValidTransition(TransactionStatus from, TransactionStatus to)
    {
        return (from, to) switch
        {
            (TransactionStatus.Pending, TransactionStatus.Confirmed) => true,
            (TransactionStatus.Pending, TransactionStatus.Failed) => true,
            _ => false  // No other transitions allowed
        };
    }
}
```

**Acceptance Criteria**:
- [x] Status updates validated (state machine)
- [x] Balance cache invalidated on confirmation
- [x] Webhook notifications sent
- [x] Events emitted for audit log
- [x] Concurrent updates handled safely
- [x] Unit tests for all transitions

**Definition of Done**:
- Service tested with various scenarios
- Side effects verified
- Code reviewed

---

### Epic 2.2: Transaction History API (12.00 days)

#### BE-203: GET /api/transactions/history Endpoint (3.00 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-111

**Description**:
Implement transaction history endpoint with pagination support.

**Technical Requirements**:
- Endpoint: GET `/api/transactions/history?walletId={guid}&page=1&pageSize=20`
- Support pagination (default: page 1, pageSize 20)
- Return transaction list with metadata
- Include wallet information
- Support query by wallet ID or user ID

**Request Parameters**:
```csharp
public class TransactionHistoryRequest
{
    public Guid? WalletId { get; set; }
    public Guid? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

**Response DTO**:
```csharp
public class TransactionHistoryResponse
{
    public List<TransactionDto> Transactions { get; set; }
    public PaginationMetadata Pagination { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public string UserOpHash { get; set; }
    public string TransactionHash { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Amount { get; set; }
    public string FormattedAmount { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string BlockExplorerUrl { get; set; }
}

public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
```

**Implementation**:
```csharp
[HttpGet("history")]
public async Task<ActionResult<TransactionHistoryResponse>> GetHistory(
    [FromQuery] TransactionHistoryRequest request)
{
    // Validate request
    if (!request.WalletId.HasValue && !request.UserId.HasValue)
        return BadRequest("Either WalletId or UserId must be provided");

    Guid walletId;
    if (request.WalletId.HasValue)
    {
        walletId = request.WalletId.Value;
    }
    else
    {
        // Get wallet from user ID
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId.Value);
        if (wallet == null)
            return NotFound("Wallet not found");
        walletId = wallet.Id;
    }

    // Get transactions
    var transactions = await _transactionRepository.GetHistoryAsync(
        walletId, request.Page, request.PageSize, "date", false);

    var totalCount = await _transactionRepository.GetCountAsync(walletId);

    var response = new TransactionHistoryResponse
    {
        Transactions = transactions.Select(MapToDto).ToList(),
        Pagination = new PaginationMetadata
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            HasPrevious = request.Page > 1,
            HasNext = request.Page * request.PageSize < totalCount
        }
    };

    return Ok(response);
}
```

**Acceptance Criteria**:
- [x] Endpoint returns paginated transaction history
- [x] Pagination metadata accurate
- [x] Supports query by walletId or userId
- [x] Returns 400 if neither walletId nor userId provided
- [x] Returns 404 if wallet not found
- [x] Integration tests with 100+ transactions

**Definition of Done**:
- API documented in Swagger
- Performance tested (< 500ms for 100 transactions)
- Frontend can integrate

---

#### BE-204: Transaction History Pagination & Sorting (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-203

**Description**:
Enhance transaction history endpoint with sorting capabilities.

**Query Parameters**:
```csharp
public class TransactionHistoryRequest
{
    public Guid? WalletId { get; set; }
    public Guid? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    // Sorting
    public string SortBy { get; set; } = "date";  // date, amount
    public bool Ascending { get; set; } = false;  // false = descending (newest first)
}
```

**Supported Sorting**:
- `date` - Sort by transaction creation date
- `amount` - Sort by transaction amount

**Examples**:
- GET `/api/transactions/history?sortBy=date&ascending=false` - Newest first (default)
- GET `/api/transactions/history?sortBy=amount&ascending=true` - Smallest amount first

**Implementation**:
```csharp
[HttpGet("history")]
public async Task<ActionResult<TransactionHistoryResponse>> GetHistory(
    [FromQuery] TransactionHistoryRequest request)
{
    // Validation
    if (!IsValidSortField(request.SortBy))
        return BadRequest($"Invalid sort field: {request.SortBy}");

    // Get transactions with sorting
    var transactions = await _transactionRepository.GetHistoryAsync(
        walletId,
        request.Page,
        request.PageSize,
        request.SortBy,
        request.Ascending);

    // ... rest of implementation
}

private bool IsValidSortField(string sortBy)
{
    return sortBy.ToLower() switch
    {
        "date" => true,
        "amount" => true,
        _ => false
    };
}
```

**Acceptance Criteria**:
- [x] Sorting by date works (ascending/descending)
- [x] Sorting by amount works (ascending/descending)
- [x] Returns 400 for invalid sort fields
- [x] Default sorting: date descending (newest first)
- [x] Performance acceptable with sorting (< 500ms)
- [x] Integration tests for all sorting combinations

**Definition of Done**:
- API documented with sorting examples
- Performance tested
- Frontend can use sorting

---

#### BE-205: Transaction Filtering (Status, Date, Amount) (2.00 days)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-203

**Description**:
Add filtering capabilities to transaction history endpoint.

**Query Parameters**:
```csharp
public class TransactionHistoryRequest
{
    // ... existing parameters

    // Filtering
    public TransactionStatus? Status { get; set; }  // Pending, Confirmed, Failed
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}
```

**Filter Examples**:
- GET `/api/transactions/history?status=Confirmed` - Only confirmed transactions
- GET `/api/transactions/history?fromDate=2025-01-01&toDate=2025-01-31` - January transactions
- GET `/api/transactions/history?minAmount=100&maxAmount=1000` - Amount range

**Implementation**:
```csharp
public async Task<List<Transaction>> GetHistoryAsync(
    Guid walletId,
    int page,
    int pageSize,
    string sortBy,
    bool ascending,
    TransactionStatus? statusFilter = null,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    decimal? minAmount = null,
    decimal? maxAmount = null)
{
    var query = _context.Transactions.Where(t => t.WalletId == walletId);

    // Apply filters
    if (statusFilter.HasValue)
        query = query.Where(t => t.Status == statusFilter.Value);

    if (fromDate.HasValue)
        query = query.Where(t => t.CreatedAt >= fromDate.Value);

    if (toDate.HasValue)
        query = query.Where(t => t.CreatedAt <= toDate.Value);

    if (minAmount.HasValue)
        query = query.Where(t => decimal.Parse(t.Amount) >= minAmount.Value);

    if (maxAmount.HasValue)
        query = query.Where(t => decimal.Parse(t.Amount) <= maxAmount.Value);

    // ... apply sorting and pagination
}
```

**Acceptance Criteria**:
- [x] Status filtering works (Pending, Confirmed, Failed)
- [x] Date range filtering works
- [x] Amount range filtering works
- [x] Multiple filters can be combined
- [x] Filters return accurate results
- [x] Performance acceptable with filters (< 500ms)
- [x] Integration tests for all filter combinations

**Definition of Done**:
- API documented with filter examples
- Performance tested
- Frontend can use filters

---

#### BE-206: GET /api/transactions/{id}/details Endpoint (1.00 day)
**Owner**: Backend Engineer
**Priority**: P1 - High
**Dependencies**: BE-111

**Description**:
Implement transaction detail endpoint with complete information.

**Response DTO**:
```csharp
public class TransactionDetailResponse
{
    public Guid Id { get; set; }
    public string UserOpHash { get; set; }
    public string TransactionHash { get; set; }

    // Addresses
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }

    // Amount
    public string Amount { get; set; }
    public string FormattedAmount { get; set; }
    public string TokenAddress { get; set; }
    public string TokenSymbol { get; set; } = "USDC";

    // Status
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    // Blockchain info
    public string BlockNumber { get; set; }
    public int Confirmations { get; set; }
    public string GasUsed { get; set; }
    public string GasPaidByUser { get; set; } = "0";  // Gasless!
    public string BlockExplorerUrl { get; set; }

    // User Operation details
    public string Nonce { get; set; }
    public string Signature { get; set; }
}
```

**Implementation**:
```csharp
[HttpGet("{id}/details")]
public async Task<ActionResult<TransactionDetailResponse>> GetDetails(Guid id)
{
    var transaction = await _transactionRepository.GetByIdAsync(id);
    if (transaction == null)
        return NotFound();

    var response = new TransactionDetailResponse
    {
        Id = transaction.Id,
        UserOpHash = transaction.UserOpHash,
        TransactionHash = transaction.TransactionHash,
        FromAddress = transaction.FromAddress,
        ToAddress = transaction.ToAddress,
        Amount = transaction.Amount,
        FormattedAmount = FormatAmount(transaction.Amount),
        Status = transaction.Status,
        CreatedAt = transaction.CreatedAt,
        ConfirmedAt = transaction.ConfirmedAt,
        BlockExplorerUrl = GetBlockExplorerUrl(transaction.TransactionHash)
    };

    // If confirmed, fetch blockchain details
    if (transaction.Status == TransactionStatus.Confirmed && !string.IsNullOrEmpty(transaction.TransactionHash))
    {
        var receipt = await _blockchainService.GetTransactionReceiptAsync(transaction.TransactionHash);
        if (receipt != null)
        {
            response.BlockNumber = receipt.BlockNumber;
            response.Confirmations = await _blockchainService.GetConfirmationsAsync(receipt.BlockNumber);
            response.GasUsed = receipt.GasUsed;
        }
    }

    return Ok(response);
}
```

**Acceptance Criteria**:
- [x] Endpoint returns complete transaction details
- [x] Blockchain information included (if confirmed)
- [x] Block explorer URL generated
- [x] Returns 404 if transaction not found
- [x] Performance acceptable (< 1s)
- [x] Integration tests

**Definition of Done**:
- API documented
- Frontend can display details
- Code reviewed

---

#### BE-207: Webhook Endpoint for Transaction Status (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P2 - Medium
**Dependencies**: BE-202

**Description**:
Implement webhook endpoint to notify external systems of transaction status changes.

**Technical Requirements**:
- POST endpoint to register webhook URLs
- Webhook payload with transaction status
- Signature verification (HMAC)
- Retry logic for failed webhook deliveries
- Webhook history and logs

**Webhook Registration**:
```csharp
public class WebhookRegistration
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Url { get; set; }
    public string Secret { get; set; }  // For HMAC signature
    public List<string> Events { get; set; }  // transaction.confirmed, transaction.failed
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

[HttpPost("webhooks")]
public async Task<ActionResult<WebhookRegistration>> RegisterWebhook(
    [FromBody] RegisterWebhookRequest request)
{
    // Validate URL
    if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
        return BadRequest("Invalid webhook URL");

    var webhook = new WebhookRegistration
    {
        UserId = GetCurrentUserId(),
        Url = request.Url,
        Secret = GenerateWebhookSecret(),
        Events = request.Events ?? new List<string> { "transaction.confirmed", "transaction.failed" },
        IsActive = true
    };

    await _webhookRepository.CreateAsync(webhook);

    return CreatedAtAction(nameof(GetWebhook), new { id = webhook.Id }, webhook);
}
```

**Webhook Payload**:
```csharp
public class WebhookPayload
{
    public string Event { get; set; }  // transaction.confirmed, transaction.failed
    public DateTime Timestamp { get; set; }
    public TransactionData Transaction { get; set; }
    public string Signature { get; set; }  // HMAC-SHA256
}

public class TransactionData
{
    public Guid Id { get; set; }
    public string UserOpHash { get; set; }
    public string TransactionHash { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Amount { get; set; }
    public TransactionStatus Status { get; set; }
}
```

**Webhook Service**:
```csharp
public class WebhookService : IWebhookService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebhookRepository _webhookRepository;

    public async Task NotifyTransactionStatusChangeAsync(
        Transaction transaction,
        TransactionStatus oldStatus,
        TransactionStatus newStatus)
    {
        var eventName = newStatus switch
        {
            TransactionStatus.Confirmed => "transaction.confirmed",
            TransactionStatus.Failed => "transaction.failed",
            _ => null
        };

        if (eventName == null) return;

        // Get webhooks for this user
        var webhooks = await _webhookRepository.GetActiveWebhooksForUserAsync(transaction.UserId);

        foreach (var webhook in webhooks.Where(w => w.Events.Contains(eventName)))
        {
            await SendWebhookAsync(webhook, transaction, eventName);
        }
    }

    private async Task SendWebhookAsync(
        WebhookRegistration webhook,
        Transaction transaction,
        string eventName)
    {
        var payload = new WebhookPayload
        {
            Event = eventName,
            Timestamp = DateTime.UtcNow,
            Transaction = MapToTransactionData(transaction),
            Signature = GenerateSignature(webhook.Secret, transaction)
        };

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        var response = await client.PostAsJsonAsync(webhook.Url, payload);

        // Log webhook delivery
        await _webhookRepository.LogDeliveryAsync(new WebhookDeliveryLog
        {
            WebhookId = webhook.Id,
            EventName = eventName,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode,
            Timestamp = DateTime.UtcNow
        });
    }

    private string GenerateSignature(string secret, Transaction transaction)
    {
        var data = $"{transaction.Id}{transaction.UserOpHash}{transaction.Status}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }
}
```

**Acceptance Criteria**:
- [x] Webhook registration endpoint works
- [x] Webhooks sent on transaction status change
- [x] HMAC signature included in payload
- [x] Retry logic for failed deliveries (3 attempts)
- [x] Webhook delivery logs stored
- [x] Integration tests with mock webhook server

**Definition of Done**:
- API documented
- Webhook examples provided
- Code reviewed

---

#### BE-208: Blockchain Event Listener (STRETCH GOAL) (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P3 - Low
**Dependencies**: BE-112 (Blockchain RPC Service from Sprint N01)

**Description**:
Implement real-time blockchain event listener for transaction confirmations.

**Technical Requirements**:
- Subscribe to Polygon Amoy blockchain events
- Listen for transaction confirmation events
- Update transaction status in real-time
- Handle reconnection on network failure
- Log event processing

**Note**: This is a stretch goal. If time is tight, defer to Sprint N03 and continue using polling mechanism.

**Implementation**:
```csharp
public class BlockchainEventListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BlockchainEventListener> _logger;
    private readonly string _rpcUrl;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Blockchain Event Listener started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ListenForEventsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in blockchain event listener");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);  // Retry after 10s
            }
        }

        _logger.LogInformation("Blockchain Event Listener stopped");
    }

    private async Task ListenForEventsAsync(CancellationToken stoppingToken)
    {
        // Subscribe to new blocks
        var web3 = new Web3(_rpcUrl);
        var filterBlocks = await web3.Eth.Filters.NewBlockFilter.SendRequestAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            var blocks = await web3.Eth.Filters.GetFilterChangesForBlockFilter.SendRequestAsync(filterBlocks);

            foreach (var blockHash in blocks)
            {
                await ProcessBlockAsync(blockHash);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessBlockAsync(string blockHash)
    {
        using var scope = _scopeFactory.CreateScope();
        var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Get block transactions
        var web3 = new Web3(_rpcUrl);
        var block = await web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(blockHash);

        foreach (var tx in block.Transactions)
        {
            // Check if this transaction is in our database
            var transaction = await transactionRepository.GetByTransactionHashAsync(tx.TransactionHash);

            if (transaction != null && transaction.Status == TransactionStatus.Pending)
            {
                _logger.LogInformation(
                    "Transaction {TxHash} confirmed in block {BlockHash}",
                    tx.TransactionHash,
                    blockHash);

                await transactionRepository.UpdateStatusAsync(
                    transaction.Id,
                    TransactionStatus.Confirmed,
                    tx.TransactionHash);
            }
        }
    }
}
```

**Acceptance Criteria**:
- [x] Event listener subscribes to blockchain events
- [x] Transaction statuses updated in real-time
- [x] Handles network disconnections gracefully
- [x] Logging comprehensive
- [x] Integration tests with test blockchain

**Definition of Done**:
- Event listener tested
- Performance acceptable
- Code reviewed

**Note**: If not completed in Sprint N02, defer to Sprint N03.

---

## Daily Milestone Plan

### Days 1-2 (Sprint Start)
**Focus**: Phase 1 completion and monitoring worker

**Tasks**:
- BE-108: Balance caching enhancement
- BE-110: Transaction status endpoint
- BE-111: Transaction repository completion
- BE-201: Background worker (started)

**Deliverable**: Phase 1 complete, monitoring worker in progress

---

### Days 3-4
**Focus**: Transaction monitoring and history

**Tasks**:
- BE-201: Background worker (completed)
- BE-202: Status update service
- BE-203: Transaction history endpoint (started)

**Deliverable**: Transaction monitoring operational

---

### Days 5-6 (Mid-Sprint)
**Focus**: History API completion

**Tasks**:
- BE-203: History endpoint (completed)
- BE-204: Pagination & sorting
- BE-205: Filtering (started)

**Checkpoint Meeting**: Demo transaction monitoring and history API

**Deliverable**: Transaction history API functional

---

### Days 7-8
**Focus**: Advanced features

**Tasks**:
- BE-205: Filtering (completed)
- BE-206: Transaction detail endpoint
- BE-207: Webhook support (started)

**Deliverable**: Complete transaction history with filtering

---

### Days 9-10 (Sprint End)
**Focus**: Polish and testing

**Tasks**:
- BE-207: Webhook support (completed)
- BE-208: Blockchain event listener (stretch goal)
- Performance testing
- Documentation updates

**Sprint Review**: Demo all Phase 2 backend features

**Deliverable**: Production-ready transaction management

---

## Risks, Dependencies & Mitigation

### Technical Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Background worker complexity | MEDIUM | HIGH | Allocate senior engineer, start Day 1, pair programming |
| Transaction history performance | MEDIUM | HIGH | Database indexing, caching, early performance testing |
| Webhook delivery failures | LOW | MEDIUM | Implement retry logic, use Polly |
| Blockchain event listener complexity | MEDIUM | LOW | Make this a stretch goal, defer if needed |

### External Dependencies

| Dependency | Risk Level | Impact | Mitigation |
|------------|------------|--------|------------|
| Circle bundler availability | LOW | HIGH | Retry logic, polling fallback |
| Polygon Amoy RPC | LOW | MEDIUM | Multiple RPC endpoints, retry logic |
| Frontend integration | MEDIUM | MEDIUM | Use mock data, API contract first |

---

## Success Criteria

### Functional Success Metrics

- [ ] Background worker runs continuously and updates transaction statuses
- [ ] Transaction history endpoint returns paginated results
- [ ] Sorting and filtering work correctly
- [ ] Transaction detail endpoint returns complete information
- [ ] Webhook notifications sent on status changes
- [ ] All API endpoints documented in Swagger

### Quality Gates

- [ ] Unit test coverage > 80% for new services
- [ ] Integration tests pass for all endpoints
- [ ] Performance: API response time < 1s (P95)
- [ ] Performance: Background worker processes 100+ transactions in < 5s
- [ ] Code reviewed and approved
- [ ] No Critical or High severity bugs

### Sprint Review Demo Checklist

1. Show background worker monitoring pending transactions
2. Demonstrate transaction status updates in real-time
3. Show transaction history with pagination
4. Demonstrate sorting (date, amount)
5. Demonstrate filtering (status, date range, amount range)
6. Show transaction detail endpoint
7. Show webhook registration and delivery
8. Performance test results (100+ concurrent users)

---

## Handoff Points

### To Frontend Team (Day 3)

**Deliverables**:
- Transaction history API contract (pagination, sorting, filtering)
- Transaction detail API response format
- Webhook payload structure
- Postman collection with examples

**Coordination**:
- Review API contract together
- Agree on filter parameters
- Define error handling conventions

---

### To QA Team (Day 7)

**Deliverables**:
- All API endpoints deployed to test environment
- Test data: 100+ transactions in various states
- API documentation with examples
- Test scenarios for history, sorting, filtering

**Testing Scenarios**:
1. Transaction monitoring (status updates)
2. Transaction history (pagination, sorting, filtering)
3. Transaction detail (blockchain information)
4. Webhook delivery (registration, notification)
5. Performance testing (100+ concurrent users)

---

## Definition of Done (Sprint N02)

### Code Quality

- [x] All code follows .NET coding conventions
- [x] SOLID principles applied
- [x] Async/await used consistently
- [x] XML documentation comments for public APIs
- [x] No critical SonarQube issues
- [x] Code reviewed and approved

### Testing

- [x] Unit tests for service layer (>80% coverage)
- [x] Integration tests for all endpoints
- [x] Performance tests with 100+ concurrent users
- [x] Manual testing of background worker
- [x] Webhook delivery tested with mock server

### Documentation

- [x] Swagger documentation complete
- [x] Architecture decision records (ADRs)
- [x] API integration guide updated
- [x] Background worker configuration documented

---

## Appendix: Useful Commands

**Run Background Worker**:
```bash
dotnet run --project CoinPay.Api
```

**Test Transaction History**:
```bash
curl -X GET "http://localhost:5000/api/transactions/history?walletId={guid}&page=1&pageSize=20&sortBy=date&status=Confirmed"
```

**Register Webhook**:
```bash
curl -X POST "http://localhost:5000/api/webhooks" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://example.com/webhook",
    "events": ["transaction.confirmed", "transaction.failed"]
  }'
```

---

**Sprint N02 Backend Plan Version**: 1.0
**Last Updated**: 2025-10-28
**Status**: Ready for Execution
**Next Steps**: Day 1 - Start BE-108, BE-110, BE-111, BE-201

---

**End of Sprint N02 Backend Plan**
