import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import { Token } from '@/constants/tokens';
import { SwapQuote, DEFAULT_SLIPPAGE } from '@/types/swap';
import { executeSwap } from '@/api/swapApi';

interface SwapState {
  fromToken: Token | null;
  toToken: Token | null;
  fromAmount: string;
  toAmount: string;
  slippageTolerance: number;
  quote: SwapQuote | null;
  isExecuting: boolean;
  error: string | null;
  lastSwapId: string | null;
}

interface SwapActions {
  setFromToken: (token: Token | null) => void;
  setToToken: (token: Token | null) => void;
  setFromAmount: (amount: string) => void;
  setToAmount: (amount: string) => void;
  setSlippageTolerance: (slippage: number) => void;
  setQuote: (quote: SwapQuote | null) => void;
  flipTokens: () => void;
  executeSwapAction: () => Promise<string>;
  setError: (error: string | null) => void;
  reset: () => void;
}

type SwapStore = SwapState & SwapActions;

const initialState: SwapState = {
  fromToken: null,
  toToken: null,
  fromAmount: '',
  toAmount: '',
  slippageTolerance: DEFAULT_SLIPPAGE,
  quote: null,
  isExecuting: false,
  error: null,
  lastSwapId: null,
};

export const useSwapStore = create<SwapStore>()(
  devtools(
    persist(
      (set, get) => ({
        ...initialState,

        setFromToken: (token) => {
          set({ fromToken: token, error: null }, false, 'swap/setFromToken');
        },

        setToToken: (token) => {
          set({ toToken: token, error: null }, false, 'swap/setToToken');
        },

        setFromAmount: (amount) => {
          set({ fromAmount: amount, error: null }, false, 'swap/setFromAmount');
        },

        setToAmount: (amount) => {
          set({ toAmount: amount }, false, 'swap/setToAmount');
        },

        setSlippageTolerance: (slippage) => {
          set({ slippageTolerance: slippage }, false, 'swap/setSlippageTolerance');
          // Persist to localStorage
          localStorage.setItem('slippageTolerance', slippage.toString());
        },

        setQuote: (quote) => {
          set({ quote }, false, 'swap/setQuote');
        },

        flipTokens: () => {
          const { fromToken, toToken } = get();
          set(
            {
              fromToken: toToken,
              toToken: fromToken,
              fromAmount: '',
              toAmount: '',
              quote: null,
              error: null,
            },
            false,
            'swap/flipTokens'
          );
        },

        executeSwapAction: async () => {
          const { fromToken, toToken, fromAmount, slippageTolerance } = get();

          if (!fromToken || !toToken || !fromAmount) {
            const error = 'Invalid swap parameters';
            set({ error }, false, 'swap/executeSwap/error');
            throw new Error(error);
          }

          const amount = parseFloat(fromAmount);
          if (isNaN(amount) || amount <= 0) {
            const error = 'Invalid amount';
            set({ error }, false, 'swap/executeSwap/error');
            throw new Error(error);
          }

          set({ isExecuting: true, error: null }, false, 'swap/executeSwap/start');

          try {
            const result = await executeSwap({
              fromToken: fromToken.address,
              toToken: toToken.address,
              fromAmount: amount,
              slippageTolerance,
            });

            set(
              {
                isExecuting: false,
                lastSwapId: result.swapId,
              },
              false,
              'swap/executeSwap/success'
            );

            return result.swapId;
          } catch (error) {
            const errorMessage =
              error instanceof Error ? error.message : 'Swap execution failed';
            set(
              { error: errorMessage, isExecuting: false },
              false,
              'swap/executeSwap/error'
            );
            throw error;
          }
        },

        setError: (error) => {
          set({ error }, false, 'swap/setError');
        },

        reset: () => {
          set(
            {
              fromToken: null,
              toToken: null,
              fromAmount: '',
              toAmount: '',
              quote: null,
              isExecuting: false,
              error: null,
            },
            false,
            'swap/reset'
          );
        },
      }),
      {
        name: 'swap-storage',
        partialize: (state) => ({
          slippageTolerance: state.slippageTolerance,
        }),
      }
    ),
    { name: 'SwapStore' }
  )
);
