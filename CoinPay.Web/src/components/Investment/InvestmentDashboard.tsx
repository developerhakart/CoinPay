import React, { useEffect, useState } from 'react';
import { useInvestmentStore } from '@/store/investmentStore';
import { investmentService } from '@/services';
import ConnectWhiteBitForm from './ConnectWhiteBitForm';
import CreateInvestmentWizard from './CreateInvestmentWizard';
import PositionCard from './PositionCard';
import PositionDetailsModal from './PositionDetailsModal';
import type { InvestmentPosition } from '@/types';

export const InvestmentDashboard: React.FC = () => {
  const {
    isConnected,
    positions,
    setPositions,
    totalPortfolioValue,
    totalPrincipal,
    totalRewards,
    showCreateWizard,
    toggleCreateWizard,
    setConnectionStatus,
    updatePosition,
    setError,
  } = useInvestmentStore();

  const [isLoadingPositions, setIsLoadingPositions] = useState(false);
  const [selectedPositionId, setSelectedPositionId] = useState<string | null>(null);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showWithdrawConfirm, setShowWithdrawConfirm] = useState<string | null>(null);
  const [isWithdrawing, setIsWithdrawing] = useState(false);

  // Check connection status on mount
  useEffect(() => {
    const checkConnection = async () => {
      try {
        const status = await investmentService.getWhiteBitStatus();
        setConnectionStatus(status);
      } catch (error) {
        console.error('Failed to check connection status:', error);
      }
    };
    checkConnection();
  }, [setConnectionStatus]);

  // Fetch positions when connected
  useEffect(() => {
    if (isConnected) {
      fetchPositions();
    }
  }, [isConnected]);

  const fetchPositions = async () => {
    setIsLoadingPositions(true);

    try {
      const fetchedPositions = await investmentService.getPositions();
      setPositions(fetchedPositions);
    } catch (error: any) {
      const errorMessage = error.response?.data?.error || 'Failed to load positions';
      setError(errorMessage);
    } finally {
      setIsLoadingPositions(false);
    }
  };

  const handleViewDetails = (position: InvestmentPosition) => {
    setSelectedPositionId(position.id);
    setShowDetailsModal(true);
  };

  const handleWithdrawRequest = (positionId: string) => {
    setShowWithdrawConfirm(positionId);
  };

  const handleWithdrawConfirm = async (positionId: string) => {
    setIsWithdrawing(true);
    setError(null);

    try {
      await investmentService.withdrawInvestment(positionId);

      // Update position status
      updatePosition(positionId, { status: 'Closed' });

      // Refresh positions
      await fetchPositions();

      setShowWithdrawConfirm(null);
    } catch (error: any) {
      const errorMessage = error.response?.data?.error || 'Failed to withdraw investment';
      setError(errorMessage);
    } finally {
      setIsWithdrawing(false);
    }
  };

  const activePositions = positions.filter((p) => p.status === 'Active');
  const closedPositions = positions.filter((p) => p.status === 'Closed');

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Investment Dashboard
          </h1>
          <p className="text-gray-600">
            Manage your crypto investment positions and earn rewards
          </p>
        </div>

        {/* Connection Section */}
        {!isConnected && (
          <div className="mb-8">
            <ConnectWhiteBitForm />
          </div>
        )}

        {isConnected && (
          <>
            {/* Portfolio Summary */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-sm text-gray-600">Total Portfolio Value</p>
                  <svg className="w-5 h-5 text-blue-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.51-1.31c-.562-.649-1.413-1.076-2.353-1.253V5z" clipRule="evenodd" />
                  </svg>
                </div>
                <p className="text-3xl font-bold text-gray-900">
                  {investmentService.formatCurrency(totalPortfolioValue)}
                </p>
                <p className="text-sm text-green-600 mt-2">
                  +{totalRewards > 0 ? ((totalRewards / totalPrincipal) * 100).toFixed(2) : '0.00'}% Total Gain
                </p>
              </div>

              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-sm text-gray-600">Total Principal</p>
                  <svg className="w-5 h-5 text-gray-600" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zm2 6a2 2 0 012-2h8a2 2 0 012 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2v-4zm6 4a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                  </svg>
                </div>
                <p className="text-3xl font-bold text-gray-900">
                  {investmentService.formatCurrency(totalPrincipal)}
                </p>
                <p className="text-sm text-gray-600 mt-2">
                  {activePositions.length} Active Position{activePositions.length !== 1 ? 's' : ''}
                </p>
              </div>

              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-sm text-gray-600">Total Rewards</p>
                  <svg className="w-5 h-5 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M3.293 9.707a1 1 0 010-1.414l6-6a1 1 0 011.414 0l6 6a1 1 0 01-1.414 1.414L11 5.414V17a1 1 0 11-2 0V5.414L4.707 9.707a1 1 0 01-1.414 0z" clipRule="evenodd" />
                  </svg>
                </div>
                <p className="text-3xl font-bold text-green-600">
                  {investmentService.formatCurrency(totalRewards)}
                </p>
                <p className="text-sm text-gray-600 mt-2">
                  Accrued Earnings
                </p>
              </div>
            </div>

            {/* Quick Actions */}
            <div className="bg-white rounded-lg shadow-md p-6 mb-8">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <button
                  onClick={toggleCreateWizard}
                  className="flex items-center justify-center py-3 px-4 bg-blue-600 text-white rounded-md font-medium hover:bg-blue-700 transition-colors"
                >
                  <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 10-2 0v2H7a1 1 0 100 2h2v2a1 1 0 102 0v-2h2a1 1 0 100-2h-2V7z" clipRule="evenodd" />
                  </svg>
                  Create New Investment
                </button>

                <button
                  onClick={fetchPositions}
                  disabled={isLoadingPositions}
                  className="flex items-center justify-center py-3 px-4 bg-white border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50 transition-colors"
                >
                  <svg className={`w-5 h-5 mr-2 ${isLoadingPositions ? 'animate-spin' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  Refresh Positions
                </button>

                <button className="flex items-center justify-center py-3 px-4 bg-white border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50 transition-colors">
                  <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  View Analytics
                </button>
              </div>
            </div>

            {/* Active Positions */}
            {activePositions.length > 0 && (
              <div className="mb-8">
                <h3 className="text-xl font-semibold text-gray-900 mb-4">
                  Active Positions ({activePositions.length})
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {activePositions.map((position) => (
                    <PositionCard
                      key={position.id}
                      position={position}
                      onViewDetails={handleViewDetails}
                      onWithdraw={handleWithdrawRequest}
                    />
                  ))}
                </div>
              </div>
            )}

            {/* Empty State */}
            {activePositions.length === 0 && !isLoadingPositions && (
              <div className="bg-white rounded-lg shadow-md p-12 text-center mb-8">
                <div className="mx-auto w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mb-4">
                  <svg className="w-8 h-8 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-2">
                  No Active Investments
                </h3>
                <p className="text-gray-600 mb-6">
                  Start earning rewards by creating your first investment position.
                </p>
                <button
                  onClick={toggleCreateWizard}
                  className="inline-flex items-center px-6 py-3 bg-blue-600 text-white rounded-md font-medium hover:bg-blue-700 transition-colors"
                >
                  <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 10-2 0v2H7a1 1 0 100 2h2v2a1 1 0 102 0v-2h2a1 1 0 100-2h-2V7z" clipRule="evenodd" />
                  </svg>
                  Create Your First Investment
                </button>
              </div>
            )}

            {/* Closed Positions */}
            {closedPositions.length > 0 && (
              <div className="mb-8">
                <details className="group">
                  <summary className="cursor-pointer list-none">
                    <div className="flex items-center justify-between p-4 bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow">
                      <h3 className="text-lg font-semibold text-gray-900">
                        Closed Positions ({closedPositions.length})
                      </h3>
                      <svg className="w-5 h-5 text-gray-600 group-open:rotate-180 transition-transform" fill="currentColor" viewBox="0 0 20 20">
                        <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                      </svg>
                    </div>
                  </summary>
                  <div className="mt-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {closedPositions.map((position) => (
                      <PositionCard
                        key={position.id}
                        position={position}
                        onViewDetails={handleViewDetails}
                      />
                    ))}
                  </div>
                </details>
              </div>
            )}

            {/* Loading State */}
            {isLoadingPositions && (
              <div className="text-center py-12">
                <svg className="animate-spin h-8 w-8 text-blue-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <p className="text-gray-600">Loading positions...</p>
              </div>
            )}
          </>
        )}

        {/* Modals */}
        {showCreateWizard && (
          <div className="fixed inset-0 z-50 overflow-y-auto flex items-center justify-center">
            <div className="fixed inset-0 bg-gray-500 bg-opacity-75" onClick={toggleCreateWizard}></div>
            <div className="relative z-50">
              <CreateInvestmentWizard
                onSuccess={() => {
                  fetchPositions();
                }}
                onCancel={toggleCreateWizard}
              />
            </div>
          </div>
        )}

        {showDetailsModal && selectedPositionId && (
          <PositionDetailsModal
            positionId={selectedPositionId}
            isOpen={showDetailsModal}
            onClose={() => {
              setShowDetailsModal(false);
              setSelectedPositionId(null);
            }}
            onWithdraw={handleWithdrawRequest}
          />
        )}

        {/* Withdraw Confirmation Modal */}
        {showWithdrawConfirm && (
          <div className="fixed inset-0 z-50 overflow-y-auto flex items-center justify-center">
            <div className="fixed inset-0 bg-gray-500 bg-opacity-75" onClick={() => setShowWithdrawConfirm(null)}></div>
            <div className="relative bg-white rounded-lg p-6 max-w-md w-full mx-4 z-50">
              <h3 className="text-lg font-bold text-gray-900 mb-4">Confirm Withdrawal</h3>
              <p className="text-gray-600 mb-6">
                Are you sure you want to withdraw this investment? This action cannot be undone.
              </p>
              <div className="flex space-x-3">
                <button
                  onClick={() => setShowWithdrawConfirm(null)}
                  disabled={isWithdrawing}
                  className="flex-1 py-2 px-4 bg-white border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  onClick={() => handleWithdrawConfirm(showWithdrawConfirm)}
                  disabled={isWithdrawing}
                  className={`flex-1 py-2 px-4 bg-red-600 text-white rounded-md font-medium hover:bg-red-700 ${
                    isWithdrawing ? 'opacity-50 cursor-not-allowed' : ''
                  }`}
                >
                  {isWithdrawing ? 'Withdrawing...' : 'Confirm'}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default InvestmentDashboard;
