import { useState, useEffect } from 'react';
import { useDebounce } from './useDebounce';
import { useExchangeRate } from './useExchangeRate';
import { Token } from '@/constants/tokens';

interface UseSwapCalculationOptions {
  fromToken: Token | null;
  toToken: Token | null;
  fromAmount: string;
  slippage: number;
}

/**
 * Hook to calculate swap amounts with debounced API calls
 */
export const useSwapCalculation = ({
  fromToken,
  toToken,
  fromAmount,
  slippage,
}: UseSwapCalculationOptions) => {
  const [toAmount, setToAmount] = useState<string>('');
  const debouncedFromAmount = useDebounce(fromAmount, 500);

  const amount = parseFloat(debouncedFromAmount) || 0;

  const { data: quote, isLoading, error } = useExchangeRate({
    fromToken: fromToken?.address || null,
    toToken: toToken?.address || null,
    amount,
    slippage,
  });

  useEffect(() => {
    if (quote && quote.toAmount) {
      setToAmount(quote.toAmount.toFixed(6));
    } else {
      setToAmount('');
    }
  }, [quote]);

  return {
    toAmount,
    quote,
    isCalculating: isLoading,
    error,
  };
};
