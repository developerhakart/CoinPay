import React, { useState, useEffect } from 'react';
import { investmentService } from '@/services';
import type { InvestmentPosition } from '@/types';

interface PositionCardProps {
  position: InvestmentPosition;
  onViewDetails?: (position: InvestmentPosition) => void;
  onWithdraw?: (positionId: string) => void;
}

export const PositionCard: React.FC<PositionCardProps> = ({
  position,
  onViewDetails,
  onWithdraw,
}) => {
  const [liveRewards, setLiveRewards] = useState(position.accruedRewards);
  const [liveValue, setLiveValue] = useState(position.currentValue);

  // Update rewards in real-time (every second)
  useEffect(() => {
    const interval = setInterval(() => {
      if (position.status === 'Active') {
        // Calculate time since last sync
        const now = new Date();
        const startDate = position.startDate ? new Date(position.startDate) : new Date();
        const secondsElapsed = (now.getTime() - startDate.getTime()) / 1000;
        const daysElapsed = secondsElapsed / (24 * 60 * 60);

        // Calculate accrued rewards up to current second
        const dailyReward = position.estimatedDailyReward;
        const newAccrued = dailyReward * daysElapsed;
        const newValue = position.principalAmount + newAccrued;

        setLiveRewards(newAccrued);
        setLiveValue(newValue);
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [position]);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active':
        return 'bg-green-100 text-green-800';
      case 'Closed':
        return 'bg-gray-100 text-gray-800';
      case 'Failed':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Active':
        return (
          <span className="w-2 h-2 rounded-full bg-green-500 mr-2 animate-pulse"></span>
        );
      case 'Closed':
        return <span className="w-2 h-2 rounded-full bg-gray-500 mr-2"></span>;
      case 'Failed':
        return <span className="w-2 h-2 rounded-full bg-red-500 mr-2"></span>;
      default:
        return null;
    }
  };

  const profitLoss = liveValue - position.principalAmount;
  const profitLossPercent = (profitLoss / position.principalAmount) * 100;

  return (
    <div className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow overflow-hidden">
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center">
            <div className="w-12 h-12 bg-white bg-opacity-20 rounded-full flex items-center justify-center mr-3">
              <span className="text-2xl font-bold text-white">
                {position.asset[0]}
              </span>
            </div>
            <div>
              <h3 className="text-lg font-bold text-white">{position.asset}</h3>
              <p className="text-blue-100 text-sm">{position.planId}</p>
            </div>
          </div>

          <span
            className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(
              position.status
            )}`}
          >
            {getStatusIcon(position.status)}
            {position.status}
          </span>
        </div>
      </div>

      {/* Main Content */}
      <div className="px-6 py-4">
        {/* Current Value */}
        <div className="mb-4">
          <p className="text-sm text-gray-600 mb-1">Current Value</p>
          <div className="flex items-baseline">
            <p className="text-3xl font-bold text-gray-900">
              {investmentService.formatCurrency(liveValue)}
            </p>
            <span
              className={`ml-3 text-sm font-medium ${
                profitLoss >= 0 ? 'text-green-600' : 'text-red-600'
              }`}
            >
              {profitLoss >= 0 ? '+' : ''}
              {investmentService.formatCurrency(profitLoss)} ({profitLossPercent.toFixed(2)}%)
            </span>
          </div>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div>
            <p className="text-xs text-gray-600 mb-1">Principal</p>
            <p className="text-lg font-semibold text-gray-900">
              {investmentService.formatCurrency(position.principalAmount)}
            </p>
          </div>

          <div>
            <p className="text-xs text-gray-600 mb-1">Accrued Rewards</p>
            <p className="text-lg font-semibold text-green-600">
              {investmentService.formatCurrency(liveRewards, 4)}
            </p>
          </div>

          <div>
            <p className="text-xs text-gray-600 mb-1">APY</p>
            <p className="text-lg font-semibold text-blue-600">
              {investmentService.formatAPY(position.apy)}
            </p>
          </div>

          <div>
            <p className="text-xs text-gray-600 mb-1">Days Held</p>
            <p className="text-lg font-semibold text-gray-900">
              {position.daysHeld} days
            </p>
          </div>
        </div>

        {/* Earnings Breakdown */}
        <div className="bg-gray-50 rounded-lg p-3 mb-4">
          <p className="text-xs font-semibold text-gray-700 mb-2">
            Estimated Earnings
          </p>
          <div className="space-y-1 text-xs">
            <div className="flex justify-between">
              <span className="text-gray-600">Daily:</span>
              <span className="font-medium text-gray-900">
                {investmentService.formatCurrency(position.estimatedDailyReward, 4)}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Monthly:</span>
              <span className="font-medium text-gray-900">
                {investmentService.formatCurrency(position.estimatedMonthlyReward)}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Yearly:</span>
              <span className="font-medium text-gray-900">
                {investmentService.formatCurrency(position.estimatedYearlyReward)}
              </span>
            </div>
          </div>
        </div>

        {/* Timestamps */}
        {position.startDate && (
          <div className="text-xs text-gray-500 mb-4">
            <p>
              Started: {new Date(position.startDate).toLocaleDateString()}{' '}
              {new Date(position.startDate).toLocaleTimeString()}
            </p>
            {position.lastSyncedAt && (
              <p className="mt-1">
                Last Synced:{' '}
                {new Date(position.lastSyncedAt).toLocaleDateString()}{' '}
                {new Date(position.lastSyncedAt).toLocaleTimeString()}
              </p>
            )}
          </div>
        )}
      </div>

      {/* Actions */}
      <div className="px-6 py-4 bg-gray-50 border-t border-gray-200 flex space-x-3">
        <button
          onClick={() => onViewDetails && onViewDetails(position)}
          className="flex-1 py-2 px-4 bg-white border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50 transition-colors"
        >
          View Details
        </button>

        {position.status === 'Active' && onWithdraw && (
          <button
            onClick={() => onWithdraw(position.id)}
            className="flex-1 py-2 px-4 bg-red-600 text-white rounded-md font-medium hover:bg-red-700 transition-colors"
          >
            Withdraw
          </button>
        )}
      </div>
    </div>
  );
};

export default PositionCard;
