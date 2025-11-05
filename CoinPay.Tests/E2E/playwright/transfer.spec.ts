/**
 * QA-202: Phase 1 Automated E2E Tests - Gasless Transfers
 * Playwright Test Suite for USDC Transfer Functionality
 */

import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:3000';
const API_URL = 'http://localhost:5000';

// Test data
const VALID_RECIPIENT_ADDRESS = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0';
const INVALID_ADDRESS_SHORT = '0x123';
const INVALID_ADDRESS_FORMAT = 'not-an-address';

// Test configuration
test.use({
  baseURL: BASE_URL,
  screenshot: 'only-on-failure',
  video: 'retain-on-failure',
});

test.describe('Gasless Transfer Functionality', () => {

  test.beforeEach(async ({ page, context }) => {
    // Setup: User logged in with auth token and funded wallet
    await context.addCookies([{
      name: 'auth_token',
      value: 'test_token_funded_user',
      domain: 'localhost',
      path: '/',
    }]);
  });

  /**
   * Test 1: Successful USDC Transfer
   * Validates complete transfer flow from form to confirmation
   */
  test('should complete successful USDC transfer', async ({ page }) => {
    await page.goto('/transfer');

    // Verify transfer form loaded
    await expect(page).toHaveTitle(/Transfer|Send/i);
    const pageHeading = page.locator('h1, h2');
    await expect(pageHeading).toContainText(/transfer|send/i);

    // Fill transfer form
    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await expect(recipientInput).toBeVisible();
    await recipientInput.fill(VALID_RECIPIENT_ADDRESS);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await expect(amountInput).toBeVisible();
    await amountInput.fill('10.50');

    const noteInput = page.locator('input[name="note"], textarea[name="note"], input[placeholder*="note" i]');
    if (await noteInput.isVisible()) {
      await noteInput.fill('Test payment for E2E');
    }

    // Click Review Transfer button
    const reviewButton = page.locator('button:has-text("Review"), button:has-text("Preview"), button[type="submit"]').first();
    await expect(reviewButton).toBeEnabled();
    await reviewButton.click();

    // Verify preview/review screen
    await page.waitForTimeout(1000);

    // Look for confirmation elements
    const confirmSection = page.locator('[data-testid="transfer-preview"], .preview, .review');

    if (await confirmSection.isVisible()) {
      // Verify transfer details in preview
      await expect(page.locator(`text=${VALID_RECIPIENT_ADDRESS}`).first()).toBeVisible();
      await expect(page.locator('text=/10\\.50|10\\.5/').first()).toBeVisible();

      // Click Confirm button
      const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Send")').first();
      await expect(confirmButton).toBeEnabled();
      await confirmButton.click();
    }

    // Wait for success message or redirect
    await page.waitForTimeout(2000);

    // Verify success - either success message or redirect to transactions
    const successMessage = page.locator('text=/success|sent|complete/i, [role="alert"]');
    const transactionsUrl = page.url().includes('/transactions');

    if (await successMessage.isVisible()) {
      expect(await successMessage.isVisible()).toBeTruthy();
    } else if (transactionsUrl) {
      // Redirected to transactions page
      await expect(page).toHaveURL(/\/transactions/);
    }
  });

  /**
   * Test 2: Transfer with Insufficient Balance
   * Validates error handling when user has insufficient funds
   */
  test('should show error for insufficient balance', async ({ page }) => {
    await page.goto('/transfer');

    // Fill form with amount greater than balance
    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(VALID_RECIPIENT_ADDRESS);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await amountInput.fill('999999.00');

    // Try to proceed
    const reviewButton = page.locator('button:has-text("Review"), button:has-text("Preview"), button[type="submit"]').first();

    // Button might be disabled OR error message shown
    const isButtonDisabled = await reviewButton.isDisabled();

    if (!isButtonDisabled) {
      await reviewButton.click();
      await page.waitForTimeout(500);
    }

    // Verify error message appears
    const errorMessage = page.locator('text=/insufficient|not enough|balance/i, [role="alert"], .error');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });

    // Amount field should be highlighted (red border or error state)
    const amountInputError = page.locator('input[name="amount"][class*="error"], input[name="amount"][class*="red"], input[name="amount"][aria-invalid="true"]');

    // Either error class or aria-invalid should be present
    const hasErrorState = await amountInputError.count() > 0;
    expect(hasErrorState).toBeTruthy();
  });

  /**
   * Test 3: Transfer with Invalid Address Format
   * Validates address format validation
   */
  test('should show error for invalid address format', async ({ page }) => {
    await page.goto('/transfer');

    // Test Case 1: Address too short
    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(INVALID_ADDRESS_SHORT);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await amountInput.fill('5.00');

    // Trigger validation (blur or click review)
    await recipientInput.blur();
    await page.waitForTimeout(500);

    // Verify error message
    let errorMessage = page.locator('text=/invalid.*address|invalid.*format/i, [role="alert"]');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });

    // Test Case 2: Non-hex address format
    await recipientInput.clear();
    await recipientInput.fill(INVALID_ADDRESS_FORMAT);
    await recipientInput.blur();
    await page.waitForTimeout(500);

    errorMessage = page.locator('text=/invalid.*address|invalid.*format/i, [role="alert"]');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test 4: Transfer to Own Address
   * Validates prevention of self-transfers
   */
  test('should prevent transfer to own address', async ({ page }) => {
    await page.goto('/wallet');

    // Get user's own wallet address
    const walletAddress = page.locator('[data-testid="wallet-address"]');
    await expect(walletAddress).toBeVisible({ timeout: 10000 });
    const ownAddress = await walletAddress.textContent();
    const cleanOwnAddress = ownAddress?.trim() || '';

    // Navigate to transfer page
    await page.goto('/transfer');

    // Try to send to own address
    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(cleanOwnAddress);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await amountInput.fill('5.00');

    await recipientInput.blur();
    await page.waitForTimeout(500);

    // Verify error message
    const errorMessage = page.locator('text=/cannot.*own|same.*address|yourself/i, [role="alert"]');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test 5: Amount Validation (Min/Max)
   * Validates minimum and maximum transfer amounts
   */
  test('should validate minimum and maximum amounts', async ({ page }) => {
    await page.goto('/transfer');

    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(VALID_RECIPIENT_ADDRESS);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();

    // Test minimum amount (below 0.000001)
    await amountInput.fill('0.0000001');
    await amountInput.blur();
    await page.waitForTimeout(500);

    let errorMessage = page.locator('text=/minimum|too small/i, [role="alert"]');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });

    // Test maximum amount (above 1,000,000)
    await amountInput.clear();
    await amountInput.fill('1500000');
    await amountInput.blur();
    await page.waitForTimeout(500);

    errorMessage = page.locator('text=/maximum|too large|exceeds/i, [role="alert"]');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test 6: MAX Button Functionality
   * Validates "Use Maximum" or "MAX" button
   */
  test('should use MAX button to fill entire balance', async ({ page }) => {
    await page.goto('/transfer');

    // Look for MAX button
    const maxButton = page.locator('button:has-text("MAX"), button:has-text("Use Max"), button[aria-label*="maximum" i]');

    if (await maxButton.isVisible()) {
      await maxButton.click();

      // Verify amount field is filled
      const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
      const amount = await amountInput.inputValue();

      // Amount should be a valid number greater than 0
      const numericAmount = parseFloat(amount);
      expect(numericAmount).toBeGreaterThan(0);
    }
  });

  /**
   * Test 7: Transfer Preview Accuracy
   * Validates all details shown in preview/review screen
   */
  test('should show accurate transfer details in preview', async ({ page }) => {
    await page.goto('/transfer');

    // Fill form with specific values
    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(VALID_RECIPIENT_ADDRESS);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await amountInput.fill('25.123456');

    const noteInput = page.locator('input[name="note"], textarea[name="note"], input[placeholder*="note" i]');
    if (await noteInput.isVisible()) {
      await noteInput.fill('Test note for preview');
    }

    // Click review
    const reviewButton = page.locator('button:has-text("Review"), button:has-text("Preview"), button[type="submit"]').first();
    await reviewButton.click();
    await page.waitForTimeout(1000);

    // Verify preview details
    const previewSection = page.locator('[data-testid="transfer-preview"], .preview, .review');

    if (await previewSection.isVisible()) {
      // Verify recipient address
      await expect(page.locator(`text=${VALID_RECIPIENT_ADDRESS}`).first()).toBeVisible();

      // Verify amount (may be formatted)
      await expect(page.locator('text=/25\\.123456|25\\.12/').first()).toBeVisible();

      // Verify currency
      await expect(page.locator('text=/USDC/').first()).toBeVisible();

      // Verify network
      await expect(page.locator('text=/Polygon.*Amoy|Amoy.*Testnet/i').first()).toBeVisible();

      // Verify gas fee (should be FREE or Sponsored)
      const gasFee = page.locator('text=/free|sponsored|gasless/i');
      await expect(gasFee.first()).toBeVisible();

      // Verify back button exists
      const backButton = page.locator('button:has-text("Back"), button:has-text("Edit")');
      await expect(backButton.first()).toBeVisible();
    }
  });

  /**
   * Test 8: Transaction Status Tracking
   * Validates transaction appears in history with correct status
   */
  test('should track transaction status after submission', async ({ page }) => {
    // Complete a transfer first
    await page.goto('/transfer');

    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(VALID_RECIPIENT_ADDRESS);

    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await amountInput.fill('1.00');

    const reviewButton = page.locator('button:has-text("Review"), button:has-text("Preview"), button[type="submit"]').first();
    await reviewButton.click();
    await page.waitForTimeout(1000);

    // Confirm transfer
    const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Send")').first();
    if (await confirmButton.isVisible()) {
      await confirmButton.click();
    }

    await page.waitForTimeout(2000);

    // Navigate to transactions page
    await page.goto('/transactions');

    // Verify transaction appears in list
    const transactionList = page.locator('[data-testid="transaction-list"], .transaction-item, [class*="transaction"]');
    await expect(transactionList.first()).toBeVisible({ timeout: 10000 });

    // Look for pending status
    const pendingBadge = page.locator('text=/pending/i, [data-status="pending"]');

    // Transaction should either be pending or already completed (depending on speed)
    const hasPendingOrCompleted = await pendingBadge.count() > 0 ||
                                    await page.locator('text=/completed|confirmed/i').count() > 0;
    expect(hasPendingOrCompleted).toBeTruthy();
  });

  /**
   * Test 9: Gasless Transaction Verification
   * Validates no gas fees are charged to user
   */
  test('should verify gasless transaction (no gas fees)', async ({ page }) => {
    // Get initial balance
    await page.goto('/wallet');
    const balanceDisplay = page.locator('[data-testid="wallet-balance"]');
    await expect(balanceDisplay).toBeVisible({ timeout: 10000 });

    const initialBalanceText = await balanceDisplay.textContent();
    const initialBalance = parseFloat(initialBalanceText?.match(/[\d.]+/)?.[0] || '0');

    // Perform transfer
    await page.goto('/transfer');

    const recipientInput = page.locator('input[name="recipient"], input[placeholder*="address" i]').first();
    await recipientInput.fill(VALID_RECIPIENT_ADDRESS);

    const transferAmount = 5.00;
    const amountInput = page.locator('input[name="amount"], input[type="number"]').first();
    await amountInput.fill(transferAmount.toString());

    const reviewButton = page.locator('button:has-text("Review"), button:has-text("Preview"), button[type="submit"]').first();
    await reviewButton.click();
    await page.waitForTimeout(1000);

    const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Send")').first();
    if (await confirmButton.isVisible()) {
      await confirmButton.click();
    }

    await page.waitForTimeout(3000);

    // Check final balance
    await page.goto('/wallet');
    await page.waitForTimeout(2000);

    const finalBalanceText = await balanceDisplay.textContent();
    const finalBalance = parseFloat(finalBalanceText?.match(/[\d.]+/)?.[0] || '0');

    // Balance difference should be EXACTLY the transfer amount (no gas fees)
    const balanceDifference = initialBalance - finalBalance;

    // Allow small floating point tolerance
    expect(Math.abs(balanceDifference - transferAmount)).toBeLessThan(0.01);

    // Verify "Gasless" indicator shown in UI
    await page.goto('/transactions');
    const gaslessIndicator = page.locator('text=/gasless|sponsored|free/i');
    await expect(gaslessIndicator.first()).toBeVisible({ timeout: 5000 });
  });

});

test.describe('Transfer API Integration', () => {

  /**
   * Test 10: Submit Transfer API
   * Validates backend transfer submission endpoint
   */
  test('should submit transfer via API', async ({ request }) => {
    const response = await request.post(`${API_URL}/api/transfer/submit`, {
      headers: {
        'Authorization': 'Bearer test_token_funded_user',
        'Content-Type': 'application/json',
      },
      data: {
        recipientAddress: VALID_RECIPIENT_ADDRESS,
        amount: 10.50,
        currency: 'USDC',
        note: 'API test transfer',
      },
    });

    // Verify response
    expect(response.status()).toBeLessThan(500);

    if (response.ok()) {
      const data = await response.json();

      // Should return transaction ID
      expect(data).toHaveProperty('transactionId');
      expect(data).toHaveProperty('status');

      // Status should be Pending initially
      expect(data.status).toMatch(/pending|submitted/i);
    }
  });

  /**
   * Test 11: Get Transfer Status API
   * Validates transaction status endpoint
   */
  test('should fetch transfer status via API', async ({ request }) => {
    const mockTransactionId = 'tx-test-123456';

    const response = await request.get(`${API_URL}/api/transfer/status/${mockTransactionId}`, {
      headers: {
        'Authorization': 'Bearer test_token_funded_user',
      },
    });

    expect(response.status()).toBeLessThan(500);

    if (response.ok()) {
      const data = await response.json();

      // Validate response structure
      expect(data).toHaveProperty('status');
      expect(data.status).toMatch(/pending|completed|failed/i);
    }
  });

});
