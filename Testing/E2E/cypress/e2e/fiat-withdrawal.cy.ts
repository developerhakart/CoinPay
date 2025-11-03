/**
 * E2E Tests for Fiat Withdrawal Flow
 * Sprint N03 - Phase 3: Fiat Off-Ramp
 *
 * Test Coverage:
 * - Complete withdrawal wizard flow (happy path)
 * - Insufficient balance error handling
 * - Back navigation state preservation
 * - Exchange rate display and refresh
 * - Fee calculation display
 * - Payout confirmation
 */

describe('Fiat Withdrawal Flow', () => {
  beforeEach(() => {
    // Login with test user
    cy.login('testuser@test.com');

    // Mock API responses
    cy.intercept('GET', '/api/bank-account', {
      statusCode: 200,
      body: {
        bankAccounts: [
          {
            id: 'test-bank-account-id-1',
            accountHolderName: 'John Doe',
            lastFourDigits: '1234',
            accountType: 'checking',
            bankName: 'Wells Fargo',
            isPrimary: true,
            isVerified: true
          },
          {
            id: 'test-bank-account-id-2',
            accountHolderName: 'John Doe',
            lastFourDigits: '5678',
            accountType: 'savings',
            bankName: 'Chase Bank',
            isPrimary: false,
            isVerified: true
          }
        ],
        total: 2
      }
    }).as('getBankAccounts');

    cy.intercept('GET', '/api/rates/usdc-usd', {
      statusCode: 200,
      body: {
        rate: 0.9998,
        baseCurrency: 'USDC',
        quoteCurrency: 'USD',
        timestamp: new Date().toISOString(),
        validForSeconds: 30,
        source: 'Mock',
        isCached: false,
        expiresAt: new Date(Date.now() + 30000).toISOString(),
        isValid: true,
        secondsUntilExpiration: 30
      }
    }).as('getExchangeRate');

    cy.intercept('POST', '/api/payout/preview', {
      statusCode: 200,
      body: {
        usdcAmount: 100.00,
        exchangeRate: 0.9998,
        usdAmountBeforeFees: 99.98,
        conversionFeePercent: 1.5,
        conversionFeeAmount: 1.50,
        payoutFeeAmount: 1.00,
        totalFees: 2.50,
        netUsdAmount: 97.48,
        expiresAt: new Date(Date.now() + 30000).toISOString()
      }
    }).as('getPreview');

    // Visit withdrawal page
    cy.visit('/withdraw');
  });

  it('TC-313.1: Complete withdrawal flow (happy path)', () => {
    // Step 1: Enter amount
    cy.get('[data-testid="usdc-amount-input"]').should('be.visible');
    cy.get('[data-testid="usdc-amount-input"]').type('100');

    // Verify exchange rate is displayed
    cy.get('[data-testid="exchange-rate"]').should('contain', '0.9998');
    cy.get('[data-testid="exchange-rate-countdown"]').should('exist');

    // Verify preview calculation
    cy.get('[data-testid="usd-preview"]').should('contain', '99.98');

    cy.get('[data-testid="next-button"]').click();

    // Step 2: Select bank account
    cy.wait('@getBankAccounts');
    cy.get('[data-testid="bank-account-card"]').should('have.length', 2);

    // Verify primary account is marked
    cy.get('[data-testid="bank-account-card"]').first()
      .should('have.class', 'primary')
      .and('contain', 'Wells Fargo')
      .and('contain', '••• 1234');

    // Select first bank account
    cy.get('[data-testid="bank-account-card"]').first().click();
    cy.get('[data-testid="bank-account-card"]').first()
      .should('have.class', 'selected');

    cy.get('[data-testid="next-button"]').click();

    // Step 3: Review details
    cy.wait('@getPreview');
    cy.get('[data-testid="review-usdc-amount"]').should('contain', '100.00');
    cy.get('[data-testid="review-exchange-rate"]').should('contain', '0.9998');
    cy.get('[data-testid="review-usd-amount"]').should('contain', '99.98');
    cy.get('[data-testid="review-conversion-fee"]').should('contain', '1.50');
    cy.get('[data-testid="review-payout-fee"]').should('contain', '1.00');
    cy.get('[data-testid="review-total-fees"]').should('contain', '2.50');
    cy.get('[data-testid="review-net-amount"]').should('contain', '97.48');

    // Verify bank account details shown
    cy.get('[data-testid="review-bank-account"]')
      .should('contain', 'Wells Fargo')
      .and('contain', '••• 1234');

    // Mock payout initiation
    cy.intercept('POST', '/api/payout/initiate', {
      statusCode: 201,
      body: {
        id: 'payout-test-id-123',
        status: 'pending',
        usdcAmount: 100.00,
        netAmount: 97.48,
        gatewayTransactionId: 'gw-tx-12345',
        initiatedAt: new Date().toISOString(),
        estimatedArrival: new Date(Date.now() + 86400000 * 3).toISOString()
      }
    }).as('initiatePayout');

    cy.get('[data-testid="confirm-button"]').click();

    // Step 4: Confirmation
    cy.wait('@initiatePayout');
    cy.get('[data-testid="success-message"]').should('be.visible')
      .and('contain', 'Payout initiated successfully');
    cy.get('[data-testid="payout-id"]').should('contain', 'payout-test-id-123');
    cy.get('[data-testid="estimated-arrival"]').should('be.visible');
    cy.get('[data-testid="view-payouts-button"]').should('be.visible');
  });

  it('TC-313.2: Withdrawal with insufficient balance', () => {
    // Mock balance check failure
    cy.intercept('POST', '/api/payout/initiate', {
      statusCode: 400,
      body: {
        error: {
          code: 'INSUFFICIENT_BALANCE',
          message: 'Insufficient USDC balance. Available: 50.00 USDC'
        }
      }
    }).as('insufficientBalance');

    // Enter large amount
    cy.get('[data-testid="usdc-amount-input"]').type('10000');
    cy.get('[data-testid="next-button"]').click();

    // Select bank account
    cy.wait('@getBankAccounts');
    cy.get('[data-testid="bank-account-card"]').first().click();
    cy.get('[data-testid="next-button"]').click();

    // Confirm
    cy.wait('@getPreview');
    cy.get('[data-testid="confirm-button"]').click();

    // Verify error message
    cy.wait('@insufficientBalance');
    cy.get('[data-testid="error-message"]')
      .should('be.visible')
      .and('contain', 'Insufficient USDC balance');
  });

  it('TC-313.3: Back navigation preserves form state', () => {
    // Step 1: Enter amount
    cy.get('[data-testid="usdc-amount-input"]').type('100');
    cy.get('[data-testid="next-button"]').click();

    // Step 2: Select bank
    cy.wait('@getBankAccounts');
    cy.get('[data-testid="bank-account-card"]').first().click();
    cy.get('[data-testid="next-button"]').click();

    // Step 3: Go back to bank selection
    cy.wait('@getPreview');
    cy.get('[data-testid="back-button"]').click();
    cy.get('[data-testid="bank-account-card"]').first()
      .should('have.class', 'selected');

    // Go back to amount entry
    cy.get('[data-testid="back-button"]').click();
    cy.get('[data-testid="usdc-amount-input"]')
      .should('have.value', '100');

    // Verify exchange rate still displayed
    cy.get('[data-testid="exchange-rate"]').should('be.visible');
  });

  it('TC-313.4: Exchange rate auto-refresh after 30 seconds', () => {
    cy.get('[data-testid="usdc-amount-input"]').type('100');

    // Verify countdown starts at 30
    cy.get('[data-testid="rate-countdown"]').should('contain', '30');

    // Advance clock by 30 seconds
    cy.clock();
    cy.tick(30000);

    // Verify rate refresh API called
    cy.wait('@getExchangeRate');
    cy.get('[data-testid="rate-updated-indicator"]').should('be.visible');
  });

  it('TC-313.5: Manual exchange rate refresh', () => {
    cy.get('[data-testid="usdc-amount-input"]').type('100');

    // Click refresh button
    cy.get('[data-testid="refresh-rate-button"]').click();

    // Verify loading state
    cy.get('[data-testid="loading-spinner"]').should('be.visible');

    // Wait for refresh
    cy.wait('@getExchangeRate');
    cy.get('[data-testid="exchange-rate"]').should('be.visible');
    cy.get('[data-testid="loading-spinner"]').should('not.exist');
  });

  it('TC-313.6: Validation errors on invalid input', () => {
    // Test negative amount
    cy.get('[data-testid="usdc-amount-input"]').type('-50');
    cy.get('[data-testid="validation-error"]')
      .should('contain', 'Amount must be greater than 0');

    // Test zero amount
    cy.get('[data-testid="usdc-amount-input"]').clear().type('0');
    cy.get('[data-testid="validation-error"]')
      .should('contain', 'Amount must be greater than 0');

    // Test non-numeric
    cy.get('[data-testid="usdc-amount-input"]').clear().type('abc');
    cy.get('[data-testid="validation-error"]')
      .should('contain', 'Please enter a valid number');
  });

  it('TC-313.7: Add bank account from withdrawal flow', () => {
    cy.get('[data-testid="usdc-amount-input"]').type('100');
    cy.get('[data-testid="next-button"]').click();

    // Click add bank account button
    cy.wait('@getBankAccounts');
    cy.get('[data-testid="add-bank-account-button"]').click();

    // Verify redirected to add bank account page
    cy.url().should('include', '/bank-accounts/add');
    cy.get('[data-testid="return-to-withdrawal"]').should('be.visible');
  });

  it('TC-313.8: Cancel withdrawal and return to dashboard', () => {
    cy.get('[data-testid="usdc-amount-input"]').type('100');
    cy.get('[data-testid="cancel-button"]').click();

    // Verify confirmation dialog
    cy.get('[data-testid="cancel-confirmation-dialog"]').should('be.visible');
    cy.get('[data-testid="confirm-cancel"]').click();

    // Verify redirected to dashboard
    cy.url().should('include', '/dashboard');
  });
});

/**
 * Helper: Login command
 */
Cypress.Commands.add('login', (email: string) => {
  // Mock authentication
  cy.intercept('POST', '/api/auth/login/dev', {
    statusCode: 200,
    body: {
      token: 'fake-jwt-token',
      username: email.split('@')[0],
      walletAddress: '0x1234567890abcdef',
      expiresAt: new Date(Date.now() + 86400000).toISOString()
    }
  });

  // Store token in localStorage
  window.localStorage.setItem('authToken', 'fake-jwt-token');
  window.localStorage.setItem('walletAddress', '0x1234567890abcdef');
});
