import React, { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getSwapDetails } from '@/api/swapApi';
import { SwapStatus } from '@/types/swap';

interface SwapStatusTrackerProps {
  swapId: string;
  onComplete?: () => void;
}

export const SwapStatusTracker: React.FC<SwapStatusTrackerProps> = ({
  swapId,
  onComplete,
}) => {
  const { data: swap, isLoading } = useQuery({
    queryKey: ['swapStatus', swapId],
    queryFn: async () => {
      return await getSwapDetails(swapId);
    },
    refetchInterval: (query) => {
      // Stop polling if confirmed or failed
      return query.state.data?.status === 'pending' ? 5000 : false;
    },
    enabled: !!swapId,
  });

  useEffect(() => {
    if (swap && swap.status !== 'pending' && onComplete) {
      onComplete();
    }
  }, [swap, onComplete]);

  if (isLoading || !swap) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="w-8 h-8 border-4 border-gray-300 border-t-indigo-600 rounded-full animate-spin" />
      </div>
    );
  }

  const getStatusConfig = (status: SwapStatus) => {
    switch (status) {
      case 'pending':
        return {
          icon: (
            <div className="w-12 h-12 border-4 border-indigo-200 border-t-indigo-600 rounded-full animate-spin" />
          ),
          title: 'Swap in progress...',
          description: 'Your swap is being processed on the blockchain',
          bgColor: 'bg-blue-50',
          textColor: 'text-blue-900',
          borderColor: 'border-blue-200',
        };
      case 'confirmed':
        return {
          icon: (
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <svg
                className="w-8 h-8 text-green-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M5 13l4 4L19 7"
                />
              </svg>
            </div>
          ),
          title: 'Swap completed!',
          description: `Successfully swapped ${swap.fromAmount.toFixed(4)} ${
            swap.fromTokenSymbol
          } for ${swap.toAmount.toFixed(4)} ${swap.toTokenSymbol}`,
          bgColor: 'bg-green-50',
          textColor: 'text-green-900',
          borderColor: 'border-green-200',
        };
      case 'failed':
        return {
          icon: (
            <div className="w-12 h-12 bg-red-100 rounded-full flex items-center justify-center">
              <svg
                className="w-8 h-8 text-red-600"
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
            </div>
          ),
          title: 'Swap failed',
          description: swap.errorMessage || 'The transaction was reverted',
          bgColor: 'bg-red-50',
          textColor: 'text-red-900',
          borderColor: 'border-red-200',
        };
    }
  };

  const config = getStatusConfig(swap.status);

  return (
    <div
      className={`rounded-lg border p-6 ${config.bgColor} ${config.borderColor}`}
    >
      <div className="flex items-start gap-4">
        <div className="flex-shrink-0">{config.icon}</div>

        <div className="flex-1 min-w-0">
          <h3 className={`text-lg font-semibold mb-1 ${config.textColor}`}>
            {config.title}
          </h3>
          <p className={`text-sm mb-3 ${config.textColor}`}>
            {config.description}
          </p>

          {swap.transactionHash && (
            <a
              href={`https://amoy.polygonscan.com/tx/${swap.transactionHash}`}
              target="_blank"
              rel="noopener noreferrer"
              className={`inline-flex items-center gap-2 text-sm font-medium ${config.textColor} hover:underline`}
            >
              <span>View on Explorer</span>
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
                  d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"
                />
              </svg>
            </a>
          )}

          {swap.status === 'pending' && (
            <div className="mt-3 flex items-center gap-2 text-xs text-blue-700">
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
                  d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <span>Estimated time: ~45 seconds</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
