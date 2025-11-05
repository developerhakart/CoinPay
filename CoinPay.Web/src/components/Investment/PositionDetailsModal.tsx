import React, { useState, useEffect } from 'react';
import { investmentService } from '@/services';
import type { InvestmentPositionDetail } from '@/types';

interface PositionDetailsModalProps {
  positionId: string;
  isOpen: boolean;
  onClose: () => void;
  onWithdraw?: (positionId: string) => void;
}

export const PositionDetailsModal: React.FC<PositionDetailsModalProps> = ({
  positionId,
  isOpen,
  onClose,
  onWithdraw,
}) => {
  const [position, setPosition] = useState<InvestmentPositionDetail | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showWithdrawConfirm, setShowWithdrawConfirm] = useState(false);

  // Fetch position details when modal opens
  useEffect(() => {
    if (isOpen && positionId) {
      fetchPositionDetails();
    }
  }, [isOpen, positionId]);

  const fetchPositionDetails = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const details = await investmentService.getPositionDetails(positionId);
      setPosition(details);
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || 'Failed to load position details';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const handleWithdraw = () => {
    if (onWithdraw) {
      onWithdraw(positionId);
    }
    setShowWithdrawConfirm(false);
    onClose();
  };

  const handleClose = () => {
    setShowWithdrawConfirm(false);
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        {/* Backdrop */}
        <div
          className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75"
          onClick={handleClose}
        ></div>

        {/* Modal */}
        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full">
          {/* Header */}
          <div className="bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-4">
            <div className="flex items-center justify-between">
              <h3 className="text-xl font-bold text-white">Position Details</h3>
              <button
                onClick={handleClose}
                className="text-white hover:text-gray-200 transition-colors"
              >
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          </div>

          {/* Content */}
          <div className="px-6 py-6 max-h-[70vh] overflow-y-auto">
            {isLoading && (
              <div className="text-center py-12">
                <svg className="animate-spin h-8 w-8 text-blue-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <p className="text-gray-600">Loading position details...</p>
              </div>
            )}

            {error && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
                <p className="text-red-800">{error}</p>
              </div>
            )}

            {position && !showWithdrawConfirm && (
              <div className="space-y-6">
                {/* Overview */}
                <div className="bg-gradient-to-br from-blue-50 to-purple-50 rounded-lg p-6">
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <p className="text-sm text-gray-600 mb-1">Asset</p>
                      <p className="text-2xl font-bold text-gray-900">{position.asset}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-600 mb-1">Plan</p>
                      <p className="text-lg font-semibold text-gray-900">{position.planName}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-600 mb-1">Status</p>
                      <span className={`inline-block px-3 py-1 rounded-full text-sm font-medium ${
                        position.status === 'Active' ? 'bg-green-100 text-green-800' :
                        position.status === 'Closed' ? 'bg-gray-100 text-gray-800' :
                        'bg-red-100 text-red-800'
                      }`}>
                        {position.status}
                      </span>
                    </div>
                    <div>
                      <p className="text-sm text-gray-600 mb-1">APY</p>
                      <p className="text-2xl font-bold text-blue-600">{investmentService.formatAPY(position.apy)}</p>
                    </div>
                  </div>
                </div>

                {/* Financial Summary */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div className="bg-white border border-gray-200 rounded-lg p-4">
                    <p className="text-sm text-gray-600 mb-2">Principal Amount</p>
                    <p className="text-2xl font-bold text-gray-900">
                      {investmentService.formatCurrency(position.principalAmount)}
                    </p>
                  </div>

                  <div className="bg-white border border-gray-200 rounded-lg p-4">
                    <p className="text-sm text-gray-600 mb-2">Accrued Rewards</p>
                    <p className="text-2xl font-bold text-green-600">
                      {investmentService.formatCurrency(position.accruedRewards)}
                    </p>
                  </div>

                  <div className="bg-white border border-gray-200 rounded-lg p-4">
                    <p className="text-sm text-gray-600 mb-2">Current Value</p>
                    <p className="text-2xl font-bold text-blue-600">
                      {investmentService.formatCurrency(position.currentValue)}
                    </p>
                    <p className="text-sm text-gray-600 mt-1">
                      +{((position.accruedRewards / position.principalAmount) * 100).toFixed(2)}%
                    </p>
                  </div>
                </div>

                {/* Projected Rewards */}
                <div className="bg-white border border-gray-200 rounded-lg p-4">
                  <h4 className="text-lg font-semibold text-gray-900 mb-4">Projected Earnings</h4>
                  <div className="grid grid-cols-3 gap-4">
                    <div className="text-center">
                      <p className="text-sm text-gray-600 mb-1">Daily</p>
                      <p className="text-lg font-bold text-blue-600">
                        {investmentService.formatCurrency(position.projectedRewards.daily, 4)}
                      </p>
                    </div>
                    <div className="text-center">
                      <p className="text-sm text-gray-600 mb-1">Monthly (30d)</p>
                      <p className="text-lg font-bold text-green-600">
                        {investmentService.formatCurrency(position.projectedRewards.monthly)}
                      </p>
                    </div>
                    <div className="text-center">
                      <p className="text-sm text-gray-600 mb-1">Yearly (365d)</p>
                      <p className="text-lg font-bold text-purple-600">
                        {investmentService.formatCurrency(position.projectedRewards.yearly)}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Timeline */}
                <div className="bg-white border border-gray-200 rounded-lg p-4">
                  <h4 className="text-lg font-semibold text-gray-900 mb-4">Timeline</h4>
                  <div className="space-y-3 text-sm">
                    {position.startDate && (
                      <div className="flex justify-between">
                        <span className="text-gray-600">Start Date:</span>
                        <span className="font-medium text-gray-900">
                          {new Date(position.startDate).toLocaleString()}
                        </span>
                      </div>
                    )}
                    {position.endDate && (
                      <div className="flex justify-between">
                        <span className="text-gray-600">End Date:</span>
                        <span className="font-medium text-gray-900">
                          {new Date(position.endDate).toLocaleString()}
                        </span>
                      </div>
                    )}
                    <div className="flex justify-between">
                      <span className="text-gray-600">Days Held:</span>
                      <span className="font-medium text-gray-900">{position.daysHeld} days</span>
                    </div>
                    {position.lastSyncedAt && (
                      <div className="flex justify-between">
                        <span className="text-gray-600">Last Synced:</span>
                        <span className="font-medium text-gray-900">
                          {new Date(position.lastSyncedAt).toLocaleString()}
                        </span>
                      </div>
                    )}
                  </div>
                </div>

                {/* Transaction History */}
                <div className="bg-white border border-gray-200 rounded-lg p-4">
                  <h4 className="text-lg font-semibold text-gray-900 mb-4">Transaction History</h4>
                  {position.transactions && position.transactions.length > 0 ? (
                    <div className="space-y-2">
                      {position.transactions.map((tx) => (
                        <div
                          key={tx.id}
                          className="flex items-center justify-between py-3 px-4 bg-gray-50 rounded-lg"
                        >
                          <div className="flex items-center">
                            <div className={`w-2 h-2 rounded-full mr-3 ${
                              tx.status === 'Confirmed' ? 'bg-green-500' :
                              tx.status === 'Pending' ? 'bg-yellow-500' :
                              'bg-red-500'
                            }`}></div>
                            <div>
                              <p className="font-medium text-gray-900">{tx.type}</p>
                              <p className="text-sm text-gray-600">
                                {new Date(tx.createdAt).toLocaleString()}
                              </p>
                            </div>
                          </div>
                          <div className="text-right">
                            <p className="font-medium text-gray-900">
                              {investmentService.formatCurrency(tx.amount)}
                            </p>
                            <p className="text-sm text-gray-600">{tx.status}</p>
                          </div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="text-center text-gray-600 py-4">No transactions yet</p>
                  )}
                </div>
              </div>
            )}

            {/* Withdraw Confirmation */}
            {position && showWithdrawConfirm && (
              <div className="space-y-6">
                <div className="text-center py-6">
                  <div className="mx-auto w-16 h-16 bg-yellow-100 rounded-full flex items-center justify-center mb-4">
                    <svg className="w-8 h-8 text-yellow-600" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                    </svg>
                  </div>
                  <h3 className="text-xl font-bold text-gray-900 mb-2">Confirm Withdrawal</h3>
                  <p className="text-gray-600 mb-6">
                    Are you sure you want to withdraw this investment? This action cannot be undone.
                  </p>

                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
                    <div className="space-y-2 text-sm">
                      <div className="flex justify-between">
                        <span className="text-gray-700">Principal:</span>
                        <span className="font-bold text-gray-900">
                          {investmentService.formatCurrency(position.principalAmount)}
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-700">Accrued Rewards:</span>
                        <span className="font-bold text-green-600">
                          {investmentService.formatCurrency(position.accruedRewards)}
                        </span>
                      </div>
                      <div className="flex justify-between pt-2 border-t border-blue-300">
                        <span className="text-gray-700 font-semibold">Total to Withdraw:</span>
                        <span className="font-bold text-gray-900 text-lg">
                          {investmentService.formatCurrency(position.currentValue)}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="flex space-x-3">
                  <button
                    onClick={() => setShowWithdrawConfirm(false)}
                    className="flex-1 py-2 px-4 bg-white border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50"
                  >
                    Cancel
                  </button>
                  <button
                    onClick={handleWithdraw}
                    className="flex-1 py-2 px-4 bg-red-600 text-white rounded-md font-medium hover:bg-red-700"
                  >
                    Confirm Withdrawal
                  </button>
                </div>
              </div>
            )}
          </div>

          {/* Footer */}
          {position && !showWithdrawConfirm && (
            <div className="px-6 py-4 bg-gray-50 border-t border-gray-200 flex justify-between">
              <button
                onClick={handleClose}
                className="px-6 py-2 bg-white border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50"
              >
                Close
              </button>

              {position.status === 'Active' && onWithdraw && (
                <button
                  onClick={() => setShowWithdrawConfirm(true)}
                  className="px-6 py-2 bg-red-600 text-white rounded-md font-medium hover:bg-red-700"
                >
                  Withdraw Investment
                </button>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default PositionDetailsModal;
