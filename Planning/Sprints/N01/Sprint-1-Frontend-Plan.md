# CoinPay Wallet MVP - Sprint 1 Frontend Plan

**Version**: 1.0
**Sprint Duration**: 2 weeks (10 working days)
**Sprint Period**: January 6 - January 17, 2025
**Team Composition**: 2-3 Frontend Engineers
**Available Capacity**: 20-30 engineering days
**Planned Effort**: ~24.25 days (within capacity)
**Sprint Type**: Foundation Sprint

---

## Sprint Goal

**Establish React frontend foundation with WebAuthn passkey integration and core wallet UI components to support gasless USDC wallet creation and transfers.**

By the end of Sprint 1, we will have:
- A working React application with TypeScript and Tailwind CSS
- WebAuthn passkey registration and login flows (client-side)
- Wallet creation UI integrated with Backend API
- Transfer UI mockup (functional in Sprint 2)
- Responsive design framework (mobile-first)
- API client configured with error handling
- Component library foundation
- Authentication state management
- Integration with Backend API endpoints

---

## Selected Tasks & Effort Distribution

### Phase 0: Frontend Infrastructure Setup (8.08 days)
- React project structure and configuration
- TypeScript and ESLint setup
- Tailwind CSS and UI framework integration
- Folder structure and routing
- Environment configuration
- API client service setup
- State management setup (Context API or Zustand)
- Error boundary implementation

### Phase 1: Authentication & WebAuthn Integration (10.17 days)
- WebAuthn passkey client library integration
- Passkey registration flow UI
- Passkey login flow UI
- Authentication context and state management
- Protected route wrapper
- Session management (JWT storage)
- Login/Register page components
- User profile display

### Phase 2: Wallet & Transfer UI (6.00 days)
- Wallet creation UI
- Wallet dashboard component
- Balance display component
- Transfer form UI
- Transaction status display
- Loading states and skeletons
- Error handling UI patterns

**Total Sprint 1 Effort**: ~24.25 days of tasks (fits 20-30 day capacity with 2-3 engineers)

---

## Task Breakdown with Details

### Epic 0.2: Frontend Infrastructure Setup (8.08 days)

#### FE-001: Initialize React Project Structure (0.54 days)
**Owner**: Frontend Lead
**Priority**: P0 - Critical
**Dependencies**: None

**Description**:
Verify and enhance existing Vite + React + TypeScript project structure.

**Technical Requirements**:
- Verify Vite configuration (already at port 3000)
- Enhance folder structure:
  ```
  src/
  ├── components/       # Reusable UI components
  │   ├── common/       # Buttons, inputs, cards
  │   ├── layout/       # Header, footer, navigation
  │   └── wallet/       # Wallet-specific components
  ├── pages/            # Page components (routes)
  │   ├── auth/         # Login, register
  │   ├── wallet/       # Dashboard, transfer
  │   └── home/         # Landing page
  ├── services/         # API clients, auth service
  ├── hooks/            # Custom React hooks
  ├── context/          # React context providers
  ├── types/            # TypeScript type definitions
  ├── utils/            # Helper functions
  ├── assets/           # Images, icons
  └── styles/           # Global styles, themes
  ```
- Update `tsconfig.json` with path aliases:
  ```json
  {
    "compilerOptions": {
      "paths": {
        "@/*": ["./src/*"],
        "@components/*": ["./src/components/*"],
        "@pages/*": ["./src/pages/*"],
        "@services/*": ["./src/services/*"],
        "@hooks/*": ["./src/hooks/*"],
        "@types/*": ["./src/types/*"],
        "@utils/*": ["./src/utils/*"]
      }
    }
  }
  ```

**Acceptance Criteria**:
- [x] Folder structure created with all directories
- [x] TypeScript path aliases configured and working
- [x] Sample components in each directory
- [x] Project builds without errors
- [x] ESLint runs with no errors

**Definition of Done**:
- Code committed to repository
- Folder structure documented in README
- Team can import using path aliases

---

#### FE-002: Configure TypeScript and ESLint (0.54 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-001

**Description**:
Enhance TypeScript strict mode and ESLint rules for code quality.

**Technical Requirements**:
- Enable additional TypeScript strict checks:
  ```json
  {
    "compilerOptions": {
      "strict": true,
      "noUnusedLocals": true,
      "noUnusedParameters": true,
      "noFallthroughCasesInSwitch": true,
      "forceConsistentCasingInFileNames": true,
      "noImplicitReturns": true,
      "skipLibCheck": true
    }
  }
  ```
- Add ESLint rules for React best practices:
  ```json
  {
    "extends": [
      "eslint:recommended",
      "plugin:@typescript-eslint/recommended",
      "plugin:react-hooks/recommended",
      "plugin:react/recommended"
    ],
    "rules": {
      "react/react-in-jsx-scope": "off",
      "react-hooks/exhaustive-deps": "warn",
      "@typescript-eslint/no-unused-vars": "error",
      "@typescript-eslint/explicit-function-return-type": "warn"
    }
  }
  ```
- Add Prettier for code formatting (optional but recommended)

**Acceptance Criteria**:
- [x] TypeScript strict mode enabled
- [x] ESLint catches common React mistakes
- [x] All existing code passes linting
- [x] Pre-commit hook runs linter (optional)

**Definition of Done**:
- ESLint configuration documented
- Team aware of linting rules
- CI/CD runs linter on PR

---

#### FE-003: Setup Tailwind CSS and UI Framework (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-001

**Description**:
Configure Tailwind CSS with custom theme and optionally integrate a component library.

**Technical Requirements**:
- Verify Tailwind CSS configuration (already installed)
- Create custom theme in `tailwind.config.js`:
  ```javascript
  module.exports = {
    theme: {
      extend: {
        colors: {
          primary: {
            50: '#eff6ff',
            500: '#3b82f6',
            600: '#2563eb',
            700: '#1d4ed8',
          },
          success: '#10b981',
          warning: '#f59e0b',
          error: '#ef4444',
        },
        fontFamily: {
          sans: ['Inter', 'system-ui', 'sans-serif'],
        },
      },
    },
  }
  ```
- Consider integrating **shadcn/ui** or **Headless UI** for accessible components:
  - **Recommended**: shadcn/ui (Tailwind-based, customizable)
  - Alternative: Headless UI + Heroicons
- Create base component library:
  - Button component with variants (primary, secondary, danger)
  - Input component with validation states
  - Card component
  - Modal/Dialog component
  - Toast notification component

**Acceptance Criteria**:
- [x] Tailwind CSS custom theme configured
- [x] UI component library installed (if using one)
- [x] 5+ base components created (Button, Input, Card, Modal, Toast)
- [x] Components use consistent design tokens
- [x] Components are accessible (keyboard navigation, ARIA)

**Definition of Done**:
- Storybook setup (optional for Sprint 1, recommended for Sprint 2)
- Component documentation
- Design system documented

---

#### FE-004: Setup React Router and Navigation (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-001

**Description**:
Configure React Router v6 for client-side routing.

**Technical Requirements**:
- Install React Router: `npm install react-router-dom`
- Create route structure:
  ```typescript
  // src/routes/index.tsx
  import { createBrowserRouter } from 'react-router-dom';

  export const router = createBrowserRouter([
    {
      path: '/',
      element: <Layout />,
      children: [
        { index: true, element: <HomePage /> },
        { path: 'login', element: <LoginPage /> },
        { path: 'register', element: <RegisterPage /> },
        {
          path: 'wallet',
          element: <ProtectedRoute><WalletLayout /></ProtectedRoute>,
          children: [
            { index: true, element: <WalletDashboard /> },
            { path: 'transfer', element: <TransferPage /> },
            { path: 'history', element: <TransactionHistoryPage /> },
          ],
        },
      ],
    },
  ]);
  ```
- Create `<ProtectedRoute>` wrapper for authenticated routes
- Create `<Layout>` component with header and navigation
- Implement route-based code splitting with lazy loading:
  ```typescript
  const WalletDashboard = lazy(() => import('./pages/wallet/Dashboard'));
  ```

**Acceptance Criteria**:
- [x] React Router configured with all routes
- [x] Navigation between pages works
- [x] Protected routes redirect to login if not authenticated
- [x] Lazy loading implemented for code splitting
- [x] 404 page created

**Definition of Done**:
- All routes documented
- Navigation tested manually
- Route guards functional

---

#### FE-005: Environment Configuration (0.50 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-001

**Description**:
Configure environment variables for different environments (dev, staging, production).

**Technical Requirements**:
- Create `.env` files:
  ```
  # .env.development
  VITE_API_BASE_URL=http://localhost:5100
  VITE_GATEWAY_URL=http://localhost:5100
  VITE_ENVIRONMENT=development
  VITE_ENABLE_LOGGING=true

  # .env.production
  VITE_API_BASE_URL=https://api.coinpay.com
  VITE_GATEWAY_URL=https://gateway.coinpay.com
  VITE_ENVIRONMENT=production
  VITE_ENABLE_LOGGING=false
  ```
- Create environment config service:
  ```typescript
  // src/config/env.ts
  export const config = {
    apiBaseUrl: import.meta.env.VITE_API_BASE_URL,
    gatewayUrl: import.meta.env.VITE_GATEWAY_URL,
    environment: import.meta.env.VITE_ENVIRONMENT,
    enableLogging: import.meta.env.VITE_ENABLE_LOGGING === 'true',
  } as const;
  ```
- Add `.env.example` template for team
- Update `.gitignore` to exclude `.env.local`

**Acceptance Criteria**:
- [x] Environment variables load correctly
- [x] Different configs for dev/staging/production
- [x] Environment config service accessible
- [x] `.env.example` documented
- [x] No sensitive data in `.env` files committed

**Definition of Done**:
- Environment setup documented
- Team can run app locally
- Configuration validated

---

#### FE-006: API Client Service Setup (2.00 days)
**Owner**: Senior Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-005

**Description**:
Create HTTP client service for Backend API communication with error handling, interceptors, and retry logic.

**Technical Requirements**:
- Install axios: `npm install axios`
- Create API client with interceptors:
  ```typescript
  // src/services/api/client.ts
  import axios, { AxiosInstance, AxiosError } from 'axios';
  import { config } from '@/config/env';

  class ApiClient {
    private client: AxiosInstance;

    constructor() {
      this.client = axios.create({
        baseURL: config.apiBaseUrl,
        timeout: 30000,
        headers: {
          'Content-Type': 'application/json',
        },
      });

      this.setupInterceptors();
    }

    private setupInterceptors() {
      // Request interceptor - add auth token
      this.client.interceptors.request.use(
        (config) => {
          const token = localStorage.getItem('accessToken');
          if (token) {
            config.headers.Authorization = `Bearer ${token}`;
          }
          // Add correlation ID
          config.headers['X-Correlation-ID'] = crypto.randomUUID();
          return config;
        },
        (error) => Promise.reject(error)
      );

      // Response interceptor - handle errors
      this.client.interceptors.response.use(
        (response) => response,
        async (error: AxiosError) => {
          if (error.response?.status === 401) {
            // Redirect to login
            window.location.href = '/login';
          }
          return Promise.reject(this.handleError(error));
        }
      );
    }

    private handleError(error: AxiosError) {
      if (error.response) {
        // Server responded with error
        return {
          message: error.response.data?.message || 'An error occurred',
          status: error.response.status,
          correlationId: error.response.headers['x-correlation-id'],
        };
      } else if (error.request) {
        // Network error
        return {
          message: 'Network error. Please check your connection.',
          status: 0,
        };
      }
      return { message: error.message, status: 0 };
    }

    async get<T>(url: string): Promise<T> {
      const response = await this.client.get<T>(url);
      return response.data;
    }

    async post<T>(url: string, data?: unknown): Promise<T> {
      const response = await this.client.post<T>(url, data);
      return response.data;
    }

    async put<T>(url: string, data?: unknown): Promise<T> {
      const response = await this.client.put<T>(url, data);
      return response.data;
    }

    async delete<T>(url: string): Promise<T> {
      const response = await this.client.delete<T>(url);
      return response.data;
    }
  }

  export const apiClient = new ApiClient();
  ```
- Create typed API service methods:
  ```typescript
  // src/services/api/auth.service.ts
  export const authService = {
    register: (data: RegisterRequest) =>
      apiClient.post<RegisterResponse>('/api/auth/register', data),

    verify: (data: VerifyRequest) =>
      apiClient.post<VerifyResponse>('/api/auth/verify', data),
  };
  ```

**Acceptance Criteria**:
- [x] API client makes requests to Backend API
- [x] Request interceptor adds JWT token
- [x] Response interceptor handles 401 errors
- [x] Correlation ID added to requests
- [x] Error handling centralized
- [x] TypeScript types for all requests/responses

**Definition of Done**:
- API client tested with mock server
- Error handling documented
- Team can use API client for all requests

---

#### FE-007: State Management Setup (1.50 days)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: FE-001

**Description**:
Setup state management for authentication and global app state.

**Technical Requirements**:
- **Option 1**: React Context API (recommended for MVP):
  ```typescript
  // src/context/AuthContext.tsx
  interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    login: (credentials: LoginCredentials) => Promise<void>;
    logout: () => void;
    register: (data: RegisterData) => Promise<void>;
  }

  export const AuthContext = createContext<AuthContextType | undefined>(undefined);

  export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);

    // Implementation...
  };
  ```
- **Option 2**: Zustand (lightweight alternative):
  ```typescript
  import { create } from 'zustand';

  interface AuthStore {
    user: User | null;
    setUser: (user: User | null) => void;
    // ...
  }

  export const useAuthStore = create<AuthStore>((set) => ({ /*...*/ }));
  ```
- Create custom hooks for consuming context:
  ```typescript
  export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
      throw new Error('useAuth must be used within AuthProvider');
    }
    return context;
  };
  ```

**Acceptance Criteria**:
- [x] Auth state management implemented
- [x] User state persists across page refresh
- [x] Custom hooks created for easy access
- [x] State updates trigger re-renders correctly
- [x] State management documented

**Definition of Done**:
- State management tested
- Usage examples documented
- Team trained on state access patterns

---

#### FE-008: Error Boundary Implementation (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: FE-001

**Description**:
Implement React Error Boundaries for graceful error handling.

**Technical Requirements**:
- Create Error Boundary component:
  ```typescript
  // src/components/common/ErrorBoundary.tsx
  class ErrorBoundary extends React.Component<Props, State> {
    constructor(props) {
      super(props);
      this.state = { hasError: false, error: null };
    }

    static getDerivedStateFromError(error) {
      return { hasError: true, error };
    }

    componentDidCatch(error, errorInfo) {
      // Log to error reporting service (e.g., Sentry)
      console.error('Error Boundary caught error:', error, errorInfo);
    }

    render() {
      if (this.state.hasError) {
        return (
          <div className="error-fallback">
            <h1>Something went wrong</h1>
            <p>{this.state.error?.message}</p>
            <button onClick={() => window.location.reload()}>
              Reload Page
            </button>
          </div>
        );
      }
      return this.props.children;
    }
  }
  ```
- Create error fallback UI components
- Wrap app in Error Boundary
- Add error logging (console for now, Sentry later)

**Acceptance Criteria**:
- [x] Error Boundary catches React errors
- [x] Error fallback UI displayed on crash
- [x] Error details logged
- [x] User can recover from error
- [x] Error Boundary documented

**Definition of Done**:
- Error handling tested
- Fallback UI reviewed
- Team aware of error boundaries

---

### Epic 1.2: Authentication & WebAuthn Integration (10.17 days)

#### FE-101: Install WebAuthn Passkey Library (1.00 day)
**Owner**: Senior Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-006

**Description**:
Install and configure WebAuthn library for passkey authentication.

**Technical Requirements**:
- Install **@simplewebauthn/browser**:
  ```bash
  npm install @simplewebauthn/browser
  ```
- Create WebAuthn service wrapper:
  ```typescript
  // src/services/auth/webauthn.service.ts
  import {
    startRegistration,
    startAuthentication,
    PublicKeyCredentialCreationOptionsJSON,
    PublicKeyCredentialRequestOptionsJSON
  } from '@simplewebauthn/browser';

  export class WebAuthnService {
    async registerPasskey(options: PublicKeyCredentialCreationOptionsJSON) {
      try {
        const credential = await startRegistration(options);
        return credential;
      } catch (error) {
        if (error.name === 'NotAllowedError') {
          throw new Error('User cancelled passkey registration');
        }
        throw error;
      }
    }

    async authenticatePasskey(options: PublicKeyCredentialRequestOptionsJSON) {
      try {
        const credential = await startAuthentication(options);
        return credential;
      } catch (error) {
        if (error.name === 'NotAllowedError') {
          throw new Error('User cancelled passkey authentication');
        }
        throw error;
      }
    }

    async isPlatformAuthenticatorAvailable(): Promise<boolean> {
      return window.PublicKeyCredential &&
             window.PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable();
    }
  }

  export const webAuthnService = new WebAuthnService();
  ```
- Create TypeScript types for WebAuthn responses

**Acceptance Criteria**:
- [x] @simplewebauthn/browser installed
- [x] WebAuthn service wrapper created
- [x] Platform authenticator availability check works
- [x] TypeScript types defined
- [x] Service documented

**Definition of Done**:
- Library installed and tested
- Service wrapper ready for use
- Browser compatibility documented

---

#### FE-102: Passkey Registration Flow UI (2.50 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-101, FE-104 (Auth Context)

**Description**:
Build user interface for passkey registration (sign up).

**Technical Requirements**:
- Create Register page component:
  ```typescript
  // src/pages/auth/RegisterPage.tsx
  const RegisterPage = () => {
    const [username, setUsername] = useState('');
    const [isRegistering, setIsRegistering] = useState(false);
    const { register } = useAuth();

    const handleRegister = async () => {
      setIsRegistering(true);
      try {
        // Step 1: Check platform authenticator availability
        const available = await webAuthnService.isPlatformAuthenticatorAvailable();
        if (!available) {
          toast.error('Passkeys not supported on this device');
          return;
        }

        // Step 2: Call backend to get registration options
        const options = await authService.getRegistrationOptions(username);

        // Step 3: Prompt user to create passkey
        const credential = await webAuthnService.registerPasskey(options);

        // Step 4: Send credential to backend
        await register({ username, credential });

        // Step 5: Redirect to wallet
        navigate('/wallet');
      } catch (error) {
        toast.error(error.message);
      } finally {
        setIsRegistering(false);
      }
    };

    return (
      <div className="register-page">
        <h1>Create Account</h1>
        <Input
          label="Username"
          value={username}
          onChange={setUsername}
        />
        <Button
          onClick={handleRegister}
          loading={isRegistering}
          disabled={!username}
        >
          Register with Passkey
        </Button>
      </div>
    );
  };
  ```
- Create registration flow states:
  - Initial form (username input)
  - Loading state (creating passkey)
  - Success state (redirect to wallet)
  - Error state (display error message)
- Add form validation (username 3-50 characters)
- Add loading indicators
- Add error handling with user-friendly messages

**Acceptance Criteria**:
- [x] Registration page UI complete
- [x] Username validation works
- [x] Passkey creation flow works on supported devices
- [x] Error messages displayed for failures
- [x] Loading states implemented
- [x] Redirects to wallet on success

**Definition of Done**:
- Registration tested on Chrome/Edge (passkey support)
- Error handling tested
- UI reviewed for UX

---

#### FE-103: Passkey Login Flow UI (2.50 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-101, FE-104 (Auth Context)

**Description**:
Build user interface for passkey login (sign in).

**Technical Requirements**:
- Create Login page component:
  ```typescript
  // src/pages/auth/LoginPage.tsx
  const LoginPage = () => {
    const [username, setUsername] = useState('');
    const [isLoggingIn, setIsLoggingIn] = useState(false);
    const { login } = useAuth();

    const handleLogin = async () => {
      setIsLoggingIn(true);
      try {
        // Step 1: Call backend to get authentication options
        const options = await authService.getAuthenticationOptions(username);

        // Step 2: Prompt user to use passkey
        const credential = await webAuthnService.authenticatePasskey(options);

        // Step 3: Send credential to backend for verification
        await login({ username, credential });

        // Step 4: Redirect to wallet
        navigate('/wallet');
      } catch (error) {
        toast.error(error.message);
      } finally {
        setIsLoggingIn(false);
      }
    };

    return (
      <div className="login-page">
        <h1>Welcome Back</h1>
        <Input
          label="Username"
          value={username}
          onChange={setUsername}
        />
        <Button
          onClick={handleLogin}
          loading={isLoggingIn}
          disabled={!username}
        >
          Login with Passkey
        </Button>
        <p>
          Don't have an account? <Link to="/register">Register</Link>
        </p>
      </div>
    );
  };
  ```
- Create login flow states (similar to registration)
- Add conditional rendering for passkey prompt
- Add "forgot passkey" flow (delete and re-register)

**Acceptance Criteria**:
- [x] Login page UI complete
- [x] Passkey authentication flow works
- [x] JWT token stored on successful login
- [x] Error messages displayed for failures
- [x] Redirects to wallet on success
- [x] "Register" link visible for new users

**Definition of Done**:
- Login tested on supported devices
- Token storage validated
- UI reviewed for UX

---

#### FE-104: Authentication Context and State Management (2.00 days)
**Owner**: Senior Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-007, FE-006

**Description**:
Implement authentication context with login, logout, and session management.

**Technical Requirements**:
- Create AuthContext with state:
  ```typescript
  // src/context/AuthContext.tsx
  interface User {
    id: string;
    username: string;
    walletAddress?: string;
  }

  interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (credentials: LoginCredentials) => Promise<void>;
    logout: () => void;
    register: (data: RegisterData) => Promise<void>;
    refreshToken: () => Promise<void>;
  }

  export const AuthProvider: React.FC = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
      // Check if user is already logged in (token in localStorage)
      const token = localStorage.getItem('accessToken');
      if (token) {
        // Validate token and fetch user info
        validateSession();
      } else {
        setIsLoading(false);
      }
    }, []);

    const login = async (credentials) => {
      const response = await authService.verify(credentials);
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      setUser(response.user);
    };

    const logout = () => {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      setUser(null);
      navigate('/login');
    };

    const register = async (data) => {
      const response = await authService.register(data);
      // Auto-login after registration
      await login(response);
    };

    return (
      <AuthContext.Provider value={{
        user,
        isAuthenticated: !!user,
        isLoading,
        login,
        logout,
        register
      }}>
        {children}
      </AuthContext.Provider>
    );
  };
  ```
- Implement token refresh logic (before token expires)
- Store tokens in localStorage (consider httpOnly cookies for production)

**Acceptance Criteria**:
- [x] AuthContext provides user state
- [x] Login/logout functions work
- [x] User state persists across page refresh
- [x] Token refresh implemented
- [x] isAuthenticated flag accurate

**Definition of Done**:
- Auth context tested
- Session persistence validated
- Token refresh tested

---

#### FE-105: Protected Route Wrapper (1.00 day)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-104

**Description**:
Create route wrapper to protect authenticated pages.

**Technical Requirements**:
- Create ProtectedRoute component:
  ```typescript
  // src/components/auth/ProtectedRoute.tsx
  export const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { isAuthenticated, isLoading } = useAuth();
    const location = useLocation();

    if (isLoading) {
      return <LoadingSpinner />;
    }

    if (!isAuthenticated) {
      // Redirect to login, but save the attempted URL
      return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <>{children}</>;
  };
  ```
- Handle redirect after login (return to original page)
- Add loading state while checking authentication

**Acceptance Criteria**:
- [x] Unauthenticated users redirected to login
- [x] Original URL saved for post-login redirect
- [x] Loading state shown during auth check
- [x] Protected pages inaccessible without auth

**Definition of Done**:
- Route protection tested
- Redirect flow validated
- Team aware of usage

---

#### FE-106: User Profile Display Component (1.17 days)
**Owner**: Frontend Engineer
**Priority**: P2 - Medium
**Dependencies**: FE-104

**Description**:
Create user profile component to display username and wallet address.

**Technical Requirements**:
- Create UserProfile component:
  ```typescript
  // src/components/layout/UserProfile.tsx
  export const UserProfile = () => {
    const { user, logout } = useAuth();

    return (
      <div className="user-profile">
        <Avatar username={user.username} />
        <div>
          <p className="username">{user.username}</p>
          {user.walletAddress && (
            <p className="wallet-address">
              {truncateAddress(user.walletAddress)}
              <CopyButton value={user.walletAddress} />
            </p>
          )}
        </div>
        <Button onClick={logout} variant="ghost">
          Logout
        </Button>
      </div>
    );
  };
  ```
- Add avatar component (initials-based)
- Add copy-to-clipboard for wallet address
- Add logout button

**Acceptance Criteria**:
- [x] User profile displays username
- [x] Wallet address displayed (if available)
- [x] Copy button works for wallet address
- [x] Logout button works
- [x] Responsive design

**Definition of Done**:
- Component tested
- UI reviewed
- Accessible

---

### Epic 1.3: Wallet & Transfer UI (6.00 days)

#### FE-201: Wallet Creation UI (1.50 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-104, FE-006

**Description**:
Build UI for creating a new wallet (Circle Smart Account).

**Technical Requirements**:
- Create WalletCreation component:
  ```typescript
  // src/components/wallet/WalletCreation.tsx
  export const WalletCreation = () => {
    const [isCreating, setIsCreating] = useState(false);
    const { user } = useAuth();

    const handleCreateWallet = async () => {
      setIsCreating(true);
      try {
        const wallet = await walletService.createWallet();
        toast.success(`Wallet created: ${wallet.address}`);
        // Redirect to wallet dashboard
        navigate('/wallet');
      } catch (error) {
        toast.error(error.message);
      } finally {
        setIsCreating(false);
      }
    };

    return (
      <div className="wallet-creation">
        <h2>Create Your Wallet</h2>
        <p>Create a secure, gasless wallet for USDC transfers</p>
        <Button onClick={handleCreateWallet} loading={isCreating}>
          Create Wallet
        </Button>
      </div>
    );
  };
  ```
- Add loading state with progress indicator
- Display wallet address after creation
- Auto-redirect to wallet dashboard

**Acceptance Criteria**:
- [x] Wallet creation button works
- [x] Loading state displayed during creation
- [x] Wallet address displayed after creation
- [x] Error handling implemented
- [x] Redirects to dashboard on success

**Definition of Done**:
- Integration tested with Backend API
- UI reviewed
- Error handling tested

---

#### FE-202: Wallet Dashboard Component (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-201

**Description**:
Build main wallet dashboard showing balance and recent transactions.

**Technical Requirements**:
- Create WalletDashboard component:
  ```typescript
  // src/pages/wallet/Dashboard.tsx
  export const WalletDashboard = () => {
    const { user } = useAuth();
    const [balance, setBalance] = useState<string>('0');
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
      fetchBalance();
    }, []);

    const fetchBalance = async () => {
      try {
        const balanceData = await walletService.getBalance(user.walletAddress);
        setBalance(balanceData.formattedBalance);
      } catch (error) {
        toast.error('Failed to fetch balance');
      } finally {
        setIsLoading(false);
      }
    };

    return (
      <div className="wallet-dashboard">
        <WalletHeader address={user.walletAddress} />
        <BalanceCard balance={balance} isLoading={isLoading} />
        <QuickActions />
        <RecentTransactions />
      </div>
    );
  };
  ```
- Create sub-components:
  - WalletHeader (address, copy button)
  - BalanceCard (USDC balance, refresh button)
  - QuickActions (Send, Receive buttons)
  - RecentTransactions (last 5 transactions)
- Add pull-to-refresh functionality
- Add auto-refresh every 30 seconds

**Acceptance Criteria**:
- [x] Dashboard displays wallet address
- [x] Balance displayed correctly
- [x] Refresh button works
- [x] Quick action buttons present
- [x] Recent transactions displayed
- [x] Loading states implemented

**Definition of Done**:
- Dashboard tested with real data
- UI reviewed for UX
- Responsive design tested

---

#### FE-203: Transfer Form UI (2.00 days)
**Owner**: Frontend Engineer
**Priority**: P0 - Critical
**Dependencies**: FE-202

**Description**:
Build transfer form for sending USDC to another address.

**Technical Requirements**:
- Create TransferForm component:
  ```typescript
  // src/components/wallet/TransferForm.tsx
  export const TransferForm = () => {
    const [recipient, setRecipient] = useState('');
    const [amount, setAmount] = useState('');
    const [isSending, setIsSending] = useState(false);

    const handleTransfer = async () => {
      setIsSending(true);
      try {
        // Validate inputs
        if (!isValidAddress(recipient)) {
          toast.error('Invalid recipient address');
          return;
        }

        // Submit transfer
        const result = await walletService.transfer({
          toAddress: recipient,
          amount: parseFloat(amount),
        });

        toast.success(`Transfer submitted: ${result.userOpHash}`);

        // Redirect to transaction status page
        navigate(`/wallet/transaction/${result.userOpHash}`);
      } catch (error) {
        toast.error(error.message);
      } finally {
        setIsSending(false);
      }
    };

    return (
      <form className="transfer-form">
        <Input
          label="Recipient Address"
          value={recipient}
          onChange={setRecipient}
          placeholder="0x..."
          validation={isValidAddress}
        />
        <Input
          label="Amount (USDC)"
          type="number"
          value={amount}
          onChange={setAmount}
          placeholder="0.00"
        />
        <BalanceInfo available={userBalance} />
        <Button
          onClick={handleTransfer}
          loading={isSending}
          disabled={!recipient || !amount}
        >
          Send USDC
        </Button>
      </form>
    );
  };
  ```
- Add form validation:
  - Recipient address format (Ethereum address)
  - Amount > 0 and <= balance
  - Prevent duplicate submissions
- Add "Max" button to send entire balance
- Display estimated gas (0.00 for gasless)

**Acceptance Criteria**:
- [x] Transfer form validates inputs
- [x] Transfer submits to Backend API
- [x] UserOpHash returned and displayed
- [x] Error messages displayed for failures
- [x] Loading state during submission
- [x] Redirects to transaction status

**Definition of Done**:
- Transfer tested with testnet USDC
- Validation tested
- UI reviewed

---

#### FE-204: Transaction Status Display (0.50 days)
**Owner**: Frontend Engineer
**Priority**: P1 - High
**Dependencies**: FE-203

**Description**:
Create component to display transaction status (pending, confirmed, failed).

**Technical Requirements**:
- Create TransactionStatus component:
  ```typescript
  // src/components/wallet/TransactionStatus.tsx
  export const TransactionStatus = ({ userOpHash }: Props) => {
    const [status, setStatus] = useState<TransactionStatus>('pending');
    const [txHash, setTxHash] = useState<string | null>(null);

    useEffect(() => {
      // Poll for transaction status every 5 seconds
      const interval = setInterval(async () => {
        const result = await walletService.getTransactionStatus(userOpHash);
        setStatus(result.status);
        setTxHash(result.transactionHash);

        if (result.status !== 'pending') {
          clearInterval(interval);
        }
      }, 5000);

      return () => clearInterval(interval);
    }, [userOpHash]);

    return (
      <div className="transaction-status">
        <StatusBadge status={status} />
        {status === 'pending' && <LoadingSpinner />}
        {txHash && (
          <a
            href={`https://amoy.polygonscan.com/tx/${txHash}`}
            target="_blank"
          >
            View on Explorer
          </a>
        )}
      </div>
    );
  };
  ```
- Add status badges (pending, confirmed, failed)
- Add link to Polygon Amoy block explorer
- Add polling for status updates (every 5 seconds)

**Acceptance Criteria**:
- [x] Transaction status displayed correctly
- [x] Polling updates status automatically
- [x] Block explorer link works
- [x] Status changes reflected in UI
- [x] Polling stops when transaction confirmed/failed

**Definition of Done**:
- Component tested with real transactions
- UI reviewed
- Polling logic tested

---

## Daily Milestone Plan

### Days 1-2 (Sprint Start)
**Focus**: Project setup and infrastructure

**Tasks**:
- FE-001: Project structure ✅
- FE-002: TypeScript/ESLint config ✅
- FE-005: Environment configuration ✅
- FE-003: Tailwind CSS setup (started)

**Deliverable**: Team can run React app locally with proper structure

---

### Days 3-4
**Focus**: Routing, API client, state management

**Tasks**:
- FE-003: Tailwind CSS (completed) ✅
- FE-004: React Router setup ✅
- FE-006: API client service ✅
- FE-007: State management ✅

**Deliverable**: API client can communicate with Backend, routing works

---

### Days 5-6 (Mid-Sprint)
**Focus**: WebAuthn integration and authentication

**Tasks**:
- FE-008: Error boundaries ✅
- FE-101: WebAuthn library installed ✅
- FE-104: Auth context (started)
- FE-102: Registration UI (started)

**Checkpoint Meeting**: Demo project structure, API client, routing

**Deliverable**: WebAuthn library ready, auth context in progress

---

### Days 7-8
**Focus**: Authentication UI completion

**Tasks**:
- FE-104: Auth context (completed) ✅
- FE-102: Registration UI (completed) ✅
- FE-103: Login UI ✅
- FE-105: Protected routes ✅
- FE-106: User profile component ✅

**Deliverable**: Users can register and login with passkeys

---

### Days 9-10 (Sprint End)
**Focus**: Wallet UI and integration

**Tasks**:
- FE-201: Wallet creation UI ✅
- FE-202: Wallet dashboard ✅
- FE-203: Transfer form UI ✅
- FE-204: Transaction status ✅

**Sprint Review**: Demo passkey auth flow, wallet creation, transfer UI

**Deliverable**: Complete UI for wallet management

---

## External Dependencies

### From Backend Team

| Dependency | Required By | Impact if Delayed | Status |
|------------|-------------|-------------------|--------|
| Swagger API documentation | Day 3 | Cannot design API client | Day 2-3 |
| Authentication endpoints | Day 7 | Cannot build login/register UI | Day 7-8 |
| Wallet creation endpoint | Day 8 | Cannot build wallet UI | Day 8-9 |
| Transfer endpoint | Day 9 | Cannot build transfer UI | Day 9-10 |
| CORS configuration | Day 3 | API calls blocked | Day 2 |
| JWT token format | Day 7 | Cannot store sessions | Day 7 |
| Error response format | Day 3 | Cannot handle errors | Day 2 |

**Mitigation**: Use mock API responses until Backend endpoints ready

---

### From QA Team

| Dependency | Required By | Impact if Delayed | Status |
|------------|-------------|-------------------|--------|
| data-testid conventions | Day 7 | QA tests fragile | Day 7 |
| Accessibility requirements | Day 4 | Cannot design accessible UI | Day 4 |
| Component testing requirements | Day 5 | Cannot write proper tests | Day 5 |

**Mitigation**: Follow standard accessibility practices, add data-testid later

---

## Risks, Dependencies & Mitigation

### Technical Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| WebAuthn browser compatibility issues | MEDIUM | Authentication blocked | Test on Chrome/Edge/Safari, provide fallback message |
| Backend API not ready by Day 7 | MEDIUM | Cannot integrate auth | Use mock API with MSW, switch to real API later |
| Passkey UX unclear to users | HIGH | User confusion | Add clear instructions, video tutorial |
| CORS issues with Backend | LOW | API calls fail | Coordinate with Backend early, test on Day 3 |

### Team Capacity Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| WebAuthn integration more complex than expected | MEDIUM | 1-2 days delay | Allocate senior engineer, consult documentation |
| UI/UX changes requested | LOW | 0.5-1 day delay | Keep components modular, use design system |

---

## Success Criteria

### Functional Success Metrics

- [ ] React app runs on port 3000
- [ ] Users can register with passkeys
- [ ] Users can login with passkeys
- [ ] Protected routes redirect to login
- [ ] Wallet creation UI works
- [ ] Transfer form submits to Backend
- [ ] Transaction status displays correctly
- [ ] Responsive design on mobile/tablet/desktop

### Quality Gates

- [ ] All components use TypeScript strict mode
- [ ] ESLint passes with 0 errors
- [ ] Component tests written (React Testing Library)
- [ ] data-testid attributes added for QA
- [ ] Accessibility: Keyboard navigation works
- [ ] Accessibility: ARIA labels present
- [ ] No console errors in production build
- [ ] Code reviewed and approved

### Sprint Review Demo Checklist

1. Show project structure and component organization
2. Demonstrate React Router navigation
3. Register new user with passkey (Chrome/Edge)
4. Login with passkey
5. Show protected route redirect
6. Create wallet (call Backend API)
7. Display wallet dashboard with balance
8. Fill transfer form (UI only, may not submit if Backend not ready)
9. Show responsive design on mobile viewport
10. Show error handling (network error, validation)

---

## Handoff Points

### To Backend Team (Day 3)

**Deliverables**:
- API client implementation (how we'll call endpoints)
- Expected request/response formats (TypeScript types)
- Error handling expectations
- CORS origins needed (http://localhost:3000)

**Coordination**:
- Review Swagger documentation together
- Agree on JWT token structure
- Define error response format
- Test API calls with Postman first

---

### To QA Team (Day 8)

**Deliverables**:
- Component structure and page object models
- data-testid conventions:
  ```typescript
  <button data-testid="login-button">Login</button>
  <input data-testid="username-input" />
  <div data-testid="wallet-balance">100.50 USDC</div>
  ```
- Accessibility checklist completed
- Test user credentials for QA
- Frontend app deployed to dev environment

**Testing Scenarios**:
1. Register with passkey (Chrome/Edge required)
2. Login with passkey
3. Create wallet
4. View wallet dashboard
5. Submit transfer (may be mock if Backend not ready)
6. View transaction status
7. Logout and session cleared

---

## Definition of Done (Sprint 1)

### Code Quality

- [x] All code uses TypeScript strict mode
- [x] ESLint passes with 0 errors, 0 warnings
- [x] Components follow React best practices
- [x] No any types (use proper TypeScript types)
- [x] Accessibility: Keyboard navigation works
- [x] Accessibility: ARIA labels present
- [x] Code reviewed and approved

### Testing

- [x] Component tests written (React Testing Library) - **at least 5 components**
- [x] Integration tests for auth flow
- [x] Manual testing on Chrome, Edge, Firefox
- [x] Responsive design tested (mobile, tablet, desktop)
- [x] Error handling tested (network errors, validation)

### Documentation

- [x] Component documentation (JSDoc comments)
- [x] README.md updated with setup instructions
- [x] API client usage documented
- [x] WebAuthn integration documented
- [x] data-testid conventions documented for QA

### Integration

- [x] API client tested with Backend (or mocked)
- [x] CORS working (no preflight errors)
- [x] JWT tokens stored and sent correctly
- [x] Error responses handled gracefully
- [x] Loading states implemented

### Security

- [x] No hardcoded secrets
- [x] Environment variables used for config
- [x] JWT tokens stored in localStorage (httpOnly cookies for production)
- [x] Input validation on all forms
- [x] XSS protection (React escapes by default)

---

## Sprint Retrospective Topics

### What Went Well
- WebAuthn integration smooth?
- React Router setup easy?
- Backend API integration clear?

### What Could Be Improved
- Estimation accuracy?
- Component reusability?
- Testing coverage?

### Action Items for Sprint 2
- Increase component test coverage to 80%
- Refine UI/UX based on user feedback
- Implement transaction history page
- Add wallet balance refresh animation
- Integrate swap functionality UI

---

## Next Steps After Sprint 1

### Sprint 2 Focus (Phase 1 Completion)

**Frontend Tasks for Sprint 2**:
1. Transaction history page with pagination
2. Transaction detail view
3. QR code generation for wallet address
4. Enhanced error handling and retry logic
5. Wallet balance chart (optional)
6. Dark mode support (optional)
7. Notification system for transaction confirmations
8. User settings page
9. Help documentation / FAQ page
10. Performance optimization (code splitting, lazy loading)

**Estimated Effort**: ~18-20 days

---

## Team Roster

| Name | Role | Seniority | Capacity (days) | Primary Focus |
|------|------|-----------|-----------------|---------------|
| TBD | Frontend Lead | Senior | 10 days | WebAuthn, Architecture |
| TBD | Frontend Engineer 1 | Mid-level | 10 days | UI Components, Routing |
| TBD | Frontend Engineer 2 (optional) | Mid-level | 10 days | API Integration, State Management |

**Total Frontend Capacity**: 20-30 days

---

## Key Contacts

| Role | Name | Email | Slack | Escalation |
|------|------|-------|-------|------------|
| Frontend Lead | TBD | TBD | @frontend-lead | Team Lead |
| Backend Lead | TBD | TBD | @backend-lead | Team Lead |
| QA Lead | TBD | TBD | @qa-lead | Team Lead |

---

## Useful Resources

### WebAuthn Documentation
- **SimpleWebAuthn Docs**: https://simplewebauthn.dev/docs/packages/browser
- **WebAuthn Guide**: https://webauthn.guide/
- **MDN WebAuthn API**: https://developer.mozilla.org/en-US/docs/Web/API/Web_Authentication_API

### React & TypeScript
- **React Docs**: https://react.dev/
- **TypeScript Handbook**: https://www.typescriptlang.org/docs/
- **Vite Documentation**: https://vitejs.dev/

### UI Libraries
- **shadcn/ui**: https://ui.shadcn.com/ (recommended)
- **Headless UI**: https://headlessui.com/
- **Tailwind CSS**: https://tailwindcss.com/

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-01-06 | Frontend Lead | Initial Frontend Sprint 1 Plan created |

---

**SPRINT 1 STATUS**: **READY TO START**

**NEXT STEPS**:
1. Team kickoff meeting (Day 1, 9:00 AM)
2. Setup development environment (Day 1 morning)
3. Begin FE-001, FE-002, FE-005 in parallel (Day 1 afternoon)
4. Day 3: API contract review with Backend team
5. Daily standups at 9:00 AM starting Day 2

---

**End of Sprint 1 Frontend Plan**
