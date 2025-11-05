# QA-201: Phase 1 Functional Testing

**Test Owner**: QA Engineer 1
**Effort**: 4.00 days
**Status**: Ready for Execution
**Date Created**: 2025-10-29

---

## Test Objectives

Validate core Phase 1 functionality:
- Passkey authentication flows
- Wallet creation and initialization
- Gasless USDC transfers

---

## Test Environment

**Frontend**: http://localhost:3000
**Backend API**: http://localhost:5000
**Network**: Polygon Amoy Testnet
**Test Data**: Prepared test accounts and wallets

---

## Test Suite 1: Passkey Authentication (8 test cases)

### TC-AUTH-001: User Registration with Passkey
**Priority**: Critical
**Preconditions**: New user, browser supports WebAuthn

**Steps**:
1. Navigate to /register
2. Enter username: "testuser001"
3. Enter email: "testuser001@example.com"
4. Click "Register with Passkey"
5. Complete passkey creation flow (biometric/PIN)
6. Verify redirect to dashboard

**Expected Result**:
- ✅ Passkey created successfully
- ✅ User registered in database
- ✅ Redirected to /dashboard
- ✅ Welcome message displayed
- ✅ Wallet automatically created

**Test Data**:
```
Username: testuser001
Email: testuser001@example.com
```

**Status**: ⏳ Pending

---

### TC-AUTH-002: User Login with Passkey
**Priority**: Critical
**Preconditions**: Existing user with passkey

**Steps**:
1. Navigate to /login
2. Enter username: "testuser001"
3. Click "Login with Passkey"
4. Complete passkey authentication (biometric/PIN)
5. Verify redirect to dashboard

**Expected Result**:
- ✅ Passkey authentication successful
- ✅ Session token created
- ✅ Redirected to /dashboard
- ✅ User data loaded correctly

**Status**: ⏳ Pending

---

### TC-AUTH-003: Login with Invalid Passkey
**Priority**: High
**Preconditions**: Existing user

**Steps**:
1. Navigate to /login
2. Enter username: "testuser001"
3. Click "Login with Passkey"
4. Cancel or fail passkey authentication
5. Verify error handling

**Expected Result**:
- ✅ Authentication fails gracefully
- ✅ Error message displayed: "Authentication failed"
- ✅ User remains on login page
- ✅ No session created

**Status**: ⏳ Pending

---

### TC-AUTH-004: Login with Non-existent User
**Priority**: High
**Preconditions**: None

**Steps**:
1. Navigate to /login
2. Enter username: "nonexistentuser999"
3. Click "Login with Passkey"

**Expected Result**:
- ✅ Error message: "User not found"
- ✅ No passkey prompt shown
- ✅ User remains on login page

**Status**: ⏳ Pending

---

### TC-AUTH-005: Session Persistence
**Priority**: Medium
**Preconditions**: User logged in

**Steps**:
1. Login successfully
2. Refresh page
3. Verify session maintained

**Expected Result**:
- ✅ User remains logged in after refresh
- ✅ Dashboard loads without re-authentication
- ✅ User data persists

**Status**: ⏳ Pending

---

### TC-AUTH-006: Logout Functionality
**Priority**: High
**Preconditions**: User logged in

**Steps**:
1. Login successfully
2. Navigate to dashboard
3. Click "Logout" button
4. Verify redirect to login page
5. Attempt to access /dashboard directly

**Expected Result**:
- ✅ Session cleared
- ✅ Redirected to /login
- ✅ Cannot access protected routes
- ✅ Redirected to /login when accessing /dashboard

**Status**: ⏳ Pending

---

### TC-AUTH-007: Passkey Browser Compatibility
**Priority**: Medium
**Preconditions**: Multiple browsers available

**Steps**:
1. Test registration in Chrome
2. Test registration in Firefox
3. Test registration in Edge
4. Test registration in Safari (if available)

**Expected Result**:
- ✅ Passkey works in Chrome
- ✅ Passkey works in Firefox
- ✅ Passkey works in Edge
- ✅ Fallback message shown if WebAuthn not supported

**Status**: ⏳ Pending

---

### TC-AUTH-008: Concurrent Sessions
**Priority**: Low
**Preconditions**: User account

**Steps**:
1. Login on Device A (Chrome)
2. Login on Device B (Firefox)
3. Perform actions on both devices
4. Verify both sessions work independently

**Expected Result**:
- ✅ Both sessions active
- ✅ No session conflicts
- ✅ Actions independent per session

**Status**: ⏳ Pending

---

## Test Suite 2: Wallet Creation (6 test cases)

### TC-WALLET-001: Automatic Wallet Creation on Registration
**Priority**: Critical
**Preconditions**: New user registration

**Steps**:
1. Complete registration (TC-AUTH-001)
2. Navigate to /wallet
3. Verify wallet created

**Expected Result**:
- ✅ Wallet address generated (0x...)
- ✅ Wallet address is 42 characters (0x + 40 hex)
- ✅ Initial balance is 0 USDC
- ✅ Wallet address displayed on dashboard
- ✅ Wallet stored in database

**Status**: ⏳ Pending

---

### TC-WALLET-002: Wallet Address Display
**Priority**: High
**Preconditions**: User logged in with wallet

**Steps**:
1. Navigate to /wallet
2. Verify wallet address displayed
3. Verify address format

**Expected Result**:
- ✅ Full address shown: 0x1234...5678 (truncated)
- ✅ Copy button present
- ✅ Address matches database record

**Status**: ⏳ Pending

---

### TC-WALLET-003: Copy Wallet Address
**Priority**: Medium
**Preconditions**: User on wallet page

**Steps**:
1. Navigate to /wallet
2. Click copy button
3. Paste into text editor
4. Verify full address copied

**Expected Result**:
- ✅ Full address copied to clipboard
- ✅ "Copied!" feedback shown
- ✅ Address format valid (0x + 40 hex chars)

**Status**: ⏳ Pending

---

### TC-WALLET-004: Balance Display
**Priority**: High
**Preconditions**: Wallet with balance

**Steps**:
1. Fund wallet with test USDC
2. Navigate to /wallet
3. Verify balance displayed correctly

**Expected Result**:
- ✅ Balance shows with 6 decimal places
- ✅ Currency shows as "USDC"
- ✅ Balance updates after transactions
- ✅ Balance formatted correctly (e.g., 100.000000 USDC)

**Status**: ⏳ Pending

---

### TC-WALLET-005: Balance Refresh
**Priority**: Medium
**Preconditions**: Wallet with balance

**Steps**:
1. Navigate to /wallet
2. Note current balance
3. Click refresh button
4. Verify balance re-fetched

**Expected Result**:
- ✅ Loading indicator shown
- ✅ Balance refreshed from blockchain
- ✅ Last updated timestamp shown
- ✅ Auto-refresh every 30 seconds works

**Status**: ⏳ Pending

---

### TC-WALLET-006: QR Code Generation
**Priority**: Low
**Preconditions**: User on wallet page

**Steps**:
1. Navigate to /wallet
2. Click "Receive" or "QR Code" button
3. Verify QR code modal opens
4. Verify QR code displayed

**Expected Result**:
- ✅ Modal opens with QR code
- ✅ Wallet address shown below QR code
- ✅ Copy address button works
- ✅ Download QR code button works
- ✅ Warning about network compatibility shown

**Status**: ⏳ Pending

---

## Test Suite 3: Gasless Transfers (10 test cases)

### TC-TRANSFER-001: Successful Transfer
**Priority**: Critical
**Preconditions**: Wallet with 100 USDC balance

**Steps**:
1. Navigate to /transfer
2. Enter recipient address: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0
3. Enter amount: 10.50
4. Enter note: "Test payment"
5. Click "Review Transfer"
6. Review details
7. Click "Confirm & Send"

**Expected Result**:
- ✅ Transfer form validates inputs
- ✅ Preview shows correct details
- ✅ Transaction submitted successfully
- ✅ Success message shown
- ✅ Redirected to /transactions
- ✅ Transaction shows as "Pending"
- ✅ Balance reduced by 10.50 USDC
- ✅ No gas fee charged (gasless)

**Status**: ⏳ Pending

---

### TC-TRANSFER-002: Transfer with Insufficient Balance
**Priority**: High
**Preconditions**: Wallet with 5 USDC balance

**Steps**:
1. Navigate to /transfer
2. Enter recipient address
3. Enter amount: 10.00
4. Click "Review Transfer"

**Expected Result**:
- ✅ Error message: "Insufficient balance. You have 5.00 USDC"
- ✅ Cannot proceed to preview
- ✅ Amount field highlighted in red
- ✅ Submit button disabled

**Status**: ⏳ Pending

---

### TC-TRANSFER-003: Transfer with Invalid Address
**Priority**: High
**Preconditions**: Wallet with balance

**Steps**:
1. Navigate to /transfer
2. Enter invalid address: "0x123" (too short)
3. Enter amount: 5.00
4. Click "Review Transfer"

**Expected Result**:
- ✅ Error message: "Invalid Ethereum address format"
- ✅ Address field highlighted in red
- ✅ Cannot proceed to preview

**Status**: ⏳ Pending

---

### TC-TRANSFER-004: Transfer to Own Address
**Priority**: Medium
**Preconditions**: User logged in

**Steps**:
1. Navigate to /transfer
2. Enter own wallet address
3. Enter amount: 5.00
4. Click "Review Transfer"

**Expected Result**:
- ✅ Error message: "Cannot send to your own address"
- ✅ Address field highlighted in red
- ✅ Cannot proceed to preview

**Status**: ⏳ Pending

---

### TC-TRANSFER-005: Transfer with Amount Below Minimum
**Priority**: Medium
**Preconditions**: Wallet with balance

**Steps**:
1. Navigate to /transfer
2. Enter valid recipient address
3. Enter amount: 0.0000001 (below 0.000001)
4. Click "Review Transfer"

**Expected Result**:
- ✅ Error message: "Minimum amount is 0.000001 USDC"
- ✅ Amount field highlighted in red
- ✅ Cannot proceed

**Status**: ⏳ Pending

---

### TC-TRANSFER-006: Transfer with Amount Above Maximum
**Priority**: Medium
**Preconditions**: Wallet with balance

**Steps**:
1. Navigate to /transfer
2. Enter valid recipient address
3. Enter amount: 1500000 (above 1,000,000)
4. Click "Review Transfer"

**Expected Result**:
- ✅ Error message: "Maximum amount is 1,000,000 USDC"
- ✅ Amount field highlighted in red
- ✅ Cannot proceed

**Status**: ⏳ Pending

---

### TC-TRANSFER-007: Transfer with MAX Button
**Priority**: Medium
**Preconditions**: Wallet with 100 USDC

**Steps**:
1. Navigate to /transfer
2. Enter valid recipient address
3. Click "MAX" button
4. Verify amount auto-filled
5. Complete transfer

**Expected Result**:
- ✅ Amount field shows 100 (full balance)
- ✅ Transfer proceeds successfully
- ✅ Balance becomes 0 after transfer

**Status**: ⏳ Pending

---

### TC-TRANSFER-008: Transfer Preview Accuracy
**Priority**: High
**Preconditions**: Ready to transfer

**Steps**:
1. Fill transfer form:
   - Recipient: 0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0
   - Amount: 25.123456
   - Note: "Test note"
2. Click "Review Transfer"
3. Verify all details in preview

**Expected Result**:
- ✅ From address matches user's wallet
- ✅ To address matches input
- ✅ Amount shows 25.123456 USDC (6 decimals)
- ✅ Note displays correctly
- ✅ Network shows "Polygon Amoy Testnet"
- ✅ Gas fee shows "FREE (Sponsored)"
- ✅ Back button works
- ✅ Confirm button enabled

**Status**: ⏳ Pending

---

### TC-TRANSFER-009: Transaction Status Tracking
**Priority**: High
**Preconditions**: Transfer submitted

**Steps**:
1. Complete a transfer
2. Navigate to /transactions
3. Find the transaction
4. Click on transaction to view details
5. Wait for confirmation

**Expected Result**:
- ✅ Transaction appears in list immediately
- ✅ Initial status: "Pending"
- ✅ Status updates automatically (polling every 5s)
- ✅ Status changes to "Confirmed" after blockchain confirmation
- ✅ Transaction details modal shows all info
- ✅ Block explorer link works

**Status**: ⏳ Pending

---

### TC-TRANSFER-010: Gasless Transaction Verification
**Priority**: Critical
**Preconditions**: Wallet with USDC

**Steps**:
1. Note balance before transfer
2. Complete transfer of 10 USDC
3. Note balance after transfer
4. Calculate difference

**Expected Result**:
- ✅ Balance reduced by EXACTLY transfer amount
- ✅ No additional gas fees deducted
- ✅ "Gasless Transaction" indicator shown in UI
- ✅ Paymaster service sponsored the gas

**Status**: ⏳ Pending

---

## Test Execution Summary

| Test Suite | Total Cases | Passed | Failed | Blocked | Pending |
|------------|-------------|--------|--------|---------|---------|
| Passkey Authentication | 8 | 0 | 0 | 0 | 8 |
| Wallet Creation | 6 | 0 | 0 | 0 | 6 |
| Gasless Transfers | 10 | 0 | 0 | 0 | 10 |
| **TOTAL** | **24** | **0** | **0** | **0** | **24** |

---

## Test Report Template

### Execution Date: _______________
### Tester: _______________
### Environment: _______________

**Summary**:
- Total test cases: 24
- Passed: ___
- Failed: ___
- Blocked: ___
- Pass Rate: ___%

**Critical Issues Found**:
1.
2.
3.

**Recommendations**:
1.
2.
3.

**Sign-off**: _______________ Date: _______________

---

## Notes

- All test cases should be executed in order
- Record screenshots for failed test cases
- Document any deviations from expected results
- Report critical bugs immediately
- Update test case status after execution
