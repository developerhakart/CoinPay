import { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface User {
  id: number;
  username: string;
  walletAddress?: string;
  circleUserId?: string;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Load token and user from localStorage on mount
  useEffect(() => {
    const savedToken = localStorage.getItem('auth_token');
    const savedUser = localStorage.getItem('auth_user');

    if (savedToken && savedUser) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
    }
    setIsLoading(false);
  }, []);

  const register = async (username: string, password: string) => {
    setIsLoading(true);
    try {
      // Step 1: Check if username exists
      const checkResponse = await fetch('http://localhost:5100/api/auth/check-username', {
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
      const initiateResponse = await fetch('http://localhost:5100/api/auth/register/initiate', {
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
      const completeResponse = await fetch('http://localhost:5100/api/auth/register/complete', {
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
      await login(username, password);

      alert('Registration successful! You are now logged in.');
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (username: string, _password?: string) => {
    setIsLoading(true);
    try {
      // Step 1: Initiate login (get challenge)
      const initiateResponse = await fetch('http://localhost:5100/api/auth/login/initiate', {
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
      const completeResponse = await fetch('http://localhost:5100/api/auth/login/complete', {
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
      const profileResponse = await fetch('http://localhost:5100/api/me', {
        headers: {
          'Authorization': `Bearer ${authToken}`,
        },
      });

      if (profileResponse.ok) {
        const userProfile = await profileResponse.json();
        setUser(userProfile);
        localStorage.setItem('auth_user', JSON.stringify(userProfile));
      } else {
        // Fallback user data from login response
        const userData = {
          id: 0,
          username: returnedUsername,
          walletAddress
        };
        setUser(userData);
        localStorage.setItem('auth_user', JSON.stringify(userData));
      }

      setToken(authToken);
      localStorage.setItem('auth_token', authToken);
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        login,
        register,
        logout,
        isAuthenticated: !!token && !!user,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
