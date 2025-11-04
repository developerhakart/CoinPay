import React, { useState, useEffect } from 'react';
import { FeeBreakdown } from '../Fees/FeeBreakdown';

interface PayoutDetails {
  id: string;
  bankAccountId: string;
  gatewayTransactionId?: string;
  usdcAmount: number;
  usdAmount: number;
  exchangeRate: number;
  conversionFee: number;
  payoutFee: number;
  totalFees: number;
  netAmount: number;
  status: string;
  initiatedAt: string;
  completedAt?: string;
  estimatedArrival?: string;
  failureReason?: string;
  bankAccount?: {
    id: string;
    accountHolderName: string;
    lastFourDigits: string;
    accountType: string;
    bankName?: string;
  };
}

interface PayoutDetailModalProps {
  payoutId: string;
  isOpen: boolean;
  onClose: () => void;
  onCancel?: (payoutId: string) => void;
}

/**
 * Payout Detail Modal
 * Shows comprehensive payout information with fee breakdown
 * Allows cancellation of pending payouts
 */
export const PayoutDetailModal: React.FC<PayoutDetailModalProps> = ({
  payoutId,
  isOpen,
  onClose,
  onCancel,
}) => {
  const [payout, setPayout] = useState<PayoutDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isCancelling, setIsCancelling] = useState(false);

  useEffect(() => {
    if (isOpen && payoutId) {
      fetchPayoutDetails();
    }
  }, [isOpen, payoutId]);

  const fetchPayoutDetails = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch(
        `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5100'}/api/payout/${payoutId}/details`,
        {
          headers: token ? { Authorization: `Bearer ${token}` } : {},
        }
      );

      if (!response.ok) {
        throw new Error('Failed to load payout details');
      }

      const data: PayoutDetails = await response.json();
      setPayout(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load payout details');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = async () => {
    if (!payout || payout.status !== 'pending') return;

    if (!window.confirm('Are you sure you want to cancel this payout?')) {
      return;
    }

    setIsCancelling(true);

    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch(
        `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5100'}/api/payout/${payoutId}/cancel`,
        {
          method: 'POST',
          headers: token ? { Authorization: `Bearer ${token}` } : {},
        }
      );

      if (!response.ok) {
        throw new Error('Failed to cancel payout');
      }

      if (onCancel) {
        onCancel(payoutId);
      }

      // Refresh details
      await fetchPayoutDetails();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to cancel payout');
    } finally {
      setIsCancelling(false);
    }
  };

  if (!isOpen) return null;

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'failed':
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      case 'processing':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-yellow-100 text-yellow-800';
    }
  };

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4">
      <div className="relative bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 rounded-t-lg flex items-center justify-between">
          <h2 className="text-xl font-semibold text-gray-900">Payout Details</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 focus:outline-none"
          >
            <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Content */}
        <div className="px-6 py-4">
          {isLoading && (
            <div className="flex justify-center py-8">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
          )}

          {error && (
            <div className="bg-red-50 border border-red-200 rounded-md p-4 text-red-800">
              {error}
            </div>
          )}

          {payout && (
            <div className="space-y-6">
              {/* Status and ID */}
              <div className="flex items-center justify-between">
                <div>
                  <span className={`inline-flex px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(payout.status)}`}>
                    {payout.status.charAt(0).toUpperCase() + payout.status.slice(1)}
                  </span>
                </div>
                <div className="text-sm text-gray-500">
                  ID: <span className="font-mono text-xs">{payout.id.slice(0, 8)}...</span>
                </div>
              </div>

              {/* Fee Breakdown */}
              <FeeBreakdown
                usdcAmount={payout.usdcAmount}
                usdAmountBeforeFees={payout.usdAmount}
                exchangeRate={payout.exchangeRate}
                conversionFeePercent={((payout.conversionFee / payout.usdAmount) * 100)}
                conversionFeeAmount={payout.conversionFee}
                payoutFeeAmount={payout.payoutFee}
                totalFees={payout.totalFees}
                netAmount={payout.netAmount}
                variant="detailed"
              />

              {/* Bank Account */}
              {payout.bankAccount && (
                <div className="bg-gray-50 rounded-lg p-4">
                  <h3 className="text-sm font-medium text-gray-900 mb-3">Bank Account</h3>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Account Holder:</span>
                      <span className="font-medium">{payout.bankAccount.accountHolderName}</span>
                    </div>
                    {payout.bankAccount.bankName && (
                      <div className="flex justify-between">
                        <span className="text-gray-600">Bank:</span>
                        <span className="font-medium">{payout.bankAccount.bankName}</span>
                      </div>
                    )}
                    <div className="flex justify-between">
                      <span className="text-gray-600">Account:</span>
                      <span className="font-medium font-mono">
                        {payout.bankAccount.accountType} •••• {payout.bankAccount.lastFourDigits}
                      </span>
                    </div>
                  </div>
                </div>
              )}

              {/* Timestamps */}
              <div className="bg-gray-50 rounded-lg p-4">
                <h3 className="text-sm font-medium text-gray-900 mb-3">Timeline</h3>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Initiated:</span>
                    <span className="font-medium">
                      {new Date(payout.initiatedAt).toLocaleString()}
                    </span>
                  </div>

                  {payout.estimatedArrival && !payout.completedAt && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Estimated Arrival:</span>
                      <span className="font-medium text-blue-600">
                        {new Date(payout.estimatedArrival).toLocaleDateString()}
                      </span>
                    </div>
                  )}

                  {payout.completedAt && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Completed:</span>
                      <span className="font-medium text-green-600">
                        {new Date(payout.completedAt).toLocaleString()}
                      </span>
                    </div>
                  )}

                  {payout.failureReason && (
                    <div className="mt-3 pt-3 border-t border-gray-200">
                      <span className="text-gray-600 block mb-1">Failure Reason:</span>
                      <div className="bg-red-50 border border-red-200 rounded p-2 text-red-800">
                        {payout.failureReason}
                      </div>
                    </div>
                  )}
                </div>
              </div>

              {/* Gateway Transaction ID */}
              {payout.gatewayTransactionId && (
                <div className="text-xs text-gray-500">
                  Gateway Transaction: <span className="font-mono">{payout.gatewayTransactionId}</span>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Footer */}
        {payout && (
          <div className="sticky bottom-0 bg-white border-t border-gray-200 px-6 py-4 rounded-b-lg flex justify-between">
            <button
              onClick={onClose}
              className="px-4 py-2 text-gray-700 border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Close
            </button>

            {payout.status === 'pending' && (
              <button
                onClick={handleCancel}
                disabled={isCancelling}
                className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isCancelling ? 'Cancelling...' : 'Cancel Payout'}
              </button>
            )}
          </div>
        )}
      </div>
    </div>
  );
};
