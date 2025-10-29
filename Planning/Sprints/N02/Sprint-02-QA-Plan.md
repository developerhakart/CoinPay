# CoinPay Wallet MVP - Sprint N02 QA Plan

**Version**: 1.0
**Sprint Duration**: 2 weeks (10 working days)
**Sprint Period**: January 20 - January 31, 2025
**Team Composition**: 2-3 QA Engineers
**Available Capacity**: 20-30 engineering days
**Planned Effort**: ~23 days (77% utilization)
**Sprint Type**: Feature Testing Sprint

---

## Sprint Goal

**Achieve comprehensive test coverage for Phase 1 (Core Wallet Foundation) and Phase 2 (Transaction History & UI Polish) to ensure production-ready quality.**

By the end of Sprint N02, we will have:
- Phase 1 functional testing complete (Passkey Auth, Wallet Creation, Gasless Transfers)
- Phase 2 functional testing complete (Transaction History, Monitoring, UI Polish)
- Automated E2E tests for critical user journeys (Playwright + Cypress)
- Performance testing with 100+ concurrent users (K6)
- Security testing (OWASP Top 10 compliance)
- Accessibility testing (WCAG 2.1 AA > 90 score)
- Regression test suite for Sprint N01 features
- Bug triage and resolution support
- Test documentation and quality metrics

---

## Selected Tasks & Effort Distribution

### Phase 1 Testing (7.00 days)
- Functional testing of passkey auth, wallet creation, transfers
- Automated E2E tests for Phase 1 flows

### Phase 2 Testing (7.00 days)
- Functional testing of transaction history, monitoring, UI polish
- Automated E2E tests for Phase 2 flows

### Specialized Testing (9.00 days)
- Performance testing (100+ concurrent users)
- Security testing (OWASP Top 10)
- Accessibility testing (WCAG 2.1 AA)
- Regression testing
- Bug triage and resolution support

**Total Sprint N02 Effort**: ~23.00 days (within 20-30 day capacity)

---

## Task Breakdown with Details

### Epic 3.1: Phase 1 Testing (7.00 days)

#### QA-201: Phase 1 Functional Testing (4.00 days)
**Owner**: QA Engineer 1
**Priority**: P0 - Critical
**Dependencies**: Sprint N01 features deployed

**Description**:
Execute comprehensive functional testing for Phase 1 features.

**Test Scenarios**:

**1. Passkey Authentication (8 test cases)**:
- TC-101: User registration with passkey (happy path)
- TC-102: User registration with invalid username
- TC-103: User registration on unsupported browser
- TC-104: User login with passkey (happy path)
- TC-105: User login with invalid passkey
- TC-106: User login without registered passkey
- TC-107: Session persistence across page refresh
- TC-108: Logout functionality

**2. Wallet Creation (6 test cases)**:
- TC-201: Create wallet for new user
- TC-202: Create wallet - verify deterministic address
- TC-203: Create wallet - verify smart account properties
- TC-204: View wallet balance (0 USDC initially)
- TC-205: Copy wallet address to clipboard
- TC-206: Wallet dashboard displays correctly

**3. Gasless USDC Transfers (10 test cases)**:
- TC-301: Transfer USDC to valid address (happy path)
- TC-302: Transfer USDC - verify gasless (0 gas fee)
- TC-303: Transfer USDC with insufficient balance
- TC-304: Transfer USDC to invalid address
- TC-305: Transfer USDC with amount = 0
- TC-306: Transfer USDC with negative amount
- TC-307: Transfer USDC - verify transaction status updates
- TC-308: Transfer USDC - verify balance updates after confirmation
- TC-309: Transfer maximum balance ("Max" button)
- TC-310: Transfer USDC - transaction appears in history

**Test Environment**:
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Database: PostgreSQL (test database)
- Blockchain: Polygon Amoy testnet
- Test wallets: 10+ funded with testnet USDC

**Test Data**:
- Test users: 5 users with passkeys registered
- Test wallets: 10 wallets with various balances (0, 10, 100, 1000 USDC)
- Test transactions: 20+ pending/confirmed/failed transactions

**Acceptance Criteria**:
- [x] All 24 Phase 1 test cases executed
- [x] Test results documented in test management tool
- [x] Bugs logged for any failures
- [x] Pass rate > 90% (22/24 tests pass)
- [x] Critical bugs escalated immediately

**Definition of Done**:
- Test execution report generated
- Bugs logged and assigned
- Test cases updated with actual results

---

#### QA-202: Phase 1 Automated E2E Tests (3.00 days)
**Owner**: QA Engineer 1
**Priority**: P0 - Critical
**Dependencies**: Playwright setup (Sprint N01)

**Description**:
Create automated E2E tests for Phase 1 critical user journeys.

**Automated Test Suite**:

**File: `e2e/phase1-auth.spec.ts` (5 tests)**
```typescript
import { test, expect } from '@playwright/test';

test.describe('Phase 1: Passkey Authentication', () => {
  test('should register new user with passkey', async ({ page }) => {
    await page.goto('/register');

    // Fill username
    await page.fill('[data-testid="username-input"]', 'testuser_' + Date.now());

    // Click register button
    await page.click('[data-testid="register-button"]');

    // Simulate passkey creation (virtual authenticator)
    const cdpSession = await page.context().newCDPSession(page);
    await cdpSession.send('WebAuthn.enable');
    await cdpSession.send('WebAuthn.addVirtualAuthenticator', {
      options: {
        protocol: 'ctap2',
        transport: 'internal',
        hasResidentKey: true,
        hasUserVerification: true,
        isUserVerified: true,
      },
    });

    // Wait for registration to complete
    await expect(page).toHaveURL('/wallet', { timeout: 10000 });

    // Verify wallet dashboard displayed
    await expect(page.locator('[data-testid="wallet-dashboard"]')).toBeVisible();
  });

  test('should login existing user with passkey', async ({ page }) => {
    // Assume user already registered
    await page.goto('/login');

    await page.fill('[data-testid="username-input"]', 'existinguser');
    await page.click('[data-testid="login-button"]');

    // Simulate passkey authentication
    // (Virtual authenticator already configured)

    await expect(page).toHaveURL('/wallet', { timeout: 10000 });
    await expect(page.locator('[data-testid="wallet-dashboard"]')).toBeVisible();
  });

  test('should persist session across page refresh', async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('[data-testid="username-input"]', 'existinguser');
    await page.click('[data-testid="login-button"]');
    await expect(page).toHaveURL('/wallet');

    // Refresh page
    await page.reload();

    // Should still be logged in
    await expect(page).toHaveURL('/wallet');
    await expect(page.locator('[data-testid="wallet-dashboard"]')).toBeVisible();
  });

  test('should logout user', async ({ page }) => {
    // Login first
    await page.goto('/wallet');

    // Click logout button
    await page.click('[data-testid="logout-button"]');

    // Should redirect to login
    await expect(page).toHaveURL('/login');
  });

  test('should protect routes when not authenticated', async ({ page }) => {
    // Try to access protected route without login
    await page.goto('/wallet');

    // Should redirect to login
    await expect(page).toHaveURL('/login');
  });
});
```

**File: `e2e/phase1-wallet.spec.ts` (3 tests)**
```typescript
test.describe('Phase 1: Wallet Creation', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await loginAsTestUser(page);
  });

  test('should create wallet for new user', async ({ page }) => {
    await page.goto('/wallet');

    // Check if wallet creation prompt appears
    const createWalletButton = page.locator('[data-testid="create-wallet-button"]');
    if (await createWalletButton.isVisible()) {
      await createWalletButton.click();

      // Wait for wallet creation
      await expect(page.locator('[data-testid="wallet-address"]')).toBeVisible({ timeout: 15000 });
    }

    // Verify wallet address displayed
    const walletAddress = await page.locator('[data-testid="wallet-address"]').textContent();
    expect(walletAddress).toMatch(/^0x[a-fA-F0-9]{40}$/);
  });

  test('should display wallet balance', async ({ page }) => {
    await page.goto('/wallet');

    // Verify balance card displayed
    await expect(page.locator('[data-testid="balance-card"]')).toBeVisible();

    // Verify balance amount displayed
    const balance = await page.locator('[data-testid="balance-amount"]').textContent();
    expect(balance).toMatch(/^\d+(\.\d+)?\s*USDC$/);
  });

  test('should copy wallet address to clipboard', async ({ page }) => {
    await page.goto('/wallet');

    // Click copy button
    await page.click('[data-testid="copy-address-button"]');

    // Verify toast notification
    await expect(page.locator('.toast-success')).toContainText('copied', { ignoreCase: true });
  });
});
```

**File: `e2e/phase1-transfer.spec.ts` (4 tests)**
```typescript
test.describe('Phase 1: USDC Transfers', () => {
  test.beforeEach(async ({ page }) => {
    await loginAsTestUser(page);
    await page.goto('/transfer');
  });

  test('should transfer USDC successfully', async ({ page }) => {
    const recipientAddress = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb2';
    const amount = '10';

    // Fill transfer form
    await page.fill('[data-testid="recipient-input"]', recipientAddress);
    await page.fill('[data-testid="amount-input"]', amount);

    // Submit transfer
    await page.click('[data-testid="submit-transfer-button"]');

    // Confirm transfer
    await page.click('[data-testid="confirm-transfer-button"]');

    // Wait for transaction status page
    await expect(page).toHaveURL(/\/transactions\/[a-f0-9-]+/, { timeout: 10000 });

    // Verify transaction status is Pending
    await expect(page.locator('[data-testid="transaction-status"]')).toContainText('Pending');
  });

  test('should validate recipient address', async ({ page }) => {
    // Enter invalid address
    await page.fill('[data-testid="recipient-input"]', 'invalid-address');
    await page.fill('[data-testid="amount-input"]', '10');

    // Submit button should be disabled
    await expect(page.locator('[data-testid="submit-transfer-button"]')).toBeDisabled();

    // Error message should be displayed
    await expect(page.locator('[data-testid="recipient-error"]')).toContainText('Invalid');
  });

  test('should validate insufficient balance', async ({ page }) => {
    const recipientAddress = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb2';

    // Get current balance
    await page.goto('/wallet');
    const balanceText = await page.locator('[data-testid="balance-amount"]').textContent();
    const balance = parseFloat(balanceText.replace(/[^\d.]/g, ''));

    // Try to send more than balance
    await page.goto('/transfer');
    await page.fill('[data-testid="recipient-input"]', recipientAddress);
    await page.fill('[data-testid="amount-input"]', (balance + 100).toString());

    // Error message should be displayed
    await expect(page.locator('[data-testid="amount-error"]')).toContainText('Insufficient');
  });

  test('should use Max button to fill entire balance', async ({ page }) => {
    // Click Max button
    await page.click('[data-testid="max-button"]');

    // Amount should be filled with balance
    const amountValue = await page.locator('[data-testid="amount-input"]').inputValue();
    expect(parseFloat(amountValue)).toBeGreaterThan(0);
  });
});
```

**CI/CD Integration**:
```yaml
# .github/workflows/e2e-tests.yml
name: E2E Tests

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  e2e-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: npm ci

      - name: Install Playwright browsers
        run: npx playwright install --with-deps chromium

      - name: Run E2E tests
        run: npm run test:e2e

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: playwright-report
          path: playwright-report/
```

**Acceptance Criteria**:
- [x] 12+ automated E2E tests created
- [x] Tests cover critical user journeys
- [x] Tests run in CI/CD pipeline
- [x] Tests pass consistently (>95% pass rate)
- [x] Test reports generated

**Definition of Done**:
- E2E tests passing in CI/CD
- Test code reviewed
- Documentation updated

---

### Epic 3.2: Phase 2 Testing (7.00 days)

#### QA-203: Phase 2 Functional Testing (4.00 days)
**Owner**: QA Engineer 2
**Priority**: P0 - Critical
**Dependencies**: Backend BE-203 to BE-207, Frontend FE-205 to FE-213

**Description**:
Execute comprehensive functional testing for Phase 2 features.

**Test Scenarios**:

**1. Transaction Monitoring (5 test cases)**:
- TC-401: Background worker monitors pending transactions
- TC-402: Transaction status updates from Pending to Confirmed
- TC-403: Transaction status updates from Pending to Failed
- TC-404: Status updates occur within 60 seconds
- TC-405: Balance cache invalidated on confirmation

**2. Transaction History (12 test cases)**:
- TC-501: View transaction history with pagination
- TC-502: Navigate between pages (prev/next)
- TC-503: Sort by date (ascending/descending)
- TC-504: Sort by amount (ascending/descending)
- TC-505: Filter by status (Pending/Confirmed/Failed)
- TC-506: Filter by date range (Last 7/30 days)
- TC-507: Search by recipient address
- TC-508: Search by transaction hash
- TC-509: Combine multiple filters
- TC-510: Clear all filters
- TC-511: Empty state when no transactions
- TC-512: Display 100+ transactions smoothly

**3. Transaction Details (4 test cases)**:
- TC-601: View transaction detail modal
- TC-602: Display complete blockchain information
- TC-603: Copy address and transaction hash
- TC-604: Link to block explorer works

**4. UI Polish (8 test cases)**:
- TC-701: QR code generation displays correctly
- TC-702: QR code is scannable
- TC-703: Copy-to-clipboard works across all components
- TC-704: Loading skeletons display during data fetch
- TC-705: Error handling with retry mechanism
- TC-706: Responsive design on mobile (320px+)
- TC-707: Responsive design on tablet (768px+)
- TC-708: Responsive design on desktop (1024px+)

**Test Data Requirements**:
- Test transactions: 100+ transactions in various states
- Test wallets: 5+ wallets with transaction history
- Test filters: Various date ranges, statuses, amounts

**Acceptance Criteria**:
- [x] All 29 Phase 2 test cases executed
- [x] Test results documented
- [x] Bugs logged for any failures
- [x] Pass rate > 90% (26/29 tests pass)
- [x] Critical bugs escalated immediately

**Definition of Done**:
- Test execution report generated
- Bugs logged and assigned
- Test cases updated

---

#### QA-204: Phase 2 Automated E2E Tests (3.00 days)
**Owner**: QA Engineer 2
**Priority**: P0 - Critical
**Dependencies**: Cypress setup (Sprint N01)

**Description**:
Create automated E2E tests for Phase 2 features using Cypress.

**Automated Test Suite**:

**File: `cypress/e2e/phase2-transaction-history.cy.js`**
```javascript
describe('Phase 2: Transaction History', () => {
  beforeEach(() => {
    cy.loginAsTestUser();
    cy.visit('/transactions');
  });

  it('should display transaction history with pagination', () => {
    // Verify transaction list displayed
    cy.get('[data-testid="transaction-list"]').should('be.visible');

    // Verify transactions displayed
    cy.get('[data-testid="transaction-card"]').should('have.length.greaterThan', 0);

    // Verify pagination controls
    cy.get('[data-testid="pagination"]').should('be.visible');
  });

  it('should navigate between pages', () => {
    // Click next page
    cy.get('[data-testid="next-page-button"]').click();

    // Verify page number changed
    cy.get('[data-testid="current-page"]').should('contain', '2');

    // Click previous page
    cy.get('[data-testid="prev-page-button"]').click();

    // Verify page number changed back
    cy.get('[data-testid="current-page"]').should('contain', '1');
  });

  it('should sort transactions by date', () => {
    // Select sort option
    cy.get('[data-testid="sort-select"]').select('date-desc');

    // Verify transactions sorted (newest first)
    cy.get('[data-testid="transaction-card"]').first().should('contain', 'minutes ago');
  });

  it('should filter transactions by status', () => {
    // Select Confirmed filter
    cy.get('[data-testid="status-filter"]').select('Confirmed');

    // Verify only confirmed transactions displayed
    cy.get('[data-testid="transaction-status"]').each(($status) => {
      cy.wrap($status).should('contain', 'Confirmed');
    });
  });

  it('should search transactions by address', () => {
    const searchAddress = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb2';

    // Enter search query
    cy.get('[data-testid="search-input"]').type(searchAddress);

    // Verify filtered results
    cy.get('[data-testid="transaction-card"]').should('have.length.greaterThan', 0);
    cy.get('[data-testid="transaction-card"]').first().should('contain', searchAddress.substring(0, 10));
  });

  it('should clear all filters', () => {
    // Apply filters
    cy.get('[data-testid="status-filter"]').select('Confirmed');
    cy.get('[data-testid="search-input"]').type('0x123');

    // Verify active filters displayed
    cy.get('[data-testid="active-filters"]').should('be.visible');

    // Click clear all button
    cy.get('[data-testid="clear-filters-button"]').click();

    // Verify filters cleared
    cy.get('[data-testid="active-filters"]').should('not.exist');
  });

  it('should open transaction detail modal', () => {
    // Click on first transaction
    cy.get('[data-testid="transaction-card"]').first().click();

    // Verify modal opened
    cy.get('[data-testid="transaction-detail-modal"]').should('be.visible');

    // Verify transaction details displayed
    cy.get('[data-testid="transaction-amount"]').should('be.visible');
    cy.get('[data-testid="transaction-status"]').should('be.visible');
  });
});
```

**File: `cypress/e2e/phase2-ui-polish.cy.js`**
```javascript
describe('Phase 2: UI Polish', () => {
  beforeEach(() => {
    cy.loginAsTestUser();
  });

  it('should generate QR code for wallet address', () => {
    cy.visit('/wallet');

    // Click QR code button
    cy.get('[data-testid="qr-code-button"]').click();

    // Verify QR code modal displayed
    cy.get('[data-testid="qrcode-modal"]').should('be.visible');

    // Verify QR code canvas exists
    cy.get('canvas').should('be.visible');

    // Verify wallet address displayed
    cy.get('[data-testid="wallet-address"]').should('be.visible');
  });

  it('should copy address to clipboard from multiple locations', () => {
    cy.visit('/wallet');

    // Copy from wallet header
    cy.get('[data-testid="copy-address-button"]').click();
    cy.get('.toast-success').should('contain', 'copied');

    // Navigate to transaction detail
    cy.visit('/transactions');
    cy.get('[data-testid="transaction-card"]').first().click();

    // Copy from transaction detail modal
    cy.get('[data-testid="copy-from-address-button"]').click();
    cy.get('.toast-success').should('contain', 'copied');
  });

  it('should display loading skeletons', () => {
    // Intercept API call to delay response
    cy.intercept('GET', '/api/transactions/history*', (req) => {
      req.reply((res) => {
        res.delay = 2000;  // 2 second delay
      });
    });

    cy.visit('/transactions');

    // Verify loading skeleton displayed
    cy.get('[data-testid="transaction-skeleton"]').should('be.visible');

    // Wait for data to load
    cy.get('[data-testid="transaction-card"]', { timeout: 5000 }).should('be.visible');

    // Skeleton should be gone
    cy.get('[data-testid="transaction-skeleton"]').should('not.exist');
  });

  it('should handle API errors with retry', () => {
    // Intercept API call to return error
    cy.intercept('GET', '/api/transactions/history*', {
      statusCode: 500,
      body: { message: 'Internal server error' }
    }).as('apiError');

    cy.visit('/transactions');

    // Wait for error
    cy.wait('@apiError');

    // Verify error message displayed
    cy.get('[data-testid="error-message"]').should('be.visible');

    // Verify retry button displayed
    cy.get('[data-testid="retry-button"]').should('be.visible');

    // Mock successful response for retry
    cy.intercept('GET', '/api/transactions/history*', {
      fixture: 'transactions.json'
    }).as('apiSuccess');

    // Click retry button
    cy.get('[data-testid="retry-button"]').click();

    // Verify data loaded
    cy.wait('@apiSuccess');
    cy.get('[data-testid="transaction-card"]').should('be.visible');
  });

  it('should be responsive on mobile', () => {
    cy.viewport('iphone-x');
    cy.visit('/wallet');

    // Verify mobile layout
    cy.get('[data-testid="mobile-menu-button"]').should('be.visible');

    // Verify components stack vertically
    cy.get('[data-testid="balance-card"]').should('be.visible');
    cy.get('[data-testid="quick-actions"]').should('be.visible');
  });

  it('should be responsive on tablet', () => {
    cy.viewport('ipad-2');
    cy.visit('/transactions');

    // Verify tablet layout
    cy.get('[data-testid="transaction-list"]').should('be.visible');

    // Verify 2-column grid on tablet
    cy.get('[data-testid="transaction-card"]').should('have.css', 'grid-column');
  });
});
```

**Acceptance Criteria**:
- [x] 15+ automated Cypress tests created
- [x] Tests cover Phase 2 features
- [x] Tests run in CI/CD pipeline
- [x] Tests pass consistently (>95% pass rate)
- [x] Test reports generated

**Definition of Done**:
- Cypress tests passing in CI/CD
- Test code reviewed
- Documentation updated

---

### Epic 3.3: Specialized Testing (9.00 days)

#### QA-205: Performance Testing (100+ users) (2.00 days)
**Owner**: QA Lead
**Priority**: P1 - High
**Dependencies**: K6 setup (Sprint N01), Backend APIs deployed

**Description**:
Execute load testing with 100+ concurrent users using Grafana K6.

**K6 Test Script**:
```javascript
// k6/load-test-phase2.js
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('errors');

export const options = {
  stages: [
    { duration: '2m', target: 20 },   // Ramp up to 20 users
    { duration: '5m', target: 50 },   // Ramp up to 50 users
    { duration: '5m', target: 100 },  // Ramp up to 100 users
    { duration: '5m', target: 100 },  // Stay at 100 users
    { duration: '2m', target: 0 },    // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<1000'],  // 95% of requests < 1s
    http_req_failed: ['rate<0.01'],     // Error rate < 1%
    errors: ['rate<0.01'],
  },
};

const BASE_URL = __ENV.API_BASE_URL || 'http://localhost:5000';

export default function () {
  // 1. Get transaction history
  const historyRes = http.get(
    `${BASE_URL}/api/transactions/history?page=1&pageSize=20`,
    {
      headers: {
        'Authorization': `Bearer ${__ENV.API_TOKEN}`,
      },
    }
  );

  check(historyRes, {
    'history status 200': (r) => r.status === 200,
    'history response time < 1s': (r) => r.timings.duration < 1000,
  }) || errorRate.add(1);

  sleep(1);

  // 2. Get transaction details
  if (historyRes.json('transactions') && historyRes.json('transactions').length > 0) {
    const transactionId = historyRes.json('transactions')[0].id;

    const detailRes = http.get(
      `${BASE_URL}/api/transactions/${transactionId}/details`,
      {
        headers: {
          'Authorization': `Bearer ${__ENV.API_TOKEN}`,
        },
      }
    );

    check(detailRes, {
      'detail status 200': (r) => r.status === 200,
      'detail response time < 500ms': (r) => r.timings.duration < 500,
    }) || errorRate.add(1);
  }

  sleep(2);

  // 3. Get wallet balance
  const balanceRes = http.get(
    `${BASE_URL}/api/wallet/${__ENV.WALLET_ADDRESS}/balance`,
    {
      headers: {
        'Authorization': `Bearer ${__ENV.API_TOKEN}`,
      },
    }
  );

  check(balanceRes, {
    'balance status 200': (r) => r.status === 200,
    'balance response time < 500ms': (r) => r.timings.duration < 500,
  }) || errorRate.add(1);

  sleep(1);
}
```

**Performance Targets**:
| Metric | Target | Measurement |
|--------|--------|-------------|
| Response Time (P95) | < 1s | K6 `http_req_duration` |
| Error Rate | < 1% | K6 `http_req_failed` |
| Concurrent Users | 100+ | K6 `stages` |
| Transactions/sec | 50+ | K6 `http_reqs` |
| Database Queries/sec | 200+ | PostgreSQL monitoring |

**Acceptance Criteria**:
- [x] Load test simulates 100+ concurrent users
- [x] All performance targets met
- [x] No critical errors during load test
- [x] Performance report generated
- [x] Bottlenecks identified and documented

**Definition of Done**:
- Load test results reviewed with team
- Performance optimizations recommended
- Report published

---

#### QA-206: Security Testing (OWASP Top 10) (2.00 days)
**Owner**: QA Lead
**Priority**: P1 - High
**Dependencies**: Application deployed

**Description**:
Execute security testing to validate OWASP Top 10 compliance.

**Security Test Checklist**:

**1. Injection (SQL, NoSQL, Command)**:
- [ ] Test SQL injection in transaction history filters
- [ ] Test NoSQL injection in search queries
- [ ] Verify parameterized queries used
- [ ] Test input sanitization

**2. Broken Authentication**:
- [ ] Test session timeout (should expire after inactivity)
- [ ] Test JWT token expiration
- [ ] Verify passkey implementation security
- [ ] Test logout functionality (token invalidation)

**3. Sensitive Data Exposure**:
- [ ] Verify HTTPS enforced (production)
- [ ] Test wallet address encryption (not applicable - public)
- [ ] Verify JWT tokens stored securely (not in plain text)
- [ ] Test API does not expose sensitive user data

**4. XML External Entities (XXE)**:
- [ ] Not applicable (no XML parsing)

**5. Broken Access Control**:
- [ ] Test unauthorized access to other users' wallets
- [ ] Test unauthorized access to other users' transactions
- [ ] Verify JWT authorization checks on all endpoints
- [ ] Test role-based access control (if applicable)

**6. Security Misconfiguration**:
- [ ] Verify CORS configured correctly
- [ ] Test default credentials disabled
- [ ] Verify error messages do not expose system info
- [ ] Test security headers (CSP, X-Frame-Options, etc.)

**7. Cross-Site Scripting (XSS)**:
- [ ] Test XSS in transaction search
- [ ] Test XSS in username input
- [ ] Verify React escapes user input by default
- [ ] Test stored XSS in transaction notes (if applicable)

**8. Insecure Deserialization**:
- [ ] Test JWT token tampering
- [ ] Verify signature validation on JWT tokens

**9. Using Components with Known Vulnerabilities**:
- [ ] Run `npm audit` on frontend dependencies
- [ ] Run `dotnet list package --vulnerable` on backend
- [ ] Verify no critical vulnerabilities

**10. Insufficient Logging & Monitoring**:
- [ ] Verify failed login attempts logged
- [ ] Verify transaction events logged
- [ ] Verify correlation IDs used for tracing
- [ ] Test log injection attacks

**Security Tools**:
- **OWASP ZAP**: Automated security scanning
- **npm audit**: Frontend dependency vulnerability scan
- **Trivy**: Container vulnerability scanning
- **Manual testing**: Postman for API security tests

**Acceptance Criteria**:
- [x] All OWASP Top 10 categories tested
- [x] Security scan report generated
- [x] Critical vulnerabilities (if any) escalated immediately
- [x] No high-risk vulnerabilities remaining

**Definition of Done**:
- Security report reviewed with team
- Vulnerabilities logged as bugs
- Security recommendations documented

---

#### QA-207: Accessibility Testing (WCAG 2.1 AA) (2.00 days)
**Owner**: QA Engineer 1
**Priority**: P1 - High
**Dependencies**: Frontend deployed

**Description**:
Execute accessibility testing to achieve WCAG 2.1 AA compliance.

**Accessibility Test Checklist**:

**1. Perceivable**:
- [ ] All images have alt text
- [ ] Color contrast ratio ≥ 4.5:1 (normal text)
- [ ] Color contrast ratio ≥ 3:1 (large text)
- [ ] Content is not conveyed by color alone
- [ ] Text can be resized up to 200%

**2. Operable**:
- [ ] All functionality available via keyboard
- [ ] Keyboard focus visible
- [ ] No keyboard traps
- [ ] Skip navigation link present
- [ ] Page titles descriptive

**3. Understandable**:
- [ ] Language of page declared (lang attribute)
- [ ] Form labels associated with inputs
- [ ] Error messages clear and helpful
- [ ] Consistent navigation across pages

**4. Robust**:
- [ ] Valid HTML (no errors)
- [ ] ARIA labels used correctly
- [ ] Compatible with screen readers (NVDA, JAWS)

**Testing Tools**:
- **Lighthouse**: Automated accessibility audit
- **axe DevTools**: Browser extension for accessibility testing
- **NVDA**: Screen reader testing (Windows)
- **WAVE**: Web accessibility evaluation tool
- **Keyboard navigation**: Manual testing

**Test Pages**:
- [ ] Home/Landing page
- [ ] Login page
- [ ] Register page
- [ ] Wallet dashboard
- [ ] Transfer page
- [ ] Transaction history page
- [ ] Transaction detail modal

**Lighthouse Accessibility Score Target**: > 90

**Acceptance Criteria**:
- [x] Lighthouse accessibility score > 90
- [x] All pages keyboard accessible
- [x] Screen reader compatible
- [x] Color contrast meets WCAG AA
- [x] Accessibility audit report generated

**Definition of Done**:
- Accessibility issues logged as bugs
- Recommendations provided to frontend team
- Report published

---

#### QA-208: Regression Testing (Sprint N01) (2.00 days)
**Owner**: QA Engineer 2
**Priority**: P1 - High
**Dependencies**: Sprint N01 features

**Description**:
Execute regression testing to ensure Sprint N01 features still work after Sprint N02 changes.

**Regression Test Suite**:

**1. Sprint N01 Features** (from `SPRINT_N01_PROGRESS.md`):
- [ ] Docker Compose starts all services
- [ ] PostgreSQL database accessible
- [ ] API Gateway (YARP) routes requests correctly
- [ ] Health check endpoints work
- [ ] DocFx documentation site loads
- [ ] Passkey registration works
- [ ] Passkey login works
- [ ] Wallet creation works
- [ ] Balance display works
- [ ] USDC transfer works (basic)
- [ ] Transaction status tracking works

**2. Integration Points**:
- [ ] Frontend → Gateway → API communication
- [ ] API → PostgreSQL queries
- [ ] API → Circle SDK integration
- [ ] Background worker runs without errors

**3. Data Integrity**:
- [ ] Old transactions still display correctly
- [ ] Old wallets still accessible
- [ ] User sessions still valid

**Automated Regression Tests**:
- Re-run all Sprint N01 E2E tests
- Verify pass rate > 95%

**Acceptance Criteria**:
- [x] All Sprint N01 features still functional
- [x] No regressions introduced by Sprint N02 changes
- [x] Automated regression tests pass
- [x] Regression test report generated

**Definition of Done**:
- Regression issues logged as bugs
- Pass rate > 95%
- Report reviewed with team

---

#### QA-209: Bug Triage & Resolution Support (1.00 day)
**Owner**: QA Lead
**Priority**: P0 - Critical
**Dependencies**: Ongoing throughout sprint

**Description**:
Provide ongoing bug triage and resolution support throughout Sprint N02.

**Activities**:
- Daily bug triage meeting (15 minutes)
- Prioritize bugs by severity and impact
- Assign bugs to appropriate developers
- Verify bug fixes
- Retest closed bugs
- Track bug metrics

**Bug Severity Definitions** (from Sprint N01):

| Severity | Definition | Examples | Response Time |
|----------|------------|----------|---------------|
| **Critical** | System crash, data loss, security vulnerability | Auth completely broken, transfers fail 100% | Immediate (< 1 hour) |
| **High** | Major feature degradation | Transaction history not loading, filters broken | < 4 hours |
| **Medium** | Minor feature issues with workarounds | Sorting incorrect, copy button doesn't work | < 1 day |
| **Low** | Cosmetic issues | Text alignment off, icon size wrong | < 1 week |

**Bug Metrics to Track**:
- Total bugs logged
- Critical bugs (target: 0)
- High bugs (target: < 5)
- Bug resolution time
- Reopen rate

**Acceptance Criteria**:
- [x] Daily bug triage meetings held
- [x] All bugs prioritized and assigned
- [x] Critical bugs resolved within 1 hour
- [x] High bugs resolved within 4 hours
- [x] Bug metrics tracked and reported

**Definition of Done**:
- Bug metrics reviewed at sprint retrospective
- Bug resolution process improvements identified

---

## Daily Milestone Plan

### Days 1-2 (Sprint Start)
**Focus**: Phase 1 functional testing

**Tasks**:
- QA-201: Phase 1 functional testing (started)
- QA-202: Phase 1 E2E automation (started)
- Test environment validation

**Deliverable**: Phase 1 test execution 50% complete

---

### Days 3-4
**Focus**: Phase 1 automation and Phase 2 testing begins

**Tasks**:
- QA-201: Phase 1 functional testing (completed)
- QA-202: Phase 1 E2E automation (continued)
- QA-203: Phase 2 functional testing (started)

**Deliverable**: Phase 1 testing complete, Phase 2 testing started

---

### Days 5-6 (Mid-Sprint)
**Focus**: Phase 2 testing and automation

**Tasks**:
- QA-202: Phase 1 E2E automation (completed)
- QA-203: Phase 2 functional testing (continued)
- QA-204: Phase 2 E2E automation (started)

**Checkpoint Meeting**: Demo test results and coverage

**Deliverable**: Phase 1 automation complete, Phase 2 testing ongoing

---

### Days 7-8
**Focus**: Specialized testing

**Tasks**:
- QA-203: Phase 2 functional testing (completed)
- QA-204: Phase 2 E2E automation (continued)
- QA-205: Performance testing (started)
- QA-206: Security testing (started)

**Deliverable**: Phase 2 testing complete, specialized testing started

---

### Days 9-10 (Sprint End)
**Focus**: Final testing and reporting

**Tasks**:
- QA-204: Phase 2 E2E automation (completed)
- QA-205: Performance testing (completed)
- QA-206: Security testing (completed)
- QA-207: Accessibility testing
- QA-208: Regression testing
- QA-209: Bug triage and final verification
- Test report generation

**Sprint Review**: Present test results, coverage, and quality metrics

**Deliverable**: Comprehensive test coverage, quality report

---

## Success Criteria

### Functional Success Metrics

- [ ] Phase 1 test coverage: 24/24 test cases executed
- [ ] Phase 2 test coverage: 29/29 test cases executed
- [ ] Automated E2E tests: 27+ tests created and passing
- [ ] Performance testing: 100+ concurrent users simulated
- [ ] Security testing: OWASP Top 10 compliance verified
- [ ] Accessibility testing: Lighthouse score > 90
- [ ] Regression testing: Sprint N01 features validated

### Quality Gates

- [ ] Unit test coverage > 80%
- [ ] E2E test pass rate > 95%
- [ ] Performance targets met (P95 < 1s)
- [ ] Zero Critical bugs
- [ ] < 5 High bugs at sprint end
- [ ] Accessibility score > 90
- [ ] Security scan passed (no high-risk vulnerabilities)

---

**Sprint N02 QA Plan Version**: 1.0
**Last Updated**: 2025-10-28
**Status**: Ready for Execution
**Next Steps**: Day 1 - Start QA-201 (Phase 1 Functional Testing)

---

**End of Sprint N02 QA Plan**
