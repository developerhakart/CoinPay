/**
 * QA-204: Phase 2 Automated E2E Tests - Merchant Dashboard
 * Cypress Test Suite for Merchant Dashboard Features
 */

describe('Merchant Dashboard', () => {
  const merchantEmail = Cypress.env('testMerchantEmail');
  const apiUrl = Cypress.env('apiUrl');

  beforeEach(() => {
    // Setup: Login as merchant
    cy.session('merchant-session', () => {
      cy.visit('/login');
      cy.get('input[name="email"]').type(merchantEmail);
      cy.get('button').contains(/login/i).click();
      cy.url().should('include', '/merchant/dashboard');
    });

    cy.visit('/merchant/dashboard');
  });

  describe('Dashboard Overview', () => {
    it('should display merchant dashboard with overview widgets', () => {
      // Verify dashboard loads
      cy.get('h1, h2').should('contain.text', /dashboard|overview/i);

      // Verify overview widgets
      cy.get('[data-testid="total-revenue"], [class*="revenue"]').should('be.visible');
      cy.get('[data-testid="transactions-today"], [class*="transactions"]').should('be.visible');
      cy.get('[data-testid="pending-payments"], [class*="pending"]').should('be.visible');

      // Verify quick action buttons
      cy.get('button').contains(/create.*payment.*link|new.*invoice/i).should('be.visible');
    });

    it('should display recent transactions list', () => {
      // Verify recent transactions section
      cy.get('[data-testid="recent-transactions"], [class*="recent"]').should('be.visible');

      // Verify transaction items displayed
      cy.get('[data-testid="transaction-item"], [class*="transaction"]')
        .should('have.length.greaterThan', 0);

      // Verify transaction details
      cy.get('[data-testid="transaction-item"]').first().within(() => {
        cy.get('[data-testid="customer-name"], [class*="customer"]').should('be.visible');
        cy.get('[data-testid="amount"], [class*="amount"]').should('be.visible');
        cy.get('[data-testid="status"], [class*="status"]').should('be.visible');
        cy.get('[data-testid="timestamp"], [class*="time"]').should('be.visible');
      });
    });
  });

  describe('Transaction Monitoring', () => {
    beforeEach(() => {
      cy.visit('/merchant/transactions');
    });

    it('should display all merchant transactions', () => {
      cy.get('h1, h2').should('contain.text', /transactions/i);

      // Verify transactions load
      cy.get('[data-testid="transaction-list"]').should('be.visible');
      cy.get('[data-testid="transaction-card"]').should('have.length.greaterThan', 0);

      // Verify transaction details
      cy.get('[data-testid="transaction-card"]').first().within(() => {
        cy.get('[data-testid="customer-name"]').should('be.visible');
        cy.get('[data-testid="amount"]').should('be.visible');
        cy.get('[data-testid="status-badge"]').should('be.visible');
      });
    });

    it('should filter transactions by status', () => {
      // Apply status filter
      cy.get('select[name="status"], button').contains(/status|filter/i).click();
      cy.get('option, li').contains(/completed/i).click();

      // Verify filtered results
      cy.get('[data-testid="transaction-card"]').each(($card) => {
        cy.wrap($card).find('[data-testid="status-badge"]')
          .should('contain.text', /completed/i);
      });

      // Verify filter chip displayed
      cy.get('[data-testid="active-filters"], [class*="chip"]')
        .should('contain.text', /completed/i);
    });

    it('should filter transactions by date range', () => {
      // Open date filter
      cy.get('button').contains(/date.*range|filter.*date/i).click();

      // Select date range (last 7 days)
      cy.get('input[name="startDate"], [data-testid="start-date"]')
        .type('2025-10-22');
      cy.get('input[name="endDate"], [data-testid="end-date"]')
        .type('2025-10-29');
      cy.get('button').contains(/apply|filter/i).click();

      // Verify filter applied
      cy.get('[data-testid="active-filters"]')
        .should('contain.text', /2025-10-22.*2025-10-29/i);
    });

    it('should search transactions by customer name', () => {
      // Enter search query
      cy.get('input[placeholder*="search" i]').type('John Doe');

      // Verify search results
      cy.get('[data-testid="transaction-card"]').should('have.length.greaterThan', 0);
      cy.get('[data-testid="customer-name"]')
        .should('contain.text', /john.*doe/i);
    });
  });

  describe('Transaction Details', () => {
    beforeEach(() => {
      cy.visit('/merchant/transactions');
    });

    it('should open transaction detail modal', () => {
      // Click on first transaction
      cy.get('[data-testid="transaction-card"]').first().click();

      // Verify modal opens
      cy.get('[role="dialog"], [data-testid="transaction-modal"]')
        .should('be.visible');

      // Verify all transaction details displayed
      cy.get('[data-testid="transaction-id"]').should('be.visible');
      cy.get('[data-testid="customer-info"]').should('be.visible');
      cy.get('[data-testid="amount-detail"]').should('be.visible');
      cy.get('[data-testid="payment-method"]').should('be.visible');
      cy.get('[data-testid="transaction-date"]').should('be.visible');
    });

    it('should allow issuing refund for completed transaction', () => {
      // Find completed transaction
      cy.get('[data-testid="transaction-card"]')
        .contains(/completed/i)
        .first()
        .click();

      // Click refund button
      cy.get('button').contains(/issue.*refund|refund/i).click();

      // Fill refund form
      cy.get('[role="dialog"]').within(() => {
        cy.get('select[name="refundType"]').select('Full');
        cy.get('textarea[name="reason"]').type('Customer requested refund');
        cy.get('button').contains(/confirm.*refund/i).click();
      });

      // Verify success message
      cy.get('[role="alert"], [data-testid="success-message"]')
        .should('contain.text', /refund.*successful|refund.*processed/i);
    });
  });

  describe('Analytics Dashboard', () => {
    beforeEach(() => {
      cy.visit('/merchant/analytics');
    });

    it('should display revenue chart', () => {
      cy.get('h1, h2').should('contain.text', /analytics/i);

      // Verify revenue chart present
      cy.get('[data-testid="revenue-chart"], canvas, svg[class*="chart"]')
        .should('be.visible');

      // Verify total revenue displayed
      cy.get('[data-testid="total-revenue"]')
        .should('be.visible')
        .and('contain.text', /\d+.*USDC/);
    });

    it('should change time range for analytics', () => {
      // Select different time range
      cy.get('button').contains(/7.*days|last.*week/i).click();
      cy.get('option, li').contains(/30.*days|last.*month/i).click();

      // Verify chart updates (wait for API call)
      cy.wait(1000);

      // Verify time range label updated
      cy.get('[data-testid="time-range"]')
        .should('contain.text', /30.*days|month/i);
    });

    it('should display transaction volume chart', () => {
      // Verify transaction volume chart
      cy.get('[data-testid="volume-chart"], canvas, svg')
        .eq(1)
        .should('be.visible');

      // Verify metrics
      cy.get('[data-testid="total-transactions"]').should('be.visible');
      cy.get('[data-testid="average-transaction"]').should('be.visible');
    });
  });

  describe('Transaction Export', () => {
    beforeEach(() => {
      cy.visit('/merchant/transactions');
    });

    it('should export transactions to CSV', () => {
      // Click export button
      cy.get('button').contains(/export|download/i).click();

      // Select CSV format
      cy.get('option, li').contains(/csv/i).click();

      // Verify download initiated (check for download attribute or mock)
      cy.get('a[download*=".csv"]').should('exist');
    });

    it('should export transactions to PDF', () => {
      // Click export button
      cy.get('button').contains(/export|download/i).click();

      // Select PDF format
      cy.get('option, li').contains(/pdf/i).click();

      // Verify download initiated
      cy.get('a[download*=".pdf"]').should('exist');
    });
  });

  describe('Payment Link Generation', () => {
    beforeEach(() => {
      cy.visit('/merchant/payment-links');
    });

    it('should create payment link', () => {
      cy.get('h1, h2').should('contain.text', /payment.*links/i);

      // Click create button
      cy.get('button').contains(/create.*payment.*link|new.*link/i).click();

      // Fill form
      cy.get('input[name="amount"]').type('100.00');
      cy.get('input[name="description"]').type('Test payment link');
      cy.get('select[name="currency"]').select('USDC');
      cy.get('input[name="expiry"]').type('24'); // hours

      // Submit
      cy.get('button').contains(/generate.*link|create/i).click();

      // Verify link created
      cy.get('[data-testid="payment-link"], [class*="link-display"]')
        .should('be.visible')
        .and('contain.text', /https?:\/\//);

      // Verify QR code generated
      cy.get('[data-testid="qr-code"], canvas, svg').should('be.visible');

      // Verify copy button
      cy.get('button').contains(/copy.*link/i).should('be.visible');
    });

    it('should display payment link status', () => {
      // Find existing payment link
      cy.get('[data-testid="payment-link-item"]').first().within(() => {
        cy.get('[data-testid="link-status"]').should('be.visible');
        cy.get('[data-testid="view-count"]').should('be.visible');
        cy.get('[data-testid="expiry-time"]').should('be.visible');
      });
    });
  });

  describe('Merchant Settings', () => {
    beforeEach(() => {
      cy.visit('/merchant/settings');
    });

    it('should display merchant profile settings', () => {
      cy.get('h1, h2').should('contain.text', /settings/i);

      // Verify settings form
      cy.get('input[name="businessName"]').should('be.visible');
      cy.get('input[name="email"]').should('be.visible');
      cy.get('input[name="phone"]').should('be.visible');
    });

    it('should update notification preferences', () => {
      // Navigate to notifications tab
      cy.get('button, a').contains(/notifications/i).click();

      // Toggle notification setting
      cy.get('input[type="checkbox"][name*="payment"]')
        .first()
        .click();

      // Save settings
      cy.get('button').contains(/save.*settings/i).click();

      // Verify success message
      cy.get('[role="alert"]')
        .should('contain.text', /settings.*saved|updated.*successfully/i);
    });
  });

  describe('Webhook Configuration', () => {
    beforeEach(() => {
      cy.visit('/merchant/webhooks');
    });

    it('should add webhook URL', () => {
      cy.get('h1, h2').should('contain.text', /webhooks/i);

      // Click add webhook
      cy.get('button').contains(/add.*webhook|new.*webhook/i).click();

      // Fill webhook form
      cy.get('input[name="webhookUrl"]')
        .type('https://merchant-api.com/webhook');

      // Select events
      cy.get('input[type="checkbox"][value*="payment.completed"]').check();
      cy.get('input[type="checkbox"][value*="payment.failed"]').check();

      // Save webhook
      cy.get('button').contains(/save.*webhook|add/i).click();

      // Verify webhook created
      cy.get('[data-testid="webhook-item"]')
        .should('contain.text', 'https://merchant-api.com/webhook');
    });

    it('should test webhook', () => {
      // Find webhook
      cy.get('[data-testid="webhook-item"]').first().within(() => {
        cy.get('button').contains(/test|send.*test/i).click();
      });

      // Verify test event sent
      cy.get('[role="alert"]')
        .should('contain.text', /test.*event.*sent|webhook.*tested/i);
    });
  });
});
