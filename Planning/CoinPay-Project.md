# CoinPay Wallet MVP - Project Planning Guide

**Version**: 1.0
**Status**: Ready for Implementation
**Duration**: 14-18 weeks
**Team Size**: 6-8 engineers (2-3 Backend, 2 Frontend, 2-3 QA)
**Based On**: wallet-mvp.md PRD v1.0, Estimation-mvp-detailed.csv

---

## 📋 Table of Contents

1. [Project Overview](#project-overview)
2. [Epic Breakdown](#epic-breakdown)
3. [Phase 0: Project Setup & Infrastructure](#phase-0-project-setup--infrastructure)
4. [Phase 1: Core Wallet Foundation](#phase-1-core-wallet-foundation)
5. [Phase 2: Transaction Management](#phase-2-transaction-management)
6. [Phase 3: Fiat Off-Ramp](#phase-3-fiat-off-ramp)
7. [Phase 4: Exchange Investment](#phase-4-exchange-investment)
8. [Phase 5: Basic Swap](#phase-5-basic-swap)
9. [Cross-Cutting Concerns](#cross-cutting-concerns)
10. [Testing & Launch](#testing--launch)

---

## 🎯 Project Overview

### Mission
Build a streamlined crypto wallet enabling:
- **Gasless USDC transactions** (Circle paymaster sponsorship)
- **Passkey authentication** (no seed phrases)
- **Crypto-to-fiat payouts** (bank withdrawals)
- **Stablecoin yield generation** (WhiteBit Flex integration)

### Success Criteria
- ✅ 100 wallet creations in first month
- ✅ 70%+ passkey authentication success rate
- ✅ 90%+ transaction success rate
- ✅ <45 seconds average transaction confirmation
- ✅ $10K+ total value locked in investments

### Technical Stack
- **Backend**: ASP.NET Core, PostgreSQL, Hashicorp Vault, YARP Gateway
- **Frontend**: React 18, TypeScript, Vite, Circle SDK
- **Blockchain**: Polygon Amoy testnet, Circle Smart Accounts (ERC-4337)
- **Testing**: Playwright (UI), Cypress (E2E), Grafana K6 (Load)

---

## 📊 Epic Breakdown

| Epic | Backend (days) | Frontend (days) | QA (days) | Total (days) | Priority |
|------|----------------|-----------------|-----------|--------------|----------|
| **Phase 0: Setup & Infrastructure** | 9.88 | 17.29 | 7.33 | 35.20 | 🔴 Critical |
| **Phase 1: Core Wallet Foundation** | 29.00 | 19.21 | 21.08 | 88.29 | 🔴 Critical |
| **Phase 2: Transaction Management** | 13.17 | 11.75 | 11.25 | 36.08 | 🟠 High |
| **Phase 3: Fiat Off-Ramp** | 33.50 | 18.92 | 26.08 | 78.50 | 🟡 Medium* |
| **Phase 4: Exchange Investment** | 43.67 | 21.58 | 36.92 | 102.17 | 🔴 Critical |
| **Phase 5: Basic Swap** | 13.42 | 16.79 | 13.42 | 43.63 | 🟢 Low |
| **Cross-Cutting Concerns** | 85.73 | 49.91 | 75.84 | 211.48 | 🔴 Critical |
| **TOTAL MVP** | **289.37** | **180.25** | **192.92** | **662.54** | - |

*Note: Phase 3 (Fiat Off-Ramp) marked as "deferred to post-MVP" in PRD - confirm with stakeholders before implementation.

---

## 🚀 Phase 0: Project Setup & Infrastructure

**Duration**: 2 weeks (Weeks 1-2)
**Effort**: 35.20 days (BE: 9.88, FE: 17.29, QA: 7.33)
**Dependencies**: DevOps access, Circle Console access, Cloud infrastructure provisioning

### 0.1 Backend Infrastructure Setup

#### Epic: Backend Project Structure
- **Story 0.1.1**: Initialize ASP.NET Core Project
  - [ ] **Task BE-001**: Create ASP.NET Core Web API with clean architecture (0.54d)
    - Layers: API, Application, Domain, Infrastructure
    - Configure project structure and dependencies
    - Acceptance: Solution builds successfully with layered architecture

  - [ ] **Task BE-002**: Configure development environment (0.54d)
    - Setup Docker Compose for local PostgreSQL
    - Configure .env files for local development
    - Acceptance: Local dev environment runs with database connectivity

#### Epic: Database Foundation
- **Story 0.1.2**: PostgreSQL Database Setup
  - [ ] **Task BE-003**: Setup PostgreSQL database (1.08d)
    - Configure connection pooling
    - Setup EF Core Code-First migrations
    - Configure database context and providers
    - Acceptance: Database connects successfully, migrations run

#### Epic: Secrets Management
- **Story 0.1.3**: Hashicorp Vault Integration
  - [ ] **Task BE-004**: Configure Hashicorp Vault (2.00d)
    - Setup Vault SDK integration
    - Configure connection management
    - Store Circle API keys, WhiteBit credentials, encryption keys
    - Acceptance: Vault reads/writes secrets successfully

#### Epic: Observability & Monitoring
- **Story 0.1.4**: Logging Infrastructure
  - [ ] **Task BE-005**: Setup structured logging with Serilog (1.00d)
    - Configure correlation IDs for request tracing
    - Setup log levels (Debug, Info, Warning, Error)
    - JSON output to console/file
    - Acceptance: Logs capture request/response with correlation

  - [ ] **Task BE-006**: Implement health check endpoints (1.00d)
    - /health and /health/ready endpoints
    - Check DB, Vault, external API connectivity
    - Acceptance: Health checks return status for all dependencies

#### Epic: API Gateway & Cross-Cutting
- **Story 0.1.5**: API Gateway Configuration
  - [ ] **Task BE-007**: Configure YARP gateway (2.00d)
    - Setup routing rules
    - Configure load balancing
    - Based on existing gateway infrastructure
    - Acceptance: Gateway routes requests to backend APIs

  - [ ] **Task BE-008**: Configure CORS policies (0.54d)
    - Development and production CORS settings
    - Allow frontend communication
    - Acceptance: Frontend can make API calls without CORS errors

  - [ ] **Task BE-009**: Global exception handling middleware (1.08d)
    - Consistent error response format
    - Error logging integration
    - Acceptance: All errors return standard format with correlation ID

### 0.2 Frontend Infrastructure Setup

#### Epic: React Project Foundation
- **Story 0.2.1**: React Project Initialization
  - [ ] **Task FE-001**: Setup Vite/Create React App with TypeScript (0.54d)
    - Configure ESLint, Prettier
    - Setup path aliases
    - Acceptance: React app builds and runs in dev mode

  - [ ] **Task FE-002**: Install core dependencies (0.54d)
    - React, viem, Circle SDK packages
    - Package.json version management
    - Acceptance: All dependencies install without conflicts

  - [ ] **Task FE-003**: Environment configuration (0.50d)
    - .env files with CLIENT_KEY, CLIENT_URL, API endpoints
    - Environment variable validation
    - Acceptance: App loads environment config correctly

  - [ ] **Task FE-004**: Configure build pipeline (0.54d)
    - Dev and production build configurations
    - Hot Module Replacement (HMR)
    - Acceptance: Dev and prod builds work correctly

#### Epic: State Management & Architecture
- **Story 0.2.2**: React State Management
  - [ ] **Task FE-005**: Setup React Context API (1.00d)
    - WalletContext for wallet state
    - UserContext for authentication state
    - Acceptance: Contexts provide/consume data across components

  - [ ] **Task FE-006**: Create custom hooks for SDK (1.08d)
    - useCircleWallet, useTransaction, usePasskey hooks
    - Encapsulate SDK interaction logic
    - Acceptance: Hooks provide clean API for SDK operations

  - [ ] **Task FE-007**: Setup state persistence (0.88d)
    - localStorage/sessionStorage for auth state
    - Persist wallet address between sessions
    - Acceptance: State persists after page reload

#### Epic: Routing & Navigation
- **Story 0.2.3**: Application Routing
  - [ ] **Task FE-008**: Configure React Router (0.75d)
    - Setup SPA navigation
    - Protected routes with auth guards
    - Acceptance: Routing works, protected routes redirect unauthenticated users

  - [ ] **Task FE-009**: Create navigation component (1.00d)
    - Sidebar/navbar structure
    - Active route highlighting
    - Acceptance: Navigation renders with active states

#### Epic: Design System Foundation
- **Story 0.2.4**: UI Design System
  - [ ] **Task FE-010**: Setup CSS framework (1.08d)
    - Tailwind/MUI setup and theme configuration
    - Based on project preference
    - Acceptance: Styles apply correctly, theme variables work

  - [ ] **Task FE-011**: Create design tokens (0.75d)
    - Colors, typography, spacing
    - CSS custom properties / theme variables
    - Acceptance: Design tokens defined and usable

  - [ ] **Task FE-012**: Build Button component (0.50d)
    - Primary, secondary, disabled variants
    - Reusable across app
    - Acceptance: Button component renders all variants

  - [ ] **Task FE-013**: Build Input component (0.75d)
    - Text, number input types
    - Error/success validation states
    - Acceptance: Input component handles validation states

  - [ ] **Task FE-014**: Build Card component (0.50d)
    - Content container with shadow, border, padding variants
    - Acceptance: Card component displays content correctly

  - [ ] **Task FE-015**: Build Modal/Dialog component (1.00d)
    - Overlay with close handlers
    - Accessibility (keyboard navigation, focus trap)
    - Acceptance: Modal opens/closes, traps focus correctly

  - [ ] **Task FE-016**: Build Toast/Notification component (0.88d)
    - Success, error, info variants
    - Auto-dismiss functionality
    - Acceptance: Toasts display and auto-dismiss

  - [ ] **Task FE-017**: Build Loading/Spinner components (0.50d)
    - Full-page, inline, skeleton loaders
    - Acceptance: Loading states render correctly

#### Epic: Error Handling
- **Story 0.2.5**: Global Error Management
  - [ ] **Task FE-018**: Create error boundary component (0.75d)
    - Catch React errors globally
    - Fallback UI for crashes
    - Acceptance: Error boundary catches and displays errors

  - [ ] **Task FE-019**: Implement error logging (0.88d)
    - Console logging for development
    - Error tracking service integration (optional)
    - Acceptance: Errors log to console/tracking service

### 0.3 QA Infrastructure Setup

#### Epic: Test Environment Configuration
- **Story 0.3.1**: Test Environments
  - [ ] **Task QA-001**: Configure test environments (1.67d)
    - Polygon Amoy testnet access
    - Backend API test environment
    - Frontend test deployment
    - Acceptance: All test environments accessible

  - [ ] **Task QA-002**: Test data management (1.08d)
    - Generate test wallets
    - Fund test USDC on Polygon Amoy
    - Create test user accounts
    - Acceptance: Test data available for all scenarios

#### Epic: CI/CD Integration
- **Story 0.3.2**: Test Automation Pipeline
  - [ ] **Task QA-003**: Integrate Playwright, Cypress, K6 into CI/CD (2.00d)
    - GitHub Actions / Azure DevOps pipeline
    - Automated test runs on commit
    - Acceptance: Tests run automatically in CI/CD

  - [ ] **Task QA-004**: Configure test reporting (1.00d)
    - Allure or ReportPortal dashboards
    - Test result visualization
    - Acceptance: Test reports display pass/fail with details

#### Epic: Mock Services
- **Story 0.3.3**: Service Mocking
  - [ ] **Task QA-005**: Setup mocks for external services (1.58d)
    - WhiteBit API mock server
    - Fiat gateway mock server
    - Enable isolated testing without external dependencies
    - Acceptance: Tests run with mock services

### 0.4 Acceptance Criteria (Phase 0)
- ✅ Backend API runs locally and in dev environment
- ✅ PostgreSQL database connects and migrations execute
- ✅ Hashicorp Vault stores and retrieves secrets
- ✅ Frontend React app runs with routing and state management
- ✅ Design system components render correctly
- ✅ Test environments configured with CI/CD integration
- ✅ Mock services operational for testing

---

## 🏗️ Phase 1: Core Wallet Foundation

**Duration**: 4 weeks (Weeks 1-4, overlaps with Phase 0)
**Effort**: 88.29 days (BE: 29.00, FE: 19.21, QA: 21.08)
**Dependencies**: Circle SDK access, Polygon Amoy testnet, test USDC tokens
**Goal**: Enable passkey authentication, wallet creation, and gasless USDC transfers

### 1.1 Backend - Authentication & SDK Integration

#### Epic: Circle SDK Configuration
- **Story 1.1.1**: Circle SDK Setup
  - [ ] **Task BE-101**: Configure Circle Modular Wallets SDK in .NET (2.00d)
    - HTTP client wrapper for Circle APIs
    - Retry policies for network failures
    - Acceptance: SDK communicates with Circle services

  - [ ] **Task BE-102**: Environment configuration service (1.00d)
    - Load Circle client keys from Vault
    - Validate configuration on startup
    - Acceptance: App loads Circle keys from Vault successfully

#### Epic: Passkey Authentication APIs
- **Story 1.1.2**: Passkey Registration
  - [ ] **Task BE-103**: POST /api/auth/register endpoint (2.00d)
    - Initiate passkey registration flow
    - WebAuthn challenge generation
    - Link passkey credential to user account
    - Acceptance: Endpoint returns challenge, registers passkey

  - [ ] **Task BE-104**: POST /api/auth/verify endpoint (2.17d)
    - Verify passkey credential assertion
    - WebAuthn assertion validation
    - Create user session on success
    - Acceptance: Endpoint validates passkey and creates session

  - [ ] **Task BE-105**: Session management (2.00d)
    - JWT token generation
    - Refresh token handling
    - Optional Redis session storage
    - Acceptance: JWT tokens issued and validated

#### Epic: Smart Account & Wallet Management
- **Story 1.1.3**: Wallet Creation
  - [ ] **Task BE-106**: POST /api/wallet/create endpoint (3.08d)
    - Create Circle Smart Account with passkey owner
    - Deterministic address generation
    - Store wallet in database
    - Acceptance: Endpoint creates smart account and returns address

  - [ ] **Task BE-107**: Wallet repository (1.08d)
    - Data access layer for wallet CRUD operations
    - EF Core repository pattern
    - Acceptance: Repository performs CRUD on wallets

  - [ ] **Task BE-108**: GET /api/wallet/{address}/balance endpoint (2.00d)
    - Fetch USDC balance from Polygon RPC
    - Cache balance with TTL
    - Acceptance: Endpoint returns current USDC balance

#### Epic: USDC Transfer & Transaction Management
- **Story 1.1.4**: Gasless USDC Transfers
  - [ ] **Task BE-109**: POST /api/transactions/transfer endpoint (4.00d)
    - Encode transfer operation (recipient, token, amount)
    - Construct UserOperation with paymaster
    - Submit to Circle bundler
    - Acceptance: Endpoint submits gasless USDC transfer

  - [ ] **Task BE-110**: GET /api/transactions/{id}/status endpoint (2.00d)
    - Poll transaction status from bundler
    - Parse receipt for confirmation/failure
    - Update database with status
    - Acceptance: Endpoint returns transaction status

  - [ ] **Task BE-111**: Transaction repository (1.00d)
    - Data access for transaction records
    - Store userOpHash, txHash, status, timestamps
    - Acceptance: Repository stores transaction history

#### Epic: Blockchain Integration Services
- **Story 1.1.5**: Blockchain Services
  - [ ] **Task BE-112**: Blockchain RPC service (2.17d)
    - Service for interacting with Polygon Amoy testnet
    - Viem/Web3 .NET integration
    - Balance queries, transaction monitoring
    - Acceptance: Service reads blockchain state

  - [ ] **Task BE-113**: UserOperation service (3.17d)
    - Construct ERC-4337 UserOperations
    - Integrate with Circle bundler
    - Handle gas estimation, nonce management
    - Acceptance: Service submits UserOperations successfully

  - [ ] **Task BE-114**: Paymaster integration (2.17d)
    - Configure Circle paymaster for gas sponsorship
    - Include paymaster data in UserOperations
    - Acceptance: All transactions are gasless (0 gas paid by user)

### 1.2 Frontend - Authentication & Wallet UI

#### Epic: Passkey Registration Flow
- **Story 1.2.1**: Wallet Creation UI
  - [ ] **Task FE-101**: Create wallet creation landing page (1.00d)
    - Hero section with CTA
    - Feature highlights (gasless, secure, easy)
    - Acceptance: Landing page renders with create wallet CTA

  - [ ] **Task FE-102**: Build username input form (0.75d)
    - Input with regex validation
    - Error messages for invalid usernames
    - Acceptance: Form validates username correctly

  - [ ] **Task FE-103**: Integrate WebAuthn passkey creation (2.00d)
    - Circle SDK integration for passkey creation
    - Browser compatibility handling
    - Acceptance: Passkey created successfully in browser

  - [ ] **Task FE-104**: Handle registration success/failure states (1.00d)
    - Success: redirect to dashboard
    - Failure: display error message
    - Acceptance: User sees appropriate feedback

  - [ ] **Task FE-105**: Build passkey setup instructions (0.75d)
    - Multi-device setup guide
    - Security tips
    - Acceptance: Instructions display clearly

  - [ ] **Task FE-106**: Add loading states (0.50d)
    - Progress indicators during passkey creation
    - UX feedback during async operations
    - Acceptance: Loading states show during operations

#### Epic: Passkey Login Flow
- **Story 1.2.2**: Login UI
  - [ ] **Task FE-107**: Create login page (1.00d)
    - Passkey prompt UI
    - Fallback options if needed
    - Acceptance: Login page renders with passkey button

  - [ ] **Task FE-108**: Integrate WebAuthn authentication (1.58d)
    - Circle SDK login flow
    - Credential verification
    - Acceptance: User can log in with passkey

  - [ ] **Task FE-109**: Handle authentication success and session setup (1.00d)
    - Store auth token in secure storage
    - Redirect to dashboard
    - Acceptance: Authenticated users reach dashboard

  - [ ] **Task FE-110**: Build authentication error handling (0.88d)
    - Invalid passkey errors
    - Timeout scenarios
    - User cancellation handling
    - Acceptance: Errors display user-friendly messages

  - [ ] **Task FE-111**: Implement "Remember this device" (1.08d)
    - localStorage persistence
    - Session duration management
    - Acceptance: Device remembered across sessions

#### Epic: Wallet Dashboard
- **Story 1.2.3**: Dashboard UI
  - [ ] **Task FE-112**: Build dashboard layout (1.00d)
    - Sidebar, header, main content area
    - Responsive grid layout
    - Acceptance: Dashboard renders with navigation

  - [ ] **Task FE-113**: Create wallet address display (0.75d)
    - Truncated address with copy-to-clipboard
    - QR code display (optional)
    - Acceptance: Address displays and copies correctly

  - [ ] **Task FE-114**: Build USDC balance display (0.88d)
    - Format with 6 decimals correctly
    - Show USD equivalent (optional)
    - Acceptance: Balance displays formatted correctly

  - [ ] **Task FE-115**: Add balance refresh functionality (0.75d)
    - Manual refresh button
    - Auto-refresh timer (optional)
    - Acceptance: Balance updates on refresh

  - [ ] **Task FE-116**: Implement connection status indicator (0.50d)
    - Connected/disconnected visual state
    - Acceptance: Indicator shows connection status

#### Epic: USDC Transfer Form
- **Story 1.2.4**: Send Transaction UI
  - [ ] **Task FE-117**: Build recipient address input (1.00d)
    - Address format validation (Ethereum address)
    - ENS support planning (future)
    - Acceptance: Input validates Ethereum addresses

  - [ ] **Task FE-118**: Create amount input (0.75d)
    - Numeric validation
    - Max balance button
    - Insufficient balance check
    - Acceptance: Amount input validates correctly

  - [ ] **Task FE-119**: Display transaction preview (0.88d)
    - Recipient address
    - USDC amount
    - Gas fee (highlight: FREE)
    - Acceptance: Preview shows transaction details

  - [ ] **Task FE-120**: Integrate Circle SDK sendUserOperation (2.00d)
    - UserOperation construction
    - Paymaster integration
    - Acceptance: Transaction submits successfully

  - [ ] **Task FE-121**: Handle passkey signature request (1.00d)
    - Trigger WebAuthn for transaction signature
    - Handle user denial
    - Acceptance: User signs transaction with passkey

  - [ ] **Task FE-122**: Build transaction submission loading state (0.75d)
    - Progress bar
    - Estimated time display
    - Acceptance: Loading state shows during submission

  - [ ] **Task FE-123**: Implement success/failure notifications (0.88d)
    - Toast messages
    - Transaction hash display
    - Link to block explorer (optional)
    - Acceptance: Notifications show transaction outcome

  - [ ] **Task FE-124**: Add form validation (0.75d)
    - Required field validation
    - Format validation
    - Error message display
    - Acceptance: Form validates all inputs

### 1.3 QA - Core Wallet Testing

#### Epic: Passkey Authentication Testing
- **Story 1.3.1**: Passkey Registration Testing
  - [ ] **Task QA-101**: Manual cross-browser testing (1.50d)
    - Test on Chrome, Firefox, Edge, Safari
    - Verify passkey creation works on all browsers
    - Acceptance: Passkey registers successfully on all major browsers

  - [ ] **Task QA-102**: Playwright automation for passkey registration (2.67d)
    - WebAuthn simulation in Playwright
    - Automate registration flow
    - Acceptance: Automated test passes for registration

  - [ ] **Task QA-103**: Manual passkey login testing (1.00d)
    - Test login flow
    - Error scenarios (invalid passkey, timeout)
    - Acceptance: Login works correctly, errors handled

  - [ ] **Task QA-104**: Playwright automation for passkey login (1.58d)
    - Automate login flow
    - Reuse patterns from registration tests
    - Acceptance: Automated test passes for login

  - [ ] **Task QA-105**: Passkey recovery testing (1.00d)
    - Test recovery instructions display
    - Multi-device setup flow
    - Acceptance: Recovery instructions clear and accurate

  - [ ] **Task QA-106**: Passkey security validation (1.50d)
    - Validate passkeys never leave device
    - Verify domain binding
    - Acceptance: Security audit passes

#### Epic: Smart Account & Wallet Testing
- **Story 1.3.2**: Wallet Creation Testing
  - [ ] **Task QA-107**: Functional wallet creation testing (1.50d)
    - Test wallet address generation
    - Verify deterministic behavior
    - Validate contract deployment on first transaction
    - Acceptance: Wallet creates successfully with correct address

  - [ ] **Task QA-108**: Cypress E2E for wallet creation (1.50d)
    - End-to-end automated test
    - Registration → wallet creation → dashboard
    - Acceptance: E2E test passes for complete flow

  - [ ] **Task QA-109**: Account properties validation (1.00d)
    - Verify single signer
    - Correct chain ID (Polygon Amoy)
    - Address format validation
    - Acceptance: All properties match specification

#### Epic: USDC Transfer Testing
- **Story 1.3.3**: Gasless Transfer Testing
  - [ ] **Task QA-110**: Happy path transfer testing (1.50d)
    - Test successful USDC transfer with paymaster
    - Requires test USDC on Polygon Amoy
    - Acceptance: Transfer completes successfully with 0 gas

  - [ ] **Task QA-111**: Error scenario testing (1.58d)
    - Insufficient balance
    - Invalid recipient address
    - Network errors
    - Acceptance: All error cases handled gracefully

  - [ ] **Task QA-112**: Playwright automation for transfers (2.08d)
    - Automate happy path
    - Automate error scenarios
    - Acceptance: Automated tests cover transfer flows

  - [ ] **Task QA-113**: UserOperation validation (1.50d)
    - Verify UserOperation construction
    - Validate paymaster inclusion
    - Acceptance: Technical validation of bundler integration

  - [ ] **Task QA-114**: Gas sponsorship verification (1.00d)
    - Validate 100% gas sponsorship
    - User pays 0 gas for all transactions
    - Acceptance: Critical business requirement validated

#### Epic: Transaction Status Testing
- **Story 1.3.4**: Transaction Monitoring Testing
  - [ ] **Task QA-115**: Status tracking testing (1.50d)
    - Test pending → confirmed → success flow
    - Test error handling
    - Acceptance: State machine validates correctly

  - [ ] **Task QA-116**: Receipt parsing validation (1.00d)
    - Validate receipt data accuracy
    - Hash consistency checks
    - Acceptance: Data integrity validated

  - [ ] **Task QA-117**: Failure handling testing (1.50d)
    - Test graceful failure handling
    - User feedback validation
    - Acceptance: UX validates for error scenarios

  - [ ] **Task QA-118**: Phase 1 regression testing (2.00d)
    - Execute full regression suite
    - Assumes 2-3 regression cycles
    - Acceptance: All Phase 1 features pass regression

### 1.4 Acceptance Criteria (Phase 1)
- ✅ Users can register and login with passkeys
- ✅ Smart account wallets created with deterministic addresses
- ✅ Gasless USDC transfers work on Polygon Amoy (0 gas paid by user)
- ✅ Transaction status tracked (pending → confirmed/failed)
- ✅ Wallet dashboard displays address and balance
- ✅ 70%+ passkey authentication success rate
- ✅ 90%+ transaction success rate
- ✅ <45 seconds average transaction confirmation time

---

## 📊 Phase 2: Transaction Management

**Duration**: 3-4 weeks (Weeks 3-6, overlaps with Phase 1)
**Effort**: 36.08 days (BE: 13.17, FE: 11.75, QA: 11.25)
**Dependencies**: Phase 1 complete (transaction infrastructure)
**Goal**: Enable transaction history, filtering, and real-time status updates

### 2.1 Backend - Transaction History APIs

#### Epic: Transaction History
- **Story 2.1.1**: Transaction History API
  - [ ] **Task BE-201**: GET /api/transactions endpoint (2.00d)
    - Fetch last 20 transactions for user
    - Support pagination
    - Filter by status (pending, confirmed, failed)
    - Acceptance: Endpoint returns paginated transaction list

  - [ ] **Task BE-202**: GET /api/transactions/{id} endpoint (1.08d)
    - Get detailed transaction information
    - Include blockchain explorer link
    - Acceptance: Endpoint returns full transaction details

#### Epic: Transaction Monitoring
- **Story 2.1.2**: Real-time Transaction Monitoring
  - [ ] **Task BE-203**: Background monitoring service (3.17d)
    - Hosted service to monitor pending transactions
    - Periodic polling of transaction status
    - Update database on confirmation/failure
    - Acceptance: Service polls and updates transaction status

  - [ ] **Task BE-204**: POST /api/webhooks/transactions endpoint (2.00d)
    - Receive Circle webhook notifications (if supported)
    - Update transaction status on webhook
    - Acceptance: Webhook handler updates transaction status

  - [ ] **Task BE-205**: Retry logic and error handling (2.17d)
    - Retry failed operations with exponential backoff
    - Circuit breaker pattern for external services
    - Acceptance: Failed operations retry automatically

#### Epic: Balance Management
- **Story 2.1.3**: Balance Refresh Service
  - [ ] **Task BE-206**: Balance refresh service (1.67d)
    - Periodic balance updates
    - Cache with TTL (time-to-live)
    - Invalidate cache on transaction completion
    - Acceptance: Balances refresh periodically and on demand

### 2.2 Frontend - Transaction History UI

#### Epic: Transaction List Component
- **Story 2.2.1**: Transaction History Display
  - [ ] **Task FE-201**: Create transaction list component (1.00d)
    - Table or card view layout
    - Responsive design (desktop/mobile)
    - Acceptance: Transaction list renders correctly

  - [ ] **Task FE-202**: Build transaction item card (1.00d)
    - Display from/to addresses
    - Amount with token symbol
    - Status badge (pending, confirmed, failed)
    - Timestamp formatting
    - Acceptance: Transaction card displays all details

  - [ ] **Task FE-203**: Implement status indicators (0.75d)
    - Color-coded badges
    - Icons for each status
    - Acceptance: Status visually distinguishable

  - [ ] **Task FE-204**: Add timestamp formatting (0.50d)
    - Relative time ("2 hours ago")
    - Full date on hover
    - Acceptance: Timestamps formatted correctly

  - [ ] **Task FE-205**: Create transaction detail modal (1.00d)
    - Full transaction details
    - Block explorer link
    - Acceptance: Modal displays complete transaction info

#### Epic: Transaction History API Integration
- **Story 2.2.2**: API Integration
  - [ ] **Task FE-206**: Integrate API for fetching transactions (1.08d)
    - Fetch last 20 transactions
    - Setup pagination
    - Loading states
    - Acceptance: Transactions load from API

  - [ ] **Task FE-207**: Build filter UI (0.88d)
    - Dropdown or tabs for status filtering
    - Filter by: All, Pending, Confirmed, Failed
    - Acceptance: Filter updates displayed transactions

  - [ ] **Task FE-208**: Implement empty state (0.50d)
    - Illustration for no transactions
    - Call-to-action to make first transaction
    - Acceptance: Empty state displays when no transactions

#### Epic: Real-time Transaction Updates
- **Story 2.2.3**: Real-time Monitoring UI
  - [ ] **Task FE-209**: Setup polling mechanism (1.08d)
    - Poll transaction status every 5-10 seconds
    - Stop polling on confirmation/failure
    - Acceptance: Transaction status updates without manual refresh

  - [ ] **Task FE-210**: Display transaction progress indicator (0.88d)
    - Estimated confirmation time
    - Progress steps (submitted → bundled → confirmed)
    - Acceptance: Progress indicator shows transaction stages

  - [ ] **Task FE-211**: Handle confirmation updates (0.75d)
    - Update status in UI when confirmed
    - Show success notification
    - Acceptance: UI updates on transaction confirmation

  - [ ] **Task FE-212**: Implement failure handling UI (1.00d)
    - Error messages for failed transactions
    - Retry option (if applicable)
    - Acceptance: Failed transactions show error details

#### Epic: Balance Display
- **Story 2.2.4**: Balance Management UI
  - [ ] **Task FE-213**: Create balance card component (0.75d)
    - Display USDC amount
    - USD equivalent (optional)
    - Acceptance: Balance card renders correctly

  - [ ] **Task FE-214**: Add manual refresh button (0.50d)
    - Refresh button with loading spinner
    - Cooldown timer to prevent spam
    - Acceptance: Balance refreshes on button click

  - [ ] **Task FE-215**: Implement auto-refresh on transaction completion (0.75d)
    - Listen to transaction completion events
    - Refresh balance automatically
    - Acceptance: Balance updates after transactions

  - [ ] **Task FE-216**: Display last updated timestamp (0.50d)
    - Show when balance was last refreshed
    - Relative time format
    - Acceptance: Timestamp displays correctly

### 2.3 QA - Transaction Management Testing

#### Epic: Transaction History Testing
- **Story 2.3.1**: History Display Testing
  - [ ] **Task QA-201**: UI validation testing (1.00d)
    - Test transaction list display
    - Pagination functionality
    - Acceptance: UI displays transactions correctly

  - [ ] **Task QA-202**: Playwright automation for history UI (1.50d)
    - Automate transaction list tests
    - Test data generation required
    - Acceptance: Automated tests validate UI

  - [ ] **Task QA-203**: Filter testing (1.00d)
    - Test filtering by status
    - Validate filter logic
    - Acceptance: Filters work correctly

  - [ ] **Task QA-204**: Data accuracy validation (1.50d)
    - Verify recipient, amount, status, timestamp accuracy
    - Cross-reference with blockchain data
    - Acceptance: All data matches blockchain

#### Epic: Real-time Updates Testing
- **Story 2.3.2**: Real-time Monitoring Testing
  - [ ] **Task QA-205**: Real-time updates testing (1.58d)
    - Test transaction status updates without refresh
    - Validate WebSocket or polling behavior
    - Acceptance: Status updates automatically

#### Epic: Balance Testing
- **Story 2.3.3**: Balance Display Testing
  - [ ] **Task QA-206**: Balance accuracy testing (1.00d)
    - Validate USDC balance accuracy
    - Test refresh behavior
    - Acceptance: Balance matches blockchain state

  - [ ] **Task QA-207**: Dashboard UI testing (1.00d)
    - Test overall dashboard layout
    - Responsive testing (desktop/tablet/mobile)
    - Acceptance: Dashboard works on all devices

#### Epic: Error Handling Testing
- **Story 2.3.4**: Error Validation
  - [ ] **Task QA-208**: Error message validation (1.50d)
    - Test all error messages are clear and actionable
    - UX-focused testing
    - Acceptance: Error messages user-friendly

  - [ ] **Task QA-209**: User feedback testing (1.00d)
    - Test loading states
    - Success/failure notifications
    - Acceptance: UI state management correct

  - [ ] **Task QA-210**: Phase 2 regression testing (1.08d)
    - Execute Phase 2 regression suite
    - Lighter regression than Phase 1
    - Acceptance: All Phase 2 features pass regression

### 2.4 Acceptance Criteria (Phase 2)
- ✅ Transaction history displays last 20 transactions
- ✅ Transactions filterable by status
- ✅ Real-time status updates without page refresh
- ✅ Balance refreshes automatically after transactions
- ✅ Clear error messages for all failure scenarios
- ✅ Dashboard responsive on desktop, tablet, mobile

---

## 💰 Phase 3: Fiat Off-Ramp

**Duration**: 4-5 weeks (Weeks 5-9)
**Effort**: 78.50 days (BE: 33.50, FE: 18.92, QA: 26.08)
**Dependencies**: Fiat gateway provider API access, bank validation services
**Goal**: Enable crypto-to-fiat conversion and bank payouts
**⚠️ NOTE**: Marked as "deferred to post-MVP" in PRD - confirm with stakeholders before implementation

### 3.1 Backend - Fiat Gateway Integration

#### Epic: Bank Account Management
- **Story 3.1.1**: Bank Account APIs
  - [ ] **Task BE-301**: POST /api/bank-accounts endpoint (3.08d)
    - Add encrypted bank account
    - Field-level encryption for sensitive data
    - Validation of required fields
    - Acceptance: Endpoint stores encrypted bank account

  - [ ] **Task BE-302**: Bank account validation service (2.00d)
    - Validate routing number format (US ACH)
    - Validate account number format
    - Acceptance: Invalid bank details rejected

  - [ ] **Task BE-303**: GET /api/bank-accounts endpoint (1.00d)
    - List user's bank accounts
    - Decrypt for display (masked)
    - Acceptance: Endpoint returns user's bank accounts

  - [ ] **Task BE-304**: DELETE /api/bank-accounts/{id} endpoint (1.00d)
    - Remove bank account (soft delete preferred)
    - Acceptance: Endpoint deletes bank account

#### Epic: Fiat Gateway Integration
- **Story 3.1.2**: Gateway Client Setup
  - [ ] **Task BE-305**: Fiat gateway HTTP client (4.00d)
    - HTTP client for RedotPay or alternative provider
    - API discovery and authentication
    - Acceptance: Client communicates with fiat gateway

  - [ ] **Task BE-306**: Exchange rate service (2.00d)
    - Real-time USDC/USD rate fetching
    - 30-second rate lock mechanism
    - Cache with short TTL
    - Acceptance: Service provides current exchange rates

  - [ ] **Task BE-307**: Fee calculation service (1.08d)
    - Calculate conversion fees (1-2%)
    - Configurable fee structure
    - Acceptance: Fees calculated correctly

#### Epic: Payout Execution
- **Story 3.1.3**: Payout APIs
  - [ ] **Task BE-308**: POST /api/payouts/preview endpoint (2.00d)
    - Calculate USD amount and fees
    - No database write (preview only)
    - Acceptance: Endpoint returns payout preview

  - [ ] **Task BE-309**: POST /api/payouts/execute endpoint (4.00d)
    - Submit payout to gateway
    - Transaction atomicity critical
    - Deduct USDC from wallet
    - Acceptance: Endpoint executes payout successfully

  - [ ] **Task BE-310**: Payout status tracking service (3.17d)
    - Background service to poll gateway for status
    - Update database on status changes
    - Acceptance: Service tracks payout status

  - [ ] **Task BE-311**: GET /api/payouts endpoint (2.00d)
    - List fiat payout transactions
    - Filter by status, pagination
    - Acceptance: Endpoint returns payout history

  - [ ] **Task BE-312**: Payout repository (1.00d)
    - Data access layer for fiat payout records
    - CRUD operations
    - Acceptance: Repository stores payout records

  - [ ] **Task BE-313**: POST /api/webhooks/payouts endpoint (2.17d)
    - Receive gateway status updates
    - Signature verification for security
    - Acceptance: Webhook updates payout status

  - [ ] **Task BE-314**: Payout notification service (1.67d)
    - Send notifications on payout completion
    - Email or push notification integration
    - Acceptance: Users notified on payout completion

### 3.2 Frontend - Fiat Off-Ramp UI

#### Epic: Bank Account Management UI
- **Story 3.2.1**: Bank Account UI
  - [ ] **Task FE-301**: Create bank account list/empty state (0.75d)
    - Display single account (MVP limit)
    - Empty state with add account CTA
    - Acceptance: Bank account list renders

  - [ ] **Task FE-302**: Build add bank account form (1.00d)
    - Account holder name, routing, account number inputs
    - Form validation
    - Acceptance: Form validates and submits bank details

  - [ ] **Task FE-303**: Implement form validation (1.00d)
    - US routing number validation
    - Account number format validation
    - Error messages
    - Acceptance: Form rejects invalid bank details

  - [ ] **Task FE-304**: Add bank account display (0.75d)
    - Show masked account details (last 4 digits)
    - Bank name display
    - Acceptance: Account displays securely

  - [ ] **Task FE-305**: Create verification status indicator (0.50d)
    - Verified badge
    - Pending verification state
    - Acceptance: Status displays correctly

  - [ ] **Task FE-306**: Integrate API for saving bank details (1.08d)
    - POST request with encrypted data
    - Error handling
    - Acceptance: Bank account saved via API

#### Epic: Crypto-to-Fiat Conversion UI
- **Story 3.2.2**: Conversion Calculator
  - [ ] **Task FE-307**: Build conversion calculator component (1.00d)
    - USDC input field
    - USD output (calculated in real-time)
    - Acceptance: Calculator updates USD amount dynamically

  - [ ] **Task FE-308**: Display real-time exchange rate (0.88d)
    - Fetch from API every 30 seconds
    - Display USDC/USD rate prominently
    - Acceptance: Rate updates in real-time

  - [ ] **Task FE-309**: Show conversion fee breakdown (0.75d)
    - Display percentage and dollar amount
    - Fee transparency
    - Acceptance: Fees shown clearly

  - [ ] **Task FE-310**: Implement rate lock countdown timer (1.00d)
    - 30-second countdown visual indicator
    - Lock rate during countdown
    - Acceptance: Timer counts down, rate locked

  - [ ] **Task FE-311**: Handle rate expiration (0.88d)
    - Auto-refresh on expiration
    - Manual refresh option
    - Acceptance: Expired rate refreshes

#### Epic: Payout Execution UI
- **Story 3.2.3**: Payout Flow UI
  - [ ] **Task FE-312**: Create payout form (1.00d)
    - USDC amount input
    - Validate against wallet balance
    - Acceptance: Form validates USDC amount

  - [ ] **Task FE-313**: Build bank account selection dropdown (0.50d)
    - Single account for MVP
    - Link to add account if none
    - Acceptance: Dropdown selects bank account

  - [ ] **Task FE-314**: Display payout summary preview (1.00d)
    - USDC sent, USD received, fees, rate
    - Settlement time estimate
    - Acceptance: Summary displays all payout details

  - [ ] **Task FE-315**: Integrate payout submission API (1.08d)
    - POST request to execute payout
    - Handle response
    - Acceptance: Payout submits successfully

  - [ ] **Task FE-316**: Add passkey confirmation (0.88d)
    - WebAuthn signature for payout
    - User confirmation prompt
    - Acceptance: Payout requires passkey signature

  - [ ] **Task FE-317**: Build payout loading state (0.75d)
    - Progress indicator
    - Estimated processing time
    - Acceptance: Loading state displays during payout

  - [ ] **Task FE-318**: Implement success notification (0.75d)
    - Toast message on success
    - Redirect to payout history
    - Acceptance: User notified of successful payout

  - [ ] **Task FE-319**: Handle payout errors (1.00d)
    - Insufficient balance errors
    - API error handling
    - User-friendly feedback
    - Acceptance: Errors display clearly

#### Epic: Payout History UI
- **Story 3.2.4**: Payout History Display
  - [ ] **Task FE-320**: Create payout history list component (1.00d)
    - Table or card layout
    - Responsive design
    - Acceptance: Payout history displays

  - [ ] **Task FE-321**: Build payout transaction card (1.00d)
    - USDC/USD amounts, rate, fee, status, date
    - Status badge
    - Acceptance: Payout card shows all details

  - [ ] **Task FE-322**: Add status filter (0.88d)
    - Dropdown or tabs
    - Filter: pending, processing, completed, failed
    - Acceptance: Filter updates displayed payouts

  - [ ] **Task FE-323**: Implement payout detail modal (1.00d)
    - Full transaction details
    - Timestamps, confirmation info
    - Acceptance: Modal displays complete payout info

  - [ ] **Task FE-324**: Integrate payout history API (1.08d)
    - Fetch payout history
    - Pagination, loading states
    - Acceptance: History loads from API

  - [ ] **Task FE-325**: Build empty state (0.50d)
    - Illustration for no payouts
    - CTA to create first payout
    - Acceptance: Empty state displays

### 3.3 QA - Fiat Off-Ramp Testing

#### Epic: Bank Account Testing
- **Story 3.3.1**: Bank Account Management Testing
  - [ ] **Task QA-301**: Bank account addition testing (1.50d)
    - Test adding US bank account
    - Validation rules testing
    - MVP: 1 account per user
    - Acceptance: Bank account adds successfully

  - [ ] **Task QA-302**: Playwright automation for bank account (1.50d)
    - Automate form validation
    - Automate bank account addition
    - Acceptance: Automated tests pass

  - [ ] **Task QA-303**: Storage security testing (2.00d)
    - Verify encryption of account/routing numbers
    - Security audit
    - Acceptance: Bank details encrypted at rest

  - [ ] **Task QA-304**: Bank detail validation testing (1.00d)
    - Test routing number format validation
    - Account number validation
    - Edge cases (invalid formats)
    - Acceptance: Validation rejects invalid data

#### Epic: Conversion Testing
- **Story 3.3.2**: Crypto-to-Fiat Conversion Testing
  - [ ] **Task QA-305**: Exchange rate testing (1.00d)
    - Test real-time USDC/USD rate accuracy
    - Integration with price feed
    - Acceptance: Rates accurate and up-to-date

  - [ ] **Task QA-306**: Conversion calculation testing (1.50d)
    - Validate recipient amount calculation
    - Fee deduction accuracy
    - Financial calculation precision
    - Acceptance: Calculations correct to 2 decimal places

  - [ ] **Task QA-307**: Fee transparency testing (1.00d)
    - Verify 1-2% fee display
    - Fee calculation accuracy
    - Acceptance: Fees calculated and displayed correctly

  - [ ] **Task QA-308**: Rate lock mechanism testing (1.58d)
    - Test 30-second rate lock
    - Time-based testing
    - Acceptance: Rate locked for 30 seconds

#### Epic: Payout Execution Testing
- **Story 3.3.3**: Payout Flow Testing
  - [ ] **Task QA-309**: Happy path payout testing (2.08d)
    - Manual test of successful USDC → USD payout
    - Requires fiat gateway integration
    - Acceptance: Payout completes successfully

  - [ ] **Task QA-310**: Payout error scenario testing (2.00d)
    - Test insufficient balance
    - Invalid bank account
    - Gateway errors
    - Acceptance: Errors handled gracefully

  - [ ] **Task QA-311**: Payout status tracking testing (1.58d)
    - Test pending → processing → completed flow
    - Async flow validation
    - Acceptance: Status transitions correctly

  - [ ] **Task QA-312**: Payout notification testing (1.00d)
    - Verify user notifications on completion/failure
    - Email or push notification
    - Acceptance: Users receive notifications

  - [ ] **Task QA-313**: Cypress E2E for payout (3.08d)
    - End-to-end automation
    - Complete payout flow test
    - Complex integration test
    - Acceptance: E2E test passes

#### Epic: Gateway Integration Testing
- **Story 3.3.4**: Fiat Gateway Testing
  - [ ] **Task QA-314**: Gateway API integration testing (2.08d)
    - Test RedotPay API calls
    - Error responses
    - Depends on gateway availability
    - Acceptance: Gateway integration works

  - [ ] **Task QA-315**: Retry mechanism testing (1.50d)
    - Validate retry logic for failed gateway calls
    - Resilience testing
    - Acceptance: Failed operations retry automatically

#### Epic: Payout History Testing
- **Story 3.3.5**: History Display Testing
  - [ ] **Task QA-316**: History UI testing (1.00d)
    - Test payout history display
    - Data accuracy
    - Acceptance: History displays correctly

  - [ ] **Task QA-317**: History filtering testing (1.00d)
    - Test status filter functionality
    - Acceptance: Filter works correctly

#### Epic: Performance Testing
- **Story 3.3.6**: Payout Performance
  - [ ] **Task QA-318**: Performance testing (1.50d)
    - Test 80%+ completion within 24 hours (monitoring)
    - NFR validation
    - Acceptance: Performance meets target

  - [ ] **Task QA-319**: Phase 3 regression testing (2.00d)
    - Execute Phase 3 regression suite
    - Critical financial flow testing
    - Acceptance: All Phase 3 features pass regression

### 3.4 Acceptance Criteria (Phase 3)
- ✅ Users can add one bank account (US only)
- ✅ Real-time USDC/USD exchange rates display
- ✅ Conversion fees transparent (1-2%)
- ✅ Rate locked for 30 seconds
- ✅ Payout executes to bank account
- ✅ Payout status tracked (pending → processing → completed)
- ✅ Notifications sent on payout completion
- ✅ 80%+ fiat payouts complete within 24 hours
- ✅ All bank details encrypted at rest

---

## 🏦 Phase 4: Exchange Investment

**Duration**: 5-6 weeks (Weeks 7-12)
**Effort**: 102.17 days (BE: 43.67, FE: 21.58, QA: 36.92)
**Dependencies**: WhiteBit API access, WhiteBit sandbox environment
**Goal**: Enable stablecoin yield generation via WhiteBit Flex
**⚠️ CRITICAL**: Highest complexity phase - external API dependency, multi-step USDC transfers, custody concerns

### 4.1 Backend - WhiteBit Integration

#### Epic: WhiteBit API Client
- **Story 4.1.1**: Exchange Connection
  - [ ] **Task BE-401**: WhiteBit API HTTP client (4.00d)
    - HTTP client for WhiteBit Flex Investment API
    - Authentication with API key/secret
    - Rate limiting handling
    - Acceptance: Client communicates with WhiteBit API

  - [ ] **Task BE-402**: POST /api/exchanges/connect endpoint (3.08d)
    - Store encrypted API credentials
    - Encrypt API key and secret using Vault
    - Acceptance: Endpoint stores WhiteBit credentials

  - [ ] **Task BE-403**: Validate exchange credentials (2.00d)
    - Test API call to validate credentials on connection
    - Return connection status
    - Acceptance: Invalid credentials rejected

#### Epic: Investment Plans
- **Story 4.1.2**: Investment Plan APIs
  - [ ] **Task BE-404**: GET /api/investments/plans endpoint (2.17d)
    - Fetch USDC Flex plans from WhiteBit
    - Cache plans with TTL
    - Display APY, minimum amount, plan type
    - Acceptance: Endpoint returns investment plans

  - [ ] **Task BE-405**: POST /api/investments/preview endpoint (2.00d)
    - Calculate estimated yields (daily/monthly/yearly)
    - Projection calculations
    - Acceptance: Endpoint returns yield estimates

#### Epic: Investment Creation
- **Story 4.1.3**: Create Investment Flow
  - [ ] **Task BE-406**: POST /api/investments/create endpoint (5.17d)
    - Transfer USDC from Circle wallet to WhiteBit
    - Create investment position via WhiteBit API
    - Multi-step transaction (critical)
    - Acceptance: Investment created successfully

  - [ ] **Task BE-407**: USDC transfer to WhiteBit service (3.17d)
    - Service to transfer USDC to WhiteBit deposit address
    - Deposit address management
    - Transaction monitoring
    - Acceptance: USDC transferred to WhiteBit

  - [ ] **Task BE-408**: Investment position repository (1.00d)
    - Data access for investment positions
    - CRUD operations
    - Acceptance: Repository stores investment positions

#### Epic: Investment Position Management
- **Story 4.1.4**: Position Tracking
  - [ ] **Task BE-409**: GET /api/investments/positions endpoint (2.00d)
    - List active investments
    - Calculate current value and rewards
    - Acceptance: Endpoint returns investment positions

  - [ ] **Task BE-410**: Investment sync service (4.00d)
    - Background service to sync position data from WhiteBit
    - Periodic refresh of APY, rewards, current value
    - Acceptance: Service syncs investment data

  - [ ] **Task BE-411**: GET /api/investments/summary endpoint (1.08d)
    - Total portfolio value aggregation
    - Sum all active positions
    - Acceptance: Endpoint returns portfolio summary

#### Epic: Investment Withdrawal
- **Story 4.1.5**: Withdraw Investment Flow
  - [ ] **Task BE-412**: POST /api/investments/{id}/withdraw endpoint (4.00d)
    - Full withdrawal (partial not supported in MVP)
    - Submit withdrawal request to WhiteBit
    - Multi-step: API call + transfer back
    - Acceptance: Investment withdrawn successfully

  - [ ] **Task BE-413**: USDC transfer from WhiteBit service (3.17d)
    - Receive USDC back from WhiteBit to Circle wallet
    - Withdrawal address validation
    - Acceptance: USDC received from WhiteBit

  - [ ] **Task BE-414**: Investment status tracking (2.00d)
    - Track lifecycle states (pending, active, withdrawn)
    - State machine implementation
    - Acceptance: Status transitions correctly

### 4.2 Frontend - Investment UI

#### Epic: WhiteBit Connection UI
- **Story 4.2.1**: Exchange Connection UI
  - [ ] **Task FE-401**: Create WhiteBit connection form (1.00d)
    - API key and secret input fields
    - Password input type for secret
    - Acceptance: Connection form renders

  - [ ] **Task FE-402**: Add connection instructions (0.75d)
    - Link to WhiteBit API docs
    - Screenshots or step-by-step guide
    - Acceptance: Instructions clear and helpful

  - [ ] **Task FE-403**: Implement credential validation (1.08d)
    - Test API call on form submission
    - Verify credentials before saving
    - Acceptance: Invalid credentials rejected

  - [ ] **Task FE-404**: Build connection status indicator (0.50d)
    - Connected/disconnected badge
    - Visual indicator
    - Acceptance: Status displays correctly

  - [ ] **Task FE-405**: Display last used timestamp (0.50d)
    - Show when credentials last used
    - Relative time format
    - Acceptance: Timestamp displays correctly

  - [ ] **Task FE-406**: Handle connection errors (1.00d)
    - Invalid credentials errors
    - Network error handling
    - User-friendly error messages
    - Acceptance: Errors display clearly

#### Epic: Investment Plan Display
- **Story 4.2.2**: Investment Plans UI
  - [ ] **Task FE-407**: Fetch and display USDC Flex plans (1.08d)
    - API integration
    - Loading state
    - Acceptance: Plans load from API

  - [ ] **Task FE-408**: Build investment plan card component (1.00d)
    - APY, minimum amount, plan type, terms
    - Visual card layout
    - Acceptance: Plan card displays all details

  - [ ] **Task FE-409**: Add plan details modal (0.88d)
    - Full terms and conditions
    - Risk warnings
    - Acceptance: Modal displays plan details

  - [ ] **Task FE-410**: Highlight recommended plan (0.50d)
    - Badge or visual distinction
    - If applicable
    - Acceptance: Recommended plan highlighted

#### Epic: Create Investment UI
- **Story 4.2.3**: Investment Creation Flow
  - [ ] **Task FE-411**: Build investment amount input form (1.00d)
    - Validate against balance and minimums
    - Max button
    - Acceptance: Form validates investment amount

  - [ ] **Task FE-412**: Add investment calculator (1.00d)
    - Estimated daily/monthly/yearly earnings projection
    - Real-time calculation
    - Acceptance: Calculator shows yield estimates

  - [ ] **Task FE-413**: Display investment preview (1.00d)
    - Amount, APY, estimated returns, terms
    - Summary before confirmation
    - Acceptance: Preview displays all investment details

  - [ ] **Task FE-414**: Integrate create investment API (1.08d)
    - POST request with investment data
    - USDC transfer + WhiteBit API call
    - Acceptance: Investment created via API

  - [ ] **Task FE-415**: Add passkey confirmation (0.88d)
    - WebAuthn signature for investment
    - User confirmation
    - Acceptance: Investment requires passkey

  - [ ] **Task FE-416**: Build investment creation loading state (0.75d)
    - Progress steps (transfer → create → confirm)
    - Estimated time
    - Acceptance: Loading shows multi-step progress

  - [ ] **Task FE-417**: Implement success notification (0.75d)
    - Toast message
    - Navigate to positions
    - Acceptance: User notified of successful investment

  - [ ] **Task FE-418**: Handle investment errors (1.00d)
    - Insufficient balance errors
    - API errors
    - User feedback
    - Acceptance: Errors handled gracefully

#### Epic: Investment Position View
- **Story 4.2.4**: Position Dashboard
  - [ ] **Task FE-419**: Create positions list/dashboard (1.00d)
    - Grid or table layout
    - Summary cards for each position
    - Acceptance: Positions display correctly

  - [ ] **Task FE-420**: Build position card component (1.08d)
    - Principal, current value, rewards, APY, status
    - Visual progress indicators
    - Acceptance: Position card shows all details

  - [ ] **Task FE-421**: Display total portfolio value (0.75d)
    - Sum of all positions
    - Percentage gains
    - Acceptance: Portfolio value calculated correctly

  - [ ] **Task FE-422**: Add accrued rewards calculation (1.00d)
    - Real-time or periodic updates
    - Display rewards earned
    - Acceptance: Rewards display accurately

  - [ ] **Task FE-423**: Implement position detail modal (1.00d)
    - Full position history
    - Start date, terms, withdraw button
    - Acceptance: Modal shows complete position info

  - [ ] **Task FE-424**: Integrate positions API (1.08d)
    - Fetch active positions
    - Loading states
    - Acceptance: Positions load from API

  - [ ] **Task FE-425**: Build empty state (0.50d)
    - Illustration for no positions
    - CTA to create first investment
    - Acceptance: Empty state displays

#### Epic: Withdraw Investment UI
- **Story 4.2.5**: Withdrawal Flow
  - [ ] **Task FE-426**: Build withdrawal confirmation modal (1.00d)
    - Amount preview
    - Processing time estimate
    - Risk warnings
    - Acceptance: Confirmation modal displays

  - [ ] **Task FE-427**: Display withdrawal preview (0.75d)
    - Final amount (principal + rewards)
    - Settlement time
    - Acceptance: Preview shows withdrawal details

  - [ ] **Task FE-428**: Integrate withdraw investment API (1.08d)
    - POST request to withdraw
    - WhiteBit withdrawal + transfer back
    - Acceptance: Withdrawal submits via API

  - [ ] **Task FE-429**: Add passkey confirmation (0.88d)
    - WebAuthn signature for withdrawal
    - Acceptance: Withdrawal requires passkey

  - [ ] **Task FE-430**: Build withdrawal loading state (0.75d)
    - Progress indicator
    - Multi-step process
    - Acceptance: Loading shows withdrawal progress

  - [ ] **Task FE-431**: Implement success notification (0.75d)
    - Toast message
    - Update position status
    - Acceptance: User notified of successful withdrawal

  - [ ] **Task FE-432**: Handle withdrawal errors (1.00d)
    - API errors
    - Insufficient liquidity
    - User feedback
    - Acceptance: Errors display clearly

### 4.3 QA - Investment Testing

#### Epic: Exchange Connection Testing
- **Story 4.3.1**: WhiteBit Connection Testing
  - [ ] **Task QA-401**: Connection functional testing (1.50d)
    - Test API credential connection
    - Validation testing
    - Acceptance: Connection works with valid credentials

  - [ ] **Task QA-402**: Playwright automation for connection (1.50d)
    - Automate connection flow
    - API credential UI testing
    - Acceptance: Automated tests pass

  - [ ] **Task QA-403**: Credential storage security (2.00d)
    - Verify encryption of API key/secret
    - Security audit
    - Acceptance: Credentials encrypted at rest

  - [ ] **Task QA-404**: Connection status testing (1.00d)
    - Test active/inactive status display
    - UI validation
    - Acceptance: Status displays correctly

#### Epic: Investment Plan Testing
- **Story 4.3.2**: Plan Display Testing
  - [ ] **Task QA-405**: Plan retrieval testing (1.50d)
    - Test WhiteBit Flex plan fetch
    - Data accuracy validation
    - API integration testing
    - Acceptance: Plans display correctly

  - [ ] **Task QA-406**: Plan UI testing (1.00d)
    - Verify APY, minimum amount, terms display
    - UI data binding validation
    - Acceptance: Plan details accurate

#### Epic: Investment Creation Testing
- **Story 4.3.3**: Create Investment Testing
  - [ ] **Task QA-407**: Happy path investment testing (2.08d)
    - Manual test of successful investment creation
    - Full flow testing
    - Acceptance: Investment created successfully

  - [ ] **Task QA-408**: Amount validation testing (1.50d)
    - Test balance check
    - Minimum amount validation
    - Business logic testing
    - Acceptance: Validation works correctly

  - [ ] **Task QA-409**: Preview testing (1.00d)
    - Verify amount, estimated yield, terms preview
    - UI validation
    - Acceptance: Preview displays correctly

  - [ ] **Task QA-410**: USDC transfer testing (2.00d)
    - Test wallet → WhiteBit transfer accuracy
    - Critical financial flow
    - Acceptance: Transfer completes successfully

  - [ ] **Task QA-411**: Position creation validation (1.00d)
    - Validate position created as "Active"
    - State validation
    - Acceptance: Position state correct

  - [ ] **Task QA-412**: Cypress E2E for investment creation (3.00d)
    - End-to-end automation
    - Complex integration test
    - Acceptance: E2E test passes

#### Epic: Position Tracking Testing
- **Story 4.3.4**: Position View Testing
  - [ ] **Task QA-413**: Position display testing (1.50d)
    - Test principal, current value, rewards display
    - UI data accuracy
    - Acceptance: Position data displays correctly

  - [ ] **Task QA-414**: Rewards calculation testing (2.00d)
    - Validate reward calculation accuracy
    - Financial calculation precision
    - Acceptance: Rewards calculated correctly

  - [ ] **Task QA-415**: APY display testing (1.00d)
    - Verify APY updates and accuracy
    - Data refresh testing
    - Acceptance: APY displays correctly

  - [ ] **Task QA-416**: Portfolio value testing (1.00d)
    - Test aggregated portfolio value calculation
    - Aggregation logic validation
    - Acceptance: Portfolio value correct

#### Epic: Withdrawal Testing
- **Story 4.3.5**: Withdraw Investment Testing
  - [ ] **Task QA-417**: Happy path withdrawal testing (2.08d)
    - Manual test of successful full withdrawal
    - End-to-end flow
    - Acceptance: Withdrawal completes successfully

  - [ ] **Task QA-418**: Preview testing (1.00d)
    - Verify withdrawal amount, processing time
    - UI validation
    - Acceptance: Preview displays correctly

  - [ ] **Task QA-419**: USDC transfer from WhiteBit testing (2.00d)
    - Test WhiteBit → wallet transfer accuracy
    - Reverse flow testing
    - Acceptance: Transfer completes successfully

  - [ ] **Task QA-420**: Status update testing (1.00d)
    - Validate position marked as "Withdrawn"
    - State transition validation
    - Acceptance: Status updates correctly

  - [ ] **Task QA-421**: Cypress E2E for withdrawal (3.00d)
    - End-to-end automation
    - Complex integration test
    - Acceptance: E2E test passes

#### Epic: WhiteBit API Integration Testing
- **Story 4.3.6**: API Integration Testing
  - [ ] **Task QA-422**: API error handling testing (2.00d)
    - Test WhiteBit API error responses
    - Timeout handling
    - Resilience testing
    - Acceptance: Errors handled gracefully

  - [ ] **Task QA-423**: API version compatibility testing (1.08d)
    - Verify version pinning
    - Adapter pattern validation
    - Risk mitigation
    - Acceptance: API version locked

#### Epic: Position Sync Testing
- **Story 4.3.7**: Synchronization Testing
  - [ ] **Task QA-424**: Position sync validation (1.58d)
    - Test investment position synchronization
    - Data consistency checks
    - Acceptance: Position data syncs correctly

  - [ ] **Task QA-425**: Phase 4 regression testing (2.58d)
    - Execute Phase 4 regression suite
    - Complex feature regression
    - Acceptance: All Phase 4 features pass regression

### 4.4 Acceptance Criteria (Phase 4)
- ✅ WhiteBit API credentials connect and validate
- ✅ USDC Flex plans fetch and display (APY, minimum, terms)
- ✅ Investment created successfully (USDC → WhiteBit)
- ✅ Investment positions track principal, value, rewards, APY
- ✅ Portfolio summary aggregates all positions
- ✅ Withdrawal completes successfully (WhiteBit → Wallet)
- ✅ Position status tracks lifecycle (pending, active, withdrawn)
- ✅ All API credentials encrypted at rest
- ✅ $10K+ total value locked in investments (success metric)

---

## 🔄 Phase 5: Basic Swap

**Duration**: 2-3 weeks (Weeks 10-13)
**Effort**: 43.63 days (BE: 13.42, FE: 16.79, QA: 13.42)
**Dependencies**: DEX aggregator API (1inch or 0x), Polygon network liquidity
**Goal**: Enable basic USDC ↔ ETH/MATIC swaps

### 5.1 Backend - DEX Integration

#### Epic: DEX Aggregator Client
- **Story 5.1.1**: Swap APIs
  - [ ] **Task BE-501**: DEX aggregator HTTP client (3.17d)
    - HTTP client for 1inch or 0x
    - Quote and swap endpoints
    - Acceptance: Client communicates with DEX

  - [ ] **Task BE-502**: POST /api/swap/quote endpoint (2.00d)
    - Get USDC ↔ ETH/MATIC exchange rate
    - Include fees in quote
    - Acceptance: Endpoint returns swap quote

  - [ ] **Task BE-503**: POST /api/swap/execute endpoint (4.00d)
    - Execute swap transaction
    - Gas estimation, slippage tolerance
    - Acceptance: Swap executes successfully

  - [ ] **Task BE-504**: Swap fee calculation (1.08d)
    - Calculate and apply 0.5-1% platform fee
    - Fee collection logic
    - Acceptance: Fees collected correctly

  - [ ] **Task BE-505**: Swap transaction tracking (1.00d)
    - Track swap transactions in database
    - Link to main transaction table
    - Acceptance: Swap history stored

  - [ ] **Task BE-506**: Update balance after swap (1.08d)
    - Refresh wallet balances post-swap
    - Cache invalidation
    - Acceptance: Balances update correctly

### 5.2 Frontend - Swap UI

#### Epic: Swap Interface
- **Story 5.2.1**: Swap Form UI
  - [ ] **Task FE-501**: Create swap form layout (1.00d)
    - From/to token selectors
    - Amount inputs for each token
    - Acceptance: Swap form renders

  - [ ] **Task FE-502**: Build token selector dropdown (1.00d)
    - USDC, ETH, MATIC for MVP
    - Token icons and balances
    - Acceptance: Token selector works

  - [ ] **Task FE-503**: Implement swap direction toggle (0.50d)
    - Reverse from/to tokens
    - Swap arrow button
    - Acceptance: Direction toggles correctly

  - [ ] **Task FE-504**: Add amount input with max balance (0.75d)
    - Validate against balance
    - Max button
    - Acceptance: Amount input validates

  - [ ] **Task FE-505**: Display real-time exchange rate (1.00d)
    - Fetch from DEX aggregator API
    - Update on amount change
    - Acceptance: Exchange rate displays

  - [ ] **Task FE-506**: Show estimated output amount (0.88d)
    - Calculate based on rate and amount
    - Real-time calculation
    - Acceptance: Output amount updates

  - [ ] **Task FE-507**: Display swap fee breakdown (0.75d)
    - Platform fee (0.5-1%)
    - Network fee estimate
    - Acceptance: Fees display clearly

  - [ ] **Task FE-508**: Add price impact warning (1.00d)
    - Color-coded warning for >5% slippage
    - High impact alert
    - Acceptance: Warning displays for high slippage

#### Epic: Swap Execution
- **Story 5.2.2**: Swap Flow UI
  - [ ] **Task FE-509**: Build swap confirmation modal (1.00d)
    - Final amounts, rate, fees
    - Slippage tolerance
    - Risk warnings
    - Acceptance: Confirmation modal displays

  - [ ] **Task FE-510**: Integrate swap execution API (1.08d)
    - POST request to execute swap
    - DEX aggregator call + transaction submission
    - Acceptance: Swap executes via API

  - [ ] **Task FE-511**: Add passkey confirmation (0.88d)
    - WebAuthn signature for swap
    - Acceptance: Swap requires passkey

  - [ ] **Task FE-512**: Build swap execution loading state (0.75d)
    - Progress indicator
    - Estimated time
    - Acceptance: Loading displays during swap

  - [ ] **Task FE-513**: Implement success notification (0.75d)
    - Toast message
    - Transaction hash
    - Balance update
    - Acceptance: User notified of successful swap

  - [ ] **Task FE-514**: Handle swap errors (1.00d)
    - Insufficient balance
    - Slippage exceeded
    - API errors
    - Acceptance: Errors display clearly

  - [ ] **Task FE-515**: Update balances after swap (0.75d)
    - Refresh wallet state
    - Update both token balances
    - Acceptance: Balances update after swap

### 5.3 QA - Swap Testing

#### Epic: Swap Interface Testing
- **Story 5.3.1**: Swap UI Testing
  - [ ] **Task QA-501**: Swap UI testing (1.00d)
    - Test USDC ↔ ETH/MATIC interface
    - Basic UI validation
    - Acceptance: Swap UI renders correctly

  - [ ] **Task QA-502**: Exchange rate testing (1.00d)
    - Verify real-time rate accuracy
    - Price feed integration
    - Acceptance: Rates accurate

  - [ ] **Task QA-503**: Output amount testing (1.50d)
    - Test estimated output calculation
    - Calculation logic validation
    - Acceptance: Output amounts correct

  - [ ] **Task QA-504**: Fee display testing (1.00d)
    - Verify 0.5-1% fee display
    - Fee calculation accuracy
    - Acceptance: Fees calculated correctly

#### Epic: Swap Execution Testing
- **Story 5.3.2**: Swap Flow Testing
  - [ ] **Task QA-505**: Happy path swap testing (1.50d)
    - Manual test of successful swap execution
    - DEX aggregator integration
    - Acceptance: Swap completes successfully

  - [ ] **Task QA-506**: Swap error scenario testing (1.58d)
    - Test insufficient balance
    - Slippage errors
    - Error handling validation
    - Acceptance: Errors handled gracefully

  - [ ] **Task QA-507**: Balance update testing (1.00d)
    - Verify balances update correctly post-swap
    - Data accuracy
    - Acceptance: Balances correct after swap

  - [ ] **Task QA-508**: Playwright automation for swap (2.08d)
    - Automate swap flow
    - Integration automation
    - Acceptance: Automated tests pass

#### Epic: DEX Integration Testing
- **Story 5.3.3**: DEX Testing
  - [ ] **Task QA-509**: DEX aggregator testing (2.00d)
    - Test 1inch/0x integration
    - Routing validation
    - External dependency testing
    - Acceptance: DEX integration works

  - [ ] **Task QA-510**: Slippage tolerance testing (1.50d)
    - Test slippage protection mechanisms
    - DeFi-specific testing
    - Acceptance: Slippage handled correctly

  - [ ] **Task QA-511**: Phase 5 regression testing (1.08d)
    - Execute Phase 5 regression suite
    - Simpler feature regression
    - Acceptance: All Phase 5 features pass regression

### 5.4 Acceptance Criteria (Phase 5)
- ✅ USDC ↔ ETH/MATIC swap interface works
- ✅ Real-time exchange rates display
- ✅ Swap fees (0.5-1%) calculated and displayed
- ✅ Swap executes via DEX aggregator
- ✅ Balances update correctly post-swap
- ✅ Slippage protection mechanisms in place
- ✅ High price impact warnings display

---

## 🔐 Cross-Cutting Concerns

**Duration**: Ongoing (Weeks 1-14)
**Effort**: 211.48 days (BE: 85.73, FE: 49.91, QA: 75.84)
**Goal**: Security, documentation, monitoring, testing, deployment infrastructure

### Cross-Cutting Epics

#### 9.1 Security & Compliance (BE: 18.58d, FE: 7.04d, QA: 21.58d)
- Input validation middleware (FluentValidation)
- Authorization middleware (JWT validation)
- Rate limiting per user/IP
- Transaction limits enforcement ($1,000 daily)
- SQL injection protection
- HTTPS enforcement
- Secure headers (CSP, X-Frame-Options)
- Audit logging for sensitive operations
- Email verification
- Terms of Service acceptance
- Risk warning display
- Input sanitization (XSS prevention)
- Secure token storage
- Session timeout
- Content Security Policy headers
- Security testing (OWASP ZAP, vulnerability scanning)
- Accessibility compliance (WCAG 2.1 AA)

#### 9.2 API Documentation (BE: 13.75d)
- Swagger/OpenAPI setup
- XML documentation comments
- Request/response examples
- Authentication flow documentation
- Error response documentation
- Integration guide for frontend
- Postman collection export
- API versioning strategy (v1)

#### 9.3 Testing & Quality (BE: 27.83d, FE: 7.67d, QA: 75.84d)
- Unit test infrastructure (xUnit, Moq)
- Repository unit tests
- Service layer unit tests
- API integration tests
- Circle SDK integration tests
- WhiteBit API integration tests
- Fiat gateway integration tests
- Load testing scripts (Grafana K6)
- Mock API endpoints
- Component testing setup (@testing-library/react)
- Test utilities and helpers
- Utility function unit tests
- data-testid attributes
- E2E testing support
- E2E integration testing (Cypress)
- Security testing (OWASP)
- Performance testing (K6)
- Cross-device testing
- Browser compatibility testing

#### 9.4 Monitoring & Observability (BE: 15.83d)
- Application Insights setup
- Custom metrics (transaction success rate, etc.)
- Performance monitoring (operation duration)
- Error tracking and alerting
- Database query monitoring (slow queries, N+1)
- External API health monitoring
- Alerting rules
- Dashboard creation (Grafana/App Insights)

#### 9.5 DevOps & Deployment (BE: 17.83d, QA: 3.08d)
- Docker configuration (multi-stage builds)
- CI/CD pipeline (GitHub Actions/Azure DevOps)
- Environment configuration (dev, staging, prod)
- Database migration strategy (automated)
- Production deployment setup (Azure App Service)
- SSL certificate management
- Backup and recovery strategy
- Rollback procedures
- Production environment setup
- Production database HA configuration
- Production Vault setup
- Load balancer configuration
- Production monitoring setup
- Production logging (ELK/Azure Log Analytics)
- Disaster recovery testing
- Production runbook documentation
- Production smoke tests

#### 9.6 Performance Optimization (FE: 10.04d)
- Code splitting (route-based)
- Split large components into chunks
- Bundle size analysis
- Optimize imports (tree-shaking)
- Production build optimization
- Lazy loading images
- Lazy load below-fold components
- React.memo for expensive components
- useMemo/useCallback optimization
- API response caching

#### 9.7 Accessibility (FE: 12.25d)
- Keyboard navigation
- Focus management
- ARIA labels and roles
- Live regions for notifications
- Alt text for images/icons
- Screen reader testing
- Color contrast (WCAG AA 4.5:1)
- Focus indicators
- Forms accessibility

#### 9.8 Documentation (FE: 6.25d, QA: 7.58d)
- Component documentation
- Storybook stories (optional)
- Setup and installation guide
- State management architecture docs
- API integration guide
- Coding standards documentation
- Test plan documentation
- Test case documentation
- Automation framework docs
- Bug report templates
- QA runbook

#### 9.9 Bug Fixes & Polish (BE: 25.00d, FE: 10.88d, QA: 10.17d)
- Phase 1-5 bug fixes (per phase)
- Performance optimization
- Security hardening
- Code review refinements
- UI/UX polish
- Animations and transitions
- Loading state improvements
- Empty states
- Error message refinement
- Cross-browser testing
- Responsive testing
- Bug verification and retesting

---

## 🚀 Testing & Launch

**Duration**: 4-5 weeks (Weeks 13-18)
**Goal**: Beta testing, production preparation, go-live

### 10.1 Beta Testing (Weeks 13-15)

#### Epic: Beta User Testing
- **Story 10.1.1**: Beta Launch
  - [ ] **Task BETA-001**: Beta user recruitment (3d)
    - Recruit 50-100 beta testers
    - Product/Marketing coordination
    - Acceptance: Beta users recruited

  - [ ] **Task BETA-002**: Beta rollout (14d)
    - Onboard 50-100 beta users
    - All teams support
    - Acceptance: Beta users active

  - [ ] **Task BETA-003**: Monitoring and bug triage (14d)
    - Monitor real-world usage
    - QA + Backend teams
    - Acceptance: Issues tracked and prioritized

  - [ ] **Task BETA-004**: Critical bug fixes (5-10d)
    - Fix critical issues discovered
    - Backend + Frontend teams
    - Acceptance: Critical bugs resolved

  - [ ] **Task BETA-005**: User feedback collection (14d)
    - Analyze user feedback
    - Product + QA teams
    - Acceptance: Feedback documented

### 10.2 Production Preparation (Weeks 15-18)

#### Epic: Production Readiness
- **Story 10.2.1**: Production Setup
  - [ ] **Task PROD-001**: Production environment testing (1.50d)
    - Test production setup and configuration
    - Pre-launch validation
    - Acceptance: Production environment ready

  - [ ] **Task PROD-002**: Security audit (5d)
    - Basic security audit
    - Identify vulnerabilities
    - Acceptance: Security audit passed

  - [ ] **Task PROD-003**: Compliance review (3d)
    - KYC/AML preparation
    - Legal review
    - Acceptance: Compliance requirements met

  - [ ] **Task PROD-004**: Documentation finalization (5d)
    - User and developer documentation
    - Support materials
    - Acceptance: Documentation complete

  - [ ] **Task PROD-005**: Production smoke tests (4d)
    - Quick validation of critical paths
    - QA team
    - Acceptance: All smoke tests pass

  - [ ] **Task PROD-006**: Monitoring validation (1.50d)
    - Validate dashboards and alerts
    - Observability check
    - Acceptance: Monitoring operational

  - [ ] **Task PROD-007**: Rollback testing (1.08d)
    - Test rollback procedures
    - Risk mitigation
    - Acceptance: Rollback works correctly

### 10.3 Go-Live Checklist
- ✅ All critical bugs resolved
- ✅ Security audit passed
- ✅ Performance benchmarks met:
  - ✅ <3s API response time
  - ✅ <45s transaction confirmation
  - ✅ 90%+ transaction success rate
  - ✅ 100% gas sponsorship
- ✅ Compliance review complete
- ✅ Monitoring dashboards operational
- ✅ Backup and recovery tested
- ✅ Runbook documentation complete
- ✅ Support team trained
- ✅ User documentation published
- ✅ Beta feedback incorporated

### 10.4 MVP Launch (Week 18)
- [ ] **Milestone**: MVP Go-Live
  - Production deployment
  - Public announcement
  - User onboarding begins

---

## 📈 Success Metrics & KPIs

### User Adoption (First Month)
- ✅ 100 wallet creations
- ✅ 70%+ successful passkey authentication rate
- ✅ 30% of users try fiat payout (if implemented)
- ✅ 20% of users create investment position

### Transaction Performance
- ✅ 90%+ transaction success rate
- ✅ <45 seconds average confirmation time
- ✅ 100% gas sponsorship (no user fees)
- ✅ 80%+ fiat payout completion within 24 hours

### Financial Metrics (Month 3)
- ✅ $10K+ total value locked in investments
- ✅ Average investment size: $200+
- ✅ $500+ monthly revenue from fees
- ✅ 60%+ user retention after 1 month

### Technical Performance
- ✅ 99% uptime
- ✅ <3s API response time (P95)
- ✅ WCAG 2.1 AA compliance

---

## 🎯 Project Governance

### Weekly Rituals
- **Monday**: Sprint planning, task assignments
- **Wednesday**: Mid-week sync, blocker review
- **Friday**: Demo day (show working features), retrospective

### Status Reporting
- **Weekly**: Progress dashboard (actual vs. planned)
- **Bi-weekly**: Stakeholder update (milestones, risks, timeline)
- **Monthly**: Executive summary (budget, timeline, scope changes)

### Escalation Path
1. **Blocker identified** → Report to Team Lead within 24 hours
2. **Timeline slip > 3 days** → Escalate to stakeholders
3. **External dependency delayed** → Activate mitigation plan
4. **Critical bug in production** → Emergency response team activated

---

## 📝 Appendix

### Epic Abbreviation Key
- **BE**: Backend Engineering
- **FE**: Frontend Engineering
- **QA**: Quality Assurance

### Task Naming Convention
- Format: `{STREAM}-{PHASE}{SEQUENCE}`
- Examples:
  - `BE-001`: Backend task 1 in Phase 0
  - `FE-101`: Frontend task 1 in Phase 1
  - `QA-401`: QA task 1 in Phase 4

### Estimation Format
- **Days**: Man-days (8-hour workday)
- **Expected**: PERT estimate = (O + 4M + P) / 6
- **O**: Optimistic (best-case)
- **M**: Most Likely (realistic)
- **P**: Pessimistic (worst-case)

### Acceptance Criteria Format
- All tasks include **Acceptance** criteria
- Format: "Acceptance: {testable outcome}"
- Must be verifiable by QA

---

**Document Version**: 1.0
**Last Updated**: 2025-10-26
**Status**: Ready for Sprint Planning
**Next Steps**: Begin Sprint 1 with Phase 0 + Phase 1 kickoff

---

**🎉 End of CoinPay Project Planning Guide**
