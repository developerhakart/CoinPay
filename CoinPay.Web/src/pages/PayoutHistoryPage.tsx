import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { payoutApi, type Payout } from '../api/payoutApi';

export const PayoutHistoryPage: React.FC = () => {
  const navigate = useNavigate();
  const [payouts, setPayouts] = useState<Payout[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<string>('all');

  useEffect(() => {
    fetchPayouts();
  }, []);

  const fetchPayouts = async () => {
    setIsLoading(true);
    try {
      const response = await payoutApi.getPayoutHistory(50, 0);
      setPayouts(response.payouts);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load payout history');
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'failed':
        return 'bg-red-100 text-red-800';
      case 'processing':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-yellow-100 text-yellow-800';
    }
  };

  const filteredPayouts = payouts.filter(payout => {
    if (filter === 'all') return true;
    return payout.status === filter;
  });

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Payout History</h1>
          <p className="mt-2 text-sm text-gray-600">
            View and track all your crypto-to-fiat withdrawals
          </p>
        </div>

        {/* Filters */}
        <div className="bg-white rounded-lg shadow-sm p-4 mb-6">
          <div className="flex items-center space-x-2">
            <span className="text-sm font-medium text-gray-700">Filter:</span>
            {['all', 'pending', 'processing', 'completed', 'failed'].map((status) => (
              <button
                key={status}
                onClick={() => setFilter(status)}
                className={`px-3 py-1 rounded-md text-sm font-medium transition-colors ${
                  filter === status
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {status.charAt(0).toUpperCase() + status.slice(1)}
              </button>
            ))}
          </div>
        </div>

        {/* Error State */}
        {error && (
          <div className="bg-red-50 border border-red-200 rounded-md p-4 mb-6">
            <div className="flex items-center">
              <svg className="h-5 w-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
              </svg>
              <p className="text-sm text-red-800">{error}</p>
            </div>
          </div>
        )}

        {/* Empty State */}
        {filteredPayouts.length === 0 && !error && (
          <div className="bg-white rounded-lg shadow-sm p-12 text-center">
            <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <h3 className="mt-2 text-sm font-medium text-gray-900">No payouts found</h3>
            <p className="mt-1 text-sm text-gray-500">
              {filter === 'all'
                ? 'You haven\'t initiated any withdrawals yet.'
                : `No ${filter} payouts found.`}
            </p>
          </div>
        )}

        {/* Payouts List */}
        {filteredPayouts.length > 0 && (
          <div className="bg-white rounded-lg shadow-sm overflow-hidden">
            <ul className="divide-y divide-gray-200">
              {filteredPayouts.map((payout) => (
                <li
                  key={payout.id}
                  onClick={() => navigate(`/payout/${payout.id}/status`)}
                  className="hover:bg-gray-50 cursor-pointer transition-colors"
                >
                  <div className="px-6 py-4">
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <div className="flex items-center space-x-3">
                          <div className="flex-shrink-0">
                            <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                              <svg className="w-5 h-5 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                              </svg>
                            </div>
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="flex items-center space-x-2">
                              <p className="text-sm font-medium text-gray-900 truncate">
                                {payout.bankAccount?.accountHolderName || 'Bank Account'}
                              </p>
                              <span className={`inline-flex px-2 py-0.5 text-xs font-medium rounded-full ${getStatusColor(payout.status)}`}>
                                {payout.status}
                              </span>
                            </div>
                            <p className="text-sm text-gray-500">
                              {payout.bankAccount?.bankName && `${payout.bankAccount.bankName} • `}
                              {payout.bankAccount?.accountType} •••• {payout.bankAccount?.lastFourDigits}
                            </p>
                            <p className="text-xs text-gray-400 mt-1">
                              {new Date(payout.initiatedAt).toLocaleDateString()} at {new Date(payout.initiatedAt).toLocaleTimeString()}
                            </p>
                          </div>
                        </div>
                      </div>
                      <div className="ml-4 flex-shrink-0 text-right">
                        <p className="text-sm font-medium text-gray-900">
                          {payout.usdcAmount.toFixed(2)} USDC
                        </p>
                        <p className="text-sm text-gray-500">
                          ≈ ${payout.netAmount.toFixed(2)}
                        </p>
                        {payout.status === 'pending' || payout.status === 'processing' ? (
                          <p className="text-xs text-blue-600 mt-1">
                            Est. {payout.estimatedArrival && new Date(payout.estimatedArrival).toLocaleDateString()}
                          </p>
                        ) : payout.status === 'completed' && payout.completedAt ? (
                          <p className="text-xs text-green-600 mt-1">
                            Completed
                          </p>
                        ) : payout.status === 'failed' ? (
                          <p className="text-xs text-red-600 mt-1">
                            Failed
                          </p>
                        ) : null}
                      </div>
                    </div>

                    {/* Fees Breakdown (collapsed by default, could expand on click) */}
                    <div className="mt-3 pt-3 border-t border-gray-100">
                      <div className="flex justify-between text-xs text-gray-500">
                        <span>Exchange Rate: 1 USDC = {payout.exchangeRate.toFixed(4)} USD</span>
                        <span>Total Fees: ${payout.totalFees.toFixed(2)}</span>
                      </div>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Summary Stats */}
        {payouts.length > 0 && (
          <div className="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-4">
            <div className="bg-white rounded-lg shadow-sm p-4">
              <p className="text-sm text-gray-600">Total Payouts</p>
              <p className="text-2xl font-bold text-gray-900">{payouts.length}</p>
            </div>
            <div className="bg-white rounded-lg shadow-sm p-4">
              <p className="text-sm text-gray-600">Completed</p>
              <p className="text-2xl font-bold text-green-600">
                {payouts.filter(p => p.status === 'completed').length}
              </p>
            </div>
            <div className="bg-white rounded-lg shadow-sm p-4">
              <p className="text-sm text-gray-600">Pending</p>
              <p className="text-2xl font-bold text-yellow-600">
                {payouts.filter(p => p.status === 'pending' || p.status === 'processing').length}
              </p>
            </div>
            <div className="bg-white rounded-lg shadow-sm p-4">
              <p className="text-sm text-gray-600">Total Withdrawn</p>
              <p className="text-2xl font-bold text-blue-600">
                {payouts.filter(p => p.status === 'completed').reduce((sum, p) => sum + p.usdcAmount, 0).toFixed(2)} USDC
              </p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
