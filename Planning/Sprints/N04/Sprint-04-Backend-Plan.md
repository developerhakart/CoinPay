# Sprint N04 - Backend Engineering Plan
# Phase 4: Exchange Investment (WhiteBit Integration)

**Sprint**: N04
**Duration**: 2 weeks (10 working days)
**Sprint Dates**: February 17 - February 28, 2025
**Total Effort**: 33.00 days
**Team Size**: 2-3 engineers
**Utilization**: 110% ⚠️ (requires 3 engineers or scope reduction)

---

## Sprint Goal

Integrate WhiteBit Flex Investment API to enable users to earn yield on USDC holdings through secure, encrypted investment positions with real-time reward tracking.

---

## Epic Breakdown

### Epic 1: WhiteBit API Integration & Authentication (10.00 days)

**Description**: Implement secure WhiteBit Flex API client with HMAC-SHA256 authentication and user-level encrypted credential storage.

**Tasks**:

#### BE-401: WhiteBit API Client Implementation (3.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Create a robust WhiteBit API client with proper authentication, error handling, and rate limiting.

**Requirements**:
- RestSharp HTTP client configured for WhiteBit API
- HMAC-SHA256 signature generation for requests
- Request/response serialization (JSON)
- Retry policy with exponential backoff (Polly)
- Rate limiting (100 req/min)
- Comprehensive error handling and logging

**Implementation**:
```csharp
// Services/Exchange/WhiteBit/WhiteBitApiClient.cs
public class WhiteBitApiClient : IWhiteBitApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WhiteBitApiClient> _logger;

    public async Task<WhiteBitResponse<T>> SendRequestAsync<T>(
        string endpoint,
        HttpMethod method,
        object? body = null,
        string apiKey = null,
        string apiSecret = null)
    {
        // Generate nonce
        // Create signature (HMAC-SHA256)
        // Add headers
        // Send request
        // Handle response
        // Log and return
    }
}
```

**API Endpoints to Support**:
- Base URL: `https://whitebit.com`
- Sandbox URL: `https://sandbox.whitebit.com` (if available)
- `/api/v4/main-account/balance`
- `/api/v4/main-account/investments`
- `/api/v4/main-account/investments/create`
- `/api/v4/main-account/investments/{id}/close`

**Acceptance Criteria**:
- [ ] WhiteBit API client successfully authenticates with sandbox
- [ ] All 4 required endpoints accessible
- [ ] Rate limiting prevents exceeding 100 req/min
- [ ] Retry policy handles transient failures
- [ ] Comprehensive logging with correlation IDs
- [ ] Unit tests cover all methods
- [ ] Integration tests pass with sandbox API

**Dependencies**: None

---

#### BE-402: WhiteBit Authentication Service (2.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Implement authentication service to handle API key/secret validation and signature generation.

**Requirements**:
- Validate API key format
- Generate HMAC-SHA256 signatures for requests
- Handle nonce generation (timestamp-based)
- Test API connectivity with credentials
- Return authentication errors clearly

**Implementation**:
```csharp
// Services/Exchange/WhiteBit/WhiteBitAuthService.cs
public class WhiteBitAuthService : IWhiteBitAuthService
{
    public string GenerateSignature(
        string apiSecret,
        string method,
        string path,
        string body,
        long nonce)
    {
        // HMAC-SHA256(secret, method + path + nonce + body)
    }

    public async Task<bool> ValidateCredentials(
        string apiKey,
        string apiSecret)
    {
        // Test /api/v4/main-account/balance
        // Return true if 200, false if 401
    }
}
```

**Acceptance Criteria**:
- [ ] Signature generation matches WhiteBit specification
- [ ] Credential validation endpoint functional
- [ ] Authentication errors return clear messages
- [ ] Nonce prevents replay attacks
- [ ] Unit tests for signature generation
- [ ] Integration tests with sandbox API

**Dependencies**: BE-401

---

#### BE-403: API Credential Storage (User-Level Encryption) (3.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 (Security Critical)

**Description**:
Implement secure, user-level encrypted storage for WhiteBit API credentials.

**Requirements**:
- User-level encryption (unique key per user)
- AES-256-GCM encryption for API key and secret
- Encrypted storage in database
- Key derivation from user credentials or secure vault
- Audit logging for all credential operations

**Database Schema**:
```sql
CREATE TABLE exchange_connections (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    exchange_name VARCHAR(50) NOT NULL, -- 'whitebit'
    api_key_encrypted TEXT NOT NULL,
    api_secret_encrypted TEXT NOT NULL,
    encryption_key_id VARCHAR(255), -- Key ID for rotation
    is_active BOOLEAN DEFAULT true,
    last_validated_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(user_id, exchange_name)
);

CREATE INDEX idx_exchange_connections_user ON exchange_connections(user_id);
```

**Implementation**:
```csharp
// Models/ExchangeConnection.cs
public class ExchangeConnection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ExchangeName { get; set; } // "whitebit"
    public string ApiKeyEncrypted { get; set; }
    public string ApiSecretEncrypted { get; set; }
    public string EncryptionKeyId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Services/Encryption/IExchangeCredentialEncryptionService.cs
public interface IExchangeCredentialEncryptionService
{
    Task<string> EncryptAsync(string plaintext, Guid userId);
    Task<string> DecryptAsync(string ciphertext, Guid userId);
}
```

**Acceptance Criteria**:
- [ ] API credentials encrypted with AES-256-GCM
- [ ] Each user has unique encryption key
- [ ] Credentials decrypt correctly for authorized user
- [ ] Audit log records all credential operations
- [ ] Database migration created
- [ ] Unit tests for encryption/decryption
- [ ] Security review passed

**Dependencies**: Phase 3 encryption service

---

#### BE-404: POST /api/exchange/whitebit/connect (1.00 day)
**Owner**: Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Create API endpoint to connect user's WhiteBit account with encrypted credential storage.

**API Specification**:
```
POST /api/exchange/whitebit/connect
Authorization: Bearer {jwt_token}

Request:
{
  "apiKey": "whitebit-api-key",
  "apiSecret": "whitebit-api-secret"
}

Response (200 OK):
{
  "connectionId": "uuid",
  "exchangeName": "whitebit",
  "status": "active",
  "connectedAt": "2025-02-17T10:00:00Z"
}

Response (400 Bad Request):
{
  "error": "Invalid API credentials"
}

Response (409 Conflict):
{
  "error": "WhiteBit account already connected"
}
```

**Implementation**:
```csharp
// Controllers/ExchangeController.cs
[HttpPost("whitebit/connect")]
public async Task<IActionResult> ConnectWhiteBit(
    [FromBody] ConnectWhiteBitRequest request)
{
    // 1. Validate credentials with WhiteBit API
    // 2. Encrypt API key and secret
    // 3. Store in exchange_connections table
    // 4. Return connection status
}
```

**Acceptance Criteria**:
- [ ] Endpoint validates WhiteBit credentials
- [ ] Credentials encrypted before storage
- [ ] Returns 409 if already connected
- [ ] Returns clear error for invalid credentials
- [ ] Audit log records connection
- [ ] Swagger documentation complete
- [ ] Integration tests pass

**Dependencies**: BE-401, BE-402, BE-403

---

#### BE-405: GET /api/exchange/whitebit/status (1.00 day)
**Owner**: Backend Engineer
**Priority**: P1 (High)

**Description**:
Create API endpoint to check user's WhiteBit connection status.

**API Specification**:
```
GET /api/exchange/whitebit/status
Authorization: Bearer {jwt_token}

Response (200 OK - Connected):
{
  "connected": true,
  "connectionId": "uuid",
  "exchangeName": "whitebit",
  "connectedAt": "2025-02-17T10:00:00Z",
  "lastValidated": "2025-02-28T09:00:00Z"
}

Response (200 OK - Not Connected):
{
  "connected": false
}
```

**Acceptance Criteria**:
- [ ] Returns connection status
- [ ] Returns last validation timestamp
- [ ] Response time < 500ms (cached)
- [ ] Swagger documentation complete
- [ ] Unit tests pass

**Dependencies**: BE-403

---

### Epic 2: Investment Plans & Creation (8.00 days)

#### BE-406: GET /api/exchange/whitebit/plans (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Fetch available WhiteBit Flex investment plans with APY rates, minimums, and terms.

**WhiteBit API Endpoint**:
```
GET /api/v4/main-account/flex-plans
Response:
{
  "plans": [
    {
      "id": "flex-usdc-1",
      "asset": "USDC",
      "apy": "8.50",
      "minAmount": "100",
      "maxAmount": "100000",
      "term": "flexible",
      "description": "Flexible USDC Flex Plan"
    }
  ]
}
```

**Our API Specification**:
```
GET /api/exchange/whitebit/plans
Authorization: Bearer {jwt_token}

Response (200 OK):
{
  "plans": [
    {
      "planId": "flex-usdc-1",
      "asset": "USDC",
      "apy": 8.50,
      "apyFormatted": "8.50%",
      "minAmount": 100.0,
      "maxAmount": 100000.0,
      "term": "flexible",
      "description": "Earn 8.5% APY on USDC with flexible withdrawal"
    }
  ]
}

Response (401 Unauthorized):
{
  "error": "WhiteBit account not connected"
}
```

**Caching Strategy**:
- Cache plans for 5 minutes (Redis)
- Invalidate cache when new plans detected
- Fallback to database-cached plans if API fails

**Acceptance Criteria**:
- [ ] Fetches plans from WhiteBit API
- [ ] Plans cached for 5 minutes
- [ ] Returns formatted APY values
- [ ] Returns clear error if not connected
- [ ] Response time < 1s (with cache)
- [ ] Swagger documentation complete
- [ ] Integration tests with sandbox

**Dependencies**: BE-401, BE-402, BE-403

---

#### BE-407: Investment Position Model & Repository (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Create database models and repository for investment positions.

**Database Schema**:
```sql
CREATE TABLE investment_positions (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    exchange_connection_id UUID NOT NULL REFERENCES exchange_connections(id),
    exchange_name VARCHAR(50) NOT NULL, -- 'whitebit'
    external_position_id VARCHAR(255), -- WhiteBit position ID
    plan_id VARCHAR(100) NOT NULL,
    asset VARCHAR(20) NOT NULL, -- 'USDC'
    principal_amount NUMERIC(20, 8) NOT NULL,
    current_value NUMERIC(20, 8) NOT NULL,
    accrued_rewards NUMERIC(20, 8) DEFAULT 0,
    apy NUMERIC(10, 4) NOT NULL,
    status VARCHAR(20) NOT NULL, -- 'pending', 'active', 'closed', 'failed'
    start_date TIMESTAMP,
    end_date TIMESTAMP,
    last_synced_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE INDEX idx_investment_positions_user ON investment_positions(user_id);
CREATE INDEX idx_investment_positions_status ON investment_positions(status);
CREATE INDEX idx_investment_positions_external ON investment_positions(external_position_id);
CREATE INDEX idx_investment_positions_created ON investment_positions(created_at DESC);

CREATE TABLE investment_transactions (
    id UUID PRIMARY KEY,
    investment_position_id UUID NOT NULL REFERENCES investment_positions(id),
    user_id UUID NOT NULL REFERENCES users(id),
    transaction_type VARCHAR(20) NOT NULL, -- 'create', 'deposit', 'withdraw', 'reward'
    amount NUMERIC(20, 8) NOT NULL,
    asset VARCHAR(20) NOT NULL,
    external_transaction_id VARCHAR(255),
    status VARCHAR(20) NOT NULL, -- 'pending', 'confirmed', 'failed'
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE INDEX idx_investment_transactions_position ON investment_transactions(investment_position_id);
CREATE INDEX idx_investment_transactions_user ON investment_transactions(user_id);
CREATE INDEX idx_investment_transactions_created ON investment_transactions(created_at DESC);
```

**Implementation**:
```csharp
// Models/InvestmentPosition.cs
public class InvestmentPosition
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExchangeConnectionId { get; set; }
    public string ExchangeName { get; set; }
    public string ExternalPositionId { get; set; }
    public string PlanId { get; set; }
    public string Asset { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal AccruedRewards { get; set; }
    public decimal Apy { get; set; }
    public InvestmentStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Repositories/IInvestmentRepository.cs
public interface IInvestmentRepository
{
    Task<InvestmentPosition> CreateAsync(InvestmentPosition position);
    Task<InvestmentPosition?> GetByIdAsync(Guid id);
    Task<List<InvestmentPosition>> GetByUserIdAsync(Guid userId);
    Task<List<InvestmentPosition>> GetActivePositionsAsync();
    Task UpdateAsync(InvestmentPosition position);
    Task<List<InvestmentTransaction>> GetTransactionsAsync(Guid positionId);
}
```

**Acceptance Criteria**:
- [ ] Database migration created
- [ ] Repository implements all CRUD operations
- [ ] Indexes optimize common queries
- [ ] Unit tests for repository methods
- [ ] Integration tests with test database

**Dependencies**: None (database only)

---

#### BE-408: POST /api/investment/create (3.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Create investment position by transferring USDC to WhiteBit and creating Flex investment.

**API Specification**:
```
POST /api/investment/create
Authorization: Bearer {jwt_token}

Request:
{
  "planId": "flex-usdc-1",
  "amount": 500.0,
  "walletId": "uuid"
}

Response (200 OK):
{
  "investmentId": "uuid",
  "planId": "flex-usdc-1",
  "asset": "USDC",
  "amount": 500.0,
  "apy": 8.50,
  "status": "pending",
  "estimatedDailyReward": 0.116438,
  "estimatedMonthlyReward": 3.541666,
  "estimatedYearlyReward": 42.50,
  "createdAt": "2025-02-17T10:00:00Z"
}

Response (400 Bad Request):
{
  "error": "Insufficient USDC balance"
}

Response (400 Bad Request):
{
  "error": "Amount below minimum (100 USDC)"
}
```

**Implementation Flow**:
1. Validate user has sufficient USDC balance in Circle wallet
2. Validate amount meets plan minimum/maximum
3. Get WhiteBit deposit address for USDC
4. Transfer USDC from Circle wallet to WhiteBit (via Circle SDK)
5. Wait for transfer confirmation (polling)
6. Create Flex investment via WhiteBit API
7. Store investment position in database
8. Return investment details

**Acceptance Criteria**:
- [ ] Validates USDC balance before transfer
- [ ] Transfers USDC to WhiteBit successfully
- [ ] Creates Flex investment position
- [ ] Returns accurate reward projections
- [ ] Handles transfer failures gracefully
- [ ] Transaction audit logged
- [ ] Swagger documentation complete
- [ ] Integration tests pass

**Dependencies**: BE-406, BE-407, Circle SDK

---

#### BE-409: USDC Transfer to WhiteBit Service (1.00 day)
**Owner**: Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Service to handle USDC transfers from Circle wallet to WhiteBit deposit address.

**Implementation**:
```csharp
// Services/Investment/UsdcTransferService.cs
public class UsdcTransferService : IUsdcTransferService
{
    private readonly IWalletService _walletService;
    private readonly IWhiteBitApiClient _whiteBitClient;
    private readonly ILogger<UsdcTransferService> _logger;

    public async Task<TransferResult> TransferToWhiteBitAsync(
        Guid userId,
        Guid walletId,
        decimal amount)
    {
        // 1. Get WhiteBit USDC deposit address
        // 2. Validate Circle wallet balance
        // 3. Create USDC transfer via Circle SDK
        // 4. Poll for transfer confirmation
        // 5. Return transfer result
    }
}
```

**Acceptance Criteria**:
- [ ] Fetches WhiteBit deposit address
- [ ] Transfers USDC via Circle SDK
- [ ] Polls for transfer confirmation (max 5 minutes)
- [ ] Returns clear error messages
- [ ] Unit tests pass
- [ ] Integration tests with testnet

**Dependencies**: BE-408, Circle SDK

---

### Epic 3: Position Management & Tracking (10.00 days)

#### BE-410: Background Worker for Position Sync (3.00 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Background service to sync investment positions with WhiteBit every 60 seconds.

**Implementation**:
```csharp
// Services/BackgroundWorkers/InvestmentPositionSyncService.cs
public class InvestmentPositionSyncService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncAllActivePositionsAsync();
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Position sync failed");
            }
        }
    }

    private async Task SyncAllActivePositionsAsync()
    {
        // 1. Get all active positions from database
        // 2. Group by user and exchange connection
        // 3. For each user, fetch positions from WhiteBit
        // 4. Update current value and accrued rewards
        // 5. Update last_synced_at timestamp
    }
}
```

**Sync Strategy**:
- Runs every 60 seconds
- Syncs only active positions (status = 'active')
- Batches API calls per user (max 10 positions per call)
- Updates position value and accrued rewards
- Logs sync duration and errors

**Acceptance Criteria**:
- [ ] Background service runs every 60 seconds
- [ ] Syncs all active positions
- [ ] Updates position values accurately
- [ ] Handles WhiteBit API failures gracefully
- [ ] Logs sync duration for monitoring
- [ ] Alert if sync duration exceeds 30 seconds
- [ ] Unit tests for sync logic
- [ ] Integration tests with sandbox

**Dependencies**: BE-407, BE-401

---

#### BE-411: Reward Calculation Engine (2.00 days)
**Owner**: Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Service to calculate accrued rewards based on APY, principal, and time held.

**Formula**:
```
Daily Reward = Principal × (APY / 365 / 100)
Accrued Reward = Sum of daily rewards since start date

Example:
Principal: 500 USDC
APY: 8.50%
Days Held: 30

Daily Reward = 500 × (8.5 / 365 / 100) = 0.116438 USDC
Accrued Reward (30 days) = 0.116438 × 30 = 3.493150 USDC
```

**Implementation**:
```csharp
// Services/Investment/RewardCalculationService.cs
public class RewardCalculationService : IRewardCalculationService
{
    public decimal CalculateDailyReward(decimal principal, decimal apy)
    {
        return principal * (apy / 365m / 100m);
    }

    public decimal CalculateAccruedReward(
        decimal principal,
        decimal apy,
        DateTime startDate,
        DateTime endDate)
    {
        var days = (endDate - startDate).TotalDays;
        var dailyReward = CalculateDailyReward(principal, apy);
        return dailyReward * (decimal)days;
    }

    public decimal CalculateProjectedReward(
        decimal principal,
        decimal apy,
        int days)
    {
        var dailyReward = CalculateDailyReward(principal, apy);
        return dailyReward * days;
    }
}
```

**Acceptance Criteria**:
- [ ] Daily reward calculation accurate to 8 decimals
- [ ] Accrued reward calculation handles partial days
- [ ] Projected reward calculations for 1d, 30d, 365d
- [ ] Unit tests validate calculations
- [ ] QA validates with manual calculations
- [ ] Edge cases handled (leap years, time zones)

**Dependencies**: BE-407

---

#### BE-412: GET /api/investment/positions (1.50 days)
**Owner**: Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
List all investment positions for authenticated user with current values and rewards.

**API Specification**:
```
GET /api/investment/positions
Authorization: Bearer {jwt_token}

Response (200 OK):
{
  "positions": [
    {
      "id": "uuid",
      "planId": "flex-usdc-1",
      "asset": "USDC",
      "principalAmount": 500.0,
      "currentValue": 503.49,
      "accruedRewards": 3.49,
      "apy": 8.50,
      "status": "active",
      "startDate": "2025-02-17T10:00:00Z",
      "lastSyncedAt": "2025-02-28T09:00:00Z",
      "daysHeld": 11,
      "estimatedDailyReward": 0.116438,
      "estimatedMonthlyReward": 3.541666,
      "estimatedYearlyReward": 42.50
    }
  ],
  "totalPrincipal": 500.0,
  "totalCurrentValue": 503.49,
  "totalAccruedRewards": 3.49
}
```

**Acceptance Criteria**:
- [ ] Returns all positions for user
- [ ] Includes calculated rewards
- [ ] Includes projected earnings
- [ ] Response time < 1s
- [ ] Pagination support (page, pageSize)
- [ ] Swagger documentation complete
- [ ] Unit tests pass

**Dependencies**: BE-407, BE-411

---

#### BE-413: GET /api/investment/{id}/details (1.50 days)
**Owner**: Backend Engineer
**Priority**: P1 (High)

**Description**:
Get detailed information for specific investment position.

**API Specification**:
```
GET /api/investment/{id}/details
Authorization: Bearer {jwt_token}

Response (200 OK):
{
  "id": "uuid",
  "planId": "flex-usdc-1",
  "planName": "USDC Flex Plan",
  "asset": "USDC",
  "principalAmount": 500.0,
  "currentValue": 503.49,
  "accruedRewards": 3.49,
  "apy": 8.50,
  "status": "active",
  "startDate": "2025-02-17T10:00:00Z",
  "lastSyncedAt": "2025-02-28T09:00:00Z",
  "daysHeld": 11,
  "transactions": [
    {
      "id": "uuid",
      "type": "create",
      "amount": 500.0,
      "status": "confirmed",
      "createdAt": "2025-02-17T10:00:00Z"
    }
  ],
  "projectedRewards": {
    "daily": 0.116438,
    "monthly": 3.541666,
    "yearly": 42.50
  }
}

Response (404 Not Found):
{
  "error": "Investment position not found"
}
```

**Acceptance Criteria**:
- [ ] Returns complete position details
- [ ] Includes transaction history
- [ ] Includes projected rewards
- [ ] Returns 404 if not found or not authorized
- [ ] Response time < 1s
- [ ] Swagger documentation complete
- [ ] Unit tests pass

**Dependencies**: BE-407, BE-411

---

#### BE-414: Investment Position Sync Service (2.00 days)
**Owner**: Backend Engineer
**Priority**: P1 (High - can defer if needed)

**Description**:
Service to synchronize individual position data with WhiteBit on-demand.

**Implementation**:
```csharp
// Services/Investment/InvestmentPositionSyncService.cs
public class InvestmentPositionSyncService
{
    public async Task<InvestmentPosition> SyncPositionAsync(
        Guid positionId)
    {
        // 1. Get position from database
        // 2. Fetch updated data from WhiteBit API
        // 3. Calculate accrued rewards
        // 4. Update position in database
        // 5. Return updated position
    }
}
```

**Acceptance Criteria**:
- [ ] Syncs single position on-demand
- [ ] Updates current value and rewards
- [ ] Handles WhiteBit API failures
- [ ] Returns updated position data
- [ ] Unit tests pass

**Dependencies**: BE-407, BE-410, BE-411

**⚠️ Deferral Candidate**: Can be deferred to Sprint N05 if capacity constrained.

---

### Epic 4: Withdrawal & History (5.00 days)

#### BE-415: POST /api/investment/{id}/withdraw (2.50 days)
**Owner**: Senior Backend Engineer
**Priority**: P0 (Critical Path)

**Description**:
Withdraw investment by closing WhiteBit position and returning USDC to Circle wallet.

**API Specification**:
```
POST /api/investment/{id}/withdraw
Authorization: Bearer {jwt_token}

Request:
{
  "walletId": "uuid"
}

Response (200 OK):
{
  "investmentId": "uuid",
  "withdrawalAmount": 503.49,
  "principal": 500.0,
  "rewards": 3.49,
  "status": "processing",
  "estimatedCompletionTime": "2025-02-28T12:00:00Z"
}

Response (400 Bad Request):
{
  "error": "Investment position is not active"
}
```

**Implementation Flow**:
1. Validate investment position exists and is active
2. Close Flex investment via WhiteBit API
3. Wait for WhiteBit to process closure (polling)
4. Get withdrawal amount (principal + rewards)
5. Transfer USDC from WhiteBit to Circle wallet
6. Update position status to 'closed'
7. Record withdrawal transaction
8. Return withdrawal details

**Acceptance Criteria**:
- [ ] Closes WhiteBit investment successfully
- [ ] Transfers USDC back to Circle wallet
- [ ] Updates position status to 'closed'
- [ ] Records withdrawal transaction
- [ ] Returns accurate withdrawal amount
- [ ] Handles WhiteBit API failures gracefully
- [ ] Swagger documentation complete
- [ ] Integration tests pass

**Dependencies**: BE-407, Circle SDK, WhiteBit API

---

#### BE-416: GET /api/investment/history (1.50 days)
**Owner**: Backend Engineer
**Priority**: P2 (Medium - can defer if needed)

**Description**:
Get investment history with filtering and pagination.

**API Specification**:
```
GET /api/investment/history?status=closed&page=1&pageSize=20
Authorization: Bearer {jwt_token}

Response (200 OK):
{
  "positions": [
    {
      "id": "uuid",
      "planId": "flex-usdc-1",
      "asset": "USDC",
      "principalAmount": 500.0,
      "finalValue": 503.49,
      "totalRewards": 3.49,
      "apy": 8.50,
      "status": "closed",
      "startDate": "2025-02-17T10:00:00Z",
      "endDate": "2025-02-28T09:00:00Z",
      "daysHeld": 11
    }
  ],
  "totalItems": 5,
  "page": 1,
  "pageSize": 20
}
```

**Query Parameters**:
- `status`: Filter by status (active, closed, all)
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20, max: 100)
- `sortBy`: Sort field (startDate, endDate, amount)
- `sortOrder`: asc or desc

**Acceptance Criteria**:
- [ ] Returns investment history with pagination
- [ ] Supports status filtering
- [ ] Supports sorting by date/amount
- [ ] Response time < 1s
- [ ] Swagger documentation complete
- [ ] Unit tests pass

**Dependencies**: BE-407

**⚠️ Deferral Candidate**: Can be deferred to Sprint N05 if capacity constrained.

---

#### BE-417: Investment Audit Trail Service (1.00 day)
**Owner**: Backend Engineer
**Priority**: P2 (Medium - can defer if needed)

**Description**:
Service to log all investment operations for audit trail.

**Implementation**:
```csharp
// Services/Investment/InvestmentAuditService.cs
public class InvestmentAuditService
{
    public async Task LogInvestmentCreatedAsync(
        Guid userId,
        Guid investmentId,
        decimal amount)
    {
        // Log to audit_logs table
    }

    public async Task LogInvestmentWithdrawnAsync(
        Guid userId,
        Guid investmentId,
        decimal amount)
    {
        // Log to audit_logs table
    }
}
```

**Audit Log Schema**:
```sql
CREATE TABLE investment_audit_logs (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id),
    investment_position_id UUID REFERENCES investment_positions(id),
    action VARCHAR(50) NOT NULL, -- 'create', 'withdraw', 'sync', 'update'
    details JSONB,
    ip_address VARCHAR(45),
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT NOW()
);

CREATE INDEX idx_investment_audit_logs_user ON investment_audit_logs(user_id);
CREATE INDEX idx_investment_audit_logs_position ON investment_audit_logs(investment_position_id);
CREATE INDEX idx_investment_audit_logs_created ON investment_audit_logs(created_at DESC);
```

**Acceptance Criteria**:
- [ ] Logs all investment operations
- [ ] Includes user context (IP, user agent)
- [ ] Queryable by user, position, date range
- [ ] Unit tests pass

**Dependencies**: BE-407

**⚠️ Deferral Candidate**: Can be deferred to Sprint N05 if capacity constrained.

---

## Task Dependencies (Critical Path)

```
BE-401 (WhiteBit API Client)
  └── BE-402 (Authentication)
        └── BE-403 (Credential Storage)
              ├── BE-404 (Connect Endpoint)
              ├── BE-405 (Status Endpoint)
              └── BE-406 (Plans Endpoint)
                    ├── BE-407 (Position Model)
                    │     ├── BE-410 (Position Sync Worker)
                    │     │     ├── BE-411 (Reward Calculation)
                    │     │     │     ├── BE-412 (List Positions)
                    │     │     │     ├── BE-413 (Position Details)
                    │     │     │     └── BE-414 (Sync Service) ⚠️ Can defer
                    │     │     └── BE-415 (Withdrawal)
                    │     │           └── BE-416 (History) ⚠️ Can defer
                    │     │                 └── BE-417 (Audit Trail) ⚠️ Can defer
                    │     └── BE-408 (Create Investment)
                    │           └── BE-409 (USDC Transfer)
                    └── (Continue with other tasks)
```

**Critical Path Duration**: ~23 days (without deferrals)
**With Deferrals**: ~18.5 days (defer BE-414, BE-416, BE-417)

---

## Sprint Backlog (Priority Order)

### Must Have (P0) - Sprint Cannot Ship Without These
1. BE-401: WhiteBit API Client
2. BE-402: Authentication Service
3. BE-403: API Credential Storage
4. BE-404: Connect Endpoint
5. BE-405: Status Endpoint
6. BE-406: Investment Plans Endpoint
7. BE-407: Investment Position Model
8. BE-408: Create Investment
9. BE-409: USDC Transfer Service
10. BE-410: Position Sync Background Worker
11. BE-411: Reward Calculation Engine
12. BE-412: List Positions Endpoint
13. BE-415: Withdraw Investment

### Should Have (P1) - Important But Can Ship Without
14. BE-413: Position Details Endpoint

### Nice to Have (P2) - Defer to Sprint N05 If Needed
15. BE-414: Position Sync Service (on-demand sync)
16. BE-416: Investment History Endpoint
17. BE-417: Investment Audit Trail Service

**Recommendation**: If backend team has only 2 engineers, defer P2 tasks (BE-414, BE-416, BE-417) to Sprint N05. This reduces backend effort from 33 days to 28.5 days (95% utilization).

---

## Testing Requirements

### Unit Tests (Target: >80% Coverage)
- [ ] WhiteBit API client methods
- [ ] Authentication signature generation
- [ ] Encryption/decryption services
- [ ] Reward calculation formulas
- [ ] Repository CRUD operations

### Integration Tests
- [ ] WhiteBit API connectivity (sandbox)
- [ ] Credential validation
- [ ] Investment creation flow
- [ ] Position sync worker
- [ ] Withdrawal flow
- [ ] Circle SDK USDC transfers

### Performance Tests
- [ ] Position sync completes in < 30s
- [ ] API response time < 2s (P95)
- [ ] Concurrent investment creation (10 users)

---

## Monitoring & Alerts

### Key Metrics to Monitor
- WhiteBit API response times (P50, P95, P99)
- WhiteBit API error rates
- Position sync duration and frequency
- Investment creation success rate
- Withdrawal completion time
- Database query performance (investment_positions table)

### Alert Thresholds
- Position sync duration > 30 seconds
- WhiteBit API error rate > 5%
- Investment creation failure rate > 2%
- Reward calculation errors detected

---

## Documentation Requirements

### API Documentation (Swagger)
- [ ] All endpoints documented with examples
- [ ] Request/response schemas defined
- [ ] Error codes and messages listed
- [ ] Authentication requirements specified

### Developer Documentation
- [ ] WhiteBit API integration guide
- [ ] Encryption service usage guide
- [ ] Position sync worker architecture
- [ ] Reward calculation formulas
- [ ] Troubleshooting guide

---

## Definition of Done (Backend)

Sprint N04 backend work is **DONE** when:

- [ ] All P0 tasks completed (13 tasks)
- [ ] All API endpoints implemented and documented
- [ ] WhiteBit API integration tested in sandbox
- [ ] API credentials encrypted at rest (user-level)
- [ ] Position sync worker running every 60 seconds
- [ ] Reward calculations accurate to 8 decimal places
- [ ] Unit test coverage > 80%
- [ ] Integration tests pass
- [ ] API response time < 2s (P95)
- [ ] Code reviewed and approved
- [ ] Swagger documentation updated
- [ ] No security vulnerabilities (Critical/High)
- [ ] Database migration scripts deployed
- [ ] Monitoring and alerts configured

---

## Sprint Risks & Mitigation

### High Risks

**Risk 1: Backend Over-Capacity (110%)**
- **Impact**: Sprint failure or quality issues
- **Mitigation**: Add 3rd backend engineer or defer P2 tasks
- **Owner**: Team Lead
- **Status**: 🔴 Unresolved

**Risk 2: WhiteBit Sandbox Unavailable**
- **Impact**: Cannot test integration
- **Mitigation**: Use mock WhiteBit server, request sandbox access early
- **Owner**: Backend Lead
- **Status**: 🟡 In Progress

**Risk 3: Financial Calculation Errors**
- **Impact**: User trust damage, potential legal issues
- **Mitigation**: Extensive unit tests, QA manual validation
- **Owner**: Backend Engineer + QA
- **Status**: 🟡 Needs Attention

### Medium Risks

**Risk 4: USDC Transfer Delays**
- **Impact**: User frustration, increased support load
- **Mitigation**: Clear user expectations (24-48h), status tracking
- **Owner**: Backend Engineer
- **Status**: 🟢 Managed

**Risk 5: WhiteBit API Rate Limits**
- **Impact**: Position sync failures
- **Mitigation**: Request queuing, exponential backoff
- **Owner**: Backend Engineer
- **Status**: 🟢 Managed

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-04 | Backend Lead | Initial Sprint N04 Backend Plan |

---

**BACKEND TEAM STATUS**: **READY TO START** (pending capacity resolution)

**CRITICAL ACTION REQUIRED**: Resolve backend capacity issue (110% utilization) by Day 1.

**NEXT STEPS**:
1. **Day 1 Morning**: Confirm 3rd backend engineer availability OR defer P2 tasks
2. **Day 1**: Obtain WhiteBit sandbox credentials
3. **Day 1**: Sprint kickoff meeting
4. **Day 1 Afternoon**: Begin BE-401 (WhiteBit API Client)

---

**End of Sprint N04 Backend Plan**
