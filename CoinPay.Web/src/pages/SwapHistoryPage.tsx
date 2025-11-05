import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getSwapHistory } from '@/api/swapApi';
import { SwapHistoryItem } from '@/components/swap/SwapHistoryItem';
import { SwapStatus } from '@/types/swap';

export const SwapHistoryPage: React.FC = () => {
  const [statusFilter, setStatusFilter] = useState<SwapStatus | 'all'>('all');
  const [page, setPage] = useState(1);
  const pageSize = 20;

  const { data, isLoading, error } = useQuery({
    queryKey: ['swapHistory', statusFilter, page],
    queryFn: async () => {
      return await getSwapHistory({
        status: statusFilter,
        page,
        pageSize,
      });
    },
  });

  const filters: Array<{ label: string; value: SwapStatus | 'all' }> = [
    { label: 'All', value: 'all' },
    { label: 'Completed', value: 'confirmed' },
    { label: 'Pending', value: 'pending' },
    { label: 'Failed', value: 'failed' },
  ];

  return (
    <div className="max-w-4xl mx-auto p-6">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">
          Swap History
        </h1>
        <p className="text-gray-600">
          View all your token swap transactions
        </p>
      </div>

      {/* Filters */}
      <div className="flex gap-2 mb-6 overflow-x-auto pb-2">
        {filters.map((filter) => (
          <button
            key={filter.value}
            onClick={() => {
              setStatusFilter(filter.value);
              setPage(1);
            }}
            className={`px-4 py-2 rounded-lg font-medium whitespace-nowrap transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500 ${
              statusFilter === filter.value
                ? 'bg-indigo-600 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            {filter.label}
          </button>
        ))}
      </div>

      {/* Loading State */}
      {isLoading && (
        <div className="flex items-center justify-center py-12">
          <div className="w-8 h-8 border-4 border-gray-300 border-t-indigo-600 rounded-full animate-spin" />
        </div>
      )}

      {/* Error State */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
          <svg
            className="w-12 h-12 text-red-600 mx-auto mb-3"
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
          <p className="text-red-800 font-medium">
            Failed to load swap history
          </p>
          <p className="text-red-600 text-sm mt-1">
            {error instanceof Error ? error.message : 'Unknown error occurred'}
          </p>
        </div>
      )}

      {/* Swap List */}
      {!isLoading && !error && data && (
        <>
          {data.swaps.length === 0 ? (
            <div className="bg-gray-50 border border-gray-200 rounded-lg p-12 text-center">
              <svg
                className="w-16 h-16 text-gray-400 mx-auto mb-4"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"
                />
              </svg>
              <p className="text-gray-600 font-medium text-lg">
                No swaps found
              </p>
              <p className="text-gray-500 text-sm mt-1">
                {statusFilter === 'all'
                  ? 'Start swapping tokens to see your history here'
                  : `No ${statusFilter} swaps found`}
              </p>
            </div>
          ) : (
            <>
              <div className="space-y-3 mb-6">
                {data.swaps.map((swap) => (
                  <SwapHistoryItem key={swap.id} swap={swap} />
                ))}
              </div>

              {/* Pagination */}
              {data.totalPages > 1 && (
                <div className="flex items-center justify-between bg-white border border-gray-200 rounded-lg p-4">
                  <button
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="px-4 py-2 bg-gray-100 hover:bg-gray-200 disabled:bg-gray-50 disabled:text-gray-400 disabled:cursor-not-allowed text-gray-700 font-medium rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  >
                    Previous
                  </button>

                  <span className="text-sm text-gray-600">
                    Page {page} of {data.totalPages}
                  </span>

                  <button
                    onClick={() =>
                      setPage((p) => Math.min(data.totalPages, p + 1))
                    }
                    disabled={page === data.totalPages}
                    className="px-4 py-2 bg-gray-100 hover:bg-gray-200 disabled:bg-gray-50 disabled:text-gray-400 disabled:cursor-not-allowed text-gray-700 font-medium rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  >
                    Next
                  </button>
                </div>
              )}
            </>
          )}
        </>
      )}
    </div>
  );
};
