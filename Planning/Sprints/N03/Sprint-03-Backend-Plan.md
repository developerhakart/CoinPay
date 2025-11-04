# Sprint N03 Backend Plan - Phase 3: Fiat Off-Ramp

**Sprint**: N03
**Duration**: 2 weeks (10 working days)
**Dates**: February 3-14, 2025
**Team Size**: 2-3 Backend Engineers
**Total Effort**: ~28 days
**Owner**: Backend Lead

---

## Sprint Goals

### Primary Goals

1. **Secure Bank Account Management**: Implement encrypted storage and CRUD operations for user bank accounts
2. **Fiat Gateway Integration**: Integrate with RedotPay/Bridge/Wyre for crypto-to-fiat conversion
3. **Payout Processing**: Enable USDC to USD payouts with status tracking and webhooks

### Success Criteria

- ✅ Bank account data encrypted at rest and in transit
- ✅ All bank account CRUD endpoints functional
- ✅ Fiat gateway integration tested in sandbox environment
- ✅ Real-time USDC/USD exchange rates with 30s cache
- ✅ Payout flow complete: initiate → process → track → complete
- ✅ Webhook handling for external payout status updates
- ✅ Comprehensive audit trail for all payout transactions
- ✅ API response times < 2s for all endpoints
- ✅ Unit test coverage > 80%
- ✅ Zero Critical/High security vulnerabilities

---

## Architecture Overview

### New Components

```
CoinPay.Api/
├── Models/
│   ├── BankAccount.cs              # Bank account entity
│   ├── PayoutTransaction.cs        # Payout transaction entity
│   └── PayoutAuditLog.cs           # Audit log entity
├── Data/
│   ├── Configurations/
│   │   ├── BankAccountConfiguration.cs
│   │   └── PayoutTransactionConfiguration.cs
├── Services/
│   ├── Encryption/
│   │   ├── IEncryptionService.cs   # Encryption interface
│   │   └── AesEncryptionService.cs # AES-256 implementation
│   ├── FiatGateway/
│   │   ├── IFiatGatewayService.cs  # Gateway interface
│   │   ├── RedotPayService.cs      # RedotPay implementation
│   │   └── ExchangeRateService.cs  # Exchange rate fetching
│   ├── Payout/
│   │   ├── PayoutService.cs        # Payout orchestration
│   │   └── FeeCalculatorService.cs # Fee calculation
│   └── BankAccount/
│       ├── BankAccountService.cs   # Bank account operations
│       └── BankAccountValidator.cs # Validation logic
├── Repositories/
│   ├── IBankAccountRepository.cs
│   ├── BankAccountRepository.cs
│   ├── IPayoutRepository.cs
│   └── PayoutRepository.cs
├── Controllers/
│   ├── BankAccountController.cs    # Bank account endpoints
│   ├── PayoutController.cs         # Payout endpoints
│   └── ExchangeRateController.cs   # Exchange rate endpoints
└── Webhooks/
    └── PayoutWebhookHandler.cs     # Webhook processing
```

### Database Schema

```sql
-- Bank Accounts Table
CREATE TABLE bank_accounts (
    id UUID PRIMARY KEY,
    user_id VARCHAR(255) NOT NULL,
    account_holder_name VARCHAR(255) NOT NULL,
    routing_number_encrypted BYTEA NOT NULL,
    account_number_encrypted BYTEA NOT NULL,
    account_type VARCHAR(50) NOT NULL, -- 'checking' or 'savings'
    bank_name VARCHAR(255),
    is_primary BOOLEAN DEFAULT false,
    is_verified BOOLEAN DEFAULT false,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE INDEX idx_bank_accounts_user_id ON bank_accounts(user_id);

-- Payout Transactions Table
CREATE TABLE payout_transactions (
    id UUID PRIMARY KEY,
    user_id VARCHAR(255) NOT NULL,
    bank_account_id UUID NOT NULL,
    gateway_transaction_id VARCHAR(255),
    usdc_amount DECIMAL(18, 6) NOT NULL,
    usd_amount DECIMAL(18, 2) NOT NULL,
    exchange_rate DECIMAL(18, 6) NOT NULL,
    conversion_fee DECIMAL(18, 2) NOT NULL,
    payout_fee DECIMAL(18, 2) NOT NULL,
    total_fees DECIMAL(18, 2) NOT NULL,
    net_amount DECIMAL(18, 2) NOT NULL,
    status VARCHAR(50) NOT NULL, -- 'pending', 'processing', 'completed', 'failed', 'cancelled'
    failure_reason TEXT,
    initiated_at TIMESTAMP NOT NULL,
    completed_at TIMESTAMP,
    estimated_arrival TIMESTAMP,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT fk_bank_account FOREIGN KEY (bank_account_id) REFERENCES bank_accounts(id)
);

CREATE INDEX idx_payout_transactions_user_id ON payout_transactions(user_id);
CREATE INDEX idx_payout_transactions_status ON payout_transactions(status);
CREATE INDEX idx_payout_transactions_gateway_id ON payout_transactions(gateway_transaction_id);

-- Payout Audit Logs Table
CREATE TABLE payout_audit_logs (
    id UUID PRIMARY KEY,
    payout_transaction_id UUID NOT NULL,
    event_type VARCHAR(100) NOT NULL, -- 'initiated', 'status_changed', 'webhook_received', etc.
    previous_status VARCHAR(50),
    new_status VARCHAR(50),
    event_data JSONB,
    created_at TIMESTAMP NOT NULL,
    CONSTRAINT fk_payout_transaction FOREIGN KEY (payout_transaction_id) REFERENCES payout_transactions(id)
);

CREATE INDEX idx_payout_audit_logs_transaction_id ON payout_audit_logs(payout_transaction_id);
```

---

## Task Breakdown

### Phase 3.1: Bank Account Management (8.00 days)

#### BE-301: Bank Account Model & Repository (1.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-1

**Description**: Create database models and repository for bank account management with proper relationships and indexing.

**Implementation Steps**:
1. Create `BankAccount` entity model
2. Add EF Core configuration with encryption attributes
3. Create migration for bank_accounts table
4. Implement `IBankAccountRepository` interface
5. Implement `BankAccountRepository` with CRUD operations
6. Add unit tests for repository

**Acceptance Criteria**:
- [ ] BankAccount model with all required fields
- [ ] EF Core configuration for encrypted fields
- [ ] Database migration creates table correctly
- [ ] Repository implements all CRUD operations
- [ ] Unit tests cover all repository methods
- [ ] Code reviewed and merged

**Files**:
- `Models/BankAccount.cs`
- `Data/Configurations/BankAccountConfiguration.cs`
- `Repositories/IBankAccountRepository.cs`
- `Repositories/BankAccountRepository.cs`
- `Tests/Repositories/BankAccountRepositoryTests.cs`

---

#### BE-302: Encryption Service for Sensitive Data (2.00 days)
**Priority**: HIGH 🔴
**Owner**: Senior BE

**Description**: Implement AES-256 encryption service for securing sensitive bank account data (routing/account numbers).

**Implementation Steps**:
1. Create `IEncryptionService` interface
2. Implement `AesEncryptionService` with AES-256-GCM
3. Add key management (environment variable or AWS KMS)
4. Implement encrypt/decrypt methods
5. Add key rotation support
6. Add comprehensive unit tests
7. Security audit

**Acceptance Criteria**:
- [ ] AES-256-GCM encryption implemented
- [ ] Keys stored securely (env vars or KMS)
- [ ] Encrypt/decrypt methods work correctly
- [ ] Key rotation mechanism in place
- [ ] Unit tests cover encryption/decryption
- [ ] Security audit passed
- [ ] Documentation on key management

**Files**:
- `Services/Encryption/IEncryptionService.cs`
- `Services/Encryption/AesEncryptionService.cs`
- `Tests/Services/EncryptionServiceTests.cs`
- `docs/encryption-guide.md`

**Security Requirements**:
- Use AES-256-GCM (authenticated encryption)
- Unique IV for each encryption
- Secure key storage (never in code)
- FIPS 140-2 compliant (if possible)

---

#### BE-303: POST /api/bank-account - Add Bank Account (1.00 day)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-301, BE-302

**Description**: Create endpoint to add new bank account with validation and encryption.

**Request**:
```json
{
  "accountHolderName": "John Doe",
  "routingNumber": "123456789",
  "accountNumber": "9876543210",
  "accountType": "checking",
  "bankName": "Chase Bank",
  "isPrimary": true
}
```

**Response** (201 Created):
```json
{
  "id": "uuid",
  "accountHolderName": "John Doe",
  "accountType": "checking",
  "bankName": "Chase Bank",
  "lastFourDigits": "3210",
  "isPrimary": true,
  "isVerified": false,
  "createdAt": "2025-02-03T10:00:00Z"
}
```

**Acceptance Criteria**:
- [ ] Endpoint validates all required fields
- [ ] Routing number validated (9 digits, checksum)
- [ ] Account number validated (max 17 digits)
- [ ] Sensitive data encrypted before storage
- [ ] Only last 4 digits returned in response
- [ ] User can only have 1 primary account
- [ ] Returns 201 on success, 400 on validation error
- [ ] Unit and integration tests pass

---

#### BE-304: GET /api/bank-account - List Bank Accounts (0.75 days)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-301

**Description**: Endpoint to retrieve all bank accounts for authenticated user.

**Response** (200 OK):
```json
{
  "bankAccounts": [
    {
      "id": "uuid",
      "accountHolderName": "John Doe",
      "accountType": "checking",
      "bankName": "Chase Bank",
      "lastFourDigits": "3210",
      "isPrimary": true,
      "isVerified": true,
      "createdAt": "2025-02-03T10:00:00Z"
    }
  ],
  "total": 1
}
```

**Acceptance Criteria**:
- [ ] Returns all bank accounts for user
- [ ] Sensitive data NOT included in response
- [ ] Only last 4 digits shown
- [ ] Sorted by isPrimary DESC, createdAt DESC
- [ ] Returns 200 on success
- [ ] Unit and integration tests pass

---

#### BE-305: PUT /api/bank-account/{id} - Update Bank Account (0.75 days)
**Priority**: MEDIUM 🟡
**Owner**: BE-1
**Dependencies**: BE-301

**Description**: Endpoint to update bank account metadata (not routing/account numbers).

**Request**:
```json
{
  "accountHolderName": "John Doe Jr.",
  "bankName": "Chase Bank",
  "isPrimary": false
}
```

**Response** (200 OK):
```json
{
  "id": "uuid",
  "accountHolderName": "John Doe Jr.",
  "accountType": "checking",
  "bankName": "Chase Bank",
  "lastFourDigits": "3210",
  "isPrimary": false,
  "isVerified": true,
  "updatedAt": "2025-02-05T12:00:00Z"
}
```

**Acceptance Criteria**:
- [ ] Can update accountHolderName, bankName, isPrimary
- [ ] Cannot update routing/account numbers (security)
- [ ] Validates user owns the bank account
- [ ] Returns 200 on success, 404 if not found
- [ ] Unit and integration tests pass

---

#### BE-306: DELETE /api/bank-account/{id} - Remove Bank Account (0.50 days)
**Priority**: MEDIUM 🟡
**Owner**: BE-1
**Dependencies**: BE-301

**Description**: Endpoint to soft-delete or remove bank account.

**Response** (204 No Content)

**Acceptance Criteria**:
- [ ] Validates user owns the bank account
- [ ] Cannot delete if used in pending/processing payouts
- [ ] Soft delete (set deleted_at) or hard delete
- [ ] Returns 204 on success
- [ ] Returns 400 if has pending payouts
- [ ] Returns 404 if not found
- [ ] Unit and integration tests pass

---

#### BE-307: Bank Account Validation Service (1.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-2
**Dependencies**: BE-301

**Description**: Service to validate US bank account routing and account numbers.

**Validation Rules**:
1. **Routing Number**:
   - Exactly 9 digits
   - Valid checksum (ABA routing number algorithm)
   - Valid bank lookup (optional)

2. **Account Number**:
   - Between 5-17 digits
   - Only numeric characters

3. **Account Holder Name**:
   - Minimum 2 characters
   - Maximum 255 characters
   - Letters, spaces, hyphens only

**Acceptance Criteria**:
- [ ] Routing number checksum validation
- [ ] Account number format validation
- [ ] Account holder name validation
- [ ] Validation service returns clear error messages
- [ ] Unit tests cover all validation rules
- [ ] Integration with bank account endpoint

---

### Phase 3.2: Fiat Gateway Integration (10.00 days)

#### BE-308: Fiat Gateway Service Interface (1.50 days)
**Priority**: HIGH 🔴
**Owner**: Senior BE

**Description**: Create abstraction layer for fiat gateway providers (RedotPay, Bridge, Wyre).

**Interface**:
```csharp
public interface IFiatGatewayService
{
    Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    Task<PayoutResult> InitiatePayoutAsync(PayoutRequest request);
    Task<PayoutStatus> GetPayoutStatusAsync(string gatewayTransactionId);
    Task<PayoutResult> CancelPayoutAsync(string gatewayTransactionId);
    Task<bool> ValidateWebhookSignatureAsync(string payload, string signature);
}
```

**Acceptance Criteria**:
- [ ] Interface defines all required methods
- [ ] DTO models for all request/response types
- [ ] Exception handling strategy defined
- [ ] Retry logic interface
- [ ] Unit tests with mock implementations

---

#### BE-309: RedotPay/Bridge API Client Integration (3.00 days)
**Priority**: HIGH 🔴
**Owner**: Senior BE
**Dependencies**: BE-308

**Description**: Implement fiat gateway service using RedotPay or Bridge API.

**Implementation Steps**:
1. Research RedotPay/Bridge API documentation
2. Create API client with HttpClient
3. Implement authentication (API key, OAuth)
4. Implement all IFiatGatewayService methods
5. Add error handling and retry logic
6. Test in sandbox environment
7. Add comprehensive logging

**API Operations**:
- Get exchange rates
- Initiate payout (USDC → USD)
- Check payout status
- Cancel payout
- Validate webhook signatures

**Acceptance Criteria**:
- [ ] All API methods implemented
- [ ] Authentication working (API key or OAuth)
- [ ] Error handling with retry logic (3 retries)
- [ ] Sandbox testing successful
- [ ] Unit tests with mocked API responses
- [ ] Integration tests with sandbox
- [ ] API rate limiting handled
- [ ] Documentation on API configuration

**Configuration** (appsettings.json):
```json
{
  "FiatGateway": {
    "Provider": "RedotPay",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret",
    "BaseUrl": "https://sandbox.redotpay.com/api/v1",
    "WebhookSecret": "your-webhook-secret",
    "RetryAttempts": 3,
    "TimeoutSeconds": 30
  }
}
```

---

#### BE-310: Exchange Rate Service (USDC/USD) (1.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-309

**Description**: Service to fetch and cache USDC/USD exchange rates with automatic refresh.

**Implementation Steps**:
1. Create `ExchangeRateService`
2. Fetch rates from fiat gateway
3. Implement Redis caching (30s TTL)
4. Add fallback to previous rate if fetch fails
5. Background worker to refresh rates every 30s
6. Add rate history tracking (optional)

**Acceptance Criteria**:
- [ ] Rates fetched from fiat gateway
- [ ] Redis caching with 30s TTL
- [ ] Background refresh every 30 seconds
- [ ] Fallback to last known rate on failure
- [ ] Rate change alerts (>2% change)
- [ ] Unit tests with mocked gateway
- [ ] Integration tests with Redis

---

#### BE-311: Conversion Fee Calculator Service (1.00 day)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-310

**Description**: Service to calculate conversion and payout fees.

**Fee Structure**:
- Conversion fee: 1.5% of USDC amount
- Payout fee: $1.00 flat fee (US ACH)
- Total fees = Conversion fee + Payout fee

**Formula**:
```
USDC Amount: 100 USDC
Exchange Rate: 1 USDC = 0.9998 USD
Conversion Fee: 100 * 0.015 = 1.50 USD
Payout Fee: 1.00 USD
Total Fees: 2.50 USD
USD Amount Before Fees: 100 * 0.9998 = 99.98 USD
Net Amount: 99.98 - 2.50 = 97.48 USD
```

**Acceptance Criteria**:
- [ ] Fee calculation accurate
- [ ] Fees configurable in appsettings.json
- [ ] Fee breakdown returned in response
- [ ] Unit tests cover all scenarios
- [ ] Edge cases handled (zero amount, large amounts)

---

#### BE-312: GET /api/rates/usdc-usd - Exchange Rate (0.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-310

**Description**: Public endpoint to get current USDC/USD exchange rate.

**Response** (200 OK):
```json
{
  "fromCurrency": "USDC",
  "toCurrency": "USD",
  "rate": 0.9998,
  "timestamp": "2025-02-03T10:00:00Z",
  "expiresAt": "2025-02-03T10:00:30Z"
}
```

**Acceptance Criteria**:
- [ ] Returns current exchange rate
- [ ] Rate sourced from cache (fast response)
- [ ] Includes expiration timestamp
- [ ] Returns 200 on success
- [ ] Unit and integration tests pass

---

#### BE-313: POST /api/payout/initiate - Initiate Payout (2.00 days)
**Priority**: HIGH 🔴
**Owner**: BE-2
**Dependencies**: BE-309, BE-311

**Description**: Endpoint to initiate crypto-to-fiat payout with fee calculation.

**Request**:
```json
{
  "bankAccountId": "uuid",
  "usdcAmount": 100.50,
  "lockRate": true
}
```

**Response** (201 Created):
```json
{
  "payoutId": "uuid",
  "usdcAmount": 100.50,
  "exchangeRate": 0.9998,
  "conversionFee": 1.51,
  "payoutFee": 1.00,
  "totalFees": 2.51,
  "usdAmount": 100.48,
  "netAmount": 97.97,
  "status": "pending",
  "estimatedArrival": "2025-02-05T10:00:00Z",
  "createdAt": "2025-02-03T10:00:00Z"
}
```

**Business Logic**:
1. Validate bank account exists and belongs to user
2. Check user has sufficient USDC balance
3. Get current exchange rate
4. Calculate fees
5. Lock exchange rate (30s)
6. Create payout transaction record
7. Submit to fiat gateway
8. Return payout details

**Acceptance Criteria**:
- [ ] Validates bank account ownership
- [ ] Checks sufficient USDC balance
- [ ] Exchange rate locked for 30 seconds
- [ ] Fees calculated correctly
- [ ] Payout submitted to gateway
- [ ] Transaction record created
- [ ] Returns 201 on success
- [ ] Returns 400 on validation errors
- [ ] Unit and integration tests pass

---

#### BE-314: Webhook Handler for Payout Status (1.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-2
**Dependencies**: BE-313

**Description**: Webhook endpoint to receive payout status updates from fiat gateway.

**Webhook Payload** (example):
```json
{
  "event": "payout.status_changed",
  "transaction_id": "gateway-tx-id",
  "status": "completed",
  "completed_at": "2025-02-04T15:30:00Z",
  "signature": "hmac-signature"
}
```

**Implementation Steps**:
1. Create webhook endpoint: POST /api/webhooks/payout
2. Validate webhook signature (HMAC-SHA256)
3. Parse webhook payload
4. Update payout transaction status
5. Create audit log entry
6. Trigger notifications (optional)
7. Return 200 OK immediately

**Acceptance Criteria**:
- [ ] Webhook signature validated
- [ ] Payout status updated correctly
- [ ] Audit log entry created
- [ ] Idempotent (duplicate webhooks ignored)
- [ ] Returns 200 immediately (async processing)
- [ ] Unit tests with mock payloads
- [ ] Integration tests with sandbox webhooks

**Status Mapping**:
- gateway: "pending" → db: "pending"
- gateway: "processing" → db: "processing"
- gateway: "completed" → db: "completed"
- gateway: "failed" → db: "failed"

---

### Phase 3.3: Payout Management (10.00 days)

#### BE-315: Payout Transaction Model & Repository (1.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-1

**Description**: Create database models and repository for payout transactions.

**Implementation Steps**:
1. Create `PayoutTransaction` entity model
2. Create `PayoutAuditLog` entity model
3. Add EF Core configurations
4. Create migrations
5. Implement `IPayoutRepository` interface
6. Implement `PayoutRepository`
7. Add unit tests

**Acceptance Criteria**:
- [ ] PayoutTransaction model with all fields
- [ ] PayoutAuditLog model for audit trail
- [ ] EF Core configurations
- [ ] Database migrations create tables
- [ ] Repository implements all operations
- [ ] Unit tests cover repository methods
- [ ] Code reviewed and merged

---

#### BE-316: Payout Status Update Service (1.50 days)
**Priority**: HIGH 🔴
**Owner**: BE-2
**Dependencies**: BE-315, BE-314

**Description**: Service to update payout status and create audit logs.

**Methods**:
```csharp
Task UpdatePayoutStatusAsync(Guid payoutId, string newStatus, string reason = null);
Task<PayoutTransaction> GetPayoutByGatewayIdAsync(string gatewayTransactionId);
Task CreateAuditLogAsync(Guid payoutId, string eventType, object eventData);
```

**Acceptance Criteria**:
- [ ] Status update creates audit log entry
- [ ] Status transitions validated (state machine)
- [ ] Completed status sets completed_at timestamp
- [ ] Failed status stores failure reason
- [ ] Unit tests cover all scenarios
- [ ] Integration tests with database

**Status State Machine**:
```
pending → processing → completed
pending → processing → failed
pending → cancelled
```

---

#### BE-317: GET /api/payout/history - Payout History (2.00 days)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-315

**Description**: Endpoint to retrieve payout transaction history with pagination and filtering.

**Query Parameters**:
- `page` (default: 1)
- `pageSize` (default: 20, max: 100)
- `status` (optional: pending, processing, completed, failed, cancelled)
- `fromDate` (optional: ISO 8601)
- `toDate` (optional: ISO 8601)
- `sortBy` (default: createdAt, options: createdAt, usdcAmount, usdAmount)
- `sortOrder` (default: desc, options: asc, desc)

**Response** (200 OK):
```json
{
  "payouts": [
    {
      "id": "uuid",
      "bankAccount": {
        "bankName": "Chase Bank",
        "lastFourDigits": "3210"
      },
      "usdcAmount": 100.50,
      "usdAmount": 100.48,
      "netAmount": 97.97,
      "totalFees": 2.51,
      "status": "completed",
      "initiatedAt": "2025-02-03T10:00:00Z",
      "completedAt": "2025-02-04T15:30:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 45,
    "totalPages": 3
  }
}
```

**Acceptance Criteria**:
- [ ] Pagination working correctly
- [ ] Filtering by status functional
- [ ] Date range filtering functional
- [ ] Sorting working (createdAt, amount)
- [ ] Returns only user's payouts
- [ ] Returns 200 on success
- [ ] Unit and integration tests pass

---

#### BE-318: GET /api/payout/{id}/status - Payout Status (1.00 day)
**Priority**: HIGH 🔴
**Owner**: BE-1
**Dependencies**: BE-315

**Description**: Endpoint to get real-time payout status.

**Response** (200 OK):
```json
{
  "payoutId": "uuid",
  "status": "processing",
  "statusMessage": "Payment is being processed by the bank",
  "estimatedArrival": "2025-02-05T10:00:00Z",
  "lastUpdated": "2025-02-04T12:00:00Z"
}
```

**Acceptance Criteria**:
- [ ] Returns current payout status
- [ ] Includes user-friendly status message
- [ ] Validates user owns the payout
- [ ] Returns 200 on success, 404 if not found
- [ ] Unit and integration tests pass

---

#### BE-319: GET /api/payout/{id}/details - Payout Details (1.00 day)
**Priority**: MEDIUM 🟡
**Owner**: BE-1
**Dependencies**: BE-315

**Description**: Endpoint to get complete payout transaction details.

**Response** (200 OK):
```json
{
  "id": "uuid",
  "bankAccount": {
    "id": "uuid",
    "bankName": "Chase Bank",
    "accountType": "checking",
    "lastFourDigits": "3210"
  },
  "usdcAmount": 100.50,
  "exchangeRate": 0.9998,
  "usdAmount": 100.48,
  "conversionFee": 1.51,
  "payoutFee": 1.00,
  "totalFees": 2.51,
  "netAmount": 97.97,
  "status": "completed",
  "failureReason": null,
  "initiatedAt": "2025-02-03T10:00:00Z",
  "completedAt": "2025-02-04T15:30:00Z",
  "estimatedArrival": "2025-02-05T10:00:00Z",
  "gatewayTransactionId": "gw-tx-123456"
}
```

**Acceptance Criteria**:
- [ ] Returns complete transaction details
- [ ] Includes fee breakdown
- [ ] Includes bank account details (masked)
- [ ] Validates user owns the payout
- [ ] Returns 200 on success, 404 if not found
- [ ] Unit and integration tests pass

---

#### BE-320: POST /api/payout/{id}/cancel - Cancel Payout (1.00 day)
**Priority**: LOW 🟢
**Owner**: BE-1
**Dependencies**: BE-315

**Description**: Endpoint to cancel pending payout transaction.

**Response** (200 OK):
```json
{
  "payoutId": "uuid",
  "status": "cancelled",
  "cancelledAt": "2025-02-03T11:00:00Z"
}
```

**Business Logic**:
1. Validate payout exists and belongs to user
2. Check payout is in "pending" status
3. Cancel with fiat gateway
4. Update status to "cancelled"
5. Create audit log
6. Return confirmation

**Acceptance Criteria**:
- [ ] Can only cancel "pending" payouts
- [ ] Gateway cancellation attempted
- [ ] Status updated to "cancelled"
- [ ] Audit log entry created
- [ ] Returns 200 on success
- [ ] Returns 400 if cannot cancel (already processing)
- [ ] Unit and integration tests pass

---

#### BE-321: Payout Audit Trail & Logging (2.00 days)
**Priority**: HIGH 🔴
**Owner**: Senior BE
**Dependencies**: BE-315

**Description**: Comprehensive audit logging for all payout operations.

**Audit Events**:
- `payout.initiated` - Payout created
- `payout.status_changed` - Status updated
- `payout.webhook_received` - Gateway webhook
- `payout.cancelled` - User cancellation
- `payout.failed` - Gateway failure
- `payout.completed` - Successful completion

**Implementation**:
1. Create audit log service
2. Log all state changes
3. Log all API calls to gateway
4. Store webhook payloads
5. Add structured logging (Serilog)
6. Implement audit log viewer (optional)

**Acceptance Criteria**:
- [ ] All payout events logged
- [ ] Audit logs include user, timestamp, event data
- [ ] Audit logs queryable by payout ID
- [ ] Structured logging configured
- [ ] Log retention policy defined
- [ ] Unit tests cover logging
- [ ] Documentation on audit log format

---

## Technical Specifications

### Security Requirements

1. **Encryption**:
   - AES-256-GCM for bank account data
   - Unique IV per encryption operation
   - Secure key storage (environment variable or KMS)

2. **Authentication**:
   - All endpoints require valid JWT token
   - User can only access their own data

3. **Authorization**:
   - Bank account ownership validation
   - Payout ownership validation

4. **Data Protection**:
   - Never log/expose full routing/account numbers
   - Sensitive data encrypted at rest
   - TLS 1.3 for data in transit

5. **Webhook Security**:
   - HMAC-SHA256 signature validation
   - Replay attack prevention (timestamp validation)

### Performance Requirements

- API response time: < 2s (P95)
- Exchange rate refresh: Every 30 seconds
- Webhook processing: < 1s
- Database queries: < 500ms
- Support 100+ concurrent payout initiations

### Error Handling

**Error Response Format**:
```json
{
  "error": {
    "code": "INSUFFICIENT_BALANCE",
    "message": "Insufficient USDC balance for payout",
    "details": {
      "required": 100.50,
      "available": 50.25
    }
  }
}
```

**Error Codes**:
- `INSUFFICIENT_BALANCE` - Not enough USDC
- `INVALID_BANK_ACCOUNT` - Invalid bank account
- `GATEWAY_ERROR` - Fiat gateway error
- `RATE_EXPIRED` - Exchange rate expired
- `PAYOUT_CANCELLED` - Payout already cancelled
- `VALIDATION_ERROR` - Input validation failed

---

## Testing Strategy

### Unit Tests (Target: 80% coverage)

- Repository layer: All CRUD operations
- Service layer: Business logic, fee calculations
- Validation: Bank account validation rules
- Encryption: Encrypt/decrypt operations

### Integration Tests

- Database operations with real PostgreSQL
- Redis caching operations
- API endpoints with TestServer

### External Integration Tests

- Fiat gateway sandbox testing
- Webhook simulation

---

## Configuration

### appsettings.json

```json
{
  "Encryption": {
    "Provider": "AES",
    "KeySource": "Environment"
  },
  "FiatGateway": {
    "Provider": "RedotPay",
    "ApiKey": "",
    "ApiSecret": "",
    "BaseUrl": "https://sandbox.redotpay.com/api/v1",
    "WebhookSecret": "",
    "RetryAttempts": 3,
    "TimeoutSeconds": 30
  },
  "PayoutSettings": {
    "ConversionFeePercent": 1.5,
    "PayoutFeeUsd": 1.00,
    "ExchangeRateCacheTtlSeconds": 30,
    "ExchangeRateLockSeconds": 30,
    "MinPayoutAmountUsdc": 10.00,
    "MaxPayoutAmountUsdc": 10000.00
  }
}
```

### Environment Variables

```bash
ENCRYPTION_KEY=base64-encoded-32-byte-key
FIAT_GATEWAY_API_KEY=your-api-key
FIAT_GATEWAY_API_SECRET=your-api-secret
FIAT_GATEWAY_WEBHOOK_SECRET=your-webhook-secret
```

---

## Dependencies

### NuGet Packages

- `Microsoft.EntityFrameworkCore` (9.x) - Already installed
- `StackExchange.Redis` (2.x) - Already installed
- `System.Security.Cryptography` (Built-in)
- `Newtonsoft.Json` or `System.Text.Json` (Built-in)

### External Services

- RedotPay/Bridge/Wyre (Fiat Gateway)
- PostgreSQL (Database)
- Redis (Caching)

---

## Timeline & Milestones

### Week 1 (February 3-7)

**Day 1-2**:
- BE-301: Bank account model ✅
- BE-302: Encryption service (started)
- BE-308: Gateway interface ✅

**Day 3-4**:
- BE-302: Encryption service ✅
- BE-303-306: All bank account APIs ✅
- BE-307: Validation service ✅

**Day 5** (Mid-Sprint Demo):
- BE-309: Gateway integration (started)
- Demo: Bank account CRUD working

### Week 2 (February 10-14)

**Day 6-7**:
- BE-309: Gateway integration ✅
- BE-310: Exchange rate service ✅
- BE-311: Fee calculator ✅
- BE-312: Exchange rate endpoint ✅

**Day 8-9**:
- BE-313: Initiate payout ✅
- BE-314: Webhook handler ✅
- BE-315: Payout repository ✅
- BE-316: Status update service ✅

**Day 10** (Sprint Review):
- BE-317-320: Payout endpoints ✅
- BE-321: Audit trail ✅
- Demo: Complete fiat off-ramp flow

---

## Definition of Done

- [ ] All 21 backend tasks completed
- [ ] Unit tests: > 80% coverage
- [ ] Integration tests: All passing
- [ ] API documentation updated (Swagger)
- [ ] Security audit passed
- [ ] Code reviewed and approved
- [ ] Database migrations applied
- [ ] Configuration documented
- [ ] No Critical/High bugs
- [ ] Performance metrics met (<2s API)

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Fiat gateway API complexity | High | Medium | Early POC, thorough documentation review |
| Encryption key management | High | Low | Use AWS KMS or secure env vars |
| Gateway sandbox downtime | Medium | Low | Mock gateway responses for development |
| Bank validation complexity | Medium | Medium | Start with basic validation, iterate |
| Webhook reliability | Medium | Low | Implement polling fallback |

---

**Document Owner**: Backend Lead
**Last Updated**: 2025-10-29
**Version**: 1.0
