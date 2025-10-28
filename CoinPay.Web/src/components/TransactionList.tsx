import { useState, useEffect } from 'react';
import { transactionApi } from '../services/api';
import type { Transaction } from '../types/transaction';
import { StatusBadge } from './StatusBadge';

export function TransactionList() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<string>('all');

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      setError(null);
      let data: Transaction[];

      if (filter === 'all') {
        data = await transactionApi.getAll();
      } else {
        data = await transactionApi.getByStatus(filter);
      }

      setTransactions(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, [filter]);

  const formatCurrency = (amount: number, currency: string) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency,
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getTypeIcon = (type: string) => {
    switch (type) {
      case 'Payment':
        return '💰';
      case 'Transfer':
        return '🔄';
      case 'Refund':
        return '↩️';
      default:
        return '📝';
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-800">
        <p className="font-semibold">Error loading transactions</p>
        <p className="text-sm">{error}</p>
        <button
          onClick={fetchTransactions}
          className="mt-2 text-sm bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Filter Buttons */}
      <div className="flex gap-2 flex-wrap">
        <button
          onClick={() => setFilter('all')}
          className={`px-4 py-2 rounded-lg font-medium transition ${
            filter === 'all'
              ? 'bg-blue-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          All
        </button>
        <button
          onClick={() => setFilter('Pending')}
          className={`px-4 py-2 rounded-lg font-medium transition ${
            filter === 'Pending'
              ? 'bg-yellow-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          Pending
        </button>
        <button
          onClick={() => setFilter('Completed')}
          className={`px-4 py-2 rounded-lg font-medium transition ${
            filter === 'Completed'
              ? 'bg-green-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          Completed
        </button>
        <button
          onClick={() => setFilter('Failed')}
          className={`px-4 py-2 rounded-lg font-medium transition ${
            filter === 'Failed'
              ? 'bg-red-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          }`}
        >
          Failed
        </button>
      </div>

      {/* Transaction Cards */}
      {transactions.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          <p className="text-lg">No transactions found</p>
          <p className="text-sm">Try selecting a different filter</p>
        </div>
      ) : (
        <div className="space-y-3">
          {transactions.map((transaction) => (
            <div
              key={transaction.id}
              className="bg-white rounded-lg shadow-md hover:shadow-lg transition p-4 border border-gray-200"
            >
              <div className="flex justify-between items-start mb-3">
                <div className="flex items-center gap-2">
                  <span className="text-2xl">{getTypeIcon(transaction.type)}</span>
                  <div>
                    <p className="font-semibold text-gray-800">
                      {transaction.transactionId || `TX-${transaction.id}`}
                    </p>
                    <p className="text-xs text-gray-500">{transaction.type}</p>
                  </div>
                </div>
                <StatusBadge status={transaction.status as any} />
              </div>

              <div className="grid grid-cols-2 gap-4 mb-3">
                <div>
                  <p className="text-xs text-gray-500">From</p>
                  <p className="text-sm font-medium text-gray-700">
                    {transaction.senderName || 'N/A'}
                  </p>
                </div>
                <div>
                  <p className="text-xs text-gray-500">To</p>
                  <p className="text-sm font-medium text-gray-700">
                    {transaction.receiverName || 'N/A'}
                  </p>
                </div>
              </div>

              <div className="mb-3">
                <p className="text-xs text-gray-500">Description</p>
                <p className="text-sm text-gray-700">{transaction.description}</p>
              </div>

              <div className="flex justify-between items-center pt-3 border-t border-gray-200">
                <div>
                  <p className="text-2xl font-bold text-blue-600">
                    {formatCurrency(transaction.amount, transaction.currency)}
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-xs text-gray-500">Created</p>
                  <p className="text-xs text-gray-700">
                    {formatDate(transaction.createdAt)}
                  </p>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
