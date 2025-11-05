import React, { useState } from 'react';
import { formatDistanceToNow } from 'date-fns';
import { SwapDetails, SwapStatus } from '@/types/swap';
import { SwapDetailModal } from './SwapDetailModal';

interface SwapHistoryItemProps {
  swap: SwapDetails;
}

export const SwapHistoryItem: React.FC<SwapHistoryItemProps> = ({ swap }) => {
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  const formatNumber = (value: number, decimals: number = 4) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: decimals,
    }).format(value);
  };

  const getStatusConfig = (status: SwapStatus) => {
    switch (status) {
      case 'pending':
        return {
          bg: 'bg-blue-100',
          text: 'text-blue-800',
          label: 'Pending',
          icon: (
            <div className="w-2 h-2 bg-blue-500 rounded-full animate-pulse" />
          ),
        };
      case 'confirmed':
        return {
          bg: 'bg-green-100',
          text: 'text-green-800',
          label: 'Completed',
          icon: <div className="w-2 h-2 bg-green-500 rounded-full" />,
        };
      case 'failed':
        return {
          bg: 'bg-red-100',
          text: 'text-red-800',
          label: 'Failed',
          icon: <div className="w-2 h-2 bg-red-500 rounded-full" />,
        };
    }
  };

  const statusConfig = getStatusConfig(swap.status);

  return (
    <>
      <button
        onClick={() => setIsDetailOpen(true)}
        className="w-full bg-white hover:bg-gray-50 rounded-lg border border-gray-200 p-4 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500 text-left"
      >
        <div className="flex items-center justify-between mb-3">
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-2 text-base font-semibold text-gray-900">
              <span>{formatNumber(swap.fromAmount)}</span>
              <span className="text-gray-600">{swap.fromTokenSymbol}</span>
            </div>
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
                d="M14 5l7 7m0 0l-7 7m7-7H3"
              />
            </svg>
            <div className="flex items-center gap-2 text-base font-semibold text-gray-900">
              <span>{formatNumber(swap.toAmount)}</span>
              <span className="text-gray-600">{swap.toTokenSymbol}</span>
            </div>
          </div>

          <div
            className={`flex items-center gap-2 px-3 py-1 rounded-full ${statusConfig.bg}`}
          >
            {statusConfig.icon}
            <span className={`text-xs font-semibold ${statusConfig.text}`}>
              {statusConfig.label}
            </span>
          </div>
        </div>

        <div className="flex items-center justify-between text-sm text-gray-500">
          <span>
            {formatDistanceToNow(new Date(swap.createdAt), {
              addSuffix: true,
            })}
          </span>
          {swap.transactionHash && (
            <span className="truncate max-w-[200px]">
              {swap.transactionHash.slice(0, 10)}...
              {swap.transactionHash.slice(-8)}
            </span>
          )}
        </div>
      </button>

      <SwapDetailModal
        isOpen={isDetailOpen}
        onClose={() => setIsDetailOpen(false)}
        swapId={swap.id}
      />
    </>
  );
};
