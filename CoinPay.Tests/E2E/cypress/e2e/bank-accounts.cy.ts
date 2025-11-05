/**
 * E2E Tests for Bank Account Management
 * Sprint N03 - Phase 3: Fiat Off-Ramp
 *
 * Test Coverage:
 * - Add new bank account
 * - Form validation
 * - Routing number lookup
 * - Edit bank account
 * - Delete bank account
 * - Primary account management
 */

describe('Bank Account Management', () => {
  beforeEach(() => {
    cy.login('testuser@test.com');
    cy.visit('/bank-accounts');

    // Mock empty bank accounts list
    cy.intercept('GET', '/api/bank-account', {
      statusCode: 200,
      body: {
        bankAccounts: [],
        total: 0
      }
    }).as('getEmptyBankAccounts');
  });

  it('TC-314.1: Add new bank account', () => {
    cy.wait('@getEmptyBankAccounts');

    // Verify empty state
    cy.get('[data-testid="empty-state"]').should('be.visible')
      .and('contain', 'No bank accounts');

    // Click add bank account button
    cy.get('[data-testid="add-bank-button"]').click();

    // Verify form is displayed
    cy.get('[data-testid="bank-account-form"]').should('be.visible');

    // Mock routing number lookup
    cy.intercept('GET', '/api/bank-account/lookup/011401533', {
      statusCode: 200,
      body: {
        routingNumber: '011401533',
        bankName: 'Wells Fargo'
      }
    }).as('lookupRouting');

    // Fill routing number (triggers lookup)
    cy.get('[data-testid="routing-number"]').type('011401533');
    cy.wait('@lookupRouting');

    // Verify bank name auto-populated
    cy.get('[data-testid="bank-name"]').should('have.value', 'Wells Fargo');

    // Fill remaining fields
    cy.get('[data-testid="account-holder-name"]').type('John Doe');
    cy.get('[data-testid="account-number"]').type('1234567890');
    cy.get('[data-testid="account-type"]').select('checking');

    // Set as primary
    cy.get('[data-testid="is-primary-checkbox"]').check();

    // Mock create bank account
    cy.intercept('POST', '/api/bank-account', {
      statusCode: 201,
      body: {
        id: 'bank-account-id-1',
        accountHolderName: 'John Doe',
        lastFourDigits: '7890',
        accountType: 'checking',
        bankName: 'Wells Fargo',
        isPrimary: true,
        isVerified: false,
        createdAt: new Date().toISOString()
      }
    }).as('createBankAccount');

    // Submit form
    cy.get('[data-testid="submit-button"]').click();

    // Verify success notification
    cy.wait('@createBankAccount');
    cy.get('[data-testid="success-notification"]')
      .should('be.visible')
      .and('contain', 'Bank account added successfully');

    // Verify redirected to list
    cy.url().should('include', '/bank-accounts');

    // Mock bank accounts list with new account
    cy.intercept('GET', '/api/bank-account', {
      statusCode: 200,
      body: {
        bankAccounts: [
          {
            id: 'bank-account-id-1',
            accountHolderName: 'John Doe',
            lastFourDigits: '7890',
            accountType: 'checking',
            bankName: 'Wells Fargo',
            isPrimary: true,
            isVerified: false
          }
        ],
        total: 1
      }
    });

    // Verify account appears in list
    cy.get('[data-testid="bank-account-card"]')
      .should('have.length', 1)
      .and('contain', 'Wells Fargo')
      .and('contain', '•••• 7890')
      .and('contain', 'PRIMARY');
  });

  it('TC-314.2: Validation errors shown for invalid input', () => {
    cy.get('[data-testid="add-bank-button"]').click();

    // Test invalid routing number (invalid checksum)
    cy.get('[data-testid="routing-number"]').type('123456789');
    cy.get('[data-testid="routing-number"]').blur();
    cy.get('[data-testid="routing-error"]')
      .should('be.visible')
      .and('contain', 'Invalid routing number checksum');

    // Clear and test short routing number
    cy.get('[data-testid="routing-number"]').clear().type('12345');
    cy.get('[data-testid="routing-number"]').blur();
    cy.get('[data-testid="routing-error"]')
      .should('contain', 'Routing number must be exactly 9 digits');

    // Test short account number
    cy.get('[data-testid="account-number"]').type('123');
    cy.get('[data-testid="account-number"]').blur();
    cy.get('[data-testid="account-error"]')
      .should('contain', 'Account number must be at least 5 digits');

    // Test long account number
    cy.get('[data-testid="account-number"]').clear()
      .type('123456789012345678'); // 18 digits
    cy.get('[data-testid="account-number"]').blur();
    cy.get('[data-testid="account-error"]')
      .should('contain', 'Account number must be at most 17 digits');

    // Test short account holder name
    cy.get('[data-testid="account-holder-name"]').type('A');
    cy.get('[data-testid="account-holder-name"]').blur();
    cy.get('[data-testid="name-error"]')
      .should('contain', 'Account holder name must be at least 2 characters');

    // Test invalid characters in name
    cy.get('[data-testid="account-holder-name"]').clear().type('John123');
    cy.get('[data-testid="account-holder-name"]').blur();
    cy.get('[data-testid="name-error"]')
      .should('contain', 'Only letters, spaces, hyphens, and apostrophes allowed');

    // Submit button should be disabled
    cy.get('[data-testid="submit-button"]').should('be.disabled');
  });

  it('TC-314.3: Edit existing bank account', () => {
    // Mock bank accounts list
    cy.intercept('GET', '/api/bank-account', {
      statusCode: 200,
      body: {
        bankAccounts: [
          {
            id: 'bank-account-id-1',
            accountHolderName: 'John Doe',
            lastFourDigits: '7890',
            accountType: 'checking',
            bankName: 'Wells Fargo',
            isPrimary: true,
            isVerified: true
          }
        ],
        total: 1
      }
    }).as('getBankAccounts');

    cy.wait('@getBankAccounts');

    // Click edit button
    cy.get('[data-testid="edit-bank-button"]').click();

    // Verify form pre-populated
    cy.get('[data-testid="account-holder-name"]')
      .should('have.value', 'John Doe');
    cy.get('[data-testid="bank-name"]')
      .should('have.value', 'Wells Fargo');

    // Verify routing/account number fields disabled
    cy.get('[data-testid="routing-number"]').should('be.disabled');
    cy.get('[data-testid="account-number"]').should('be.disabled');

    // Update account holder name
    cy.get('[data-testid="account-holder-name"]')
      .clear()
      .type('Jonathan Doe');

    // Mock update API
    cy.intercept('PUT', '/api/bank-account/bank-account-id-1', {
      statusCode: 200,
      body: {
        id: 'bank-account-id-1',
        accountHolderName: 'Jonathan Doe',
        lastFourDigits: '7890',
        accountType: 'checking',
        bankName: 'Wells Fargo',
        isPrimary: true,
        isVerified: true
      }
    }).as('updateBankAccount');

    // Submit
    cy.get('[data-testid="submit-button"]').click();

    // Verify success
    cy.wait('@updateBankAccount');
    cy.get('[data-testid="success-notification"]')
      .should('contain', 'Bank account updated');
  });

  it('TC-314.4: Delete bank account with confirmation', () => {
    // Mock bank accounts list
    cy.intercept('GET', '/api/bank-account', {
      statusCode: 200,
      body: {
        bankAccounts: [
          {
            id: 'bank-account-id-1',
            accountHolderName: 'John Doe',
            lastFourDigits: '7890',
            accountType: 'checking',
            bankName: 'Wells Fargo',
            isPrimary: true,
            isVerified: true
          }
        ],
        total: 1
      }
    });

    cy.wait(100); // Wait for API call

    // Click delete button
    cy.get('[data-testid="delete-bank-button"]').click();

    // Verify confirmation dialog
    cy.get('[data-testid="delete-confirmation-dialog"]').should('be.visible')
      .and('contain', 'Are you sure')
      .and('contain', '•••• 7890');

    // Cancel deletion
    cy.get('[data-testid="cancel-delete"]').click();
    cy.get('[data-testid="delete-confirmation-dialog"]').should('not.exist');

    // Try delete again
    cy.get('[data-testid="delete-bank-button"]').click();

    // Mock delete API
    cy.intercept('DELETE', '/api/bank-account/bank-account-id-1', {
      statusCode: 204
    }).as('deleteBankAccount');

    // Confirm deletion
    cy.get('[data-testid="confirm-delete"]').click();

    // Verify deletion
    cy.wait('@deleteBankAccount');
    cy.get('[data-testid="success-notification"]')
      .should('contain', 'Bank account deleted');

    // Verify empty state shown
    cy.get('[data-testid="empty-state"]').should('be.visible');
  });

  it('TC-314.5: Primary account badge and management', () => {
    // Mock multiple bank accounts
    cy.intercept('GET', '/api/bank-account', {
      statusCode: 200,
      body: {
        bankAccounts: [
          {
            id: 'bank-account-id-1',
            accountHolderName: 'John Doe',
            lastFourDigits: '7890',
            accountType: 'checking',
            bankName: 'Wells Fargo',
            isPrimary: true,
            isVerified: true
          },
          {
            id: 'bank-account-id-2',
            accountHolderName: 'John Doe',
            lastFourDigits: '1234',
            accountType: 'savings',
            bankName: 'Chase Bank',
            isPrimary: false,
            isVerified: true
          }
        ],
        total: 2
      }
    });

    cy.wait(100);

    // Verify primary badge on first account
    cy.get('[data-testid="bank-account-card"]').first()
      .should('contain', 'PRIMARY');

    // Verify no badge on second account
    cy.get('[data-testid="bank-account-card"]').eq(1)
      .should('not.contain', 'PRIMARY');

    // Set second account as primary
    cy.get('[data-testid="bank-account-card"]').eq(1)
      .find('[data-testid="set-primary-button"]').click();

    // Mock update API
    cy.intercept('PUT', '/api/bank-account/bank-account-id-2', {
      statusCode: 200,
      body: {
        id: 'bank-account-id-2',
        accountHolderName: 'John Doe',
        lastFourDigits: '1234',
        accountType: 'savings',
        bankName: 'Chase Bank',
        isPrimary: true,
        isVerified: true
      }
    });

    // Verify primary badge moved
    cy.wait(500);
    cy.get('[data-testid="bank-account-card"]').eq(1)
      .should('contain', 'PRIMARY');
  });

  it('TC-314.6: Routing number lookup - not found', () => {
    cy.get('[data-testid="add-bank-button"]').click();

    // Mock routing number lookup - not found
    cy.intercept('GET', '/api/bank-account/lookup/999999999', {
      statusCode: 404,
      body: {
        error: {
          code: 'BANK_NOT_FOUND',
          message: 'Bank not found for this routing number'
        }
      }
    });

    // Enter unknown routing number
    cy.get('[data-testid="routing-number"]').type('999999999');

    // Verify bank name field remains empty
    cy.get('[data-testid="bank-name"]').should('have.value', '');

    // User can manually enter bank name
    cy.get('[data-testid="bank-name"]').type('Unknown Bank');
    cy.get('[data-testid="bank-name"]').should('have.value', 'Unknown Bank');
  });
});
