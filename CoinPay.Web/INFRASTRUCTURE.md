# CoinPay Frontend Infrastructure Documentation

This document outlines the complete frontend infrastructure setup for the CoinPay application, covering tasks FE-001 through FE-008.

## Table of Contents

1. [Project Structure](#project-structure)
2. [TypeScript Configuration](#typescript-configuration)
3. [Tailwind CSS Theme](#tailwind-css-theme)
4. [React Router Setup](#react-router-setup)
5. [Environment Configuration](#environment-configuration)
6. [API Client Service](#api-client-service)
7. [State Management](#state-management)
8. [Error Handling](#error-handling)
9. [Development Workflow](#development-workflow)

---

## Project Structure

The project follows a well-organized folder structure for scalability and maintainability:

```
src/
├── components/          # Reusable UI components
│   ├── common/         # Common components (ErrorBoundary, ProtectedRoute)
│   ├── layout/         # Layout components (Header, Footer, Sidebar)
│   └── auth/           # Auth-specific components
├── pages/              # Page components (HomePage, LoginPage, etc.)
├── services/           # API services (apiClient, authService, etc.)
├── hooks/              # Custom React hooks
├── contexts/           # React Context providers (AuthContext)
├── store/              # Zustand state management stores
├── utils/              # Utility functions (formatters, validation)
├── types/              # TypeScript type definitions
├── config/             # Configuration files (env.ts)
├── constants/          # Application constants
├── routes/             # Route configuration
└── assets/             # Images, fonts, etc.
```

### Key Features:

- **Barrel Exports**: Each folder has an `index.ts` file for clean imports
- **Co-location**: Types are defined close to where they're used
- **Separation of Concerns**: Clear separation between components, services, and business logic

---

## TypeScript Configuration

### Strict Mode Enabled

TypeScript is configured with strict mode for maximum type safety:

```json
{
  "strict": true,
  "noUnusedLocals": true,
  "noUnusedParameters": true,
  "noFallthroughCasesInSwitch": true,
  "noImplicitReturns": true,
  "noUncheckedIndexedAccess": true,
  "forceConsistentCasingInFileNames": true
}
```

### Path Aliases

Clean imports using path aliases:

```typescript
import { Button } from '@/components/common';
import { authService } from '@/services';
import { User } from '@/types';
import { env } from '@/config';
```

Available aliases:
- `@/*` - Root src directory
- `@/components/*` - Components
- `@/pages/*` - Pages
- `@/services/*` - Services
- `@/store/*` - Zustand stores
- `@/types/*` - Type definitions
- `@/utils/*` - Utilities
- `@/config/*` - Configuration
- `@/constants/*` - Constants

---

## Tailwind CSS Theme

### Custom CoinPay Brand Theme

Extended Tailwind configuration with custom colors, fonts, and utilities:

#### Brand Colors

```javascript
// Primary (Blue)
primary-500: '#3b82f6'

// Secondary (Green)
secondary-500: '#22c55e'

// Accent (Purple)
accent-500: '#a855f7'

// Danger (Red)
danger-500: '#ef4444'

// Warning (Amber)
warning-500: '#f59e0b'
```

#### Custom Fonts

- **Sans**: Inter, system-ui
- **Mono**: JetBrains Mono, Menlo, Monaco

#### Custom Utilities

- Additional spacing: `18`, `88`, `100`, `128`
- Custom animations: `fade-in`, `slide-in`, `pulse-slow`
- Extended z-index: `60` through `100`
- Custom shadows and border radius values

---

## React Router Setup

### Route Configuration

Routes are defined in `src/routes/router.tsx`:

```typescript
const router = createBrowserRouter([
  { path: '/', element: <HomePage /> },
  { path: '/login', element: <LoginPage /> },
  { path: '/register', element: <RegisterPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      { path: '/dashboard', element: <DashboardPage /> },
      { path: '/wallet', element: <WalletPage /> },
      { path: '/transfer', element: <TransferPage /> },
    ],
  },
]);
```

### Protected Routes

The `ProtectedRoute` component wraps authenticated routes:

- Checks authentication status via AuthContext
- Shows loading spinner during auth check
- Redirects to `/login` if not authenticated
- Uses React Router's `<Outlet />` for nested routes

### Available Routes

| Path | Component | Protected |
|------|-----------|-----------|
| `/` | HomePage | No |
| `/login` | LoginPage | No |
| `/register` | RegisterPage | No |
| `/dashboard` | DashboardPage | Yes |
| `/wallet` | WalletPage | Yes |
| `/transfer` | TransferPage | Yes |

---

## Environment Configuration

### Environment Files

**`.env.example`** - Template for environment variables
**`.env.development`** - Development environment settings

### Environment Variables

```bash
# API Configuration
VITE_API_BASE_URL=http://localhost:5100
VITE_API_TIMEOUT=30000

# Application
VITE_APP_NAME=CoinPay
VITE_APP_VERSION=1.0.0

# Feature Flags
VITE_ENABLE_LOGGING=true
VITE_ENABLE_MOCK_API=false
```

### Type-Safe Environment Access

Environment variables are accessed through `src/config/env.ts`:

```typescript
import { env } from '@/config';

console.log(env.apiBaseUrl);  // Type-safe!
console.log(env.isDevelopment);
```

---

## API Client Service

### Axios-Based Client

Centralized API client with interceptors in `src/services/apiClient.ts`:

#### Features:

1. **Base Configuration**
   - Base URL from environment
   - 30-second timeout
   - JSON content type

2. **Request Interceptor**
   - Automatically adds JWT token from localStorage
   - Logs requests in development mode
   - Handles request errors

3. **Response Interceptor**
   - Logs responses in development mode
   - Handles global error cases:
     - 401: Clear token and redirect to login
     - 403: Forbidden access
     - 404: Not found
     - 500+: Server errors
   - Network error handling

### Service Modules

#### Authentication Service (`authService`)

```typescript
import { authService } from '@/services';

// Check username
await authService.checkUsername('john');

// Register user (passkey flow)
await authService.registerInitiate('john');
await authService.registerComplete(data);

// Login user (passkey flow)
await authService.loginInitiate('john');
await authService.loginComplete(data);

// Get user profile
const user = await authService.getProfile();

// Logout
authService.logout();
```

#### Wallet Service (`walletService`)

```typescript
import { walletService } from '@/services';

// Create wallet
await walletService.createWallet({ userId: 1 });

// Get wallet
const wallet = await walletService.getWalletByUserId(1);

// Get balance
const balance = await walletService.getBalance(walletAddress);
```

#### Transaction Service (`transactionService`)

```typescript
import { transactionService } from '@/services';

// Get all transactions
const txs = await transactionService.getAll();

// Create transaction
await transactionService.create({
  fromAddress: '0x...',
  toAddress: '0x...',
  amount: 100,
  currency: 'USDC',
});

// Get by status
const pending = await transactionService.getByStatus('pending');
```

---

## State Management

### Zustand Stores

Three main stores for state management:

#### 1. Auth Store (`useAuthStore`)

```typescript
import { useAuthStore } from '@/store';

function MyComponent() {
  const { user, token, isAuthenticated, login, logout } = useAuthStore();

  // Use authentication state and actions
}
```

**State:**
- `user`: User object or null
- `token`: JWT token or null
- `isAuthenticated`: Boolean
- `isLoading`: Loading state
- `error`: Error message

**Actions:**
- `login(user, token)`: Set user and token
- `logout()`: Clear authentication
- `setError(error)`: Set error message

**Features:**
- Persisted to localStorage
- DevTools integration
- Type-safe actions

#### 2. Wallet Store (`useWalletStore`)

```typescript
import { useWalletStore } from '@/store';

const { wallet, balance, setWallet } = useWalletStore();
```

**State:**
- `wallet`: Wallet object
- `balance`: Current balance
- `currency`: Default currency
- `isLoading`: Loading state
- `error`: Error message

#### 3. Transaction Store (`useTransactionStore`)

```typescript
import { useTransactionStore } from '@/store';

const { transactions, addTransaction } = useTransactionStore();
```

**State:**
- `transactions`: Array of transactions
- `currentTransaction`: Selected transaction
- `isLoading`: Loading state
- `error`: Error message

**Actions:**
- `setTransactions(txs)`: Set all transactions
- `addTransaction(tx)`: Add single transaction
- `updateTransaction(id, updates)`: Update transaction
- `removeTransaction(id)`: Remove transaction

---

## Error Handling

### Error Boundary Component

Wraps the entire app to catch React errors:

```typescript
import { ErrorBoundary } from '@/components/common';

<ErrorBoundary>
  <App />
</ErrorBoundary>
```

**Features:**
- Catches component errors
- Shows user-friendly error UI
- Displays error details in development
- Provides "Try Again" and "Go Home" actions
- Logs errors to console

**Custom Fallback:**

```typescript
<ErrorBoundary fallback={<CustomErrorUI />}>
  <MyComponent />
</ErrorBoundary>
```

### API Error Handling

API errors are handled at multiple levels:

1. **Axios Interceptor** - Global error handling
2. **Service Layer** - Service-specific error handling
3. **Component Level** - UI error states

```typescript
try {
  await authService.login(username, password);
} catch (error) {
  setError(error.message);
}
```

---

## Development Workflow

### Getting Started

1. **Install Dependencies**
   ```bash
   npm install
   ```

2. **Setup Environment**
   ```bash
   cp .env.example .env.development
   # Edit .env.development with your values
   ```

3. **Start Development Server**
   ```bash
   npm run dev
   # Runs on http://localhost:3000
   ```

### Build for Production

```bash
npm run build
# Output: dist/
```

### Preview Production Build

```bash
npm run preview
```

### Linting

```bash
npm run lint
```

---

## Type Definitions

### Core Types

#### User
```typescript
interface User {
  id: number;
  username: string;
  email?: string;
  walletAddress?: string;
  circleUserId?: string;
}
```

#### Wallet
```typescript
interface Wallet {
  id: string;
  userId: number;
  walletAddress: string;
  balance: number;
  currency: string;
  createdAt: string;
}
```

#### Transaction
```typescript
interface Transaction {
  id: number;
  fromAddress: string;
  toAddress: string;
  amount: number;
  currency: string;
  status: TransactionStatus;
  transactionHash?: string;
  createdAt: string;
}

enum TransactionStatus {
  Pending = 'pending',
  Processing = 'processing',
  Completed = 'completed',
  Failed = 'failed',
  Cancelled = 'cancelled',
}
```

---

## Utility Functions

### Formatters (`src/utils/formatters.ts`)

```typescript
import { formatCurrency, formatDate, formatWalletAddress } from '@/utils';

formatCurrency(100.5, 'USDC'); // "$100.50 USDC"
formatDate(new Date()); // "Jan 1, 2024, 12:00 PM"
formatWalletAddress('0x1234...5678'); // "0x1234...5678"
```

### Validation (`src/utils/validation.ts`)

```typescript
import { isValidEmail, isValidUsername, isValidPassword } from '@/utils';

isValidEmail('test@example.com'); // true
isValidUsername('john_doe'); // true
isValidPassword('secret123'); // true
```

---

## Constants

Application-wide constants in `src/constants/index.ts`:

```typescript
import { ROUTES, TRANSACTION_STATUS, CURRENCIES } from '@/constants';

// Routes
ROUTES.DASHBOARD; // '/dashboard'

// Transaction statuses
TRANSACTION_STATUS.PENDING; // 'pending'

// Currencies
CURRENCIES.USDC; // 'USDC'
```

---

## Next Steps

The infrastructure is complete and ready for feature development:

1. **Sprint N01 Phase 1**: Implement wallet management UI
2. **Sprint N01 Phase 2**: Implement transaction/transfer UI
3. **Sprint N01 Phase 3**: Add transaction history and status tracking
4. **Future**: WebAuthn passkey integration, enhanced error handling, unit tests

---

## Summary

All tasks FE-001 through FE-008 have been successfully completed:

- ✅ **FE-001**: Enhanced folder structure with barrel exports
- ✅ **FE-002**: TypeScript strict mode configuration
- ✅ **FE-003**: Tailwind CSS custom theme
- ✅ **FE-004**: React Router v6 with protected routes
- ✅ **FE-005**: Environment configuration files
- ✅ **FE-006**: API client service with interceptors
- ✅ **FE-007**: Zustand state management stores
- ✅ **FE-008**: Error Boundary component

The application compiles successfully and is ready for UI component development.
