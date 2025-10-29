# CoinPay Wallet MVP - Sprint N02 Frontend Plan

**Version**: 1.0
**Sprint Duration**: 2 weeks (10 working days)
**Sprint Period**: January 20 - January 31, 2025
**Team Composition**: 2-3 Frontend Engineers
**Available Capacity**: 20-30 engineering days
**Planned Effort**: ~24 days (80% utilization)
**Sprint Type**: Feature Enhancement Sprint

---

## Sprint Goal

**Complete Phase 1 (Core Wallet UI) and implement Phase 2 (Transaction History & UI Polish) to deliver a professional, responsive, and performant wallet experience.**

By the end of Sprint N02, we will have:
- Phase 1 UI fully complete (dashboard, transfer, status display)
- Transaction history page with advanced filtering and search
- Transaction detail modal with blockchain explorer integration
- QR code generation for wallet address sharing
- Professional loading states and error handling
- Responsive design optimized for mobile, tablet, desktop
- Performance optimizations (code splitting, lazy loading)
- Accessibility improvements (WCAG 2.1 AA compliance)
- Component tests for critical flows

---

## Selected Tasks & Effort Distribution

### Phase 1: Core Wallet UI - COMPLETION (5.00 days)
- Wallet dashboard enhancement
- Transfer form completion
- Transaction status display
- Loading states and error handling

### Phase 2: Transaction History & UI Polish (17.00 days)
- Transaction history page with pagination
- Filters and search functionality
- Transaction detail modal
- QR code generation
- Copy-to-clipboard enhancements
- Loading skeletons and progress indicators
- Enhanced error handling with retry mechanisms
- Responsive design refinements
- Performance optimization

**Total Sprint N02 Effort**: ~22.00 days (within 20-30 day capacity)

---

## Task Breakdown with Details

### Epic 1.5: Phase 1 UI Completion (5.00 days)

#### FE-202: Wallet Dashboard Component Enhancement (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-201 (Wallet Creation from Sprint N01)

**Description**:
Enhance wallet dashboard with balance display, quick actions, and recent transactions.

**Technical Requirements**:
- Complete dashboard layout
- Balance card with refresh capability
- Quick action buttons (Send, Receive, QR Code)
- Recent transactions preview (last 5)
- Auto-refresh balance every 30 seconds
- Pull-to-refresh on mobile

**Component Structure**:
```typescript
// src/pages/wallet/Dashboard.tsx
export const WalletDashboard = () => {
  const { user } = useAuth();
  const { wallet, balance, refreshBalance } = useWallet();
  const { transactions, loading } = useTransactions(wallet?.id, 5);

  useEffect(() => {
    const interval = setInterval(() => {
      refreshBalance();
    }, 30000);  // Refresh every 30 seconds

    return () => clearInterval(interval);
  }, [refreshBalance]);

  return (
    <div className="wallet-dashboard">
      <WalletHeader wallet={wallet} />

      <BalanceCard
        balance={balance}
        onRefresh={refreshBalance}
        loading={loading}
      />

      <QuickActions
        onSend={() => navigate('/transfer')}
        onReceive={() => setShowQRCode(true)}
        onQRCode={() => setShowQRCode(true)}
      />

      <RecentTransactions
        transactions={transactions}
        onViewAll={() => navigate('/transactions')}
      />

      {showQRCode && (
        <QRCodeModal
          address={wallet.address}
          onClose={() => setShowQRCode(false)}
        />
      )}
    </div>
  );
};
```

**Sub-Components**:

**WalletHeader**:
```typescript
// src/components/wallet/WalletHeader.tsx
export const WalletHeader = ({ wallet }: Props) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    await navigator.clipboard.writeText(wallet.address);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="wallet-header">
      <h2>My Wallet</h2>
      <div className="address-container">
        <span className="address">{truncateAddress(wallet.address)}</span>
        <button onClick={handleCopy} className="copy-button">
          {copied ? <CheckIcon /> : <CopyIcon />}
        </button>
      </div>
    </div>
  );
};
```

**BalanceCard**:
```typescript
// src/components/wallet/BalanceCard.tsx
export const BalanceCard = ({ balance, onRefresh, loading }: Props) => {
  return (
    <div className="balance-card">
      <div className="balance-header">
        <span className="label">Total Balance</span>
        <button
          onClick={onRefresh}
          disabled={loading}
          className="refresh-button"
        >
          <RefreshIcon className={loading ? 'spin' : ''} />
        </button>
      </div>

      {loading ? (
        <BalanceSkeleton />
      ) : (
        <>
          <div className="balance-amount">
            <span className="amount">{balance.formatted}</span>
            <span className="currency">USDC</span>
          </div>
          <div className="balance-usd">
            ≈ ${balance.usdValue}
          </div>
        </>
      )}
    </div>
  );
};
```

**QuickActions**:
```typescript
// src/components/wallet/QuickActions.tsx
export const QuickActions = ({ onSend, onReceive, onQRCode }: Props) => {
  return (
    <div className="quick-actions">
      <button onClick={onSend} className="action-button primary">
        <SendIcon />
        <span>Send</span>
      </button>

      <button onClick={onReceive} className="action-button secondary">
        <ReceiveIcon />
        <span>Receive</span>
      </button>

      <button onClick={onQRCode} className="action-button secondary">
        <QRCodeIcon />
        <span>QR Code</span>
      </button>
    </div>
  );
};
```

**Acceptance Criteria**:
- [x] Dashboard displays wallet address with copy button
- [x] Balance card shows current USDC balance
- [x] Refresh button works (manual refresh)
- [x] Auto-refresh every 30 seconds
- [x] Quick action buttons navigate correctly
- [x] Recent transactions preview (last 5)
- [x] Loading states implemented
- [x] Responsive design (mobile/tablet/desktop)

**Definition of Done**:
- Component tested on multiple devices
- UI reviewed for UX
- Accessible (keyboard navigation, ARIA labels)

---

#### FE-203: Transfer Form UI Completion (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-202

**Description**:
Complete transfer form with validation, confirmation, and error handling.

**Technical Requirements**:
- Transfer form with recipient address and amount inputs
- Form validation (address format, amount range)
- Balance check before submission
- Confirmation modal before sending
- Error handling with user-friendly messages
- "Max" button to send entire balance
- Transaction fee display (0.00 for gasless)

**Component**:
```typescript
// src/pages/wallet/TransferPage.tsx
export const TransferPage = () => {
  const { wallet, balance } = useWallet();
  const [recipient, setRecipient] = useState('');
  const [amount, setAmount] = useState('');
  const [errors, setErrors] = useState<ValidationErrors>({});
  const [showConfirmation, setShowConfirmation] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate
    const validationErrors = validateTransferForm({ recipient, amount, balance });
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    // Show confirmation
    setShowConfirmation(true);
  };

  const handleConfirm = async () => {
    setIsSubmitting(true);
    setShowConfirmation(false);

    try {
      const result = await walletService.transfer({
        toAddress: recipient,
        amount: parseFloat(amount),
      });

      toast.success('Transfer submitted successfully!');
      navigate(`/transactions/${result.id}`);
    } catch (error) {
      toast.error(error.message || 'Transfer failed');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="transfer-page">
      <h1>Send USDC</h1>

      <form onSubmit={handleSubmit} className="transfer-form">
        <Input
          label="Recipient Address"
          value={recipient}
          onChange={(e) => setRecipient(e.target.value)}
          placeholder="0x..."
          error={errors.recipient}
          helpText="Enter the recipient's wallet address"
        />

        <Input
          label="Amount (USDC)"
          type="number"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          placeholder="0.00"
          error={errors.amount}
          step="0.000001"
          min="0"
          rightElement={
            <button
              type="button"
              onClick={() => setAmount(balance.formatted)}
              className="max-button"
            >
              Max
            </button>
          }
        />

        <BalanceInfo
          available={balance.formatted}
          amount={amount}
        />

        <FeeInfo fee="0.00" currency="USDC" gasless />

        <Button
          type="submit"
          loading={isSubmitting}
          disabled={!recipient || !amount || parseFloat(amount) <= 0}
          fullWidth
        >
          Send USDC
        </Button>
      </form>

      {showConfirmation && (
        <TransferConfirmationModal
          recipient={recipient}
          amount={amount}
          onConfirm={handleConfirm}
          onCancel={() => setShowConfirmation(false)}
        />
      )}
    </div>
  );
};
```

**Validation**:
```typescript
// src/utils/validation.ts
export const validateTransferForm = ({ recipient, amount, balance }: Props) => {
  const errors: ValidationErrors = {};

  // Validate recipient address
  if (!recipient) {
    errors.recipient = 'Recipient address is required';
  } else if (!isValidEthereumAddress(recipient)) {
    errors.recipient = 'Invalid Ethereum address format';
  }

  // Validate amount
  if (!amount) {
    errors.amount = 'Amount is required';
  } else {
    const amountNum = parseFloat(amount);
    if (isNaN(amountNum) || amountNum <= 0) {
      errors.amount = 'Amount must be greater than 0';
    } else if (amountNum > parseFloat(balance.formatted)) {
      errors.amount = 'Insufficient balance';
    }
  }

  return errors;
};

export const isValidEthereumAddress = (address: string): boolean => {
  return /^0x[a-fA-F0-9]{40}$/.test(address);
};
```

**Confirmation Modal**:
```typescript
// src/components/wallet/TransferConfirmationModal.tsx
export const TransferConfirmationModal = ({
  recipient,
  amount,
  onConfirm,
  onCancel
}: Props) => {
  return (
    <Modal onClose={onCancel}>
      <div className="confirmation-modal">
        <h2>Confirm Transfer</h2>

        <div className="transfer-details">
          <DetailRow label="Recipient" value={truncateAddress(recipient)} />
          <DetailRow label="Amount" value={`${amount} USDC`} />
          <DetailRow label="Gas Fee" value="0.00 USDC (Gasless)" />
        </div>

        <p className="warning">
          Please verify the recipient address. This transaction cannot be reversed.
        </p>

        <div className="actions">
          <Button variant="secondary" onClick={onCancel}>
            Cancel
          </Button>
          <Button variant="primary" onClick={onConfirm}>
            Confirm & Send
          </Button>
        </div>
      </div>
    </Modal>
  );
};
```

**Acceptance Criteria**:
- [x] Transfer form validates recipient address
- [x] Transfer form validates amount (> 0, <= balance)
- [x] "Max" button fills entire balance
- [x] Confirmation modal shows before sending
- [x] Transfer submits to Backend API
- [x] UserOpHash returned and displayed
- [x] Navigates to transaction status page
- [x] Error messages displayed for failures
- [x] Loading state during submission

**Definition of Done**:
- Transfer tested with testnet USDC
- Validation tested (all error cases)
- UI reviewed for UX

---

#### FE-204: Transaction Status Display (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: FE-203

**Description**:
Create component to display real-time transaction status.

**Technical Requirements**:
- Display transaction status (Pending, Confirmed, Failed)
- Poll for status updates every 5 seconds
- Show transaction details
- Link to block explorer (Polygon Amoy)
- Stop polling when transaction confirmed/failed

**Component**:
```typescript
// src/components/wallet/TransactionStatusDisplay.tsx
export const TransactionStatusDisplay = ({ transactionId }: Props) => {
  const [transaction, setTransaction] = useState<Transaction | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchStatus = async () => {
      try {
        const result = await transactionService.getStatus(transactionId);
        setTransaction(result);

        // Stop polling if confirmed or failed
        if (result.status !== 'Pending') {
          clearInterval(intervalId);
        }
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    // Initial fetch
    fetchStatus();

    // Poll every 5 seconds
    const intervalId = setInterval(fetchStatus, 5000);

    return () => clearInterval(intervalId);
  }, [transactionId]);

  if (loading) return <LoadingSkeleton />;
  if (error) return <ErrorMessage message={error} />;
  if (!transaction) return <NotFoundMessage />;

  return (
    <div className="transaction-status">
      <StatusBadge status={transaction.status} />

      <div className="transaction-info">
        <InfoRow label="Amount" value={`${transaction.amount} USDC`} />
        <InfoRow label="To" value={truncateAddress(transaction.toAddress)} />
        <InfoRow label="Status" value={transaction.status} />
        {transaction.status === 'Pending' && (
          <LoadingSpinner text="Confirming transaction..." />
        )}
      </div>

      {transaction.transactionHash && (
        <a
          href={`https://amoy.polygonscan.com/tx/${transaction.transactionHash}`}
          target="_blank"
          rel="noopener noreferrer"
          className="explorer-link"
        >
          View on Block Explorer <ExternalLinkIcon />
        </a>
      )}

      {transaction.status === 'Confirmed' && (
        <SuccessMessage text="Transaction confirmed!" />
      )}

      {transaction.status === 'Failed' && (
        <ErrorMessage text="Transaction failed. Please try again." />
      )}
    </div>
  );
};
```

**StatusBadge Component**:
```typescript
// src/components/common/StatusBadge.tsx
export const StatusBadge = ({ status }: Props) => {
  const statusConfig = {
    Pending: { color: 'yellow', icon: <ClockIcon />, label: 'Pending' },
    Confirmed: { color: 'green', icon: <CheckIcon />, label: 'Confirmed' },
    Failed: { color: 'red', icon: <XIcon />, label: 'Failed' },
  };

  const config = statusConfig[status];

  return (
    <span className={`status-badge status-${config.color}`}>
      {config.icon}
      {config.label}
    </span>
  );
};
```

**Acceptance Criteria**:
- [x] Transaction status displayed correctly
- [x] Polling updates status every 5 seconds
- [x] Block explorer link works (when confirmed)
- [x] Status changes reflected in UI
- [x] Polling stops when confirmed/failed
- [x] Loading state while fetching
- [x] Error handling for failed requests

**Definition of Done**:
- Component tested with real transactions
- UI reviewed
- Polling logic tested

---

### Epic 2.3: Transaction History & UI Polish (17.00 days)

#### FE-205: Transaction History Page (3.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-203 (Transaction History API)

**Description**:
Build comprehensive transaction history page with pagination.

**Technical Requirements**:
- Display transaction list with pagination
- Show transaction details (amount, recipient, status, date)
- Pagination controls (prev/next, page numbers)
- Empty state when no transactions
- Loading state with skeleton
- Responsive design

**Component**:
```typescript
// src/pages/transactions/TransactionHistoryPage.tsx
export const TransactionHistoryPage = () => {
  const { wallet } = useWallet();
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [pagination, setPagination] = useState<PaginationMetadata>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTransactions();
  }, [page, wallet]);

  const fetchTransactions = async () => {
    setLoading(true);
    try {
      const result = await transactionService.getHistory({
        walletId: wallet.id,
        page,
        pageSize: 20,
      });

      setTransactions(result.transactions);
      setPagination(result.pagination);
    } catch (error) {
      toast.error('Failed to load transaction history');
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <TransactionListSkeleton count={10} />;

  if (transactions.length === 0) {
    return (
      <EmptyState
        icon={<TransactionIcon />}
        title="No Transactions Yet"
        description="Your transaction history will appear here once you send or receive USDC."
        action={
          <Button onClick={() => navigate('/transfer')}>
            Send USDC
          </Button>
        }
      />
    );
  }

  return (
    <div className="transaction-history-page">
      <h1>Transaction History</h1>

      <TransactionList transactions={transactions} />

      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        onPageChange={setPage}
        hasPrevious={pagination.hasPrevious}
        hasNext={pagination.hasNext}
      />
    </div>
  );
};
```

**TransactionList**:
```typescript
// src/components/transactions/TransactionList.tsx
export const TransactionList = ({ transactions }: Props) => {
  const [selectedTransaction, setSelectedTransaction] = useState<Transaction | null>(null);

  return (
    <>
      <div className="transaction-list">
        {transactions.map((transaction) => (
          <TransactionCard
            key={transaction.id}
            transaction={transaction}
            onClick={() => setSelectedTransaction(transaction)}
          />
        ))}
      </div>

      {selectedTransaction && (
        <TransactionDetailModal
          transaction={selectedTransaction}
          onClose={() => setSelectedTransaction(null)}
        />
      )}
    </>
  );
};
```

**TransactionCard**:
```typescript
// src/components/transactions/TransactionCard.tsx
export const TransactionCard = ({ transaction, onClick }: Props) => {
  const isSent = transaction.fromAddress === wallet.address;

  return (
    <div className="transaction-card" onClick={onClick}>
      <div className="card-icon">
        {isSent ? <SendIcon /> : <ReceiveIcon />}
      </div>

      <div className="card-content">
        <div className="card-header">
          <span className="type">{isSent ? 'Sent' : 'Received'}</span>
          <StatusBadge status={transaction.status} />
        </div>

        <div className="card-details">
          <span className="address">
            {isSent ? 'To' : 'From'}: {truncateAddress(
              isSent ? transaction.toAddress : transaction.fromAddress
            )}
          </span>
          <span className="date">{formatDate(transaction.createdAt)}</span>
        </div>
      </div>

      <div className="card-amount">
        <span className={`amount ${isSent ? 'sent' : 'received'}`}>
          {isSent ? '-' : '+'}{transaction.formattedAmount} USDC
        </span>
      </div>
    </div>
  );
};
```

**Pagination Component**:
```typescript
// src/components/common/Pagination.tsx
export const Pagination = ({
  currentPage,
  totalPages,
  onPageChange,
  hasPrevious,
  hasNext
}: Props) => {
  return (
    <div className="pagination">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={!hasPrevious}
        className="pagination-button"
      >
        <ChevronLeftIcon /> Previous
      </button>

      <span className="page-info">
        Page {currentPage} of {totalPages}
      </span>

      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={!hasNext}
        className="pagination-button"
      >
        Next <ChevronRightIcon />
      </button>
    </div>
  );
};
```

**Acceptance Criteria**:
- [x] Transaction history displays all transactions
- [x] Pagination works (prev/next)
- [x] Transaction cards show key information
- [x] Click on transaction opens detail modal
- [x] Empty state shows when no transactions
- [x] Loading state with skeleton
- [x] Responsive design (mobile/tablet/desktop)

**Definition of Done**:
- Page tested with 100+ transactions
- UI reviewed
- Responsive design tested

---

#### FE-206: Transaction Filters & Search Component (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-205, BE-204, BE-205

**Description**:
Add filtering and search functionality to transaction history.

**Technical Requirements**:
- Filter by status (Pending, Confirmed, Failed)
- Filter by date range (Last 7 days, Last 30 days, Custom range)
- Sort by date or amount
- Search by recipient address or transaction hash
- Filter chips to show active filters
- Clear filters button

**Component**:
```typescript
// src/components/transactions/TransactionFilters.tsx
export const TransactionFilters = ({ onFilterChange }: Props) => {
  const [status, setStatus] = useState<TransactionStatus | null>(null);
  const [dateRange, setDateRange] = useState<DateRange>('all');
  const [sortBy, setSortBy] = useState<'date' | 'amount'>('date');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    // Build filter object
    const filters = {
      status,
      dateRange,
      sortBy,
      sortOrder,
      searchQuery,
    };

    onFilterChange(filters);
  }, [status, dateRange, sortBy, sortOrder, searchQuery]);

  const handleClearFilters = () => {
    setStatus(null);
    setDateRange('all');
    setSortBy('date');
    setSortOrder('desc');
    setSearchQuery('');
  };

  const activeFilterCount = [status, dateRange !== 'all', searchQuery].filter(Boolean).length;

  return (
    <div className="transaction-filters">
      <div className="filter-row">
        <SearchInput
          value={searchQuery}
          onChange={setSearchQuery}
          placeholder="Search by address or transaction hash..."
        />

        <Select
          value={status || 'all'}
          onChange={(value) => setStatus(value === 'all' ? null : value)}
          options={[
            { label: 'All Status', value: 'all' },
            { label: 'Pending', value: 'Pending' },
            { label: 'Confirmed', value: 'Confirmed' },
            { label: 'Failed', value: 'Failed' },
          ]}
        />

        <Select
          value={dateRange}
          onChange={setDateRange}
          options={[
            { label: 'All Time', value: 'all' },
            { label: 'Last 7 Days', value: '7days' },
            { label: 'Last 30 Days', value: '30days' },
            { label: 'Custom Range', value: 'custom' },
          ]}
        />

        <Select
          value={`${sortBy}-${sortOrder}`}
          onChange={(value) => {
            const [by, order] = value.split('-');
            setSortBy(by);
            setSortOrder(order);
          }}
          options={[
            { label: 'Date (Newest First)', value: 'date-desc' },
            { label: 'Date (Oldest First)', value: 'date-asc' },
            { label: 'Amount (Highest First)', value: 'amount-desc' },
            { label: 'Amount (Lowest First)', value: 'amount-asc' },
          ]}
        />
      </div>

      {activeFilterCount > 0 && (
        <div className="active-filters">
          <span className="filter-label">Active Filters:</span>
          {status && <FilterChip label={status} onRemove={() => setStatus(null)} />}
          {dateRange !== 'all' && (
            <FilterChip label={dateRange} onRemove={() => setDateRange('all')} />
          )}
          {searchQuery && (
            <FilterChip
              label={`Search: ${truncate(searchQuery, 20)}`}
              onRemove={() => setSearchQuery('')}
            />
          )}
          <button onClick={handleClearFilters} className="clear-filters">
            Clear All
          </button>
        </div>
      )}
    </div>
  );
};
```

**FilterChip**:
```typescript
// src/components/common/FilterChip.tsx
export const FilterChip = ({ label, onRemove }: Props) => {
  return (
    <span className="filter-chip">
      {label}
      <button onClick={onRemove} className="remove-button">
        <XIcon size={14} />
      </button>
    </span>
  );
};
```

**Updated TransactionHistoryPage**:
```typescript
export const TransactionHistoryPage = () => {
  const [filters, setFilters] = useState<TransactionFilters>({});

  const fetchTransactions = async () => {
    // ... existing code

    const result = await transactionService.getHistory({
      walletId: wallet.id,
      page,
      pageSize: 20,
      ...filters,  // Apply filters
    });

    // ... existing code
  };

  return (
    <div className="transaction-history-page">
      <h1>Transaction History</h1>

      <TransactionFilters onFilterChange={setFilters} />

      <TransactionList transactions={transactions} />

      <Pagination {...paginationProps} />
    </div>
  );
};
```

**Acceptance Criteria**:
- [x] Status filter works (Pending, Confirmed, Failed)
- [x] Date range filter works (Last 7/30 days)
- [x] Sort by date and amount works
- [x] Search by address or transaction hash works
- [x] Filter chips display active filters
- [x] Clear filters button works
- [x] Filters applied to API request
- [x] Responsive design

**Definition of Done**:
- Filters tested with various combinations
- UI reviewed
- Performance acceptable

---

#### FE-207: Transaction Detail Modal (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: BE-206 (Transaction Detail API)

**Description**:
Create modal to display complete transaction details.

**Component**:
```typescript
// src/components/transactions/TransactionDetailModal.tsx
export const TransactionDetailModal = ({ transaction, onClose }: Props) => {
  const [details, setDetails] = useState<TransactionDetails | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDetails();
  }, [transaction.id]);

  const fetchDetails = async () => {
    try {
      const result = await transactionService.getDetails(transaction.id);
      setDetails(result);
    } catch (error) {
      toast.error('Failed to load transaction details');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal onClose={onClose} size="large">
      <div className="transaction-detail-modal">
        <h2>Transaction Details</h2>

        {loading ? (
          <LoadingSkeleton />
        ) : (
          <>
            <StatusBadge status={details.status} size="large" />

            <section className="detail-section">
              <h3>Transaction Information</h3>
              <DetailRow label="Amount" value={`${details.formattedAmount} USDC`} />
              <DetailRow
                label="From"
                value={details.fromAddress}
                copyable
              />
              <DetailRow
                label="To"
                value={details.toAddress}
                copyable
              />
              <DetailRow label="Status" value={details.status} />
              <DetailRow label="Submitted" value={formatDateTime(details.createdAt)} />
              {details.confirmedAt && (
                <DetailRow
                  label="Confirmed"
                  value={formatDateTime(details.confirmedAt)}
                />
              )}
            </section>

            {details.status === 'Confirmed' && (
              <section className="detail-section">
                <h3>Blockchain Information</h3>
                <DetailRow
                  label="Transaction Hash"
                  value={details.transactionHash}
                  copyable
                  link={`https://amoy.polygonscan.com/tx/${details.transactionHash}`}
                />
                <DetailRow label="Block Number" value={details.blockNumber} />
                <DetailRow label="Confirmations" value={details.confirmations} />
                <DetailRow label="Gas Used" value={details.gasUsed} />
                <DetailRow
                  label="Gas Paid by User"
                  value="0 USDC (Gasless)"
                  highlight="success"
                />
              </section>
            )}

            <section className="detail-section">
              <h3>UserOperation Details</h3>
              <DetailRow
                label="UserOp Hash"
                value={details.userOpHash}
                copyable
              />
              <DetailRow label="Nonce" value={details.nonce} />
            </section>

            <div className="modal-actions">
              {details.transactionHash && (
                <Button
                  variant="secondary"
                  onClick={() => window.open(
                    `https://amoy.polygonscan.com/tx/${details.transactionHash}`,
                    '_blank'
                  )}
                >
                  View on Explorer <ExternalLinkIcon />
                </Button>
              )}
              <Button variant="primary" onClick={onClose}>
                Close
              </Button>
            </div>
          </>
        )}
      </div>
    </Modal>
  );
};
```

**DetailRow Component**:
```typescript
// src/components/common/DetailRow.tsx
export const DetailRow = ({ label, value, copyable, link, highlight }: Props) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    await navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="detail-row">
      <span className="label">{label}</span>
      <div className="value-container">
        {link ? (
          <a href={link} target="_blank" rel="noopener noreferrer" className="value-link">
            {truncate(value, 20)} <ExternalLinkIcon size={14} />
          </a>
        ) : (
          <span className={`value ${highlight ? `highlight-${highlight}` : ''}`}>
            {value}
          </span>
        )}
        {copyable && (
          <button onClick={handleCopy} className="copy-button">
            {copied ? <CheckIcon /> : <CopyIcon />}
          </button>
        )}
      </div>
    </div>
  );
};
```

**Acceptance Criteria**:
- [x] Modal displays complete transaction details
- [x] Blockchain information shown (if confirmed)
- [x] Copy buttons work for addresses and hashes
- [x] External links to block explorer work
- [x] Gasless transaction highlighted
- [x] Loading state while fetching
- [x] Close button works
- [x] Responsive design

**Definition of Done**:
- Modal tested with various transaction states
- UI reviewed
- Accessible (keyboard navigation, focus management)

---

#### FE-208: QR Code Generation for Wallet Address (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: FE-202 (Dashboard)

**Description**:
Implement QR code generation for easy wallet address sharing.

**Technical Requirements**:
- Install qrcode.react library
- Generate QR code from wallet address
- Display in modal with wallet address
- Download QR code as image
- Share functionality (if supported)

**Installation**:
```bash
npm install qrcode.react
```

**Component**:
```typescript
// src/components/wallet/QRCodeModal.tsx
import QRCode from 'qrcode.react';

export const QRCodeModal = ({ address, onClose }: Props) => {
  const qrRef = useRef<HTMLDivElement>(null);

  const handleDownload = () => {
    const canvas = qrRef.current.querySelector('canvas');
    if (canvas) {
      const url = canvas.toDataURL('image/png');
      const link = document.createElement('a');
      link.href = url;
      link.download = `wallet-address-${truncateAddress(address)}.png`;
      link.click();
    }
  };

  const handleShare = async () => {
    if (navigator.share) {
      try {
        await navigator.share({
          title: 'My Wallet Address',
          text: `Send USDC to my wallet:\n${address}`,
        });
      } catch (error) {
        // User cancelled share
      }
    } else {
      // Fallback: copy to clipboard
      await navigator.clipboard.writeText(address);
      toast.success('Address copied to clipboard');
    }
  };

  return (
    <Modal onClose={onClose}>
      <div className="qrcode-modal">
        <h2>Receive USDC</h2>

        <p className="description">
          Scan this QR code to receive USDC at your wallet address
        </p>

        <div className="qrcode-container" ref={qrRef}>
          <QRCode
            value={address}
            size={256}
            level="H"
            includeMargin
          />
        </div>

        <div className="address-display">
          <span className="address">{address}</span>
          <button
            onClick={() => navigator.clipboard.writeText(address)}
            className="copy-button"
          >
            <CopyIcon />
          </button>
        </div>

        <div className="modal-actions">
          <Button variant="secondary" onClick={handleDownload}>
            <DownloadIcon /> Download QR Code
          </Button>
          <Button variant="primary" onClick={handleShare}>
            <ShareIcon /> Share Address
          </Button>
        </div>
      </div>
    </Modal>
  );
};
```

**Acceptance Criteria**:
- [x] QR code generated from wallet address
- [x] QR code scannable by standard QR code apps
- [x] Download button saves QR code as PNG
- [x] Share button copies address (or uses Web Share API)
- [x] Wallet address displayed with copy button
- [x] Modal closes correctly
- [x] Responsive design

**Definition of Done**:
- QR code tested with multiple QR readers
- Download functionality tested
- UI reviewed

---

#### FE-209: Copy-to-Clipboard Enhancements (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P2 - Medium
**Dependencies**: All components with copy functionality

**Description**:
Enhance copy-to-clipboard functionality with better UX.

**Technical Requirements**:
- Unified copy button component
- Toast notification on copy
- Visual feedback (checkmark icon)
- Keyboard shortcut support (Ctrl+C)
- Error handling for unsupported browsers

**Component**:
```typescript
// src/components/common/CopyButton.tsx
export const CopyButton = ({ value, label }: Props) => {
  const [copied, setCopied] = useState(false);
  const timeoutRef = useRef<NodeJS.Timeout>(null);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(value);
      setCopied(true);

      toast.success(`${label || 'Value'} copied to clipboard`);

      // Reset after 2 seconds
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
      timeoutRef.current = setTimeout(() => setCopied(false), 2000);
    } catch (error) {
      toast.error('Failed to copy to clipboard');
    }
  };

  return (
    <button
      onClick={handleCopy}
      className="copy-button"
      title={`Copy ${label || 'value'}`}
    >
      {copied ? (
        <>
          <CheckIcon className="icon-success" />
          <span className="sr-only">Copied!</span>
        </>
      ) : (
        <>
          <CopyIcon />
          <span className="sr-only">Copy</span>
        </>
      )}
    </button>
  );
};
```

**Toast Notification**:
```typescript
// src/utils/toast.ts
import { toast as sonner } from 'sonner';

export const toast = {
  success: (message: string) => {
    sonner.success(message, {
      duration: 2000,
      position: 'bottom-right',
    });
  },

  error: (message: string) => {
    sonner.error(message, {
      duration: 3000,
      position: 'bottom-right',
    });
  },

  info: (message: string) => {
    sonner.info(message, {
      duration: 2000,
      position: 'bottom-right',
    });
  },
};
```

**Acceptance Criteria**:
- [x] Copy button component reusable across app
- [x] Toast notification shows on successful copy
- [x] Visual feedback (checkmark icon) for 2 seconds
- [x] Error handling for unsupported browsers
- [x] Accessible (keyboard navigation, screen reader support)

**Definition of Done**:
- Copy button tested in all locations
- UI reviewed
- Accessible

---

#### FE-210: Loading Skeletons & Progress Indicators (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: All pages and components

**Description**:
Implement professional loading states across the application.

**Technical Requirements**:
- Loading skeletons for all list/grid components
- Progress bars for long-running operations
- Shimmer effect for placeholder content
- Spinner for button loading states
- Page-level loading indicators

**Skeleton Components**:
```typescript
// src/components/common/Skeleton.tsx
export const Skeleton = ({ width, height, className }: Props) => {
  return (
    <div
      className={`skeleton ${className}`}
      style={{ width, height }}
    />
  );
};

export const SkeletonText = ({ lines = 1 }: Props) => {
  return (
    <div className="skeleton-text">
      {Array.from({ length: lines }).map((_, i) => (
        <Skeleton key={i} height="16px" width={`${80 + Math.random() * 20}%`} />
      ))}
    </div>
  );
};

// Transaction Card Skeleton
export const TransactionCardSkeleton = () => {
  return (
    <div className="transaction-card skeleton-card">
      <Skeleton width="48px" height="48px" className="skeleton-circle" />
      <div className="skeleton-content">
        <Skeleton width="120px" height="16px" />
        <Skeleton width="180px" height="14px" />
      </div>
      <Skeleton width="80px" height="20px" />
    </div>
  );
};

// Transaction List Skeleton
export const TransactionListSkeleton = ({ count = 5 }: Props) => {
  return (
    <div className="transaction-list">
      {Array.from({ length: count }).map((_, i) => (
        <TransactionCardSkeleton key={i} />
      ))}
    </div>
  );
};

// Balance Card Skeleton
export const BalanceSkeleton = () => {
  return (
    <div className="balance-skeleton">
      <Skeleton width="200px" height="48px" />
      <Skeleton width="120px" height="24px" />
    </div>
  );
};
```

**Shimmer Effect (CSS)**:
```css
.skeleton {
  background: linear-gradient(
    90deg,
    #f0f0f0 25%,
    #e0e0e0 50%,
    #f0f0f0 75%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: 4px;
}

@keyframes shimmer {
  0% {
    background-position: -200% 0;
  }
  100% {
    background-position: 200% 0;
  }
}
```

**Progress Indicator**:
```typescript
// src/components/common/ProgressBar.tsx
export const ProgressBar = ({ progress, label }: Props) => {
  return (
    <div className="progress-bar-container">
      {label && <span className="progress-label">{label}</span>}
      <div className="progress-bar">
        <div
          className="progress-fill"
          style={{ width: `${progress}%` }}
        />
      </div>
      <span className="progress-percentage">{progress}%</span>
    </div>
  );
};
```

**Button Loading State**:
```typescript
// src/components/common/Button.tsx
export const Button = ({ loading, children, disabled, ...props }: Props) => {
  return (
    <button
      {...props}
      disabled={disabled || loading}
      className={`button ${loading ? 'loading' : ''}`}
    >
      {loading ? (
        <>
          <Spinner size="small" />
          <span>Loading...</span>
        </>
      ) : (
        children
      )}
    </button>
  );
};
```

**Acceptance Criteria**:
- [x] Loading skeletons match final component layout
- [x] Shimmer effect smooth and professional
- [x] Progress bars show accurate progress
- [x] Button loading states disable interaction
- [x] All loading states tested
- [x] Performance acceptable (no jank)

**Definition of Done**:
- Loading states reviewed across all pages
- UI reviewed
- Performance tested

---

#### FE-211: Error Handling & Retry Mechanisms (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: API client

**Description**:
Enhance error handling with user-friendly messages and retry capabilities.

**Technical Requirements**:
- Error boundary for React component errors
- API error handling with retry logic
- User-friendly error messages
- Retry button for failed requests
- Network error detection
- Offline mode indicator

**Enhanced API Client**:
```typescript
// src/services/apiClient.ts
import axios, { AxiosError } from 'axios';
import { toast } from '@/utils/toast';

const apiClient = axios.create({
  baseURL: config.apiBaseUrl,
  timeout: 30000,
});

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor with retry
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config;

    // Retry logic for network errors
    if (!error.response && !originalRequest._retry) {
      originalRequest._retry = true;
      await new Promise((resolve) => setTimeout(resolve, 1000));  // Wait 1s
      return apiClient(originalRequest);
    }

    // Handle specific error cases
    if (error.response) {
      switch (error.response.status) {
        case 401:
          // Unauthorized - redirect to login
          localStorage.removeItem('accessToken');
          window.location.href = '/login';
          toast.error('Session expired. Please login again.');
          break;

        case 403:
          toast.error('You do not have permission to perform this action.');
          break;

        case 404:
          toast.error('Resource not found.');
          break;

        case 500:
          toast.error('Server error. Please try again later.');
          break;

        default:
          toast.error(
            error.response.data?.message || 'An unexpected error occurred.'
          );
      }
    } else if (error.request) {
      // Network error
      toast.error('Network error. Please check your connection.');
    }

    return Promise.reject(error);
  }
);

export default apiClient;
```

**Error Boundary**:
```typescript
// src/components/common/ErrorBoundary.tsx
export class ErrorBoundary extends React.Component<Props, State> {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    console.error('Error Boundary caught error:', error, errorInfo);
    // Log to error reporting service (e.g., Sentry)
  }

  handleReset = () => {
    this.setState({ hasError: false, error: null });
  };

  render() {
    if (this.state.hasError) {
      return (
        <div className="error-boundary-fallback">
          <div className="error-icon">
            <AlertTriangleIcon size={48} />
          </div>
          <h1>Something went wrong</h1>
          <p>We're sorry, but something unexpected happened.</p>
          <div className="error-actions">
            <Button onClick={this.handleReset}>
              Try Again
            </Button>
            <Button variant="secondary" onClick={() => window.location.href = '/'}>
              Go Home
            </Button>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
```

**Retry Component**:
```typescript
// src/components/common/ErrorRetry.tsx
export const ErrorRetry = ({ error, onRetry }: Props) => {
  return (
    <div className="error-retry">
      <div className="error-icon">
        <AlertCircleIcon size={32} />
      </div>
      <p className="error-message">{error}</p>
      <Button onClick={onRetry}>
        <RefreshIcon /> Try Again
      </Button>
    </div>
  );
};

// Usage in components
export const TransactionHistoryPage = () => {
  const [error, setError] = useState<string | null>(null);

  const fetchTransactions = async () => {
    try {
      // ... fetch logic
    } catch (err) {
      setError(err.message);
    }
  };

  if (error) {
    return (
      <ErrorRetry
        error={error}
        onRetry={() => {
          setError(null);
          fetchTransactions();
        }}
      />
    );
  }

  // ... rest of component
};
```

**Network Status Indicator**:
```typescript
// src/components/common/NetworkStatus.tsx
export const NetworkStatus = () => {
  const [isOnline, setIsOnline] = useState(navigator.onLine);

  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  if (isOnline) return null;

  return (
    <div className="network-status offline">
      <WifiOffIcon />
      <span>You are offline. Please check your connection.</span>
    </div>
  );
};
```

**Acceptance Criteria**:
- [x] Error boundary catches component errors
- [x] API client retries on network errors
- [x] User-friendly error messages displayed
- [x] Retry button allows users to retry failed requests
- [x] Network status indicator shows offline mode
- [x] All error scenarios tested

**Definition of Done**:
- Error handling tested (all scenarios)
- UI reviewed
- User experience validated

---

#### FE-212: Responsive Design Refinements (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: All components

**Description**:
Refine responsive design for optimal experience on mobile, tablet, and desktop.

**Technical Requirements**:
- Mobile-first approach (320px+)
- Tablet optimization (768px+)
- Desktop optimization (1024px+)
- Touch-friendly UI elements (44x44px minimum)
- Responsive navigation
- Image/asset optimization
- Performance on mobile devices

**Responsive Breakpoints**:
```css
/* Mobile First */
.container {
  padding: 16px;
}

/* Tablet (768px+) */
@media (min-width: 768px) {
  .container {
    padding: 24px;
  }
}

/* Desktop (1024px+) */
@media (min-width: 1024px) {
  .container {
    padding: 32px;
    max-width: 1200px;
    margin: 0 auto;
  }
}
```

**Mobile Navigation**:
```typescript
// src/components/layout/MobileNav.tsx
export const MobileNav = () => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <button
        className="mobile-menu-button"
        onClick={() => setIsOpen(!isOpen)}
      >
        {isOpen ? <XIcon /> : <MenuIcon />}
      </button>

      {isOpen && (
        <div className="mobile-menu">
          <nav className="mobile-nav">
            <NavLink to="/dashboard" onClick={() => setIsOpen(false)}>
              <DashboardIcon /> Dashboard
            </NavLink>
            <NavLink to="/wallet" onClick={() => setIsOpen(false)}>
              <WalletIcon /> Wallet
            </NavLink>
            <NavLink to="/transactions" onClick={() => setIsOpen(false)}>
              <TransactionIcon /> Transactions
            </NavLink>
          </nav>
        </div>
      )}
    </>
  );
};
```

**Touch-Friendly Buttons**:
```css
/* Ensure minimum touch target size */
.button,
.icon-button {
  min-width: 44px;
  min-height: 44px;
  padding: 12px 20px;
}

/* Increase tap target for mobile */
@media (max-width: 767px) {
  .button {
    min-height: 48px;
    font-size: 16px;  /* Prevent iOS zoom on focus */
  }
}
```

**Responsive Layout Grid**:
```css
/* Transaction Cards */
.transaction-list {
  display: grid;
  gap: 16px;
}

/* Mobile: 1 column */
@media (max-width: 767px) {
  .transaction-list {
    grid-template-columns: 1fr;
  }
}

/* Tablet: 2 columns */
@media (min-width: 768px) {
  .transaction-list {
    grid-template-columns: repeat(2, 1fr);
  }
}

/* Desktop: 3 columns */
@media (min-width: 1024px) {
  .transaction-list {
    grid-template-columns: repeat(3, 1fr);
  }
}
```

**Testing Checklist**:
- [ ] Test on iPhone (Safari)
- [ ] Test on Android (Chrome)
- [ ] Test on iPad (Safari)
- [ ] Test on Chrome DevTools device emulation
- [ ] Test touch interactions (tap, swipe, scroll)
- [ ] Test landscape and portrait orientations
- [ ] Test with large text (accessibility)

**Acceptance Criteria**:
- [x] App usable on 320px+ screens
- [x] Touch targets meet 44x44px minimum
- [x] Navigation works on mobile devices
- [x] All pages responsive (mobile/tablet/desktop)
- [x] Text readable without zoom
- [x] Images scale appropriately
- [x] Performance acceptable on mobile devices

**Definition of Done**:
- Responsive design tested on 5+ devices
- UI reviewed on physical devices
- Accessibility validated

---

#### FE-213: Performance Optimization (2.00 days)
**Owner**: Senior Frontend Engineer
**Priority**: P1 - High
**Dependencies**: All pages and components

**Description**:
Optimize frontend performance for fast load times and smooth interactions.

**Technical Requirements**:
- Code splitting and lazy loading
- Bundle size optimization
- Image optimization
- Memoization for expensive operations
- Debouncing for search inputs
- Virtual scrolling for large lists
- Performance monitoring

**Code Splitting**:
```typescript
// src/routes/router.tsx
import { lazy, Suspense } from 'react';

const Dashboard = lazy(() => import('@/pages/DashboardPage'));
const Transactions = lazy(() => import('@/pages/TransactionHistoryPage'));
const Transfer = lazy(() => import('@/pages/TransferPage'));

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: 'dashboard',
        element: (
          <Suspense fallback={<PageLoadingSkeleton />}>
            <Dashboard />
          </Suspense>
        ),
      },
      {
        path: 'transactions',
        element: (
          <Suspense fallback={<PageLoadingSkeleton />}>
            <Transactions />
          </Suspense>
        ),
      },
      // ... other routes
    ],
  },
]);
```

**Memoization**:
```typescript
// src/components/transactions/TransactionList.tsx
import { useMemo } from 'react';

export const TransactionList = ({ transactions, filters }: Props) => {
  // Memoize filtered transactions
  const filteredTransactions = useMemo(() => {
    return transactions.filter((tx) => {
      if (filters.status && tx.status !== filters.status) return false;
      if (filters.searchQuery &&
          !tx.toAddress.includes(filters.searchQuery) &&
          !tx.transactionHash?.includes(filters.searchQuery)) return false;
      return true;
    });
  }, [transactions, filters]);

  // Memoize sorted transactions
  const sortedTransactions = useMemo(() => {
    return [...filteredTransactions].sort((a, b) => {
      if (filters.sortBy === 'date') {
        return filters.sortOrder === 'asc'
          ? a.createdAt.getTime() - b.createdAt.getTime()
          : b.createdAt.getTime() - a.createdAt.getTime();
      }
      // ... other sorting
    });
  }, [filteredTransactions, filters]);

  return (
    <div className="transaction-list">
      {sortedTransactions.map((tx) => (
        <TransactionCard key={tx.id} transaction={tx} />
      ))}
    </div>
  );
};
```

**Debounced Search**:
```typescript
// src/hooks/useDebounce.ts
export const useDebounce = <T>(value: T, delay: number): T => {
  const [debouncedValue, setDebouncedValue] = useState(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => clearTimeout(handler);
  }, [value, delay]);

  return debouncedValue;
};

// Usage in search component
export const TransactionFilters = ({ onFilterChange }: Props) => {
  const [searchQuery, setSearchQuery] = useState('');
  const debouncedSearchQuery = useDebounce(searchQuery, 500);  // 500ms delay

  useEffect(() => {
    onFilterChange({ searchQuery: debouncedSearchQuery });
  }, [debouncedSearchQuery]);

  return (
    <SearchInput
      value={searchQuery}
      onChange={setSearchQuery}
      placeholder="Search transactions..."
    />
  );
};
```

**Virtual Scrolling (Optional)**:
```typescript
// src/components/transactions/VirtualTransactionList.tsx
import { useVirtualizer } from '@tanstack/react-virtual';

export const VirtualTransactionList = ({ transactions }: Props) => {
  const parentRef = useRef<HTMLDivElement>(null);

  const rowVirtualizer = useVirtualizer({
    count: transactions.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => 80,  // Estimated row height
  });

  return (
    <div ref={parentRef} className="virtual-list" style={{ height: '600px', overflow: 'auto' }}>
      <div style={{ height: `${rowVirtualizer.getTotalSize()}px`, position: 'relative' }}>
        {rowVirtualizer.getVirtualItems().map((virtualItem) => {
          const transaction = transactions[virtualItem.index];
          return (
            <div
              key={virtualItem.key}
              style={{
                position: 'absolute',
                top: 0,
                left: 0,
                width: '100%',
                height: `${virtualItem.size}px`,
                transform: `translateY(${virtualItem.start}px)`,
              }}
            >
              <TransactionCard transaction={transaction} />
            </div>
          );
        })}
      </div>
    </div>
  );
};
```

**Bundle Size Analysis**:
```bash
# Analyze bundle size
npm run build -- --analyze

# Check bundle sizes
npx vite-bundle-visualizer
```

**Performance Targets**:
- First Contentful Paint (FCP) < 1.5s
- Largest Contentful Paint (LCP) < 2.5s
- Time to Interactive (TTI) < 3.5s
- Cumulative Layout Shift (CLS) < 0.1
- First Input Delay (FID) < 100ms

**Acceptance Criteria**:
- [x] Code splitting reduces initial bundle size
- [x] Lazy loading defers non-critical components
- [x] Search input debounced (reduces API calls)
- [x] Large lists virtualized (if >100 items)
- [x] Performance metrics meet targets
- [x] Lighthouse score > 90

**Definition of Done**:
- Performance tested with Lighthouse
- Bundle size analyzed and optimized
- Code reviewed

---

## Daily Milestone Plan

### Days 1-2 (Sprint Start)
**Focus**: Phase 1 completion

**Tasks**:
- FE-202: Dashboard enhancement
- FE-203: Transfer form completion
- FE-204: Transaction status display

**Deliverable**: Phase 1 UI fully functional

---

### Days 3-4
**Focus**: Transaction history

**Tasks**:
- FE-205: Transaction history page
- FE-206: Filters & search component
- FE-207: Transaction detail modal (started)

**Deliverable**: Transaction history with filtering

---

### Days 5-6 (Mid-Sprint)
**Focus**: UI enhancements

**Tasks**:
- FE-207: Detail modal (completed)
- FE-208: QR code generation
- FE-209: Copy-to-clipboard enhancements
- FE-210: Loading skeletons (started)

**Checkpoint Meeting**: Demo transaction history and QR codes

**Deliverable**: Enhanced UX features

---

### Days 7-8
**Focus**: Polish and responsive design

**Tasks**:
- FE-210: Loading skeletons (completed)
- FE-211: Error handling improvements
- FE-212: Responsive design refinements

**Deliverable**: Professional loading and error UX

---

### Days 9-10 (Sprint End)
**Focus**: Performance and testing

**Tasks**:
- FE-213: Performance optimization
- Cross-browser testing
- Component tests
- Documentation updates

**Sprint Review**: Demo complete Phase 2 frontend features

**Deliverable**: Production-ready UI

---

## Sprint N02 Frontend Success Criteria

### Functional Success Metrics

- [ ] Wallet dashboard displays balance and recent transactions
- [ ] Transfer form validates and submits correctly
- [ ] Transaction history displays 100+ transactions smoothly
- [ ] Filters and search work correctly
- [ ] Transaction detail modal shows complete information
- [ ] QR code generation works
- [ ] Copy-to-clipboard works across all components
- [ ] Loading states implemented everywhere
- [ ] Error handling with retry mechanisms
- [ ] Responsive design works on mobile, tablet, desktop

### Quality Gates

- [ ] Component tests cover critical flows
- [ ] No console errors in production build
- [ ] Accessibility score > 90 (Lighthouse)
- [ ] Performance score > 90 (Lighthouse)
- [ ] Code reviewed and approved
- [ ] No Critical or High severity bugs

---

**Sprint N02 Frontend Plan Version**: 1.0
**Last Updated**: 2025-10-28
**Status**: Ready for Execution
**Next Steps**: Day 1 - Start FE-202, FE-203, FE-204

---

**End of Sprint N02 Frontend Plan**
