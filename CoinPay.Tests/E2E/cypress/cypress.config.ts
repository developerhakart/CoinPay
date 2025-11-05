/**
 * Cypress Configuration for CoinPay Phase 2 E2E Tests
 * QA-204: Phase 2 Automated E2E Tests
 */

import { defineConfig } from 'cypress';

export default defineConfig({
  e2e: {
    baseUrl: 'http://localhost:3000',
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    supportFile: 'cypress/support/e2e.ts',

    // Viewport settings
    viewportWidth: 1280,
    viewportHeight: 720,

    // Video and screenshot settings
    video: true,
    videoCompression: 32,
    screenshotOnRunFailure: true,
    screenshotsFolder: 'cypress/screenshots',
    videosFolder: 'cypress/videos',

    // Timeouts
    defaultCommandTimeout: 10000,
    requestTimeout: 10000,
    responseTimeout: 10000,
    pageLoadTimeout: 30000,

    // Retry configuration
    retries: {
      runMode: 2,
      openMode: 0,
    },

    // Environment variables
    env: {
      apiUrl: 'http://localhost:5000',
      testMerchantEmail: 'merchant@test.com',
      testCustomerEmail: 'customer@test.com',
    },

    setupNodeEvents(on, config) {
      // implement node event listeners here
      on('task', {
        log(message) {
          console.log(message);
          return null;
        },
      });

      return config;
    },
  },

  component: {
    devServer: {
      framework: 'react',
      bundler: 'vite',
    },
    specPattern: 'src/**/*.cy.{js,jsx,ts,tsx}',
  },
});
