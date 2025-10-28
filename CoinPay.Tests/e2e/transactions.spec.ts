import { test, expect } from '@playwright/test';

test.describe('Transactions', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login');
    await page.getByLabel(/username/i).fill('testuser');
    await page.getByRole('button', { name: /login/i }).click();
    await expect(page).toHaveURL(/\/dashboard/);
  });

  test('should display transactions page', async ({ page }) => {
    await page.goto('/transactions');
    await expect(page.getByRole('heading', { name: /transactions/i })).toBeVisible();
  });

  test('should filter transactions by status', async ({ page }) => {
    await page.goto('/transactions');

    // Click Pending filter
    await page.getByRole('button', { name: /pending/i }).click();

    // Should show only pending transactions
    await expect(page.getByText(/pending/i)).toBeVisible();
  });

  test('should create new transaction', async ({ page }) => {
    await page.goto('/transactions');

    // Open transaction form
    await page.getByRole('button', { name: /new transaction/i }).click();

    // Fill form
    await page.getByLabel(/amount/i).fill('100.50');
    await page.getByLabel(/sender name/i).fill('Alice');
    await page.getByLabel(/receiver name/i).fill('Bob');
    await page.getByLabel(/description/i).fill('Test payment');

    // Submit
    await page.getByRole('button', { name: /create transaction/i }).click();

    // Should see success message or new transaction in list
    await expect(page.getByText(/100.50/)).toBeVisible();
  });

  test('should display transaction details', async ({ page }) => {
    await page.goto('/transactions');

    // Wait for transactions to load
    await page.waitForSelector('[class*="transaction"]', { timeout: 5000 });

    // Transaction should be visible
    await expect(page.locator('[class*="transaction"]').first()).toBeVisible();
  });
});
