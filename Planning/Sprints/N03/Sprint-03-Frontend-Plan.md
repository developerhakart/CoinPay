# Sprint N03 Frontend Plan - Phase 3: Fiat Off-Ramp

**Sprint**: N03
**Duration**: 2 weeks (10 working days)
**Dates**: February 3-14, 2025
**Team Size**: 2-3 Frontend Engineers
**Total Effort**: ~22 days
**Owner**: Frontend Lead

---

## Sprint Goals

### Primary Goals

1. **Bank Account Management**: Build intuitive UI for adding, viewing, and managing bank accounts
2. **Fiat Withdrawal Flow**: Create seamless multi-step wizard for crypto-to-fiat conversion
3. **Payout Tracking**: Implement real-time payout status tracking and history

### Success Criteria

- ✅ Bank account form validates correctly (US ACH format)
- ✅ Withdrawal wizard completes in < 5 clicks
- ✅ Conversion calculator updates in real-time
- ✅ Exchange rates refresh automatically every 30 seconds
- ✅ Fee breakdown clearly displayed and transparent
- ✅ Payout status tracked with real-time updates
- ✅ Mobile responsive (tested on 3+ devices)
- ✅ Accessibility score > 90 (Lighthouse)
- ✅ Zero console errors in production build
- ✅ Component tests cover critical flows

---

## Architecture Overview

### New Components

```
CoinPay.Web/src/
├── pages/
│   ├── BankAccountsPage.tsx           # Bank account management
│   ├── FiatWithdrawalPage.tsx         # Withdrawal wizard
│   ├── PayoutHistoryPage.tsx          # Payout history list
│   └── PayoutDetailsPage.tsx          # Single payout details
├── components/
│   ├── BankAccounts/
│   │   ├── BankAccountForm.tsx        # Add/edit bank account
│   │   ├── BankAccountCard.tsx        # Bank account display card
│   │   ├── BankAccountList.tsx        # List of bank accounts
│   │   └── BankAccountValidator.ts    # Client-side validation
│   ├── Withdrawal/
│   │   ├── WithdrawalWizard.tsx       # Multi-step wizard
│   │   ├── AmountStep.tsx             # Step 1: Enter amount
│   │   ├── BankAccountStep.tsx        # Step 2: Select bank
│   │   ├── ReviewStep.tsx             # Step 3: Review details
│   │   ├── ConfirmationStep.tsx       # Step 4: Confirmation
│   │   ├── ConversionCalculator.tsx   # USDC→USD calculator
│   │   ├── ExchangeRateDisplay.tsx    # Real-time rate display
│   │   └── FeeBreakdown.tsx           # Fee transparency
│   ├── Payout/
│   │   ├── PayoutStatusBadge.tsx      # Status indicator
│   │   ├── PayoutCard.tsx             # Payout summary card
│   │   ├── PayoutDetailModal.tsx      # Full details modal
│   │   └── PayoutFilters.tsx          # History filters
│   └── Common/
│       ├── CurrencyInput.tsx          # Currency input component
│       └── LoadingSpinner.tsx         # Loading states
├── store/
│   ├── bankAccountStore.ts            # Bank account state
│   ├── payoutStore.ts                 # Payout state
│   └── exchangeRateStore.ts           # Exchange rate state
├── api/
│   ├── bankAccountApi.ts              # Bank account API calls
│   ├── payoutApi.ts                   # Payout API calls
│   └── exchangeRateApi.ts             # Exchange rate API calls
├── hooks/
│   ├── useBankAccounts.ts             # Bank account hook
│   ├── useExchangeRate.ts             # Exchange rate hook
│   └── usePayoutStatus.ts             # Payout polling hook
└── utils/
    ├── bankAccountValidation.ts       # Validation helpers
    ├── currencyFormatters.ts          # Currency formatting
    └── payoutHelpers.ts               # Payout utilities
```

---

## Task Breakdown

### Phase 3.1: Bank Account Management (7.00 days)

#### FE-301: Bank Account Form Component (2.00 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: BE-303

**Description**: Create reusable form component for adding/editing bank accounts with real-time validation.

**Features**:
- Account holder name input
- Routing number input (9 digits, formatted)
- Account number input (masked for security)
- Account type selector (checking/savings)
- Bank name input (optional)
- Primary account checkbox
- Real-time validation feedback
- Error message display

**Validation Rules**:
- Account holder name: 2-255 characters, letters/spaces/hyphens only
- Routing number: Exactly 9 digits, checksum validation
- Account number: 5-17 digits
- All required fields validated

**UI/UX**:
```tsx
<BankAccountForm
  mode="add" // or "edit"
  initialData={existingAccount}
  onSubmit={handleSubmit}
  onCancel={handleCancel}
/>
```

**Acceptance Criteria**:
- [ ] Form validates all fields client-side
- [ ] Routing number formatted as XXX-XXX-XXX
- [ ] Account number masked (shows only last 4)
- [ ] Real-time validation with debounce (300ms)
- [ ] Clear error messages for each field
- [ ] Submit button disabled until valid
- [ ] Loading state during submission
- [ ] Success/error notifications
- [ ] Mobile responsive
- [ ] Unit tests for validation logic
- [ ] Integration tests with form submission

**Files**:
- `components/BankAccounts/BankAccountForm.tsx`
- `utils/bankAccountValidation.ts`
- `tests/components/BankAccountForm.test.tsx`

---

#### FE-302: Bank Account List Component (1.50 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: BE-304

**Description**: Display list of user's bank accounts with edit/delete actions.

**Features**:
- List of bank account cards
- Bank name and logo (if available)
- Account type (checking/savings)
- Last 4 digits display
- Primary account indicator
- Edit button
- Delete button with confirmation
- Empty state (no accounts yet)

**Card Layout**:
```
┌────────────────────────────────────┐
│ 🏦 Chase Bank               [Edit] │
│ Checking •••• 3210     [PRIMARY]   │
│ Added: Feb 3, 2025         [Delete]│
└────────────────────────────────────┘
```

**Acceptance Criteria**:
- [ ] Displays all user's bank accounts
- [ ] Shows only last 4 digits
- [ ] Primary account highlighted
- [ ] Edit opens form in modal/side panel
- [ ] Delete shows confirmation dialog
- [ ] Empty state with "Add Bank Account" CTA
- [ ] Loading skeleton while fetching
- [ ] Mobile responsive (stacked cards)
- [ ] Unit tests for component
- [ ] Integration tests for edit/delete

**Files**:
- `components/BankAccounts/BankAccountList.tsx`
- `components/BankAccounts/BankAccountCard.tsx`
- `tests/components/BankAccountList.test.tsx`

---

#### FE-303: Bank Account Validation (Client-Side) (1.50 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: FE-301

**Description**: Implement comprehensive client-side validation for bank account data.

**Validation Functions**:

```typescript
// Routing number validation
export function validateRoutingNumber(routing: string): ValidationResult {
  // 1. Check length (9 digits)
  // 2. Validate checksum (ABA algorithm)
  // 3. Return error or success
}

// Account number validation
export function validateAccountNumber(account: string): ValidationResult {
  // 1. Check length (5-17 digits)
  // 2. Check format (numbers only)
  // 3. Return error or success
}

// Account holder name validation
export function validateAccountHolderName(name: string): ValidationResult {
  // 1. Check length (2-255 chars)
  // 2. Check format (letters, spaces, hyphens)
  // 3. Return error or success
}
```

**ABA Routing Number Checksum**:
```typescript
function validateRoutingChecksum(routing: string): boolean {
  const weights = [3, 7, 1, 3, 7, 1, 3, 7, 1];
  const sum = routing
    .split('')
    .reduce((acc, digit, i) => acc + parseInt(digit) * weights[i], 0);
  return sum % 10 === 0;
}
```

**Acceptance Criteria**:
- [ ] All validation functions implemented
- [ ] Routing number checksum validated
- [ ] Clear error messages for each validation
- [ ] Validation runs on blur and on submit
- [ ] Validation results cached (don't re-validate unnecessarily)
- [ ] Unit tests cover all validation rules
- [ ] Unit tests cover edge cases

**Files**:
- `utils/bankAccountValidation.ts`
- `tests/utils/bankAccountValidation.test.ts`

---

#### FE-304: Bank Account Management Page (2.00 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: FE-301, FE-302

**Description**: Complete page for managing bank accounts with add/edit/delete functionality.

**Page Layout**:
```
┌────────────────────────────────────────┐
│ Bank Accounts            [+ Add Bank] │
├────────────────────────────────────────┤
│ 🏦 Chase Bank                   [Edit]│
│ Checking •••• 3210         [PRIMARY]  │
│ Added: Feb 3, 2025            [Delete]│
├────────────────────────────────────────┤
│ 🏦 Bank of America             [Edit] │
│ Savings •••• 7890                     │
│ Added: Jan 15, 2025           [Delete]│
└────────────────────────────────────────┘
```

**Features**:
- Page header with "Add Bank" button
- List of bank accounts
- Add bank modal/drawer
- Edit bank modal/drawer
- Delete confirmation dialog
- Success/error notifications
- Loading states

**User Flows**:
1. **Add Bank Account**:
   - Click "Add Bank" → Modal opens
   - Fill form → Submit → Success notification
   - Modal closes → List refreshes

2. **Edit Bank Account**:
   - Click "Edit" → Modal opens with pre-filled data
   - Update fields → Submit → Success notification
   - Modal closes → List refreshes

3. **Delete Bank Account**:
   - Click "Delete" → Confirmation dialog
   - Confirm → Delete → Success notification
   - List refreshes

**Acceptance Criteria**:
- [ ] Page displays bank account list
- [ ] "Add Bank" opens form modal
- [ ] Form submission creates bank account
- [ ] Edit opens pre-filled form
- [ ] Delete shows confirmation dialog
- [ ] Success/error notifications displayed
- [ ] Loading states for all async operations
- [ ] Mobile responsive
- [ ] Navigation in sidebar/menu
- [ ] Unit tests for page component
- [ ] E2E tests for complete flows

**Files**:
- `pages/BankAccountsPage.tsx`
- `store/bankAccountStore.ts`
- `api/bankAccountApi.ts`
- `tests/pages/BankAccountsPage.test.tsx`

---

### Phase 3.2: Fiat Withdrawal Flow (9.50 days)

#### FE-305: Fiat Withdrawal Wizard (Multi-Step) (3.00 days)
**Priority**: HIGH 🔴
**Owner**: FE-2
**Dependencies**: BE-313

**Description**: Multi-step wizard for seamless fiat withdrawal experience.

**Wizard Steps**:
1. **Amount** - Enter USDC amount to withdraw
2. **Bank Account** - Select destination bank account
3. **Review** - Review conversion, fees, and details
4. **Confirmation** - Success screen with payout ID

**Step Navigation**:
```
┌──────────────────────────────────────┐
│  [1]Amount → [2]Bank → [3]Review → [4]✓ │
└──────────────────────────────────────┘
```

**State Management**:
```typescript
interface WithdrawalState {
  step: number;
  usdcAmount: number;
  bankAccountId: string;
  exchangeRate: number;
  fees: FeeBreakdown;
  payoutId?: string;
}
```

**Features**:
- Progress indicator
- Step validation (can't proceed if invalid)
- Back button (except on confirmation)
- Form state persistence (don't lose data when going back)
- Cancel button (confirmation dialog)
- Keyboard navigation (Enter to proceed, Esc to cancel)

**Acceptance Criteria**:
- [ ] 4-step wizard implemented
- [ ] Progress indicator shows current step
- [ ] Can navigate back to previous steps
- [ ] Form state persists across steps
- [ ] Validation prevents invalid progression
- [ ] Cancel shows confirmation dialog
- [ ] Mobile responsive (one column layout)
- [ ] Keyboard navigation works
- [ ] Unit tests for wizard logic
- [ ] Integration tests for complete flow

**Files**:
- `components/Withdrawal/WithdrawalWizard.tsx`
- `components/Withdrawal/AmountStep.tsx`
- `components/Withdrawal/BankAccountStep.tsx`
- `components/Withdrawal/ReviewStep.tsx`
- `components/Withdrawal/ConfirmationStep.tsx`
- `tests/components/WithdrawalWizard.test.tsx`

---

#### FE-306: USDC to USD Conversion Calculator (2.00 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: BE-310, BE-311

**Description**: Real-time conversion calculator showing USDC to USD with fees.

**Calculator Display**:
```
┌──────────────────────────────────────┐
│ You Send:        100.00 USDC         │
│ Exchange Rate:   1 USDC = 0.9998 USD │
│ USD Amount:      99.98 USD           │
│ - Conversion Fee: 1.50 USD (1.5%)   │
│ - Payout Fee:     1.00 USD           │
│ = You Receive:   97.48 USD           │
└──────────────────────────────────────┘
```

**Features**:
- USDC amount input
- Real-time USD calculation
- Fee breakdown (itemized)
- Net amount display (large, bold)
- Exchange rate with timestamp
- Rate expiration countdown (30s)
- Refresh button for rate
- Warning if rate is about to expire

**Calculation Logic**:
```typescript
interface CalculationResult {
  usdcAmount: number;
  exchangeRate: number;
  usdAmount: number;
  conversionFee: number;
  payoutFee: number;
  totalFees: number;
  netAmount: number;
  rateExpiresAt: Date;
}

function calculateConversion(
  usdcAmount: number,
  exchangeRate: number
): CalculationResult {
  const usdAmount = usdcAmount * exchangeRate;
  const conversionFee = usdAmount * 0.015; // 1.5%
  const payoutFee = 1.00; // flat fee
  const totalFees = conversionFee + payoutFee;
  const netAmount = usdAmount - totalFees;

  return {
    usdcAmount,
    exchangeRate,
    usdAmount,
    conversionFee,
    payoutFee,
    totalFees,
    netAmount,
    rateExpiresAt: new Date(Date.now() + 30000)
  };
}
```

**Acceptance Criteria**:
- [ ] Real-time calculation on amount change
- [ ] Debounced input (300ms)
- [ ] Fee breakdown clearly displayed
- [ ] Exchange rate updates every 30s
- [ ] Countdown shows rate expiration
- [ ] Refresh button fetches new rate
- [ ] Warning shown when rate about to expire
- [ ] Mobile responsive
- [ ] Unit tests for calculation logic
- [ ] Integration tests with API

**Files**:
- `components/Withdrawal/ConversionCalculator.tsx`
- `utils/currencyFormatters.ts`
- `tests/components/ConversionCalculator.test.tsx`

---

#### FE-307: Exchange Rate Display Component (1.00 day)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: BE-312

**Description**: Component to display current exchange rate with auto-refresh.

**Display**:
```
┌────────────────────────────────────┐
│ 💱 1 USDC = 0.9998 USD            │
│ Updated 15 seconds ago   [Refresh]│
│ Next update in 15s       ⟳        │
└────────────────────────────────────┘
```

**Features**:
- Current exchange rate display
- Last updated timestamp
- Next update countdown
- Manual refresh button
- Loading state during fetch
- Error state with retry
- Rate change indicator (↑ ↓)

**Auto-Refresh**:
```typescript
useEffect(() => {
  const fetchRate = async () => {
    const rate = await exchangeRateApi.getRate('USDC', 'USD');
    setExchangeRate(rate);
  };

  fetchRate(); // Initial fetch
  const interval = setInterval(fetchRate, 30000); // Every 30s

  return () => clearInterval(interval);
}, []);
```

**Acceptance Criteria**:
- [ ] Displays current exchange rate
- [ ] Auto-refreshes every 30 seconds
- [ ] Countdown shows time until next refresh
- [ ] Manual refresh button works
- [ ] Loading spinner during fetch
- [ ] Error message with retry button
- [ ] Rate change indicator (up/down arrow)
- [ ] Mobile responsive
- [ ] Unit tests for component
- [ ] Integration tests with API

**Files**:
- `components/Withdrawal/ExchangeRateDisplay.tsx`
- `hooks/useExchangeRate.ts`
- `api/exchangeRateApi.ts`
- `tests/components/ExchangeRateDisplay.test.tsx`

---

#### FE-308: Payout Confirmation Screen (1.50 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: FE-305

**Description**: Success screen after payout initiation with transaction details.

**Screen Layout**:
```
┌────────────────────────────────────┐
│         ✅ Payout Initiated        │
│                                    │
│  Payout ID: #PO-123456             │
│  Amount: 97.48 USD                 │
│  Bank: Chase •••• 3210             │
│  Status: Pending                   │
│                                    │
│  Estimated Arrival:                │
│  February 5, 2025                  │
│                                    │
│  [Track Payout]  [Return to Wallet]│
└────────────────────────────────────┘
```

**Features**:
- Success checkmark animation
- Payout ID (copyable)
- Amount and bank details
- Current status
- Estimated arrival date
- Action buttons:
  - "Track Payout" → Navigate to payout details
  - "Return to Wallet" → Navigate to wallet dashboard
- Share button (optional)
- Print receipt button (optional)

**Acceptance Criteria**:
- [ ] Success animation plays
- [ ] All transaction details displayed
- [ ] Payout ID copyable
- [ ] Buttons navigate correctly
- [ ] Mobile responsive
- [ ] Unit tests for component

**Files**:
- `components/Withdrawal/ConfirmationStep.tsx`
- `tests/components/ConfirmationStep.test.tsx`

---

#### FE-309: Payout Status Tracking Page (2.00 days)
**Priority**: HIGH 🔴
**Owner**: FE-2
**Dependencies**: BE-318

**Description**: Real-time payout status tracking with progress indicator.

**Page Layout**:
```
┌────────────────────────────────────┐
│ Payout #PO-123456                  │
├────────────────────────────────────┤
│ Status: Processing                 │
│ [●────○────○] Initiated → Processing → Completed │
│                                    │
│ Details:                           │
│ Amount: 97.48 USD                  │
│ Bank: Chase •••• 3210              │
│ Initiated: Feb 3, 2025 10:00 AM   │
│ Est. Arrival: Feb 5, 2025          │
│                                    │
│ Timeline:                          │
│ ✓ Feb 3, 10:00 - Payout initiated │
│ ✓ Feb 3, 10:05 - Processing       │
│ ⏳ Feb 5, 12:00 - Expected arrival │
└────────────────────────────────────┘
```

**Features**:
- Progress bar with status steps
- Current status highlighted
- Transaction details
- Status timeline (chronological)
- Last updated timestamp
- Auto-refresh every 30 seconds
- Manual refresh button
- Cancel button (if pending)

**Status Steps**:
1. Pending
2. Processing
3. Completed / Failed

**Auto-Polling**:
```typescript
useEffect(() => {
  const pollStatus = async () => {
    const status = await payoutApi.getStatus(payoutId);
    setPayoutStatus(status);

    // Stop polling if completed or failed
    if (['completed', 'failed'].includes(status.status)) {
      clearInterval(pollingInterval);
    }
  };

  pollStatus(); // Initial fetch
  const pollingInterval = setInterval(pollStatus, 30000);

  return () => clearInterval(pollingInterval);
}, [payoutId]);
```

**Acceptance Criteria**:
- [ ] Progress bar shows current step
- [ ] Timeline shows all status changes
- [ ] Auto-refreshes every 30 seconds
- [ ] Stops polling when completed/failed
- [ ] Manual refresh button works
- [ ] Cancel button shown for pending
- [ ] Mobile responsive
- [ ] Unit tests for component
- [ ] Integration tests with polling

**Files**:
- `pages/PayoutDetailsPage.tsx`
- `hooks/usePayoutStatus.ts`
- `tests/pages/PayoutDetailsPage.test.tsx`

---

#### FE-310: Payout History Page (2.50 days)
**Priority**: HIGH 🔴
**Owner**: FE-2
**Dependencies**: BE-317

**Description**: Paginated list of payout transactions with filtering and search.

**Page Layout**:
```
┌────────────────────────────────────┐
│ Payout History                     │
│ [All Status ▼] [Date Range ▼]     │
├────────────────────────────────────┤
│ Feb 4, 2025 - #PO-123456           │
│ 97.48 USD → Chase •••• 3210        │
│ Status: Completed ✓                │
├────────────────────────────────────┤
│ Feb 3, 2025 - #PO-123455           │
│ 150.00 USD → BofA •••• 7890        │
│ Status: Processing ⏳              │
├────────────────────────────────────┤
│ [Load More]                        │
└────────────────────────────────────┘
```

**Features**:
- List of payout cards
- Status filter (All, Pending, Processing, Completed, Failed)
- Date range filter (Last 7 days, Last 30 days, Custom)
- Sort by (Date, Amount)
- Pagination (load more / infinite scroll)
- Empty state (no payouts yet)
- Click card to view details

**Filters**:
```typescript
interface PayoutFilters {
  status?: 'pending' | 'processing' | 'completed' | 'failed';
  fromDate?: Date;
  toDate?: Date;
  sortBy: 'createdAt' | 'usdcAmount' | 'usdAmount';
  sortOrder: 'asc' | 'desc';
  page: number;
  pageSize: number;
}
```

**Acceptance Criteria**:
- [ ] Displays paginated payout list
- [ ] Status filter works correctly
- [ ] Date range filter works correctly
- [ ] Sort by date/amount works
- [ ] Pagination (load more) works
- [ ] Click card navigates to details
- [ ] Empty state displayed when no payouts
- [ ] Loading skeleton while fetching
- [ ] Mobile responsive
- [ ] Unit tests for component
- [ ] Integration tests with API

**Files**:
- `pages/PayoutHistoryPage.tsx`
- `components/Payout/PayoutCard.tsx`
- `components/Payout/PayoutFilters.tsx`
- `tests/pages/PayoutHistoryPage.test.tsx`

---

#### FE-311: Payout Detail Modal (1.50 days)
**Priority**: MEDIUM 🟡
**Owner**: FE-1
**Dependencies**: BE-319

**Description**: Modal or drawer showing complete payout transaction details.

**Modal Layout**:
```
┌────────────────────────────────────┐
│ Payout Details              [×]    │
├────────────────────────────────────┤
│ Payout ID: #PO-123456              │
│ Status: Completed ✓                │
│                                    │
│ Transaction Details:               │
│ • USDC Amount: 100.00 USDC         │
│ • Exchange Rate: 1 USDC = 0.9998   │
│ • USD Amount: 99.98 USD            │
│                                    │
│ Fees:                              │
│ • Conversion Fee: 1.50 USD (1.5%)  │
│ • Payout Fee: 1.00 USD             │
│ • Total Fees: 2.50 USD             │
│                                    │
│ Net Amount: 97.48 USD              │
│                                    │
│ Bank Account:                      │
│ • Bank: Chase Bank                 │
│ • Type: Checking                   │
│ • Account: •••• 3210               │
│                                    │
│ Timeline:                          │
│ • Initiated: Feb 3, 10:00 AM       │
│ • Completed: Feb 4, 3:30 PM        │
│                                    │
│ [Download Receipt]     [Close]     │
└────────────────────────────────────┘
```

**Features**:
- All transaction details
- Fee breakdown (itemized)
- Bank account details (masked)
- Timeline
- Download receipt button (PDF)
- Close button

**Acceptance Criteria**:
- [ ] Displays all payout details
- [ ] Fee breakdown clearly shown
- [ ] Bank account masked (last 4 digits)
- [ ] Timeline shows all events
- [ ] Download receipt generates PDF (optional)
- [ ] Close button works
- [ ] Mobile responsive (full screen on mobile)
- [ ] Unit tests for component

**Files**:
- `components/Payout/PayoutDetailModal.tsx`
- `tests/components/PayoutDetailModal.test.tsx`

---

#### FE-312: Fee Transparency UI (Breakdown) (1.50 days)
**Priority**: HIGH 🔴
**Owner**: FE-1
**Dependencies**: BE-311

**Description**: Clear, transparent fee breakdown component used throughout the app.

**Fee Breakdown Component**:
```
┌────────────────────────────────────┐
│ Fee Breakdown          [?]         │
├────────────────────────────────────┤
│ USD Amount:            99.98 USD   │
│ Conversion Fee (1.5%): -1.50 USD   │
│ Payout Fee:            -1.00 USD   │
│ ─────────────────────────────────  │
│ Total Fees:            -2.50 USD   │
│ You Receive:           97.48 USD   │
└────────────────────────────────────┘
```

**Features**:
- Itemized fee list
- Percentage shown for variable fees
- Tooltip explaining each fee
- Total fees highlighted
- Net amount (large, bold)
- Collapsible/expandable (optional)

**Fee Types**:
- Conversion fee (percentage-based)
- Payout fee (flat fee)
- Network fee (if applicable)

**Acceptance Criteria**:
- [ ] All fees clearly itemized
- [ ] Percentages shown for variable fees
- [ ] Tooltips explain each fee
- [ ] Total fees calculated correctly
- [ ] Net amount prominently displayed
- [ ] Mobile responsive
- [ ] Unit tests for component

**Files**:
- `components/Withdrawal/FeeBreakdown.tsx`
- `tests/components/FeeBreakdown.test.tsx`

---

### Phase 3.3: Supporting Components (5.50 days)

#### Common Components (Included in tasks above)

**CurrencyInput.tsx** (included in FE-306):
- Formatted currency input
- Locale-aware formatting
- Min/max validation
- Debounced input

**LoadingSpinner.tsx** (included in FE-304):
- Reusable loading spinner
- Different sizes (small, medium, large)
- Optional text label

**PayoutStatusBadge.tsx** (included in FE-309):
- Visual status indicator
- Color-coded by status
- Icon for each status

---

## State Management

### Zustand Stores

#### Bank Account Store
```typescript
interface BankAccountStore {
  bankAccounts: BankAccount[];
  loading: boolean;
  error: string | null;
  fetchBankAccounts: () => Promise<void>;
  addBankAccount: (data: BankAccountFormData) => Promise<void>;
  updateBankAccount: (id: string, data: BankAccountFormData) => Promise<void>;
  deleteBankAccount: (id: string) => Promise<void>;
}
```

#### Payout Store
```typescript
interface PayoutStore {
  payouts: PayoutTransaction[];
  currentPayout: PayoutTransaction | null;
  loading: boolean;
  error: string | null;
  fetchPayouts: (filters: PayoutFilters) => Promise<void>;
  initiatePayout: (data: PayoutRequest) => Promise<PayoutTransaction>;
  fetchPayoutStatus: (id: string) => Promise<PayoutStatus>;
  cancelPayout: (id: string) => Promise<void>;
}
```

#### Exchange Rate Store
```typescript
interface ExchangeRateStore {
  rate: ExchangeRate | null;
  loading: boolean;
  error: string | null;
  lastUpdated: Date | null;
  fetchExchangeRate: () => Promise<void>;
  refreshRate: () => Promise<void>;
}
```

---

## API Integration

### API Client Methods

```typescript
// Bank Account API
export const bankAccountApi = {
  getAll: () => api.get('/api/bank-account'),
  add: (data: BankAccountFormData) => api.post('/api/bank-account', data),
  update: (id: string, data: BankAccountFormData) =>
    api.put(`/api/bank-account/${id}`, data),
  delete: (id: string) => api.delete(`/api/bank-account/${id}`),
};

// Payout API
export const payoutApi = {
  initiate: (data: PayoutRequest) => api.post('/api/payout/initiate', data),
  getHistory: (filters: PayoutFilters) =>
    api.get('/api/payout/history', { params: filters }),
  getStatus: (id: string) => api.get(`/api/payout/${id}/status`),
  getDetails: (id: string) => api.get(`/api/payout/${id}/details`),
  cancel: (id: string) => api.post(`/api/payout/${id}/cancel`),
};

// Exchange Rate API
export const exchangeRateApi = {
  getRate: (from: string, to: string) =>
    api.get(`/api/rates/${from.toLowerCase()}-${to.toLowerCase()}`),
};
```

---

## UI/UX Guidelines

### Design Principles

1. **Clarity**: All fees and rates clearly displayed
2. **Transparency**: No hidden costs
3. **Feedback**: Immediate feedback for all actions
4. **Safety**: Confirmation for irreversible actions (payout, delete)
5. **Accessibility**: WCAG 2.1 AA compliant

### Color Scheme (Status)

```css
/* Status Colors */
.status-pending { color: #FFA500; }    /* Orange */
.status-processing { color: #2196F3; } /* Blue */
.status-completed { color: #4CAF50; }  /* Green */
.status-failed { color: #F44336; }     /* Red */
.status-cancelled { color: #9E9E9E; }  /* Gray */
```

### Responsive Breakpoints

```css
/* Mobile First */
@media (min-width: 640px) { /* sm */ }
@media (min-width: 768px) { /* md */ }
@media (min-width: 1024px) { /* lg */ }
@media (min-width: 1280px) { /* xl */ }
```

---

## Testing Strategy

### Unit Tests (Target: 80% coverage)

- Component rendering
- Form validation logic
- Calculation functions
- Store actions

### Integration Tests

- API integration
- Form submission flows
- Multi-step wizard navigation

### E2E Tests (Cypress)

- Complete bank account management flow
- Complete withdrawal wizard flow
- Payout history viewing

---

## Accessibility Requirements

### WCAG 2.1 AA Compliance

- [ ] Keyboard navigation (Tab, Enter, Esc)
- [ ] Screen reader support (ARIA labels)
- [ ] Color contrast > 4.5:1
- [ ] Focus indicators visible
- [ ] Error messages associated with inputs
- [ ] Form labels properly associated
- [ ] Semantic HTML (buttons, inputs, headings)

### Testing Tools

- Lighthouse (target score > 90)
- axe DevTools
- Screen reader testing (NVDA/VoiceOver)

---

## Performance Requirements

- Initial page load: < 2s
- Form interactions: < 100ms
- API responses: < 2s
- Exchange rate updates: Every 30s (background)
- Smooth animations (60 FPS)

---

## Timeline & Milestones

### Week 1 (February 3-7)

**Day 1-2**:
- FE-301: Bank account form ✅
- FE-303: Client-side validation ✅

**Day 3-4**:
- FE-302: Bank account list ✅
- FE-304: Bank account page ✅
- FE-305: Withdrawal wizard (started)

**Day 5** (Mid-Sprint Demo):
- FE-305: Withdrawal wizard (continued)
- Demo: Bank account management complete

### Week 2 (February 10-14)

**Day 6-7**:
- FE-305: Withdrawal wizard ✅
- FE-306: Conversion calculator ✅
- FE-307: Exchange rate display ✅
- FE-308: Confirmation screen ✅

**Day 8-9**:
- FE-309: Payout status tracking ✅
- FE-310: Payout history ✅
- FE-311: Payout detail modal ✅

**Day 10** (Sprint Review):
- FE-312: Fee breakdown ✅
- UI polish and accessibility audit
- Demo: Complete fiat off-ramp flow

---

## Definition of Done

- [ ] All 12 frontend tasks completed
- [ ] Bank account management functional
- [ ] Withdrawal wizard completes in < 5 clicks
- [ ] Payout tracking working with auto-refresh
- [ ] Mobile responsive (tested on 3 devices)
- [ ] Accessibility score > 90 (Lighthouse)
- [ ] Zero console errors
- [ ] Unit tests: > 80% coverage
- [ ] Integration tests: Critical flows pass
- [ ] E2E tests: All user journeys pass
- [ ] Code reviewed and approved
- [ ] No Critical/High bugs

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Complex wizard UX | High | Medium | Prototype early, user testing |
| Real-time rate updates | Medium | Low | Polling fallback, cached rates |
| Mobile form usability | Medium | Low | Test early on real devices |
| Accessibility compliance | Medium | Low | Use automated tools, manual testing |

---

**Document Owner**: Frontend Lead
**Last Updated**: 2025-10-29
**Version**: 1.0
