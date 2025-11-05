import React from 'react';
import { formatDistanceToNow } from 'date-fns';
import { Token } from '@/constants/tokens';
import { SwapQuote } from '@/types/swap';

interface ExchangeRateDisplayProps {
  fromToken: Token | null;
  toToken: Token | null;
  quote: SwapQuote | null;
  isLoading: boolean;
  error: Error | null;
}

export const ExchangeRateDisplay: React.FC<ExchangeRateDisplayProps> = ({
  fromToken,
  toToken,
  quote,
  isLoading,
  error,
}) => {
  if (!fromToken || !toToken) {
    return null;
  }

  if (isLoading) {
    return (
      <div className="flex items-center gap-2 text-sm text-gray-500">
        <div className="w-4 h-4 border-2 border-gray-300 border-t-indigo-600 rounded-full animate-spin" />
        <span>Fetching best price...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center gap-2 text-sm text-red-600">
        <svg
          className="w-4 h-4"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <span>Failed to fetch price</span>
      </div>
    );
  }

  if (!quote) {
    return null;
  }

  const formatRate = (rate: number) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 6,
    }).format(rate);
  };

  return (
    <div className="bg-gray-50 rounded-lg p-3 space-y-2">
      <div className="flex items-center justify-between">
        <span className="text-sm text-gray-600">Exchange Rate</span>
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium text-gray-900">
            1 {fromToken.symbol} = {formatRate(quote.exchangeRate)}{' '}
            {toToken.symbol}
          </span>
        </div>
      </div>

      <div className="flex items-center justify-between text-xs text-gray-500">
        <span className="flex items-center gap-1">
          <span className="w-2 h-2 bg-green-500 rounded-full" />
          via {quote.provider}
        </span>
        {quote.quoteValidUntil && (
          <span>
            Updated{' '}
            {formatDistanceToNow(new Date(quote.quoteValidUntil), {
              addSuffix: true,
            })}
          </span>
        )}
      </div>
    </div>
  );
};
