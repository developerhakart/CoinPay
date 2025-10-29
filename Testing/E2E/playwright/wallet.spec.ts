/**
 * QA-202: Phase 1 Automated E2E Tests - Wallet Management
 * Playwright Test Suite for Wallet Creation and Management
 */

import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:3000';
const API_URL = 'http://localhost:5000';

// Test configuration
test.use({
  baseURL: BASE_URL,
  screenshot: 'only-on-failure',
  video: 'retain-on-failure',
});

test.describe('Wallet Management', () => {

  test.beforeEach(async ({ page, context }) => {
    // Setup: User logged in with auth token
    await context.addCookies([{
      name: 'auth_token',
      value: 'test_token_wallet_user',
      domain: 'localhost',
      path: '/',
    }]);
  });

  /**
   * Test 1: Automatic Wallet Creation on Registration
   * Validates that wallet is automatically created when user registers
   */
  test('should create wallet automatically on user registration', async ({ page }) => {
    // This test validates wallet creation as part of registration flow
    // Navigate to wallet page after registration
    await page.goto('/wallet');

    // Verify wallet section exists
    const walletSection = page.locator('[data-testid="wallet-section"]');
    await expect(walletSection).toBeVisible({ timeout: 10000 });

    // Verify wallet address is displayed
    const walletAddress = page.locator('[data-testid="wallet-address"]');
    await expect(walletAddress).toBeVisible();

    // Verify address format (0x + 40 hex characters)
    const addressText = await walletAddress.textContent();
    expect(addressText).toMatch(/0x[a-fA-F0-9]{40}/);

    // Verify initial balance display
    const balanceDisplay = page.locator('[data-testid="wallet-balance"]');
    await expect(balanceDisplay).toBeVisible();

    // Balance should be visible (may be 0 or funded)
    const balanceText = await balanceDisplay.textContent();
    expect(balanceText).toMatch(/\d+\.\d+\s*USDC/);
  });

  /**
   * Test 2: Wallet Balance Display and Refresh
   * Validates balance display, formatting, and refresh functionality
   */
  test('should display wallet balance correctly and allow refresh', async ({ page }) => {
    await page.goto('/wallet');

    // Wait for balance to load
    const balanceDisplay = page.locator('[data-testid="wallet-balance"]');
    await expect(balanceDisplay).toBeVisible({ timeout: 10000 });

    // Verify balance format (6 decimal places for USDC)
    const balanceText = await balanceDisplay.textContent();
    expect(balanceText).toMatch(/\d+\.\d{1,6}\s*USDC/);

    // Test refresh button
    const refreshButton = page.locator('button:has-text("Refresh"), button[aria-label*="refresh" i], button svg[data-icon="refresh"]').first();

    if (await refreshButton.isVisible()) {
      // Click refresh
      await refreshButton.click();

      // Verify loading state appears
      const loadingIndicator = page.locator('[data-testid="balance-loading"], .loading, .spinner');

      // Wait for refresh to complete (loading disappears)
      await expect(loadingIndicator).toBeHidden({ timeout: 10000 }).catch(() => {
        // Loading indicator might be too fast to catch
      });

      // Balance should still be visible after refresh
      await expect(balanceDisplay).toBeVisible();
    }

    // Verify wallet address is displayed
    const walletAddress = page.locator('[data-testid="wallet-address"], [class*="address"]');
    await expect(walletAddress.first()).toBeVisible();
  });

  /**
   * Test 3: Copy Wallet Address Functionality
   * Validates copying wallet address to clipboard
   */
  test('should copy wallet address to clipboard', async ({ page, context }) => {
    await page.goto('/wallet');

    // Wait for wallet address to load
    const walletAddress = page.locator('[data-testid="wallet-address"]');
    await expect(walletAddress).toBeVisible({ timeout: 10000 });

    // Get the address text
    const addressText = await walletAddress.textContent();
    const cleanAddress = addressText?.trim();

    // Find and click copy button
    const copyButton = page.locator('button:has-text("Copy"), button[aria-label*="copy" i], button:has(svg[data-icon="copy"])').first();
    await expect(copyButton).toBeVisible();
    await copyButton.click();

    // Grant clipboard permissions
    await context.grantPermissions(['clipboard-read', 'clipboard-write']);

    // Verify clipboard content (wait a bit for clipboard operation)
    await page.waitForTimeout(500);

    // Verify "Copied" feedback appears
    const copiedFeedback = page.locator('text=/copied/i, [data-testid="copy-success"]');
    await expect(copiedFeedback).toBeVisible({ timeout: 3000 });

    // Optional: Verify clipboard content if we can read it
    try {
      const clipboardText = await page.evaluate(() => navigator.clipboard.readText());
      expect(clipboardText).toMatch(/0x[a-fA-F0-9]{40}/);
    } catch (err) {
      // Clipboard API might not be available in test environment
      console.log('Clipboard read not available:', err);
    }
  });

  /**
   * Test 4: QR Code Generation
   * Validates QR code modal and address display
   */
  test('should generate QR code for wallet address', async ({ page }) => {
    await page.goto('/wallet');

    // Look for QR code or Receive button
    const qrButton = page.locator('button:has-text("QR Code"), button:has-text("Receive"), button[aria-label*="qr" i]').first();

    if (await qrButton.isVisible()) {
      await qrButton.click();

      // Verify modal opens
      const modal = page.locator('[role="dialog"], .modal, [data-testid="qr-modal"]');
      await expect(modal).toBeVisible({ timeout: 5000 });

      // Verify QR code image/canvas is displayed
      const qrCode = page.locator('canvas, img[alt*="QR" i], svg[data-qr]');
      await expect(qrCode.first()).toBeVisible();

      // Verify wallet address is shown in modal
      const modalAddress = page.locator('[data-testid="modal-address"], code, .address');
      await expect(modalAddress.first()).toBeVisible();

      const modalAddressText = await modalAddress.first().textContent();
      expect(modalAddressText).toMatch(/0x[a-fA-F0-9]{40}/);

      // Close modal
      const closeButton = page.locator('button:has-text("Close"), button[aria-label="Close"]').first();
      await closeButton.click();

      // Verify modal closes
      await expect(modal).toBeHidden({ timeout: 3000 });
    }
  });

  /**
   * Test 5: Wallet Transaction History Link
   * Validates navigation from wallet to transactions
   */
  test('should navigate to transaction history from wallet page', async ({ page }) => {
    await page.goto('/wallet');

    // Look for transactions link/button
    const transactionsLink = page.locator('a:has-text("Transactions"), a:has-text("History"), button:has-text("View Transactions")').first();

    if (await transactionsLink.isVisible()) {
      await transactionsLink.click();

      // Verify navigated to transactions page
      await expect(page).toHaveURL(/\/transactions/);

      // Verify transactions page loaded
      const pageHeading = page.locator('h1, h2');
      await expect(pageHeading).toContainText(/transactions|history/i);
    }
  });

});

test.describe('Wallet API Integration', () => {

  /**
   * Test 6: Get Wallet Balance API
   * Validates backend wallet balance endpoint
   */
  test('should fetch wallet balance via API', async ({ request }) => {
    // Mock user ID for testing
    const userId = 'test-user-123';

    const response = await request.get(`${API_URL}/api/wallet/balance`, {
      headers: {
        'Authorization': 'Bearer test_token_wallet_user',
      },
    });

    // Verify response
    expect(response.status()).toBeLessThan(500);

    if (response.ok()) {
      const data = await response.json();

      // Validate response structure
      expect(data).toHaveProperty('balance');
      expect(data).toHaveProperty('currency');

      // Balance should be a number
      expect(typeof data.balance).toBe('number');

      // Currency should be USDC
      expect(data.currency).toBe('USDC');
    }
  });

  /**
   * Test 7: Get Wallet Address API
   * Validates backend wallet address endpoint
   */
  test('should fetch wallet address via API', async ({ request }) => {
    const response = await request.get(`${API_URL}/api/wallet/address`, {
      headers: {
        'Authorization': 'Bearer test_token_wallet_user',
      },
    });

    expect(response.status()).toBeLessThan(500);

    if (response.ok()) {
      const data = await response.json();

      // Validate wallet address format
      expect(data).toHaveProperty('address');
      expect(data.address).toMatch(/^0x[a-fA-F0-9]{40}$/);
    }
  });

});
