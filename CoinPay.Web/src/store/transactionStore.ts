import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import { Transaction } from '@/types';

interface TransactionState {
  transactions: Transaction[];
  currentTransaction: Transaction | null;
  isLoading: boolean;
  error: string | null;
}

interface TransactionActions {
  setTransactions: (transactions: Transaction[]) => void;
  setCurrentTransaction: (transaction: Transaction | null) => void;
  addTransaction: (transaction: Transaction) => void;
  updateTransaction: (id: number, updates: Partial<Transaction>) => void;
  removeTransaction: (id: number) => void;
  setLoading: (isLoading: boolean) => void;
  setError: (error: string | null) => void;
  clearTransactions: () => void;
  clearError: () => void;
}

type TransactionStore = TransactionState & TransactionActions;

export const useTransactionStore = create<TransactionStore>()(
  devtools(
    (set) => ({
      // Initial state
      transactions: [],
      currentTransaction: null,
      isLoading: false,
      error: null,

      // Actions
      setTransactions: (transactions) =>
        set({ transactions }, false, 'transaction/setTransactions'),

      setCurrentTransaction: (transaction) =>
        set(
          { currentTransaction: transaction },
          false,
          'transaction/setCurrentTransaction'
        ),

      addTransaction: (transaction) =>
        set(
          (state) => ({
            transactions: [transaction, ...state.transactions],
          }),
          false,
          'transaction/addTransaction'
        ),

      updateTransaction: (id, updates) =>
        set(
          (state) => ({
            transactions: state.transactions.map((tx) =>
              tx.id === id ? { ...tx, ...updates } : tx
            ),
          }),
          false,
          'transaction/updateTransaction'
        ),

      removeTransaction: (id) =>
        set(
          (state) => ({
            transactions: state.transactions.filter((tx) => tx.id !== id),
          }),
          false,
          'transaction/removeTransaction'
        ),

      setLoading: (isLoading) =>
        set({ isLoading }, false, 'transaction/setLoading'),

      setError: (error) =>
        set({ error }, false, 'transaction/setError'),

      clearTransactions: () =>
        set(
          {
            transactions: [],
            currentTransaction: null,
            error: null,
          },
          false,
          'transaction/clearTransactions'
        ),

      clearError: () =>
        set({ error: null }, false, 'transaction/clearError'),
    }),
    { name: 'TransactionStore' }
  )
);
