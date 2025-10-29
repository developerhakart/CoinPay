import React from 'react';
import { useNavigate } from 'react-router-dom';
import { StatusBadge } from '../StatusBadge';
import type { Transaction } from '@/types/transaction';

interface RecentTransactionsProps {
  transactions: Transaction[];
  isLoading: boolean;
}

export const RecentTransactions: React.FC<RecentTransactionsProps> = ({ transactions, isLoading }) => {
  const navigate = useNavigate();

  const formatAmount = (amount: number, currency: string) => {
    return `${amount.toFixed(2)} ${currency}`;
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diff < 60) return 'Just now';
    if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
    if (diff < 604800) return `${Math.floor(diff / 86400)}d ago`;
    return date.toLocaleDateString();
  };

  if (isLoading) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Recent Transactions</h3>
        <div className="space-y-3">
          {[1, 2, 3].map((i) => (
            <div key={i} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg animate-pulse">
              <div className="flex-1">
                <div className="h-4 w-24 bg-gray-200 rounded mb-2"></div>
                <div className="h-3 w-32 bg-gray-200 rounded"></div>
              </div>
              <div className="h-6 w-16 bg-gray-200 rounded"></div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-sm p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold text-gray-900">Recent Transactions</h3>
        <button
          onClick={() => navigate('/transactions')}
          className="text-sm text-indigo-600 hover:text-indigo-700 font-medium"
        >
          View All
        </button>
      </div>

      {transactions.length === 0 ? (
        <div className="text-center py-8">
          <svg className="w-16 h-16 mx-auto text-gray-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <p className="text-gray-500 text-sm">No transactions yet</p>
          <button
            onClick={() => navigate('/transfer')}
            className="mt-3 text-indigo-600 hover:text-indigo-700 text-sm font-medium"
          >
            Send your first transaction
          </button>
        </div>
      ) : (
        <div className="space-y-3">
          {transactions.slice(0, 5).map((transaction) => (
            <div
              key={transaction.id}
              onClick={() => navigate('/transactions')}
              className="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 cursor-pointer transition-colors"
            >
              <div className="flex items-center gap-3 flex-1">
                <div className={`w-10 h-10 rounded-full flex items-center justify-center ${
                  transaction.type === 'Payment' ? 'bg-blue-100' :
                  transaction.type === 'Transfer' ? 'bg-green-100' : 'bg-purple-100'
                }`}>
                  <svg className={`w-5 h-5 ${
                    transaction.type === 'Payment' ? 'text-blue-600' :
                    transaction.type === 'Transfer' ? 'text-green-600' : 'text-purple-600'
                  }`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    {transaction.type === 'Payment' ? (
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                    ) : (
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                    )}
                  </svg>
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <p className="text-sm font-medium text-gray-900 truncate">
                      {transaction.type} {transaction.receiverName ? `to ${transaction.receiverName}` : ''}
                    </p>
                    <StatusBadge status={transaction.status as any} />
                  </div>
                  <p className="text-xs text-gray-500">{formatDate(transaction.createdAt)}</p>
                </div>
              </div>
              <div className="text-right ml-4">
                <p className="text-sm font-semibold text-gray-900">
                  {formatAmount(transaction.amount, transaction.currency)}
                </p>
                {transaction.transactionId && (
                  <p className="text-xs text-gray-500 mt-1">{transaction.transactionId}</p>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
