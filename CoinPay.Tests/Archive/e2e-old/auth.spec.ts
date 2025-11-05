import { test, expect } from '@playwright/test';

test.describe('Authentication', () => {
  test('should display login page', async ({ page }) => {
    await page.goto('/login');
    await expect(page).toHaveTitle(/CoinPay/);
    await expect(page.getByRole('heading', { name: /login/i })).toBeVisible();
  });

  test('should navigate to register page', async ({ page }) => {
    await page.goto('/login');
    await page.getByRole('link', { name: /register/i }).click();
    await expect(page).toHaveURL(/\/register/);
  });

  test('should perform dev login successfully', async ({ page }) => {
    await page.goto('/login');

    // Fill login form
    await page.getByLabel(/username/i).fill('testuser');

    // Submit form
    await page.getByRole('button', { name: /login/i }).click();

    // Should redirect to dashboard
    await expect(page).toHaveURL(/\/dashboard/);
  });

  test('should display user profile after login', async ({ page }) => {
    // Perform login
    await page.goto('/login');
    await page.getByLabel(/username/i).fill('testuser');
    await page.getByRole('button', { name: /login/i }).click();

    // Wait for dashboard
    await expect(page).toHaveURL(/\/dashboard/);

    // Check if user info is displayed
    await expect(page.getByText(/testuser/i)).toBeVisible();
  });

  test('should logout successfully', async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.getByLabel(/username/i).fill('testuser');
    await page.getByRole('button', { name: /login/i }).click();
    await expect(page).toHaveURL(/\/dashboard/);

    // Logout
    await page.getByRole('button', { name: /logout/i }).click();

    // Should redirect to login
    await expect(page).toHaveURL(/\/login/);
  });
});
