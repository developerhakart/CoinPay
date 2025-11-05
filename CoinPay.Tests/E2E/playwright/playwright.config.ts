/**
 * Playwright Configuration for CoinPay E2E Tests
 * QA-202: Phase 1 Automated E2E Tests
 */

import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './',

  // Maximum time one test can run for
  timeout: 30 * 1000,

  // Test execution settings
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,

  // Reporter configuration
  reporter: [
    ['html', { outputFolder: '../reports/playwright-html' }],
    ['json', { outputFile: '../reports/playwright-results.json' }],
    ['junit', { outputFile: '../reports/playwright-junit.xml' }],
    ['list'],
  ],

  // Shared settings for all projects
  use: {
    // Base URL to use in actions like `await page.goto('/')`
    baseURL: 'http://localhost:3000',

    // Collect trace when retrying the failed test
    trace: 'on-first-retry',

    // Screenshot on failure
    screenshot: 'only-on-failure',

    // Video on failure
    video: 'retain-on-failure',

    // Browser viewport
    viewport: { width: 1280, height: 720 },

    // Ignore HTTPS errors
    ignoreHTTPSErrors: true,

    // Action timeout
    actionTimeout: 10 * 1000,

    // Navigation timeout
    navigationTimeout: 15 * 1000,
  },

  // Configure projects for major browsers
  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        // Enable WebAuthn (Passkey) support
        launchOptions: {
          args: [
            '--enable-features=WebAuthentication',
            '--enable-virtual-authenticators',
          ],
        },
      },
    },

    {
      name: 'firefox',
      use: {
        ...devices['Desktop Firefox'],
      },
    },

    {
      name: 'webkit',
      use: {
        ...devices['Desktop Safari'],
      },
    },

    // Mobile viewports
    {
      name: 'Mobile Chrome',
      use: {
        ...devices['Pixel 5'],
      },
    },

    {
      name: 'Mobile Safari',
      use: {
        ...devices['iPhone 12'],
      },
    },

    // Tablet viewports
    {
      name: 'iPad',
      use: {
        ...devices['iPad Pro'],
      },
    },
  ],

  // Run your local dev server before starting the tests
  webServer: {
    command: 'cd ../../CoinPay.Web && npm run dev',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000,
    stdout: 'ignore',
    stderr: 'pipe',
  },
});
