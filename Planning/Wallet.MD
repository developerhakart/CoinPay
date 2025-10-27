# Product Requirements Document: CoinPay Unified Wallet Platform

## Executive Summary

This PRD outlines the implementation of a comprehensive crypto payment solution combining Circle's Modular Wallets SDK for gasless transactions and RedotPay's global payout infrastructure for crypto-to-fiat conversions. The platform enables users to manage crypto assets, send gasless on-chain transactions, and cash out to local bank accounts worldwide. The solution leverages Circle's infrastructure for passkey-based authentication, smart contract accounts, and sponsored transactions, while integrating RedotPay's multi-currency payout network for seamless crypto-to-fiat conversions.

## Product Overview

### Objective
Create a unified crypto payment platform that provides users with:

**Blockchain Operations (Circle Integration)**
- Gasless USDC transactions (sponsored by paymaster)
- Passkey-based authentication (no seed phrases)
- Smart account wallets with account abstraction
- Multi-chain support across EVM-compatible networks

**Global Payout & Fiat Integration (RedotPay Integration)**
- Crypto-to-fiat conversion and local bank payouts
- Multi-currency support (BTC, ETH, USDC, and 100+ local currencies)
- Low-cost international transfers
- 24/7 instant settlement to bank accounts worldwide

**Enhanced Services**
- In-app crypto swap/exchange
- Yield earning on crypto holdings
- P2P marketplace for peer-to-peer trading
- Credit services for liquidity access

### Value Proposition
- **User Experience**: Eliminate gas fees for end users, seamless crypto-to-fiat conversion, improving onboarding and transaction flow
- **Security**: Passkey authentication provides phishing-resistant, device-based security
- **Simplification**: Abstract blockchain complexity with account abstraction (ERC-4337), direct bank account integration
- **Cost Efficiency**: Circle sponsors transaction fees via paymaster service, low-cost global payouts
- **Global Reach**: Send crypto, receive local currency in THB, PHP, BRL, USD, and other supported fiat currencies
- **Accessibility**: 24/7 availability for both on-chain and off-ramp operations

## Technical Architecture

### Core Components

#### 1. SDK Integration
- **Package**: `@circle-fin/modular-wallets-core`
- **Platforms**: Web, iOS, Android
- **Version**: 1.x.x (latest stable)

#### 2. Authentication System
- **Method**: WebAuthn Passkeys
- **Domain Configuration**: Required for passkey registration
- **Modes**:
  - Register: Create new passkey for new users
  - Login: Authenticate existing users with stored passkey

#### 3. Smart Account Architecture
- **Account Type**: Circle Smart Account (ERC-4337 compatible)
- **Owner**: Passkey credential (WebAuthn)
- **Bundler**: Circle's bundler service for UserOperation submission
- **Paymaster**: Circle's paymaster for gas sponsorship

#### 4. Blockchain Support
Supported networks include:
- **Mainnet**: Arbitrum, Avalanche, Base, Optimism, Polygon, Unichain
- **Testnet**: Arbitrum Sepolia, Avalanche Fuji, Base Sepolia, Optimism Sepolia, Polygon Amoy, Unichain Sepolia

#### 5. Fiat Gateway Integration (RedotPay)
- **Service Provider**: RedotPay Global Payout
- **Capabilities**: Crypto-to-fiat conversion, bank account settlement
- **Supported Assets**: BTC, ETH, USDC, USDT, and other major cryptocurrencies
- **Supported Fiat Currencies**: USD, THB (Thai Baht), PHP (Philippine Peso), BRL (Brazilian Real), and 100+ additional currencies
- **Settlement Methods**: Direct bank transfer, local payment networks
- **Processing Time**: Instant to 24 hours depending on destination country

#### 6. Exchange & Liquidity Services
- **Swap Service**: In-app crypto-to-crypto exchange
- **Liquidity Aggregation**: Best rate routing across multiple DEXs and CEXs
- **Earn Service**: Staking and yield farming opportunities
- **P2P Marketplace**: Peer-to-peer trading platform
- **Credit Service**: Collateralized lending against crypto holdings

## Functional Requirements

### FR-1: Configuration & Setup

#### FR-1.1: Console Configuration
- Create Client Key in Circle Console
- Configure Passkey Domain Name
- Retrieve Client URL for SDK initialization
- Set up webhook subscriptions for transaction monitoring

#### FR-1.2: Environment Configuration
```
CLIENT_KEY=<circle-client-key>
CLIENT_URL=<circle-client-url>
```

### FR-2: Passkey Management

#### FR-2.1: Passkey Registration
- User initiates wallet creation
- System prompts for username
- WebAuthn creates passkey credential
- Credential stored securely on user's device
- Credential linked to Circle account

#### FR-2.2: Passkey Authentication
- User initiates login
- System requests passkey authentication
- WebAuthn validates credential
- Session established with authenticated credential

#### FR-2.3: Passkey Recovery
- Document recovery flows for lost device scenarios
- Consider backup passkey registration
- Implement account recovery mechanisms

### FR-3: Wallet Creation

#### FR-3.1: Smart Account Initialization
- Initialize modular transport with client configuration
- Create public client for blockchain interaction
- Generate Circle Smart Account with passkey owner
- Create bundler client for UserOperation handling

#### FR-3.2: Account Properties
- Deterministic address generation based on owner credentials
- Smart contract deployment (on first transaction)
- Support for multiple signers (future enhancement)

### FR-4: Transaction Management

#### FR-4.1: Gasless USDC Transfers
- Encode transfer operation with recipient, token contract, and amount
- Construct UserOperation with paymaster flag enabled
- Submit UserOperation to bundler
- Return transaction hash for tracking

#### FR-4.2: Transaction Monitoring
- Wait for UserOperation receipt
- Parse transaction receipt for status
- Handle transaction failures gracefully
- Emit events for UI updates

#### FR-4.3: Batch Transactions
- Support multiple calls in single UserOperation
- Atomic execution of batched operations
- Optimized gas sponsorship for batches

### FR-5: Token Support

#### FR-5.1: USDC Integration
- Support USDC transfers across all supported chains
- Network-specific contract addresses:
  - Polygon Amoy (Testnet): `0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582`
- Decimal handling (6 decimals for USDC)

#### FR-5.2: Multi-Token Support (Future)
- ERC-20 token transfers
- Native token handling
- Token approval flows

### FR-6: Indexing & Data Retrieval

#### FR-6.1: Transaction History
- Integrate Circle's indexing service
- Query wallet transaction history
- Filter by token type, date range, status

#### FR-6.2: Webhook Integration
- Subscribe to transfer activity events
- Subscribe to UserOperation status events
- Process webhook payloads securely
- Update application state based on events

### FR-7: Global Payout & Fiat Off-Ramp

#### FR-7.1: Bank Account Management
- User can add and verify bank account details
- Support multiple bank accounts per user
- Store bank routing information securely (encrypted)
- Validate bank account details before first payout
- Support international bank account formats (IBAN, SWIFT, local formats)

#### FR-7.2: Crypto-to-Fiat Conversion
- Display real-time exchange rates for crypto to fiat
- Calculate recipient amount before transaction
- Support conversion from BTC, ETH, USDC, USDT to local fiat
- Show conversion fees transparently
- Lock exchange rate for transaction duration

#### FR-7.3: Global Payout Execution
- User selects crypto asset and amount to convert
- System displays available fiat currencies based on region
- User selects recipient bank account
- System calculates conversion and fees
- User confirms payout transaction
- Submit payout request to RedotPay gateway
- Track payout status (pending, processing, completed, failed)
- Notify user upon successful bank deposit

#### FR-7.4: Payout Transaction History
- Display history of all fiat payouts
- Filter by currency, status, date range
- Show exchange rates used for historical transactions
- Export payout history (CSV, PDF)
- Display fees paid for each transaction

#### FR-7.5: Multi-Currency Support
- Support major fiat currencies: USD, EUR, GBP
- Support Asian currencies: THB, PHP, IDR, VND, MYR, SGD
- Support Latin American currencies: BRL, MXN, ARS
- Support African currencies: ZAR, NGN, KES
- Automatically detect user's region and suggest local currency
- Allow manual currency selection

### FR-8: Crypto Exchange & Swap

#### FR-8.1: In-App Swap Interface
- Display available trading pairs
- Real-time price feeds for all supported assets
- Slippage tolerance settings
- Price impact indicators
- Estimated gas fees for swap transactions

#### FR-8.2: Swap Execution
- User selects source and destination tokens
- User enters amount to swap
- System displays exchange rate and output amount
- User confirms swap
- Execute swap through integrated DEX aggregator
- Gasless swap for whitelisted pairs (if available)
- Return swap transaction hash

#### FR-8.3: Liquidity Routing
- Aggregate liquidity from multiple sources
- Find best execution price across DEXs
- Split orders across multiple pools if beneficial
- MEV protection for swap transactions

### FR-9: Exchange Investment Integration (Stablecoin Yield)

#### FR-9.1: Exchange Connection Management
- Support multiple centralized exchanges (WhiteBit, Binance, Kraken, etc.)
- Secure API credential storage per exchange
- Exchange account linking and verification
- API health monitoring and status tracking
- Automatic credential refresh and validation
- Exchange-specific rate limits management

#### FR-9.2: Investment Plan Discovery
- Fetch available Flex/Earn plans from connected exchanges
- Display plan metadata: APY/APR, minimum/maximum amounts, terms
- Filter plans by asset type (USDC, USDT, other stablecoins)
- Sort plans by yield rate, risk level, liquidity terms
- Show plan availability based on user region
- Cache plan data with automatic refresh (hourly)
- Compare yields across multiple exchanges

#### FR-9.3: Stablecoin Investment Execution
- User selects investment plan from available options
- Display investment preview: amount, expected yield, terms
- Transfer stablecoin from Circle wallet to exchange
- Create investment position via exchange API (WhiteBit Flex Investment)
- Support auto-reinvestment configuration (compound rewards)
- Return investment transaction ID for tracking
- Confirm investment creation and show position details

#### FR-9.4: Investment Position Management
- Display all active investments across exchanges
- Show per-position metrics: principal, accrued rewards, current APY
- Track investment status (active, pending, matured, withdrawn)
- Calculate aggregate portfolio yield and total value
- Support position modifications (increase investment, change reinvest settings)
- Display investment timeline and maturity dates
- Show historical performance and reward claims

#### FR-9.5: Rewards and Withdrawal
- Display accrued rewards in real-time
- Manual reward claiming (if not auto-reinvested)
- Withdrawal request to return funds to Circle wallet
- Show withdrawal processing time and fees
- Track reward history and compound growth
- Tax reporting for earned rewards (if applicable)

#### FR-9.6: Risk Management
- Display investment risk indicators (platform risk, liquidity risk)
- Show platform insurance/protection details
- Set investment limits and alerts
- Monitor exchange health and platform status
- Automatic position monitoring for anomalies
- Emergency withdrawal mechanisms

### FR-10: Additional Services

#### FR-10.1: On-Chain Staking Service
- Display available on-chain staking opportunities
- Show APY/APR for each staking option
- Support for liquid staking (receive derivative tokens)
- Automatic reward claiming and compounding
- Unstaking with withdrawal period indicators
- Staking history and rewards tracking

#### FR-10.2: P2P Marketplace
- User can create buy/sell offers for crypto
- Price negotiation interface
- Escrow service for secure P2P trades
- Reputation system for traders
- Dispute resolution mechanism
- Support for multiple payment methods (bank transfer, cash, etc.)

#### FR-10.3: Credit Services
- Collateralized lending against crypto holdings
- Loan-to-Value (LTV) calculator
- Interest rate display for different collateral types
- Liquidation threshold warnings
- Automatic collateral top-up
- Loan repayment tracking and history

## Non-Functional Requirements

### NFR-1: Security
- Client keys stored securely (environment variables, secure storage)
- API keys for backend operations (separate from client keys)
- Passkey credentials never leave device
- HTTPS-only communication with Circle APIs
- Validate webhook signatures

### NFR-2: Performance
- Transaction submission < 2 seconds
- UserOperation confirmation < 30 seconds (network dependent)
- Support for 1000+ concurrent users
- Efficient caching of blockchain state

### NFR-3: Reliability
- 99.9% uptime for wallet operations
- Retry logic for failed UserOperations
- Graceful degradation when bundler unavailable
- Transaction queue management

### NFR-4: Scalability
- Horizontal scaling for backend services
- Rate limiting adherence for Circle APIs
- Efficient webhook processing
- Database indexing for transaction queries

### NFR-5: Usability
- Clear error messages for failed transactions
- Transaction status indicators
- Estimated confirmation times
- User-friendly passkey prompts

## API Integration Requirements

### REST API Endpoints (Backend)

#### Authentication
- **Endpoint**: Authenticate with API Key
- **Purpose**: Backend access to Circle Indexing Service
- **Headers**: `X-API-Key: <api-key>`

#### Transaction Query
- **Purpose**: Retrieve wallet transaction history
- **Authentication**: API Key
- **Response**: Paginated transaction list

### SDK Methods (Frontend)

#### Core Methods
```typescript
// Passkey creation
toPasskeyTransport(clientUrl, clientKey)
toWebAuthnCredential({ transport, mode, username })

// Client setup
toModularTransport(clientUrl + "/chain", clientKey)
createPublicClient({ chain, transport })

// Account creation
toCircleSmartAccount({ client, owner })
createBundlerClient({ smartAccount, chain, transport })

// Transactions
encodeTransfer(to, tokenAddress, amount)
bundlerClient.sendUserOperation({ calls, paymaster })
bundlerClient.waitForUserOperationReceipt({ hash })
```

## Data Models

### Wallet Account
```typescript
interface WalletAccount {
  address: string;              // Smart account address
  ownerCredentialId: string;    // Passkey credential ID
  chainId: number;              // Blockchain network ID
  createdAt: Date;
  lastActivityAt: Date;
}
```

### Transaction Record
```typescript
interface TransactionRecord {
  id: string;
  userOpHash: string;           // UserOperation hash
  transactionHash: string;      // Blockchain tx hash
  from: string;                 // Smart account address
  to: string;                   // Recipient address
  tokenAddress: string;         // Token contract address
  amount: bigint;               // Transfer amount
  status: 'pending' | 'confirmed' | 'failed';
  gasSponsored: boolean;        // Always true for gasless
  chainId: number;
  createdAt: Date;
  confirmedAt?: Date;
}
```

### Passkey Credential
```typescript
interface PasskeyCredential {
  id: string;                   // Credential ID
  publicKey: string;            // Public key (hex)
  username: string;             // User-provided name
  deviceType: string;           // Device info
  createdAt: Date;
}
```

### Bank Account
```typescript
interface BankAccount {
  id: string;
  userId: string;
  accountHolderName: string;
  accountNumber: string;        // Encrypted
  routingNumber?: string;       // For US accounts
  iban?: string;                // For European accounts
  swiftCode?: string;           // For international transfers
  bankName: string;
  bankBranch?: string;
  country: string;              // ISO country code
  currency: string;             // ISO currency code (USD, THB, PHP, etc.)
  isVerified: boolean;
  isPrimary: boolean;
  createdAt: Date;
  verifiedAt?: Date;
}
```

### Fiat Payout Transaction
```typescript
interface FiatPayoutTransaction {
  id: string;
  userId: string;
  walletAddress: string;
  bankAccountId: string;
  cryptoAsset: string;          // BTC, ETH, USDC, etc.
  cryptoAmount: bigint;
  fiatCurrency: string;         // USD, THB, PHP, BRL, etc.
  fiatAmount: number;
  exchangeRate: number;
  fees: {
    conversionFee: number;
    networkFee: number;
    payoutFee: number;
    totalFee: number;
  };
  status: 'pending' | 'processing' | 'completed' | 'failed' | 'cancelled';
  providerTransactionId?: string;  // RedotPay transaction ID
  failureReason?: string;
  estimatedArrival?: Date;
  createdAt: Date;
  processedAt?: Date;
  completedAt?: Date;
}
```

### Swap Transaction
```typescript
interface SwapTransaction {
  id: string;
  userId: string;
  walletAddress: string;
  fromToken: string;            // Token contract address
  toToken: string;              // Token contract address
  fromAmount: bigint;
  toAmount: bigint;
  exchangeRate: number;
  slippageTolerance: number;    // Percentage
  priceImpact: number;          // Percentage
  route: string[];              // DEX routing path
  gasSponsored: boolean;
  transactionHash?: string;
  status: 'pending' | 'confirmed' | 'failed';
  createdAt: Date;
  confirmedAt?: Date;
}
```

### Staking Position
```typescript
interface StakingPosition {
  id: string;
  userId: string;
  walletAddress: string;
  tokenAddress: string;
  stakedAmount: bigint;
  derivativeToken?: string;     // Liquid staking token address
  apy: number;
  rewardsEarned: bigint;
  lastRewardClaim?: Date;
  stakingStartDate: Date;
  unstakePeriod?: number;       // Days
  status: 'active' | 'unstaking' | 'completed';
  createdAt: Date;
}
```

### P2P Trade
```typescript
interface P2PTrade {
  id: string;
  sellerId: string;
  buyerId?: string;
  cryptoAsset: string;
  cryptoAmount: bigint;
  fiatCurrency: string;
  fiatAmount: number;
  exchangeRate: number;
  paymentMethod: string;
  tradeType: 'buy' | 'sell';
  status: 'open' | 'locked' | 'in_escrow' | 'completed' | 'disputed' | 'cancelled';
  escrowAddress?: string;
  disputeReason?: string;
  createdAt: Date;
  expiresAt: Date;
  completedAt?: Date;
}
```

### Credit Loan
```typescript
interface CreditLoan {
  id: string;
  userId: string;
  walletAddress: string;
  collateralToken: string;
  collateralAmount: bigint;
  loanToken: string;
  loanAmount: bigint;
  interestRate: number;         // APR percentage
  loanToValue: number;          // LTV percentage
  liquidationThreshold: number; // Percentage
  currentLTV: number;           // Current LTV percentage
  repaidAmount: bigint;
  status: 'active' | 'repaid' | 'liquidated' | 'defaulted';
  createdAt: Date;
  dueDate: Date;
  repaidAt?: Date;
  liquidatedAt?: Date;
}
```

### Exchange Connection
```typescript
interface ExchangeConnection {
  id: string;
  userId: string;
  exchangeName: string;         // 'whitebit', 'binance', 'kraken', etc.
  exchangeDisplayName: string;  // 'WhiteBit', 'Binance', 'Kraken'
  apiKey: string;               // Encrypted
  apiSecret: string;            // Encrypted
  apiPassphrase?: string;       // Encrypted (if required by exchange)
  permissions: string[];        // ['read', 'trade', 'withdraw']
  isActive: boolean;
  lastHealthCheck?: Date;
  healthStatus: 'healthy' | 'degraded' | 'failed';
  rateLimit: {
    requestsPerInterval: number;
    intervalSeconds: number;
    currentUsage: number;
  };
  createdAt: Date;
  verifiedAt?: Date;
  lastUsedAt?: Date;
}
```

### Investment Plan
```typescript
interface InvestmentPlan {
  id: string;
  exchangeConnectionId: string;
  exchangeName: string;
  planExternalId: string;       // UUID from exchange (e.g., WhiteBit plan ID)
  planName: string;
  planType: 'flex' | 'fixed' | 'locked';
  asset: string;                // 'USDC', 'USDT', 'DAI'
  apy: number;                  // Annual percentage yield
  apr?: number;                 // Annual percentage rate (if different from APY)
  minimumAmount: number;
  maximumAmount?: number;
  lockPeriod?: number;          // Days (null for flexible plans)
  withdrawalPeriod?: number;    // Days notice required
  compoundingFrequency: string; // 'daily', 'weekly', 'monthly', 'none'
  supportsAutoReinvest: boolean;
  riskLevel: 'low' | 'medium' | 'high';
  insuranceCoverage?: {
    provider: string;
    coverageAmount: number;
    coveragePercentage: number;
  };
  regionRestrictions?: string[]; // ISO country codes where not available
  isActive: boolean;
  totalCapacity?: number;       // Maximum total investment allowed
  currentUtilization?: number;  // Current total invested
  metadata: {
    description?: string;
    termsUrl?: string;
    insuranceDetails?: string;
  };
  createdAt: Date;
  updatedAt: Date;
  lastSyncedAt: Date;
}
```

### Exchange Investment Position
```typescript
interface ExchangeInvestmentPosition {
  id: string;
  userId: string;
  walletAddress: string;
  exchangeConnectionId: string;
  investmentPlanId: string;
  exchangeName: string;
  planName: string;
  asset: string;                // 'USDC', 'USDT', 'DAI'
  principalAmount: bigint;      // Original investment amount
  currentValue: bigint;         // Principal + accrued rewards
  accruedRewards: bigint;       // Total rewards earned
  claimedRewards: bigint;       // Total rewards claimed/withdrawn
  currentAPY: number;           // Current APY (may change over time)
  autoReinvest: boolean;
  transactionId: string;        // Exchange transaction ID
  circleTransactionHash?: string; // Circle wallet transfer hash
  status: 'pending' | 'active' | 'maturing' | 'matured' | 'withdrawn' | 'failed';
  startDate: Date;
  maturityDate?: Date;          // For fixed/locked plans
  lastRewardCalculation: Date;
  nextRewardDate?: Date;
  withdrawalRequestedAt?: Date;
  withdrawalCompletedAt?: Date;
  performanceMetrics: {
    totalReturnAmount: bigint;
    totalReturnPercentage: number;
    dailyAverageYield: number;
    compoundedTimes: number;
  };
  createdAt: Date;
  updatedAt: Date;
}
```

### Investment Reward
```typescript
interface InvestmentReward {
  id: string;
  investmentPositionId: string;
  userId: string;
  exchangeName: string;
  asset: string;
  rewardAmount: bigint;
  rewardType: 'interest' | 'airdrop' | 'bonus' | 'referral';
  rewardDate: Date;
  wasReinvested: boolean;
  wasClaimed: boolean;
  claimedAt?: Date;
  transactionHash?: string;     // If withdrawn to Circle wallet
  exchangeTransactionId?: string;
  createdAt: Date;
}
```

### Exchange Investment Transaction
```typescript
interface ExchangeInvestmentTransaction {
  id: string;
  userId: string;
  investmentPositionId?: string;
  exchangeConnectionId: string;
  exchangeName: string;
  transactionType: 'deposit' | 'invest' | 'reward' | 'reinvest' | 'withdraw' | 'claim';
  asset: string;
  amount: bigint;
  status: 'pending' | 'processing' | 'completed' | 'failed' | 'cancelled';
  exchangeTransactionId?: string;
  circleTransactionHash?: string;
  errorMessage?: string;
  metadata: {
    planName?: string;
    apy?: number;
    autoReinvest?: boolean;
  };
  createdAt: Date;
  processedAt?: Date;
  completedAt?: Date;
}
```

## User Flows

### Flow 1: New User Onboarding
1. User clicks "Create Wallet"
2. System prompts for username
3. Browser/OS displays passkey creation dialog
4. User confirms with biometric/PIN
5. Passkey created and stored on device
6. Circle Smart Account initialized
7. Wallet address displayed to user

### Flow 2: Existing User Login
1. User clicks "Login with Wallet"
2. System requests passkey authentication
3. Browser/OS displays passkey selection
4. User confirms with biometric/PIN
5. Session authenticated
6. Wallet dashboard displayed

### Flow 3: Send Gasless Transaction
1. User enters recipient address and amount
2. User clicks "Send"
3. System constructs UserOperation
4. Passkey signature requested
5. User confirms with biometric/PIN
6. UserOperation submitted to bundler
7. Progress indicator shown
8. Transaction confirmed
9. Success notification displayed

### Flow 4: Cash Out to Bank Account (Fiat Payout)
1. User navigates to "Cash Out" section
2. User selects crypto asset (BTC, ETH, USDC) and amount
3. System displays available fiat currencies based on user location
4. User selects destination currency (e.g., THB, PHP, USD)
5. System shows conversion preview:
   - Exchange rate
   - Estimated fiat amount
   - Conversion fees
   - Total amount to receive
6. User selects or adds bank account
7. User confirms payout details
8. System submits payout request to RedotPay
9. Crypto deducted from wallet
10. Payout status shown as "Processing"
11. System displays estimated arrival time
12. User receives notification when funds arrive in bank
13. Transaction marked as "Completed"

### Flow 5: Crypto Swap/Exchange
1. User navigates to "Swap" section
2. User selects source token and amount
3. User selects destination token
4. System displays:
   - Current exchange rate
   - Output amount
   - Price impact
   - Estimated fees
   - Best route across DEXs
5. User sets slippage tolerance (optional)
6. User clicks "Swap"
7. Passkey signature requested
8. User confirms with biometric/PIN
9. Swap transaction submitted
10. Progress indicator shown
11. Transaction confirmed on blockchain
12. Token balances updated
13. Success notification displayed

### Flow 6: Stake Crypto for Yield
1. User navigates to "Earn" section
2. System displays available staking opportunities with APY
3. User selects staking product
4. User enters amount to stake
5. System shows estimated rewards and unstaking period
6. User confirms staking
7. Passkey signature requested
8. User confirms with biometric/PIN
9. Tokens staked, liquid derivative tokens received (if applicable)
10. Staking position shown in dashboard
11. Rewards automatically claimed and compounded
12. User can unstake anytime (subject to unstaking period)

### Flow 7: P2P Trade
1. User navigates to "P2P Marketplace"
2. User creates offer or browses existing offers
3. User selects trade (buy or sell crypto)
4. Both parties agree on terms
5. Crypto locked in escrow
6. Buyer transfers fiat via selected payment method
7. Buyer marks payment as sent
8. Seller confirms receipt of fiat
9. Crypto released from escrow to buyer
10. Trade marked as completed
11. Both parties can rate each other

### Flow 8: Connect Exchange for Investments
1. User navigates to "Earn" section
2. User selects "Connect Exchange"
3. System displays list of supported exchanges (WhiteBit, Binance, Kraken)
4. User selects exchange (e.g., WhiteBit)
5. System displays instructions for creating API credentials
6. User creates read-only or read+trade API key on exchange
7. User enters API key, secret, and passphrase (if required)
8. System validates API credentials with test call
9. System verifies required permissions (read, trade)
10. Exchange connection saved and marked as active
11. System begins syncing available investment plans
12. User sees connected exchange in dashboard

### Flow 9: Create Stablecoin Investment
1. User navigates to "Invest" section
2. System displays available investment plans from all connected exchanges
3. Plans show: exchange name, asset (USDC/USDT), APY, minimum amount, type (Flex/Fixed)
4. User can filter by: exchange, asset, APY range, plan type
5. User can sort by: highest APY, lowest minimum, plan type
6. User selects investment plan (e.g., WhiteBit USDC Flex 8.5% APY)
7. System displays plan details:
   - Current APY rate
   - Minimum/maximum investment amounts
   - Lock period (if any)
   - Withdrawal terms
   - Risk level and insurance coverage
   - Auto-reinvest option
8. User enters investment amount
9. System validates amount against balance and plan limits
10. User toggles auto-reinvest option (default: enabled)
11. System displays investment preview:
    - Principal amount
    - Estimated daily/monthly/yearly earnings
    - Total expected value after timeframe
    - Network fees (for transfer to exchange)
12. User confirms investment
13. Passkey signature requested
14. User confirms with biometric/PIN
15. System transfers stablecoin from Circle wallet to exchange
16. System creates investment via exchange API (WhiteBit Flex Investment endpoint)
17. Exchange returns transaction ID
18. Investment position created and shown as "Pending"
19. System polls exchange for confirmation
20. Position updated to "Active"
21. User sees investment in portfolio with accruing rewards
22. Push notification sent: "Your USDC investment is now earning 8.5% APY"

### Flow 10: Monitor and Manage Investment Positions
1. User navigates to "Portfolio" or "Investments" section
2. System displays all active investments across exchanges:
   - Exchange name and logo
   - Asset and plan name
   - Principal amount
   - Current value (principal + accrued rewards)
   - Accrued rewards (highlighted if claimable)
   - Current APY
   - Status (Active, Maturing, etc.)
   - Auto-reinvest status
3. User can filter by: exchange, asset, status
4. User can sort by: highest value, highest APY, newest
5. User selects specific investment to view details
6. Detail view shows:
   - Complete position summary
   - Performance chart (value over time)
   - Reward history (daily/weekly accruals)
   - Compounding events (if auto-reinvest enabled)
   - Projected earnings for next 30/90/365 days
   - Risk indicators and insurance info
7. User can modify position:
   - Toggle auto-reinvest on/off
   - Add more to investment (if plan allows)
   - Request withdrawal (if flexible plan)
8. System displays aggregate metrics:
   - Total invested across all exchanges
   - Total accrued rewards
   - Average portfolio APY
   - Total portfolio value

### Flow 11: Withdraw Investment and Rewards
1. User navigates to specific investment position
2. User clicks "Withdraw" button
3. System displays withdrawal options:
   - Withdraw rewards only (keep principal invested)
   - Withdraw partial amount (if plan allows)
   - Withdraw full amount (close position)
4. User selects withdrawal type and amount
5. System displays withdrawal preview:
   - Amount to withdraw
   - Remaining invested (if partial)
   - Processing time (instant, 1-3 days, etc.)
   - Withdrawal fees (if any)
   - Network fees (for transfer to Circle wallet)
   - Early withdrawal penalties (if applicable)
6. User confirms withdrawal request
7. Passkey signature requested (if required)
8. User confirms with biometric/PIN
9. System submits withdrawal request to exchange
10. Exchange processes withdrawal
11. Position status updated to "Withdrawing"
12. User receives notification when funds released from exchange
13. Stablecoin automatically transferred back to Circle wallet
14. Transaction hash shown for on-chain transfer
15. Position updated:
    - If full withdrawal: Status = "Withdrawn", position archived
    - If partial: Principal updated, status remains "Active"
16. Success notification: "X USDC withdrawn and deposited to your wallet"
17. Withdrawal transaction appears in transaction history

### Flow 12: Compare Investment Plans Across Exchanges
1. User navigates to "Invest" section
2. User selects "Compare Plans"
3. System displays comparison view with all available plans
4. Comparison table shows:
   - Exchange name
   - Asset (USDC, USDT, DAI)
   - Plan type (Flex, Fixed, Locked)
   - Current APY (sortable)
   - Minimum investment
   - Lock period
   - Auto-reinvest support
   - Risk level
   - Insurance coverage
5. User can filter by:
   - Specific asset (e.g., USDC only)
   - Plan type (Flexible only)
   - Minimum APY threshold
   - Maximum lock period
   - Exchanges (select/deselect)
6. User can sort by:
   - Highest APY
   - Lowest minimum investment
   - Shortest lock period
   - Best risk-adjusted return
7. Plans highlight best options:
   - Badge: "Highest APY" for top rate
   - Badge: "Most Flexible" for flex plans
   - Badge: "Best Security" for insured plans
8. User selects plan and proceeds to Flow 9 (Create Investment)

## Testing Requirements

### Unit Tests
- Passkey transport creation
- WebAuthn credential handling
- Transaction encoding
- UserOperation construction

### Integration Tests
- End-to-end wallet creation flow
- Transaction submission and confirmation
- Webhook processing
- Error handling scenarios

### Test Networks
- Use Polygon Amoy testnet for development
- Obtain testnet USDC from faucets
- Test all supported chains before mainnet

### Test Cases
| ID | Scenario | Expected Result |
|----|----------|----------------|
| TC-1 | Create new passkey | Credential stored, account created |
| TC-2 | Login with existing passkey | Authentication successful |
| TC-3 | Send gasless USDC | Transaction confirmed, no gas fee |
| TC-4 | Send with insufficient balance | Clear error message |
| TC-5 | Network switch | Correct chain configuration |
| TC-6 | Failed passkey auth | User prompted to retry |
| TC-7 | Bundler timeout | Retry logic triggered |
| TC-8 | Webhook processing | Event captured, state updated |
| TC-9 | Add bank account | Account verified and saved |
| TC-10 | Cash out to bank (valid) | Fiat arrives in bank account |
| TC-11 | Cash out with insufficient crypto | Clear error message |
| TC-12 | Crypto swap execution | Tokens exchanged at quoted rate |
| TC-13 | Swap with high slippage | Warning shown or transaction reverted |
| TC-14 | Stake tokens | Position created, rewards accruing |
| TC-15 | Unstake before period ends | Early withdrawal penalty applied |
| TC-16 | P2P trade completion | Escrow released, trade completed |
| TC-17 | P2P trade dispute | Dispute mechanism triggered |
| TC-18 | Take crypto loan | Collateral locked, loan issued |
| TC-19 | Loan liquidation warning | Alert sent before liquidation |
| TC-20 | Multi-currency payout rates | Correct exchange rates displayed |
| TC-21 | Connect exchange account | API credentials validated, connection active |
| TC-22 | Invalid exchange API credentials | Clear error message shown |
| TC-23 | Fetch investment plans | Plans displayed with correct APY and terms |
| TC-24 | Create stablecoin investment | Investment position created, status "Active" |
| TC-25 | Investment with insufficient balance | Clear error message displayed |
| TC-26 | Investment below minimum amount | Error shown with minimum requirement |
| TC-27 | Monitor investment rewards | Rewards accruing correctly based on APY |
| TC-28 | Auto-reinvest enabled | Rewards automatically compounded |
| TC-29 | Withdraw investment rewards only | Rewards withdrawn, principal remains invested |
| TC-30 | Withdraw full investment | Position closed, funds returned to wallet |
| TC-31 | Compare plans across exchanges | Correct sorting and filtering |
| TC-32 | Exchange API rate limit hit | Retry logic triggered, request queued |
| TC-33 | Exchange API downtime | Graceful degradation, cached data shown |
| TC-34 | Investment plan becomes inactive | User notified, new investments blocked |
| TC-35 | Multiple exchange connections | All connections managed independently |

## Implementation Phases

### Phase 1: Foundation & Core Wallet (Week 1-3)
- [ ] Circle Console setup (client key, domain, webhooks)
- [ ] SDK installation and configuration
- [ ] Environment setup for testnet
- [ ] Basic passkey registration flow
- [ ] Smart account creation
- [ ] USDC transfer implementation
- [ ] Transaction status tracking
- [ ] Error handling

### Phase 2: Wallet UI & Basic Features (Week 4-5)
- [ ] UI for wallet creation
- [ ] UI for transaction sending
- [ ] Transaction history display
- [ ] Webhook integration
- [ ] Multi-chain support UI
- [ ] Token balance display

### Phase 3: RedotPay Integration - Fiat Gateway (Week 6-8)
- [ ] RedotPay API integration research and setup
- [ ] Bank account management implementation
- [ ] Crypto-to-fiat conversion engine
- [ ] Exchange rate API integration
- [ ] Fiat payout execution flow
- [ ] Payout status tracking
- [ ] Bank account verification flow
- [ ] Multi-currency support (THB, PHP, BRL, USD, etc.)
- [ ] Fee calculation and display

### Phase 4: Swap & Exchange Features (Week 9-10)
- [ ] DEX aggregator integration (1inch, 0x, or similar)
- [ ] Swap interface UI
- [ ] Real-time price feeds
- [ ] Liquidity routing optimization
- [ ] Slippage protection
- [ ] Price impact calculation
- [ ] Gasless swap for whitelisted pairs
- [ ] Swap transaction history

### Phase 5: Exchange Investment Integration (Week 11-13)
- [ ] Exchange adapter architecture design
- [ ] WhiteBit API integration (Flex Investment endpoint)
- [ ] Secure API credential storage and management
- [ ] Exchange connection UI (add/verify/manage connections)
- [ ] Investment plan discovery and caching service
- [ ] Investment plan listing and comparison UI
- [ ] Stablecoin investment execution flow
- [ ] Transfer from Circle wallet to exchange integration
- [ ] Investment position tracking and storage
- [ ] Auto-reinvest configuration
- [ ] Rewards calculation and display
- [ ] Investment dashboard and portfolio view
- [ ] Withdrawal flow (partial and full)
- [ ] Exchange webhook integration for status updates
- [ ] Rate limiting and retry logic
- [ ] Investment transaction history
- [ ] Performance metrics and analytics
- [ ] Multi-exchange support framework (prepare for Binance, Kraken)

### Phase 6: Additional Services - On-Chain Staking & P2P (Week 14-16)
- [ ] Staking protocol integrations
- [ ] Earn/staking UI
- [ ] APY calculation and display
- [ ] Rewards tracking
- [ ] P2P marketplace infrastructure
- [ ] Escrow smart contracts
- [ ] P2P trading UI
- [ ] Reputation system
- [ ] Dispute resolution mechanism

### Phase 7: Credit Services (Week 17-18)
- [ ] Lending protocol integration
- [ ] Loan origination flow
- [ ] LTV calculator
- [ ] Liquidation monitoring system
- [ ] Automatic collateral management
- [ ] Interest accrual tracking
- [ ] Loan repayment UI

### Phase 8: Testing & Security Audit (Week 19-20)
- [ ] Comprehensive testing on testnet for all features
- [ ] End-to-end testing of fiat payouts
- [ ] Swap execution testing across multiple DEXs
- [ ] Exchange investment integration testing (WhiteBit API)
- [ ] Investment position lifecycle testing
- [ ] Multi-exchange coordination testing
- [ ] Security audit for smart contracts
- [ ] Security audit for exchange API credential storage
- [ ] Penetration testing for APIs
- [ ] Performance optimization
- [ ] Load testing
- [ ] Exchange API rate limit testing

### Phase 9: Documentation & Compliance (Week 21)
- [ ] Developer documentation
- [ ] User guides and tutorials
- [ ] API documentation
- [ ] Exchange integration documentation
- [ ] Investment risk disclosures
- [ ] Compliance review (KYC/AML for fiat)
- [ ] Compliance review for exchange custody
- [ ] Terms of service updates (investment features)
- [ ] Privacy policy updates (API credential storage)

### Phase 10: Beta Launch (Week 22-23)
- [ ] Testnet to mainnet migration
- [ ] WhiteBit production API access setup
- [ ] Limited beta user rollout
- [ ] Monitoring and alerting setup
- [ ] Exchange health monitoring
- [ ] Customer support training (including investment features)
- [ ] Bug fixes from beta feedback
- [ ] Performance tuning

### Phase 11: Production Launch & Scale (Week 24-25)
- [ ] Full mainnet configuration
- [ ] Additional exchange integrations (Binance, Kraken)
- [ ] Gradual rollout to all users
- [ ] Marketing and user acquisition (highlight investment yields)
- [ ] 24/7 monitoring (including exchange health)
- [ ] Support documentation
- [ ] Continuous optimization
- [ ] Investment performance reporting and analytics

## Dependencies

### External Services

**Circle Infrastructure**
- Circle Modular Wallets SDK
- Circle Console access
- Circle Bundler service
- Circle Paymaster service
- Circle Indexing service

**RedotPay Services**
- RedotPay Global Payout API
- RedotPay Crypto-to-Fiat conversion
- RedotPay Bank settlement network
- RedotPay Swap/Exchange API
- RedotPay Earn/Staking platform
- RedotPay P2P Marketplace
- RedotPay Credit services

**Exchange Integrations (Stablecoin Investment)**
- WhiteBit Flex Investment API (`/api/v4/main-account/smart-flex/investments/invest`)
- WhiteBit B2B Partner Program access
- WhiteBit API authentication (API key + signature)
- WhiteBit rate limits: 1000 requests/10 seconds
- Binance Earn API (future integration)
- Kraken Staking API (future integration)
- Coinbase Earn API (future integration)
- Exchange webhook endpoints for position updates

**Third-Party Integrations**
- DEX aggregator (1inch, 0x, or Uniswap)
- Price feed oracles (Chainlink, Band Protocol)
- KYC/AML provider (Sumsub, Onfido, or similar)
- SMS/Email notification service
- Analytics platform

### Development Dependencies
```json
{
  "@circle-fin/modular-wallets-core": "^1.x.x",
  "viem": "^2.21.27",
  "react": "^18.2.0",
  "typescript": "^5.0.3"
}
```

### Infrastructure
- HTTPS-enabled domain for passkey
- Backend server for API key operations
- Webhook endpoint for Circle events
- Webhook endpoint for RedotPay payout notifications
- Webhook endpoint for exchange status updates
- Database for transaction records (crypto + fiat + investments)
- Secure vault for encrypted bank account data
- Secure vault for encrypted exchange API credentials
- Redis cache for exchange rates
- Redis cache for investment plan metadata
- Queue system for async payout processing
- Queue system for investment position sync jobs
- Background workers for reward calculation
- CDN for static assets
- Rate limiter for exchange API calls

## Risk & Mitigation

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Bundler downtime | High | Low | Implement retry logic, status page monitoring |
| Passkey device loss | Medium | Medium | Multi-device backup, recovery flows |
| Gas sponsorship limits | Medium | Low | Monitor quota, implement fallback to user-paid gas |
| Chain congestion | Low | Medium | Set appropriate UserOp gas limits |
| SDK breaking changes | Low | Low | Pin SDK versions, test updates thoroughly |
| RedotPay API downtime | High | Low | Queue failed payouts, retry mechanism, status notifications |
| Exchange rate volatility | Medium | Medium | Lock rates during transaction, slippage protection |
| Bank transfer failures | Medium | Low | Retry logic, manual review process, refund mechanism |
| Regulatory compliance | High | Medium | KYC/AML integration, legal review, license acquisition |
| Liquidity shortages | Medium | Low | Multi-source liquidity, reserve monitoring, circuit breakers |
| Smart contract vulnerabilities | Critical | Low | Professional audits, bug bounty, gradual rollout |
| P2P fraud/scams | Medium | Medium | Escrow system, reputation scores, dispute resolution |
| Loan liquidations | Medium | Medium | Real-time monitoring, early warnings, auto-collateral top-up |
| Data breach (bank details) | Critical | Low | Encryption at rest/transit, vault storage, security audits |
| Currency conversion errors | High | Low | Rate validation, sanity checks, transaction limits |
| Exchange API credential theft | Critical | Low | Encrypted storage, API permission scoping, IP whitelisting, 2FA |
| Exchange platform insolvency | Critical | Low | Diversify across exchanges, monitor platform health, withdrawal limits |
| Exchange API changes | Medium | Medium | Version pinning, API change monitoring, adapter pattern |
| Investment plan rate changes | Low | High | Real-time plan sync, user notifications, rate change alerts |
| Exchange rate limiting | Medium | Medium | Request queuing, rate limit monitoring, exponential backoff |
| Stablecoin depeg risk | Medium | Low | Multi-stablecoin support, depeg monitoring, automatic alerts |
| Delayed withdrawals | Medium | Medium | Clear SLA communication, status tracking, escalation process |
| APY calculation errors | High | Low | Independent verification, automated testing, audit logs |

## Success Metrics

### User Adoption
- 80% of new users create passkey wallet
- 90% login success rate
- <5% passkey authentication failures
- 50% of users try fiat payout within first month
- 30% of users use swap feature monthly
- 40% of users create at least one investment position
- 25% of users connect an exchange account
- 15% of users have active investments on multiple exchanges

### Transaction Performance
- 95% of crypto transactions confirm within 30 seconds
- 99.5% success rate for UserOperations
- 100% gas sponsorship (no user fees for whitelisted operations)
- 90% of fiat payouts complete within 24 hours
- 95% success rate for bank transfers
- 99% accuracy for exchange rate calculations
- 95% of investment positions activate within 5 minutes
- 99% success rate for investment creation
- 90% of investment withdrawals complete within 24 hours
- 99.9% accuracy for APY calculations and reward accruals

### System Performance
- <2s average API response time
- <100ms webhook processing time
- 99.9% uptime for wallet services
- <5s exchange rate quote generation
- <3s swap execution time
- <1s balance refresh time
- <3s investment plan sync time
- <2s investment portfolio load time
- <1s reward calculation update time
- 99.5% uptime for exchange API connections

### Financial Metrics
- Average transaction value >$50
- Monthly active users (MAU) growth rate >20%
- User retention rate >70% after 3 months
- Average revenue per user (ARPU) from fees
- Total value locked (TVL) in on-chain staking >$1M
- Total value locked (TVL) in exchange investments >$5M
- Average investment position size >$500
- Investment retention rate >80% (users keep positions active)
- Average portfolio APY >6%
- P2P marketplace transaction volume >$100K/month

## Documentation Requirements

### Developer Documentation
- SDK integration guide
- API reference
- Code examples for common operations
- Troubleshooting guide

### User Documentation
- Passkey setup instructions
- Transaction sending guide
- Recovery procedures
- FAQ for common issues

## Compliance & Regulatory

### Crypto Wallet Operations
- Ensure compliance with local regulations for digital wallets
- Privacy policy updates for passkey storage
- Terms of service for gas sponsorship
- Smart contract audit reports

### Fiat Operations & Money Transmission
- **KYC/AML Requirements**: Implement Know Your Customer verification for fiat operations
- **Transaction Monitoring**: Monitor for suspicious activities and report as required
- **Money Transmitter Licenses**: Obtain necessary licenses in operating jurisdictions
- **Bank Account Verification**: Verify ownership of bank accounts before payouts
- **Transaction Limits**: Implement daily/monthly limits per regulatory requirements
- **Record Keeping**: Maintain transaction records for required duration (typically 5-7 years)
- **Sanctions Screening**: Screen users against OFAC and other sanctions lists
- **Tax Reporting**: Implement reporting mechanisms for tax authorities (e.g., 1099 forms in US)

### Data Privacy
- **GDPR Compliance**: For European users (data portability, right to deletion)
- **CCPA Compliance**: For California users
- **Data Encryption**: End-to-end encryption for sensitive data (bank details, personal info)
- **Data Retention**: Clear policies on data retention and deletion
- **Third-Party Sharing**: Transparent disclosure of data sharing with RedotPay and other services

### Consumer Protection
- **Dispute Resolution**: Clear process for handling user disputes
- **Refund Policy**: Defined refund policies for failed transactions
- **Fee Transparency**: Clear disclosure of all fees
- **Risk Warnings**: Appropriate warnings for volatile assets and DeFi risks
- **Terms of Service**: Comprehensive ToS covering all features

## Support & Maintenance

### Monitoring
- Transaction success/failure rates (crypto & fiat)
- UserOperation confirmation times
- Bundler availability
- Webhook delivery success (Circle & RedotPay)
- Fiat payout processing times
- Bank transfer success rates
- Exchange rate API response times
- Swap execution success rates
- Staking rewards accrual
- P2P trade completion rates
- Loan liquidation risk levels
- Exchange API health status (WhiteBit, Binance, Kraken)
- Exchange API rate limit usage
- Investment position creation success rates
- Investment withdrawal processing times
- Reward calculation accuracy
- Investment plan sync status and freshness
- Total value locked (TVL) in investments per exchange
- Average APY across all active positions
- Exchange credential validation failures

### Logging
- All API calls to Circle services
- All API calls to RedotPay services
- All API calls to exchange services (WhiteBit, Binance, Kraken)
- UserOperation submissions
- Passkey authentication events
- Bank account additions/verifications
- Exchange connection events (add, verify, disconnect)
- Exchange API credential updates
- Fiat payout requests and status changes
- Exchange rate quotes and locks
- Swap transactions and routes
- Investment plan sync operations
- Investment position creation and updates
- Investment withdrawal requests and completions
- Reward calculation events
- Auto-reinvest operations
- KYC verification attempts
- Error conditions and stack traces
- Security events (failed auth, rate limiting, credential access)

### Alerting
- Failed transactions exceeding threshold
- Bundler API errors
- Webhook delivery failures
- Unusual transaction patterns
- RedotPay API failures
- Failed bank transfers
- Exchange rate API downtime
- Large swap slippage events
- Loan approaching liquidation threshold
- P2P trade disputes
- KYC verification failures exceeding normal rate
- Potential security breaches
- Compliance violations
- Exchange API connection failures
- Exchange API rate limit approaching threshold (>80% usage)
- Investment position creation failures exceeding 5%
- Investment withdrawal delays beyond SLA
- Reward calculation discrepancies
- Investment plan sync failures
- APY changes exceeding 20% for active positions
- Exchange credential validation failures
- Stablecoin depeg events (>2% from peg)
- Total TVL decrease >10% in 24 hours
- Exchange platform health degradation

## Appendix

### Glossary

**Blockchain & Smart Accounts**
- **UserOperation**: ERC-4337 transaction initiated by smart account
- **Bundler**: Service that submits UserOperations to blockchain
- **Paymaster**: Service that sponsors gas fees for transactions
- **Passkey**: WebAuthn credential for authentication
- **Smart Account**: Contract-based account with programmable logic
- **Account Abstraction**: ERC-4337 standard for flexible account management

**Fiat & Payments**
- **Global Payout**: Crypto-to-fiat conversion with bank settlement
- **Off-Ramp**: Process of converting crypto to fiat currency
- **On-Ramp**: Process of converting fiat to crypto currency
- **Settlement**: Transfer of funds to recipient's bank account
- **Exchange Rate**: Conversion rate between crypto and fiat currencies
- **Slippage**: Difference between expected and actual execution price

**DeFi & Trading**
- **Swap**: Exchange of one cryptocurrency for another
- **DEX**: Decentralized Exchange
- **Liquidity Pool**: Smart contract holding token reserves for trading
- **APY/APR**: Annual Percentage Yield/Rate for staking rewards
- **LTV**: Loan-to-Value ratio for collateralized loans
- **Liquidation**: Forced sale of collateral when LTV threshold is breached
- **Escrow**: Third-party holding of funds during P2P trades

**Compliance**
- **KYC**: Know Your Customer - identity verification process
- **AML**: Anti-Money Laundering - fraud prevention measures
- **OFAC**: Office of Foreign Assets Control - US sanctions list

**Exchange Investment**
- **Flex Investment**: Flexible investment plan with no lock period, allowing withdrawals anytime
- **Fixed Investment**: Investment plan with fixed term and predetermined return
- **Locked Investment**: Investment plan with mandatory lock period before withdrawal
- **Auto-Reinvest**: Automatic compounding of earned rewards back into the principal
- **Investment Position**: Active investment held on an exchange earning yield
- **Principal**: Original investment amount deposited
- **Accrued Rewards**: Earned interest/yield that has accumulated but not yet claimed
- **Exchange Adapter**: Software abstraction layer for integrating different exchange APIs
- **Plan Sync**: Process of fetching and updating available investment plans from exchanges
- **Custody Risk**: Risk associated with holding funds on centralized exchange platforms
- **Stablecoin Depeg**: When a stablecoin's value deviates significantly from its target peg (usually $1)

### References

**Circle Documentation**
- [Circle Modular Wallets Documentation](https://developers.circle.com/wallets/modular/create-a-wallet-and-send-gasless-txn)
- [Circle Console](https://console.circle.com/)
- [Circle Smart Account Example](https://github.com/circlefin/modularwallets-web-sdk/tree/master/examples/circle-smart-account)

**RedotPay Documentation**
- [RedotPay Global Payout](https://www.redotpay.com/global-payout)
- [RedotPay Developer Portal](https://www.redotpay.com/) (to be confirmed)
- [RedotPay API Documentation](https://www.redotpay.com/) (to be confirmed)

**Exchange Integration Documentation**
- [WhiteBit API Documentation](https://docs.whitebit.com/)
- [WhiteBit Flex Investment API](https://docs.whitebit.com/private/http-main-v4/#create-flex-investment)
- [WhiteBit B2B Partner Program](https://whitebit.com/b2b)
- [Binance Earn API](https://developers.binance.com/) (future integration)
- [Kraken Staking API](https://docs.kraken.com/rest/) (future integration)
- [Coinbase Earn API](https://developers.coinbase.com/) (future integration)

**Standards & Specifications**
- [ERC-4337 Specification](https://eips.ethereum.org/EIPS/eip-4337)
- [WebAuthn Standard](https://www.w3.org/TR/webauthn/)
- [ERC-20 Token Standard](https://eips.ethereum.org/EIPS/eip-20)

**Compliance & Regulatory**
- [FinCEN - Money Services Business](https://www.fincen.gov/money-services-business-msb-information-center)
- [GDPR Overview](https://gdpr.eu/)
- [OFAC Sanctions Programs](https://home.treasury.gov/policy-issues/office-of-foreign-assets-control-sanctions-programs-and-information)

### Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         CoinPay Platform                        │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │
        ┌─────────────────────┴──────────────────────┐
        │                                            │
        ▼                                            ▼
┌───────────────┐                          ┌─────────────────┐
│  Circle SDK   │                          │  RedotPay API   │
│               │                          │                 │
│ - Passkey Auth│                          │ - Fiat Payout   │
│ - Smart Wallet│                          │ - Bank Transfer │
│ - Gasless Tx  │                          │ - Swap/Exchange │
│ - Multi-Chain │                          │ - Earn/Stake    │
│               │                          │ - P2P/Credit    │
└───────────────┘                          └─────────────────┘
        │                                            │
        │                                            │
        ▼                                            ▼
┌───────────────┐                          ┌─────────────────┐
│  Blockchain   │                          │  Banking System │
│               │                          │                 │
│ - Polygon     │                          │ - Global Banks  │
│ - Arbitrum    │                          │ - Local Payment │
│ - Base        │                          │   Networks      │
│ - Optimism    │                          │ - 100+ Fiat     │
│ - Avalanche   │                          │   Currencies    │
└───────────────┘                          └─────────────────┘
```

### Example Code Reference

See the Circle Smart Account example implementation:
https://github.com/circlefin/modularwallets-web-sdk/tree/master/examples/circle-smart-account

---

**Document Version**: 3.0
**Last Updated**: 2025-10-25
**Author**: Development Team
**Status**: Draft for Review - Enhanced with Exchange Investment Integration
**Changes from v2.0**: Added comprehensive exchange investment integration (WhiteBit Flex Investment as primary implementation) with support for stablecoin yield generation, multi-exchange architecture, investment position management, auto-reinvest functionality, and detailed risk management for centralized exchange custody
**Changes from v1.0**: Added RedotPay global payout, swap, earn, P2P, and credit features
