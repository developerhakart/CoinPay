# QA-203: Phase 2 Functional Testing

**Test Owner**: QA Engineer 1
**Effort**: 4.00 days
**Status**: Ready for Execution
**Date Created**: 2025-10-29

---

## Test Objectives

Validate Phase 2 advanced functionality:
- Merchant Dashboard features
- Customer Dashboard features
- Multi-currency support
- Recurring payments
- Dispute resolution
- Transaction export and reporting

---

## Test Environment

**Frontend**: http://localhost:3000
**Backend API**: http://localhost:5000
**Network**: Polygon Amoy Testnet
**Test Data**: Merchant and customer test accounts

---

## Test Suite 1: Merchant Dashboard (12 test cases)

### TC-MERCHANT-001: Merchant Dashboard Access
**Priority**: Critical
**Preconditions**: Merchant account registered

**Steps**:
1. Login as merchant user
2. Navigate to /merchant/dashboard
3. Verify dashboard loads

**Expected Result**:
- ✅ Merchant dashboard accessible
- ✅ Overview widgets displayed (total revenue, transactions today, pending payments)
- ✅ Quick action buttons visible
- ✅ Recent transactions list shown

**Status**: ⏳ Pending

---

### TC-MERCHANT-002: Transaction Monitoring
**Priority**: Critical
**Preconditions**: Merchant with transaction history

**Steps**:
1. Navigate to merchant dashboard
2. View "Recent Transactions" section
3. Verify transaction details

**Expected Result**:
- ✅ Transactions displayed in reverse chronological order
- ✅ Shows: Customer name, amount, status, timestamp
- ✅ Status badges color-coded (Pending=yellow, Completed=green, Failed=red)
- ✅ Real-time updates for new transactions
- ✅ Pagination available for large datasets

**Status**: ⏳ Pending

---

### TC-MERCHANT-003: Transaction Filtering
**Priority**: High
**Preconditions**: Merchant with multiple transactions

**Steps**:
1. Navigate to /merchant/transactions
2. Apply status filter: "Completed"
3. Apply date range filter: Last 7 days
4. Apply amount range filter: $10 - $100

**Expected Result**:
- ✅ Transactions filtered correctly
- ✅ Multiple filters can be applied simultaneously
- ✅ Filter chips displayed showing active filters
- ✅ "Clear all filters" button works
- ✅ Filtered count displayed

**Status**: ⏳ Pending

---

### TC-MERCHANT-004: Transaction Search
**Priority**: High
**Preconditions**: Merchant with transactions

**Steps**:
1. Navigate to /merchant/transactions
2. Search by customer name: "John Doe"
3. Search by transaction ID
4. Search by amount

**Expected Result**:
- ✅ Search works across customer name, transaction ID, amount
- ✅ Results update in real-time as user types
- ✅ No results state shown when appropriate
- ✅ Search is case-insensitive

**Status**: ⏳ Pending

---

### TC-MERCHANT-005: Analytics Dashboard
**Priority**: High
**Preconditions**: Merchant with 30+ days of transaction data

**Steps**:
1. Navigate to /merchant/analytics
2. View revenue chart
3. Select different time ranges (7 days, 30 days, 90 days)
4. View transaction volume chart

**Expected Result**:
- ✅ Revenue chart displays correctly
- ✅ Transaction volume chart shows accurate data
- ✅ Time range selector works
- ✅ Charts update smoothly
- ✅ Tooltips show exact values on hover
- ✅ Total revenue, average transaction, and count displayed

**Status**: ⏳ Pending

---

### TC-MERCHANT-006: Export Transactions to CSV
**Priority**: High
**Preconditions**: Merchant with transactions

**Steps**:
1. Navigate to /merchant/transactions
2. Select date range for export
3. Click "Export to CSV" button
4. Verify file download

**Expected Result**:
- ✅ CSV file downloads successfully
- ✅ Filename includes date range: `transactions_2025-10-01_2025-10-29.csv`
- ✅ CSV contains all columns: Date, Customer, Amount, Currency, Status, Transaction ID
- ✅ Data matches displayed transactions
- ✅ Proper CSV formatting (quoted fields, comma-separated)

**Status**: ⏳ Pending

---

### TC-MERCHANT-007: Export Transactions to PDF
**Priority**: Medium
**Preconditions**: Merchant with transactions

**Steps**:
1. Navigate to /merchant/transactions
2. Select transactions for export
3. Click "Export to PDF" button
4. Verify PDF download

**Expected Result**:
- ✅ PDF file downloads successfully
- ✅ PDF includes merchant branding/logo
- ✅ PDF contains transaction summary table
- ✅ PDF includes totals and date range
- ✅ Professional formatting

**Status**: ⏳ Pending

---

### TC-MERCHANT-008: Payment Link Generation
**Priority**: Critical
**Preconditions**: Merchant logged in

**Steps**:
1. Navigate to /merchant/payment-links
2. Click "Create Payment Link"
3. Enter amount: 50.00 USDC
4. Enter description: "Product purchase"
5. Set expiry: 24 hours
6. Generate link

**Expected Result**:
- ✅ Payment link generated successfully
- ✅ Link format: `https://coinpay.app/pay/{unique-id}`
- ✅ Link is copyable
- ✅ QR code generated for link
- ✅ Link expiry time displayed
- ✅ Link can be shared via email/SMS

**Status**: ⏳ Pending

---

### TC-MERCHANT-009: Payment Link Usage Tracking
**Priority**: High
**Preconditions**: Payment link created

**Steps**:
1. Create payment link
2. Share link with customer (simulate)
3. Customer opens link
4. Monitor payment link status

**Expected Result**:
- ✅ Link shows "Not Paid" status initially
- ✅ After customer opens, status shows "Viewed"
- ✅ After payment, status shows "Paid"
- ✅ View count tracked
- ✅ Payment timestamp recorded

**Status**: ⏳ Pending

---

### TC-MERCHANT-010: Refund Processing
**Priority**: Critical
**Preconditions**: Completed transaction exists

**Steps**:
1. Navigate to transaction details
2. Click "Issue Refund" button
3. Select refund type: Full or Partial
4. If partial, enter amount: 25.00 USDC
5. Enter reason: "Customer request"
6. Confirm refund

**Expected Result**:
- ✅ Refund confirmation dialog appears
- ✅ Refund processed successfully
- ✅ Transaction status updates to "Refunded"
- ✅ Refund appears in customer's transaction history
- ✅ Customer balance updated
- ✅ Merchant balance debited
- ✅ Refund transaction ID generated

**Status**: ⏳ Pending

---

### TC-MERCHANT-011: Merchant Profile Settings
**Priority**: Medium
**Preconditions**: Merchant logged in

**Steps**:
1. Navigate to /merchant/settings
2. Update business name
3. Update notification preferences
4. Update payout settings
5. Save changes

**Expected Result**:
- ✅ All settings editable
- ✅ Changes saved successfully
- ✅ Success message displayed
- ✅ Settings persist after page refresh
- ✅ Validation for required fields

**Status**: ⏳ Pending

---

### TC-MERCHANT-012: Webhook Configuration
**Priority**: Medium
**Preconditions**: Merchant logged in

**Steps**:
1. Navigate to /merchant/webhooks
2. Add webhook URL: https://merchant-api.com/webhook
3. Select events: payment.completed, payment.failed
4. Save webhook
5. Test webhook with "Send Test Event"

**Expected Result**:
- ✅ Webhook URL saved
- ✅ Events selectable
- ✅ Test event sent successfully
- ✅ Webhook secret key generated
- ✅ Webhook logs displayed
- ✅ Retry configuration available

**Status**: ⏳ Pending

---

## Test Suite 2: Customer Dashboard (8 test cases)

### TC-CUSTOMER-001: Customer Dashboard Access
**Priority**: Critical
**Preconditions**: Customer account registered

**Steps**:
1. Login as customer
2. Navigate to /customer/dashboard
3. Verify dashboard loads

**Expected Result**:
- ✅ Customer dashboard accessible
- ✅ Balance displayed
- ✅ Recent payments shown
- ✅ Quick actions available (Send Money, Request Payment)

**Status**: ⏳ Pending

---

### TC-CUSTOMER-002: Payment History View
**Priority**: High
**Preconditions**: Customer with payment history

**Steps**:
1. Navigate to /customer/payments
2. View payment history
3. Click on a payment for details

**Expected Result**:
- ✅ All payments listed chronologically
- ✅ Shows: Merchant name, amount, date, status
- ✅ Payment detail modal opens
- ✅ Receipt download option available
- ✅ Dispute option available for completed payments

**Status**: ⏳ Pending

---

### TC-CUSTOMER-003: Receipt Download
**Priority**: High
**Preconditions**: Completed payment exists

**Steps**:
1. Navigate to payment history
2. Select a completed payment
3. Click "Download Receipt"
4. Verify PDF download

**Expected Result**:
- ✅ PDF receipt downloads successfully
- ✅ Receipt includes: Transaction ID, date, merchant, amount, status
- ✅ Receipt professionally formatted
- ✅ Filename: `receipt_{transaction-id}.pdf`

**Status**: ⏳ Pending

---

### TC-CUSTOMER-004: Request Payment Feature
**Priority**: Medium
**Preconditions**: Customer logged in

**Steps**:
1. Navigate to /customer/request-payment
2. Enter amount: 100.00 USDC
3. Enter description: "Invoice #12345"
4. Enter recipient email: merchant@example.com
5. Send request

**Expected Result**:
- ✅ Payment request created
- ✅ Email sent to recipient
- ✅ Request link generated
- ✅ Request status: "Pending"
- ✅ Request visible in "Sent Requests" list

**Status**: ⏳ Pending

---

### TC-CUSTOMER-005: Saved Recipients Management
**Priority**: Medium
**Preconditions**: Customer logged in

**Steps**:
1. Navigate to /customer/recipients
2. Click "Add Recipient"
3. Enter name: "Alice"
4. Enter wallet address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0
5. Save recipient
6. Send payment to saved recipient

**Expected Result**:
- ✅ Recipient saved successfully
- ✅ Recipient appears in saved list
- ✅ Can select recipient from dropdown in transfer form
- ✅ Address auto-filled when selected
- ✅ Can edit or delete saved recipient

**Status**: ⏳ Pending

---

### TC-CUSTOMER-006: Notification Preferences
**Priority**: Medium
**Preconditions**: Customer logged in

**Steps**:
1. Navigate to /customer/settings/notifications
2. Toggle "Payment Received" email notification
3. Toggle "Payment Failed" push notification
4. Save preferences

**Expected Result**:
- ✅ Notification preferences saved
- ✅ Changes persist after logout/login
- ✅ Email notifications sent based on preferences
- ✅ Push notifications sent based on preferences

**Status**: ⏳ Pending

---

### TC-CUSTOMER-007: Transaction Dispute Initiation
**Priority**: High
**Preconditions**: Completed payment exists

**Steps**:
1. Navigate to payment history
2. Select a payment
3. Click "Dispute Transaction"
4. Select reason: "Product not received"
5. Provide description: "Order never arrived"
6. Attach evidence (optional)
7. Submit dispute

**Expected Result**:
- ✅ Dispute created successfully
- ✅ Dispute ID generated
- ✅ Payment status changes to "Disputed"
- ✅ Merchant notified of dispute
- ✅ Dispute visible in "Active Disputes" section
- ✅ Dispute timeline created

**Status**: ⏳ Pending

---

### TC-CUSTOMER-008: Account Security Settings
**Priority**: High
**Preconditions**: Customer logged in

**Steps**:
1. Navigate to /customer/settings/security
2. View connected passkeys
3. Add new passkey
4. View active sessions
5. Revoke a session

**Expected Result**:
- ✅ All passkeys listed
- ✅ Can add new passkey
- ✅ Can remove passkey (requires re-authentication)
- ✅ Active sessions shown with device/browser info
- ✅ Can revoke sessions remotely
- ✅ Activity log displayed

**Status**: ⏳ Pending

---

## Test Suite 3: Multi-Currency Support (6 test cases)

### TC-CURRENCY-001: Multi-Currency Balance Display
**Priority**: Critical
**Preconditions**: Wallet with multiple currencies

**Steps**:
1. Navigate to /wallet
2. Verify balance display for USDC
3. Add USDT to wallet
4. Verify both balances displayed

**Expected Result**:
- ✅ Multiple currency balances shown separately
- ✅ Each currency has its own row/card
- ✅ Total value in USD displayed
- ✅ Exchange rates shown
- ✅ Last updated timestamp

**Status**: ⏳ Pending

---

### TC-CURRENCY-002: Currency Selection in Transfer
**Priority**: Critical
**Preconditions**: Wallet with multiple currencies

**Steps**:
1. Navigate to /transfer
2. Select currency: USDC
3. Enter amount: 50
4. Complete transfer
5. Repeat with USDT

**Expected Result**:
- ✅ Currency dropdown available
- ✅ Balance shown for selected currency
- ✅ Transfer uses correct currency
- ✅ Transaction record shows correct currency
- ✅ Correct contract address used

**Status**: ⏳ Pending

---

### TC-CURRENCY-003: Currency Conversion Calculator
**Priority**: Medium
**Preconditions**: User on transfer page

**Steps**:
1. Navigate to /transfer
2. Select USDC
3. Enter amount: 100
4. Click "Show in other currencies"

**Expected Result**:
- ✅ Shows equivalent amounts in USDT, BUSD, DAI
- ✅ Exchange rates displayed
- ✅ Real-time rate updates
- ✅ Source of exchange rate shown (e.g., Chainlink)

**Status**: ⏳ Pending

---

### TC-CURRENCY-004: Add New Currency to Wallet
**Priority**: High
**Preconditions**: User logged in

**Steps**:
1. Navigate to /wallet
2. Click "Add Currency"
3. Select USDT from list
4. Confirm addition

**Expected Result**:
- ✅ USDT added to wallet
- ✅ USDT balance shows 0.000000
- ✅ Can now receive USDT
- ✅ Can transfer USDT
- ✅ Currency appears in dropdown

**Status**: ⏳ Pending

---

### TC-CURRENCY-005: Currency-Specific Transaction History
**Priority**: Medium
**Preconditions**: Transactions in multiple currencies

**Steps**:
1. Navigate to /transactions
2. Filter by currency: USDC
3. Verify only USDC transactions shown
4. Filter by currency: USDT

**Expected Result**:
- ✅ Transactions filtered correctly
- ✅ Currency badge shown on each transaction
- ✅ Filter persists on page refresh
- ✅ Can clear filter

**Status**: ⏳ Pending

---

### TC-CURRENCY-006: Exchange Rate Accuracy
**Priority**: High
**Preconditions**: Multi-currency support enabled

**Steps**:
1. Navigate to /wallet
2. Note displayed exchange rates
3. Verify rates against Chainlink oracle
4. Wait 5 minutes, verify rates update

**Expected Result**:
- ✅ Exchange rates accurate (within 0.1%)
- ✅ Rates update every 5 minutes
- ✅ Last updated timestamp correct
- ✅ Source of rate displayed
- ✅ Warning if rate data stale

**Status**: ⏳ Pending

---

## Test Suite 4: Recurring Payments (8 test cases)

### TC-RECURRING-001: Create Recurring Payment
**Priority**: Critical
**Preconditions**: Customer with sufficient balance

**Steps**:
1. Navigate to /recurring-payments
2. Click "Create Recurring Payment"
3. Enter recipient: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0
4. Enter amount: 100 USDC
5. Select frequency: Monthly
6. Select start date: 2025-11-01
7. Select end condition: After 12 occurrences
8. Create payment

**Expected Result**:
- ✅ Recurring payment created
- ✅ Payment ID generated
- ✅ Status: "Active"
- ✅ Next payment date: 2025-11-01
- ✅ Payment schedule visible
- ✅ Can view upcoming payments

**Status**: ⏳ Pending

---

### TC-RECURRING-002: Recurring Payment Execution
**Priority**: Critical
**Preconditions**: Recurring payment scheduled for today

**Steps**:
1. Wait for scheduled time (or trigger manually in test)
2. Verify payment executes automatically
3. Check transaction history
4. Verify next occurrence scheduled

**Expected Result**:
- ✅ Payment executed at scheduled time
- ✅ Transaction appears in history
- ✅ Recurring payment status updated
- ✅ Next payment date calculated correctly
- ✅ Occurrence count incremented
- ✅ Email notification sent

**Status**: ⏳ Pending

---

### TC-RECURRING-003: Pause Recurring Payment
**Priority**: High
**Preconditions**: Active recurring payment

**Steps**:
1. Navigate to /recurring-payments
2. Select a recurring payment
3. Click "Pause"
4. Confirm pause

**Expected Result**:
- ✅ Status changes to "Paused"
- ✅ No further payments executed
- ✅ Can resume later
- ✅ Pause reason optional

**Status**: ⏳ Pending

---

### TC-RECURRING-004: Resume Paused Recurring Payment
**Priority**: High
**Preconditions**: Paused recurring payment

**Steps**:
1. Navigate to paused recurring payment
2. Click "Resume"
3. Confirm resume

**Expected Result**:
- ✅ Status changes to "Active"
- ✅ Next payment date recalculated
- ✅ Payments resume on schedule

**Status**: ⏳ Pending

---

### TC-RECURRING-005: Cancel Recurring Payment
**Priority**: High
**Preconditions**: Active or paused recurring payment

**Steps**:
1. Navigate to recurring payment
2. Click "Cancel"
3. Confirm cancellation

**Expected Result**:
- ✅ Status changes to "Cancelled"
- ✅ No future payments executed
- ✅ Cannot be resumed
- ✅ Past payments still visible in history

**Status**: ⏳ Pending

---

### TC-RECURRING-006: Insufficient Balance Handling
**Priority**: High
**Preconditions**: Recurring payment with insufficient balance

**Steps**:
1. Create recurring payment for 100 USDC
2. Ensure wallet balance < 100 USDC
3. Wait for scheduled payment time

**Expected Result**:
- ✅ Payment fails due to insufficient balance
- ✅ Status shows "Failed - Insufficient Balance"
- ✅ Retry scheduled for next day (3 attempts)
- ✅ Email notification sent
- ✅ After 3 failed attempts, payment skipped
- ✅ Next occurrence still scheduled

**Status**: ⏳ Pending

---

### TC-RECURRING-007: Edit Recurring Payment
**Priority**: Medium
**Preconditions**: Active recurring payment

**Steps**:
1. Navigate to recurring payment
2. Click "Edit"
3. Change amount: 150 USDC
4. Change frequency: Bi-weekly
5. Save changes

**Expected Result**:
- ✅ Changes saved successfully
- ✅ Next payment uses new amount
- ✅ Schedule recalculated based on new frequency
- ✅ Changelog/audit trail created

**Status**: ⏳ Pending

---

### TC-RECURRING-008: Recurring Payment Notifications
**Priority**: Medium
**Preconditions**: Recurring payment scheduled

**Steps**:
1. Verify email notification 24 hours before payment
2. Verify email notification after successful payment
3. Verify email notification for failed payment

**Expected Result**:
- ✅ Reminder sent 24h before: "Upcoming payment of 100 USDC tomorrow"
- ✅ Success notification: "Recurring payment of 100 USDC completed"
- ✅ Failure notification: "Recurring payment failed - insufficient balance"
- ✅ All emails include payment details and manage link

**Status**: ⏳ Pending

---

## Test Suite 5: Dispute Resolution (6 test cases)

### TC-DISPUTE-001: Customer Initiates Dispute
**Priority**: Critical
**Preconditions**: Completed payment from customer to merchant

**Steps**:
1. Customer navigates to transaction
2. Clicks "Dispute Transaction"
3. Selects reason: "Product not as described"
4. Provides description
5. Uploads evidence (screenshot)
6. Submits dispute

**Expected Result**:
- ✅ Dispute created with ID
- ✅ Transaction status: "Disputed"
- ✅ Merchant notified via email
- ✅ Dispute visible in customer dashboard
- ✅ Evidence uploaded successfully
- ✅ Dispute timeline initiated

**Status**: ⏳ Pending

---

### TC-DISPUTE-002: Merchant Responds to Dispute
**Priority**: Critical
**Preconditions**: Dispute initiated by customer

**Steps**:
1. Merchant logs in
2. Views dispute in /merchant/disputes
3. Clicks "Respond"
4. Provides response: "Product shipped on time, tracking #123"
5. Uploads evidence (shipping receipt)
6. Submits response

**Expected Result**:
- ✅ Response recorded
- ✅ Customer notified of merchant response
- ✅ Evidence attached to dispute
- ✅ Dispute status: "Under Review"
- ✅ Timeline updated

**Status**: ⏳ Pending

---

### TC-DISPUTE-003: Customer Accepts Merchant Response
**Priority**: High
**Preconditions**: Merchant responded to dispute

**Steps**:
1. Customer views merchant response
2. Reviews evidence
3. Clicks "Accept Resolution"
4. Confirms acceptance

**Expected Result**:
- ✅ Dispute status: "Resolved"
- ✅ Transaction status returns to "Completed"
- ✅ Both parties notified
- ✅ Dispute closed

**Status**: ⏳ Pending

---

### TC-DISPUTE-004: Escalate Dispute to Admin
**Priority**: High
**Preconditions**: Merchant responded, customer disagrees

**Steps**:
1. Customer views merchant response
2. Clicks "Escalate to Support"
3. Provides additional details
4. Submits escalation

**Expected Result**:
- ✅ Dispute status: "Escalated"
- ✅ Admin team notified
- ✅ Dispute assigned to support agent
- ✅ 48-hour SLA timer starts
- ✅ Both parties notified

**Status**: ⏳ Pending

---

### TC-DISPUTE-005: Admin Resolves Dispute (Refund Customer)
**Priority**: Critical
**Preconditions**: Dispute escalated to admin

**Steps**:
1. Admin logs in
2. Reviews dispute details and evidence
3. Decides in favor of customer
4. Issues full refund
5. Closes dispute with note

**Expected Result**:
- ✅ Refund processed automatically
- ✅ Customer balance increased
- ✅ Merchant balance decreased
- ✅ Dispute status: "Resolved - Refunded"
- ✅ Both parties notified with admin decision
- ✅ Dispute cannot be reopened

**Status**: ⏳ Pending

---

### TC-DISPUTE-006: Admin Resolves Dispute (Favor Merchant)
**Priority**: High
**Preconditions**: Dispute escalated to admin

**Steps**:
1. Admin reviews dispute
2. Decides in favor of merchant
3. Closes dispute with decision note

**Expected Result**:
- ✅ No refund issued
- ✅ Transaction remains "Completed"
- ✅ Dispute status: "Resolved - No Action"
- ✅ Both parties notified
- ✅ Decision rationale provided

**Status**: ⏳ Pending

---

## Test Execution Summary

| Test Suite | Total Cases | Passed | Failed | Blocked | Pending |
|------------|-------------|--------|--------|---------|------------|
| Merchant Dashboard | 12 | 0 | 0 | 0 | 12 |
| Customer Dashboard | 8 | 0 | 0 | 0 | 8 |
| Multi-Currency Support | 6 | 0 | 0 | 0 | 6 |
| Recurring Payments | 8 | 0 | 0 | 0 | 8 |
| Dispute Resolution | 6 | 0 | 0 | 0 | 6 |
| **TOTAL** | **40** | **0** | **0** | **0** | **40** |

---

## Test Report Template

### Execution Date: _______________
### Tester: _______________
### Environment: _______________

**Summary**:
- Total test cases: 40
- Passed: ___
- Failed: ___
- Blocked: ___
- Pass Rate: ___%

**Critical Issues Found**:
1.
2.
3.

**Phase 2 Features Validated**:
- [ ] Merchant dashboard and analytics
- [ ] Customer payment history
- [ ] Multi-currency wallet support
- [ ] Recurring payment automation
- [ ] Dispute resolution workflow

**Recommendations**:
1.
2.
3.

**Sign-off**: _______________ Date: _______________

---

## Notes

- Phase 2 tests depend on Phase 1 functionality being stable
- Test merchant and customer flows independently first
- Multi-currency tests require testnet token faucets
- Recurring payment tests may require time manipulation or cron job simulation
- Dispute resolution requires admin panel access

