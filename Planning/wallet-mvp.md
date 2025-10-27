# Product Requirements Document: CoinPay Wallet MVP

## Executive Summary

This MVP PRD defines the minimum viable product for CoinPay - a crypto wallet platform that enables gasless USDC transactions, crypto-to-fiat payouts, and stablecoin yield generation. The MVP focuses on proving core value propositions: simple wallet management, fiat off-ramp capabilities, and passive income through exchange investments.

## Product Overview

### MVP Objective
Create a streamlined crypto wallet that provides:

**Core Wallet Features**
- Gasless USDC transactions (sponsored by Circle paymaster)
- Passkey-based authentication (no seed phrases)
- Smart account wallet with account abstraction
- Single-chain support (Polygon Amoy testnet)

**Fiat Integration (Basic)**
- Crypto-to-fiat conversion and bank payout (USD only for MVP)
- Direct bank account integration ({manully added:deferred to post-MVP})
- Instant to 24-hour settlement

**Monetization Features**
- Stablecoin investment with WhiteBit Flex (6-10% APY)
- Basic swap functionality with small transaction fees
- Fiat payout service fees

### Value Proposition
- **Simplicity**: No gas fees, no seed phrases, easy bank withdrawals
- **Security**: Passkey authentication, phishing-resistant
- **Earnings**: Generate yield on stablecoin holdings
- **Access**: Convert crypto to local currency easily

## Technical Architecture

### Core Components

#### 1. SDK Integration
- **Package**: `@circle-fin/modular-wallets-core`
- **Platform**: Web only (mobile deferred to post-MVP)
- **Version**: 1.x.x (latest stable)

#### 2. Authentication System
- **Method**: WebAuthn Passkeys
- **Domain Configuration**: Required for passkey registration
- **Modes**: Register (new users), Login (existing users)

#### 3. Smart Account Architecture
- **Account Type**: Circle Smart Account (ERC-4337 compatible)
- **Owner**: Passkey credential (WebAuthn)
- **Bundler**: Circle's bundler service
- **Paymaster**: Circle's paymaster for gas sponsorship

#### 4. Blockchain Support (MVP)
- **Testnet Only**: Polygon Amoy
- **Token**: USDC (6 decimals)
- **Contract**: `0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582`

#### 5. Fiat Gateway Integration
- **Service Provider**: RedotPay (or similar provider - {manully added:deferred to post-MVP})
- **MVP Capability**: Crypto-to-USD conversion and bank transfers - ({manully added: deferred to post-MVP and bank cooparation and integration to plan})
- **Supported Assets**: USDC only
- **Settlement**: Direct bank transfer (ACH)

#### 6. Exchange Investment Service
- **Service Provider**: WhiteBit Flex Investment API
- **Capability**: Stablecoin yield generation
- **Supported Assets**: USDC only
- **Expected APY**: 6-10% (variable)

## Functional Requirements

### FR-1: Configuration & Setup

#### FR-1.1: Console Configuration
- Create Client Key in Circle Console
- Configure Passkey Domain Name
- Retrieve Client URL for SDK initialization

#### FR-1.2: Environment Configuration
```
CLIENT_KEY=<circle-client-key>
CLIENT_URL=<circle-client-url>
WHITEBIT_API_KEY=<whitebit-api-key>
WHITEBIT_API_SECRET=<whitebit-api-secret>
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
- Session established

#### FR-2.3: Passkey Recovery (Simplified)
- Display recovery instructions
- Recommend multi-device passkey registration
- Manual recovery contact support flow

### FR-3: Wallet Creation

#### FR-3.1: Smart Account Initialization
- Initialize modular transport with client configuration
- Create public client for blockchain interaction
- Generate Circle Smart Account with passkey owner
- Create bundler client for UserOperation handling

#### FR-3.2: Account Properties
- Deterministic address generation
- Smart contract deployment on first transaction
- Single signer only (MVP)

### FR-4: Transaction Management

#### FR-4.1: Gasless USDC Transfers
- Encode transfer operation (recipient, token, amount)
- Construct UserOperation with paymaster enabled
- Submit UserOperation to bundler
- Return transaction hash for tracking

#### FR-4.2: Transaction Monitoring
- Wait for UserOperation receipt
- Parse transaction receipt for status
- Handle transaction failures gracefully
- Display status updates in UI

#### FR-4.3: Transaction History (Basic)
- Display last 20 transactions
- Show: recipient, amount, status, timestamp
- Filter by status (pending, confirmed, failed)

### FR-5: Fiat Off-Ramp (Basic)

#### FR-5.1: Bank Account Management
- User can add ONE bank account (US only)
- Store: account holder name, routing number, account number (encrypted)
- Basic validation of bank account details

#### FR-5.2: Crypto-to-Fiat Conversion
- Display real-time USDC to USD exchange rate
- Calculate recipient amount before transaction
- Show conversion fees transparently (1-2%)
- Lock exchange rate for 30 seconds

#### FR-5.3: Fiat Payout Execution
- User selects USDC amount to convert
- System displays USD amount and fees
- User confirms payout transaction
- Submit payout request to gateway
- Track payout status (pending, processing, completed, failed)
- Notify user upon completion

#### FR-5.4: Payout Transaction History
- Display history of fiat payouts
- Show: amount, exchange rate, fees, status, date
- Filter by status

### FR-6: Basic Swap Functionality

#### FR-6.1: Simple Swap Interface
- Support USDC <-> ETH/MATIC only (MVP)
- Display current exchange rate
- Show estimated output amount
- Display swap fee (0.5-1%)

#### FR-6.2: Swap Execution
- User enters amount to swap
- System displays output amount and fees
- User confirms swap
- Execute swap through DEX aggregator
- Return transaction hash
- Update balances

### FR-7: Exchange Investment (Monetization Core)

#### FR-7.1: Exchange Connection
- Connect WhiteBit account via API credentials
- Store encrypted API key and secret
- Validate credentials with test API call
- Display connection status

#### FR-7.2: Investment Plan Display
- Fetch USDC Flex plans from WhiteBit
- Display: APY, minimum amount, plan type
- Show plan details and terms

#### FR-7.3: Create Investment
- User enters USDC amount to invest
- System validates amount against balance and minimums
- Display investment preview: amount, estimated yield, terms
- User confirms investment
- Transfer USDC from Circle wallet to WhiteBit
- Create investment position via WhiteBit API
- Display position as "Active"

#### FR-7.4: Investment Position View
- Display active investments:
  - Principal amount
  - Current value
  - Accrued rewards
  - Current APY
  - Status
- Show total portfolio value

#### FR-7.5: Withdraw Investment
- User requests full withdrawal (partial not supported in MVP)
- Display withdrawal preview: amount, processing time
- User confirms withdrawal
- Submit withdrawal request to WhiteBit
- Transfer USDC back to Circle wallet
- Update position status to "Withdrawn"

## Non-Functional Requirements

### NFR-1: Security
- Client keys in environment variables
- Passkey credentials never leave device
- HTTPS-only communication
- Encrypted storage for bank details and API credentials

### NFR-2: Performance
- Transaction submission < 3 seconds
- UserOperation confirmation < 45 seconds
- Support 100+ concurrent users (MVP)
- API response time < 3 seconds

### NFR-3: Reliability
- 99% uptime target for MVP
- Retry logic for failed operations
- Transaction queue management

### NFR-4: Usability
- Clear error messages
- Transaction status indicators
- Estimated confirmation times
- Simple, intuitive UI

## Data Models

### Wallet Account
```typescript
interface WalletAccount {
  address: string;
  ownerCredentialId: string;
  chainId: number;
  createdAt: Date;
  lastActivityAt: Date;
}
```

### Transaction Record
```typescript
interface TransactionRecord {
  id: string;
  userOpHash: string;
  transactionHash: string;
  from: string;
  to: string;
  tokenAddress: string;
  amount: bigint;
  status: 'pending' | 'confirmed' | 'failed';
  chainId: number;
  createdAt: Date;
  confirmedAt?: Date;
}
```

### Bank Account (Simplified)
```typescript
interface BankAccount {
  id: string;
  userId: string;
  accountHolderName: string;
  accountNumber: string;        // Encrypted
  routingNumber: string;         // US ACH routing
  bankName: string;
  isVerified: boolean;
  createdAt: Date;
}
```

### Fiat Payout Transaction
```typescript
interface FiatPayoutTransaction {
  id: string;
  userId: string;
  walletAddress: string;
  bankAccountId: string;
  usdcAmount: bigint;
  usdAmount: number;
  exchangeRate: number;
  fee: number;
  status: 'pending' | 'processing' | 'completed' | 'failed';
  providerTransactionId?: string;
  createdAt: Date;
  completedAt?: Date;
}
```

### Exchange Connection
```typescript
interface ExchangeConnection {
  id: string;
  userId: string;
  exchangeName: 'whitebit';
  apiKey: string;               // Encrypted
  apiSecret: string;            // Encrypted
  isActive: boolean;
  createdAt: Date;
  lastUsedAt?: Date;
}
```

### Investment Position
```typescript
interface InvestmentPosition {
  id: string;
  userId: string;
  walletAddress: string;
  exchangeName: 'whitebit';
  planName: string;
  principalAmount: bigint;      // USDC
  currentValue: bigint;         // Principal + rewards
  accruedRewards: bigint;
  currentAPY: number;
  status: 'pending' | 'active' | 'withdrawn';
  startDate: Date;
  lastUpdated: Date;
}
```

## User Flows

### Flow 1: New User Onboarding
1. User clicks "Create Wallet"
2. Enter username
3. Browser displays passkey creation dialog
4. User confirms with biometric/PIN
5. Passkey created
6. Wallet address displayed
7. User shown wallet dashboard

### Flow 2: Send Gasless Transaction
1. User enters recipient address and USDC amount
2. User clicks "Send"
3. Passkey signature requested
4. User confirms with biometric
5. Transaction submitted
6. Progress indicator shown
7. Transaction confirmed
8. Success notification

### Flow 3: Cash Out to Bank (USD)
1. User navigates to "Cash Out"
2. User enters USDC amount
3. System shows USD amount, exchange rate, fees
4. User selects bank account (or adds new one)
5. User confirms payout
6. USDC deducted from wallet
7. Payout status shown as "Processing"
8. Notification when USD arrives in bank

### Flow 4: Create Investment
1. User navigates to "Earn" section
2. System displays WhiteBit USDC Flex plan (8.5% APY example)
3. User enters USDC amount
4. System shows estimated daily/monthly/yearly earnings
5. User confirms investment
6. Passkey signature requested
7. USDC transferred to WhiteBit
8. Investment position created as "Active"
9. Dashboard shows position with accruing rewards

### Flow 5: Withdraw Investment
1. User views investment position
2. User clicks "Withdraw"
3. System shows withdrawal preview
4. User confirms
5. Withdrawal request sent to WhiteBit
6. USDC returned to Circle wallet
7. Position marked as "Withdrawn"

## Implementation Phases

### Phase 1: Core Wallet Foundation (Week 1-2)
- [ ] Circle Console setup
- [ ] SDK installation and configuration
- [ ] Passkey registration and authentication
- [ ] Smart account creation
- [ ] Basic USDC transfer on Polygon Amoy
- [ ] Transaction status tracking
- [ ] Basic UI for wallet creation and transactions

### Phase 2: Transaction History & UI Polish (Week 3)
- [ ] Transaction history display
- [ ] Basic wallet dashboard
- [ ] Error handling and user feedback
- [ ] Balance display and refresh

### Phase 3: Fiat Off-Ramp Integration (Week 4-5)
- [ ] Bank account management (add/store)
- [ ] Fiat gateway API integration
- [ ] USDC to USD conversion engine
- [ ] Payout execution flow
- [ ] Payout status tracking
- [ ] Fee calculation and display

### Phase 4: Exchange Investment (Week 6-7)
- [ ] WhiteBit API integration
- [ ] API credential storage (encrypted)
- [ ] Fetch investment plans
- [ ] Investment creation flow
- [ ] Transfer USDC to WhiteBit
- [ ] Position tracking and display
- [ ] Rewards calculation
- [ ] Withdrawal flow

### Phase 5: Basic Swap (Week 8)
- [ ] DEX aggregator integration (simple)
- [ ] USDC <-> ETH swap UI
- [ ] Swap execution
- [ ] Fee collection

### Phase 6: Testing & Bug Fixes (Week 9)
- [ ] End-to-end testing on testnet
- [ ] Fiat payout testing
- [ ] Investment lifecycle testing
- [ ] Bug fixes
- [ ] Performance optimization

### Phase 7: Beta Launch (Week 10)
- [ ] Limited beta rollout (50-100 users)
- [ ] Monitoring and alerting setup
- [ ] User feedback collection
- [ ] Critical bug fixes

### Phase 8: Production Preparation (Week 11)
- [ ] Security audit (basic)
- [ ] Compliance review (KYC/AML preparation)
- [ ] Documentation
- [ ] Support materials

## MVP Exclusions (Post-MVP Features)

### Deferred to V2
- Multi-chain support (Arbitrum, Base, Optimism, etc.)
- Mobile apps (iOS, Android)
- Multi-currency fiat support (EUR, GBP, THB, PHP, etc.)
- Advanced swap features (liquidity routing, MEV protection)
- Multiple bank accounts per user
- P2P marketplace
- Credit/lending services
- On-chain staking
- Batch transactions
- Multiple exchange integrations (Binance, Kraken, etc.)
- Advanced investment features (partial withdrawals, auto-reinvest)

## Success Metrics (MVP)

### User Adoption
- 100 wallet creations in first month
- 70%+ successful passkey authentication rate
- 30% of users try fiat payout
- 20% of users create investment position

### Transaction Performance
- 90%+ transaction success rate
- <45 seconds average confirmation time
- 100% gas sponsorship (no user fees)
- 80%+ fiat payout completion within 24 hours

### Financial Metrics
- $10K+ total value locked in investments
- Average investment size: $200+
- $500+ monthly revenue from fees
- 60%+ user retention after 1 month

## Risks & Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| Bundler downtime | High | Retry logic, monitoring |
| Passkey device loss | Medium | Multi-device setup guide, recovery docs |
| WhiteBit API changes | Medium | Version pinning, adapter pattern |
| Fiat payout failures | High | Retry mechanism, manual review process |
| Low investment APY | Medium | Clear communication, diversify exchanges in V2 |
| Regulatory compliance | High | Legal review, KYC prep for production |
| Exchange insolvency | Critical | Insurance info, risk warnings, start small |

## Monetization Model (MVP)

### Revenue Streams
1. **Swap Fees**: 0.5-1% on swap transactions
2. **Fiat Payout Fees**: 1-2% on crypto-to-fiat conversions
3. **Investment Spread**: 0.5-1% spread on investment APY (optional)

### Target Revenue (Month 3)
- 100 active users
- Average 2 swaps/month: $200 * 0.5% * 2 * 100 = $200
- Average 1 payout/month: $300 * 1.5% * 1 * 100 = $450
- Investments: $10K TVL (relationship building, minimal revenue initially)
- **Total**: ~$650/month (proof of concept)

## Compliance & Regulatory (MVP Scope)

### MVP Approach
- **KYC/AML**: Basic email verification only (no full KYC in MVP)
- **Transaction Limits**: $1,000 daily limit per user
- **Restricted Regions**: Block high-risk jurisdictions
- **Terms of Service**: Basic ToS covering wallet and investment services
- **Risk Warnings**: Clear disclosures about crypto volatility and exchange custody risks

### Production Requirements (Post-MVP)
- Full KYC integration (Sumsub/Onfido)
- AML transaction monitoring
- Money transmitter licenses
- Regulatory compliance review

## Technical Dependencies

### Required Services
- Circle Modular Wallets SDK and infrastructure
- WhiteBit Flex Investment API access
- Fiat gateway provider (RedotPay or alternative)
- DEX aggregator (1inch/0x)
- HTTPS domain for passkeys
- Backend server (.net)
- Database (PostgreSQL)
- Encrypted storage vault (hashicorp vault)

### Development Stack
```json
{
  "@circle-fin/modular-wallets-core": "^1.x.x",
  "viem": "^2.21.27",
  "react": "^18.2.0",
  "typescript": "^5.0.3",
  ".net": "latest",
  "postgresql": "^14.x"
}
```

## Monitoring & Support

### Key Metrics to Monitor
- Transaction success/failure rates
- Passkey authentication success rate
- Fiat payout processing times
- Investment position creation success
- WhiteBit API health
- User balance accuracy
- Error rates by operation type

### Alerting Thresholds
- Transaction failure rate >10%
- Fiat payout delays >48 hours
- WhiteBit API errors
- Investment position sync failures
- Unusual balance discrepancies

## Documentation Requirements

### User Documentation
- How to create wallet with passkey
- How to send USDC
- How to cash out to bank
- How to earn yield on stablecoins
- FAQ and troubleshooting

### Developer Documentation
- API integration guide
- SDK setup instructions
- Deployment guide
- Troubleshooting guide

---

**Document Version**: 1.0 (MVP)
**Last Updated**: 2025-10-25
**Author**: Development Team
**Status**: MVP Specification - Ready for Implementation
**Based On**: Wallet.MD v3.0 (Full PRD)

**MVP Philosophy**: Launch fast with core value propositions (gasless wallet, fiat off-ramp, stablecoin yield). Validate user demand before expanding to multi-chain, multi-currency, and advanced DeFi features.
