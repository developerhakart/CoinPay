/**
 * QA-204: Phase 2 Automated E2E Tests - Customer Dashboard
 * Cypress Test Suite for Customer Dashboard Features
 */

describe('Customer Dashboard', () => {
  const customerEmail = Cypress.env('testCustomerEmail');

  beforeEach(() => {
    // Setup: Login as customer
    cy.session('customer-session', () => {
      cy.visit('/login');
      cy.get('input[name="email"]').type(customerEmail);
      cy.get('button').contains(/login/i).click();
      cy.url().should('include', '/customer/dashboard');
    });

    cy.visit('/customer/dashboard');
  });

  describe('Dashboard Overview', () => {
    it('should display customer dashboard', () => {
      // Verify dashboard loads
      cy.get('h1, h2').should('contain.text', /dashboard|overview/i);

      // Verify balance displayed
      cy.get('[data-testid="wallet-balance"]').should('be.visible');

      // Verify recent payments section
      cy.get('[data-testid="recent-payments"]').should('be.visible');

      // Verify quick actions
      cy.get('button').contains(/send.*money|transfer/i).should('be.visible');
      cy.get('button').contains(/request.*payment/i).should('be.visible');
    });
  });

  describe('Payment History', () => {
    beforeEach(() => {
      cy.visit('/customer/payments');
    });

    it('should display payment history', () => {
      cy.get('h1, h2').should('contain.text', /payment.*history|payments/i);

      // Verify payments list
      cy.get('[data-testid="payment-list"]').should('be.visible');
      cy.get('[data-testid="payment-item"]').should('have.length.greaterThan', 0);

      // Verify payment details displayed
      cy.get('[data-testid="payment-item"]').first().within(() => {
        cy.get('[data-testid="merchant-name"]').should('be.visible');
        cy.get('[data-testid="amount"]').should('be.visible');
        cy.get('[data-testid="date"]').should('be.visible');
        cy.get('[data-testid="status"]').should('be.visible');
      });
    });

    it('should open payment detail modal', () => {
      // Click on payment
      cy.get('[data-testid="payment-item"]').first().click();

      // Verify modal opens
      cy.get('[role="dialog"]').should('be.visible');

      // Verify payment details
      cy.get('[data-testid="payment-id"]').should('be.visible');
      cy.get('[data-testid="merchant-info"]').should('be.visible');
      cy.get('[data-testid="amount-detail"]').should('be.visible');
      cy.get('[data-testid="payment-date"]').should('be.visible');

      // Verify receipt download button
      cy.get('button').contains(/download.*receipt/i).should('be.visible');

      // Verify dispute option for completed payments
      cy.get('[data-testid="status"]').then(($status) => {
        if ($status.text().match(/completed/i)) {
          cy.get('button').contains(/dispute/i).should('be.visible');
        }
      });
    });
  });

  describe('Receipt Download', () => {
    beforeEach(() => {
      cy.visit('/customer/payments');
    });

    it('should download payment receipt', () => {
      // Open payment detail
      cy.get('[data-testid="payment-item"]').first().click();

      // Click download receipt
      cy.get('button').contains(/download.*receipt/i).click();

      // Verify download initiated (PDF)
      cy.get('a[download*=".pdf"]').should('exist');
    });
  });

  describe('Request Payment', () => {
    beforeEach(() => {
      cy.visit('/customer/request-payment');
    });

    it('should create payment request', () => {
      cy.get('h1, h2').should('contain.text', /request.*payment/i);

      // Fill request form
      cy.get('input[name="amount"]').type('50.00');
      cy.get('input[name="description"]').type('Invoice #12345');
      cy.get('input[name="recipientEmail"]').type('merchant@example.com');

      // Submit request
      cy.get('button').contains(/send.*request|create/i).click();

      // Verify success message
      cy.get('[role="alert"]')
        .should('contain.text', /request.*sent|payment.*request.*created/i);

      // Verify request appears in sent requests list
      cy.get('[data-testid="sent-requests"]')
        .should('contain.text', 'Invoice #12345');
    });

    it('should display sent payment requests', () => {
      cy.visit('/customer/requests');

      // Verify requests list
      cy.get('[data-testid="request-item"]').should('have.length.greaterThan', 0);

      // Verify request details
      cy.get('[data-testid="request-item"]').first().within(() => {
        cy.get('[data-testid="amount"]').should('be.visible');
        cy.get('[data-testid="recipient"]').should('be.visible');
        cy.get('[data-testid="status"]').should('be.visible');
      });
    });
  });

  describe('Saved Recipients', () => {
    beforeEach(() => {
      cy.visit('/customer/recipients');
    });

    it('should add new recipient', () => {
      cy.get('h1, h2').should('contain.text', /recipients/i);

      // Click add recipient
      cy.get('button').contains(/add.*recipient/i).click();

      // Fill recipient form
      cy.get('input[name="name"]').type('Alice Johnson');
      cy.get('input[name="walletAddress"]')
        .type('0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0');

      // Save recipient
      cy.get('button').contains(/save.*recipient|add/i).click();

      // Verify recipient added
      cy.get('[data-testid="recipient-item"]')
        .should('contain.text', 'Alice Johnson');
    });

    it('should use saved recipient in transfer', () => {
      // Select recipient
      cy.get('[data-testid="recipient-item"]').first().within(() => {
        cy.get('button').contains(/send|transfer/i).click();
      });

      // Verify redirected to transfer page with pre-filled address
      cy.url().should('include', '/transfer');
      cy.get('input[name="recipient"]').should('have.value', /0x/);
    });

    it('should delete recipient', () => {
      // Get initial count
      cy.get('[data-testid="recipient-item"]').its('length').then((initialCount) => {
        // Click delete on first recipient
        cy.get('[data-testid="recipient-item"]').first().within(() => {
          cy.get('button[aria-label*="delete"], button').contains(/delete|remove/i).click();
        });

        // Confirm deletion
        cy.get('button').contains(/confirm|yes|delete/i).click();

        // Verify recipient removed
        cy.get('[data-testid="recipient-item"]')
          .should('have.length', initialCount - 1);
      });
    });
  });

  describe('Notification Settings', () => {
    beforeEach(() => {
      cy.visit('/customer/settings/notifications');
    });

    it('should update notification preferences', () => {
      cy.get('h1, h2').should('contain.text', /notifications/i);

      // Toggle email notifications
      cy.get('input[type="checkbox"][name*="email"]').first().click();

      // Toggle push notifications
      cy.get('input[type="checkbox"][name*="push"]').first().click();

      // Save settings
      cy.get('button').contains(/save.*settings/i).click();

      // Verify success
      cy.get('[role="alert"]')
        .should('contain.text', /preferences.*saved|settings.*updated/i);

      // Refresh page and verify settings persisted
      cy.reload();
      cy.get('input[type="checkbox"][name*="email"]')
        .first()
        .should('be.checked');
    });
  });

  describe('Transaction Dispute', () => {
    beforeEach(() => {
      cy.visit('/customer/payments');
    });

    it('should initiate dispute for completed payment', () => {
      // Find completed payment
      cy.get('[data-testid="payment-item"]').first().click();

      // Click dispute button
      cy.get('button').contains(/dispute/i).click();

      // Fill dispute form
      cy.get('select[name="disputeReason"]')
        .select('Product not received');
      cy.get('textarea[name="description"]')
        .type('Order #123 never arrived after 2 weeks');

      // Submit dispute
      cy.get('button').contains(/submit.*dispute/i).click();

      // Verify dispute created
      cy.get('[role="alert"]')
        .should('contain.text', /dispute.*submitted|dispute.*created/i);

      // Verify dispute appears in disputes list
      cy.visit('/customer/disputes');
      cy.get('[data-testid="dispute-item"]')
        .first()
        .should('contain.text', 'Product not received');
    });

    it('should view dispute status', () => {
      cy.visit('/customer/disputes');

      // Verify disputes list
      cy.get('[data-testid="dispute-item"]').should('be.visible');

      // Click on dispute
      cy.get('[data-testid="dispute-item"]').first().click();

      // Verify dispute details
      cy.get('[data-testid="dispute-status"]').should('be.visible');
      cy.get('[data-testid="dispute-timeline"]').should('be.visible');
      cy.get('[data-testid="merchant-response"]').should('exist');
    });
  });

  describe('Account Security', () => {
    beforeEach(() => {
      cy.visit('/customer/settings/security');
    });

    it('should display passkey management', () => {
      cy.get('h1, h2').should('contain.text', /security/i);

      // Verify passkeys list
      cy.get('[data-testid="passkey-list"]').should('be.visible');
      cy.get('[data-testid="passkey-item"]').should('have.length.greaterThan', 0);

      // Verify add passkey button
      cy.get('button').contains(/add.*passkey|new.*passkey/i).should('be.visible');
    });

    it('should display active sessions', () => {
      // Navigate to sessions tab
      cy.get('button, a').contains(/sessions/i).click();

      // Verify sessions list
      cy.get('[data-testid="session-item"]').should('be.visible');

      // Verify session details
      cy.get('[data-testid="session-item"]').first().within(() => {
        cy.get('[data-testid="device-info"]').should('be.visible');
        cy.get('[data-testid="browser-info"]').should('be.visible');
        cy.get('[data-testid="last-active"]').should('be.visible');
      });

      // Verify revoke button
      cy.get('button').contains(/revoke|end.*session/i).should('be.visible');
    });

    it('should view activity log', () => {
      // Navigate to activity tab
      cy.get('button, a').contains(/activity/i).click();

      // Verify activity log
      cy.get('[data-testid="activity-item"]').should('have.length.greaterThan', 0);

      // Verify activity details
      cy.get('[data-testid="activity-item"]').first().within(() => {
        cy.get('[data-testid="activity-type"]').should('be.visible');
        cy.get('[data-testid="activity-time"]').should('be.visible');
        cy.get('[data-testid="activity-details"]').should('be.visible');
      });
    });
  });
});
