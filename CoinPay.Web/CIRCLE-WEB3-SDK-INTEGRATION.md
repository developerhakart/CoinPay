# Circle Web3 Services SDK Integration Guide

**Last Updated**: 2025-11-07
**Frontend**: React + TypeScript + Vite
**SDK Version**: Latest stable

## Overview

This guide provides step-by-step instructions for integrating Circle's Web3 Services SDK into the CoinPay React frontend. The SDK enables passwordless authentication using passkeys and secure wallet management.

## Prerequisites

- **Circle Developer Account**: https://console.circle.com/
- **API Key**: Obtained from Circle Console
- **App ID**: Created in Circle Console
- **Node.js**: v18+ installed
- **npm**: v9+ installed

## Installation

### 1. Install Circle Web3 SDK

```bash
cd CoinPay.Web

# Install the main SDK
npm install @circle-fin/w3s-pw-web-sdk

# Install peer dependencies (if needed)
npm install ethers@^6.0.0
```

### 2. Update package.json

After installation, verify dependencies:

```json
{
  "dependencies": {
    "@circle-fin/w3s-pw-web-sdk": "^2.x.x",
    "ethers": "^6.x.x",
    // ... existing dependencies
  }
}
```

## Configuration

### 1. Environment Variables

Create `.env.local` file in `CoinPay.Web/`:

```env
# Circle Web3 Services Configuration
VITE_CIRCLE_APP_ID=your-app-id-here
VITE_API_BASE_URL=http://localhost:5100
VITE_ENVIRONMENT=sandbox
```

**⚠️ Important**: Add `.env.local` to `.gitignore`

### 2. Vite Configuration

Update `vite.config.ts` to use port 5173 (for E2E tests):

```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173, // Updated from 3000 for E2E tests
  },
})
```

### 3. TypeScript Configuration

Add SDK types (if not auto-generated):

```typescript
// src/types/circle.d.ts
declare module '@circle-fin/w3s-pw-web-sdk' {
  export interface W3SSdk {
    init(config: CircleConfig): Promise<void>;
    execute(challenge: string, onSuccess: (result: any) => void, onError: (error: any) => void): void;
  }

  export interface CircleConfig {
    appId: string;
  }

  export const W3SSdk: W3SSdk;
}
```

## SDK Initialization

### 1. Create Circle SDK Service

Create `src/services/circleService.ts`:

```typescript
import { W3SSdk } from '@circle-fin/w3s-pw-web-sdk';

class CircleService {
  private sdk: typeof W3SSdk | null = null;
  private initialized: boolean = false;

  /**
   * Initialize the Circle SDK
   */
  async initialize(): Promise<void> {
    if (this.initialized) {
      console.log('Circle SDK already initialized');
      return;
    }

    const appId = import.meta.env.VITE_CIRCLE_APP_ID;

    if (!appId) {
      throw new Error('Circle App ID not configured. Check VITE_CIRCLE_APP_ID in .env.local');
    }

    try {
      await W3SSdk.init({
        appId: appId,
      });

      this.sdk = W3SSdk;
      this.initialized = true;
      console.log('Circle SDK initialized successfully');
    } catch (error) {
      console.error('Failed to initialize Circle SDK:', error);
      throw error;
    }
  }

  /**
   * Execute a Circle challenge (for user actions like registration, login, transactions)
   */
  async executeChallenge(challengeId: string): Promise<any> {
    if (!this.initialized || !this.sdk) {
      throw new Error('Circle SDK not initialized. Call initialize() first.');
    }

    return new Promise((resolve, reject) => {
      this.sdk!.execute(
        challengeId,
        (result) => {
          console.log('Challenge completed successfully:', result);
          resolve(result);
        },
        (error) => {
          console.error('Challenge failed:', error);
          reject(error);
        }
      );
    });
  }

  /**
   * Check if SDK is initialized
   */
  isInitialized(): boolean {
    return this.initialized;
  }
}

// Export singleton instance
export const circleService = new CircleService();
export default circleService;
```

### 2. Initialize SDK on App Load

Update `src/App.tsx`:

```typescript
import { useEffect, useState } from 'react';
import { BrowserRouter } from 'react-router-dom';
import circleService from './services/circleService';
import AppRoutes from './routes/router';
import ErrorBoundary from './components/common/ErrorBoundary';

function App() {
  const [sdkReady, setSdkReady] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Initialize Circle SDK on app load
    circleService
      .initialize()
      .then(() => {
        setSdkReady(true);
      })
      .catch((err) => {
        setError(`Failed to initialize Circle SDK: ${err.message}`);
        console.error(err);
      });
  }, []);

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-red-50">
        <div className="max-w-md p-6 bg-white rounded-lg shadow-lg">
          <h2 className="text-xl font-bold text-red-600 mb-2">
            Initialization Error
          </h2>
          <p className="text-gray-700">{error}</p>
          <button
            onClick={() => window.location.reload()}
            className="mt-4 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
          >
            Retry
          </button>
        </div>
      </div>
    );
  }

  if (!sdkReady) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Initializing CoinPay...</p>
        </div>
      </div>
    );
  }

  return (
    <ErrorBoundary>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </ErrorBoundary>
  );
}

export default App;
```

## User Registration with Passkeys

### 1. Update Registration Page

Update `src/pages/RegisterPage.tsx`:

```typescript
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import circleService from '../services/circleService';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

interface RegistrationData {
  username: string;
  email: string;
}

export default function RegisterPage() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<RegistrationData>({
    username: '',
    email: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // Step 1: Initiate registration with backend
      const initResponse = await axios.post(`${API_BASE_URL}/api/auth/register/initiate`, {
        username: formData.username,
        email: formData.email,
      });

      const { challengeId, userToken } = initResponse.data;

      // Step 2: Execute Circle challenge (user creates passkey)
      const challengeResult = await circleService.executeChallenge(challengeId);

      // Step 3: Complete registration with backend
      const completeResponse = await axios.post(`${API_BASE_URL}/api/auth/register/complete`, {
        username: formData.username,
        userToken: userToken,
        challengeResult: challengeResult,
      });

      // Step 4: Save auth token and redirect
      localStorage.setItem('authToken', completeResponse.data.token);
      localStorage.setItem('userId', completeResponse.data.userId);

      console.log('Registration successful!');
      navigate('/dashboard');
    } catch (err: any) {
      console.error('Registration failed:', err);
      setError(
        err.response?.data?.message ||
        'Registration failed. Please try again.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full bg-white rounded-lg shadow-md p-8">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">
          Create Your Account
        </h2>

        {error && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded">
            <p className="text-red-800 text-sm">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Username
            </label>
            <input
              type="text"
              required
              value={formData.username}
              onChange={(e) => setFormData({ ...formData, username: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your username"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              type="email"
              required
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="your@email.com"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
          >
            {loading ? 'Creating Account...' : 'Register with Passkey'}
          </button>
        </form>

        <p className="mt-4 text-center text-sm text-gray-600">
          Already have an account?{' '}
          <a href="/login" className="text-blue-600 hover:underline">
            Login
          </a>
        </p>

        <div className="mt-6 p-4 bg-blue-50 rounded-md">
          <h3 className="text-sm font-medium text-blue-900 mb-2">
            🔐 Passwordless Authentication
          </h3>
          <p className="text-xs text-blue-800">
            You'll be prompted to create a passkey using your device's biometric authentication
            (fingerprint, face ID) or PIN. This is more secure than traditional passwords!
          </p>
        </div>
      </div>
    </div>
  );
}
```

## User Login with Passkeys

### Update Login Page

Update `src/pages/LoginPage.tsx`:

```typescript
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import circleService from '../services/circleService';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export default function LoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // Step 1: Initiate login with backend
      const initResponse = await axios.post(`${API_BASE_URL}/api/auth/login/initiate`, {
        username: username,
      });

      const { challengeId } = initResponse.data;

      // Step 2: Execute Circle challenge (user authenticates with passkey)
      const challengeResult = await circleService.executeChallenge(challengeId);

      // Step 3: Complete login with backend
      const completeResponse = await axios.post(`${API_BASE_URL}/api/auth/login/complete`, {
        username: username,
        challengeResult: challengeResult,
      });

      // Step 4: Save auth token and redirect
      localStorage.setItem('authToken', completeResponse.data.token);
      localStorage.setItem('userId', completeResponse.data.userId);

      console.log('Login successful!');
      navigate('/dashboard');
    } catch (err: any) {
      console.error('Login failed:', err);
      setError(
        err.response?.data?.message ||
        'Login failed. Please try again.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full bg-white rounded-lg shadow-md p-8">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">
          Welcome Back
        </h2>

        {error && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded">
            <p className="text-red-800 text-sm">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Username
            </label>
            <input
              type="text"
              required
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your username"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
          >
            {loading ? 'Authenticating...' : 'Login with Passkey'}
          </button>
        </form>

        <p className="mt-4 text-center text-sm text-gray-600">
          Don't have an account?{' '}
          <a href="/register" className="text-blue-600 hover:underline">
            Register
          </a>
        </p>
      </div>
    </div>
  );
}
```

## Wallet Creation

### Create Wallet Creation Hook

Create `src/hooks/useWalletCreation.ts`:

```typescript
import { useState } from 'react';
import axios from 'axios';
import circleService from '../services/circleService';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

interface WalletCreationResult {
  walletId: string;
  address: string;
  blockchain: string;
}

export function useWalletCreation() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createWallet = async (
    blockchain: string = 'ETH-SEPOLIA'
  ): Promise<WalletCreationResult | null> => {
    setLoading(true);
    setError(null);

    try {
      const userId = localStorage.getItem('userId');
      const token = localStorage.getItem('authToken');

      if (!userId || !token) {
        throw new Error('Not authenticated');
      }

      // Call backend to create wallet
      const response = await axios.post(
        `${API_BASE_URL}/api/wallets`,
        { blockchain },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      return {
        walletId: response.data.id,
        address: response.data.address,
        blockchain: response.data.blockchain,
      };
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Failed to create wallet';
      setError(errorMessage);
      console.error('Wallet creation failed:', err);
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { createWallet, loading, error };
}
```

### Update Wallet Page

Update `src/pages/WalletPage.tsx` to include wallet creation:

```typescript
import { useState, useEffect } from 'react';
import { useWalletCreation } from '../hooks/useWalletCreation';
import WalletHeader from '../components/wallet/WalletHeader';
import BalanceCard from '../components/wallet/BalanceCard';
import QuickActions from '../components/wallet/QuickActions';
import RecentTransactions from '../components/wallet/RecentTransactions';

export default function WalletPage() {
  const [wallet, setWallet] = useState<any>(null);
  const { createWallet, loading: creating, error: createError } = useWalletCreation();

  useEffect(() => {
    // Load user's wallet on mount
    // Implementation depends on your API structure
  }, []);

  const handleCreateWallet = async () => {
    const result = await createWallet('ETH-SEPOLIA');
    if (result) {
      setWallet(result);
      console.log('Wallet created:', result);
    }
  };

  if (!wallet) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="max-w-md w-full bg-white rounded-lg shadow-md p-8 text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Create Your Wallet
          </h2>
          <p className="text-gray-600 mb-6">
            You don't have a wallet yet. Create one to start using CoinPay.
          </p>

          {createError && (
            <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded">
              <p className="text-red-800 text-sm">{createError}</p>
            </div>
          )}

          <button
            onClick={handleCreateWallet}
            disabled={creating}
            className="w-full px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 transition-colors"
          >
            {creating ? 'Creating Wallet...' : 'Create Wallet'}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <WalletHeader address={wallet.address} />
        <div className="mt-6 grid grid-cols-1 lg:grid-cols-2 gap-6">
          <BalanceCard />
          <QuickActions />
        </div>
        <div className="mt-6">
          <RecentTransactions />
        </div>
      </div>
    </div>
  );
}
```

## Transaction Signing

### Create Transaction Hook

Create `src/hooks/useTransaction.ts`:

```typescript
import { useState } from 'react';
import axios from 'axios';
import circleService from '../services/circleService';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

interface TransactionParams {
  toAddress: string;
  amount: number;
  token: string;
  blockchain: string;
}

export function useTransaction() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const sendTransaction = async (params: TransactionParams): Promise<boolean> => {
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error('Not authenticated');
      }

      // Step 1: Initiate transaction with backend
      const initResponse = await axios.post(
        `${API_BASE_URL}/api/transactions/initiate`,
        params,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      const { challengeId, transactionId } = initResponse.data;

      // Step 2: Execute Circle challenge (user signs transaction with passkey)
      const challengeResult = await circleService.executeChallenge(challengeId);

      // Step 3: Execute transaction with backend
      await axios.post(
        `${API_BASE_URL}/api/transactions/${transactionId}/execute`,
        { challengeResult },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      console.log('Transaction sent successfully!');
      return true;
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Transaction failed';
      setError(errorMessage);
      console.error('Transaction failed:', err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  return { sendTransaction, loading, error };
}
```

### Update Transfer Page

Update `src/pages/TransferPage.tsx`:

```typescript
import { useState } from 'react';
import { useTransaction } from '../hooks/useTransaction';

export default function TransferPage() {
  const [formData, setFormData] = useState({
    toAddress: '',
    amount: '',
    token: 'USDC',
    blockchain: 'ETH-SEPOLIA',
  });

  const { sendTransaction, loading, error } = useTransaction();
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSuccess(false);

    const result = await sendTransaction({
      toAddress: formData.toAddress,
      amount: parseFloat(formData.amount),
      token: formData.token,
      blockchain: formData.blockchain,
    });

    if (result) {
      setSuccess(true);
      setFormData({
        toAddress: '',
        amount: '',
        token: 'USDC',
        blockchain: 'ETH-SEPOLIA',
      });
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-2xl mx-auto px-4">
        <h1 className="text-3xl font-bold text-gray-900 mb-6">Send Crypto</h1>

        {success && (
          <div className="mb-6 p-4 bg-green-50 border border-green-200 rounded">
            <p className="text-green-800">Transaction sent successfully!</p>
          </div>
        )}

        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded">
            <p className="text-red-800">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-md p-6 space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Recipient Address
            </label>
            <input
              type="text"
              required
              value={formData.toAddress}
              onChange={(e) => setFormData({ ...formData, toAddress: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="0x..."
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Amount
            </label>
            <input
              type="number"
              required
              step="0.000001"
              min="0"
              value={formData.amount}
              onChange={(e) => setFormData({ ...formData, amount: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="0.00"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Token
            </label>
            <select
              value={formData.token}
              onChange={(e) => setFormData({ ...formData, token: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="USDC">USDC</option>
              <option value="ETH">ETH</option>
            </select>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
          >
            {loading ? 'Sending...' : 'Send Transaction'}
          </button>
        </form>
      </div>
    </div>
  );
}
```

## Testing

### Local Testing

1. **Start backend API**:
   ```bash
   cd CoinPay.Api
   dotnet run --urls "http://localhost:5100"
   ```

2. **Start frontend**:
   ```bash
   cd CoinPay.Web
   npm run dev
   ```

3. **Test registration flow**:
   - Navigate to http://localhost:5173/register
   - Fill in username and email
   - Click "Register with Passkey"
   - Follow passkey creation prompt
   - Verify redirect to dashboard

4. **Test login flow**:
   - Navigate to http://localhost:5173/login
   - Enter username
   - Click "Login with Passkey"
   - Authenticate with passkey
   - Verify redirect to dashboard

5. **Test wallet creation**:
   - Navigate to /wallet
   - Click "Create Wallet"
   - Verify wallet address is displayed

6. **Test transaction**:
   - Navigate to /transfer
   - Fill in recipient, amount, token
   - Click "Send Transaction"
   - Sign with passkey
   - Verify transaction success

## Troubleshooting

### SDK Initialization Fails

**Error**: `Failed to initialize Circle SDK: Invalid App ID`

**Solution**:
1. Verify `VITE_CIRCLE_APP_ID` in `.env.local`
2. Check App ID in Circle Console
3. Ensure environment variables are loaded (restart dev server)

### Passkey Creation Fails

**Error**: `User cancelled passkey creation`

**Solution**:
- User clicked "Cancel" on passkey prompt
- Ask user to retry and complete the passkey setup

**Error**: `Passkey not supported on this device`

**Solution**:
- Test on different browser (Chrome, Safari, Edge supported)
- Ensure device supports WebAuthn/FIDO2
- Use desktop/mobile with biometric auth

### Challenge Execution Timeout

**Error**: `Challenge timeout after 5 minutes`

**Solution**:
1. User must complete action within 5 minutes
2. If timeout occurs, restart the flow
3. Backend should handle expired challenges

### CORS Errors

**Error**: `CORS policy: No 'Access-Control-Allow-Origin' header`

**Solution**:
Update API CORS configuration to include frontend URL:
```csharp
// In CoinPay.Api/Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Updated port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

## Security Best Practices

1. **Never expose API keys client-side**
   - Use backend proxy for Circle API calls
   - Only expose App ID (public identifier)

2. **Validate all inputs**
   - Sanitize addresses
   - Validate amounts
   - Check token/blockchain values

3. **Use HTTPS in production**
   - Passkeys require secure context
   - Configure SSL certificates

4. **Implement rate limiting**
   - Limit registration attempts
   - Throttle transaction requests

5. **Handle sensitive data carefully**
   - Never log user tokens
   - Clear sensitive data from memory
   - Use secure storage for auth tokens

## Production Deployment

### Environment Setup

**Production `.env`**:
```env
VITE_CIRCLE_APP_ID=production-app-id
VITE_API_BASE_URL=https://api.coinpay.com
VITE_ENVIRONMENT=production
```

### Build Process

```bash
# Install dependencies
npm install

# Build for production
npm run build

# Output in dist/ directory
ls dist/
```

### Deployment Checklist

- [ ] Circle App ID configured for production
- [ ] HTTPS enabled
- [ ] CORS configured correctly
- [ ] Environment variables set
- [ ] Error tracking configured (Sentry, etc.)
- [ ] Analytics configured
- [ ] Rate limiting enabled
- [ ] Logging configured
- [ ] CDN configured for static assets

## Resources

- **Circle Web3 Services Docs**: https://developers.circle.com/w3s/docs
- **SDK Reference**: https://www.npmjs.com/package/@circle-fin/w3s-pw-web-sdk
- **Circle Console**: https://console.circle.com/
- **WebAuthn Guide**: https://webauthn.guide/
- **Passkeys Info**: https://passkeys.dev/

---

**Integration Status**: Not yet implemented
**Estimated Implementation Time**: 4-6 hours
**Priority**: High (Core MVP feature)
