# CoinPay E2E Tests - Cypress (Phase 2)

**QA-204: Phase 2 Automated E2E Tests**

Comprehensive E2E test suite for CoinPay Phase 2 features using Cypress.

---

## 📋 Test Coverage

### Merchant Dashboard Tests (merchant-dashboard.cy.ts)
- ✅ Dashboard overview widgets (revenue, transactions, pending payments)
- ✅ Transaction monitoring and filtering
- ✅ Transaction details modal
- ✅ Refund processing
- ✅ Analytics dashboard with charts
- ✅ Transaction export (CSV, PDF)
- ✅ Payment link generation
- ✅ Merchant settings
- ✅ Webhook configuration

**Total: 15 test cases**

### Customer Dashboard Tests (customer-dashboard.cy.ts)
- ✅ Customer dashboard overview
- ✅ Payment history display
- ✅ Payment detail modal
- ✅ Receipt download
- ✅ Payment request creation
- ✅ Saved recipients management
- ✅ Notification preferences
- ✅ Transaction disputes
- ✅ Account security (passkeys, sessions, activity log)

**Total: 14 test cases**

### Multi-Currency Tests (multi-currency.cy.ts)
- ✅ Multi-currency wallet display
- ✅ Add/remove currencies
- ✅ Exchange rate display and refresh
- ✅ Currency selection in transfers
- ✅ Currency conversion calculator
- ✅ Currency-specific transaction filtering
- ✅ Exchange rate accuracy validation
- ✅ Currency management

**Total: 13 test cases**

**Grand Total: 42 Phase 2 E2E Test Cases**

---

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ installed
- CoinPay.Web frontend running on http://localhost:3000
- CoinPay.Api backend running on http://localhost:5000

### Installation

```bash
# Navigate to cypress directory
cd Testing/E2E/cypress

# Install dependencies
npm install
```

---

## 🧪 Running Tests

### Interactive Mode (Cypress UI)

```bash
# Open Cypress Test Runner
npm run cy:open
```

This launches the interactive Cypress UI where you can:
- Select which tests to run
- Watch tests execute in real-time
- Debug failed tests
- Take screenshots

### Headless Mode (CI/CD)

```bash
# Run all tests
npm run cy:run

# Run in specific browser
npm run cy:run:chrome
npm run cy:run:firefox
npm run cy:run:edge

# Run specific test suite
npm run cy:run:merchant
npm run cy:run:customer
npm run cy:run:currency
```

---

## 📊 Test Reports

### Mochawesome Reports

```bash
# Generate HTML report
npm run report
```

Reports are generated in `cypress/reports/`:
- `mochawesome.html` - Interactive HTML report
- `mochawesome.json` - JSON results for CI/CD

---

## 🏗️ Test Structure

### Configuration

**cypress.config.ts** - Main configuration file:
- Base URL: http://localhost:3000
- API URL: http://localhost:5000
- Viewport: 1280x720
- Timeouts: 10s default, 30s page load
- Retries: 2 in CI, 0 in dev
- Video: Enabled (compressed)
- Screenshots: On failure

### Test Files

```
cypress/
├── e2e/
│   ├── merchant-dashboard.cy.ts   # Merchant features
│   ├── customer-dashboard.cy.ts   # Customer features
│   └── multi-currency.cy.ts       # Multi-currency support
├── support/
│   └── e2e.ts                      # Custom commands
├── fixtures/                       # Test data
├── screenshots/                    # Test failure screenshots
└── videos/                         # Test run recordings
```

---

## 🎯 Test Patterns

### Session Management

```typescript
beforeEach(() => {
  cy.session('merchant-session', () => {
    cy.visit('/login');
    cy.get('input[name="email"]').type(merchantEmail);
    cy.get('button').contains(/login/i).click();
    cy.url().should('include', '/merchant/dashboard');
  });
  cy.visit('/merchant/dashboard');
});
```

### Data Test IDs

Tests use `data-testid` attributes for stable selectors:

```typescript
cy.get('[data-testid="transaction-card"]').first().click();
```

### Assertions

Clear, descriptive assertions:

```typescript
cy.get('[data-testid="amount"]')
  .should('be.visible')
  .and('contain.text', /\d+\.\d+.*USDC/);
```

---

## 🔧 Configuration

### Environment Variables

Set in `cypress.config.ts`:

```typescript
env: {
  apiUrl: 'http://localhost:5000',
  testMerchantEmail: 'merchant@test.com',
  testCustomerEmail: 'customer@test.com',
}
```

Access in tests:

```typescript
const apiUrl = Cypress.env('apiUrl');
```

### Browser Support

- ✅ Chrome (recommended)
- ✅ Firefox
- ✅ Edge
- ✅ Electron (headless)

---

## 🐛 Debugging

### Visual Debugging

```bash
# Open Cypress UI and select test
npm run cy:open
```

Features:
- Time-travel debugging
- DOM snapshots
- Console logs
- Network requests
- Screenshot comparison

### Screenshots

Automatically captured on failure:
- Location: `cypress/screenshots/`
- Format: PNG
- Naming: `{test-name}--{timestamp}.png`

### Videos

Recorded for all test runs:
- Location: `cypress/videos/`
- Format: MP4
- Compression: 32 (configurable)

---

## 📈 CI/CD Integration

### GitHub Actions

```yaml
- name: Run Cypress E2E Tests
  run: |
    cd Testing/E2E/cypress
    npm install
    npm run cy:run

- name: Upload test results
  if: always()
  uses: actions/upload-artifact@v3
  with:
    name: cypress-results
    path: |
      Testing/E2E/cypress/screenshots
      Testing/E2E/cypress/videos
      Testing/E2E/cypress/reports
```

### Test Artifacts

- Screenshots (failures only)
- Videos (all runs)
- Mochawesome reports
- JUnit XML (for test runners)

---

## 🎨 Best Practices

### Selector Strategy

1. **Data Test IDs** (preferred):
   ```typescript
   cy.get('[data-testid="submit-button"]')
   ```

2. **Semantic selectors**:
   ```typescript
   cy.get('button').contains(/submit|save/i)
   ```

3. **CSS selectors** (last resort):
   ```typescript
   cy.get('.btn-primary')
   ```

### Waiting Strategy

Cypress auto-waits, but use explicit waits when needed:

```typescript
// Wait for element
cy.get('[data-testid="modal"]').should('be.visible');

// Wait for URL
cy.url().should('include', '/dashboard');

// Wait for API (if needed)
cy.wait('@apiCall');
```

### Clean Up

```typescript
// Clean screenshots/videos before new run
npm run clean
npm run cy:run
```

---

## 📦 Test Data

### Fixtures

Store test data in `cypress/fixtures/`:

```json
// cypress/fixtures/merchant.json
{
  "email": "merchant@test.com",
  "businessName": "Test Merchant"
}
```

Use in tests:

```typescript
cy.fixture('merchant').then((merchant) => {
  cy.get('input[name="email"]').type(merchant.email);
});
```

---

## 🆘 Troubleshooting

### Tests Failing

**Issue**: Element not found
**Solution**: Check selector, add wait, verify element exists

**Issue**: Timeout errors
**Solution**: Increase timeout in config or specific command

**Issue**: Flaky tests
**Solution**:
- Add proper waits
- Use `should()` assertions
- Enable retries in config

### Performance

**Issue**: Tests running slow
**Solution**:
- Disable video for faster runs
- Use `cy.session()` for login
- Mock API calls when appropriate

### Installation Issues

**Issue**: Cypress binary not found
**Solution**:
```bash
npx cypress install --force
```

---

## 📚 Resources

- [Cypress Documentation](https://docs.cypress.io/)
- [Cypress Best Practices](https://docs.cypress.io/guides/references/best-practices)
- [Cypress API](https://docs.cypress.io/api/table-of-contents)

---

## 👥 Contributing

### Adding New Tests

1. Create new test file in `cypress/e2e/`
2. Follow existing test patterns
3. Use data-testid selectors
4. Add descriptive test names
5. Update this README

### Test Naming Convention

```typescript
describe('Feature Name', () => {
  it('should <action> <expected result>', () => {
    // Test implementation
  });
});
```

Examples:
- `should display merchant dashboard`
- `should filter transactions by status`
- `should export transactions to CSV`

---

## 📄 License

MIT License - See LICENSE file for details

---

## 📞 Support

For issues or questions:
- Create an issue in the project repository
- Contact the QA team
- Review Cypress documentation

---

**Last Updated**: 2025-10-29
**Version**: 1.0.0
**Sprint**: N02 - QA Phase 2 (Optional)
**Test Count**: 42 E2E tests
