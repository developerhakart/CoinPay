# Sprint N03 QA Plan - Phase 3: Fiat Off-Ramp Testing

**Sprint**: N03
**Duration**: 2 weeks (10 working days)
**Dates**: February 3-14, 2025
**Team Size**: 2-3 QA Engineers
**Total Effort**: ~20 days
**Owner**: QA Lead

---

## Sprint Goals

### Primary Goals

1. **Functional Testing**: Comprehensive testing of bank account management and fiat withdrawal features
2. **Security Testing**: Validate encryption, data protection, and secure handling of sensitive information
3. **Integration Testing**: Test fiat gateway integration in sandbox environment
4. **E2E Automation**: Automate critical user journeys for withdrawal flow

### Success Criteria

- ✅ All Phase 3 features tested (bank accounts, payouts)
- ✅ Security audit passed (encryption, data protection)
- ✅ Integration tests with fiat gateway sandbox successful
- ✅ E2E tests automated for critical flows
- ✅ Performance testing meets thresholds
- ✅ Compliance testing passed (KYC/AML basic checks)
- ✅ Regression testing ensures Phases 1-2 still work
- ✅ Zero Critical bugs, < 3 High bugs at sprint end
- ✅ Test coverage > 80%

---

## Test Strategy

### Testing Levels

```
┌─────────────────────────────────────┐
│  Manual Exploratory Testing (20%)  │
├─────────────────────────────────────┤
│  E2E Automated Tests (30%)          │
├─────────────────────────────────────┤
│  Integration Tests (25%)            │
├─────────────────────────────────────┤
│  Unit Tests (25%)                   │
└─────────────────────────────────────┘
```

### Test Types

1. **Functional Testing** - Feature correctness
2. **Security Testing** - Data protection, encryption
3. **Integration Testing** - External gateway testing
4. **Performance Testing** - Load testing payout APIs
5. **Usability Testing** - User experience validation
6. **Accessibility Testing** - WCAG 2.1 AA compliance
7. **Compliance Testing** - KYC/AML workflows
8. **Regression Testing** - Existing features still work

---

## Task Breakdown

### QA-301: Phase 3 Functional Test Plan (1.00 day)
**Priority**: HIGH 🔴
**Owner**: QA Lead

**Description**: Create comprehensive test plan for Phase 3 fiat off-ramp features.

**Test Plan Sections**:
1. **Scope**: Bank accounts, payouts, exchange rates
2. **Test Objectives**: Validate all Phase 3 features
3. **Test Approach**: Manual + automated
4. **Test Environment**: Sandbox fiat gateway
5. **Test Data**: Test bank accounts, test users
6. **Entry/Exit Criteria**: When to start/stop testing
7. **Risk Assessment**: Identify high-risk areas
8. **Test Schedule**: Timeline for testing activities

**Test Areas**:
- Bank Account Management (CRUD)
- Bank Account Validation
- Fiat Gateway Integration
- Exchange Rate Display
- Payout Initiation
- Payout Status Tracking
- Payout History
- Webhook Handling
- Fee Calculation
- Encryption/Decryption

**Deliverables**:
- [ ] Test plan document
- [ ] Test case inventory
- [ ] Risk assessment matrix
- [ ] Test schedule
- [ ] Resource allocation plan

**Files**:
- `Testing/QA/QA-301-Phase3-Test-Plan.md`

---

### QA-302: Bank Account Management Testing (3.00 days)
**Priority**: HIGH 🔴
**Owner**: QA-1
**Dependencies**: Backend/Frontend

**Description**: Comprehensive testing of bank account CRUD operations and validation.

**Test Scenarios**:

#### Add Bank Account
- [ ] TC-301.1: Add valid US bank account (checking)
- [ ] TC-301.2: Add valid US bank account (savings)
- [ ] TC-301.3: Add bank account with invalid routing number
- [ ] TC-301.4: Add bank account with invalid account number
- [ ] TC-301.5: Add bank account with invalid account holder name
- [ ] TC-301.6: Add bank account with all optional fields
- [ ] TC-301.7: Add bank account and set as primary
- [ ] TC-301.8: Add second bank account (non-primary)
- [ ] TC-301.9: Verify sensitive data encrypted in database
- [ ] TC-301.10: Verify only last 4 digits shown in UI

#### View Bank Accounts
- [ ] TC-302.1: View empty bank accounts list
- [ ] TC-302.2: View single bank account
- [ ] TC-302.3: View multiple bank accounts
- [ ] TC-302.4: Verify primary account highlighted
- [ ] TC-302.5: Verify sensitive data not exposed in API response

#### Edit Bank Account
- [ ] TC-303.1: Edit bank account holder name
- [ ] TC-303.2: Edit bank name
- [ ] TC-303.3: Toggle primary account flag
- [ ] TC-303.4: Verify cannot edit routing/account number
- [ ] TC-303.5: Edit bank account owned by different user (negative)

#### Delete Bank Account
- [ ] TC-304.1: Delete bank account with no payouts
- [ ] TC-304.2: Cannot delete bank account with pending payout
- [ ] TC-304.3: Delete confirmation dialog shown
- [ ] TC-304.4: Delete bank account owned by different user (negative)

#### Validation Testing
- [ ] TC-305.1: Routing number - valid 9 digits
- [ ] TC-305.2: Routing number - invalid checksum
- [ ] TC-305.3: Routing number - < 9 digits
- [ ] TC-305.4: Routing number - > 9 digits
- [ ] TC-305.5: Routing number - non-numeric characters
- [ ] TC-306.1: Account number - valid (5-17 digits)
- [ ] TC-306.2: Account number - < 5 digits
- [ ] TC-306.3: Account number - > 17 digits
- [ ] TC-306.4: Account number - non-numeric characters
- [ ] TC-307.1: Account holder name - valid
- [ ] TC-307.2: Account holder name - < 2 characters
- [ ] TC-307.3: Account holder name - > 255 characters
- [ ] TC-307.4: Account holder name - special characters

**Total Test Cases**: ~40

**Test Data**:
```json
Valid Test Bank Accounts:
{
  "routing": "011401533",  // Valid Wells Fargo routing
  "account": "1234567890",
  "holderName": "John Doe"
}

Invalid Routing Numbers:
- "123456789" (invalid checksum)
- "12345678" (too short)
- "1234567890" (too long)
- "ABCDEFGHI" (non-numeric)
```

**Deliverables**:
- [ ] Test cases executed
- [ ] Defect reports filed
- [ ] Test execution report
- [ ] Security validation (encryption verified)

**Files**:
- `Testing/QA/QA-302-Bank-Account-Tests.md`

---

### QA-303: Fiat Gateway Integration Testing (3.00 days)
**Priority**: HIGH 🔴
**Owner**: QA-2
**Dependencies**: BE-309

**Description**: Integration testing with fiat gateway sandbox environment.

**Test Scenarios**:

#### Exchange Rate API
- [ ] TC-308.1: Fetch USDC/USD exchange rate
- [ ] TC-308.2: Verify rate within acceptable range (0.99-1.01)
- [ ] TC-308.3: Verify rate caching (30s TTL)
- [ ] TC-308.4: Verify rate refresh after expiration
- [ ] TC-308.5: Handle gateway API timeout
- [ ] TC-308.6: Handle gateway API error (500)
- [ ] TC-308.7: Fallback to previous rate on failure

#### Payout Initiation
- [ ] TC-309.1: Initiate payout with valid data
- [ ] TC-309.2: Verify payout submitted to gateway
- [ ] TC-309.3: Verify gateway transaction ID stored
- [ ] TC-309.4: Initiate payout with insufficient balance
- [ ] TC-309.5: Initiate payout with invalid bank account
- [ ] TC-309.6: Initiate payout with expired exchange rate
- [ ] TC-309.7: Verify payout fees calculated correctly
- [ ] TC-309.8: Verify conversion calculated correctly
- [ ] TC-309.9: Handle gateway rejection
- [ ] TC-309.10: Handle gateway timeout

#### Payout Status Tracking
- [ ] TC-310.1: Query payout status from gateway
- [ ] TC-310.2: Verify status mapped correctly (pending)
- [ ] TC-310.3: Verify status mapped correctly (processing)
- [ ] TC-310.4: Verify status mapped correctly (completed)
- [ ] TC-310.5: Verify status mapped correctly (failed)

#### Webhook Processing
- [ ] TC-311.1: Receive valid webhook (pending→processing)
- [ ] TC-311.2: Receive valid webhook (processing→completed)
- [ ] TC-311.3: Receive valid webhook (processing→failed)
- [ ] TC-311.4: Verify webhook signature validation
- [ ] TC-311.5: Reject webhook with invalid signature
- [ ] TC-311.6: Handle duplicate webhooks (idempotency)
- [ ] TC-311.7: Verify audit log created on webhook
- [ ] TC-311.8: Verify payout status updated in database

#### Payout Cancellation
- [ ] TC-312.1: Cancel pending payout
- [ ] TC-312.2: Cannot cancel processing payout
- [ ] TC-312.3: Cannot cancel completed payout
- [ ] TC-312.4: Verify cancellation submitted to gateway
- [ ] TC-312.5: Verify audit log for cancellation

**Total Test Cases**: ~35

**Test Environment**:
- Fiat gateway sandbox: RedotPay/Bridge test environment
- Test API credentials configured
- Webhook endpoint accessible (ngrok for local testing)

**Mock Gateway Responses**:
```json
// Success Response
{
  "transaction_id": "gw-tx-123456",
  "status": "pending",
  "estimated_arrival": "2025-02-05T10:00:00Z"
}

// Error Response
{
  "error": "insufficient_balance",
  "message": "Insufficient balance in source account"
}
```

**Deliverables**:
- [ ] Test cases executed in sandbox
- [ ] Gateway API documentation reviewed
- [ ] Webhook testing completed
- [ ] Defect reports filed
- [ ] Test execution report

**Files**:
- `Testing/QA/QA-303-Gateway-Integration-Tests.md`

---

### QA-304: Withdrawal Flow E2E Tests (Cypress) (3.00 days)
**Priority**: HIGH 🔴
**Owner**: QA-1
**Dependencies**: Frontend

**Description**: Automated E2E tests for complete withdrawal user journey.

**Test Scenarios**:

#### Complete Withdrawal Flow
```typescript
describe('Fiat Withdrawal Flow', () => {
  beforeEach(() => {
    cy.login('testuser@test.com');
    cy.visit('/withdraw');
  });

  it('TC-313.1: Complete withdrawal flow (happy path)', () => {
    // Step 1: Enter amount
    cy.get('[data-testid="usdc-amount-input"]').type('100');
    cy.get('[data-testid="next-button"]').click();

    // Step 2: Select bank account
    cy.get('[data-testid="bank-account-card"]').first().click();
    cy.get('[data-testid="next-button"]').click();

    // Step 3: Review details
    cy.get('[data-testid="review-usdc-amount"]').should('contain', '100.00');
    cy.get('[data-testid="review-net-amount"]').should('be.visible');
    cy.get('[data-testid="confirm-button"]').click();

    // Step 4: Confirmation
    cy.get('[data-testid="success-message"]').should('be.visible');
    cy.get('[data-testid="payout-id"]').should('exist');
  });

  it('TC-313.2: Withdrawal with insufficient balance', () => {
    cy.get('[data-testid="usdc-amount-input"]').type('10000');
    cy.get('[data-testid="next-button"]').click();
    cy.get('[data-testid="error-message"]')
      .should('contain', 'Insufficient balance');
  });

  it('TC-313.3: Back navigation preserves form state', () => {
    // Enter amount
    cy.get('[data-testid="usdc-amount-input"]').type('100');
    cy.get('[data-testid="next-button"]').click();

    // Select bank
    cy.get('[data-testid="bank-account-card"]').first().click();
    cy.get('[data-testid="next-button"]').click();

    // Go back
    cy.get('[data-testid="back-button"]').click();
    cy.get('[data-testid="bank-account-card"]').first()
      .should('have.class', 'selected');

    // Go back again
    cy.get('[data-testid="back-button"]').click();
    cy.get('[data-testid="usdc-amount-input"]')
      .should('have.value', '100');
  });
});
```

#### Bank Account Management Flow
```typescript
describe('Bank Account Management', () => {
  it('TC-314.1: Add new bank account', () => {
    cy.visit('/bank-accounts');
    cy.get('[data-testid="add-bank-button"]').click();

    // Fill form
    cy.get('[data-testid="account-holder-name"]').type('John Doe');
    cy.get('[data-testid="routing-number"]').type('011401533');
    cy.get('[data-testid="account-number"]').type('1234567890');
    cy.get('[data-testid="account-type"]').select('checking');
    cy.get('[data-testid="bank-name"]').type('Wells Fargo');

    // Submit
    cy.get('[data-testid="submit-button"]').click();

    // Verify success
    cy.get('[data-testid="success-notification"]').should('be.visible');
    cy.get('[data-testid="bank-account-card"]')
      .should('contain', 'Wells Fargo')
      .and('contain', '•••• 7890');
  });

  it('TC-314.2: Validation errors shown', () => {
    cy.visit('/bank-accounts');
    cy.get('[data-testid="add-bank-button"]').click();

    // Invalid routing number
    cy.get('[data-testid="routing-number"]').type('123456789');
    cy.get('[data-testid="routing-error"]')
      .should('contain', 'Invalid routing number');

    // Too short account number
    cy.get('[data-testid="account-number"]').type('123');
    cy.get('[data-testid="account-error"]')
      .should('contain', 'Account number must be 5-17 digits');
  });
});
```

#### Exchange Rate Display
```typescript
describe('Exchange Rate', () => {
  it('TC-315.1: Exchange rate displays and refreshes', () => {
    cy.visit('/withdraw');

    // Verify rate displayed
    cy.get('[data-testid="exchange-rate"]')
      .should('contain', 'USDC')
      .and('contain', 'USD');

    // Verify countdown
    cy.get('[data-testid="rate-countdown"]').should('exist');

    // Wait and verify auto-refresh
    cy.wait(30000);
    cy.get('[data-testid="rate-updated-indicator"]').should('be.visible');
  });

  it('TC-315.2: Manual rate refresh', () => {
    cy.visit('/withdraw');
    cy.get('[data-testid="refresh-rate-button"]').click();
    cy.get('[data-testid="loading-spinner"]').should('be.visible');
    cy.get('[data-testid="exchange-rate"]').should('be.visible');
  });
});
```

#### Payout History
```typescript
describe('Payout History', () => {
  it('TC-316.1: View payout history', () => {
    cy.visit('/payouts');

    // Verify list displayed
    cy.get('[data-testid="payout-card"]').should('have.length.gt', 0);

    // Click to view details
    cy.get('[data-testid="payout-card"]').first().click();
    cy.get('[data-testid="payout-detail-modal"]').should('be.visible');
  });

  it('TC-316.2: Filter by status', () => {
    cy.visit('/payouts');
    cy.get('[data-testid="status-filter"]').select('completed');

    // Verify only completed payouts shown
    cy.get('[data-testid="payout-card"]').each(($card) => {
      cy.wrap($card).find('[data-testid="status-badge"]')
        .should('contain', 'Completed');
    });
  });
});
```

**Total E2E Tests**: ~25 tests

**Test Coverage**:
- Complete withdrawal wizard (happy path)
- Bank account management (add, edit, delete)
- Validation error handling
- Exchange rate display and refresh
- Payout history and filtering
- Payout status tracking
- Mobile responsive testing

**Deliverables**:
- [ ] Cypress test suite created
- [ ] All E2E tests passing
- [ ] Test execution report
- [ ] Screenshots/videos of test runs

**Files**:
- `Testing/E2E/cypress/e2e/fiat-withdrawal.cy.ts`
- `Testing/E2E/cypress/e2e/bank-accounts.cy.ts`
- `Testing/E2E/cypress/e2e/payout-history.cy.ts`

---

### QA-305: Security Testing (Encryption, Data Protection) (2.50 days)
**Priority**: HIGH 🔴
**Owner**: QA Lead
**Dependencies**: BE-302

**Description**: Comprehensive security testing for sensitive data protection.

**Test Scenarios**:

#### Encryption Testing
- [ ] TC-317.1: Verify routing number encrypted in database
- [ ] TC-317.2: Verify account number encrypted in database
- [ ] TC-317.3: Verify unique IV used for each encryption
- [ ] TC-317.4: Verify AES-256-GCM algorithm used
- [ ] TC-317.5: Decrypt and verify data integrity
- [ ] TC-317.6: Verify encryption keys stored securely (not in code)

#### Data Exposure Testing
- [ ] TC-318.1: Verify full routing number NOT in API responses
- [ ] TC-318.2: Verify full account number NOT in API responses
- [ ] TC-318.3: Verify only last 4 digits shown in UI
- [ ] TC-318.4: Verify sensitive data NOT in application logs
- [ ] TC-318.5: Verify sensitive data NOT in error messages
- [ ] TC-318.6: Verify network traffic encrypted (HTTPS)

#### Authentication & Authorization
- [ ] TC-319.1: Verify all endpoints require authentication
- [ ] TC-319.2: Verify user can only access their own bank accounts
- [ ] TC-319.3: Verify user can only access their own payouts
- [ ] TC-319.4: Verify JWT token expiration enforced
- [ ] TC-319.5: Verify invalid tokens rejected

#### Webhook Security
- [ ] TC-320.1: Verify webhook signature validation (HMAC)
- [ ] TC-320.2: Reject webhook with invalid signature
- [ ] TC-320.3: Verify replay attack prevention (timestamp check)
- [ ] TC-320.4: Verify webhook endpoint requires HTTPS

#### SQL Injection Testing
- [ ] TC-321.1: Test bank account name with SQL injection
- [ ] TC-321.2: Test payout filters with SQL injection
- [ ] TC-321.3: Verify parameterized queries used

#### XSS Testing
- [ ] TC-322.1: Test bank account name with XSS payload
- [ ] TC-322.2: Verify input sanitization on frontend
- [ ] TC-322.3: Verify output encoding in UI

**Total Test Cases**: ~25

**Security Tools**:
- OWASP ZAP (automated scan)
- Burp Suite (manual testing)
- Database inspector (check encrypted columns)
- Wireshark (network traffic analysis)

**Acceptance Criteria**:
- [ ] Zero Critical security vulnerabilities
- [ ] Zero High security vulnerabilities
- [ ] All sensitive data encrypted
- [ ] No data exposure in logs/errors
- [ ] Authentication/authorization working

**Deliverables**:
- [ ] Security test report
- [ ] OWASP ZAP scan results
- [ ] Encryption verification report
- [ ] Remediation recommendations (if issues found)

**Files**:
- `Testing/Security/QA-305-Security-Testing.md`
- `Testing/Security/OWASP-Scan-Results.pdf`

---

### QA-306: Negative Testing (Invalid Data, Edge Cases) (2.00 days)
**Priority**: HIGH 🔴
**Owner**: QA-2
**Dependencies**: All APIs

**Description**: Test system behavior with invalid inputs and edge cases.

**Test Scenarios**:

#### Invalid Bank Account Data
- [ ] TC-323.1: Null routing number
- [ ] TC-323.2: Empty string routing number
- [ ] TC-323.3: Negative account number
- [ ] TC-323.4: SQL injection in account holder name
- [ ] TC-323.5: XSS payload in bank name
- [ ] TC-323.6: Extremely long account holder name (1000+ chars)
- [ ] TC-323.7: Unicode characters in routing number

#### Invalid Payout Data
- [ ] TC-324.1: Negative USDC amount
- [ ] TC-324.2: Zero USDC amount
- [ ] TC-324.3: Extremely large amount (1000000+ USDC)
- [ ] TC-324.4: Non-existent bank account ID
- [ ] TC-324.5: Invalid UUID format for bank account
- [ ] TC-324.6: Payout with deleted bank account
- [ ] TC-324.7: Concurrent payout initiations (race condition)

#### Exchange Rate Edge Cases
- [ ] TC-325.1: Rate fetch when gateway is down
- [ ] TC-325.2: Rate fetch timeout (30s+)
- [ ] TC-325.3: Invalid rate response format
- [ ] TC-325.4: Rate = 0
- [ ] TC-325.5: Negative rate
- [ ] TC-325.6: Extremely volatile rate (>10% change)

#### Webhook Edge Cases
- [ ] TC-326.1: Webhook with missing fields
- [ ] TC-326.2: Webhook with invalid status
- [ ] TC-326.3: Webhook for non-existent payout
- [ ] TC-326.4: Webhook with malformed JSON
- [ ] TC-326.5: Webhook received multiple times (duplicate)

#### Boundary Testing
- [ ] TC-327.1: Min payout amount (exactly at limit)
- [ ] TC-327.2: Below min payout amount
- [ ] TC-327.3: Max payout amount (exactly at limit)
- [ ] TC-327.4: Above max payout amount
- [ ] TC-327.5: Balance exactly equal to payout amount
- [ ] TC-327.6: Balance 0.01 USDC below required

**Total Test Cases**: ~30

**Error Handling Verification**:
- Appropriate HTTP status codes (400, 404, 500)
- Clear error messages
- No system crashes or unhandled exceptions
- Graceful degradation

**Deliverables**:
- [ ] Test cases executed
- [ ] Defect reports for poor error handling
- [ ] Test execution report

**Files**:
- `Testing/QA/QA-306-Negative-Testing.md`

---

### QA-307: Performance Testing (Payout API Load) (1.50 days)
**Priority**: MEDIUM 🟡
**Owner**: QA-1
**Dependencies**: K6

**Description**: Load testing for payout APIs with 100+ concurrent users.

**Test Script** (K6):
```javascript
// payout-load-test.js
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const payoutInitiationDuration = new Trend('payout_initiation_duration');
const payoutSuccessRate = new Rate('payout_success_rate');

export const options = {
  stages: [
    { duration: '2m', target: 50 },   // Ramp up to 50 users
    { duration: '3m', target: 100 },  // Ramp up to 100 users
    { duration: '5m', target: 100 },  // Stay at 100 users
    { duration: '2m', target: 0 },    // Ramp down
  ],
  thresholds: {
    'payout_initiation_duration': ['p(95)<2000'], // P95 < 2s
    'http_req_failed': ['rate<0.01'], // <1% errors
    'payout_success_rate': ['rate>0.95'], // >95% success
  },
};

export default function () {
  // Login
  const loginRes = http.post(`${BASE_URL}/api/auth/login`, {
    email: 'testuser@test.com',
    password: 'testpass',
  });

  const token = loginRes.json('token');

  // Initiate payout
  const payoutRes = http.post(
    `${BASE_URL}/api/payout/initiate`,
    JSON.stringify({
      bankAccountId: 'test-bank-account-id',
      usdcAmount: 100.0,
      lockRate: true,
    }),
    {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    }
  );

  payoutInitiationDuration.add(payoutRes.timings.duration);

  const success = check(payoutRes, {
    'payout initiated successfully': (r) => r.status === 201,
    'payout response time acceptable': (r) => r.timings.duration < 2000,
  });

  payoutSuccessRate.add(success);

  sleep(1);
}
```

**Performance Targets**:
- P95 payout initiation: < 2s
- P95 payout history: < 1.5s
- P95 exchange rate fetch: < 500ms
- Error rate: < 1%
- Success rate: > 95%
- Concurrent users: 100+

**Test Scenarios**:
- [ ] TC-328.1: Load test - 100 concurrent users
- [ ] TC-328.2: Stress test - 200 concurrent users
- [ ] TC-328.3: Spike test - sudden surge to 300 users
- [ ] TC-328.4: Endurance test - 100 users for 30 minutes

**Deliverables**:
- [ ] K6 test scripts
- [ ] Performance test results
- [ ] Performance report with metrics
- [ ] Recommendations for optimization

**Files**:
- `Testing/Performance/k6/payout-load-test.js`
- `Testing/Performance/QA-307-Performance-Report.md`

---

### QA-308: Compliance Testing (KYC/AML) (1.50 days)
**Priority**: MEDIUM 🟡
**Owner**: QA Lead
**Dependencies**: Backend

**Description**: Validate KYC/AML compliance workflows (basic MVP requirements).

**Test Scenarios**:

#### User Verification
- [ ] TC-329.1: Verify user email verified before adding bank account
- [ ] TC-329.2: Verify basic identity information collected
- [ ] TC-329.3: Verify terms of service acceptance required
- [ ] TC-329.4: Verify payout limits enforced (MVP: $10K daily)

#### Transaction Monitoring
- [ ] TC-330.1: Verify payout audit logs created
- [ ] TC-330.2: Verify all payouts have complete audit trail
- [ ] TC-330.3: Verify failed payouts logged with reason
- [ ] TC-330.4: Verify suspicious activity flagged (>$5K single payout)

#### Recordkeeping
- [ ] TC-331.1: Verify bank account details stored securely
- [ ] TC-331.2: Verify payout transaction history retrievable
- [ ] TC-331.3: Verify audit logs retained (7 years requirement)

**Compliance Checklist**:
- [ ] KYC: Basic user identity collection
- [ ] AML: Transaction monitoring and reporting
- [ ] Recordkeeping: Audit trail for all transactions
- [ ] Limits: Daily/monthly payout limits enforced
- [ ] Terms: User acceptance of terms documented

**Deliverables**:
- [ ] Compliance test report
- [ ] Audit trail verification
- [ ] Recommendations for full compliance

**Files**:
- `Testing/Compliance/QA-308-Compliance-Testing.md`

---

### QA-309: Regression Testing (Phases 1-2) (2.00 days)
**Priority**: HIGH 🔴
**Owner**: QA-2

**Description**: Ensure Phases 1-2 features still work after Phase 3 changes.

**Regression Test Suite**:

#### Phase 1 Features
- [ ] Passkey authentication (login/logout)
- [ ] Wallet creation
- [ ] USDC balance display
- [ ] Gasless USDC transfers
- [ ] Transaction status tracking

#### Phase 2 Features
- [ ] Transaction history with pagination
- [ ] Transaction filtering and sorting
- [ ] Transaction detail modal
- [ ] QR code generation
- [ ] Background transaction monitoring

**Test Approach**:
1. Run existing automated test suites (Playwright + Cypress)
2. Execute critical path manual tests
3. Verify no regressions introduced

**Automated Tests**:
```bash
# Run Phase 1 tests
cd Testing/E2E/playwright
npm run test:phase1

# Run Phase 2 tests
cd Testing/E2E/cypress
npm run cy:run:phase2
```

**Deliverables**:
- [ ] Regression test execution report
- [ ] Defect reports for any regressions
- [ ] Pass/fail summary

**Files**:
- `Testing/QA/QA-309-Regression-Report.md`

---

### QA-310: Bug Triage & Resolution Support (0.50 days)
**Priority**: HIGH 🔴
**Owner**: QA Lead
**Dependencies**: Ongoing

**Description**: Daily bug triage meetings and resolution support.

**Activities**:
- Daily bug triage (15-30 min)
- Prioritize bugs (Critical, High, Medium, Low)
- Assign bugs to developers
- Verify bug fixes
- Update bug status
- Sprint end bug burndown

**Bug Severity Guidelines**:
- **Critical**: System crash, data loss, security breach
- **High**: Major feature broken, no workaround
- **Medium**: Feature partially broken, workaround available
- **Low**: Minor UI issue, cosmetic

**Deliverables**:
- [ ] Bug triage notes (daily)
- [ ] Bug priority assignments
- [ ] Bug verification reports
- [ ] Sprint bug summary

---

## Test Environments

### Development Environment
- URL: http://localhost:3000
- API: http://localhost:5000
- Database: Local PostgreSQL
- Gateway: Mock responses

### QA Environment
- URL: https://qa.coinpay.app
- API: https://api-qa.coinpay.app
- Database: QA PostgreSQL
- Gateway: Sandbox environment

### Test Data

```json
// Test Users
{
  "testuser1": {
    "email": "testuser1@test.com",
    "password": "TestPass123!",
    "balance": 1000.00
  },
  "testuser2": {
    "email": "testuser2@test.com",
    "password": "TestPass123!",
    "balance": 50.00
  }
}

// Test Bank Accounts
{
  "valid_checking": {
    "routing": "011401533",
    "account": "1234567890",
    "holderName": "John Doe",
    "type": "checking"
  },
  "valid_savings": {
    "routing": "011401533",
    "account": "0987654321",
    "holderName": "Jane Smith",
    "type": "savings"
  }
}
```

---

## Test Metrics & Reporting

### Test Coverage Metrics

- Test case coverage: > 95% of requirements
- Code coverage: > 80%
- E2E test coverage: Critical user paths

### Defect Metrics

- Defect detection rate
- Defect resolution time
- Defect by severity
- Defect by module

### Test Execution Metrics

- Tests executed vs planned
- Pass/fail rate
- Test execution time
- Automation coverage

### Sprint End Report

**Sprint N03 QA Summary Report** will include:
1. Test execution summary
2. Defect summary
3. Test coverage analysis
4. Risk assessment
5. Recommendations
6. Sign-off

---

## Timeline & Milestones

### Week 1 (February 3-7)

**Day 1**:
- QA-301: Test plan ✅

**Day 2-3**:
- QA-302: Bank account testing (started)

**Day 4-5**:
- QA-302: Bank account testing (continued)
- QA-303: Gateway integration (started)

**Mid-Sprint Checkpoint** (Day 5):
- Present test results
- Defect status update

### Week 2 (February 10-14)

**Day 6-7**:
- QA-303: Gateway integration ✅
- QA-304: E2E tests (started)
- QA-305: Security testing (started)

**Day 8-9**:
- QA-304: E2E tests ✅
- QA-305: Security testing ✅
- QA-306: Negative testing ✅
- QA-307: Performance testing ✅

**Day 10** (Sprint Review):
- QA-308: Compliance testing ✅
- QA-309: Regression testing ✅
- Test report finalization
- Sprint QA demo

---

## Definition of Done

- [ ] All 10 QA tasks completed
- [ ] Test plan approved
- [ ] All test cases executed
- [ ] E2E tests automated (25+ tests)
- [ ] Security testing passed (zero Critical/High)
- [ ] Performance testing met thresholds
- [ ] Regression testing passed
- [ ] Zero Critical bugs
- [ ] < 3 High priority bugs
- [ ] Test report published
- [ ] Sign-off from QA Lead

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Fiat gateway sandbox issues | High | Medium | Mock responses, early testing |
| Complex webhook testing | Medium | Medium | Use ngrok, automated test data |
| Security testing scope | High | Low | Focus on critical areas first |
| Limited test data | Medium | Low | Generate synthetic test data |

---

**Document Owner**: QA Lead
**Last Updated**: 2025-10-29
**Version**: 1.0
