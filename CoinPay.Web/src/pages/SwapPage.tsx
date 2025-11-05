import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useSwapStore } from '@/store/swapStore';
import { SwapInterface } from '@/components/swap/SwapInterface';
import { SwapConfirmationModal } from '@/components/swap/SwapConfirmationModal';
import { SwapStatusTracker } from '@/components/swap/SwapStatusTracker';

export const SwapPage: React.FC = () => {
  const [isConfirmModalOpen, setIsConfirmModalOpen] = useState(false);
  const [activeSwapId, setActiveSwapId] = useState<string | null>(null);
  const [showSuccess, setShowSuccess] = useState(false);

  const {
    fromToken,
    toToken,
    fromAmount,
    toAmount,
    quote,
    isExecuting,
    executeSwapAction,
    reset,
  } = useSwapStore();

  const handleSwapClick = () => {
    if (fromToken && toToken && fromAmount && quote) {
      setIsConfirmModalOpen(true);
    }
  };

  const handleConfirmSwap = async () => {
    try {
      const swapId = await executeSwapAction();
      setIsConfirmModalOpen(false);
      setActiveSwapId(swapId);
      setShowSuccess(false);
    } catch (error) {
      console.error('Swap execution failed:', error);
      // Error is already set in the store
    }
  };

  const handleSwapComplete = () => {
    setShowSuccess(true);
    // Reset form after a delay to allow user to see success message
    setTimeout(() => {
      reset();
      setActiveSwapId(null);
      setShowSuccess(false);
    }, 3000);
  };

  const handleNewSwap = () => {
    reset();
    setActiveSwapId(null);
    setShowSuccess(false);
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Token Swap</h1>
            <p className="text-gray-600 mt-1">
              Swap tokens instantly with the best rates
            </p>
          </div>
          <Link
            to="/swap/history"
            className="flex items-center gap-2 px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
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
                d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
            <span className="font-medium text-gray-700">History</span>
          </Link>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Main Swap Interface */}
          <div className="lg:col-span-2">
            {activeSwapId ? (
              <div className="space-y-4">
                <SwapStatusTracker
                  swapId={activeSwapId}
                  onComplete={handleSwapComplete}
                />

                {showSuccess && (
                  <button
                    onClick={handleNewSwap}
                    className="w-full py-3 px-4 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                  >
                    Make Another Swap
                  </button>
                )}
              </div>
            ) : (
              <SwapInterface onSwapClick={handleSwapClick} />
            )}
          </div>

          {/* Info Panel */}
          <div className="space-y-6">
            {/* How It Works */}
            <div className="bg-white rounded-lg shadow-lg p-6">
              <h3 className="text-lg font-bold text-gray-900 mb-4">
                How It Works
              </h3>
              <div className="space-y-4">
                <div className="flex gap-3">
                  <div className="flex-shrink-0 w-8 h-8 bg-indigo-100 rounded-full flex items-center justify-center">
                    <span className="text-indigo-600 font-bold text-sm">1</span>
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-900 text-sm mb-1">
                      Select Tokens
                    </h4>
                    <p className="text-xs text-gray-600">
                      Choose the tokens you want to swap
                    </p>
                  </div>
                </div>

                <div className="flex gap-3">
                  <div className="flex-shrink-0 w-8 h-8 bg-indigo-100 rounded-full flex items-center justify-center">
                    <span className="text-indigo-600 font-bold text-sm">2</span>
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-900 text-sm mb-1">
                      Enter Amount
                    </h4>
                    <p className="text-xs text-gray-600">
                      Specify how much you want to swap
                    </p>
                  </div>
                </div>

                <div className="flex gap-3">
                  <div className="flex-shrink-0 w-8 h-8 bg-indigo-100 rounded-full flex items-center justify-center">
                    <span className="text-indigo-600 font-bold text-sm">3</span>
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-900 text-sm mb-1">
                      Review & Confirm
                    </h4>
                    <p className="text-xs text-gray-600">
                      Check the details and confirm your swap
                    </p>
                  </div>
                </div>

                <div className="flex gap-3">
                  <div className="flex-shrink-0 w-8 h-8 bg-indigo-100 rounded-full flex items-center justify-center">
                    <span className="text-indigo-600 font-bold text-sm">4</span>
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-900 text-sm mb-1">
                      Done!
                    </h4>
                    <p className="text-xs text-gray-600">
                      Your tokens will be swapped in ~45 seconds
                    </p>
                  </div>
                </div>
              </div>
            </div>

            {/* Important Info */}
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <div className="flex gap-2">
                <svg
                  className="w-5 h-5 text-blue-600 flex-shrink-0"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
                <div className="text-xs text-blue-800">
                  <p className="font-semibold mb-1">Important</p>
                  <ul className="space-y-1 list-disc list-inside">
                    <li>All swaps are executed on Polygon Amoy Testnet</li>
                    <li>Platform fee: 0.5% of swap amount</li>
                    <li>Network fees vary based on congestion</li>
                    <li>Transactions are irreversible once confirmed</li>
                  </ul>
                </div>
              </div>
            </div>

            {/* Supported Tokens */}
            <div className="bg-white rounded-lg shadow-lg p-6">
              <h3 className="text-lg font-bold text-gray-900 mb-4">
                Supported Tokens
              </h3>
              <div className="space-y-2">
                <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50">
                  <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center">
                    <span className="text-sm font-medium text-blue-600">U</span>
                  </div>
                  <div>
                    <div className="text-sm font-medium text-gray-900">USDC</div>
                    <div className="text-xs text-gray-500">USD Coin</div>
                  </div>
                </div>

                <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50">
                  <div className="w-8 h-8 rounded-full bg-purple-100 flex items-center justify-center">
                    <span className="text-sm font-medium text-purple-600">
                      E
                    </span>
                  </div>
                  <div>
                    <div className="text-sm font-medium text-gray-900">WETH</div>
                    <div className="text-xs text-gray-500">Wrapped Ether</div>
                  </div>
                </div>

                <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-50">
                  <div className="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center">
                    <span className="text-sm font-medium text-indigo-600">M</span>
                  </div>
                  <div>
                    <div className="text-sm font-medium text-gray-900">
                      WMATIC
                    </div>
                    <div className="text-xs text-gray-500">Wrapped Matic</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Confirmation Modal */}
      {fromToken && toToken && quote && (
        <SwapConfirmationModal
          isOpen={isConfirmModalOpen}
          onClose={() => !isExecuting && setIsConfirmModalOpen(false)}
          onConfirm={handleConfirmSwap}
          fromToken={fromToken}
          toToken={toToken}
          fromAmount={parseFloat(fromAmount)}
          toAmount={parseFloat(toAmount)}
          quote={quote}
          isExecuting={isExecuting}
        />
      )}
    </div>
  );
};
