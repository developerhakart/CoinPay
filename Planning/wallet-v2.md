# Product Requirements Document: CoinPay Wallet V2

## Executive Summary

This V2 PRD defines the evolution of CoinPay from MVP to a full-featured crypto payment platform. Building on the proven MVP foundation (gasless USDC wallet, passkey authentication, and stablecoin yield via WhiteBit), V2 adds critical fiat integration, multi-chain support, and enhanced DeFi services to create a complete crypto-to-fiat payment solution.

## What Changed from MVP

### MVP Delivered (V1.0)
- ✅ Gasless USDC transactions on Polygon Amoy testnet
- ✅ Passkey-based authentication (no seed phrases)
- ✅ Smart account wallets with Circle SDK
- ✅ WhiteBit Flex Investment integration (6-10% APY on USDC)
- ✅ Basic swap functionality (USDC ↔ ETH/MATIC)
- ✅ Transaction history and monitoring

### V2 Additions (This Document)
- 🆕 Fiat off-ramp with RedotPay integration
- 🆕 Bank account management and verification
- 🆕 Multi-currency support (USD, THB, PHP, BRL, EUR, etc.)
- 🆕 Multi-chain expansion (Arbitrum, Base, Optimism mainnet)
- 🆕 Full KYC/AML implementation
- 🆕 Enhanced swap with DEX aggregation
- 🆕 Multi-exchange investment support (Binance, Kraken)
- 🆕 On-chain staking services
- 🆕 Mobile app support (iOS, Android)

## Product Overview

### V2 Objective
Transform CoinPay into a complete crypto payment platform that bridges crypto and fiat seamlessly:

**Enhanced Wallet Features**
- Multi-chain support across all major EVM networks
- Mobile app availability (iOS/Android)
- Hardware wallet integration
- Batch transaction support

**Fiat Integration (NEW)**
- Crypto-to-fiat conversion with RedotPay
- Direct bank account payouts globally
- Multi-currency support (100+ fiat currencies)
- 24/7 instant to 24-hour settlement
- KYC/AML compliant operations

**Enhanced Monetization**
- Advanced swap with DEX aggregation
- Multiple exchange integrations (WhiteBit, Binance, Kraken)
- On-chain staking across protocols
- Referral program
- Premium features subscription

**New Services**
- P2P marketplace for peer-to-peer trading
- Credit services with collateralized lending
- Recurring payment automation
- Invoice generation and management

### Value Proposition
- **Simplicity**: No gas fees, no seed phrases, instant fiat withdrawals
- **Security**: Passkey auth, regulated KYC, insured custody
- **Earnings**: 6-15% APY across multiple platforms
- **Access**: Convert crypto to local currency in 100+ countries
- **Flexibility**: Multi-chain, multi-currency, multi-device

## Technical Architecture

### Enhanced Core Components

#### 1. Multi-Platform SDK Integration
- **Package**: `@circle-fin/modular-wallets-core`
- **Platforms**:
  - Web (production ready from MVP)
  - iOS (native app - NEW)
  - Android (native app - NEW)
- **Version**: 1.x.x (latest stable)

#### 2. Authentication System
- **Primary Method**: WebAuthn Passkeys
- **Backup Methods** (NEW):
  - Email + 2FA recovery
  - Social login backup (Google, Apple)
  - Multi-device passkey sync
- **Session Management**: JWT with refresh tokens

#### 3. Smart Account Architecture
- **Account Type**: Circle Smart Account (ERC-4337 compatible)
- **Owner**: Passkey credential (WebAuthn)
- **Multi-sig Support** (NEW): Optional 2-of-3 for high-value accounts
- **Bundler**: Circle's bundler service
- **Paymaster**: Circle's paymaster for gas sponsorship

#### 4. Blockchain Support (EXPANDED)

**Mainnet (Production)**
- Polygon PoS (primary for MVP compatibility)
- Arbitrum One
- Base
- Optimism
- Avalanche C-Chain
- Unichain (when available)

**Testnet (Development)**
- Polygon Amoy (MVP testnet)
- Arbitrum Sepolia
- Base Sepolia
- Optimism Sepolia
- Avalanche Fuji
- Unichain Sepolia

#### 5. Fiat Gateway Integration (NEW)

**Service Provider**: RedotPay Global Payout

**Capabilities**:
- Crypto-to-fiat conversion engine
- Real-time exchange rate feeds
- Bank account settlement worldwide
- Local payment network integration

**Supported Crypto Assets**:
- USDC (primary)
- USDT
- BTC
- ETH
- Other major cryptocurrencies

**Supported Fiat Currencies** (100+ including):
- **North America**: USD, CAD, MXN
- **Europe**: EUR, GBP, CHF, PLN
- **Asia**: THB, PHP, IDR, VND, MYR, SGD, JPY, KRW, CNY
- **Latin America**: BRL, ARS, CLP, COP
- **Africa**: ZAR, NGN, KES, EGP
- **Middle East**: AED, SAR

**Settlement Methods**:
- ACH (US)
- SEPA (Europe)
- FPS (UK)
- Local bank transfers
- Mobile money (selected markets)
- Cash pickup (selected markets)

**Processing Time**:
- Instant (selected corridors)
- 1-4 hours (major markets)
- 24 hours (standard)
- 48-72 hours (emerging markets)

#### 6. Enhanced Exchange Investment Service

**Supported Exchanges**:
- WhiteBit (MVP - production ready)
- Binance Earn (NEW)
- Kraken Staking (NEW)
- Coinbase Earn (NEW - planned)
- OKX Earn (NEW - planned)

**Investment Types**:
- Flexible earning (Flex plans)
- Fixed-term deposits
- Locked staking
- DeFi yield aggregation

**Expected Returns**:
- USDC/USDT Flex: 6-10% APY
- Fixed deposits: 8-12% APY
- Locked staking: 10-15% APY

#### 7. On-Chain Staking Integration (NEW)

**Supported Protocols**:
- Lido (Ethereum, Polygon staking)
- Rocket Pool (Ethereum)
- Liquid staking derivatives
- Native chain staking

**Features**:
- Liquid staking tokens (stETH, rETH, etc.)
- Automatic reward claiming
- Compound rewards
- No minimum stake amounts

## Functional Requirements

### FR-1: Multi-Chain Wallet Management (ENHANCED)

#### FR-1.1: Chain Switching
- User can view balances across all supported chains
- Seamless chain switching in UI
- Unified transaction history across chains
- Single smart account address across compatible chains

#### FR-1.2: Cross-Chain Bridging
- Integrated bridge UI for asset transfers
- Bridge through Circle CCTP (Cross-Chain Transfer Protocol)
- Estimate bridge time and fees
- Track bridge transaction status

#### FR-1.3: Multi-Chain Portfolio View
- Aggregate balance across all chains
- Chain-specific asset breakdown
- Real-time USD value conversion
- Portfolio allocation visualization

### FR-2: Mobile Application (NEW)

#### FR-2.1: Native Mobile Apps
- iOS app (Swift/SwiftUI)
- Android app (Kotlin/Jetpack Compose)
- Biometric authentication (Face ID, Touch ID, fingerprint)
- Push notifications for transactions
- QR code scanning for addresses

#### FR-2.2: Mobile-Specific Features
- Camera permissions for QR scanning
- Biometric-protected passkey storage
- Background balance refresh
- Transaction notifications
- Deep linking support

#### FR-2.3: App Store Compliance
- iOS App Store guidelines compliance
- Google Play Store guidelines compliance
- Privacy policy integration
- App review preparation

### FR-3: KYC/AML Implementation (NEW)

#### FR-3.1: Identity Verification
- **KYC Provider**: Sumsub or Onfido
- **Verification Levels**:
  - Level 0: No KYC (crypto-only, $500/day limit)
  - Level 1: Basic KYC (name, DOB, address - $5,000/day limit)
  - Level 2: Enhanced KYC (ID document, selfie - $25,000/day limit)
  - Level 3: Full KYC (proof of address, source of funds - $100,000/day limit)

#### FR-3.2: Document Collection
- Government-issued ID (passport, driver's license, national ID)
- Proof of address (utility bill, bank statement)
- Selfie with liveness detection
- Source of funds declaration (for high-value accounts)

#### FR-3.3: AML Screening
- Real-time sanctions screening (OFAC, UN, EU lists)
- PEP (Politically Exposed Person) checks
- Adverse media screening
- Ongoing monitoring for high-risk users

#### FR-3.4: Transaction Monitoring
- Suspicious activity detection
- Unusual pattern alerts
- Velocity checks (rapid deposits/withdrawals)
- Large transaction reporting
- SAR (Suspicious Activity Report) filing capability

### FR-4: Bank Account Management (NEW)

#### FR-4.1: Add Bank Account
- User enters bank details:
  - Account holder name
  - Country and currency
  - Account number / IBAN
  - Routing number / SWIFT code
  - Bank name and branch
- System validates format based on country
- Store encrypted bank details in vault
- Support multiple bank accounts per user (up to 5)

#### FR-4.2: Bank Account Verification
- **Micro-deposit verification** (US, EU):
  - Send two small deposits ($0.01-$0.99)
  - User confirms amounts to verify ownership
- **Instant verification** (selected markets):
  - Plaid integration (US)
  - TrueLayer (UK/EU)
  - Direct bank API integration
- Manual verification for unsupported markets

#### FR-4.3: Bank Account Management
- View all linked bank accounts
- Set primary account for default payouts
- Edit account details
- Remove bank account
- View verification status
- Re-verify if needed

### FR-5: Fiat Payout Execution (NEW)

#### FR-5.1: Crypto-to-Fiat Conversion Flow
1. User selects crypto asset to convert (USDC, USDT, BTC, ETH)
2. User enters amount to cash out
3. System displays:
   - Real-time exchange rate (locked for 30 seconds)
   - Destination fiat currency options
   - Estimated fiat amount
   - Conversion fee (1-2%)
   - Network fee (if applicable)
   - Processing fee ($0-5 depending on method)
   - Total amount to receive
   - Estimated arrival time
4. User selects destination bank account
5. User confirms transaction
6. Passkey authentication required
7. System submits to RedotPay gateway
8. Crypto deducted from wallet
9. Payout status tracking begins

#### FR-5.2: Payout Status Tracking
- **Pending**: Submitted to RedotPay
- **Processing**: Crypto converted, awaiting bank settlement
- **Sent**: Funds sent to bank
- **Completed**: Funds arrived in bank account
- **Failed**: Error occurred, funds refunded

#### FR-5.3: Payout Notifications
- Email notification on submission
- Push notification when processing starts
- Push/email when funds arrive
- SMS notification (optional, for high-value)

#### FR-5.4: Payout History
- Display all fiat payout transactions
- Filter by: status, currency, date range, amount
- Export to CSV for tax purposes
- View exchange rates used historically
- Track fees paid per transaction

#### FR-5.5: Payout Limits
- Daily limits based on KYC level:
  - Level 0: No fiat payouts
  - Level 1: $5,000/day
  - Level 2: $25,000/day
  - Level 3: $100,000/day
- Monthly limits (3x daily limit)
- Lifetime limits for new accounts (gradual increase)

### FR-6: Enhanced Swap Functionality (UPGRADED)

#### FR-6.1: Advanced Swap Interface
- Support all major tokens on each chain
- DEX aggregation for best rates:
  - 1inch API integration
  - 0x Protocol integration
  - Native DEX integration (Uniswap, PancakeSwap, etc.)
- Real-time price comparison across sources
- Price impact warnings
- Slippage tolerance settings (0.1%, 0.5%, 1%, custom)
- MEV protection options

#### FR-6.2: Swap Route Optimization
- Multi-hop routing for best execution
- Split orders across multiple pools
- Visual route display showing path
- Estimated gas savings display
- Time-to-execution estimates

#### FR-6.3: Advanced Features
- Limit orders (set price, execute later)
- Dollar-cost averaging (DCA) automation
- Token approval management
- Price alerts and notifications
- Swap history with analytics

### FR-7: Multi-Exchange Investment (EXPANDED)

#### FR-7.1: Exchange Integration Architecture
- **WhiteBit** (production from MVP)
  - Flex Investment API
  - Real-time position tracking
  - Auto-reinvest support

- **Binance** (NEW)
  - Binance Earn API
  - Flexible Savings
  - Locked Savings
  - DeFi Staking

- **Kraken** (NEW)
  - Kraken Staking API
  - On-chain staking
  - Parachain auctions

- **Future**: Coinbase, OKX, Bybit

#### FR-7.2: Unified Investment Dashboard
- Aggregate view across all exchanges
- Total portfolio value (all investments)
- Combined APY calculation
- Performance comparison by exchange
- Risk-adjusted return metrics

#### FR-7.3: Investment Plan Comparison
- Side-by-side comparison table
- Filter by: asset, APY range, lock period, exchange
- Sort by: highest APY, best risk-adjusted return
- Highlight best options with badges
- Show insurance/protection details

#### FR-7.4: Auto-Rebalancing (NEW)
- Set target allocation across exchanges
- Automatic rebalancing when drift exceeds threshold
- Minimize transfer fees during rebalancing
- Rebalancing history and performance

#### FR-7.5: Risk Management Dashboard
- Exchange health monitoring
- Insurance coverage display
- Custody risk indicators
- Diversification score
- Position size recommendations

### FR-8: On-Chain Staking (NEW)

#### FR-8.1: Liquid Staking Integration
- **Lido Finance Integration**:
  - Stake ETH, get stETH (liquid staking token)
  - Stake MATIC, get stMATIC
  - Use stTokens in DeFi while earning staking rewards

- **Rocket Pool Integration**:
  - Stake ETH, get rETH
  - Decentralized node operator network

- **Other Protocols**:
  - Frax (frxETH)
  - Ankr (ankrETH)

#### FR-8.2: Staking Interface
- Display available staking options per chain
- Show current APR for each protocol
- Calculate estimated rewards (daily/monthly/yearly)
- Display unstaking period and penalties
- One-click staking with gasless transaction

#### FR-8.3: Staking Position Management
- View all active staking positions
- Track accrued rewards in real-time
- Compound rewards automatically
- Unstake with transparent period display
- Use liquid tokens in other DeFi protocols

### FR-9: P2P Marketplace (NEW)

#### FR-9.1: Create P2P Offer
- User selects offer type (buy or sell crypto)
- User enters:
  - Crypto asset and amount
  - Fiat currency and price
  - Payment method (bank transfer, cash, mobile money)
  - Trade limits (min/max per transaction)
  - Trade terms and conditions
- System calculates exchange rate vs. market
- Offer published to marketplace

#### FR-9.2: P2P Trade Execution
1. Buyer/Seller browses offers
2. Initiates trade on selected offer
3. Crypto locked in escrow smart contract
4. Buyer transfers fiat via agreed payment method
5. Buyer marks payment as sent
6. Seller confirms fiat receipt
7. Escrow releases crypto to buyer
8. Trade completed, both parties can rate

#### FR-9.3: Escrow Management
- Smart contract escrow for security
- Dispute resolution mechanism
- Mediator assignment for disputes
- Escrow timeout protection
- Automatic release after confirmation

#### FR-9.4: Reputation System
- Rating system (1-5 stars)
- Review comments
- Trade completion rate
- Average response time
- Verified trader badges
- Trust score calculation

### FR-10: Credit Services (NEW)

#### FR-10.1: Collateralized Lending
- User deposits crypto as collateral
- System calculates maximum loan amount (LTV ratio)
- Supported collateral: BTC, ETH, USDC, USDT
- Loan currencies: USDC, USDT
- LTV ratios: 50-75% depending on collateral volatility

#### FR-10.2: Loan Management
- Display available loan amount based on collateral
- Show interest rates (8-15% APR depending on LTV)
- Display liquidation threshold (80-90% LTV)
- Real-time LTV monitoring
- Add collateral to reduce LTV
- Partial or full repayment options

#### FR-10.3: Liquidation Protection
- Real-time LTV alerts (email, push, SMS)
- Warning at 75% LTV
- Urgent warning at 85% LTV
- Auto-collateral top-up (opt-in feature)
- Liquidation at 90% LTV
- Partial liquidation to restore safe LTV

#### FR-10.4: Loan History
- View all loans (active, repaid, liquidated)
- Interest accrual tracking
- Repayment history
- Liquidation events log
- Tax reporting export

### FR-11: Additional Features (NEW)

#### FR-11.1: Recurring Payments
- Set up recurring crypto transfers
- Daily, weekly, monthly schedules
- Auto-execute with balance checks
- Pause/resume/cancel anytime
- Notification before execution

#### FR-11.2: Invoice Generation
- Create crypto payment invoices
- QR code generation
- Payment link sharing
- Track invoice status (pending, paid, expired)
- Multi-currency invoice support

#### FR-11.3: Referral Program
- Unique referral code per user
- Referral link generation
- Reward structure:
  - Referrer: 20% of fees from referred user (first 6 months)
  - Referee: 50% discount on first $100 in fees
- Referral dashboard showing earnings
- Withdrawal of referral rewards

#### FR-11.4: Premium Subscription (Optional)
- **Free Tier**: Standard features, standard fees
- **Premium Tier** ($9.99/month):
  - 50% discount on swap fees
  - Priority customer support
  - Advanced analytics
  - Higher daily limits (+50%)
  - Early access to new features
  - No fiat payout fees

## Non-Functional Requirements

### NFR-1: Security (ENHANCED)
- End-to-end encryption for all sensitive data
- Bank details encrypted at rest (AES-256)
- Exchange API credentials in hardware security module (HSM)
- SOC 2 Type II compliance
- Regular penetration testing (quarterly)
- Bug bounty program
- Rate limiting on all APIs
- DDoS protection (Cloudflare)

### NFR-2: Performance (ENHANCED)
- API response time < 2 seconds (p95)
- Transaction submission < 3 seconds
- UserOperation confirmation < 45 seconds
- Fiat payout submission < 5 seconds
- Support 10,000+ concurrent users
- Database query optimization (< 100ms)
- CDN for static assets (< 500ms load time)

### NFR-3: Reliability (ENHANCED)
- 99.95% uptime SLA
- Zero-downtime deployments
- Database replication (master-slave)
- Automated backup every 6 hours
- Disaster recovery plan (RPO: 1 hour, RTO: 4 hours)
- Failover mechanisms for critical services

### NFR-4: Scalability
- Horizontal scaling for all services
- Kubernetes orchestration
- Auto-scaling based on load
- Queue-based architecture for async operations
- Microservices architecture
- Database sharding for user data

### NFR-5: Compliance
- GDPR compliance (EU users)
- CCPA compliance (California users)
- KYC/AML regulatory compliance
- Money transmitter licenses (US states)
- Payment service provider licenses (EU)
- Regular compliance audits
- Privacy policy updates
- Terms of service updates

### NFR-6: Usability
- Mobile-first responsive design
- Accessibility (WCAG 2.1 AA compliance)
- Multi-language support:
  - English (primary)
  - Spanish
  - Portuguese
  - Thai
  - Filipino
  - Mandarin Chinese
  - French
- Intuitive onboarding flow
- In-app help and tooltips
- 24/7 customer support (chat, email)

## Data Models (V2 Updates)

### User Account (ENHANCED)
```typescript
interface UserAccount {
  id: string;
  email: string;
  username: string;
  kycLevel: 0 | 1 | 2 | 3;
  kycStatus: 'not_started' | 'pending' | 'approved' | 'rejected';
  kycProvider: 'sumsub' | 'onfido' | null;
  kycVerifiedAt?: Date;
  isPremium: boolean;
  premiumExpiresAt?: Date;
  referralCode: string;
  referredBy?: string;
  totalReferrals: number;
  createdAt: Date;
  lastLoginAt: Date;
  status: 'active' | 'suspended' | 'banned';
}
```

### Wallet Account (ENHANCED)
```typescript
interface WalletAccount {
  id: string;
  userId: string;
  address: string;
  ownerCredentialId: string;
  supportedChains: number[];          // All chain IDs
  isMultiSig: boolean;
  multiSigConfig?: {
    threshold: number;
    owners: string[];
  };
  createdAt: Date;
  lastActivityAt: Date;
}
```

### Chain Balance
```typescript
interface ChainBalance {
  id: string;
  walletId: string;
  chainId: number;
  chainName: string;
  nativeBalance: bigint;              // ETH, MATIC, etc.
  tokenBalances: {
    tokenAddress: string;
    symbol: string;
    balance: bigint;
    decimals: number;
    usdValue: number;
  }[];
  totalUsdValue: number;
  lastUpdated: Date;
}
```

### Bank Account (NEW)
```typescript
interface BankAccount {
  id: string;
  userId: string;
  accountHolderName: string;
  country: string;                    // ISO 3166-1 alpha-2
  currency: string;                   // ISO 4217
  accountNumber: string;              // Encrypted
  routingNumber?: string;             // US ACH
  iban?: string;                      // Europe
  swiftCode?: string;                 // International
  bankName: string;
  bankBranch?: string;
  bankAddress?: string;
  accountType: 'checking' | 'savings';
  verificationMethod: 'micro_deposit' | 'instant' | 'manual';
  verificationStatus: 'pending' | 'verified' | 'failed';
  verificationAttempts: number;
  isPrimary: boolean;
  isActive: boolean;
  createdAt: Date;
  verifiedAt?: Date;
  lastUsedAt?: Date;
}
```

### Fiat Payout Transaction (NEW)
```typescript
interface FiatPayoutTransaction {
  id: string;
  userId: string;
  walletAddress: string;
  bankAccountId: string;
  cryptoAsset: string;                // 'USDC', 'USDT', 'BTC', 'ETH'
  cryptoAmount: bigint;
  cryptoChainId: number;
  fiatCurrency: string;               // 'USD', 'THB', 'PHP', etc.
  fiatAmount: number;
  exchangeRate: number;
  fees: {
    conversionFee: number;
    conversionFeePercentage: number;
    networkFee: number;
    processingFee: number;
    totalFee: number;
  };
  status: 'pending' | 'processing' | 'sent' | 'completed' | 'failed' | 'refunded';
  provider: 'redotpay';
  providerTransactionId?: string;
  providerResponse?: any;
  failureReason?: string;
  failureCode?: string;
  estimatedArrival?: Date;
  kycCheckPassed: boolean;
  amlCheckPassed: boolean;
  riskScore?: number;
  createdAt: Date;
  processedAt?: Date;
  completedAt?: Date;
  refundedAt?: Date;
}
```

### KYC Verification (NEW)
```typescript
interface KYCVerification {
  id: string;
  userId: string;
  provider: 'sumsub' | 'onfido';
  providerVerificationId: string;
  level: 1 | 2 | 3;
  status: 'pending' | 'approved' | 'rejected' | 'review';
  documents: {
    type: 'passport' | 'drivers_license' | 'national_id' | 'proof_of_address' | 'selfie';
    documentId: string;
    status: 'pending' | 'approved' | 'rejected';
    rejectionReason?: string;
  }[];
  personalInfo: {
    firstName: string;
    lastName: string;
    dateOfBirth: Date;
    nationality: string;
    address: {
      street: string;
      city: string;
      state?: string;
      postalCode: string;
      country: string;
    };
  };
  amlChecks: {
    sanctionsScreening: 'pass' | 'fail' | 'pending';
    pepCheck: 'pass' | 'fail' | 'pending';
    adverseMediaCheck: 'pass' | 'fail' | 'pending';
  };
  riskLevel: 'low' | 'medium' | 'high';
  reviewNotes?: string;
  submittedAt: Date;
  reviewedAt?: Date;
  approvedAt?: Date;
  rejectedAt?: Date;
  expiresAt?: Date;
}
```

### Staking Position (NEW)
```typescript
interface StakingPosition {
  id: string;
  userId: string;
  walletAddress: string;
  protocol: 'lido' | 'rocket_pool' | 'frax' | 'ankr';
  chainId: number;
  stakedToken: string;                // 'ETH', 'MATIC'
  stakedAmount: bigint;
  liquidToken?: string;               // 'stETH', 'rETH', 'stMATIC'
  liquidTokenAmount?: bigint;
  currentAPR: number;
  rewardsEarned: bigint;
  rewardsClaimed: bigint;
  autoCompound: boolean;
  unstakePeriod: number;              // Days
  status: 'active' | 'unstaking' | 'completed';
  stakingTransactionHash: string;
  unstakingTransactionHash?: string;
  createdAt: Date;
  unstakeRequestedAt?: Date;
  unstakeCompletedAt?: Date;
  lastRewardUpdate: Date;
}
```

### P2P Trade (NEW)
```typescript
interface P2PTrade {
  id: string;
  offerId: string;
  buyerId: string;
  sellerId: string;
  cryptoAsset: string;
  cryptoAmount: bigint;
  fiatCurrency: string;
  fiatAmount: number;
  exchangeRate: number;
  paymentMethod: string;
  tradeType: 'buy' | 'sell';
  status: 'initiated' | 'crypto_locked' | 'payment_sent' | 'payment_confirmed' | 'completed' | 'disputed' | 'cancelled' | 'refunded';
  escrowAddress: string;
  escrowTransactionHash?: string;
  releaseTransactionHash?: string;
  chatEnabled: boolean;
  buyerRating?: number;
  sellerRating?: number;
  buyerReview?: string;
  sellerReview?: string;
  disputeReason?: string;
  mediatorId?: string;
  mediatorDecision?: string;
  createdAt: Date;
  paymentSentAt?: Date;
  paymentConfirmedAt?: Date;
  completedAt?: Date;
  disputedAt?: Date;
  resolvedAt?: Date;
  expiresAt: Date;
}
```

### Credit Loan (NEW)
```typescript
interface CreditLoan {
  id: string;
  userId: string;
  walletAddress: string;
  collateralToken: string;
  collateralAmount: bigint;
  collateralUsdValue: number;
  loanToken: string;                  // 'USDC', 'USDT'
  loanAmount: bigint;
  loanUsdValue: number;
  interestRate: number;               // APR percentage
  initialLTV: number;
  currentLTV: number;
  liquidationThreshold: number;       // Percentage (80-90%)
  liquidationPrice: number;           // Collateral price that triggers liquidation
  accruedInterest: bigint;
  repaidAmount: bigint;
  outstandingAmount: bigint;
  status: 'active' | 'repaid' | 'liquidated' | 'defaulted';
  autoTopUp: boolean;
  alertLTV: number;                   // Alert at this LTV (75%)
  collateralTransactionHash: string;
  loanTransactionHash: string;
  liquidationTransactionHash?: string;
  createdAt: Date;
  dueDate: Date;
  lastInterestAccrual: Date;
  repaidAt?: Date;
  liquidatedAt?: Date;
}
```

### Referral (NEW)
```typescript
interface Referral {
  id: string;
  referrerId: string;
  refereeId: string;
  referralCode: string;
  status: 'pending' | 'active' | 'completed' | 'cancelled';
  totalFeesGenerated: number;
  totalRewardsPaid: number;
  rewardPercentage: number;           // 20% default
  rewardDuration: number;             // Days (180 default)
  rewardExpiresAt: Date;
  createdAt: Date;
  firstTransactionAt?: Date;
  completedAt?: Date;
}
```

### Premium Subscription (NEW)
```typescript
interface PremiumSubscription {
  id: string;
  userId: string;
  plan: 'premium';
  status: 'active' | 'cancelled' | 'expired' | 'past_due';
  billingCycle: 'monthly' | 'yearly';
  amount: number;
  currency: string;
  paymentMethod: 'crypto' | 'card';
  startDate: Date;
  currentPeriodStart: Date;
  currentPeriodEnd: Date;
  cancelledAt?: Date;
  cancelAtPeriodEnd: boolean;
  lastPaymentAt?: Date;
  nextPaymentAt?: Date;
}
```

## User Flows (V2)

### Flow 1: Complete KYC Verification
1. User navigates to "Verify Account"
2. System displays KYC levels and benefits
3. User selects desired KYC level (1, 2, or 3)
4. System redirects to Sumsub KYC flow
5. User uploads ID document (passport/license)
6. User takes selfie with liveness check
7. User provides address proof (if Level 2+)
8. User submits for review
9. System performs AML checks (sanctions, PEP)
10. System auto-approves or sends to manual review
11. User receives notification of approval/rejection
12. KYC level updated, limits increased
13. User can now access fiat features

### Flow 2: Add and Verify Bank Account
1. User navigates to "Bank Accounts"
2. User clicks "Add Bank Account"
3. User selects country
4. System displays required fields for that country
5. User enters bank details:
   - Account holder name
   - Account number / IBAN
   - Routing number / SWIFT
   - Bank name
6. System validates format
7. System encrypts and stores bank details
8. Verification method selected:
   - **Micro-deposit** (US/EU):
     - System initiates two small deposits
     - User waits 1-3 business days
     - User enters deposit amounts
     - System verifies and activates account
   - **Instant verification** (US with Plaid):
     - User redirects to Plaid
     - User logs into bank
     - Plaid confirms account ownership
     - Account verified immediately
9. Bank account marked as "Verified"
10. User can set as primary account

### Flow 3: Cash Out Crypto to Bank (Full Flow)
**Prerequisites**: KYC Level 1+, Verified bank account

1. User navigates to "Cash Out" section
2. User selects crypto asset (USDC, USDT, BTC, ETH)
3. User enters amount to cash out
4. System fetches real-time exchange rate from RedotPay
5. User selects destination fiat currency (auto-detected or manual)
6. System displays conversion preview:
   - Crypto amount: 1000 USDC
   - Exchange rate: 1 USDC = 0.98 USD (2% spread)
   - Fiat amount: $980.00
   - Conversion fee: $14.70 (1.5%)
   - Processing fee: $2.00
   - Network fee: $1.00
   - **Total to receive: $962.30**
   - Estimated arrival: 2-24 hours
7. User selects destination bank account (or adds new)
8. User reviews and confirms
9. System performs AML check:
   - Sanctions screening
   - Velocity check
   - Risk scoring
10. If high-risk, manual review triggered
11. If approved, passkey authentication requested
12. User confirms with biometric
13. System executes payout:
    - Deducts crypto from wallet
    - Submits to RedotPay gateway
    - RedotPay converts crypto to fiat
    - RedotPay initiates bank transfer
14. User sees status: "Processing"
15. Email/push notification sent
16. System polls RedotPay for status updates
17. Status updates to "Sent" when bank transfer initiated
18. Final notification when funds arrive: "Completed"
19. Transaction appears in payout history

### Flow 4: Multi-Chain Swap
1. User navigates to "Swap" section
2. User selects source chain (e.g., Polygon)
3. User selects source token (e.g., USDC)
4. User enters amount to swap (e.g., 100 USDC)
5. User selects destination chain (e.g., Arbitrum)
6. User selects destination token (e.g., ETH)
7. System queries multiple DEX aggregators:
   - 1inch API
   - 0x Protocol
   - Native DEXs
8. System displays best route:
   - Route: USDC → WETH (Uniswap) → ETH
   - Output: 0.0285 ETH
   - Price: 1 ETH = 3,508 USDC
   - Price impact: 0.15%
   - Network fees: $1.20 (gasless for user)
   - Swap fee: 0.5% = $0.50
   - **Total to receive: 0.0285 ETH on Arbitrum**
9. User sets slippage tolerance (default 0.5%)
10. User clicks "Swap"
11. Passkey authentication
12. User confirms with biometric
13. System executes swap:
    - If same chain: Single transaction
    - If cross-chain: Swap + bridge via Circle CCTP
14. Progress indicator shows stages
15. Transaction confirmed
16. Balances updated on both chains
17. Success notification

### Flow 5: Stake on Lido (Liquid Staking)
1. User navigates to "Earn" → "On-Chain Staking"
2. System displays available staking options:
   - Lido (ETH): 3.8% APR → stETH
   - Rocket Pool (ETH): 3.6% APR → rETH
   - Lido (MATIC): 4.2% APR → stMATIC
3. User selects "Lido (ETH)"
4. System displays details:
   - Current APR: 3.8%
   - Receive: stETH (1:1 with ETH)
   - Unstaking period: None (liquid)
   - Rewards: Auto-compounding
   - Use stETH in DeFi: Yes
5. User enters amount to stake (e.g., 0.5 ETH)
6. System calculates:
   - Staked: 0.5 ETH
   - Receive: ~0.5 stETH
   - Estimated daily reward: 0.000052 ETH (~$0.18)
   - Estimated yearly reward: 0.019 ETH (~$66)
7. User confirms stake
8. Passkey authentication
9. User confirms with biometric
10. System executes stake transaction (gasless)
11. ETH deposited to Lido contract
12. stETH minted to user wallet
13. Staking position created as "Active"
14. Dashboard shows:
    - Staked amount: 0.5 ETH
    - Current value: 0.5002 stETH (rewards accruing)
    - Liquid token balance: 0.5002 stETH
    - APR: 3.8%
15. User can use stETH in other DeFi protocols while earning staking rewards

### Flow 6: Create Multi-Exchange Investment Portfolio
1. User navigates to "Invest" section
2. User clicks "Compare Plans"
3. System displays all available plans:

   | Exchange | Asset | APY   | Type  | Min   | Lock | Auto-Reinvest |
   |----------|-------|-------|-------|-------|------|---------------|
   | Binance  | USDC  | 10.5% | Flex  | $100  | None | Yes           |
   | WhiteBit | USDC  | 8.5%  | Flex  | $50   | None | Yes           |
   | Kraken   | USDC  | 7.2%  | Flex  | $10   | None | No            |
   | Binance  | USDT  | 12.0% | Fixed | $500  | 30d  | No            |

4. User decides to diversify across platforms
5. User creates allocation strategy:
   - 40% Binance USDC Flex (highest APY)
   - 40% WhiteBit USDC Flex (diversification)
   - 20% Kraken USDC Flex (most flexible)
6. User has 1000 USDC total, allocates:
   - 400 USDC → Binance (10.5% APY)
   - 400 USDC → WhiteBit (8.5% APY)
   - 200 USDC → Kraken (7.2% APY)
7. System creates three investment positions simultaneously
8. Passkey authentication (single approval for all)
9. System transfers:
   - 400 USDC to Binance via API
   - 400 USDC to WhiteBit via API (existing integration from MVP)
   - 200 USDC to Kraken via API
10. All positions activate
11. Portfolio dashboard shows:
    - Total invested: 1000 USDC
    - Weighted average APY: 9.12%
    - Estimated yearly earnings: $91.20
    - Estimated monthly earnings: $7.60
    - Diversification score: High (3 exchanges)
12. Daily rewards start accruing across all platforms

### Flow 7: P2P Trade (Sell Crypto for Cash)
1. User navigates to "P2P Marketplace"
2. User clicks "Create Offer"
3. User selects "Sell USDC"
4. User enters offer details:
   - Amount: 500 USDC
   - Fiat: USD
   - Price: $0.99 per USDC (1% below market)
   - Payment method: Bank transfer
   - Min trade: $50
   - Max trade: $500
   - Trade terms: "Payment within 24 hours"
5. User publishes offer
6. Offer appears in marketplace
7. Buyer finds offer, initiates trade for $300 worth (303 USDC)
8. System locks 303 USDC in escrow smart contract
9. Escrow transaction confirmed
10. Buyer transfers $300 to seller's bank (off-platform)
11. Buyer marks payment as "Sent" with transaction reference
12. Seller receives notification to check bank account
13. Seller confirms receiving $300
14. Seller marks payment as "Confirmed"
15. System releases 303 USDC from escrow to buyer
16. Trade completed
17. Both parties rate each other (5 stars)
18. Seller's remaining offer: 197 USDC (500 - 303)

### Flow 8: Take Collateralized Loan
1. User navigates to "Credit" section
2. User views collateral options and rates:
   - ETH collateral: 60% LTV, 10% APR
   - BTC collateral: 70% LTV, 9% APR
   - USDC collateral: 80% LTV, 8% APR
3. User selects "ETH as collateral"
4. User has 1 ETH ($3,500 value)
5. System calculates:
   - Collateral value: $3,500
   - Max LTV: 60%
   - Max loan amount: $2,100 USDC
   - Liquidation threshold: 90% LTV
   - Liquidation price: ETH < $2,333
6. User enters desired loan: $2,000 USDC
7. System displays loan terms:
   - Collateral: 1 ETH ($3,500)
   - Loan: $2,000 USDC
   - Initial LTV: 57.14%
   - Interest rate: 10% APR
   - Monthly interest: $16.67
   - Liquidation threshold: 90% LTV
   - Liquidation price: ETH < $2,222
   - Loan duration: Indefinite (pay anytime)
8. User enables "Auto top-up" for liquidation protection
9. User confirms loan
10. Passkey authentication
11. System executes:
    - 1 ETH locked in lending smart contract
    - 2,000 USDC transferred to user wallet
12. Loan position created as "Active"
13. Dashboard shows:
    - Collateral: 1 ETH ($3,500)
    - Loan: $2,000 USDC
    - Current LTV: 57.14%
    - Liquidation LTV: 90%
    - Health factor: 1.58 (Healthy)
    - Accrued interest: $0.55 (daily)
14. Daily LTV monitoring begins
15. If ETH price drops to $2,500:
    - LTV increases to 80%
    - User receives "Warning" notification
16. If ETH drops to $2,300:
    - LTV increases to 87%
    - User receives "Urgent" notification
    - Auto top-up triggers if enabled (adds more ETH)
17. User repays loan anytime:
    - Repay $2,000 USDC + accrued interest
    - Collateral (1 ETH) released back to wallet

### Flow 9: Mobile App Transaction
1. User opens CoinPay mobile app (iOS/Android)
2. Face ID / Touch ID authentication
3. App loads wallet dashboard:
   - Total balance across all chains
   - Recent transactions
   - Quick actions (Send, Receive, Swap, Earn)
4. User taps "Send"
5. User taps QR code scanner icon
6. Camera activates
7. User scans recipient's QR code
8. Address auto-filled
9. User selects token (USDC)
10. User enters amount
11. User confirms
12. Biometric authentication (Face ID)
13. Transaction submitted
14. Push notification: "Transaction pending"
15. 30 seconds later: "Transaction confirmed"
16. Balance updated in real-time

### Flow 10: Recurring Investment (Auto-DCA)
1. User navigates to "Invest" → "Recurring"
2. User clicks "Create Recurring Investment"
3. User selects:
   - Investment plan: Binance USDC Flex (10.5% APY)
   - Amount: 100 USDC
   - Frequency: Weekly (every Monday)
   - Duration: 52 weeks (1 year)
4. System calculates:
   - Total investment: 5,200 USDC (100 × 52)
   - Estimated yearly earnings: $546 (assuming 10.5% APY)
   - Estimated total value after 1 year: $5,746
5. User enables "Auto-reinvest rewards"
6. User confirms recurring plan
7. System schedules weekly job
8. Every Monday at 9 AM:
   - Check wallet balance
   - If balance ≥ 100 USDC:
     - Transfer 100 USDC to Binance
     - Create investment position
     - Send notification
   - If balance < 100 USDC:
     - Skip this week
     - Send notification: "Insufficient balance"
9. After 52 weeks, recurring plan completes
10. Total invested: 5,200 USDC (if all weeks successful)
11. Compounded rewards earned: ~$546
12. Final value: ~$5,746

## Implementation Phases (V2)

### Phase 1: Foundation & Planning (Week 1-2)
- [ ] V2 technical architecture design
- [ ] Database schema updates for new models
- [ ] API design for new features
- [ ] Third-party service evaluation (Sumsub, RedotPay contracts)
- [ ] Legal and compliance consultation
- [ ] License application preparation
- [ ] Security audit planning

### Phase 2: KYC/AML Infrastructure (Week 3-5)
- [ ] Sumsub/Onfido integration
- [ ] KYC flow UI (web + mobile)
- [ ] Document upload and verification
- [ ] AML screening integration (sanctions, PEP)
- [ ] Risk scoring engine
- [ ] Transaction monitoring system
- [ ] Compliance dashboard for operations team
- [ ] Testing with various ID types and countries

### Phase 3: Bank Account Management (Week 6-7)
- [ ] Bank account data model and encryption
- [ ] Add bank account UI flow
- [ ] Country-specific form validation
- [ ] Micro-deposit verification implementation
- [ ] Plaid integration for instant verification (US)
- [ ] Bank account management UI
- [ ] Testing with multiple countries and formats

### Phase 4: RedotPay Fiat Gateway Integration (Week 8-11)
- [ ] RedotPay API integration research and setup
- [ ] Crypto-to-fiat conversion engine
- [ ] Exchange rate API integration and caching
- [ ] Payout execution flow (backend)
- [ ] Payout status tracking and webhooks
- [ ] Payout UI flow (web + mobile)
- [ ] Fee calculation and display
- [ ] Multi-currency support (USD, THB, PHP, BRL, EUR)
- [ ] Payout history and export
- [ ] Transaction limits enforcement based on KYC level
- [ ] Testing with multiple currencies and destinations
- [ ] Error handling and refund flows

### Phase 5: Multi-Chain Expansion (Week 12-14)
- [ ] Arbitrum mainnet integration
- [ ] Base mainnet integration
- [ ] Optimism mainnet integration
- [ ] Avalanche C-Chain integration
- [ ] Cross-chain balance aggregation
- [ ] Circle CCTP bridge integration
- [ ] Multi-chain transaction history
- [ ] Chain switching UI
- [ ] Testing on all mainnets

### Phase 6: Enhanced Swap Functionality (Week 15-16)
- [ ] 1inch DEX aggregator integration
- [ ] 0x Protocol integration
- [ ] Multi-hop routing optimization
- [ ] Slippage protection
- [ ] MEV protection
- [ ] Limit order functionality
- [ ] Price alerts
- [ ] Swap analytics dashboard
- [ ] Testing across all chains

### Phase 7: Multi-Exchange Investment (Week 17-19)
- [ ] Binance Earn API integration
- [ ] Kraken Staking API integration
- [ ] Unified investment dashboard
- [ ] Investment plan comparison UI
- [ ] Multi-exchange portfolio view
- [ ] Auto-rebalancing functionality
- [ ] Risk management dashboard
- [ ] Exchange health monitoring
- [ ] Testing with all exchanges

### Phase 8: On-Chain Staking (Week 20-21)
- [ ] Lido Finance integration (ETH, MATIC)
- [ ] Rocket Pool integration (ETH)
- [ ] Frax integration
- [ ] Liquid staking UI
- [ ] Staking position management
- [ ] Reward tracking
- [ ] Unstaking flow
- [ ] Testing on mainnet

### Phase 9: P2P Marketplace (Week 22-24)
- [ ] P2P offer creation UI
- [ ] Marketplace listing and search
- [ ] Escrow smart contract development
- [ ] Escrow smart contract audit
- [ ] Trade execution flow
- [ ] Chat/messaging system
- [ ] Reputation system
- [ ] Dispute resolution mechanism
- [ ] Mediator assignment
- [ ] Testing P2P flows end-to-end

### Phase 10: Credit Services (Week 25-27)
- [ ] Lending protocol integration or custom smart contract
- [ ] Loan smart contract development
- [ ] Loan smart contract audit
- [ ] Collateral management system
- [ ] LTV calculation and monitoring
- [ ] Liquidation engine
- [ ] Auto-collateral top-up
- [ ] Loan UI flow
- [ ] Interest accrual tracking
- [ ] Testing liquidation scenarios

### Phase 11: Mobile Applications (Week 28-32)
- [ ] iOS app development (Swift/SwiftUI)
- [ ] Android app development (Kotlin/Compose)
- [ ] Mobile passkey integration
- [ ] Push notification setup (FCM, APNs)
- [ ] QR code scanner
- [ ] Biometric authentication
- [ ] Mobile-specific UI/UX
- [ ] App Store submission preparation
- [ ] Google Play submission preparation
- [ ] Beta testing (TestFlight, Play Console)
- [ ] App Store release

### Phase 12: Additional Features (Week 33-35)
- [ ] Recurring payment automation
- [ ] Invoice generation system
- [ ] Referral program implementation
- [ ] Premium subscription system
- [ ] Payment processing for subscriptions
- [ ] Multi-language support
- [ ] Accessibility improvements
- [ ] Advanced analytics dashboard

### Phase 13: Security & Compliance (Week 36-38)
- [ ] SOC 2 Type II audit preparation
- [ ] Penetration testing
- [ ] Smart contract audits (escrow, lending)
- [ ] Bug bounty program launch
- [ ] GDPR compliance review
- [ ] Money transmitter license applications
- [ ] Privacy policy updates
- [ ] Terms of service updates
- [ ] Compliance documentation

### Phase 14: Performance & Scale (Week 39-40)
- [ ] Load testing (10K+ concurrent users)
- [ ] Database optimization and sharding
- [ ] Kubernetes setup for auto-scaling
- [ ] CDN optimization
- [ ] API rate limiting tuning
- [ ] Queue optimization for async jobs
- [ ] Caching strategy optimization
- [ ] Monitoring and alerting setup

### Phase 15: Beta Launch (Week 41-43)
- [ ] Mainnet deployment
- [ ] Beta user onboarding (500-1000 users)
- [ ] Customer support training
- [ ] Documentation finalization
- [ ] Marketing materials
- [ ] Bug fixes from beta feedback
- [ ] Performance tuning
- [ ] User feedback collection

### Phase 16: Production Launch (Week 44-46)
- [ ] Full public launch
- [ ] Marketing campaign
- [ ] Press release
- [ ] Influencer partnerships
- [ ] User acquisition campaigns
- [ ] 24/7 support setup
- [ ] Continuous monitoring
- [ ] Gradual rollout of advanced features

## Success Metrics (V2)

### User Adoption
- 10,000+ wallet creations in first 3 months
- 60%+ KYC completion rate (Level 1+)
- 40%+ users try fiat payout
- 50%+ users create investment position
- 30%+ users use multi-chain features
- 20%+ mobile app adoption
- 5%+ premium subscription conversion

### Transaction Performance
- 95%+ transaction success rate (crypto)
- 92%+ fiat payout success rate
- 90%+ fiat payouts complete within 24 hours
- <30 seconds average transaction confirmation
- 99.9%+ accuracy for exchange rate calculations
- 98%+ KYC approval rate (legitimate users)
- 95%+ bank account verification success

### Financial Metrics
- $50M+ Total Value Locked (TVL) in investments
- $5M+ monthly fiat payout volume
- $1M+ monthly swap volume
- $500K+ in P2P marketplace volume
- $200K+ in active loans
- Average investment position size: $1,000+
- Investment retention rate: 85%+
- User retention rate: 75%+ after 3 months
- Monthly active users (MAU) growth: 25%+

### Revenue Metrics
- Monthly recurring revenue: $100K+
- Swap fees: $15K+/month
- Fiat payout fees: $50K+/month
- Investment spread: $20K+/month
- Premium subscriptions: $10K+/month
- Loan interest: $5K+/month
- Average revenue per user (ARPU): $20+/month

### System Performance
- 99.95% uptime
- <2s API response time (p95)
- <500ms mobile app load time
- <3s fiat payout submission time
- Support for 10,000+ concurrent users
- Zero security breaches
- <1% fraud/chargeback rate

## Risk & Mitigation (V2)

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Regulatory non-compliance | Critical | Medium | Legal team, compliance monitoring, license applications |
| RedotPay integration delays | High | Medium | Parallel vendor evaluation, phased rollout |
| KYC vendor issues | High | Low | Backup vendor (Onfido), manual review process |
| Multi-chain smart contract bugs | Critical | Low | Extensive audits, gradual rollout, bug bounty |
| Bank verification fraud | Medium | Medium | Enhanced AML checks, manual review for high-risk |
| Liquidation failures (credit) | High | Low | Real-time monitoring, over-collateralization buffer |
| P2P escrow exploits | Critical | Low | Smart contract audit, escrow insurance fund |
| Exchange insolvency | High | Low | Multi-exchange diversification, insurance disclosures |
| Data breach (bank details) | Critical | Low | Encryption, HSM, SOC 2 compliance, regular audits |
| Mobile app rejection | Medium | Medium | Pre-submission review, compliance with app store policies |
| Scalability issues | High | Medium | Load testing, auto-scaling, performance monitoring |
| Currency exchange rate manipulation | Medium | Low | Multiple oracle sources, rate validation, circuit breakers |
| AML false positives | Medium | High | Tunable risk thresholds, efficient manual review process |
| Cross-chain bridge failures | High | Low | Use Circle CCTP (trusted), fallback mechanisms |

## Compliance & Regulatory (V2)

### Crypto Wallet Operations
- Smart contract audits (quarterly)
- Bug bounty program
- Terms of service updates
- Privacy policy (passkey, multi-chain)

### Fiat Operations & Money Transmission
- **KYC/AML Implementation**:
  - Sumsub/Onfido integration
  - 4-level KYC system with appropriate limits
  - Real-time sanctions screening
  - PEP and adverse media checks
  - Ongoing transaction monitoring

- **Licensing Requirements**:
  - US: Money transmitter licenses (state-by-state)
  - EU: Payment Service Provider (PSD2) license
  - UK: FCA registration
  - Other jurisdictions: Country-specific licenses

- **Transaction Limits**:
  - Tiered limits based on KYC level
  - Daily, monthly, and lifetime limits
  - Velocity checks

- **Record Keeping**:
  - 7-year retention for financial transactions
  - Complete audit trail
  - Automated reporting to authorities

- **Tax Reporting**:
  - 1099 forms (US users)
  - FATCA compliance
  - CRS reporting (international)
  - Transaction export for users

### Data Privacy
- **GDPR Compliance**:
  - Data portability
  - Right to deletion (with legal exceptions)
  - Consent management
  - DPO appointment

- **CCPA Compliance**:
  - California user data rights
  - Opt-out mechanisms

- **Data Security**:
  - End-to-end encryption
  - Encrypted storage (AES-256)
  - HSM for API credentials
  - SOC 2 Type II certification

### Consumer Protection
- Clear fee disclosure
- Dispute resolution process
- Refund policy
- Risk warnings for volatile assets
- Terms of service (comprehensive)
- Insurance disclosures for exchange custody

## Monitoring & Alerting (V2)

### Key Metrics to Monitor
- All MVP metrics (from V1)
- KYC approval/rejection rates
- Bank account verification success rates
- Fiat payout success/failure rates
- Fiat payout processing times
- Exchange rate API health
- RedotPay API health and response times
- Multi-chain transaction success rates
- Cross-chain bridge transaction status
- Staking position creation and rewards
- P2P trade completion rates
- Escrow contract health
- Loan LTV ratios and liquidation risk
- Exchange API health (Binance, Kraken)
- Mobile app crash rates
- Push notification delivery rates

### Critical Alerts
- Fiat payout failure rate > 5%
- Fiat payout delays > 48 hours
- KYC approval rate < 80%
- Bank verification fraud detected
- AML match on sanctions list
- High-risk transaction flagged
- Exchange API downtime
- Smart contract anomaly detected
- Liquidation threshold breach
- P2P dispute escalation
- Mobile app crash rate > 1%
- API response time > 5s
- Database connection failures
- Security event detected

## Dependencies (V2)

### External Services

**Circle Infrastructure** (from MVP)
- Circle Modular Wallets SDK
- Circle Bundler service
- Circle Paymaster service
- Circle Indexing service
- Circle CCTP (Cross-Chain Transfer Protocol)

**RedotPay Services** (NEW)
- RedotPay Global Payout API
- RedotPay Exchange Rate API
- RedotPay Bank Settlement Network
- RedotPay Webhook System

**KYC/AML Providers** (NEW)
- Sumsub KYC verification (primary)
- Onfido KYC verification (backup)
- ComplyAdvantage AML screening
- Chainalysis (crypto transaction monitoring)

**Bank Verification** (NEW)
- Plaid (US instant verification)
- TrueLayer (UK/EU instant verification)
- Micro-deposit system (fallback)

**Exchange Integrations**
- WhiteBit Flex Investment API (from MVP)
- Binance Earn API (NEW)
- Kraken Staking API (NEW)
- Coinbase Earn API (planned)

**DeFi Protocols** (NEW)
- Lido Finance (liquid staking)
- Rocket Pool (liquid staking)
- Uniswap V3 (DEX)
- 1inch Aggregation API
- 0x Protocol API

**Infrastructure**
- AWS/GCP/Azure (cloud hosting)
- Kubernetes (orchestration)
- PostgreSQL (database)
- Redis (caching)
- RabbitMQ / AWS SQS (message queue)
- HashiCorp Vault (secrets management)
- Cloudflare (CDN, DDoS protection)
- Datadog / New Relic (monitoring)
- Sentry (error tracking)
- Firebase Cloud Messaging (push notifications)
- Apple Push Notification Service (iOS push)

### Development Stack
```json
{
  "frontend": {
    "@circle-fin/modular-wallets-core": "^1.x.x",
    "viem": "^2.21.27",
    "react": "^18.2.0",
    "typescript": "^5.0.3",
    "next.js": "^14.0.0",
    "tailwindcss": "^3.3.0"
  },
  "mobile": {
    "ios": "Swift 5.9, SwiftUI",
    "android": "Kotlin 1.9, Jetpack Compose"
  },
  "backend": {
    "runtime": "Node.js 20 LTS",
    "framework": "Express / Fastify",
    "orm": "Prisma / TypeORM",
    "database": "PostgreSQL 15",
    "cache": "Redis 7",
    "queue": "BullMQ"
  },
  "blockchain": {
    "wagmi": "^2.x.x",
    "ethers": "^6.x.x",
    "hardhat": "^2.19.0"
  }
}
```

## Migration from MVP to V2

### Data Migration
1. Backup all MVP data (wallets, transactions, investments)
2. Update database schema for V2 models
3. Migrate existing users to V2 schema
4. Add default KYC level (0) for existing users
5. Preserve all investment positions from WhiteBit
6. Maintain transaction history

### User Communication
1. Email all MVP users about V2 features
2. Highlight new capabilities (fiat, multi-chain)
3. Explain KYC requirement for fiat features
4. Provide migration guide
5. Offer early adopter benefits (fee discounts)

### Gradual Rollout
1. **Week 1-2**: Enable V2 for 10% of users (testing)
2. **Week 3-4**: Enable for 50% of users
3. **Week 5-6**: Enable for 100% of users
4. Monitor errors and rollback if needed
5. Collect user feedback continuously

### Feature Flags
- Enable/disable fiat features per user
- Enable/disable new chains individually
- Enable/disable P2P marketplace
- Enable/disable credit services
- A/B testing for UI changes

## Documentation (V2)

### User Documentation
- Getting started guide (web + mobile)
- How to complete KYC verification
- How to add and verify bank account
- How to cash out crypto to bank
- How to use multi-chain features
- How to stake on Lido/Rocket Pool
- How to trade on P2P marketplace
- How to take a collateralized loan
- How to earn with investments
- Security best practices
- FAQ (comprehensive)

### Developer Documentation
- API reference (complete)
- SDK integration guide (web, iOS, Android)
- Smart contract documentation
- Webhook integration guide
- Testing guide
- Deployment guide
- Architecture overview
- Database schema documentation

### Compliance Documentation
- KYC/AML procedures
- Transaction monitoring procedures
- Risk assessment framework
- Incident response plan
- Data protection policy
- License application materials

## Appendix

### Glossary (V2 Additions)

**KYC/AML**
- **KYC**: Know Your Customer - identity verification
- **AML**: Anti-Money Laundering - fraud prevention
- **PEP**: Politically Exposed Person - high-risk individual
- **SAR**: Suspicious Activity Report - regulatory filing
- **OFAC**: Office of Foreign Assets Control - US sanctions
- **CRS**: Common Reporting Standard - international tax
- **FATCA**: Foreign Account Tax Compliance Act

**Fiat Operations**
- **Off-Ramp**: Converting crypto to fiat
- **On-Ramp**: Converting fiat to crypto
- **ACH**: Automated Clearing House (US bank transfers)
- **SEPA**: Single Euro Payments Area (EU bank transfers)
- **SWIFT**: Society for Worldwide Interbank Financial Telecommunication
- **IBAN**: International Bank Account Number
- **FPS**: Faster Payments Service (UK)

**Liquid Staking**
- **stETH**: Lido Staked Ether (liquid staking derivative)
- **rETH**: Rocket Pool ETH (liquid staking derivative)
- **Liquid Staking Derivative**: Token representing staked asset, can be used in DeFi

**Lending/Credit**
- **Collateral**: Asset locked to secure a loan
- **LTV**: Loan-to-Value ratio
- **Liquidation**: Forced sale of collateral
- **Health Factor**: Measure of loan safety (> 1 = safe)

### V2 Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                       CoinPay V2 Platform                           │
│                                                                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐            │
│  │   Web App    │  │   iOS App    │  │  Android App │            │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘            │
│         │                  │                  │                     │
│         └──────────────────┴──────────────────┘                     │
│                            │                                        │
│                            ▼                                        │
│                   ┌─────────────────┐                              │
│                   │   API Gateway   │                              │
│                   └────────┬────────┘                              │
│                            │                                        │
│         ┌──────────────────┼──────────────────┐                    │
│         │                  │                  │                    │
│         ▼                  ▼                  ▼                    │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐            │
│  │   Circle    │  │   RedotPay   │  │  Exchanges   │            │
│  │   Wallet    │  │     Fiat     │  │  Investment  │            │
│  │   Service   │  │    Gateway   │  │   Service    │            │
│  └──────┬──────┘  └──────┬───────┘  └──────┬───────┘            │
│         │                │                  │                     │
│         ▼                ▼                  ▼                     │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐            │
│  │ Multi-Chain │  │    Banks     │  │  WhiteBit    │            │
│  │ Blockchain  │  │   Worldwide  │  │  Binance     │            │
│  │  Networks   │  │              │  │  Kraken      │            │
│  └─────────────┘  └──────────────┘  └──────────────┘            │
│                                                                     │
│  ┌──────────────────────────────────────────────────────┐         │
│  │           Supporting Services                        │         │
│  │  - KYC/AML (Sumsub, Onfido)                         │         │
│  │  - Bank Verification (Plaid, TrueLayer)             │         │
│  │  - DeFi Protocols (Lido, Rocket Pool, Uniswap)     │         │
│  │  - DEX Aggregators (1inch, 0x)                      │         │
│  │  - Smart Contracts (P2P Escrow, Lending)           │         │
│  └──────────────────────────────────────────────────────┘         │
└─────────────────────────────────────────────────────────────────────┘
```

### References (V2 Specific)

**KYC/AML Providers**
- [Sumsub Documentation](https://developers.sumsub.com/)
- [Onfido Documentation](https://documentation.onfido.com/)
- [ComplyAdvantage](https://www.complyadvantage.com/)

**Fiat Gateway**
- [RedotPay Global Payout](https://www.redotpay.com/global-payout)
- [RedotPay API Docs](https://www.redotpay.com/developers)

**Bank Verification**
- [Plaid Documentation](https://plaid.com/docs/)
- [TrueLayer Documentation](https://docs.truelayer.com/)

**Exchange APIs**
- [Binance Earn API](https://binance-docs.github.io/apidocs/spot/en/#earn-endpoints)
- [Kraken Staking API](https://docs.kraken.com/rest/#tag/Staking)

**DeFi Protocols**
- [Lido Documentation](https://docs.lido.fi/)
- [Rocket Pool Documentation](https://docs.rocketpool.net/)
- [1inch API](https://docs.1inch.io/)
- [0x Protocol](https://docs.0x.org/)

**Compliance Resources**
- [FinCEN Guidance](https://www.fincen.gov/)
- [GDPR Official Text](https://gdpr.eu/)
- [PSD2 Overview](https://ec.europa.eu/info/law/payment-services-psd-2-directive-eu-2015-2366_en)

---

**Document Version**: 2.0
**Last Updated**: 2025-10-25
**Author**: Development Team
**Status**: V2 Specification - Ready for Implementation
**Based On**: CoinPay Wallet MVP (v1.0) - Proven Foundation

**V2 Philosophy**: Build on the proven MVP foundation (gasless wallet, passkeys, stablecoin yield) and add comprehensive fiat integration, multi-chain support, and advanced DeFi services to create a complete crypto-to-fiat payment platform that serves global users.

**Estimated Timeline**: 46 weeks (~11 months) from MVP completion to V2 full production launch
**Team Size**: 15-20 people (6 backend, 4 frontend, 2 mobile, 2 blockchain, 2 DevOps, 1 QA, 1 compliance, 1 PM, 1 designer)
**Budget Estimate**: $2.5M - $3.5M (includes licensing, audits, infrastructure, team)
