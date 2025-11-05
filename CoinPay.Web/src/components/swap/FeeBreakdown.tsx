import React, { useState } from 'react';
import { SwapQuote } from '@/types/swap';

interface FeeBreakdownProps {
  quote: SwapQuote | null;
}

export const FeeBreakdown: React.FC<FeeBreakdownProps> = ({ quote }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  if (!quote) {
    return null;
  }

  const formatNumber = (value: number, decimals: number = 4) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals,
    }).format(value);
  };

  const totalFee = quote.platformFee + parseFloat(quote.estimatedGasCost);

  return (
    <div className="bg-white rounded-lg border border-gray-200">
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className="w-full flex items-center justify-between p-4 hover:bg-gray-50 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500 rounded-lg"
        aria-expanded={isExpanded}
      >
        <span className="text-sm font-medium text-gray-700">Fees</span>
        <div className="flex items-center gap-3">
          <span className="text-sm font-semibold text-gray-900">
            {formatNumber(totalFee)} {quote.fromTokenSymbol}
          </span>
          <svg
            className={`w-5 h-5 text-gray-400 transition-transform ${
              isExpanded ? 'rotate-180' : ''
            }`}
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M19 9l-7 7-7-7"
            />
          </svg>
        </div>
      </button>

      {isExpanded && (
        <div className="px-4 pb-4 space-y-3 border-t border-gray-200 pt-3">
          <div className="flex items-center justify-between text-sm">
            <span className="text-gray-600">
              Platform Fee ({quote.platformFeePercentage}%)
            </span>
            <span className="font-medium text-gray-900">
              {formatNumber(quote.platformFee)} {quote.fromTokenSymbol}
            </span>
          </div>

          <div className="flex items-center justify-between text-sm">
            <span className="text-gray-600">Network Fee (estimated)</span>
            <span className="font-medium text-gray-900">
              {quote.estimatedGasCost} MATIC
            </span>
          </div>

          <div className="h-px bg-gray-200 my-2" />

          <div className="flex items-center justify-between text-sm">
            <span className="font-semibold text-gray-900">Total Fees</span>
            <span className="font-bold text-gray-900">
              {formatNumber(totalFee)} {quote.fromTokenSymbol}
            </span>
          </div>

          <div className="mt-3 p-3 bg-blue-50 rounded-lg">
            <p className="text-xs text-blue-800">
              Network fees are estimated and may vary based on network
              congestion. The actual fee will be determined at the time of
              transaction execution.
            </p>
          </div>
        </div>
      )}
    </div>
  );
};
