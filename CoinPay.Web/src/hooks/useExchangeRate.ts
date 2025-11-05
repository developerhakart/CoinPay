import { useQuery } from '@tanstack/react-query';
import { getSwapQuote } from '@/api/swapApi';
import { SwapQuote } from '@/types/swap';

interface UseExchangeRateOptions {
  fromToken: string | null;
  toToken: string | null;
  amount: number;
  slippage: number;
}

/**
 * Hook to fetch exchange rates and quotes
 * Auto-refreshes every 30 seconds
 */
export const useExchangeRate = ({
  fromToken,
  toToken,
  amount,
  slippage,
}: UseExchangeRateOptions) => {
  return useQuery<SwapQuote>({
    queryKey: ['exchangeRate', fromToken, toToken, amount, slippage],
    queryFn: async () => {
      if (!fromToken || !toToken || amount <= 0) {
        throw new Error('Invalid parameters');
      }

      return await getSwapQuote(fromToken, toToken, amount, slippage);
    },
    enabled: !!fromToken && !!toToken && amount > 0,
    refetchInterval: 30000, // Refresh every 30 seconds
    staleTime: 5000, // Consider data stale after 5 seconds
    retry: 2, // Retry failed requests twice
  });
};
