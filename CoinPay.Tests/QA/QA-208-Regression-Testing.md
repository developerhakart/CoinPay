# QA-208: Regression Testing - Sprint N01

**Test Owner**: QA Engineer 2
**Effort**: 2.00 days
**Status**: Ready for Execution
**Date Created**: 2025-10-29
**Priority**: CRITICAL

---

## Test Objectives

Ensure Sprint N01 functionality remains intact after Sprint N02 implementation:
- Verify no regressions in passkey authentication
- Validate wallet creation and management
- Confirm gasless transfer functionality
- Ensure transaction monitoring works
- Verify all Phase 1 features stable

---

## Test Environment

**Frontend**: http://localhost:3000
**Backend API**: http://localhost:5000
**Network**: Polygon Amoy Testnet
**Baseline**: Sprint N01 commit SHA: `b295266`

---

## Regression Testing Strategy

### 1. Smoke Tests (Critical Path)
Quick validation of core functionality (30 minutes)

### 2. Full Regression Suite
Comprehensive testing of all Sprint N01 features (4-6 hours)

### 3. Comparison Testing
Compare Sprint N01 vs Sprint N02 behavior

---

## SMOKE TESTS (Critical Path)

### SMOKE-001: User Registration
**Priority**: CRITICAL
**Execution Time**: 2 minutes

**Steps**:
1. Navigate to /register
2. Register new user with passkey
3. Verify redirect to dashboard
4. Verify wallet created automatically

**Expected Result**:
- ✅ Registration completes in < 10 seconds
- ✅ Dashboard loads successfully
- ✅ Wallet address displayed
- ✅ Balance shows 0 USDC

**Baseline Behavior** (Sprint N01): Same

**Status**: ⏳ Pending

---

### SMOKE-002: User Login
**Priority**: CRITICAL
**Execution Time**: 1 minute

**Steps**:
1. Navigate to /login
2. Login with existing passkey
3. Verify redirect to dashboard

**Expected Result**:
- ✅ Login succeeds in < 5 seconds
- ✅ Dashboard loads
- ✅ User data displayed correctly

**Baseline Behavior** (Sprint N01): Same

**Status**: ⏳ Pending

---

### SMOKE-003: USDC Transfer
**Priority**: CRITICAL
**Execution Time**: 3 minutes

**Steps**:
1. Navigate to /transfer
2. Send 10 USDC to test address
3. Verify transaction appears in history

**Expected Result**:
- ✅ Transfer form validates correctly
- ✅ Transaction submits successfully
- ✅ Transaction appears in /transactions
- ✅ Balance updates correctly (no gas fee)

**Baseline Behavior** (Sprint N01): Same

**Status**: ⏳ Pending

---

### SMOKE-004: Transaction History
**Priority**: CRITICAL
**Execution Time**: 1 minute

**Steps**:
1. Navigate to /transactions
2. Verify recent transactions displayed

**Expected Result**:
- ✅ Transactions load in < 3 seconds
- ✅ All transactions visible
- ✅ Status badges correct

**Baseline Behavior** (Sprint N01): Same

**Status**: ⏳ Pending

---

### SMOKE-005: Logout
**Priority**: HIGH
**Execution Time**: 30 seconds

**Steps**:
1. Click logout
2. Verify redirect to login
3. Try accessing /dashboard

**Expected Result**:
- ✅ Session cleared
- ✅ Redirected to /login
- ✅ Cannot access protected routes

**Baseline Behavior** (Sprint N01): Same

**Status**: ⏳ Pending

---

## FULL REGRESSION SUITE

### Section 1: Authentication Regression (8 tests)

#### REG-AUTH-001: Passkey Registration Flow
**Priority**: CRITICAL

**Test Data**:
- Username: `regtest_user_${timestamp}`
- Email: `regtest_${timestamp}@example.com`

**Steps**:
1. Navigate to /register
2. Enter username and email
3. Click "Register with Passkey"
4. Complete passkey creation
5. Verify dashboard redirect
6. Verify wallet auto-creation

**Expected Result**:
- ✅ All steps identical to Sprint N01 behavior
- ✅ No new bugs introduced
- ✅ Performance within 10% of baseline

**Baseline Time**: 8-12 seconds
**Current Time**: ___ seconds

**Status**: ⏳ Pending

---

#### REG-AUTH-002: Passkey Login Flow
**Priority**: CRITICAL

**Steps**:
1. Navigate to /login
2. Enter username
3. Complete passkey authentication
4. Verify dashboard redirect

**Expected Result**:
- ✅ Identical behavior to Sprint N01
- ✅ Session token created correctly
- ✅ User data loaded

**Baseline Time**: 4-6 seconds
**Current Time**: ___ seconds

**Status**: ⏳ Pending

---

#### REG-AUTH-003: Invalid Login Attempt
**Priority**: HIGH

**Steps**:
1. Navigate to /login
2. Enter non-existent username
3. Verify error handling

**Expected Result**:
- ✅ Same error message as Sprint N01: "User not found"
- ✅ No passkey prompt shown
- ✅ Remains on login page

**Status**: ⏳ Pending

---

#### REG-AUTH-004: Session Persistence
**Priority**: HIGH

**Steps**:
1. Login successfully
2. Refresh page
3. Navigate to different routes
4. Verify session maintained

**Expected Result**:
- ✅ Session persists after refresh
- ✅ No re-authentication required
- ✅ Same session timeout (30 minutes)

**Status**: ⏳ Pending

---

#### REG-AUTH-005: Logout Functionality
**Priority**: HIGH

**Steps**:
1. Login
2. Perform some actions
3. Logout
4. Try accessing protected routes

**Expected Result**:
- ✅ Session cleared completely
- ✅ Redirected to /login
- ✅ Cannot access /dashboard, /wallet, /transfer

**Status**: ⏳ Pending

---

#### REG-AUTH-006: Protected Route Access Control
**Priority**: CRITICAL

**Steps**:
1. Without logging in, try to access:
   - /dashboard
   - /wallet
   - /transfer
   - /transactions

**Expected Result**:
- ✅ All routes redirect to /login
- ✅ No data accessible without authentication
- ✅ Same behavior as Sprint N01

**Status**: ⏳ Pending

---

#### REG-AUTH-007: Public Route Accessibility
**Priority**: MEDIUM

**Steps**:
1. Without logging in, access:
   - /
   - /login
   - /register

**Expected Result**:
- ✅ All public routes accessible
- ✅ No authentication required
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-AUTH-008: Concurrent Session Handling
**Priority**: MEDIUM

**Steps**:
1. Login on Browser A
2. Login on Browser B (same user)
3. Perform actions on both
4. Logout from Browser A
5. Verify Browser B still works

**Expected Result**:
- ✅ Both sessions independent
- ✅ Logout on one doesn't affect other
- ✅ Same behavior as Sprint N01

**Status**: ⏳ Pending

---

### Section 2: Wallet Regression (6 tests)

#### REG-WALLET-001: Automatic Wallet Creation
**Priority**: CRITICAL

**Steps**:
1. Register new user
2. Check database for wallet record
3. Verify wallet address on dashboard

**Expected Result**:
- ✅ Wallet created automatically on registration
- ✅ Address format: 0x[40 hex characters]
- ✅ Initial balance: 0 USDC
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-WALLET-002: Wallet Balance Display
**Priority**: CRITICAL

**Steps**:
1. Navigate to /wallet
2. Verify balance displayed correctly
3. Fund wallet with testnet USDC
4. Refresh and verify balance updates

**Expected Result**:
- ✅ Balance format: X.XXXXXX USDC (6 decimals)
- ✅ Balance updates after funding
- ✅ Refresh button works
- ✅ Same precision as Sprint N01

**Status**: ⏳ Pending

---

#### REG-WALLET-003: Copy Wallet Address
**Priority**: HIGH

**Steps**:
1. Navigate to /wallet
2. Click copy address button
3. Paste into text editor

**Expected Result**:
- ✅ Full address copied
- ✅ "Copied!" feedback shown
- ✅ Same UX as Sprint N01

**Status**: ⏳ Pending

---

#### REG-WALLET-004: Wallet Address Format Validation
**Priority**: HIGH

**Steps**:
1. Verify wallet address is valid Ethereum address
2. Check address checksum
3. Verify address unique per user

**Expected Result**:
- ✅ Address passes checksum validation
- ✅ Each user has unique address
- ✅ Same validation as Sprint N01

**Status**: ⏳ Pending

---

#### REG-WALLET-005: Wallet QR Code Generation
**Priority**: MEDIUM

**Steps**:
1. Navigate to /wallet
2. Click "Receive" or "QR Code"
3. Verify QR code displays

**Expected Result**:
- ✅ QR code modal opens
- ✅ QR code contains wallet address
- ✅ Address shown below QR code
- ✅ Same as Sprint N01 (if implemented)

**Status**: ⏳ Pending

---

#### REG-WALLET-006: Balance Refresh
**Priority**: MEDIUM

**Steps**:
1. Note current balance
2. Click refresh button
3. Verify balance re-fetched from blockchain

**Expected Result**:
- ✅ Loading indicator shown
- ✅ Balance refreshed correctly
- ✅ Last updated timestamp shown
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

### Section 3: Transfer Regression (12 tests)

#### REG-TRANSFER-001: Successful Transfer
**Priority**: CRITICAL

**Test Data**:
- Recipient: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0
- Amount: 25.50 USDC

**Steps**:
1. Navigate to /transfer
2. Fill form with valid data
3. Submit transfer
4. Verify transaction created

**Expected Result**:
- ✅ Transfer completes successfully
- ✅ Transaction appears in history
- ✅ Balance reduced by exact amount (no gas fee)
- ✅ Same flow as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-002: Insufficient Balance Error
**Priority**: CRITICAL

**Steps**:
1. Navigate to /transfer
2. Enter amount > balance
3. Try to submit

**Expected Result**:
- ✅ Error message: "Insufficient balance"
- ✅ Cannot proceed
- ✅ Same error as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-003: Invalid Address Validation
**Priority**: HIGH

**Test Cases**:
- Short address: "0x123"
- Invalid format: "not-an-address"
- Non-checksum address

**Expected Result**:
- ✅ Error: "Invalid Ethereum address"
- ✅ Address field highlighted
- ✅ Same validation as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-004: Self-Transfer Prevention
**Priority**: HIGH

**Steps**:
1. Try sending to own wallet address
2. Verify error shown

**Expected Result**:
- ✅ Error: "Cannot send to your own address"
- ✅ Transfer blocked
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-005: Amount Validation (Min)
**Priority**: HIGH

**Steps**:
1. Enter amount: 0.0000001 (below min)
2. Try to submit

**Expected Result**:
- ✅ Error: "Minimum amount is 0.000001 USDC"
- ✅ Submit disabled
- ✅ Same validation as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-006: Amount Validation (Max)
**Priority**: HIGH

**Steps**:
1. Enter amount: 1,500,000 (above max)
2. Try to submit

**Expected Result**:
- ✅ Error: "Maximum amount is 1,000,000 USDC"
- ✅ Submit disabled
- ✅ Same validation as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-007: MAX Button
**Priority**: MEDIUM

**Steps**:
1. Click MAX button
2. Verify amount filled

**Expected Result**:
- ✅ Amount = full wallet balance
- ✅ Can submit transfer
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-008: Transfer Note/Description
**Priority**: MEDIUM

**Steps**:
1. Add note: "Payment for invoice #123"
2. Submit transfer
3. Verify note saved

**Expected Result**:
- ✅ Note saved in transaction
- ✅ Note visible in transaction history
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-009: Transfer Preview
**Priority**: HIGH

**Steps**:
1. Fill transfer form
2. Click "Review"
3. Verify preview details

**Expected Result**:
- ✅ All details correct in preview
- ✅ From/To addresses shown
- ✅ Amount, currency, network shown
- ✅ Gas fee: "FREE (Sponsored)"
- ✅ Same preview as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-010: Gasless Transaction Verification
**Priority**: CRITICAL

**Steps**:
1. Note balance before: X USDC
2. Transfer 10 USDC
3. Note balance after: Y USDC
4. Calculate: X - Y = ?

**Expected Result**:
- ✅ X - Y = 10.000000 USDC (exactly)
- ✅ No gas fee deducted
- ✅ Same gasless behavior as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-011: Multiple Consecutive Transfers
**Priority**: HIGH

**Steps**:
1. Send 5 USDC to Address A
2. Send 5 USDC to Address B
3. Send 5 USDC to Address C
4. Verify all transactions succeed

**Expected Result**:
- ✅ All 3 transfers succeed
- ✅ All appear in transaction history
- ✅ Total balance reduced by 15 USDC
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TRANSFER-012: Transfer Cancellation
**Priority**: MEDIUM

**Steps**:
1. Fill transfer form
2. Click "Review"
3. Click "Back" or "Cancel"
4. Verify form state

**Expected Result**:
- ✅ Returns to form
- ✅ Data preserved or cleared (same as Sprint N01)
- ✅ No transaction created

**Status**: ⏳ Pending

---

### Section 4: Transaction History Regression (8 tests)

#### REG-TX-001: Transaction List Display
**Priority**: CRITICAL

**Steps**:
1. Navigate to /transactions
2. Verify transactions displayed

**Expected Result**:
- ✅ All user's transactions shown
- ✅ Reverse chronological order (newest first)
- ✅ Shows: Date, Recipient, Amount, Status
- ✅ Same display as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TX-002: Transaction Status Updates
**Priority**: CRITICAL

**Steps**:
1. Submit new transfer
2. Navigate to /transactions
3. Wait for status to update from "Pending" to "Completed"

**Expected Result**:
- ✅ Status starts as "Pending"
- ✅ Polling every 5 seconds (same as Sprint N01)
- ✅ Status updates to "Completed"
- ✅ Status badge color changes

**Status**: ⏳ Pending

---

#### REG-TX-003: Transaction Detail Modal
**Priority**: HIGH

**Steps**:
1. Click on a transaction
2. Verify modal opens with details

**Expected Result**:
- ✅ Modal shows all transaction details
- ✅ Transaction ID, date, amount, status
- ✅ From/To addresses
- ✅ Same modal as Sprint N01 (if changed in N02, verify no regression)

**Status**: ⏳ Pending

---

#### REG-TX-004: Transaction Filtering (NEW in N02)
**Priority**: HIGH

**Steps**:
1. Apply status filter: "Completed"
2. Apply search: recipient name
3. Verify filtering works AND old data still visible

**Expected Result**:
- ✅ Filtering works correctly (new feature)
- ✅ All Sprint N01 transactions still accessible
- ✅ No data loss from Sprint N01

**Status**: ⏳ Pending

---

#### REG-TX-005: Pagination (NEW in N02)
**Priority**: MEDIUM

**Steps**:
1. Generate 25+ transactions
2. Verify pagination appears
3. Navigate through pages

**Expected Result**:
- ✅ Pagination works (new feature)
- ✅ All old transactions visible
- ✅ No Sprint N01 transactions missing

**Status**: ⏳ Pending

---

#### REG-TX-006: Empty State
**Priority**: MEDIUM

**Steps**:
1. Login as new user with no transactions
2. Navigate to /transactions

**Expected Result**:
- ✅ "No Transactions Yet" message
- ✅ Same empty state as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TX-007: Transaction Copy Features
**Priority**: MEDIUM

**Steps**:
1. Open transaction detail
2. Copy transaction ID
3. Copy recipient address

**Expected Result**:
- ✅ Copy buttons work
- ✅ "Copied!" feedback shown
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

#### REG-TX-008: Success Message from Transfer
**Priority**: MEDIUM

**Steps**:
1. Complete transfer from /transfer
2. Get redirected to /transactions
3. Verify success message shown

**Expected Result**:
- ✅ Success message appears
- ✅ Message disappears after 5 seconds
- ✅ Same behavior as Sprint N01

**Status**: ⏳ Pending

---

## Data Integrity Checks

### DATA-001: Database Migration Verification
**Priority**: CRITICAL

**Steps**:
1. Query database for Sprint N01 data
2. Verify all tables exist
3. Check for data corruption

**SQL Queries**:
```sql
-- Check users created in Sprint N01
SELECT COUNT(*) FROM Users WHERE CreatedAt < '2025-10-28';

-- Check transactions from Sprint N01
SELECT COUNT(*) FROM Transactions WHERE CreatedAt < '2025-10-28';

-- Verify wallet integrity
SELECT COUNT(*) FROM Wallets;
SELECT COUNT(DISTINCT UserId) FROM Wallets;
```

**Expected Result**:
- ✅ All Sprint N01 users present
- ✅ All Sprint N01 transactions intact
- ✅ 1 wallet per user

**Status**: ⏳ Pending

---

### DATA-002: API Endpoint Compatibility
**Priority**: CRITICAL

**Steps**:
1. Call all Sprint N01 API endpoints
2. Verify responses unchanged

**Endpoints to Test**:
- `GET /api/auth/challenge`
- `POST /api/auth/register`
- `POST /api/auth/verify`
- `GET /api/wallet/balance`
- `POST /api/transfer/submit`
- `GET /api/transactions`

**Expected Result**:
- ✅ All endpoints return expected responses
- ✅ Response schemas unchanged
- ✅ No breaking changes

**Status**: ⏳ Pending

---

## Performance Regression

### PERF-001: Page Load Times
**Priority**: HIGH

**Measure load times for**:
- /dashboard
- /wallet
- /transfer
- /transactions

**Expected Result**:
- ✅ Load times within 20% of Sprint N01 baseline
- ✅ No significant performance degradation

**Baseline** (Sprint N01):
- Dashboard: 1.2s
- Wallet: 0.8s
- Transfer: 0.9s
- Transactions: 1.5s

**Current** (Sprint N02):
- Dashboard: ___ s
- Wallet: ___ s
- Transfer: ___ s
- Transactions: ___ s

**Status**: ⏳ Pending

---

### PERF-002: API Response Times
**Priority**: HIGH

**Measure response times for**:
- `GET /api/wallet/balance`
- `GET /api/transactions`
- `POST /api/transfer/submit`

**Expected Result**:
- ✅ Response times within 20% of baseline
- ✅ No new bottlenecks

**Baseline** (Sprint N01):
- Wallet balance: 150ms
- Transactions: 200ms
- Transfer submit: 500ms

**Current** (Sprint N02):
- Wallet balance: ___ ms
- Transactions: ___ ms
- Transfer submit: ___ ms

**Status**: ⏳ Pending

---

## Cross-Browser Regression

### BROWSER-001: Chrome Compatibility
**Priority**: HIGH

**Steps**:
1. Run full regression suite on Chrome
2. Verify all features work

**Expected Result**:
- ✅ All tests pass on Chrome
- ✅ Same as Sprint N01

**Status**: ⏳ Pending

---

### BROWSER-002: Firefox Compatibility
**Priority**: MEDIUM

**Steps**:
1. Run regression suite on Firefox
2. Verify passkey authentication works

**Expected Result**:
- ✅ All tests pass on Firefox
- ✅ No new browser-specific issues

**Status**: ⏳ Pending

---

## Test Execution Summary

| Section | Total Tests | Passed | Failed | Blocked | Notes |
|---------|-------------|--------|--------|---------|-------|
| Smoke Tests | 5 | 0 | 0 | 0 | Critical path |
| Authentication Regression | 8 | 0 | 0 | 0 | Sprint N01 core |
| Wallet Regression | 6 | 0 | 0 | 0 | Sprint N01 core |
| Transfer Regression | 12 | 0 | 0 | 0 | Sprint N01 core |
| Transaction History | 8 | 0 | 0 | 0 | Includes N02 features |
| Data Integrity | 2 | 0 | 0 | 0 | Database checks |
| Performance | 2 | 0 | 0 | 0 | Load time checks |
| Cross-Browser | 2 | 0 | 0 | 0 | Chrome, Firefox |
| **TOTAL** | **45** | **0** | **0** | **0** | |

---

## Regression Test Report Template

### Sprint N02 Regression Test Report

**Date**: _______________
**Tester**: _______________
**Environment**: _______________
**Sprint N01 Baseline**: Commit `b295266`
**Sprint N02 Version**: Commit `_______`

#### Summary
- Total regression tests: 45
- Passed: ___
- Failed: ___
- Blocked: ___
- **Pass Rate**: ___%

#### Regressions Found
1. **[Feature Name]**
   - Severity: Critical/High/Medium/Low
   - Sprint N01 Behavior: ...
   - Sprint N02 Behavior: ...
   - Impact: ...
   - Ticket: ___

#### Performance Comparison
| Metric | Sprint N01 | Sprint N02 | Change |
|--------|-----------|-----------|--------|
| Dashboard Load | 1.2s | ___s | ___% |
| Transfer Submit | 500ms | ___ms | ___% |
| Transactions Load | 1.5s | ___s | ___% |

#### Recommendations
- [ ] Fix critical regressions before release
- [ ] Re-run full regression suite after fixes
- [ ] Update regression baseline for Sprint N03

**Sign-off**: _______________ Date: _______________

---

## Notes

- Run regression tests BEFORE every release
- Compare against Sprint N01 baseline commit
- Document any intentional breaking changes
- Re-baseline after major refactors
- Automate regression suite with CI/CD

