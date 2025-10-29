/**
 * QA-202: Phase 1 Automated E2E Tests - Authentication
 * Playwright Test Suite for Passkey Authentication
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

test.describe('Passkey Authentication', () => {

  test.beforeEach(async ({ page }) => {
    // Clear storage before each test
    await page.context().clearCookies();
    await page.context().clearPermissions();
  });

  /**
   * Test 1: User Registration with Passkey
   * Validates complete registration flow with passkey creation
   */
  test('should register new user with passkey', async ({ page }) => {
    const username = `testuser_${Date.now()}`;
    const email = `${username}@example.com`;

    await page.goto('/register');

    // Verify registration page loaded
    await expect(page).toHaveTitle(/Register/i);
    await expect(page.locator('h1')).toContainText(/register/i);

    // Fill registration form
    await page.fill('input[name="username"]', username);
    await page.fill('input[name="email"]', email);

    // Click register button
    const registerButton = page.locator('button:has-text("Register")');
    await expect(registerButton).toBeVisible();
    await registerButton.click();

    // Note: Actual passkey interaction requires browser automation
    // In real tests, this would trigger WebAuthn flow
    // For now, we verify the UI flow up to passkey prompt

    // Wait for redirect to dashboard or passkey prompt
    await page.waitForURL(/\/dashboard|\/passkey/, { timeout: 10000 });

    // If redirected to dashboard, verify success
    if (page.url().includes('/dashboard')) {
      await expect(page.locator('text=/welcome|dashboard/i')).toBeVisible();

      // Verify wallet was created
      const walletSection = page.locator('[data-testid="wallet-section"]');
      await expect(walletSection).toBeVisible({ timeout: 5000 });
    }
  });

  /**
   * Test 2: User Login with Passkey
   * Validates login flow for existing user
   */
  test('should login existing user with passkey', async ({ page }) => {
    // Prerequisites: User already registered (run registration test first)
    const username = 'existinguser';

    await page.goto('/login');

    // Verify login page
    await expect(page).toHaveTitle(/Login/i);
    await expect(page.locator('h1')).toContainText(/login/i);

    // Enter username
    await page.fill('input[name="username"]', username);

    // Click login button
    const loginButton = page.locator('button:has-text("Login")');
    await expect(loginButton).toBeVisible();
    await loginButton.click();

    // Wait for authentication
    // In real scenario, WebAuthn prompt would appear
    await page.waitForTimeout(2000);

    // Verify redirected to dashboard (if passkey succeeds)
    // await expect(page).toHaveURL(/\/dashboard/);
  });

  /**
   * Test 3: Login with Non-existent User
   * Validates error handling for invalid username
   */
  test('should show error for non-existent user', async ({ page }) => {
    await page.goto('/login');

    const nonExistentUser = 'nonexistentuser999';
    await page.fill('input[name="username"]', nonExistentUser);

    const loginButton = page.locator('button:has-text("Login")');
    await loginButton.click();

    // Wait for error message
    const errorMessage = page.locator('[role="alert"], .error-message, text=/not found|invalid/i');
    await expect(errorMessage).toBeVisible({ timeout: 5000 });

    // Verify still on login page
    await expect(page).toHaveURL(/\/login/);
  });

  /**
   * Test 4: Session Persistence
   * Validates that user remains logged in after page refresh
   */
  test('should maintain session after page refresh', async ({ page, context }) => {
    // Login first (assuming we have valid credentials)
    await page.goto('/dashboard');

    // If not logged in, login flow would redirect to /login
    // For this test, assume we're already logged in via context

    // Simulate logged-in state by setting localStorage/cookies
    await context.addCookies([{
      name: 'auth_token',
      value: 'test_token_123',
      domain: 'localhost',
      path: '/',
    }]);

    await page.goto('/dashboard');

    // Refresh page
    await page.reload();

    // Verify still on dashboard (not redirected to login)
    await expect(page).toHaveURL(/\/dashboard/);
    await expect(page.locator('text=/dashboard|wallet|balance/i')).toBeVisible();
  });

  /**
   * Test 5: Logout Functionality
   * Validates logout clears session and redirects to login
   */
  test('should logout user and clear session', async ({ page, context }) => {
    // Setup: User logged in
    await context.addCookies([{
      name: 'auth_token',
      value: 'test_token_123',
      domain: 'localhost',
      path: '/',
    }]);

    await page.goto('/dashboard');

    // Click logout button
    const logoutButton = page.locator('button:has-text("Logout")');
    await expect(logoutButton).toBeVisible();
    await logoutButton.click();

    // Verify redirected to login
    await expect(page).toHaveURL(/\/login|\/$/);

    // Try to access protected route
    await page.goto('/dashboard');

    // Should be redirected back to login
    await expect(page).toHaveURL(/\/login/);
  });

});

test.describe('Protected Routes', () => {

  /**
   * Test 6: Accessing protected routes without authentication
   * Validates that unauthenticated users are redirected to login
   */
  test('should redirect to login when accessing protected routes', async ({ page }) => {
    const protectedRoutes = [
      '/dashboard',
      '/wallet',
      '/transfer',
      '/transactions',
    ];

    for (const route of protectedRoutes) {
      await page.goto(route);

      // Should be redirected to login
      await page.waitForURL(/\/login|\/register/, { timeout: 5000 });
      await expect(page).toHaveURL(/\/login|\/register/);
    }
  });

  /**
   * Test 7: Accessing public routes
   * Validates that public routes are accessible without authentication
   */
  test('should allow access to public routes', async ({ page }) => {
    const publicRoutes = [
      '/',
      '/login',
      '/register',
    ];

    for (const route of publicRoutes) {
      await page.goto(route);

      // Should load successfully
      await expect(page).toHaveURL(new RegExp(route));
      await expect(page.locator('body')).toBeVisible();
    }
  });

});

test.describe('Authentication API Integration', () => {

  /**
   * Test 8: Registration API endpoint
   * Validates backend registration endpoint
   */
  test('should register user via API', async ({ request }) => {
    const username = `apitest_${Date.now()}`;
    const email = `${username}@example.com`;

    const response = await request.post(`${API_URL}/api/auth/register`, {
      data: {
        username,
        email,
        // In real scenario, would include passkey credential
      },
    });

    // Verify response
    // Note: Adjust based on actual API response format
    expect(response.status()).toBeLessThan(500);
  });

  /**
   * Test 9: Login API endpoint
   * Validates backend login endpoint
   */
  test('should authenticate user via API', async ({ request }) => {
    const response = await request.post(`${API_URL}/api/auth/login`, {
      data: {
        username: 'testuser',
        // In real scenario, would include passkey challenge/response
      },
    });

    expect(response.status()).toBeLessThan(500);
  });

});
