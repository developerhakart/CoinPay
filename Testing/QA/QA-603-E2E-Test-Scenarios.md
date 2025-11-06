# QA-603: Full System E2E Test Scenarios
**CoinPay - Sprint N06 Quality Assurance**

---

## Document Information
- **Task ID**: QA-603
- **Document Type**: End-to-End Test Scenarios
- **Created**: 2025-11-06
- **QA Engineer**: Claude QA Agent
- **Total Test Cases**: 102
- **Status**: Documentation Complete - Ready for Execution

---

## Table of Contents
1. [Authentication Flow Tests (15 cases)](#1-authentication-flow-tests)
2. [Wallet Operations Tests (12 cases)](#2-wallet-operations-tests)
3. [Send/Receive Transaction Tests (20 cases)](#3-sendreceive-transaction-tests)
4. [Token Swap Tests (25 cases)](#4-token-swap-tests)
5. [Exchange Investment Tests (20 cases)](#5-exchange-investment-tests)
6. [Cross-Phase Integration Tests (10 cases)](#6-cross-phase-integration-tests)

---

## 1. Authentication Flow Tests

### TC-AUTH-001: User Registration - Valid Data
**Priority**: Critical
**Prerequisites**: None
**Test Data**:
- Email: testuser@example.com
- Password: SecurePass123!
- Name: Test User

**Steps**:
1. Navigate to registration page
2. Enter valid email address
3. Enter strong password (min 8 chars, uppercase, lowercase, number, special char)
4. Confirm password matches
5. Click "Register" button
6. Verify email confirmation sent

**Expected Result**:
- User account created successfully
- Confirmation email sent
- User redirected to email verification page
- Success message displayed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-002: User Registration - Invalid Email Format
**Priority**: High
**Prerequisites**: None
**Test Data**:
- Email: invalid-email-format
- Password: SecurePass123!

**Steps**:
1. Navigate to registration page
2. Enter invalid email format (missing @, invalid domain)
3. Enter valid password
4. Click "Register" button

**Expected Result**:
- Registration blocked
- Error message: "Please enter a valid email address"
- Email field highlighted in red
- Form not submitted

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-003: User Registration - Weak Password
**Priority**: High
**Prerequisites**: None
**Test Data**:
- Email: testuser@example.com
- Password: weak

**Steps**:
1. Navigate to registration page
2. Enter valid email
3. Enter weak password (less than 8 characters, no uppercase/special chars)
4. Click "Register" button

**Expected Result**:
- Registration blocked
- Error message: "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character"
- Password strength indicator shows "Weak"
- Form not submitted

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-004: User Registration - Duplicate Email
**Priority**: Critical
**Prerequisites**: User with testuser@example.com already exists
**Test Data**:
- Email: testuser@example.com (existing)
- Password: SecurePass123!

**Steps**:
1. Navigate to registration page
2. Enter email that's already registered
3. Enter valid password
4. Click "Register" button

**Expected Result**:
- Registration blocked
- Error message: "An account with this email already exists"
- Option to "Login" or "Reset Password" provided
- No duplicate account created

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-005: User Login - Successful Authentication
**Priority**: Critical
**Prerequisites**: Valid user account exists
**Test Data**:
- Email: testuser@example.com
- Password: SecurePass123!

**Steps**:
1. Navigate to login page
2. Enter valid email
3. Enter correct password
4. Click "Login" button

**Expected Result**:
- User authenticated successfully
- JWT token generated and stored
- User redirected to dashboard
- User profile loaded
- Welcome message displayed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-006: User Login - Wrong Password
**Priority**: Critical
**Prerequisites**: Valid user account exists
**Test Data**:
- Email: testuser@example.com
- Password: WrongPassword123!

**Steps**:
1. Navigate to login page
2. Enter valid email
3. Enter incorrect password
4. Click "Login" button

**Expected Result**:
- Authentication fails
- Error message: "Invalid email or password"
- User remains on login page
- Login attempt logged for security monitoring
- No token issued

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-007: User Login - Non-Existent Email
**Priority**: High
**Prerequisites**: None
**Test Data**:
- Email: nonexistent@example.com
- Password: SecurePass123!

**Steps**:
1. Navigate to login page
2. Enter email that doesn't exist in system
3. Enter any password
4. Click "Login" button

**Expected Result**:
- Authentication fails
- Generic error message: "Invalid email or password" (for security)
- User remains on login page
- No information disclosed about email existence

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-008: User Login - Account Lockout After Failed Attempts
**Priority**: Critical
**Prerequisites**: Valid user account exists
**Test Data**:
- Email: testuser@example.com
- Password: WrongPassword (for 5 attempts)

**Steps**:
1. Navigate to login page
2. Enter valid email
3. Enter wrong password
4. Click "Login" button
5. Repeat steps 2-4 for 5 consecutive failed attempts

**Expected Result**:
- After 5 failed attempts, account temporarily locked
- Error message: "Account locked due to multiple failed login attempts. Please try again in 15 minutes or reset your password"
- Lockout duration: 15 minutes
- Email notification sent to user about lockout
- Further login attempts blocked

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-009: User Logout
**Priority**: Critical
**Prerequisites**: User is logged in

**Steps**:
1. User is authenticated and on dashboard
2. Click "Logout" button/link
3. Confirm logout if confirmation modal appears

**Expected Result**:
- User logged out successfully
- JWT token invalidated/cleared
- User redirected to login page
- All session data cleared
- Attempting to access protected routes redirects to login

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-010: Password Reset - Request Reset
**Priority**: High
**Prerequisites**: Valid user account exists
**Test Data**:
- Email: testuser@example.com

**Steps**:
1. Navigate to login page
2. Click "Forgot Password?" link
3. Enter registered email address
4. Click "Send Reset Link" button

**Expected Result**:
- Password reset email sent
- Success message: "If an account exists with this email, a password reset link has been sent"
- Reset token generated with 1-hour expiration
- Email contains reset link with token
- User can check email

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-011: Password Reset - Confirm Reset with Valid Token
**Priority**: High
**Prerequisites**: Valid reset token received via email
**Test Data**:
- Reset Token: valid_token_from_email
- New Password: NewSecurePass456!

**Steps**:
1. Click reset link from email
2. Redirected to password reset page with token in URL
3. Enter new password
4. Confirm new password
5. Click "Reset Password" button

**Expected Result**:
- Password updated successfully
- Success message: "Password reset successfully. Please login with your new password"
- Reset token invalidated
- User redirected to login page
- Can login with new password

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-012: Password Reset - Invalid/Expired Token
**Priority**: High
**Prerequisites**: Expired or invalid reset token
**Test Data**:
- Reset Token: expired_or_invalid_token
- New Password: NewSecurePass456!

**Steps**:
1. Access password reset page with invalid/expired token
2. Enter new password
3. Click "Reset Password" button

**Expected Result**:
- Password reset blocked
- Error message: "Invalid or expired reset token. Please request a new password reset link"
- Password not changed
- User redirected to password reset request page

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-013: Session Management - Session Timeout
**Priority**: High
**Prerequisites**: User is logged in

**Steps**:
1. User logs in successfully
2. Remain inactive for 30 minutes (or configured timeout period)
3. Attempt to perform any action (e.g., view wallet)

**Expected Result**:
- Session expires after timeout period
- User redirected to login page
- Warning message: "Your session has expired. Please login again"
- Token invalidated
- Must re-authenticate to continue

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-014: Session Management - Multiple Devices
**Priority**: Medium
**Prerequisites**: Valid user account

**Steps**:
1. Login from Device A (e.g., Chrome browser)
2. Login from Device B (e.g., Firefox browser) with same credentials
3. Perform action on Device A
4. Perform action on Device B

**Expected Result**:
- Both sessions active simultaneously (if multi-session allowed)
- OR Device A session invalidated when Device B logs in (if single-session enforced)
- Behavior matches documented session policy
- No data corruption between sessions

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-AUTH-015: Session Management - Token Refresh
**Priority**: High
**Prerequisites**: User is logged in with JWT token

**Steps**:
1. User logs in successfully (token issued with expiration)
2. Wait until token is near expiration (e.g., 5 minutes before expiry)
3. Perform any API call that requires authentication

**Expected Result**:
- Token automatically refreshed before expiration
- User session continues without interruption
- New token issued with extended expiration
- No login prompt shown
- API call completes successfully

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

## 2. Wallet Operations Tests

### TC-WALLET-001: Create New Wallet
**Priority**: Critical
**Prerequisites**: User is logged in, no wallet exists

**Steps**:
1. Navigate to wallet creation page
2. Review wallet creation information
3. Click "Create Wallet" button
4. Wait for Circle wallet creation API call
5. Wallet address generated

**Expected Result**:
- Wallet created successfully via Circle API
- Wallet address displayed
- Wallet ID stored in database
- Initial balance shows 0 for all tokens (USDC, ETH, MATIC)
- Success message displayed
- User redirected to wallet dashboard

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-002: View Balance - All Tokens
**Priority**: Critical
**Prerequisites**: Wallet exists with balances
**Test Data**:
- USDC: 100.50
- ETH: 0.05
- MATIC: 25.00

**Steps**:
1. Navigate to wallet dashboard
2. View balance section
3. Verify all token balances displayed

**Expected Result**:
- USDC balance displayed correctly with 2 decimal places
- ETH balance displayed correctly with up to 8 decimal places
- MATIC balance displayed correctly with up to 4 decimal places
- USD equivalent shown for each token
- Total portfolio value calculated
- Balances retrieved from Circle API

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-003: Refresh Balance
**Priority**: High
**Prerequisites**: Wallet exists

**Steps**:
1. Navigate to wallet dashboard
2. Click "Refresh" button/icon
3. Wait for API call to complete

**Expected Result**:
- Loading indicator shown during refresh
- Latest balances fetched from Circle API
- UI updates with current balances
- Last updated timestamp shown
- No page reload required

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-004: View Transaction History - All Transactions
**Priority**: Critical
**Prerequisites**: Wallet with transaction history exists

**Steps**:
1. Navigate to wallet dashboard
2. Click "Transaction History" tab/section
3. View all transactions

**Expected Result**:
- All transactions displayed in reverse chronological order (newest first)
- Each transaction shows:
  - Transaction ID
  - Token type (USDC/ETH/MATIC)
  - Amount
  - Type (Send/Receive/Swap)
  - Status (Pending/Confirmed/Failed)
  - Date and time
  - Transaction hash (if available)
- Pagination implemented (20 transactions per page)
- Total transaction count shown

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-005: Filter Transactions - By Token
**Priority**: Medium
**Prerequisites**: Wallet with mixed token transactions

**Steps**:
1. Navigate to transaction history
2. Select token filter dropdown
3. Select "USDC" filter
4. View filtered results

**Expected Result**:
- Only USDC transactions displayed
- Other token transactions hidden
- Filter selection persists during session
- Clear filter option available
- Transaction count updates to show filtered count

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-006: Filter Transactions - By Status
**Priority**: Medium
**Prerequisites**: Wallet with transactions in various states

**Steps**:
1. Navigate to transaction history
2. Select status filter dropdown
3. Select "Pending" status
4. View filtered results

**Expected Result**:
- Only pending transactions displayed
- Completed and failed transactions hidden
- Status filter can be combined with token filter
- Real-time updates for status changes

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-007: Filter Transactions - By Date Range
**Priority**: Medium
**Prerequisites**: Wallet with transactions spanning multiple dates

**Steps**:
1. Navigate to transaction history
2. Select date range filter
3. Choose start date: 2025-10-01
4. Choose end date: 2025-10-31
5. Apply filter

**Expected Result**:
- Only transactions within selected date range displayed
- Transactions outside range hidden
- Date picker shows valid date ranges
- Filter can be cleared
- "No transactions found" message if no results

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-008: Empty Wallet State
**Priority**: Medium
**Prerequisites**: New wallet with no transactions

**Steps**:
1. Create new wallet
2. Navigate to wallet dashboard
3. View transaction history

**Expected Result**:
- All balances show 0
- Empty state message displayed: "No transactions yet"
- Call-to-action to receive or buy crypto
- Help text explaining how to get started
- No error messages shown

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-009: View Single Transaction Details
**Priority**: High
**Prerequisites**: Wallet with at least one transaction

**Steps**:
1. Navigate to transaction history
2. Click on specific transaction
3. View transaction details modal/page

**Expected Result**:
- Transaction details displayed:
  - Full transaction ID
  - Block explorer link
  - From address
  - To address
  - Amount with token symbol
  - Fee amount
  - Status with timestamp
  - Confirmations count
  - Gas used
  - Transaction hash
- Copy buttons for addresses and hashes
- Share transaction option

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-010: Wallet Address Copy
**Priority**: Low
**Prerequisites**: Wallet exists

**Steps**:
1. Navigate to wallet dashboard
2. Locate wallet address
3. Click "Copy" button next to address

**Expected Result**:
- Wallet address copied to clipboard
- Success toast notification: "Address copied"
- Address remains displayed
- QR code option available (if implemented)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-011: View Wallet QR Code
**Priority**: Low
**Prerequisites**: Wallet exists

**Steps**:
1. Navigate to wallet dashboard
2. Click "Show QR Code" button
3. View QR code modal

**Expected Result**:
- QR code generated from wallet address
- QR code scannable by mobile wallets
- Download QR code option available
- Close button to dismiss modal
- Wallet address shown below QR code

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-WALLET-012: Multiple Wallets Support (if applicable)
**Priority**: Low
**Prerequisites**: System supports multiple wallets per user

**Steps**:
1. Create first wallet
2. Navigate to "Add Wallet" option
3. Create second wallet
4. Switch between wallets

**Expected Result**:
- Multiple wallets can be created
- Each wallet has unique address
- Can switch between wallets via dropdown
- Active wallet clearly indicated
- Each wallet has independent balances and transaction history
- OR system restricts to one wallet per user with clear message

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

## 3. Send/Receive Transaction Tests

### TC-TX-001: Send USDC - Success Path
**Priority**: Critical
**Prerequisites**: Wallet with sufficient USDC balance (min 10 USDC)
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: 5.00 USDC

**Steps**:
1. Navigate to "Send" page
2. Select token: USDC
3. Enter recipient address
4. Enter amount: 5.00
5. Review transaction details (fee, total)
6. Click "Send" button
7. Confirm transaction in modal
8. Wait for transaction processing

**Expected Result**:
- Transaction initiated successfully
- Circle Transfer API called
- Transaction ID returned
- Status updates: Pending → Confirmed → Completed
- Balance deducted (amount + fees)
- Transaction appears in history
- Success notification displayed
- Email notification sent (if configured)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-002: Send USDC - Insufficient Balance
**Priority**: Critical
**Prerequisites**: Wallet with USDC balance: 5.00
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: 10.00 USDC (more than balance)

**Steps**:
1. Navigate to "Send" page
2. Select token: USDC
3. Enter recipient address
4. Enter amount: 10.00
5. Click "Send" button

**Expected Result**:
- Transaction blocked before submission
- Error message: "Insufficient balance. Available: 5.00 USDC"
- Send button disabled
- Balance displayed prominently
- No API call made
- User can adjust amount

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-003: Send - Invalid Recipient Address
**Priority**: Critical
**Prerequisites**: Wallet with sufficient balance
**Test Data**:
- Recipient Address: invalid_address_format
- Amount: 5.00 USDC

**Steps**:
1. Navigate to "Send" page
2. Select token: USDC
3. Enter invalid recipient address
4. Enter amount: 5.00
5. Click "Send" button

**Expected Result**:
- Transaction blocked
- Error message: "Invalid recipient address. Please enter a valid Ethereum address"
- Address field highlighted in red
- Address validation occurs on blur or on submit
- No API call made

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-004: Send - Zero Amount
**Priority**: High
**Prerequisites**: Wallet with sufficient balance
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: 0.00

**Steps**:
1. Navigate to "Send" page
2. Select token: USDC
3. Enter recipient address
4. Enter amount: 0.00
5. Click "Send" button

**Expected Result**:
- Transaction blocked
- Error message: "Amount must be greater than 0"
- Send button disabled
- No API call made

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-005: Send - Negative Amount
**Priority**: High
**Prerequisites**: Wallet with sufficient balance
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: -5.00

**Steps**:
1. Navigate to "Send" page
2. Enter recipient address
3. Attempt to enter negative amount

**Expected Result**:
- Negative input prevented by input validation
- Only positive numbers accepted
- Input field shows 0 or clears negative sign automatically
- OR error message if negative submitted

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-006: Send - Maximum Amount
**Priority**: High
**Prerequisites**: Wallet with USDC balance: 100.00
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: Use "Max" button

**Steps**:
1. Navigate to "Send" page
2. Select token: USDC
3. Enter recipient address
4. Click "Max" button
5. Verify amount auto-filled
6. Review transaction

**Expected Result**:
- Maximum available amount calculated (balance - estimated fees)
- Amount field populated with max amount
- User can send entire balance
- Fee deducted from balance
- Clear indication of amount being sent vs. fees

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-007: Send - Transaction Confirmation Modal
**Priority**: High
**Prerequisites**: Wallet with sufficient balance
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: 5.00 USDC

**Steps**:
1. Navigate to "Send" page
2. Enter all transaction details
3. Click "Send" button
4. Review confirmation modal

**Expected Result**:
- Confirmation modal displays:
  - Recipient address (shortened with copy option)
  - Amount and token
  - Network fee
  - Total amount (amount + fee)
  - Estimated time
- "Confirm" and "Cancel" buttons
- Can cancel and edit details
- Confirm proceeds with transaction

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-008: Transaction Status Tracking - Pending State
**Priority**: Critical
**Prerequisites**: Transaction initiated

**Steps**:
1. Send transaction (follow TC-TX-001)
2. Immediately view transaction in history
3. Monitor status updates

**Expected Result**:
- Transaction shows "Pending" status
- Loading/spinner indicator shown
- Estimated completion time displayed
- Transaction details available
- Can view on block explorer (link provided)
- Timestamp of initiation shown

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-009: Transaction Status Tracking - Confirmed State
**Priority**: Critical
**Prerequisites**: Transaction with confirmations

**Steps**:
1. Send transaction
2. Wait for blockchain confirmations
3. Monitor status transition from Pending to Confirmed

**Expected Result**:
- Status updates to "Confirmed" after required confirmations
- Confirmation count displayed (e.g., "3/12 confirmations")
- Transaction hash available
- Block number shown
- Status updates automatically without page refresh

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-010: Transaction Status Tracking - Completed State
**Priority**: Critical
**Prerequisites**: Fully confirmed transaction

**Steps**:
1. Send transaction
2. Wait for full confirmation (e.g., 12+ confirmations)
3. View final status

**Expected Result**:
- Status updates to "Completed"
- Success indicator (green checkmark)
- Balance updated and reflected
- Full transaction details available
- Final fee amount shown
- Success notification displayed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-011: Receive USDC - Notification
**Priority**: Critical
**Prerequisites**: Wallet exists and is active
**Test Data**:
- External wallet sends USDC to test wallet

**Steps**:
1. Have external source send USDC to wallet address
2. Wait for Circle webhook notification
3. Check wallet dashboard

**Expected Result**:
- Incoming transaction detected via Circle webhook
- Notification displayed in app
- Balance updates automatically
- Transaction appears in history with "Received" type
- Sender address displayed
- Email notification sent (if configured)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-012: View Transaction Details - Sent Transaction
**Priority**: Medium
**Prerequisites**: Completed sent transaction exists

**Steps**:
1. Navigate to transaction history
2. Click on sent transaction
3. View full details

**Expected Result**:
- Transaction type: "Sent"
- Recipient address displayed
- Amount sent
- Network fee paid
- Total cost
- Transaction hash with block explorer link
- Timestamp
- Status
- Copy buttons for hash and addresses

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-013: View Transaction Details - Received Transaction
**Priority**: Medium
**Prerequisites**: Completed received transaction exists

**Steps**:
1. Navigate to transaction history
2. Click on received transaction
3. View full details

**Expected Result**:
- Transaction type: "Received"
- Sender address displayed
- Amount received
- Transaction hash with block explorer link
- Timestamp
- Status
- Copy buttons for hash and addresses
- No fee shown (paid by sender)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-014: Transaction Failed Handling
**Priority**: Critical
**Prerequisites**: Ability to simulate failed transaction

**Steps**:
1. Initiate transaction that will fail (e.g., recipient address issues, network error)
2. Monitor transaction status
3. View failed transaction details

**Expected Result**:
- Transaction status updates to "Failed"
- Error message explains reason for failure
- Failed indicator shown (red X or warning)
- Balance not deducted (or refunded if already deducted)
- Option to retry transaction
- Support contact information provided

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-015: Transaction Retry
**Priority**: High
**Prerequisites**: Failed transaction exists

**Steps**:
1. Navigate to failed transaction
2. Click "Retry" button
3. Review pre-filled transaction details
4. Confirm retry

**Expected Result**:
- Retry option available for failed transactions
- Transaction details pre-filled from original transaction
- Can edit details before retry
- New transaction ID generated for retry
- Original failed transaction marked as "Retried"
- Link between original and retry transaction

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-016: Circle Webhook Processing - Transfer Complete
**Priority**: Critical
**Prerequisites**: System configured to receive Circle webhooks

**Steps**:
1. Initiate transfer
2. Circle processes transfer
3. Circle sends webhook notification
4. System processes webhook

**Expected Result**:
- Webhook received and validated
- Signature verification passes
- Transaction status updated in database
- User notification triggered
- Balance updated
- Transaction details stored correctly
- Idempotency handled (duplicate webhooks ignored)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-017: Fee Calculation Verification - USDC
**Priority**: High
**Prerequisites**: Wallet with USDC balance
**Test Data**:
- Amount: 10.00 USDC

**Steps**:
1. Navigate to Send page
2. Enter transaction details
3. Review fee calculation
4. Compare with expected fee

**Expected Result**:
- Network fee calculated accurately
- Fee displayed before confirmation
- Fee matches actual charged amount
- Fee explanation available (tooltip or info icon)
- Fee updates if amount changed
- Fee reasonable and within expected range

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-018: Gas Estimation - ETH Transfer
**Priority**: High
**Prerequisites**: Wallet with ETH balance

**Steps**:
1. Navigate to Send page
2. Select ETH token
3. Enter recipient address and amount
4. View gas estimation

**Expected Result**:
- Gas limit estimated accurately
- Gas price displayed (in Gwei)
- Total gas cost shown in ETH and USD
- Option to adjust gas price (if available)
- Warning if gas price very high
- Estimation updates based on network conditions

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-019: Send ETH - Success Path
**Priority**: Critical
**Prerequisites**: Wallet with sufficient ETH balance
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: 0.01 ETH

**Steps**:
1. Navigate to Send page
2. Select token: ETH
3. Enter recipient address
4. Enter amount: 0.01
5. Review transaction details
6. Confirm and send

**Expected Result**:
- Transaction initiated successfully
- ETH transfer processes on blockchain
- Gas fees deducted
- Transaction tracked through confirmation stages
- Balance updated
- All steps similar to USDC transfer (TC-TX-001)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-TX-020: Send MATIC - Success Path
**Priority**: Critical
**Prerequisites**: Wallet with sufficient MATIC balance
**Test Data**:
- Recipient Address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1
- Amount: 10.00 MATIC

**Steps**:
1. Navigate to Send page
2. Select token: MATIC
3. Enter recipient address
4. Enter amount: 10.00
5. Review transaction details
6. Confirm and send

**Expected Result**:
- Transaction initiated successfully
- MATIC transfer processes on Polygon network
- Lower gas fees compared to ETH
- Transaction tracked through confirmation stages
- Balance updated
- All steps similar to USDC transfer (TC-TX-001)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

## 4. Token Swap Tests

### TC-SWAP-001: Select Tokens - USDC to ETH
**Priority**: Critical
**Prerequisites**: Wallet with USDC balance

**Steps**:
1. Navigate to Swap page
2. Select "From Token": USDC
3. Select "To Token": ETH
4. Verify token selection

**Expected Result**:
- Token dropdowns populated with available tokens
- Selected tokens displayed with icons and symbols
- Cannot select same token for both from/to
- Token balances shown in dropdowns
- Swap direction clearly indicated

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-002: Select Tokens - USDC to MATIC
**Priority**: Critical
**Prerequisites**: Wallet with USDC balance

**Steps**:
1. Navigate to Swap page
2. Select "From Token": USDC
3. Select "To Token": MATIC

**Expected Result**:
- Tokens selected successfully
- Exchange rate displayed
- All swap functionality available for USDC/MATIC pair

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-003: Select Tokens - ETH to USDC
**Priority**: Critical
**Prerequisites**: Wallet with ETH balance

**Steps**:
1. Navigate to Swap page
2. Select "From Token": ETH
3. Select "To Token": USDC

**Expected Result**:
- Tokens selected successfully
- Reverse swap direction works
- Exchange rate displayed
- All swap functionality available

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-004: Get Swap Quote - Valid Amount
**Priority**: Critical
**Prerequisites**: USDC selected for swap to ETH
**Test Data**:
- From: 100 USDC
- To: ETH

**Steps**:
1. Navigate to Swap page
2. Select tokens: USDC → ETH
3. Enter amount: 100 USDC
4. Wait for quote

**Expected Result**:
- Quote fetched from Circle API or liquidity provider
- Estimated ETH amount displayed
- Exchange rate shown (e.g., 1 USDC = 0.00035 ETH)
- Platform fee shown (0.5% of transaction)
- Network fee estimated
- Price impact percentage displayed
- Quote expires after timeframe (e.g., 30 seconds)
- Refresh quote option available

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-005: Swap with Default Slippage (1%)
**Priority**: Critical
**Prerequisites**: Wallet with 100 USDC
**Test Data**:
- From: 100 USDC
- To: ETH
- Slippage: 1% (default)

**Steps**:
1. Navigate to Swap page
2. Configure swap: USDC → ETH, amount 100
3. Verify slippage set to default 1%
4. Get quote
5. Review and confirm swap

**Expected Result**:
- Default slippage of 1% applied
- Minimum received amount calculated (quote - 1%)
- Slippage tolerance clearly displayed
- Swap executes if price within 1% of quote
- Transaction fails if slippage exceeded

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-006: Swap with Custom Slippage (0.5%)
**Priority**: High
**Prerequisites**: Wallet with USDC balance
**Test Data**:
- From: 100 USDC
- To: ETH
- Slippage: 0.5% (custom)

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. Click slippage settings
4. Set custom slippage to 0.5%
5. Get quote and execute

**Expected Result**:
- Custom slippage accepted
- Lower slippage tolerance = higher chance of failure
- Warning shown: "Low slippage may result in failed transaction"
- Minimum received calculated with 0.5% tolerance

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-007: Swap with Custom Slippage (3%)
**Priority**: High
**Prerequisites**: Wallet with USDC balance
**Test Data**:
- From: 100 USDC
- To: ETH
- Slippage: 3% (custom)

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. Set custom slippage to 3%
4. Get quote and execute

**Expected Result**:
- Custom slippage accepted
- Higher slippage = more tolerance but worse price
- Warning shown: "High slippage may result in worse exchange rate"
- Minimum received calculated with 3% tolerance

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-008: Swap with Custom Slippage (5%)
**Priority**: Medium
**Prerequisites**: Wallet with USDC balance
**Test Data**:
- From: 100 USDC
- To: ETH
- Slippage: 5% (high)

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. Set custom slippage to 5%
4. Get quote and execute

**Expected Result**:
- Warning shown for very high slippage
- May indicate low liquidity if high slippage needed
- User must confirm understanding of high slippage
- Execution more likely to succeed but at worse price

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-009: Swap with High Price Impact Warning
**Priority**: High
**Prerequisites**: Wallet with large amount relative to liquidity pool
**Test Data**:
- From: 10,000 USDC (large amount)
- To: ETH

**Steps**:
1. Navigate to Swap page
2. Enter large swap amount
3. Get quote
4. Review price impact

**Expected Result**:
- Price impact calculated and displayed
- Warning shown if impact > 5%: "High price impact! Consider reducing amount"
- User must acknowledge warning to proceed
- Explanation of price impact provided
- Suggested maximum amount for low impact

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-010: Execute Swap Successfully
**Priority**: Critical
**Prerequisites**: Wallet with 100 USDC
**Test Data**:
- From: 100 USDC
- To: ETH
- Slippage: 1%

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. Get quote
4. Click "Swap" button
5. Confirm in confirmation modal
6. Wait for execution

**Expected Result**:
- Swap initiated via Circle API or DEX
- Transaction ID generated
- Status tracked: Pending → Confirming → Completed
- USDC deducted from wallet
- ETH added to wallet
- Platform fee (0.5%) deducted
- Network fees applied
- Swap appears in transaction history
- Success notification displayed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-011: Execute Swap - Insufficient Balance
**Priority**: Critical
**Prerequisites**: Wallet with 50 USDC
**Test Data**:
- From: 100 USDC (more than balance)
- To: ETH

**Steps**:
1. Navigate to Swap page
2. Select USDC → ETH
3. Enter amount: 100 USDC
4. Attempt to get quote

**Expected Result**:
- Error message: "Insufficient USDC balance. Available: 50 USDC"
- Swap button disabled
- Cannot proceed with swap
- Available balance prominently displayed
- Option to adjust amount

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-012: Swap Amount Validation - Minimum
**Priority**: High
**Prerequisites**: Wallet with USDC
**Test Data**:
- From: 0.01 USDC (below minimum)
- To: ETH

**Steps**:
1. Navigate to Swap page
2. Enter very small amount
3. Attempt to get quote

**Expected Result**:
- Minimum swap amount enforced (e.g., $1 or $5)
- Error message: "Minimum swap amount is $1 USDC"
- Swap button disabled
- Minimum amount clearly stated

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-013: Swap Amount Validation - Maximum
**Priority**: High
**Prerequisites**: Wallet with large USDC balance
**Test Data**:
- From: 1,000,000 USDC (if maximum limit exists)
- To: ETH

**Steps**:
1. Navigate to Swap page
2. Enter very large amount
3. Attempt to get quote

**Expected Result**:
- If maximum limit exists, enforce it
- Error message: "Maximum swap amount is $X USDC"
- OR high price impact warning shown
- Liquidity limitations explained

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-014: Exchange Rate Display
**Priority**: Medium
**Prerequisites**: Any swap configured

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. View exchange rate display

**Expected Result**:
- Exchange rate shown: "1 USDC = 0.00035 ETH"
- Reverse rate also available: "1 ETH = 2857.14 USDC"
- Toggle between rate directions
- Rate updates in real-time
- Rate includes all fees
- Rate expiration timer shown

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-015: Fee Breakdown Verification - Platform Fee
**Priority**: Critical
**Prerequisites**: Swap configured
**Test Data**:
- From: 100 USDC
- To: ETH

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. Get quote
4. Review fee breakdown

**Expected Result**:
- Platform fee: 0.5% of swap amount = 0.50 USDC
- Fee displayed separately from amount
- Fee included in total cost calculation
- Fee explanation available
- Fee breakdown itemized:
  - Platform fee: 0.50 USDC
  - Network fee: X USDC
  - Total cost: 100.50 + X USDC

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-016: Network Fee Estimation
**Priority**: High
**Prerequisites**: Swap configured

**Steps**:
1. Navigate to Swap page
2. Configure swap
3. Get quote
4. Review network fee

**Expected Result**:
- Network/gas fee estimated accurately
- Fee shown in token and USD
- Fee updates based on network conditions
- Gas price (Gwei) displayed
- Warning if fees unusually high

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-017: Swap Confirmation Modal
**Priority**: High
**Prerequisites**: Quote obtained

**Steps**:
1. Navigate to Swap page
2. Configure swap and get quote
3. Click "Swap" button
4. Review confirmation modal

**Expected Result**:
- Confirmation modal displays:
  - From amount and token
  - To estimated amount and token
  - Exchange rate
  - Platform fee (0.5%)
  - Network fee
  - Total cost
  - Minimum received (with slippage)
  - Price impact %
  - Slippage tolerance
- "Confirm Swap" and "Cancel" buttons
- Can cancel and edit
- Quote expiration timer

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-018: Swap Status Tracking - Pending
**Priority**: Critical
**Prerequisites**: Swap initiated

**Steps**:
1. Execute swap
2. Immediately view status
3. Monitor progress

**Expected Result**:
- Status shows "Pending"
- Progress indicator displayed
- Estimated completion time shown
- Can view transaction details
- Transaction appears in history immediately

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-019: Swap Status Tracking - Completed
**Priority**: Critical
**Prerequisites**: Swap in progress

**Steps**:
1. Execute swap
2. Wait for completion
3. View final status

**Expected Result**:
- Status updates to "Completed"
- Success notification displayed
- Balances updated correctly
- Actual received amount shown
- Final exchange rate displayed
- All fees reconciled

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-020: View Swap History
**Priority**: High
**Prerequisites**: Multiple completed swaps exist

**Steps**:
1. Navigate to Swap History page (or Transaction History)
2. Filter by "Swap" type
3. View swap list

**Expected Result**:
- All swaps displayed in reverse chronological order
- Each swap shows:
  - From token and amount
  - To token and amount
  - Exchange rate
  - Timestamp
  - Status
  - Transaction ID
- Pagination if many swaps
- Can filter/sort swaps

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-021: Filter Swap History - By Status
**Priority**: Medium
**Prerequisites**: Swaps with various statuses

**Steps**:
1. Navigate to Swap History
2. Apply status filter
3. Select "Completed" status

**Expected Result**:
- Only completed swaps shown
- Pending and failed swaps hidden
- Filter can be cleared
- Count of filtered results shown

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-022: Filter Swap History - By Tokens
**Priority**: Medium
**Prerequisites**: Swaps involving different token pairs

**Steps**:
1. Navigate to Swap History
2. Apply token filter
3. Select "USDC → ETH" pair

**Expected Result**:
- Only USDC to ETH swaps shown
- Other token pairs hidden
- Can select multiple token pairs
- Filter persists during session

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-023: Filter Swap History - By Date
**Priority**: Medium
**Prerequisites**: Swaps spanning multiple dates

**Steps**:
1. Navigate to Swap History
2. Select date range filter
3. Choose last 7 days

**Expected Result**:
- Only swaps from last 7 days shown
- Older swaps hidden
- Date range picker works correctly
- Can select custom date ranges

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-024: View Swap Details
**Priority**: Medium
**Prerequisites**: Completed swap exists

**Steps**:
1. Navigate to Swap History
2. Click on specific swap
3. View detailed information

**Expected Result**:
- Swap details modal/page displays:
  - Transaction ID
  - From amount and token
  - To amount and token
  - Exchange rate
  - Platform fee charged
  - Network fee charged
  - Total cost
  - Actual received amount
  - Timestamp
  - Status
  - Block explorer link
- Copy buttons for transaction details

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-SWAP-025: Failed Swap Handling and Retry
**Priority**: High
**Prerequisites**: Ability to trigger failed swap (e.g., slippage exceeded)

**Steps**:
1. Execute swap with very low slippage
2. Wait for execution during high volatility
3. Swap fails due to slippage exceeded
4. View failed swap
5. Click "Retry" option

**Expected Result**:
- Swap fails gracefully
- Error message explains failure reason
- Balance not deducted (or refunded)
- Failed status shown in history
- "Retry" button available
- Retry pre-fills with original swap details
- Can adjust slippage before retry
- New transaction ID for retry

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

## 5. Exchange Investment Tests

### TC-INVEST-001: Connect WhiteBit - Valid Credentials
**Priority**: Critical
**Prerequisites**: User has WhiteBit account with API access
**Test Data**:
- API Key: valid_api_key
- API Secret: valid_api_secret

**Steps**:
1. Navigate to Exchange Integration page
2. Select WhiteBit exchange
3. Click "Connect WhiteBit"
4. Enter valid API key
5. Enter valid API secret
6. Click "Connect"

**Expected Result**:
- API credentials validated
- Connection established successfully
- Success message: "WhiteBit connected successfully"
- Connection status shows "Connected"
- User redirected to investment dashboard
- WhiteBit balance available
- Investment plans loaded

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-002: Connect WhiteBit - Invalid Credentials
**Priority**: Critical
**Prerequisites**: None
**Test Data**:
- API Key: invalid_api_key
- API Secret: invalid_api_secret

**Steps**:
1. Navigate to Exchange Integration page
2. Click "Connect WhiteBit"
3. Enter invalid API key
4. Enter invalid API secret
5. Click "Connect"

**Expected Result**:
- Connection fails
- Error message: "Invalid API credentials. Please check your API key and secret"
- Connection status remains "Not Connected"
- Can retry with correct credentials
- Help link to WhiteBit API setup guide

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-003: View WhiteBit Connection Status
**Priority**: Medium
**Prerequisites**: WhiteBit connected

**Steps**:
1. Navigate to Exchange Integration page
2. View connection status section

**Expected Result**:
- Connection status clearly displayed: "Connected" with green indicator
- Connected account email/username shown
- Last sync timestamp displayed
- Options to "Sync Now" or "Disconnect"
- Connection health indicator

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-004: View Available Investment Plans
**Priority**: Critical
**Prerequisites**: WhiteBit connected

**Steps**:
1. Navigate to Investment Plans page
2. View all available plans

**Expected Result**:
- All WhiteBit Codes/investment plans displayed
- Each plan shows:
  - Plan name
  - Token/asset
  - APY (Annual Percentage Yield)
  - Term/duration (e.g., 30 days, 90 days, flexible)
  - Minimum investment amount
  - Maximum investment amount (if applicable)
  - Risk level
  - Description
- Plans displayed in cards or table format
- Can compare multiple plans

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-005: Filter Plans - By APY
**Priority**: Medium
**Prerequisites**: Multiple investment plans available

**Steps**:
1. Navigate to Investment Plans page
2. Apply APY filter
3. Select "High APY" (e.g., > 10%)

**Expected Result**:
- Plans filtered by APY criteria
- Only high APY plans shown
- Sort option: Low to High, High to Low
- Filter count shown
- Can clear filter

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-006: Filter Plans - By Term
**Priority**: Medium
**Prerequisites**: Plans with various terms available

**Steps**:
1. Navigate to Investment Plans page
2. Apply term filter
3. Select "Short term" (e.g., 30 days)

**Expected Result**:
- Plans filtered by term length
- Only 30-day plans shown
- Term categories: Flexible, Short (30d), Medium (90d), Long (180d+)
- Multiple term selections allowed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-007: Create Investment - Valid Amount
**Priority**: Critical
**Prerequisites**:
- WhiteBit connected
- Sufficient USDC balance on WhiteBit: 100 USDC
**Test Data**:
- Plan: USDC Flexible Savings
- Amount: 50 USDC

**Steps**:
1. Navigate to Investment Plans
2. Select specific plan
3. Click "Invest"
4. Enter amount: 50 USDC
5. Review investment details
6. Confirm investment

**Expected Result**:
- Investment created successfully via WhiteBit API
- Investment position ID generated
- Amount deducted from WhiteBit balance
- Position appears in "My Investments"
- Initial position details:
  - Invested amount: 50 USDC
  - APY: (as per plan)
  - Start date: Current date
  - Maturity date: (based on term)
  - Current value: 50 USDC
  - Accrued rewards: 0 USDC
- Success notification displayed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-008: Create Investment - Insufficient Balance
**Priority**: Critical
**Prerequisites**: WhiteBit balance: 30 USDC
**Test Data**:
- Plan: USDC Flexible Savings
- Amount: 50 USDC (more than balance)

**Steps**:
1. Navigate to Investment Plans
2. Select plan
3. Click "Invest"
4. Enter amount: 50 USDC
5. Attempt to confirm

**Expected Result**:
- Investment blocked
- Error message: "Insufficient balance on WhiteBit. Available: 30 USDC"
- Invest button disabled
- Option to deposit more funds
- Link to transfer from main wallet to WhiteBit

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-009: View Investment Positions
**Priority**: Critical
**Prerequisites**: At least one active investment exists

**Steps**:
1. Navigate to "My Investments" page
2. View all positions

**Expected Result**:
- All investment positions displayed
- Each position shows:
  - Position ID
  - Plan name
  - Invested amount
  - Current value
  - APY
  - Accrued rewards
  - Start date
  - Maturity date (if applicable)
  - Status (Active/Matured/Withdrawn)
  - Days remaining (for fixed terms)
- Positions sorted by status/date
- Total invested amount summarized
- Total current value shown

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-010: View Position Details
**Priority**: High
**Prerequisites**: Active investment position exists

**Steps**:
1. Navigate to My Investments
2. Click on specific position
3. View detailed information

**Expected Result**:
- Position details page/modal displays:
  - Full position information
  - Investment timeline visualization
  - Reward accrual graph
  - Transaction history for this position
  - APY calculation breakdown
  - Early withdrawal penalties (if applicable)
  - Terms and conditions
- Options to "Withdraw" or "Add More"

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-011: Track Reward Accrual
**Priority**: High
**Prerequisites**: Investment position active for at least 1 day

**Steps**:
1. Create investment
2. Wait 24 hours
3. View position details
4. Check accrued rewards

**Expected Result**:
- Rewards accrue according to APY
- Daily reward calculation: (Amount × APY) / 365
- Accrued rewards displayed and updated
- Rewards update automatically (daily or real-time)
- Historical reward accrual viewable
- Graph showing reward growth over time

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-012: Withdraw from Position - Active
**Priority**: Critical
**Prerequisites**: Active investment position
**Test Data**:
- Position: 50 USDC USDC Flexible Savings (flexible term)
- Accrued rewards: 0.15 USDC

**Steps**:
1. Navigate to My Investments
2. Select active position
3. Click "Withdraw"
4. Review withdrawal details
5. Confirm withdrawal

**Expected Result**:
- For flexible plans: Withdrawal allowed anytime
- Withdrawal amount: Principal + Accrued Rewards = 50.15 USDC
- Withdrawal processed via WhiteBit API
- Amount returned to WhiteBit balance
- Position status updated to "Withdrawn"
- Position appears in history
- Success notification displayed

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-013: Withdraw from Position - Matured
**Priority**: Critical
**Prerequisites**: Investment position reached maturity date
**Test Data**:
- Position: 100 USDC 30-day Fixed (matured)
- Accrued rewards: 2.50 USDC

**Steps**:
1. Navigate to My Investments
2. Select matured position
3. Click "Withdraw"
4. Confirm withdrawal

**Expected Result**:
- Matured position clearly indicated
- Full withdrawal available: 102.50 USDC
- No early withdrawal penalties
- Withdrawal processed successfully
- Funds returned to WhiteBit balance
- Position status: "Completed"
- Option to reinvest in same or different plan

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-014: Investment History
**Priority**: Medium
**Prerequisites**: Multiple investment positions (active and completed)

**Steps**:
1. Navigate to Investment History page
2. View all investment activities

**Expected Result**:
- All investment activities displayed:
  - New investments
  - Withdrawals
  - Reward distributions
  - Reinvestments
- Each activity shows:
  - Type
  - Amount
  - Plan
  - Date
  - Status
- Filterable by type, date, plan
- Sortable by date, amount
- Exportable to CSV

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-015: Position Synchronization
**Priority**: High
**Prerequisites**: Active investment on WhiteBit

**Steps**:
1. Create investment via CoinPay
2. Manually verify position on WhiteBit exchange
3. Trigger sync in CoinPay (manual or automatic)
4. Compare position details

**Expected Result**:
- Position data syncs accurately from WhiteBit
- Balance matches WhiteBit
- Rewards match WhiteBit calculations
- Status synchronized
- Sync timestamp updated
- Any discrepancies logged and flagged
- Manual "Sync Now" option available

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-016: Reward Calculation Accuracy
**Priority**: Critical
**Prerequisites**: Investment position with known APY
**Test Data**:
- Amount: 100 USDC
- APY: 10%
- Term: 30 days

**Steps**:
1. Create investment
2. Track rewards over 30 days
3. Calculate expected rewards
4. Compare with actual rewards

**Expected Result**:
- Expected reward calculation:
  - Daily rate = 10% / 365 = 0.0274%
  - Daily reward = 100 × 0.000274 = 0.0274 USDC
  - 30-day reward = 0.0274 × 30 = 0.822 USDC
- Actual rewards match expected (within rounding tolerance)
- Compound interest applied if specified
- All calculations transparent and verifiable

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-017: Disconnect WhiteBit
**Priority**: High
**Prerequisites**: WhiteBit currently connected

**Steps**:
1. Navigate to Exchange Integration page
2. Click "Disconnect WhiteBit"
3. Confirm disconnection

**Expected Result**:
- Warning shown if active positions exist
- API credentials removed from system
- Connection status updated to "Not Connected"
- Investment plans no longer accessible
- Existing position history preserved
- Cannot create new investments
- Can reconnect anytime

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-018: Reconnect WhiteBit
**Priority**: Medium
**Prerequisites**: WhiteBit previously connected and disconnected

**Steps**:
1. Navigate to Exchange Integration
2. Click "Connect WhiteBit"
3. Enter credentials
4. Reconnect

**Expected Result**:
- Reconnection successful with valid credentials
- Previous investment history restored
- Active positions synchronized
- Can resume investment activities
- Historical data intact

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-019: Multiple Investment Positions
**Priority**: Medium
**Prerequisites**: WhiteBit connected with sufficient balance

**Steps**:
1. Create first investment (Plan A, 50 USDC)
2. Create second investment (Plan B, 30 USDC)
3. Create third investment (Plan A, 20 USDC)
4. View all positions

**Expected Result**:
- Multiple positions created successfully
- Each position tracked independently
- Can have multiple positions in same plan
- Positions clearly differentiated
- Total invested amount aggregated
- Portfolio summary shows diversification

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INVEST-020: Investment Notifications
**Priority**: Medium
**Prerequisites**: Active investment position

**Steps**:
1. Create investment
2. Wait for position events (rewards, maturity)
3. Check notifications

**Expected Result**:
- Notifications sent for:
  - Investment created
  - Daily/weekly reward summary
  - Position maturity approaching (e.g., 7 days before)
  - Position matured
  - Withdrawal completed
- Notifications via:
  - In-app notifications
  - Email (if configured)
  - Push notifications (if mobile app)
- Notification preferences configurable

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

## 6. Cross-Phase Integration Tests

### TC-INTEGRATION-001: Complete User Journey - Onboarding to First Transaction
**Priority**: Critical
**Prerequisites**: New user, no existing account

**Steps**:
1. **Register**: Create new account with valid credentials
2. **Verify Email**: Confirm email address
3. **Login**: Authenticate with new credentials
4. **Create Wallet**: Generate new wallet via Circle
5. **Receive Funds**: External source sends 100 USDC to wallet
6. **View Balance**: Check updated balance shows 100 USDC
7. **Send Transaction**: Send 10 USDC to another address
8. **View History**: Verify both receive and send transactions in history

**Expected Result**:
- Complete flow executes without errors
- Each phase transitions smoothly to next
- All data persists correctly across phases
- User can complete full journey in one session
- Transaction history shows complete audit trail
- Balance updates accurately throughout

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-002: Receive → Swap → Send Flow
**Priority**: Critical
**Prerequisites**: Wallet exists with 0 balance

**Steps**:
1. **Receive**: External source sends 100 USDC
2. **Wait for Confirmation**: Transaction confirms
3. **Verify Balance**: 100 USDC shown
4. **Swap**: Exchange 50 USDC for ETH
5. **Wait for Swap**: Swap completes
6. **Verify Balances**: 50 USDC and ~0.017 ETH (example)
7. **Send**: Send 0.01 ETH to external address
8. **Verify Final Balance**: ~50 USDC and ~0.007 ETH remaining

**Expected Result**:
- All operations complete successfully
- Balance calculations accurate at each step
- Transaction history shows all operations in order:
  - Receive USDC
  - Swap USDC → ETH
  - Send ETH
- Fees properly accounted for
- No balance discrepancies

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-003: Receive → Invest → Withdraw Flow
**Priority**: Critical
**Prerequisites**:
- Wallet exists
- WhiteBit connected

**Steps**:
1. **Receive**: Receive 100 USDC to wallet
2. **Transfer to WhiteBit**: Transfer USDC to WhiteBit exchange (manual or via integration)
3. **Create Investment**: Invest 50 USDC in flexible savings plan
4. **Wait**: Allow 7 days for reward accrual
5. **Check Rewards**: Verify rewards accumulated (~0.096 USDC at 10% APY)
6. **Withdraw**: Withdraw full position (50.096 USDC)
7. **Transfer Back**: Transfer funds back to main wallet
8. **Verify Balance**: Check final balance

**Expected Result**:
- Complete investment cycle works end-to-end
- Funds tracked across all platforms:
  - CoinPay wallet
  - WhiteBit exchange
  - Investment position
- Rewards calculated accurately
- Withdrawal successful
- Balance reconciliation correct
- Transaction history complete across all systems

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-004: Multiple Operations in Sequence
**Priority**: High
**Prerequisites**: Wallet with 200 USDC balance

**Steps**:
1. **Send**: Send 20 USDC to Address A
2. **Swap**: Swap 50 USDC to MATIC
3. **Send**: Send 10 MATIC to Address B
4. **Swap**: Swap 30 USDC to ETH
5. **Receive**: Receive 15 USDC from external source
6. **Verify Balances**: Check all token balances
7. **Verify History**: Review complete transaction history

**Expected Result**:
- All operations execute in sequence
- No interference between operations
- Balances update correctly after each operation:
  - After step 1: ~180 USDC
  - After step 2: ~130 USDC, 50 MATIC
  - After step 3: ~130 USDC, 40 MATIC
  - After step 4: ~100 USDC, 40 MATIC, ~0.010 ETH
  - After step 5: ~115 USDC, 40 MATIC, ~0.010 ETH
- Transaction history shows all 6 operations
- All transactions have correct timestamps and status

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-005: Error Recovery - Failed Send Then Successful Swap
**Priority**: High
**Prerequisites**: Wallet with 50 USDC

**Steps**:
1. **Attempt Send**: Try to send 100 USDC (insufficient balance)
2. **Verify Error**: Confirm error message displayed
3. **Verify Balance**: Balance unchanged (still 50 USDC)
4. **Swap**: Successfully swap 25 USDC to ETH
5. **Verify Balances**: 25 USDC and ETH balance updated
6. **Retry Send**: Send 10 USDC successfully

**Expected Result**:
- Failed operation doesn't affect subsequent operations
- Error handling doesn't leave system in bad state
- User can recover from error and continue
- Failed transaction logged but doesn't corrupt data
- Successful operations proceed normally after failure

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-006: Session Continuity Across Operations
**Priority**: Medium
**Prerequisites**: User logged in

**Steps**:
1. **Start**: Login and view dashboard
2. **Send Transaction**: Initiate send (15 minutes)
3. **View Balance**: Check balance during pending transaction
4. **Start Swap**: Begin swap while send is pending
5. **Check History**: View history showing both operations
6. **Wait**: Both operations complete
7. **Verify**: All balances and history correct

**Expected Result**:
- Multiple pending operations supported
- Session remains active throughout
- Real-time updates for all operations
- No token refresh interruption
- UI responsive during multiple operations
- Correct final state after all operations complete

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-007: Cross-Token Operations - USDC, ETH, MATIC
**Priority**: High
**Prerequisites**: Wallet with balances in all 3 tokens

**Steps**:
1. **Initial State**:
   - 100 USDC
   - 0.05 ETH
   - 50 MATIC
2. **Operation 1**: Send 20 USDC
3. **Operation 2**: Send 0.01 ETH
4. **Operation 3**: Send 10 MATIC
5. **Operation 4**: Swap 30 USDC to ETH
6. **Verify**: All balances correct, all fees applied correctly

**Expected Result**:
- Operations across different tokens work independently
- Balance tracking accurate for each token
- Fees calculated correctly per token/network
- Transaction history shows all tokens correctly
- Final balances:
  - ~50 USDC (100 - 20 - 30 - fees)
  - ~0.04 + swap result ETH (0.05 - 0.01 + from swap - fees)
  - ~40 MATIC (50 - 10 - fees)

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-008: Logout/Login During Pending Operations
**Priority**: High
**Prerequisites**: Wallet with funds

**Steps**:
1. **Initiate Swap**: Start swap operation
2. **Immediate Logout**: Logout before swap completes
3. **Wait**: Allow time for swap to complete (1-2 minutes)
4. **Login**: Login again
5. **Check Status**: View swap status and balance

**Expected Result**:
- Swap continues processing server-side
- After re-login, swap status shown correctly
- If completed during logout: Balance updated, history shows completed swap
- If still pending: Status still shows pending
- No data loss from logout
- User can pick up where they left off

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-009: Investment + Wallet Operations Parallel
**Priority**: Medium
**Prerequisites**:
- Wallet with funds
- WhiteBit connected
- Active investment

**Steps**:
1. **Create Investment**: Invest 50 USDC on WhiteBit
2. **Wallet Send**: Send 10 USDC from main wallet
3. **Wallet Swap**: Swap 20 USDC to ETH in main wallet
4. **Check Investment**: Verify investment unaffected
5. **Withdraw Investment**: Withdraw investment
6. **Verify All Balances**: Check wallet and WhiteBit balances

**Expected Result**:
- Wallet operations and investment operations independent
- Both systems track balances separately
- No interference between operations
- Each system maintains data integrity
- Complete audit trail across both systems

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

### TC-INTEGRATION-010: Complete Platform Stress Test
**Priority**: High
**Prerequisites**: Fully funded wallet and WhiteBit connection

**Steps**:
1. **Rapid Operations**: Execute 20 operations in quick succession:
   - 5 sends (various amounts and tokens)
   - 5 swaps (various pairs)
   - 5 balance refreshes
   - 3 investment creates
   - 2 investment withdrawals
2. **Monitor**: Watch all operations process
3. **Verify**: Check all final states

**Expected Result**:
- System handles rapid operations gracefully
- No race conditions or data corruption
- All operations complete successfully or fail gracefully
- Operation queue managed properly
- Rate limiting applied if necessary (with user notification)
- All balances reconcile correctly at end
- Complete transaction history maintained
- No system errors or crashes

**Actual Result**: [To be filled during testing]
**Status**: ⬜ Not Run | ⬜ Pass | ⬜ Fail | ⬜ Blocked

---

## Test Execution Summary

### Total Test Cases by Category
| Category | Test Cases | Critical | High | Medium | Low |
|----------|-----------|----------|------|--------|-----|
| Authentication Flow | 15 | 6 | 7 | 2 | 0 |
| Wallet Operations | 12 | 3 | 4 | 4 | 1 |
| Send/Receive Transactions | 20 | 12 | 6 | 2 | 0 |
| Token Swaps | 25 | 8 | 10 | 7 | 0 |
| Exchange Investments | 20 | 6 | 7 | 7 | 0 |
| Cross-Phase Integration | 10 | 5 | 4 | 1 | 0 |
| **TOTAL** | **102** | **40** | **38** | **23** | **1** |

### Test Execution Tracking

**Status Legend**:
- ⬜ Not Run: Test has not been executed
- ✅ Pass: Test passed successfully
- ❌ Fail: Test failed, bug identified
- ⏸️ Blocked: Test cannot be executed due to blocker

### Test Execution Checklist

#### Sprint N06 - Phase 1: Authentication & Wallet (27 tests)
- [ ] TC-AUTH-001 through TC-AUTH-015 (15 tests)
- [ ] TC-WALLET-001 through TC-WALLET-012 (12 tests)

#### Sprint N06 - Phase 2: Transactions (20 tests)
- [ ] TC-TX-001 through TC-TX-020 (20 tests)

#### Sprint N06 - Phase 3: Swaps (25 tests)
- [ ] TC-SWAP-001 through TC-SWAP-025 (25 tests)

#### Sprint N06 - Phase 4: Investments (20 tests)
- [ ] TC-INVEST-001 through TC-INVEST-020 (20 tests)

#### Sprint N06 - Phase 5: Integration (10 tests)
- [ ] TC-INTEGRATION-001 through TC-INTEGRATION-010 (10 tests)

---

## Bug Tracking Template

When test failures occur, log bugs using this template:

```
Bug ID: BUG-XXX
Related Test Case: TC-XXX
Severity: Critical/High/Medium/Low
Category: Frontend/Backend/Integration/Security

Title: [Brief description]
Description: [Detailed bug description]
Steps to Reproduce: [From test case]
Expected Result: [From test case]
Actual Result: [What actually happened]
Environment: [Browser, OS, version]
Screenshots: [If applicable]
Assigned To: [Developer agent]
Status: Open/In Progress/Resolved/Closed
```

---

## Testing Notes

### Test Data Management
- Use separate test accounts for different test scenarios
- Maintain test wallets with known balances
- Document all test API keys and credentials securely
- Reset test data between test runs when necessary

### Environment Requirements
- **Frontend**: Latest Chrome, Firefox, Safari, Edge browsers
- **Backend**: API endpoints accessible and functioning
- **Circle API**: Sandbox environment configured
- **WhiteBit API**: Test account with API access
- **Database**: Test database isolated from production

### Automation Considerations
- Critical path tests (40 Critical priority tests) should be automated first
- Use Playwright for UI automation
- Use API testing for backend endpoints
- Implement CI/CD integration for automated test runs

### Sign-Off Criteria
- All Critical priority tests: 100% pass rate
- All High priority tests: 95% pass rate minimum
- All Medium/Low priority tests: 90% pass rate minimum
- All identified bugs logged and prioritized
- No Critical severity bugs remaining
- Production deployment approved by QA

---

**Document End**

*This E2E test documentation provides comprehensive coverage of CoinPay's core functionality across all phases. Execute tests systematically, document all findings, and ensure quality standards are met before production deployment.*
