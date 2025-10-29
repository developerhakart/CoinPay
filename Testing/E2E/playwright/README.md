# CoinPay E2E Tests - Playwright

**QA-202: Phase 1 Automated E2E Tests**

Comprehensive end-to-end test suite for CoinPay application using Playwright.

---

## 📋 Test Coverage

### Authentication Tests (auth.spec.ts)
- ✅ User registration with passkey
- ✅ User login with passkey
- ✅ Login with non-existent user
- ✅ Session persistence after page refresh
- ✅ Logout functionality
- ✅ Protected route access control
- ✅ Public route accessibility
- ✅ Registration API endpoint
- ✅ Login API endpoint

**Total: 9 test cases**

### Wallet Management Tests (wallet.spec.ts)
- ✅ Automatic wallet creation on registration
- ✅ Wallet balance display and refresh
- ✅ Copy wallet address to clipboard
- ✅ QR code generation for wallet address
- ✅ Navigation to transaction history
- ✅ Get wallet balance API
- ✅ Get wallet address API

**Total: 7 test cases**

### Transfer Tests (transfer.spec.ts)
- ✅ Successful USDC transfer
- ✅ Transfer with insufficient balance
- ✅ Transfer with invalid address format
- ✅ Transfer to own address prevention
- ✅ Amount validation (min/max)
- ✅ MAX button functionality
- ✅ Transfer preview accuracy
- ✅ Transaction status tracking
- ✅ Gasless transaction verification
- ✅ Submit transfer API
- ✅ Get transfer status API

**Total: 11 test cases**

**Grand Total: 27 E2E Test Cases**

---

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ installed
- CoinPay.Web frontend running on http://localhost:3000
- CoinPay.Api backend running on http://localhost:5000

### Installation

```bash
# Navigate to playwright directory
cd Testing/E2E/playwright

# Install dependencies
npm install

# Install Playwright browsers
npm run install:browsers
```

---

## 🧪 Running Tests

### Run All Tests

```bash
npm test
```

### Run Tests by Suite

```bash
# Authentication tests only
npm run test:auth

# Wallet tests only
npm run test:wallet

# Transfer tests only
npm run test:transfer
```

### Run Tests by Browser

```bash
# Chromium only (fastest)
npm run test:chromium

# Firefox only
npm run test:firefox

# WebKit/Safari only
npm run test:webkit

# Mobile browsers
npm run test:mobile
```

### Interactive UI Mode

```bash
# Launch Playwright UI for interactive testing
npm run test:ui
```

### Debug Mode

```bash
# Run tests with debugger
npm run test:debug
```

### Headed Mode (See Browser)

```bash
# Run tests with browser visible
npm run test:headed
```

---

## 📊 Test Reports

### View HTML Report

```bash
npm run report
```

Reports are generated in `../reports/` directory:
- `playwright-html/` - Interactive HTML report
- `playwright-results.json` - JSON results for CI/CD
- `playwright-junit.xml` - JUnit format for test runners

---

## 🏗️ Test Configuration

### Browsers Tested

- ✅ Desktop Chrome
- ✅ Desktop Firefox
- ✅ Desktop Safari (WebKit)
- ✅ Mobile Chrome (Pixel 5)
- ✅ Mobile Safari (iPhone 12)
- ✅ iPad Pro

### Test Execution Settings

- **Timeout**: 30 seconds per test
- **Retries**: 2 retries in CI, 0 locally
- **Parallel**: Fully parallel execution
- **Screenshots**: Captured on failure
- **Videos**: Retained on failure
- **Traces**: Collected on first retry

### Viewport

- Desktop: 1280x720
- Mobile: Device-specific (configured per project)

---

## 🔧 Configuration Files

### `playwright.config.ts`

Main configuration file with:
- Browser projects (Chromium, Firefox, WebKit)
- Mobile device emulation
- Test reporters
- Web server integration
- Timeout and retry settings

### `package.json`

NPM scripts for:
- Running tests
- Installing browsers
- Viewing reports
- Running specific test suites

---

## 📝 Test Structure

Each test file follows this structure:

```typescript
import { test, expect } from '@playwright/test';

test.describe('Feature Name', () => {

  test.beforeEach(async ({ page, context }) => {
    // Setup: Login, cookies, permissions
  });

  test('should do something', async ({ page }) => {
    // Test implementation
    await page.goto('/route');
    await expect(element).toBeVisible();
  });

});
```

---

## 🎯 Best Practices

### Locator Strategy

1. **Data Test IDs** (preferred): `[data-testid="wallet-balance"]`
2. **Semantic roles**: `[role="alert"]`, `button`, `input[name="amount"]`
3. **Text content**: `text=/pattern/i`, `button:has-text("Submit")`
4. **CSS selectors**: Use as last resort

### Waiting Strategy

- ✅ Use `await expect(element).toBeVisible()`
- ✅ Use `page.waitForURL()` for navigation
- ⚠️ Avoid `page.waitForTimeout()` except for animations
- ✅ Rely on auto-waiting for actions

### Error Handling

- All tests include proper error assertions
- Failed tests capture screenshots and videos
- Traces available for debugging

---

## 🐛 Debugging Tips

### 1. Use UI Mode

```bash
npm run test:ui
```

Interactive mode with timeline, DOM snapshots, and step-by-step execution.

### 2. Use Debug Mode

```bash
npm run test:debug
```

Opens DevTools with breakpoints and console access.

### 3. Headed Mode

```bash
npm run test:headed
```

Watch tests run in real browser.

### 4. Screenshots & Videos

Check `test-results/` folder for:
- Screenshots on failure
- Video recordings
- Trace files

---

## 📦 CI/CD Integration

### GitHub Actions Example

```yaml
- name: Install dependencies
  run: |
    cd Testing/E2E/playwright
    npm install
    npm run install:browsers

- name: Run E2E tests
  run: |
    cd Testing/E2E/playwright
    npm test

- name: Upload test results
  if: always()
  uses: actions/upload-artifact@v3
  with:
    name: playwright-report
    path: Testing/reports/
```

---

## 🔐 Authentication Testing

### WebAuthn / Passkey Support

Tests include virtual authenticator support for passkey testing:

```typescript
launchOptions: {
  args: [
    '--enable-features=WebAuthentication',
    '--enable-virtual-authenticators',
  ],
}
```

### Mock Authentication

For tests requiring authenticated state:

```typescript
await context.addCookies([{
  name: 'auth_token',
  value: 'test_token_123',
  domain: 'localhost',
  path: '/',
}]);
```

---

## 📈 Test Metrics

### Current Status

- **Total Tests**: 27
- **Authentication**: 9 tests
- **Wallet**: 7 tests
- **Transfers**: 11 tests

### Coverage

- ✅ Authentication flows (100%)
- ✅ Wallet management (100%)
- ✅ Gasless transfers (100%)
- ✅ Error handling (100%)
- ✅ API integration (100%)

---

## 🆘 Troubleshooting

### Tests Fail with "Connection Refused"

**Issue**: Frontend or backend not running
**Solution**:
```bash
# Terminal 1: Start backend
cd CoinPay.Api
dotnet run

# Terminal 2: Start frontend
cd CoinPay.Web
npm run dev
```

### Browser Installation Fails

**Issue**: Missing system dependencies
**Solution**:
```bash
# Install with system dependencies
npx playwright install --with-deps chromium
```

### WebAuthn/Passkey Tests Fail

**Issue**: Virtual authenticator not enabled
**Solution**: Tests automatically enable WebAuthn features. If issues persist, run in Chromium only:
```bash
npm run test:chromium
```

### Timeout Errors

**Issue**: Tests timing out
**Solution**: Increase timeout in `playwright.config.ts`:
```typescript
timeout: 60 * 1000, // 60 seconds
```

---

## 📚 Resources

- [Playwright Documentation](https://playwright.dev/)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [WebAuthn Testing Guide](https://playwright.dev/docs/auth)
- [CI/CD Integration](https://playwright.dev/docs/ci)

---

## 👥 Contributing

### Adding New Tests

1. Create or modify test file in appropriate suite
2. Follow existing test structure and naming conventions
3. Use data-testid attributes for stable selectors
4. Add proper assertions and error handling
5. Update this README with new test coverage

### Test Naming Convention

```typescript
test('should <action> <expected result>', async ({ page }) => {
  // Test implementation
});
```

Examples:
- `should display wallet balance correctly`
- `should show error for insufficient balance`
- `should navigate to transaction history`

---

## 📄 License

MIT License - See LICENSE file for details

---

## 📞 Support

For issues or questions:
- Create an issue in the project repository
- Contact the QA team
- Review test reports in `../reports/`

---

**Last Updated**: 2025-10-29
**Version**: 1.0.0
**Sprint**: N02 - QA Phase 1
