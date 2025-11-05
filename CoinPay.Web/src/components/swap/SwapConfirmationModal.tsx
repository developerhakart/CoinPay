import React, { Fragment } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import { Token } from '@/constants/tokens';
import { SwapQuote } from '@/types/swap';

interface SwapConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  fromToken: Token;
  toToken: Token;
  fromAmount: number;
  toAmount: number;
  quote: SwapQuote;
  isExecuting: boolean;
}

export const SwapConfirmationModal: React.FC<SwapConfirmationModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  fromToken,
  toToken,
  fromAmount,
  toAmount,
  quote,
  isExecuting,
}) => {
  const formatNumber = (value: number, decimals: number = 6) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: decimals,
    }).format(value);
  };

  return (
    <Transition appear show={isOpen} as={Fragment}>
      <Dialog
        as="div"
        className="relative z-50"
        onClose={isExecuting ? () => {} : onClose}
      >
        <Transition.Child
          as={Fragment}
          enter="ease-out duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-200"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-black bg-opacity-25" />
        </Transition.Child>

        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center p-4 text-center">
            <Transition.Child
              as={Fragment}
              enter="ease-out duration-300"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-200"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 text-left align-middle shadow-xl transition-all">
                <Dialog.Title
                  as="h3"
                  className="text-xl font-bold leading-6 text-gray-900 mb-6 flex items-center justify-between"
                >
                  <span>Confirm Swap</span>
                  {!isExecuting && (
                    <button
                      onClick={onClose}
                      className="text-gray-400 hover:text-gray-600 focus:outline-none"
                      aria-label="Close modal"
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
                          d="M6 18L18 6M6 6l12 12"
                        />
                      </svg>
                    </button>
                  )}
                </Dialog.Title>

                {/* Swap Summary */}
                <div className="mb-6 space-y-4">
                  <div className="bg-gray-50 rounded-lg p-4">
                    <div className="text-sm text-gray-600 mb-1">You pay</div>
                    <div className="flex items-center justify-between">
                      <span className="text-2xl font-bold text-gray-900">
                        {formatNumber(fromAmount)}
                      </span>
                      <span className="text-lg font-semibold text-gray-700">
                        {fromToken.symbol}
                      </span>
                    </div>
                  </div>

                  <div className="flex justify-center">
                    <svg
                      className="w-6 h-6 text-gray-400"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M19 14l-7 7m0 0l-7-7m7 7V3"
                      />
                    </svg>
                  </div>

                  <div className="bg-gray-50 rounded-lg p-4">
                    <div className="text-sm text-gray-600 mb-1">
                      You receive (estimated)
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-2xl font-bold text-gray-900">
                        {formatNumber(toAmount)}
                      </span>
                      <span className="text-lg font-semibold text-gray-700">
                        {toToken.symbol}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Details */}
                <div className="mb-6 space-y-3 border-t border-gray-200 pt-4">
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-600">Exchange Rate</span>
                    <span className="font-medium text-gray-900">
                      1 {fromToken.symbol} = {formatNumber(quote.exchangeRate)}{' '}
                      {toToken.symbol}
                    </span>
                  </div>

                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-600">Platform Fee</span>
                    <span className="font-medium text-gray-900">
                      {formatNumber(quote.platformFee, 4)} {fromToken.symbol} (
                      {quote.platformFeePercentage}%)
                    </span>
                  </div>

                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-600">Slippage Tolerance</span>
                    <span className="font-medium text-gray-900">
                      {quote.slippageTolerance}%
                    </span>
                  </div>

                  <div className="flex items-center justify-between text-sm bg-blue-50 p-3 rounded-lg">
                    <span className="text-blue-900 font-medium">
                      Minimum Received
                    </span>
                    <span className="font-bold text-blue-900">
                      {formatNumber(quote.minimumReceived)} {toToken.symbol}
                    </span>
                  </div>
                </div>

                {/* Notice */}
                <div className="mb-6 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
                  <div className="flex items-start gap-2">
                    <svg
                      className="w-5 h-5 text-yellow-600 flex-shrink-0 mt-0.5"
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
                    <p className="text-xs text-yellow-800">
                      Output is estimated. You will receive at least{' '}
                      <strong>{formatNumber(quote.minimumReceived)}</strong>{' '}
                      {toToken.symbol} or the transaction will revert.
                    </p>
                  </div>
                </div>

                {/* Actions */}
                <div className="flex gap-3">
                  <button
                    onClick={onClose}
                    disabled={isExecuting}
                    className="flex-1 px-4 py-3 border border-gray-300 rounded-lg font-semibold text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
                  >
                    Cancel
                  </button>
                  <button
                    onClick={onConfirm}
                    disabled={isExecuting}
                    className="flex-1 px-4 py-3 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 text-white font-semibold rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 flex items-center justify-center gap-2"
                  >
                    {isExecuting ? (
                      <>
                        <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                        <span>Swapping...</span>
                      </>
                    ) : (
                      'Confirm Swap'
                    )}
                  </button>
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
};
