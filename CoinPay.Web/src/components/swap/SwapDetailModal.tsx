import React, { Fragment } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import { useQuery } from '@tanstack/react-query';
import { format } from 'date-fns';
import { getSwapDetails } from '@/api/swapApi';
import { SwapStatus } from '@/types/swap';

interface SwapDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  swapId: string;
}

export const SwapDetailModal: React.FC<SwapDetailModalProps> = ({
  isOpen,
  onClose,
  swapId,
}) => {
  const { data: swap, isLoading } = useQuery({
    queryKey: ['swapDetails', swapId],
    queryFn: async () => await getSwapDetails(swapId),
    enabled: isOpen && !!swapId,
  });

  const formatNumber = (value: number, decimals: number = 6) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: decimals,
    }).format(value);
  };

  const getStatusBadge = (status: SwapStatus) => {
    const config = {
      pending: {
        bg: 'bg-blue-100',
        text: 'text-blue-800',
        label: 'Pending',
      },
      confirmed: {
        bg: 'bg-green-100',
        text: 'text-green-800',
        label: 'Completed',
      },
      failed: {
        bg: 'bg-red-100',
        text: 'text-red-800',
        label: 'Failed',
      },
    };

    const statusConfig = config[status];

    return (
      <span
        className={`inline-flex px-3 py-1 text-xs font-semibold rounded-full ${statusConfig.bg} ${statusConfig.text}`}
      >
        {statusConfig.label}
      </span>
    );
  };

  return (
    <Transition appear show={isOpen} as={Fragment}>
      <Dialog as="div" className="relative z-50" onClose={onClose}>
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
              <Dialog.Panel className="w-full max-w-lg transform overflow-hidden rounded-2xl bg-white p-6 text-left align-middle shadow-xl transition-all">
                <Dialog.Title
                  as="h3"
                  className="text-xl font-bold leading-6 text-gray-900 mb-6 flex items-center justify-between"
                >
                  <span>Swap Details</span>
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
                </Dialog.Title>

                {isLoading ? (
                  <div className="flex items-center justify-center py-12">
                    <div className="w-8 h-8 border-4 border-gray-300 border-t-indigo-600 rounded-full animate-spin" />
                  </div>
                ) : swap ? (
                  <div className="space-y-6">
                    {/* Status */}
                    <div className="flex items-center justify-between">
                      <span className="text-sm font-medium text-gray-600">
                        Status
                      </span>
                      {getStatusBadge(swap.status)}
                    </div>

                    {/* Swap Details */}
                    <div className="bg-gray-50 rounded-lg p-4 space-y-3">
                      <div className="flex items-center justify-between">
                        <span className="text-sm text-gray-600">From</span>
                        <span className="text-base font-semibold text-gray-900">
                          {formatNumber(swap.fromAmount)} {swap.fromTokenSymbol}
                        </span>
                      </div>

                      <div className="flex items-center justify-between">
                        <span className="text-sm text-gray-600">To</span>
                        <span className="text-base font-semibold text-gray-900">
                          {formatNumber(swap.toAmount)} {swap.toTokenSymbol}
                        </span>
                      </div>

                      <div className="h-px bg-gray-200" />

                      <div className="flex items-center justify-between">
                        <span className="text-sm text-gray-600">
                          Exchange Rate
                        </span>
                        <span className="text-sm font-medium text-gray-900">
                          1 {swap.fromTokenSymbol} ={' '}
                          {formatNumber(swap.exchangeRate)}{' '}
                          {swap.toTokenSymbol}
                        </span>
                      </div>
                    </div>

                    {/* Fee Details */}
                    <div>
                      <h4 className="text-sm font-semibold text-gray-900 mb-3">
                        Fee Breakdown
                      </h4>
                      <div className="space-y-2">
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-gray-600">Platform Fee</span>
                          <span className="font-medium text-gray-900">
                            {formatNumber(swap.platformFee, 4)}{' '}
                            {swap.fromTokenSymbol}
                          </span>
                        </div>

                        <div className="flex items-center justify-between text-sm">
                          <span className="text-gray-600">
                            Network Fee{' '}
                            {swap.actualGasUsed ? '(actual)' : '(estimated)'}
                          </span>
                          <span className="font-medium text-gray-900">
                            {swap.actualGasUsed || swap.estimatedGasCost} MATIC
                          </span>
                        </div>

                        <div className="flex items-center justify-between text-sm">
                          <span className="text-gray-600">Price Impact</span>
                          <span className="font-medium text-gray-900">
                            {formatNumber(swap.priceImpact, 2)}%
                          </span>
                        </div>
                      </div>
                    </div>

                    {/* Transaction Hash */}
                    {swap.transactionHash && (
                      <div>
                        <h4 className="text-sm font-semibold text-gray-900 mb-2">
                          Transaction Hash
                        </h4>
                        <a
                          href={`https://amoy.polygonscan.com/tx/${swap.transactionHash}`}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="inline-flex items-center gap-2 text-sm text-indigo-600 hover:text-indigo-700 font-medium"
                        >
                          <span className="truncate max-w-xs">
                            {swap.transactionHash}
                          </span>
                          <svg
                            className="w-4 h-4 flex-shrink-0"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"
                            />
                          </svg>
                        </a>
                      </div>
                    )}

                    {/* Timestamps */}
                    <div className="border-t border-gray-200 pt-4 space-y-2 text-xs text-gray-600">
                      <div className="flex items-center justify-between">
                        <span>Created</span>
                        <span>
                          {format(
                            new Date(swap.createdAt),
                            'MMM dd, yyyy HH:mm:ss'
                          )}
                        </span>
                      </div>
                      {swap.confirmedAt && (
                        <div className="flex items-center justify-between">
                          <span>Confirmed</span>
                          <span>
                            {format(
                              new Date(swap.confirmedAt),
                              'MMM dd, yyyy HH:mm:ss'
                            )}
                          </span>
                        </div>
                      )}
                      {swap.failedAt && (
                        <div className="flex items-center justify-between">
                          <span>Failed</span>
                          <span>
                            {format(
                              new Date(swap.failedAt),
                              'MMM dd, yyyy HH:mm:ss'
                            )}
                          </span>
                        </div>
                      )}
                    </div>

                    {/* Error Message */}
                    {swap.errorMessage && (
                      <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
                        <p className="text-sm text-red-800">
                          {swap.errorMessage}
                        </p>
                      </div>
                    )}

                    {/* Close Button */}
                    <button
                      onClick={onClose}
                      className="w-full py-3 px-4 bg-gray-100 hover:bg-gray-200 text-gray-900 font-semibold rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
                    >
                      Close
                    </button>
                  </div>
                ) : (
                  <div className="text-center py-12">
                    <p className="text-gray-500">Swap details not found</p>
                  </div>
                )}
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
};
