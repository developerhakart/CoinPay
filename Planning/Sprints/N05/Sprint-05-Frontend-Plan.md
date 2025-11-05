# Sprint N05 - Frontend Engineering Plan
# Phase 5: Basic Swap UI

**Sprint**: N05
**Duration**: 2 weeks (10 working days)
**Sprint Dates**: March 3 - March 14, 2025
**Total Effort**: 18.00 days
**Team Size**: 2-3 engineers (frontend-engineer agents)
**Utilization**: 60% (healthy capacity with buffer)

---

## Sprint Goal

Implement intuitive token swap interface with real-time pricing, slippage controls, and fee transparency for USDC ↔ ETH/MATIC swaps.

---

## Component Breakdown

### Epic 1: Token Selection & Swap Interface (6.50 days)

#### FE-501: Token Selection Modal Component (2.00 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P0 (Critical Path)

**Description**:
Create modal component for selecting tokens (USDC, ETH, MATIC) with search and balance display.

**Requirements**:
- Modal overlay with token list
- Search/filter functionality
- Display token symbol, name, and balance
- Highlight selected token
- Close on selection
- Mobile responsive

**Component Structure**:
```typescript
// components/swap/TokenSelectionModal.tsx
interface TokenSelectionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectToken: (token: Token) => void;
  excludeToken?: string; // Exclude "from" token when selecting "to"
  userBalances: Map<string, number>;
}

interface Token {
  address: string;
  symbol: string;
  name: string;
  decimals: number;
  logoUrl: string;
}

const TokenSelectionModal: React.FC<TokenSelectionModalProps> = ({
  isOpen,
  onClose,
  onSelectToken,
  excludeToken,
  userBalances
}) => {
  const [searchQuery, setSearchQuery] = useState('');

  const supportedTokens: Token[] = [
    {
      address: '0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582',
      symbol: 'USDC',
      name: 'USD Coin',
      decimals: 6,
      logoUrl: '/tokens/usdc.svg'
    },
    {
      address: '0x...',
      symbol: 'WETH',
      name: 'Wrapped Ether',
      decimals: 18,
      logoUrl: '/tokens/eth.svg'
    },
    {
      address: '0x...',
      symbol: 'WMATIC',
      name: 'Wrapped Matic',
      decimals: 18,
      logoUrl: '/tokens/matic.svg'
    }
  ];

  const filteredTokens = supportedTokens
    .filter(token => token.address !== excludeToken)
    .filter(token =>
      token.symbol.toLowerCase().includes(searchQuery.toLowerCase()) ||
      token.name.toLowerCase().includes(searchQuery.toLowerCase())
    );

  return (
    <Dialog open={isOpen} onClose={onClose}>
      <div className="token-selection-modal">
        <div className="modal-header">
          <h3>Select a token</h3>
          <button onClick={onClose}>✕</button>
        </div>

        <div className="search-box">
          <input
            type="text"
            placeholder="Search by name or symbol"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>

        <div className="token-list">
          {filteredTokens.map(token => {
            const balance = userBalances.get(token.address) || 0;

            return (
              <div
                key={token.address}
                className="token-item"
                onClick={() => onSelectToken(token)}
              >
                <img src={token.logoUrl} alt={token.symbol} />
                <div className="token-info">
                  <div className="token-symbol">{token.symbol}</div>
                  <div className="token-name">{token.name}</div>
                </div>
                <div className="token-balance">
                  {balance.toFixed(6)}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </Dialog>
  );
};
```

**Styling**:
```css
.token-selection-modal {
  width: 420px;
  max-height: 600px;
  border-radius: 16px;
  background: var(--background-primary);
  padding: 16px;
}

.token-item {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  border-radius: 12px;
  cursor: pointer;
  transition: background 0.2s;
}

.token-item:hover {
  background: var(--background-hover);
}

.token-item img {
  width: 32px;
  height: 32px;
  margin-right: 12px;
}

@media (max-width: 768px) {
  .token-selection-modal {
    width: 100%;
    max-width: 90vw;
  }
}
```

**Acceptance Criteria**:
- [ ] Modal opens and closes smoothly
- [ ] Search filters tokens by symbol/name
- [ ] User balances displayed correctly
- [ ] Selected token excluded from opposite selector
- [ ] Mobile responsive
- [ ] Keyboard navigation (Esc to close)
- [ ] Component tests pass

**Dependencies**: None

---

#### FE-502: Swap Interface Layout (From/To) (2.50 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P0 (Critical Path)

**Description**:
Main swap interface with "from" and "to" token inputs, flip button, and balance display.

**Requirements**:
- Two token input sections (from/to)
- Token selector buttons
- Amount input fields
- Balance display for each token
- "Max" button to use full balance
- Flip button to reverse token pair
- Connected state validation

**Component Structure**:
```typescript
// components/swap/SwapInterface.tsx
interface SwapInterfaceProps {
  onSwapExecute: (fromToken: string, toToken: string, amount: number) => void;
}

const SwapInterface: React.FC<SwapInterfaceProps> = ({ onSwapExecute }) => {
  const [fromToken, setFromToken] = useState<Token | null>(null);
  const [toToken, setToToken] = useState<Token | null>(null);
  const [fromAmount, setFromAmount] = useState<string>('');
  const [isFromModalOpen, setIsFromModalOpen] = useState(false);
  const [isToModalOpen, setIsToModalOpen] = useState(false);

  const { data: balances } = useTokenBalances();

  const fromBalance = fromToken
    ? balances?.get(fromToken.address) || 0
    : 0;

  const handleFlipTokens = () => {
    setFromToken(toToken);
    setToToken(fromToken);
    setFromAmount('');
  };

  const handleMaxAmount = () => {
    if (fromToken && fromBalance > 0) {
      setFromAmount(fromBalance.toString());
    }
  };

  return (
    <div className="swap-interface">
      <div className="swap-header">
        <h2>Swap</h2>
        <button className="settings-button">⚙️</button>
      </div>

      {/* From Token Section */}
      <div className="token-input-section">
        <div className="section-header">
          <span>From</span>
          <span className="balance">
            Balance: {fromBalance.toFixed(6)}
          </span>
        </div>

        <div className="input-row">
          <input
            type="number"
            className="amount-input"
            placeholder="0.0"
            value={fromAmount}
            onChange={(e) => setFromAmount(e.target.value)}
          />

          <button
            className="token-selector"
            onClick={() => setIsFromModalOpen(true)}
          >
            {fromToken ? (
              <>
                <img src={fromToken.logoUrl} alt={fromToken.symbol} />
                <span>{fromToken.symbol}</span>
              </>
            ) : (
              <span>Select token</span>
            )}
            <span className="dropdown-icon">▼</span>
          </button>
        </div>

        <div className="section-footer">
          <button
            className="max-button"
            onClick={handleMaxAmount}
            disabled={!fromToken || fromBalance === 0}
          >
            MAX
          </button>
        </div>
      </div>

      {/* Flip Button */}
      <div className="flip-button-container">
        <button
          className="flip-button"
          onClick={handleFlipTokens}
          disabled={!fromToken || !toToken}
        >
          ⇅
        </button>
      </div>

      {/* To Token Section */}
      <div className="token-input-section">
        <div className="section-header">
          <span>To</span>
          <span className="balance">
            Balance: {toToken ? (balances?.get(toToken.address) || 0).toFixed(6) : '0.00'}
          </span>
        </div>

        <div className="input-row">
          <input
            type="number"
            className="amount-input"
            placeholder="0.0"
            value={toAmount}
            readOnly
          />

          <button
            className="token-selector"
            onClick={() => setIsToModalOpen(true)}
          >
            {toToken ? (
              <>
                <img src={toToken.logoUrl} alt={toToken.symbol} />
                <span>{toToken.symbol}</span>
              </>
            ) : (
              <span>Select token</span>
            )}
            <span className="dropdown-icon">▼</span>
          </button>
        </div>
      </div>

      {/* Token Selection Modals */}
      <TokenSelectionModal
        isOpen={isFromModalOpen}
        onClose={() => setIsFromModalOpen(false)}
        onSelectToken={(token) => {
          setFromToken(token);
          setIsFromModalOpen(false);
        }}
        excludeToken={toToken?.address}
        userBalances={balances || new Map()}
      />

      <TokenSelectionModal
        isOpen={isToModalOpen}
        onClose={() => setIsToModalOpen(false)}
        onSelectToken={(token) => {
          setToToken(token);
          setIsToModalOpen(false);
        }}
        excludeToken={fromToken?.address}
        userBalances={balances || new Map()}
      />
    </div>
  );
};
```

**Acceptance Criteria**:
- [ ] From/To sections display correctly
- [ ] Token selector opens modal
- [ ] Amount input accepts numeric values
- [ ] Balance displays update in real-time
- [ ] MAX button fills full balance
- [ ] Flip button swaps token positions
- [ ] Mobile responsive layout
- [ ] Component tests pass

**Dependencies**: FE-501

---

#### FE-503: Token Balance Display Component (1.00 day)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P0 (Critical Path)

**Description**:
Component to display token balances with loading and error states.

**Requirements**:
- Query Circle wallet balances
- Display loading skeleton
- Handle error states
- Auto-refresh every 30 seconds
- Format balances correctly

**Implementation**:
```typescript
// hooks/useTokenBalances.ts
export const useTokenBalances = () => {
  const { address } = useWallet();

  return useQuery({
    queryKey: ['tokenBalances', address],
    queryFn: async () => {
      if (!address) return new Map();

      const response = await fetch(`/api/wallet/${address}/balances`);
      const data = await response.json();

      const balances = new Map<string, number>();
      data.balances.forEach((item: any) => {
        balances.set(item.tokenAddress, item.balance);
      });

      return balances;
    },
    enabled: !!address,
    refetchInterval: 30000, // Refresh every 30 seconds
    staleTime: 10000
  });
};

// components/swap/TokenBalanceDisplay.tsx
const TokenBalanceDisplay: React.FC<{ tokenAddress: string }> = ({
  tokenAddress
}) => {
  const { data: balances, isLoading, error } = useTokenBalances();

  if (isLoading) {
    return <Skeleton width={80} height={16} />;
  }

  if (error) {
    return <span className="balance-error">Error loading balance</span>;
  }

  const balance = balances?.get(tokenAddress) || 0;

  return (
    <span className="token-balance">
      Balance: {balance.toFixed(6)}
    </span>
  );
};
```

**Acceptance Criteria**:
- [ ] Balances load from API
- [ ] Loading skeleton displays
- [ ] Error states handled gracefully
- [ ] Auto-refresh works
- [ ] Balance formatting correct
- [ ] Unit tests pass

**Dependencies**: BE-507 (Balance API)

---

### Epic 2: Exchange Rate & Calculator (4.50 days)

#### FE-504: Exchange Rate Display Component (1.50 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-2)
**Priority**: P0 (Critical Path)

**Description**:
Display real-time exchange rate with auto-refresh and loading states.

**Requirements**:
- Fetch exchange rate from quote API
- Display rate with proper formatting
- Auto-refresh every 30 seconds
- Show loading spinner during refresh
- Display last update timestamp

**Implementation**:
```typescript
// hooks/useExchangeRate.ts
export const useExchangeRate = (
  fromToken: string | null,
  toToken: string | null,
  amount: number
) => {
  return useQuery({
    queryKey: ['exchangeRate', fromToken, toToken, amount],
    queryFn: async () => {
      if (!fromToken || !toToken || amount <= 0) {
        return null;
      }

      const response = await fetch(
        `/api/swap/quote?fromToken=${fromToken}&toToken=${toToken}&amount=${amount}&slippage=1`
      );

      if (!response.ok) {
        throw new Error('Failed to fetch exchange rate');
      }

      return await response.json();
    },
    enabled: !!fromToken && !!toToken && amount > 0,
    refetchInterval: 30000, // Refresh every 30 seconds
    staleTime: 5000
  });
};

// components/swap/ExchangeRateDisplay.tsx
const ExchangeRateDisplay: React.FC<{
  fromToken: Token | null;
  toToken: Token | null;
  amount: number;
}> = ({ fromToken, toToken, amount }) => {
  const { data: quote, isLoading, error } = useExchangeRate(
    fromToken?.address || null,
    toToken?.address || null,
    amount
  );

  if (!fromToken || !toToken || amount <= 0) {
    return null;
  }

  if (isLoading) {
    return (
      <div className="exchange-rate loading">
        <Spinner size="small" />
        <span>Fetching best price...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="exchange-rate error">
        <span>⚠️ Failed to fetch price</span>
      </div>
    );
  }

  if (!quote) {
    return null;
  }

  return (
    <div className="exchange-rate">
      <div className="rate-display">
        <span>1 {fromToken.symbol} = </span>
        <span className="rate-value">
          {quote.exchangeRate.toFixed(6)} {toToken.symbol}
        </span>
      </div>
      <div className="rate-info">
        <span className="provider">via {quote.provider}</span>
        <span className="last-update">
          Updated {formatDistanceToNow(new Date(quote.quoteValidUntil))} ago
        </span>
      </div>
    </div>
  );
};
```

**Acceptance Criteria**:
- [ ] Exchange rate displays correctly
- [ ] Auto-refresh every 30 seconds
- [ ] Loading state shown during fetch
- [ ] Error state handled gracefully
- [ ] Last update timestamp shown
- [ ] Component tests pass

**Dependencies**: BE-504 (Quote API)

---

#### FE-505: Swap Amount Calculator (2.00 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P0 (Critical Path)

**Description**:
Calculate and display "to" amount based on "from" amount and current exchange rate.

**Requirements**:
- Calculate toAmount = fromAmount * exchangeRate
- Update on fromAmount change
- Debounce API calls (500ms)
- Display loading state
- Handle calculation errors

**Implementation**:
```typescript
// components/swap/SwapAmountCalculator.tsx
const SwapAmountCalculator: React.FC<{
  fromToken: Token | null;
  toToken: Token | null;
  fromAmount: string;
  onToAmountChange: (amount: string) => void;
}> = ({ fromToken, toToken, fromAmount, onToAmountChange }) => {
  const [debouncedFromAmount] = useDebounce(fromAmount, 500);

  const { data: quote, isLoading } = useExchangeRate(
    fromToken?.address || null,
    toToken?.address || null,
    parseFloat(debouncedFromAmount) || 0
  );

  useEffect(() => {
    if (quote && quote.toAmount) {
      onToAmountChange(quote.toAmount.toString());
    } else {
      onToAmountChange('');
    }
  }, [quote, onToAmountChange]);

  if (isLoading) {
    return <Spinner className="calculator-loader" />;
  }

  return null; // This component manages state, no UI
};

// Custom hook for swap calculation
export const useSwapCalculation = (
  fromToken: Token | null,
  toToken: Token | null,
  fromAmount: string
) => {
  const [toAmount, setToAmount] = useState<string>('');
  const [debouncedFromAmount] = useDebounce(fromAmount, 500);

  const { data: quote, isLoading, error } = useExchangeRate(
    fromToken?.address || null,
    toToken?.address || null,
    parseFloat(debouncedFromAmount) || 0
  );

  useEffect(() => {
    if (quote) {
      setToAmount(quote.toAmount.toFixed(6));
    } else {
      setToAmount('');
    }
  }, [quote]);

  return {
    toAmount,
    quote,
    isCalculating: isLoading,
    error
  };
};
```

**Acceptance Criteria**:
- [ ] To amount calculated correctly
- [ ] API calls debounced (500ms)
- [ ] Loading state during calculation
- [ ] Error handling for invalid inputs
- [ ] Updates on fromAmount change
- [ ] Unit tests pass

**Dependencies**: BE-504 (Quote API), FE-504

---

#### FE-506: Slippage Tolerance Settings (1.50 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-2)
**Priority**: P0 (Critical Path)

**Description**:
Settings panel for selecting slippage tolerance with preset and custom options.

**Requirements**:
- Preset options: 0.5%, 1%, 3%
- Custom slippage input
- Validate custom slippage (0.1% - 50%)
- Persist selection in local storage
- Warning for high slippage (>5%)

**Implementation**:
```typescript
// components/swap/SlippageSettings.tsx
const SlippageSettings: React.FC<{
  value: number;
  onChange: (slippage: number) => void;
}> = ({ value, onChange }) => {
  const [isCustom, setIsCustom] = useState(false);
  const [customValue, setCustomValue] = useState<string>('');

  const presetOptions = [0.5, 1.0, 3.0];

  const handlePresetClick = (preset: number) => {
    setIsCustom(false);
    onChange(preset);
    localStorage.setItem('slippageTolerance', preset.toString());
  };

  const handleCustomChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setCustomValue(val);

    const numVal = parseFloat(val);
    if (!isNaN(numVal) && numVal >= 0.1 && numVal <= 50) {
      onChange(numVal);
      localStorage.setItem('slippageTolerance', numVal.toString());
    }
  };

  const isHighSlippage = value > 5.0;

  return (
    <div className="slippage-settings">
      <div className="settings-header">
        <span>Slippage Tolerance</span>
        <Tooltip content="Your transaction will revert if the price changes unfavorably by more than this percentage">
          ℹ️
        </Tooltip>
      </div>

      <div className="preset-options">
        {presetOptions.map(preset => (
          <button
            key={preset}
            className={`preset-button ${!isCustom && value === preset ? 'active' : ''}`}
            onClick={() => handlePresetClick(preset)}
          >
            {preset}%
          </button>
        ))}

        <button
          className={`preset-button ${isCustom ? 'active' : ''}`}
          onClick={() => setIsCustom(true)}
        >
          Custom
        </button>
      </div>

      {isCustom && (
        <div className="custom-input">
          <input
            type="number"
            placeholder="0.5"
            value={customValue}
            onChange={handleCustomChange}
            min="0.1"
            max="50"
            step="0.1"
          />
          <span className="input-suffix">%</span>
        </div>
      )}

      {isHighSlippage && (
        <div className="warning-message">
          ⚠️ High slippage tolerance may result in unfavorable trades
        </div>
      )}
    </div>
  );
};
```

**Acceptance Criteria**:
- [ ] Preset options work (0.5%, 1%, 3%)
- [ ] Custom input validates range (0.1-50%)
- [ ] Warning shown for slippage >5%
- [ ] Settings persist in localStorage
- [ ] Mobile responsive
- [ ] Component tests pass

**Dependencies**: None

---

### Epic 3: Price Impact & Fees (3.50 days)

#### FE-507: Price Impact Indicator (1.50 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-2)
**Priority**: P1 (High)

**Description**:
Display price impact percentage with color-coded warning levels.

**Requirements**:
- Calculate price impact from quote
- Color code: Green (<1%), Yellow (1-3%), Red (>3%)
- Warning message for high impact
- Suggest trade splitting for large swaps

**Implementation**:
```typescript
// components/swap/PriceImpactIndicator.tsx
const PriceImpactIndicator: React.FC<{
  priceImpact: number | null;
}> = ({ priceImpact }) => {
  if (priceImpact === null || priceImpact === 0) {
    return null;
  }

  const getImpactLevel = (impact: number): 'low' | 'medium' | 'high' => {
    if (impact < 1) return 'low';
    if (impact < 3) return 'medium';
    return 'high';
  };

  const impactLevel = getImpactLevel(priceImpact);

  const getImpactColor = () => {
    switch (impactLevel) {
      case 'low':
        return 'green';
      case 'medium':
        return 'yellow';
      case 'high':
        return 'red';
    }
  };

  const getSuggestion = () => {
    if (impactLevel === 'high') {
      return 'Consider splitting this trade into smaller amounts to reduce price impact';
    }
    return null;
  };

  return (
    <div className={`price-impact-indicator ${impactLevel}`}>
      <div className="impact-header">
        <span>Price Impact</span>
        <span className={`impact-value ${getImpactColor()}`}>
          {priceImpact.toFixed(2)}%
        </span>
      </div>

      {getSuggestion() && (
        <div className="impact-suggestion">
          ℹ️ {getSuggestion()}
        </div>
      )}
    </div>
  );
};
```

**Acceptance Criteria**:
- [ ] Price impact displays correctly
- [ ] Color coding based on impact level
- [ ] Warning for high impact (>3%)
- [ ] Suggestion for trade splitting
- [ ] Component tests pass

**Dependencies**: BE-504 (Quote API with price impact)

---

#### FE-508: Fee Breakdown Display (1.00 day)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P0 (Critical Path)

**Description**:
Display detailed fee breakdown (platform fee, DEX fee, gas estimate).

**Requirements**:
- Platform fee (0.5%)
- Gas estimate in token and USD
- Total fee summary
- Expandable detail view

**Implementation**:
```typescript
// components/swap/FeeBreakdown.tsx
const FeeBreakdown: React.FC<{
  quote: SwapQuote | null;
}> = ({ quote }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  if (!quote) {
    return null;
  }

  return (
    <div className="fee-breakdown">
      <div
        className="fee-summary"
        onClick={() => setIsExpanded(!isExpanded)}
      >
        <span>Fees</span>
        <div className="fee-total">
          <span>{quote.platformFee.toFixed(4)} {quote.fromTokenSymbol}</span>
          <span className="expand-icon">{isExpanded ? '▲' : '▼'}</span>
        </div>
      </div>

      {isExpanded && (
        <div className="fee-details">
          <div className="fee-item">
            <span>Platform Fee ({quote.platformFeePercentage}%)</span>
            <span>{quote.platformFee.toFixed(4)} {quote.fromTokenSymbol}</span>
          </div>

          <div className="fee-item">
            <span>Network Fee (estimated)</span>
            <span>{quote.estimatedGasCost} MATIC</span>
          </div>

          <div className="fee-divider" />

          <div className="fee-item total">
            <span>Total Fees</span>
            <span>{(quote.platformFee + parseFloat(quote.estimatedGasCost)).toFixed(4)}</span>
          </div>
        </div>
      )}
    </div>
  );
};
```

**Acceptance Criteria**:
- [ ] Fee breakdown displays correctly
- [ ] Expandable detail view works
- [ ] Platform fee calculated (0.5%)
- [ ] Gas estimate shown
- [ ] Total fee accurate
- [ ] Component tests pass

**Dependencies**: BE-511 (Fee API)

---

#### FE-509: Swap Confirmation Modal (2.00 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P0 (Critical Path)

**Description**:
Confirmation modal showing swap details before execution.

**Requirements**:
- Display swap summary
- Show exchange rate
- Display fee breakdown
- Minimum received amount
- Confirm/Cancel buttons
- Loading state during execution

**Implementation**:
```typescript
// components/swap/SwapConfirmationModal.tsx
const SwapConfirmationModal: React.FC<{
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  fromToken: Token;
  toToken: Token;
  fromAmount: number;
  toAmount: number;
  quote: SwapQuote;
  isExecuting: boolean;
}> = ({
  isOpen,
  onClose,
  onConfirm,
  fromToken,
  toToken,
  fromAmount,
  toAmount,
  quote,
  isExecuting
}) => {
  return (
    <Dialog open={isOpen} onClose={onClose}>
      <div className="swap-confirmation-modal">
        <div className="modal-header">
          <h3>Confirm Swap</h3>
          <button onClick={onClose} disabled={isExecuting}>✕</button>
        </div>

        <div className="swap-summary">
          <div className="amount-section">
            <span className="label">You pay</span>
            <div className="amount-display">
              <span className="value">{fromAmount.toFixed(6)}</span>
              <span className="token">{fromToken.symbol}</span>
            </div>
          </div>

          <div className="swap-arrow">↓</div>

          <div className="amount-section">
            <span className="label">You receive (estimated)</span>
            <div className="amount-display">
              <span className="value">{toAmount.toFixed(6)}</span>
              <span className="token">{toToken.symbol}</span>
            </div>
          </div>
        </div>

        <div className="swap-details">
          <div className="detail-row">
            <span>Exchange Rate</span>
            <span>1 {fromToken.symbol} = {quote.exchangeRate.toFixed(6)} {toToken.symbol}</span>
          </div>

          <div className="detail-row">
            <span>Platform Fee</span>
            <span>{quote.platformFee.toFixed(4)} {fromToken.symbol} ({quote.platformFeePercentage}%)</span>
          </div>

          <div className="detail-row">
            <span>Slippage Tolerance</span>
            <span>{quote.slippageTolerance}%</span>
          </div>

          <div className="detail-row highlighted">
            <span>Minimum Received</span>
            <span>{quote.minimumReceived.toFixed(6)} {toToken.symbol}</span>
          </div>
        </div>

        <div className="confirmation-notice">
          ℹ️ Output is estimated. You will receive at least {quote.minimumReceived.toFixed(6)} {toToken.symbol} or the transaction will revert.
        </div>

        <div className="modal-actions">
          <button
            className="button-secondary"
            onClick={onClose}
            disabled={isExecuting}
          >
            Cancel
          </button>
          <button
            className="button-primary"
            onClick={onConfirm}
            disabled={isExecuting}
          >
            {isExecuting ? (
              <>
                <Spinner size="small" />
                <span>Swapping...</span>
              </>
            ) : (
              'Confirm Swap'
            )}
          </button>
        </div>
      </div>
    </Dialog>
  );
};
```

**Acceptance Criteria**:
- [ ] Modal displays swap summary
- [ ] Exchange rate and fees shown
- [ ] Minimum received highlighted
- [ ] Confirm button executes swap
- [ ] Loading state during execution
- [ ] Mobile responsive
- [ ] Component tests pass

**Dependencies**: FE-502, BE-510

---

### Epic 4: History & Tracking (3.50 days)

#### FE-510: Swap Status Tracking Component (1.50 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-2)
**Priority**: P1 (High)

**Description**:
Component to track and display swap transaction status.

**Requirements**:
- Display transaction status (pending, confirmed, failed)
- Show transaction hash with explorer link
- Auto-refresh status every 5 seconds
- Success/failure notifications

**Implementation**:
```typescript
// components/swap/SwapStatusTracker.tsx
const SwapStatusTracker: React.FC<{
  swapId: string;
  onComplete: () => void;
}> = ({ swapId, onComplete }) => {
  const { data: swap, isLoading } = useQuery({
    queryKey: ['swapStatus', swapId],
    queryFn: async () => {
      const response = await fetch(`/api/swap/${swapId}/details`);
      return await response.json();
    },
    refetchInterval: (data) => {
      // Stop polling if confirmed or failed
      return data?.status === 'pending' ? 5000 : false;
    }
  });

  useEffect(() => {
    if (swap && swap.status !== 'pending') {
      onComplete();

      if (swap.status === 'confirmed') {
        toast.success(`Swap completed! Received ${swap.toAmount} ${swap.toTokenSymbol}`);
      } else if (swap.status === 'failed') {
        toast.error(`Swap failed: ${swap.errorMessage}`);
      }
    }
  }, [swap, onComplete]);

  if (isLoading) {
    return <Skeleton />;
  }

  if (!swap) {
    return null;
  }

  return (
    <div className={`swap-status-tracker ${swap.status}`}>
      <div className="status-icon">
        {swap.status === 'pending' && <Spinner />}
        {swap.status === 'confirmed' && '✓'}
        {swap.status === 'failed' && '✕'}
      </div>

      <div className="status-content">
        <div className="status-title">
          {swap.status === 'pending' && 'Swap in progress...'}
          {swap.status === 'confirmed' && 'Swap completed!'}
          {swap.status === 'failed' && 'Swap failed'}
        </div>

        {swap.transactionHash && (
          <a
            href={`https://amoy.polygonscan.com/tx/${swap.transactionHash}`}
            target="_blank"
            rel="noopener noreferrer"
            className="tx-link"
          >
            View on Explorer ↗
          </a>
        )}

        {swap.status === 'pending' && (
          <div className="status-note">
            Estimated time: ~45 seconds
          </div>
        )}
      </div>
    </div>
  );
};
```

**Acceptance Criteria**:
- [ ] Status displays correctly
- [ ] Auto-refresh every 5 seconds
- [ ] Explorer link works
- [ ] Success/failure toasts shown
- [ ] Component tests pass

**Dependencies**: BE-514 (Swap Details API)

---

#### FE-511: Swap History Page (2.00 days)
**Owner**: Frontend Engineer (frontend-engineer agent FE-2)
**Priority**: P1 (High)

**Description**:
Page displaying user's swap history with filtering and pagination.

**Requirements**:
- List all swaps with details
- Filter by status (all, confirmed, failed)
- Pagination support
- Sort by date (newest first)
- Click to view details

**Implementation**:
```typescript
// pages/SwapHistoryPage.tsx
const SwapHistoryPage: React.FC = () => {
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ['swapHistory', statusFilter, page],
    queryFn: async () => {
      const response = await fetch(
        `/api/swap/history?status=${statusFilter}&page=${page}&pageSize=20`
      );
      return await response.json();
    }
  });

  return (
    <div className="swap-history-page">
      <div className="page-header">
        <h1>Swap History</h1>
      </div>

      <div className="filters">
        <button
          className={statusFilter === 'all' ? 'active' : ''}
          onClick={() => setStatusFilter('all')}
        >
          All
        </button>
        <button
          className={statusFilter === 'confirmed' ? 'active' : ''}
          onClick={() => setStatusFilter('confirmed')}
        >
          Completed
        </button>
        <button
          className={statusFilter === 'failed' ? 'active' : ''}
          onClick={() => setStatusFilter('failed')}
        >
          Failed
        </button>
      </div>

      {isLoading ? (
        <div className="loading-container">
          <Spinner />
        </div>
      ) : (
        <>
          <div className="swap-list">
            {data?.swaps.map((swap: any) => (
              <SwapHistoryItem key={swap.id} swap={swap} />
            ))}
          </div>

          {data && data.totalPages > 1 && (
            <Pagination
              currentPage={page}
              totalPages={data.totalPages}
              onPageChange={setPage}
            />
          )}
        </>
      )}
    </div>
  );
};

// components/swap/SwapHistoryItem.tsx
const SwapHistoryItem: React.FC<{ swap: any }> = ({ swap }) => {
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  return (
    <>
      <div
        className="swap-history-item"
        onClick={() => setIsDetailOpen(true)}
      >
        <div className="swap-tokens">
          <span>{swap.fromAmount.toFixed(4)} {swap.fromTokenSymbol}</span>
          <span className="arrow">→</span>
          <span>{swap.toAmount.toFixed(4)} {swap.toTokenSymbol}</span>
        </div>

        <div className="swap-date">
          {formatDistanceToNow(new Date(swap.createdAt))} ago
        </div>

        <div className={`swap-status ${swap.status}`}>
          {swap.status}
        </div>
      </div>

      <SwapDetailModal
        isOpen={isDetailOpen}
        onClose={() => setIsDetailOpen(false)}
        swapId={swap.id}
      />
    </>
  );
};
```

**Acceptance Criteria**:
- [ ] History page displays swaps
- [ ] Filtering by status works
- [ ] Pagination functional
- [ ] Click opens detail modal
- [ ] Mobile responsive
- [ ] Component tests pass

**Dependencies**: BE-513 (History API)

---

#### FE-512: Swap Detail Modal (1.00 day)
**Owner**: Frontend Engineer (frontend-engineer agent FE-1)
**Priority**: P2 (Medium)

**Description**:
Modal displaying detailed information for a single swap.

**Requirements**:
- Show complete swap details
- Display transaction hash with link
- Show fees and gas used
- Display timestamps

**Acceptance Criteria**:
- [ ] Modal displays all swap details
- [ ] Transaction link works
- [ ] Fees and gas shown
- [ ] Mobile responsive
- [ ] Component tests pass

**Dependencies**: BE-514 (Details API)

---

## State Management

### Swap State
```typescript
// store/swapStore.ts
interface SwapState {
  fromToken: Token | null;
  toToken: Token | null;
  fromAmount: string;
  toAmount: string;
  slippageTolerance: number;
  quote: SwapQuote | null;
  isExecuting: boolean;

  setFromToken: (token: Token | null) => void;
  setToToken: (token: Token | null) => void;
  setFromAmount: (amount: string) => void;
  setSlippageTolerance: (slippage: number) => void;
  flipTokens: () => void;
  executeSwap: () => Promise<void>;
  reset: () => void;
}

export const useSwapStore = create<SwapState>((set, get) => ({
  fromToken: null,
  toToken: null,
  fromAmount: '',
  toAmount: '',
  slippageTolerance: 1.0,
  quote: null,
  isExecuting: false,

  setFromToken: (token) => set({ fromToken: token }),
  setToToken: (token) => set({ toToken: token }),
  setFromAmount: (amount) => set({ fromAmount: amount }),
  setSlippageTolerance: (slippage) => set({ slippageTolerance: slippage }),

  flipTokens: () => {
    const { fromToken, toToken } = get();
    set({
      fromToken: toToken,
      toToken: fromToken,
      fromAmount: '',
      toAmount: ''
    });
  },

  executeSwap: async () => {
    const { fromToken, toToken, fromAmount, slippageTolerance } = get();

    if (!fromToken || !toToken || !fromAmount) {
      throw new Error('Invalid swap parameters');
    }

    set({ isExecuting: true });

    try {
      const response = await fetch('/api/swap/execute', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          fromToken: fromToken.address,
          toToken: toToken.address,
          fromAmount: parseFloat(fromAmount),
          slippageTolerance
        })
      });

      if (!response.ok) {
        throw new Error('Swap execution failed');
      }

      const result = await response.json();

      set({ isExecuting: false });
      return result;
    } catch (error) {
      set({ isExecuting: false });
      throw error;
    }
  },

  reset: () => set({
    fromToken: null,
    toToken: null,
    fromAmount: '',
    toAmount: '',
    quote: null,
    isExecuting: false
  })
}));
```

---

## Testing Requirements

### Component Tests (Vitest + React Testing Library)
- [ ] TokenSelectionModal renders and filters correctly
- [ ] SwapInterface handles token selection
- [ ] Amount inputs accept valid numbers
- [ ] Flip button swaps tokens
- [ ] SlippageSettings validates range
- [ ] PriceImpactIndicator shows correct color
- [ ] FeeBreakdown displays fees correctly
- [ ] ConfirmationModal shows swap details

### E2E Tests (Playwright)
- [ ] Complete swap flow (USDC → ETH)
- [ ] Token approval flow
- [ ] Slippage settings persistence
- [ ] Swap history displays correctly
- [ ] Error handling for insufficient balance

### Integration Tests
- [ ] Quote API integration
- [ ] Execute API integration
- [ ] Balance updates after swap
- [ ] History API integration

---

## Responsive Design

### Breakpoints
- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

### Mobile Optimizations
- Stack token inputs vertically
- Full-width buttons
- Touch-friendly button sizes (min 44px)
- Simplified fee breakdown
- Bottom sheet for token selection

---

## Accessibility

### WCAG 2.1 AA Compliance
- [ ] Keyboard navigation support
- [ ] Focus indicators visible
- [ ] Screen reader announcements
- [ ] Color contrast ratios > 4.5:1
- [ ] Alt text for icons
- [ ] ARIA labels for interactive elements

---

## Definition of Done (Frontend)

Sprint N05 frontend work is **DONE** when:

- [ ] All P0 tasks completed (9 tasks)
- [ ] Swap interface functional
- [ ] Token selection working
- [ ] Exchange rates update every 30s
- [ ] Slippage settings functional
- [ ] Swap execution working
- [ ] History page displays swaps
- [ ] Mobile responsive (3 devices tested)
- [ ] Accessibility score > 90
- [ ] Zero console errors
- [ ] Component tests pass
- [ ] E2E tests pass
- [ ] Code reviewed and approved

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-05 | Frontend Lead | Initial Sprint N05 Frontend Plan |

---

**FRONTEND TEAM STATUS**: **READY TO START**

**NEXT STEPS**:
1. **Day 1**: Setup project structure
2. **Day 1**: Begin FE-501 (Token Selection Modal)
3. **Day 2**: FE-502 (Swap Interface)

---

**End of Sprint N05 Frontend Plan**
