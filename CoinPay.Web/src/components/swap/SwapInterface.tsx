import React, { useState, useEffect } from 'react';
import { useSwapStore } from '@/store/swapStore';
import { useTokenBalances } from '@/hooks/useTokenBalances';
import { useSwapCalculation } from '@/hooks/useSwapCalculation';
import { TokenSelectionModal } from './TokenSelectionModal';
import { ExchangeRateDisplay } from './ExchangeRateDisplay';
import { SlippageSettings } from './SlippageSettings';
import { PriceImpactIndicator } from './PriceImpactIndicator';
import { FeeBreakdown } from './FeeBreakdown';
import { Token } from '@/constants/tokens';

interface SwapInterfaceProps {
  onSwapClick: () => void;
}

export const SwapInterface: React.FC<SwapInterfaceProps> = ({
  onSwapClick,
}) => {
  const {
    fromToken,
    toToken,
    fromAmount,
    slippageTolerance,
    setFromToken,
    setToToken,
    setFromAmount,
    setToAmount,
    setSlippageTolerance,
    setQuote,
    flipTokens,
    error: swapError,
  } = useSwapStore();

  const [isFromModalOpen, setIsFromModalOpen] = useState(false);
  const [isToModalOpen, setIsToModalOpen] = useState(false);
  const [showSettings, setShowSettings] = useState(false);

  const { data: balances, isLoading: balancesLoading } = useTokenBalances();

  const { toAmount, quote, isCalculating, error: calculationError } = useSwapCalculation({
    fromToken,
    toToken,
    fromAmount,
    slippage: slippageTolerance,
  });

  // Update toAmount and quote in store
  useEffect(() => {
    setToAmount(toAmount);
    setQuote(quote || null);
  }, [toAmount, quote, setToAmount, setQuote]);

  const fromBalance = fromToken
    ? balances?.get(fromToken.address.toLowerCase()) || 0
    : 0;

  const toBalance = toToken
    ? balances?.get(toToken.address.toLowerCase()) || 0
    : 0;

  const handleMaxAmount = () => {
    if (fromToken && fromBalance > 0) {
      setFromAmount(fromBalance.toString());
    }
  };

  const formatBalance = (balance: number) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 6,
    }).format(balance);
  };

  const isInsufficientBalance =
    fromAmount && parseFloat(fromAmount) > fromBalance;

  const canSwap =
    fromToken &&
    toToken &&
    fromAmount &&
    parseFloat(fromAmount) > 0 &&
    !isInsufficientBalance &&
    !isCalculating &&
    quote;

  return (
    <div className="bg-white rounded-lg shadow-lg p-6 max-w-lg mx-auto">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-gray-900">Swap</h2>
        <button
          onClick={() => setShowSettings(!showSettings)}
          className="p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
          aria-label="Settings"
        >
          <svg
            className="w-6 h-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
            />
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
            />
          </svg>
        </button>
      </div>

      {showSettings && (
        <div className="mb-4">
          <SlippageSettings
            value={slippageTolerance}
            onChange={setSlippageTolerance}
          />
        </div>
      )}

      {/* From Token Section */}
      <div className="bg-gray-50 rounded-lg p-4 mb-2">
        <div className="flex items-center justify-between mb-2">
          <span className="text-sm font-medium text-gray-700">From</span>
          <span className="text-sm text-gray-500">
            Balance:{' '}
            {balancesLoading ? (
              <span className="inline-block w-16 h-4 bg-gray-200 animate-pulse rounded" />
            ) : (
              formatBalance(fromBalance)
            )}
          </span>
        </div>

        <div className="flex items-center gap-3">
          <input
            type="number"
            className="flex-1 text-2xl font-semibold bg-transparent border-none outline-none focus:ring-0 p-0"
            placeholder="0.0"
            value={fromAmount}
            onChange={(e) => setFromAmount(e.target.value)}
            min="0"
            step="any"
          />

          <button
            onClick={() => setIsFromModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2 bg-white rounded-lg border border-gray-300 hover:border-gray-400 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            {fromToken ? (
              <>
                <div className="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center">
                  <span className="text-xs font-medium text-gray-600">
                    {fromToken.symbol.charAt(0)}
                  </span>
                </div>
                <span className="font-medium">{fromToken.symbol}</span>
              </>
            ) : (
              <span className="font-medium">Select token</span>
            )}
            <svg
              className="w-5 h-5 text-gray-400"
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
          </button>
        </div>

        {fromToken && (
          <div className="mt-2 flex justify-end">
            <button
              onClick={handleMaxAmount}
              disabled={!fromToken || fromBalance === 0}
              className="text-xs font-semibold text-indigo-600 hover:text-indigo-700 disabled:text-gray-400 disabled:cursor-not-allowed"
            >
              MAX
            </button>
          </div>
        )}
      </div>

      {/* Flip Button */}
      <div className="flex justify-center -my-3 relative z-10">
        <button
          onClick={flipTokens}
          disabled={!fromToken || !toToken}
          className="p-2 bg-white border-2 border-gray-200 rounded-lg hover:border-indigo-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-indigo-500"
          aria-label="Flip tokens"
        >
          <svg
            className="w-5 h-5 text-gray-600"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4"
            />
          </svg>
        </button>
      </div>

      {/* To Token Section */}
      <div className="bg-gray-50 rounded-lg p-4 mb-4">
        <div className="flex items-center justify-between mb-2">
          <span className="text-sm font-medium text-gray-700">To</span>
          <span className="text-sm text-gray-500">
            Balance:{' '}
            {balancesLoading ? (
              <span className="inline-block w-16 h-4 bg-gray-200 animate-pulse rounded" />
            ) : (
              formatBalance(toBalance)
            )}
          </span>
        </div>

        <div className="flex items-center gap-3">
          <div className="flex-1 relative">
            <input
              type="number"
              className="w-full text-2xl font-semibold bg-transparent border-none outline-none focus:ring-0 p-0"
              placeholder="0.0"
              value={toAmount}
              readOnly
            />
            {isCalculating && (
              <div className="absolute right-0 top-1/2 -translate-y-1/2">
                <div className="w-5 h-5 border-2 border-gray-300 border-t-indigo-600 rounded-full animate-spin" />
              </div>
            )}
          </div>

          <button
            onClick={() => setIsToModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2 bg-white rounded-lg border border-gray-300 hover:border-gray-400 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            {toToken ? (
              <>
                <div className="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center">
                  <span className="text-xs font-medium text-gray-600">
                    {toToken.symbol.charAt(0)}
                  </span>
                </div>
                <span className="font-medium">{toToken.symbol}</span>
              </>
            ) : (
              <span className="font-medium">Select token</span>
            )}
            <svg
              className="w-5 h-5 text-gray-400"
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
          </button>
        </div>
      </div>

      {/* Exchange Rate */}
      {fromToken && toToken && fromAmount && parseFloat(fromAmount) > 0 && (
        <div className="mb-4">
          <ExchangeRateDisplay
            fromToken={fromToken}
            toToken={toToken}
            quote={quote || null}
            isLoading={isCalculating}
            error={calculationError}
          />
        </div>
      )}

      {/* Price Impact */}
      {quote && quote.priceImpact > 0 && (
        <div className="mb-4">
          <PriceImpactIndicator priceImpact={quote.priceImpact} />
        </div>
      )}

      {/* Fee Breakdown */}
      {quote && (
        <div className="mb-4">
          <FeeBreakdown quote={quote} />
        </div>
      )}

      {/* Error Messages */}
      {(swapError || isInsufficientBalance) && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-sm text-red-800">
            {isInsufficientBalance
              ? 'Insufficient balance'
              : swapError}
          </p>
        </div>
      )}

      {/* Swap Button */}
      <button
        onClick={onSwapClick}
        disabled={!canSwap}
        className="w-full py-4 px-6 bg-indigo-600 hover:bg-indigo-700 disabled:bg-gray-300 disabled:cursor-not-allowed text-white font-semibold rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
      >
        {!fromToken || !toToken
          ? 'Select tokens'
          : !fromAmount || parseFloat(fromAmount) === 0
          ? 'Enter amount'
          : isInsufficientBalance
          ? 'Insufficient balance'
          : isCalculating
          ? 'Calculating...'
          : 'Swap'}
      </button>

      {/* Token Selection Modals */}
      <TokenSelectionModal
        isOpen={isFromModalOpen}
        onClose={() => setIsFromModalOpen(false)}
        onSelectToken={(token: Token) => {
          setFromToken(token);
          setIsFromModalOpen(false);
        }}
        excludeToken={toToken?.address}
      />

      <TokenSelectionModal
        isOpen={isToModalOpen}
        onClose={() => setIsToModalOpen(false)}
        onSelectToken={(token: Token) => {
          setToToken(token);
          setIsToModalOpen(false);
        }}
        excludeToken={fromToken?.address}
      />
    </div>
  );
};
