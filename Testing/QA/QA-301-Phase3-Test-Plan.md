# QA-301: Phase 3 Functional Test Plan

**Sprint**: N03
**Phase**: Phase 3 - Fiat Off-Ramp
**Created**: 2025-10-29
**Owner**: QA Lead
**Status**: Draft

---

## 1. Executive Summary

### Test Objectives
- Validate bank account management functionality (CRUD operations)
- Verify encryption of sensitive data (routing/account numbers)
- Test fiat withdrawal flow end-to-end
- Validate exchange rate display and calculations
- Ensure payout tracking and history work correctly
- Verify security and data protection measures

### Scope
**In Scope**:
- Bank account management (add, view, edit, delete)
- Bank account validation (US ACH format)
- Data encryption (AES-256-GCM)
- Fiat withdrawal wizard (multi-step)
- USDC to USD conversion calculator
- Exchange rate service
- Payout initiation and tracking
- Payout history with filtering
- Fee calculation and transparency
- Webhook processing

**Out of Scope**:
- KYC/AML full compliance (basic only)
- Multiple country support (US only in MVP)
- Mobile app testing (web only)
- Production fiat gateway (sandbox only)

### Test Approach
- Manual functional testing
- Automated E2E tests (Cypress)
- Security testing (encryption validation)
- Integration testing (gateway sandbox)
- Performance testing (K6 load tests)
- Regression testing (Phases 1-2)

---

## 2. Test Environment

### Test Environments
| Environment | Purpose | URL |
|-------------|---------|-----|
| Development | Developer testing | http://localhost:3000 |
| QA | QA team testing | https://qa.coinpay.app |
| Staging | Pre-production | https://staging.coinpay.app |

### Test Data Requirements
```json
{
  "testUsers": [
    {
      "email": "testuser1@test.com",
      "password": "TestPass123!",
      "usdcBalance": 1000.00
    },
    {
      "email": "testuser2@test.com",
      "password": "TestPass123!",
      "usdcBalance": 50.00
    }
  ],
  "testBankAccounts": [
    {
      "routing": "011401533",
      "account": "1234567890",
      "holderName": "John Doe",
      "type": "checking"
    },
    {
      "routing": "011401533",
      "account": "0987654321",
      "holderName": "Jane Smith",
      "type": "savings"
    }
  ]
}
```

### External Dependencies
- Fiat Gateway: RedotPay/Bridge sandbox
- Database: PostgreSQL
- Redis: Cache for exchange rates
- Circle SDK: Wallet balance

---

## 3. Test Scenarios

### 3.1 Bank Account Management

#### TC-301: Add Bank Account (Happy Path)
**Priority**: Critical 🔴
**Preconditions**: User is logged in

**Steps**:
1. Navigate to Bank Accounts page
2. Click "Add Bank Account" button
3. Enter valid account holder name: "John Doe"
4. Enter valid routing number: "011401533"
5. Enter valid account number: "1234567890"
6. Select account type: "Checking"
7. Enter bank name: "Wells Fargo"
8. Check "Set as primary" checkbox
9. Click "Submit"

**Expected Results**:
- Form validates successfully
- Bank account is created
- Success notification displayed
- Account appears in list
- Only last 4 digits shown: "•••• 7890"
- Account marked as "PRIMARY"
- Routing/account numbers encrypted in database

**Test Data**:
```
Routing: 011401533
Account: 1234567890
Holder: John Doe
Type: checking
```

---

#### TC-302: Add Bank Account - Invalid Routing Number
**Priority**: High 🟡
**Preconditions**: User is logged in

**Steps**:
1. Navigate to Bank Accounts page
2. Click "Add Bank Account"
3. Enter routing number: "123456789" (invalid checksum)
4. Tab out of field

**Expected Results**:
- Validation error displayed: "Invalid routing number checksum"
- Submit button disabled
- Form cannot be submitted

---

#### TC-303: Add Bank Account - Account Number Too Short
**Priority**: High 🟡

**Steps**:
1. Enter account number: "123" (less than 5 digits)
2. Tab out of field

**Expected Results**:
- Error: "Account number must be at least 5 digits"
- Submit disabled

---

#### TC-304: View Bank Accounts List
**Priority**: Critical 🔴
**Preconditions**: User has 2+ bank accounts

**Steps**:
1. Navigate to Bank Accounts page

**Expected Results**:
- All user's bank accounts displayed
- Primary account highlighted
- Last 4 digits shown for each
- Bank name and type visible
- "Edit" and "Delete" buttons present

---

#### TC-305: Edit Bank Account
**Priority**: Medium 🟢
**Preconditions**: User has at least 1 bank account

**Steps**:
1. Click "Edit" on a bank account
2. Change account holder name to "John Doe Jr."
3. Change bank name to "Chase Bank"
4. Click "Save"

**Expected Results**:
- Changes saved successfully
- Updated information displayed
- Cannot edit routing/account number (security)
- UpdatedAt timestamp changed

---

#### TC-306: Delete Bank Account
**Priority**: High 🟡
**Preconditions**: Bank account has no pending payouts

**Steps**:
1. Click "Delete" on a bank account
2. Confirm deletion in dialog

**Expected Results**:
- Confirmation dialog appears
- Bank account soft-deleted (DeletedAt set)
- Account no longer appears in list
- Success notification displayed

---

#### TC-307: Cannot Delete Bank Account with Pending Payout
**Priority**: Critical 🔴
**Preconditions**: Bank account has pending payout

**Steps**:
1. Initiate payout to bank account
2. Attempt to delete bank account

**Expected Results**:
- Error message: "Cannot delete bank account with pending payouts"
- Delete operation blocked
- Bank account remains in list

---

### 3.2 Bank Account Validation

#### TC-308: Routing Number Validation - Length Check
**Test Cases**:
| Input | Expected Result |
|-------|----------------|
| `12345678` (8 digits) | Error: "Must be 9 digits" |
| `011401533` (9 digits) | Valid ✓ |
| `0114015334` (10 digits) | Error: "Must be 9 digits" |

---

#### TC-309: Routing Number Validation - Checksum
**Test Cases**:
| Routing Number | Valid? | Note |
|----------------|--------|------|
| `011401533` | ✓ | Valid Wells Fargo routing |
| `123456789` | ✗ | Invalid checksum |
| `021000021` | ✓ | Valid JPMorgan Chase |

---

#### TC-310: Account Number Validation
**Test Cases**:
| Input | Expected Result |
|-------|----------------|
| `123` | Error: "At least 5 digits" |
| `12345` | Valid ✓ |
| `12345678901234567` (17 digits) | Valid ✓ |
| `123456789012345678` (18 digits) | Error: "At most 17 digits" |

---

#### TC-311: Account Holder Name Validation
**Test Cases**:
| Input | Expected Result |
|-------|----------------|
| `J` | Error: "At least 2 characters" |
| `John Doe` | Valid ✓ |
| `Mary-Jane O'Connor` | Valid ✓ |
| `John123` | Error: "Letters, spaces, hyphens only" |

---

### 3.3 Data Encryption

#### TC-312: Verify Routing Number Encrypted
**Priority**: Critical 🔴

**Steps**:
1. Add bank account with routing: "011401533"
2. Query database directly:
   ```sql
   SELECT "RoutingNumberEncrypted" FROM "BankAccounts"
   WHERE "LastFourDigits" = '7890';
   ```

**Expected Results**:
- `RoutingNumberEncrypted` column contains binary data (bytea)
- Data is NOT plaintext
- Length > 9 bytes (includes nonce + tag)

---

#### TC-313: Verify Account Number Encrypted
**Priority**: Critical 🔴

**Steps**:
1. Add bank account
2. Check `AccountNumberEncrypted` in database

**Expected Results**:
- Encrypted as bytea
- Cannot read plaintext
- Different encrypted value each time (unique nonce)

---

#### TC-314: Encryption/Decryption Round-Trip
**Priority**: Critical 🔴

**Steps**:
1. Add bank account
2. Retrieve bank account via API
3. Delete bank account
4. Add same account again

**Expected Results**:
- Data decrypts correctly
- Last 4 digits match original
- No data corruption

---

### 3.4 Fiat Withdrawal Flow

#### TC-315: Complete Withdrawal (Happy Path)
**Priority**: Critical 🔴
**Preconditions**:
- User has bank account
- User has 100+ USDC balance

**Steps**:
1. Navigate to Withdraw page
2. **Step 1 - Amount**:
   - Enter USDC amount: 100.00
   - View conversion preview
   - Click "Next"
3. **Step 2 - Bank Account**:
   - Select primary bank account
   - Click "Next"
4. **Step 3 - Review**:
   - Verify USDC amount: 100.00
   - Verify USD amount: ~99.98
   - Verify conversion fee: 1.50 (1.5%)
   - Verify payout fee: 1.00
   - Verify net amount: ~97.48
   - Click "Confirm"
5. **Step 4 - Confirmation**:
   - View success message
   - View payout ID
   - Click "Track Payout"

**Expected Results**:
- Each step validates correctly
- Can navigate back without losing data
- Fees calculated correctly
- Payout created in database
- Status: "pending"
- User balance reduced by 100 USDC

---

#### TC-316: Withdrawal - Insufficient Balance
**Priority**: High 🟡

**Steps**:
1. Enter USDC amount greater than balance
2. Click "Next"

**Expected Results**:
- Error: "Insufficient USDC balance"
- Cannot proceed
- Balance check happens client-side and server-side

---

#### TC-317: Exchange Rate Display
**Priority**: High 🟡

**Steps**:
1. Navigate to Withdraw page
2. Observe exchange rate display

**Expected Results**:
- Current rate displayed: "1 USDC = 0.9998 USD"
- Last updated timestamp shown
- Countdown to next refresh (30s)
- Manual refresh button works
- Rate updates every 30 seconds

---

#### TC-318: Conversion Calculator Real-Time
**Priority**: Medium 🟢

**Steps**:
1. Enter USDC amount: 100
2. Observe USD calculation
3. Change to 200
4. Observe recalculation

**Expected Results**:
- USD amount updates in real-time
- Fee breakdown updates
- Net amount updates
- Calculation accurate (within 0.01)

---

### 3.5 Payout Tracking

#### TC-319: View Payout Status
**Priority**: Critical 🔴
**Preconditions**: User has initiated payout

**Steps**:
1. Navigate to payout details page
2. View payout status

**Expected Results**:
- Current status displayed
- Progress bar shows current step
- Timeline shows events
- Last updated timestamp
- Auto-refreshes every 30 seconds

---

#### TC-320: Payout History
**Priority**: High 🟡

**Steps**:
1. Navigate to Payout History page

**Expected Results**:
- All payouts displayed
- Sorted by date (newest first)
- Shows: amount, bank, status, date
- Click card opens detail modal

---

#### TC-321: Filter Payout History by Status
**Priority**: Medium 🟢

**Steps**:
1. Select filter: "Completed"
2. View filtered list

**Expected Results**:
- Only completed payouts shown
- Filter applies correctly
- Count updated

---

## 4. Test Execution Schedule

### Week 1 (Day 1-5)
| Day | Tasks | Owner |
|-----|-------|-------|
| 1 | Create test plan, setup environment | QA Lead |
| 2-3 | Bank account tests (TC-301 to TC-314) | QA-1 |
| 4 | Encryption validation | QA Lead |
| 5 | Withdrawal flow tests (TC-315 to TC-318) | QA-2 |

### Week 2 (Day 6-10)
| Day | Tasks | Owner |
|-----|-------|-------|
| 6 | Payout tracking tests | QA-1 |
| 7 | E2E automation | QA-2 |
| 8 | Security testing | QA Lead |
| 9 | Performance testing | QA-1 |
| 10 | Test report | QA Lead |

---

## 5. Entry/Exit Criteria

### Entry Criteria
- [ ] All Phase 3 features deployed to QA environment
- [ ] Test data prepared
- [ ] Fiat gateway sandbox configured
- [ ] Test environment stable

### Exit Criteria
- [ ] All critical tests passed
- [ ] Zero critical bugs
- [ ] < 3 high priority bugs
- [ ] Security tests passed
- [ ] Performance tests met thresholds
- [ ] Test coverage > 95%
- [ ] Test report published

---

## 6. Risk Assessment

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Fiat gateway sandbox downtime | High | Medium | Use mock responses |
| Encryption key issues | High | Low | Verify key setup early |
| Database performance | Medium | Low | Test with realistic data volume |
| Browser compatibility | Medium | Low | Test on Chrome, Firefox, Safari |

---

## 7. Test Deliverables

- [ ] Test plan (this document)
- [ ] Test cases (detailed)
- [ ] Test execution reports
- [ ] Defect reports
- [ ] Security test report
- [ ] Performance test report
- [ ] Sprint N03 QA summary

---

## 8. Tools & Resources

### Testing Tools
- **Manual Testing**: Browser DevTools
- **E2E Automation**: Cypress
- **Performance**: K6
- **Security**: OWASP ZAP
- **Database**: PostgreSQL client (psql, pgAdmin)

### Documentation
- Sprint N03 Backend Plan
- Sprint N03 Frontend Plan
- API Documentation (Swagger)

---

**Test Plan Status**: ✅ READY FOR EXECUTION
**Next Steps**: Execute test cases as features are completed
**Last Updated**: 2025-10-29
