import React, { useState } from 'react';

interface BalanceCardProps {
  balance: number;
  currency?: string;
  isLoading: boolean;
  onRefresh: () => void;
  lastUpdated?: Date;
}

export const BalanceCard: React.FC<BalanceCardProps> = ({
  balance,
  currency = 'USDC',
  isLoading,
  onRefresh,
  lastUpdated
}) => {
  const [isRefreshing, setIsRefreshing] = useState(false);

  const handleRefresh = async () => {
    setIsRefreshing(true);
    await onRefresh();
    setTimeout(() => setIsRefreshing(false), 500);
  };

  const formatBalance = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 6
    }).format(value);
  };

  const formatLastUpdated = (date?: Date) => {
    if (!date) return '';
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diff < 60) return 'Just now';
    if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
    return date.toLocaleDateString();
  };

  return (
    <div className="bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg shadow-lg p-6 mb-6 text-white">
      <div className="flex items-start justify-between mb-4">
        <div>
          <p className="text-indigo-100 text-sm font-medium mb-1">Total Balance</p>
          {isLoading ? (
            <div className="h-10 w-48 bg-white/20 rounded animate-pulse"></div>
          ) : (
            <h3 className="text-4xl font-bold">
              {formatBalance(balance)} <span className="text-2xl font-normal">{currency}</span>
            </h3>
          )}
          {lastUpdated && (
            <p className="text-indigo-100 text-xs mt-1">
              Updated {formatLastUpdated(lastUpdated)}
            </p>
          )}
        </div>
        <button
          onClick={handleRefresh}
          disabled={isRefreshing || isLoading}
          className="p-2 bg-white/10 hover:bg-white/20 rounded-full transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          title="Refresh balance"
        >
          <svg
            className={`w-5 h-5 ${isRefreshing ? 'animate-spin' : ''}`}
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            />
          </svg>
        </button>
      </div>

      <div className="flex items-center gap-2 text-sm">
        <div className="flex items-center gap-1">
          <div className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
          <span className="text-indigo-100">Polygon Amoy</span>
        </div>
        <span className="text-indigo-200">•</span>
        <span className="text-indigo-100">Gasless Transactions</span>
      </div>
    </div>
  );
};
