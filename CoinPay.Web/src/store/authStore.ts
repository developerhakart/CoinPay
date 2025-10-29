import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import { User } from '@/types';
import { env } from '@/config/env';

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

interface AuthActions {
  setUser: (user: User | null) => void;
  setToken: (token: string | null) => void;
  setLoading: (isLoading: boolean) => void;
  setError: (error: string | null) => void;
  login: (user: User, token: string) => void;
  loginWithPasskey: (username: string, password?: string) => Promise<void>;
  registerWithPasskey: (username: string, password: string) => Promise<void>;
  logout: () => void;
  clearError: () => void;
}

type AuthStore = AuthState & AuthActions;

export const useAuthStore = create<AuthStore>()(
  devtools(
    persist(
      (set, get) => ({
        // Initial state
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
        error: null,

        // Actions
        setUser: (user) =>
          set({ user, isAuthenticated: !!user }, false, 'auth/setUser'),

        setToken: (token) =>
          set({ token, isAuthenticated: !!token }, false, 'auth/setToken'),

        setLoading: (isLoading) =>
          set({ isLoading }, false, 'auth/setLoading'),

        setError: (error) =>
          set({ error }, false, 'auth/setError'),

        login: (user, token) => {
          localStorage.setItem('auth_token', token);
          localStorage.setItem('auth_user', JSON.stringify(user));
          set(
            {
              user,
              token,
              isAuthenticated: true,
              error: null,
            },
            false,
            'auth/login'
          );
        },

        loginWithPasskey: async (username: string, _password?: string) => {
          set({ isLoading: true, error: null }, false, 'auth/loginWithPasskey/start');
          try {
            // Step 1: Initiate login (get challenge)
            const initiateResponse = await fetch(`${env.apiBaseUrl}/api/auth/login/initiate`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ username }),
            });

            if (!initiateResponse.ok) {
              throw new Error('User not found or login failed');
            }

            const { challengeId, challenge } = await initiateResponse.json();

            // For MVP: Simulate passkey signing (in production, use WebAuthn API)
            const mockSignature = {
              credentialId: `cred_${username}`,
              authenticatorData: btoa(JSON.stringify({ challenge, username })),
              signature: btoa(JSON.stringify({ username, timestamp: Date.now() })),
            };

            // Step 2: Complete login
            const completeResponse = await fetch(`${env.apiBaseUrl}/api/auth/login/complete`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({
                username,
                challengeId,
                credentialId: mockSignature.credentialId,
                authenticatorData: mockSignature.authenticatorData,
                signature: mockSignature.signature,
              }),
            });

            if (!completeResponse.ok) {
              const error = await completeResponse.json();
              throw new Error(error.error || 'Login failed');
            }

            const { token: authToken, username: returnedUsername, walletAddress } = await completeResponse.json();

            // Fetch full user profile
            const profileResponse = await fetch(`${env.apiBaseUrl}/api/me`, {
              headers: {
                'Authorization': `Bearer ${authToken}`,
              },
            });

            let userData: User;
            if (profileResponse.ok) {
              userData = await profileResponse.json();
            } else {
              // Fallback user data from login response
              userData = {
                id: 0,
                username: returnedUsername,
                walletAddress
              };
            }

            // Use the login action to set user and token
            get().login(userData, authToken);
            set({ isLoading: false }, false, 'auth/loginWithPasskey/success');
          } catch (error) {
            const errorMessage = error instanceof Error ? error.message : 'Login failed';
            set({ error: errorMessage, isLoading: false }, false, 'auth/loginWithPasskey/error');
            throw error;
          }
        },

        registerWithPasskey: async (username: string, password: string) => {
          set({ isLoading: true, error: null }, false, 'auth/registerWithPasskey/start');
          try {
            // Step 1: Check if username exists
            const checkResponse = await fetch(`${env.apiBaseUrl}/api/auth/check-username`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ username }),
            });

            if (!checkResponse.ok) {
              throw new Error('Failed to check username');
            }

            const { exists } = await checkResponse.json();
            if (exists) {
              throw new Error('Username already taken');
            }

            // Step 2: Initiate registration (get challenge)
            const initiateResponse = await fetch(`${env.apiBaseUrl}/api/auth/register/initiate`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ username }),
            });

            if (!initiateResponse.ok) {
              throw new Error('Failed to initiate registration');
            }

            const { challengeId, challenge, userId } = await initiateResponse.json();

            // For MVP: Simulate passkey creation (in production, use WebAuthn API)
            const mockCredential = {
              credentialId: `cred_${Date.now()}`,
              publicKey: btoa(JSON.stringify({ username, timestamp: Date.now() })),
              authenticatorData: btoa(JSON.stringify({ challenge, userId })),
            };

            // Step 3: Complete registration
            const completeResponse = await fetch(`${env.apiBaseUrl}/api/auth/register/complete`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({
                username,
                challengeId,
                credentialId: mockCredential.credentialId,
                publicKey: mockCredential.publicKey,
                authenticatorData: mockCredential.authenticatorData,
              }),
            });

            if (!completeResponse.ok) {
              const error = await completeResponse.json();
              throw new Error(error.error || 'Registration failed');
            }

            await completeResponse.json();

            // Auto-login after registration
            await get().loginWithPasskey(username, password);

            set({ isLoading: false }, false, 'auth/registerWithPasskey/success');
          } catch (error) {
            const errorMessage = error instanceof Error ? error.message : 'Registration failed';
            set({ error: errorMessage, isLoading: false }, false, 'auth/registerWithPasskey/error');
            throw error;
          }
        },

        logout: () => {
          localStorage.removeItem('auth_token');
          localStorage.removeItem('auth_user');
          set(
            {
              user: null,
              token: null,
              isAuthenticated: false,
              error: null,
            },
            false,
            'auth/logout'
          );
        },

        clearError: () =>
          set({ error: null }, false, 'auth/clearError'),
      }),
      {
        name: 'auth-storage',
        partialize: (state) => ({
          user: state.user,
          token: state.token,
        }),
      }
    ),
    { name: 'AuthStore' }
  )
);
