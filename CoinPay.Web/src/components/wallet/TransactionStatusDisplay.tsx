import { useState, useEffect } from 'react';
import { StatusBadge } from '@/components/StatusBadge';
import { transactionService } from '@/services/transactionService';

interface TransactionStatusDisplayProps {
  transactionId: number;
  onStatusChange?: (status: string) => void;
}

interface TransactionDetail {
  id: number;
  transactionId?: string;
  amount: number;
  currency: string;
  type: string;
  status: string;
  senderName?: string;
  receiverName?: string;
  description?: string;
  createdAt: string;
  confirmedAt?: string;
  blockchainInfo?: {
    chain: string;
    transactionHash?: string;
    blockNumber?: number;
    confirmations?: number;
    gasUsed?: string;
    explorerUrl?: string;
  };
}

export const TransactionStatusDisplay = ({ transactionId, onStatusChange }: TransactionStatusDisplayProps) => {
  const [transaction, setTransaction] = useState<TransactionDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [lastChecked, setLastChecked] = useState<Date | null>(null);

  const fetchStatus = async () => {
    try {
      setError(null);
      const result = await transactionService.getDetails(transactionId);
      setTransaction(result);
      setLastChecked(new Date());

      // Notify parent of status change if callback provided
      if (onStatusChange && result.status) {
        onStatusChange(result.status);
      }

      // Stop polling if transaction is no longer pending
      if (result.status && result.status.toLowerCase() !== 'pending') {
        return true; // Signal to stop polling
      }

      return false; // Continue polling
    } catch (err: any) {
      console.error('Failed to fetch transaction status:', err);
      setError(err.response?.data?.message || 'Failed to load transaction status');
      return false;
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    let intervalId: ReturnType<typeof setInterval> | null = null;
    let shouldPoll = true;

    // Initial fetch
    fetchStatus().then((shouldStop) => {
      if (!shouldStop && shouldPoll) {
        // Set up polling every 5 seconds
        intervalId = setInterval(async () => {
          const shouldStop = await fetchStatus();
          if (shouldStop && intervalId) {
            clearInterval(intervalId);
          }
        }, 5000);
      }
    });

    // Cleanup function
    return () => {
      shouldPoll = false;
      if (intervalId) {
        clearInterval(intervalId);
      }
    };
  }, [transactionId]);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatAmount = (amount: number, currency: string) => {
    return `${amount.toFixed(6)} ${currency}`;
  };

  const getStatusMessage = (status: string) => {
    const statusLower = status.toLowerCase();
    if (statusLower === 'pending') {
      return 'Your transaction is being processed on the blockchain. This usually takes 1-2 minutes.';
    } else if (statusLower === 'confirmed' || statusLower === 'completed') {
      return 'Your transaction has been confirmed on the blockchain.';
    } else if (statusLower === 'failed') {
      return 'Transaction failed. Please try again or contact support if the issue persists.';
    }
    return '';
  };

  if (loading) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="animate-pulse space-y-4">
          <div className="h-6 w-32 bg-gray-200 rounded"></div>
          <div className="h-4 w-full bg-gray-200 rounded"></div>
          <div className="h-4 w-3/4 bg-gray-200 rounded"></div>
          <div className="h-4 w-1/2 bg-gray-200 rounded"></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex items-start gap-3">
          <svg className="w-6 h-6 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div>
            <h3 className="text-sm font-medium text-red-800 mb-1">Error Loading Transaction</h3>
            <p className="text-sm text-red-600">{error}</p>
            <button
              onClick={() => {
                setLoading(true);
                setError(null);
                fetchStatus();
              }}
              className="mt-3 text-sm text-red-600 hover:text-red-700 font-medium"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (!transaction) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="text-center py-8">
          <svg className="w-16 h-16 mx-auto text-gray-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p className="text-gray-500 text-sm">Transaction not found</p>
        </div>
      </div>
    );
  }

  const statusLower = transaction.status.toLowerCase();
  const isConfirmed = statusLower === 'confirmed' || statusLower === 'completed';
  const isPending = statusLower === 'pending';
  const isFailed = statusLower === 'failed';

  return (
    <div className="bg-white rounded-lg shadow-sm p-6">
      {/* Status Header */}
      <div className="mb-6">
        <div className="flex items-center justify-between mb-3">
          <h3 className="text-lg font-semibold text-gray-900">Transaction Status</h3>
          <StatusBadge status={transaction.status as any} />
        </div>

        {/* Status Message */}
        <div className={`p-3 rounded-lg ${
          isConfirmed ? 'bg-green-50 border border-green-200' :
          isPending ? 'bg-yellow-50 border border-yellow-200' :
          isFailed ? 'bg-red-50 border border-red-200' :
          'bg-gray-50 border border-gray-200'
        }`}>
          <p className={`text-sm ${
            isConfirmed ? 'text-green-800' :
            isPending ? 'text-yellow-800' :
            isFailed ? 'text-red-800' :
            'text-gray-800'
          }`}>
            {getStatusMessage(transaction.status)}
          </p>
        </div>
      </div>

      {/* Transaction Details */}
      <div className="space-y-3 mb-6">
        <div className="flex justify-between py-2 border-b border-gray-100">
          <span className="text-sm text-gray-500">Amount</span>
          <span className="text-sm font-medium text-gray-900">{formatAmount(transaction.amount, transaction.currency)}</span>
        </div>

        {transaction.receiverName && (
          <div className="flex justify-between py-2 border-b border-gray-100">
            <span className="text-sm text-gray-500">To</span>
            <span className="text-sm font-medium text-gray-900">{transaction.receiverName}</span>
          </div>
        )}

        <div className="flex justify-between py-2 border-b border-gray-100">
          <span className="text-sm text-gray-500">Type</span>
          <span className="text-sm font-medium text-gray-900">{transaction.type}</span>
        </div>

        <div className="flex justify-between py-2 border-b border-gray-100">
          <span className="text-sm text-gray-500">Created</span>
          <span className="text-sm font-medium text-gray-900">{formatDate(transaction.createdAt)}</span>
        </div>

        {transaction.confirmedAt && (
          <div className="flex justify-between py-2 border-b border-gray-100">
            <span className="text-sm text-gray-500">Confirmed</span>
            <span className="text-sm font-medium text-gray-900">{formatDate(transaction.confirmedAt)}</span>
          </div>
        )}

        {transaction.description && (
          <div className="py-2 border-b border-gray-100">
            <p className="text-sm text-gray-500 mb-1">Note</p>
            <p className="text-sm text-gray-900">{transaction.description}</p>
          </div>
        )}
      </div>

      {/* Blockchain Information */}
      {transaction.blockchainInfo && (
        <div className="mb-6">
          <h4 className="text-sm font-medium text-gray-900 mb-3">Blockchain Information</h4>
          <div className="space-y-2 bg-gray-50 rounded-lg p-3">
            {transaction.blockchainInfo.chain && (
              <div className="flex justify-between">
                <span className="text-xs text-gray-500">Network</span>
                <span className="text-xs font-medium text-gray-900">{transaction.blockchainInfo.chain}</span>
              </div>
            )}

            {transaction.blockchainInfo.transactionHash && (
              <div className="flex justify-between">
                <span className="text-xs text-gray-500">Transaction Hash</span>
                <span className="text-xs font-mono text-gray-900">
                  {transaction.blockchainInfo.transactionHash.slice(0, 10)}...{transaction.blockchainInfo.transactionHash.slice(-8)}
                </span>
              </div>
            )}

            {transaction.blockchainInfo.blockNumber !== undefined && (
              <div className="flex justify-between">
                <span className="text-xs text-gray-500">Block Number</span>
                <span className="text-xs font-medium text-gray-900">{transaction.blockchainInfo.blockNumber}</span>
              </div>
            )}

            {transaction.blockchainInfo.confirmations !== undefined && (
              <div className="flex justify-between">
                <span className="text-xs text-gray-500">Confirmations</span>
                <span className="text-xs font-medium text-gray-900">{transaction.blockchainInfo.confirmations}</span>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Block Explorer Link */}
      {transaction.blockchainInfo?.explorerUrl && (
        <a
          href={transaction.blockchainInfo.explorerUrl}
          target="_blank"
          rel="noopener noreferrer"
          className="inline-flex items-center gap-2 px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 border border-indigo-200 rounded-lg hover:bg-indigo-100 transition-colors"
        >
          View on Block Explorer
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
          </svg>
        </a>
      )}

      {/* Auto-refresh indicator for pending transactions */}
      {isPending && lastChecked && (
        <div className="mt-4 pt-4 border-t border-gray-100">
          <p className="text-xs text-gray-500 flex items-center gap-1">
            <svg className="w-3 h-3 animate-spin" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Auto-refreshing every 5 seconds • Last checked: {lastChecked.toLocaleTimeString()}
          </p>
        </div>
      )}
    </div>
  );
};
