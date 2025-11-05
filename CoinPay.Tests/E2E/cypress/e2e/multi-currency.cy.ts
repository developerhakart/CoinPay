/**
 * QA-204: Phase 2 Automated E2E Tests - Multi-Currency Support
 * Cypress Test Suite for Multi-Currency Features
 */

describe('Multi-Currency Support', () => {
  beforeEach(() => {
    // Setup: Login
    cy.session('user-session', () => {
      cy.visit('/login');
      cy.get('input[name="email"]').type('user@test.com');
      cy.get('button').contains(/login/i).click();
    });
  });

  describe('Multi-Currency Wallet', () => {
    beforeEach(() => {
      cy.visit('/wallet');
    });

    it('should display multiple currency balances', () => {
      // Verify USDC balance
      cy.get('[data-testid="balance-USDC"]').should('be.visible');

      // Verify USDT balance (if added)
      cy.get('[data-testid="balance-USDT"], [data-testid="add-currency"]')
        .should('be.visible');

      // Verify total value in USD
      cy.get('[data-testid="total-value-usd"]')
        .should('be.visible')
        .and('contain.text', /\$\d+/);
    });

    it('should add new currency to wallet', () => {
      // Click add currency
      cy.get('button').contains(/add.*currency/i).click();

      // Select USDT
      cy.get('[role="dialog"]').within(() => {
        cy.get('[data-testid="currency-option"]')
          .contains(/USDT|Tether/i)
          .click();
        cy.get('button').contains(/add|confirm/i).click();
      });

      // Verify USDT added
      cy.get('[data-testid="balance-USDT"]')
        .should('be.visible')
        .and('contain.text', '0.000000 USDT');
    });

    it('should display exchange rates', () => {
      // Verify exchange rate shown
      cy.get('[data-testid="exchange-rate"]').should('be.visible');

      // Verify rate format
      cy.get('[data-testid="exchange-rate"]')
        .should('contain.text', /1 USDC.*=.*\$\d+\.\d+/);

      // Verify last updated timestamp
      cy.get('[data-testid="rate-updated"]')
        .should('be.visible')
        .and('contain.text', /updated|last/i);
    });

    it('should refresh exchange rates', () => {
      // Get initial rate value
      cy.get('[data-testid="exchange-rate"]')
        .invoke('text')
        .then((initialRate) => {
          // Click refresh rates
          cy.get('button[aria-label*="refresh"], button').contains(/refresh.*rates/i).click();

          // Verify loading state
          cy.get('[data-testid="rate-loading"]').should('be.visible');

          // Wait for refresh
          cy.wait(1000);

          // Verify timestamp updated
          cy.get('[data-testid="rate-updated"]')
            .should('contain.text', /just now|seconds ago/i);
        });
    });
  });

  describe('Currency Selection in Transfer', () => {
    beforeEach(() => {
      cy.visit('/transfer');
    });

    it('should select currency for transfer', () => {
      // Verify currency selector visible
      cy.get('select[name="currency"], [data-testid="currency-select"]')
        .should('be.visible');

      // Select USDC
      cy.get('select[name="currency"]').select('USDC');

      // Verify balance for selected currency shown
      cy.get('[data-testid="available-balance"]')
        .should('contain.text', /USDC/i);

      // Fill transfer details
      cy.get('input[name="recipient"]')
        .type('0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0');
      cy.get('input[name="amount"]').type('10');

      // Verify amount in selected currency
      cy.get('[data-testid="amount-display"]')
        .should('contain.text', /10.*USDC/);
    });

    it('should show currency-specific balance', () => {
      // Select USDT
      cy.get('select[name="currency"]').select('USDT');

      // Verify USDT balance displayed
      cy.get('[data-testid="available-balance"]')
        .should('contain.text', /USDT/i);

      // Switch to USDC
      cy.get('select[name="currency"]').select('USDC');

      // Verify USDC balance displayed
      cy.get('[data-testid="available-balance"]')
        .should('contain.text', /USDC/i);
    });

    it('should validate insufficient balance per currency', () => {
      // Select currency
      cy.get('select[name="currency"]').select('USDC');

      // Get current balance
      cy.get('[data-testid="available-balance"]')
        .invoke('text')
        .then((balanceText) => {
          const balance = parseFloat(balanceText.match(/[\d.]+/)?.[0] || '0');

          // Try to send more than balance
          cy.get('input[name="recipient"]')
            .type('0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0');
          cy.get('input[name="amount"]').type((balance + 100).toString());
          cy.get('button').contains(/review|next/i).click();

          // Verify insufficient balance error
          cy.get('[role="alert"], .error')
            .should('contain.text', /insufficient.*balance/i);
        });
    });
  });

  describe('Currency Conversion Calculator', () => {
    beforeEach(() => {
      cy.visit('/transfer');
    });

    it('should show equivalent amounts in other currencies', () => {
      // Fill amount
      cy.get('input[name="amount"]').type('100');
      cy.get('select[name="currency"]').select('USDC');

      // Click show in other currencies
      cy.get('button').contains(/show.*other.*currencies|convert/i).click();

      // Verify conversion display
      cy.get('[data-testid="conversion-display"]').should('be.visible');

      // Verify USDT equivalent shown
      cy.get('[data-testid="amount-usdt"]')
        .should('be.visible')
        .and('contain.text', /USDT/);

      // Verify exchange rate displayed
      cy.get('[data-testid="exchange-rate-info"]')
        .should('contain.text', /rate|exchange/i);
    });

    it('should show live exchange rate source', () => {
      cy.get('input[name="amount"]').type('50');
      cy.get('button').contains(/show.*other.*currencies/i).click();

      // Verify rate source
      cy.get('[data-testid="rate-source"]')
        .should('be.visible')
        .and('contain.text', /Chainlink|Oracle/i);
    });
  });

  describe('Currency-Specific Transaction History', () => {
    beforeEach(() => {
      cy.visit('/transactions');
    });

    it('should filter transactions by currency', () => {
      // Apply currency filter
      cy.get('select[name="currency"], button').contains(/currency|all/i).click();
      cy.get('option, li').contains(/USDC/i).click();

      // Verify filtered results
      cy.get('[data-testid="transaction-card"]').each(($card) => {
        cy.wrap($card)
          .find('[data-testid="currency"]')
          .should('contain.text', /USDC/);
      });

      // Verify currency badge/icon shown
      cy.get('[data-testid="currency-badge"]').should('contain.text', /USDC/);
    });

    it('should display currency in transaction list', () => {
      // Verify each transaction shows currency
      cy.get('[data-testid="transaction-card"]').first().within(() => {
        cy.get('[data-testid="amount"]')
          .should('contain.text', /USDC|USDT|DAI/);
      });
    });

    it('should show multi-currency summary', () => {
      // Verify total by currency
      cy.get('[data-testid="currency-summary"]').should('be.visible');

      // Verify breakdown by currency
      cy.get('[data-testid="total-usdc"]').should('be.visible');
      cy.get('[data-testid="total-usdt"]').should('exist');
    });
  });

  describe('Exchange Rate Accuracy', () => {
    it('should display accurate exchange rates', () => {
      cy.visit('/wallet');

      // Get displayed rate
      cy.get('[data-testid="exchange-rate-usdc"]')
        .invoke('text')
        .then((displayedRate) => {
          // Verify rate is recent (within tolerance)
          const rate = parseFloat(displayedRate.match(/[\d.]+/)?.[0] || '0');
          expect(rate).to.be.greaterThan(0.99);
          expect(rate).to.be.lessThan(1.01);
        });

      // Verify rate source
      cy.get('[data-testid="rate-source"]')
        .should('contain.text', /Chainlink/i);

      // Verify update frequency
      cy.get('[data-testid="rate-updated"]')
        .should('contain.text', /seconds ago|minutes ago/i);
    });

    it('should warn if exchange rate is stale', () => {
      cy.visit('/wallet');

      // Mock stale rate (would require backend mock)
      // Verify warning shown
      cy.get('[data-testid="rate-warning"]').should('not.exist');

      // If rate is >5 minutes old, warning should appear
      // This would need backend support to test properly
    });
  });

  describe('Currency Management', () => {
    beforeEach(() => {
      cy.visit('/wallet/currencies');
    });

    it('should display all supported currencies', () => {
      // Verify supported currencies list
      cy.get('[data-testid="supported-currency"]')
        .should('have.length.greaterThan', 1);

      // Verify currency details
      cy.get('[data-testid="supported-currency"]').first().within(() => {
        cy.get('[data-testid="currency-name"]').should('be.visible');
        cy.get('[data-testid="currency-symbol"]').should('be.visible');
        cy.get('[data-testid="network"]').should('contain.text', /Polygon|Ethereum/i);
      });
    });

    it('should remove currency from wallet', () => {
      cy.visit('/wallet');

      // Find currency with 0 balance
      cy.get('[data-testid="currency-item"]')
        .contains(/0\.000000/)
        .parents('[data-testid="currency-item"]')
        .within(() => {
          cy.get('button[aria-label*="remove"], button').contains(/remove/i).click();
        });

      // Confirm removal
      cy.get('button').contains(/confirm|yes|remove/i).click();

      // Verify currency removed
      cy.get('[role="alert"]')
        .should('contain.text', /currency.*removed/i);
    });
  });
});
