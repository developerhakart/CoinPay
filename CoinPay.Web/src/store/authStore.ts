import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import { User } from '@/types';

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
  logout: () => void;
  clearError: () => void;
}

type AuthStore = AuthState & AuthActions;

export const useAuthStore = create<AuthStore>()(
  devtools(
    persist(
      (set) => ({
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
