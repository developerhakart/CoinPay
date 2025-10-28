import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import { Wallet } from '@/types';

interface WalletState {
  wallet: Wallet | null;
  balance: number;
  currency: string;
  isLoading: boolean;
  error: string | null;
}

interface WalletActions {
  setWallet: (wallet: Wallet | null) => void;
  setBalance: (balance: number) => void;
  setLoading: (isLoading: boolean) => void;
  setError: (error: string | null) => void;
  clearWallet: () => void;
  clearError: () => void;
}

type WalletStore = WalletState & WalletActions;

export const useWalletStore = create<WalletStore>()(
  devtools(
    (set) => ({
      // Initial state
      wallet: null,
      balance: 0,
      currency: 'USDC',
      isLoading: false,
      error: null,

      // Actions
      setWallet: (wallet) =>
        set(
          {
            wallet,
            balance: wallet?.balance || 0,
            currency: wallet?.currency || 'USDC',
          },
          false,
          'wallet/setWallet'
        ),

      setBalance: (balance) =>
        set({ balance }, false, 'wallet/setBalance'),

      setLoading: (isLoading) =>
        set({ isLoading }, false, 'wallet/setLoading'),

      setError: (error) =>
        set({ error }, false, 'wallet/setError'),

      clearWallet: () =>
        set(
          {
            wallet: null,
            balance: 0,
            currency: 'USDC',
            error: null,
          },
          false,
          'wallet/clearWallet'
        ),

      clearError: () =>
        set({ error: null }, false, 'wallet/clearError'),
    }),
    { name: 'WalletStore' }
  )
);
