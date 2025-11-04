import React from 'react';

export interface FeeBreakdownProps {
  usdcAmount: number;
  usdAmountBeforeFees: number;
  exchangeRate: number;
  conversionFeePercent: number;
  conversionFeeAmount: number;
  payoutFeeAmount: number;
  totalFees: number;
  netAmount: number;
  /**
   * Display variant
   * @default 'default'
   */
  variant?: 'default' | 'compact' | 'detailed';
  /**
   * Show effective fee rate
   * @default true
   */
  showEffectiveRate?: boolean;
  /**
   * Custom className
   */
  className?: string;
}

/**
 * Fee Breakdown Component
 * Displays transparent fee structure for payout transactions
 * Helps users understand exactly what they'll receive
 */
export const FeeBreakdown: React.FC<FeeBreakdownProps> = ({
  usdcAmount,
  usdAmountBeforeFees,
  exchangeRate,
  conversionFeePercent,
  conversionFeeAmount,
  payoutFeeAmount,
  totalFees,
  netAmount,
  variant = 'default',
  showEffectiveRate = true,
  className = '',
}) => {
  const effectiveFeeRate = usdAmountBeforeFees > 0
    ? ((totalFees / usdAmountBeforeFees) * 100).toFixed(2)
    : '0.00';

  if (variant === 'compact') {
    return (
      <div className={`text-sm space-y-1 ${className}`}>
        <div className="flex justify-between text-gray-600">
          <span>{usdcAmount.toFixed(2)} USDC</span>
          <span className="font-mono">${usdAmountBeforeFees.toFixed(2)}</span>
        </div>
        <div className="flex justify-between text-gray-500 text-xs">
          <span>Total Fees</span>
          <span className="font-mono">-${totalFees.toFixed(2)}</span>
        </div>
        <div className="flex justify-between font-semibold text-green-600 pt-1 border-t border-gray-200">
          <span>You Receive</span>
          <span className="font-mono">${netAmount.toFixed(2)}</span>
        </div>
      </div>
    );
  }

  if (variant === 'detailed') {
    return (
      <div className={`bg-gray-50 rounded-lg p-4 space-y-3 ${className}`}>
        <h3 className="font-semibold text-gray-900 mb-2">Transaction Breakdown</h3>

        {/* USDC Amount */}
        <div className="flex justify-between items-center">
          <span className="text-gray-700">USDC Amount</span>
          <span className="font-mono font-medium">{usdcAmount.toFixed(2)} USDC</span>
        </div>

        {/* Exchange Rate */}
        <div className="flex justify-between items-center">
          <span className="text-gray-700">Exchange Rate</span>
          <span className="font-mono text-sm">1 USDC = {exchangeRate.toFixed(4)} USD</span>
        </div>

        <div className="border-t border-gray-300 my-2"></div>

        {/* USD Before Fees */}
        <div className="flex justify-between items-center">
          <span className="text-gray-700">USD Value</span>
          <span className="font-mono font-medium">${usdAmountBeforeFees.toFixed(2)}</span>
        </div>

        {/* Conversion Fee */}
        <div className="flex justify-between items-center text-gray-600">
          <span className="flex items-center">
            <span>Conversion Fee ({conversionFeePercent}%)</span>
            <svg className="w-4 h-4 ml-1 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </span>
          <span className="font-mono">-${conversionFeeAmount.toFixed(2)}</span>
        </div>

        {/* Payout Fee */}
        <div className="flex justify-between items-center text-gray-600">
          <span className="flex items-center">
            <span>Payout Fee</span>
            <svg className="w-4 h-4 ml-1 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </span>
          <span className="font-mono">-${payoutFeeAmount.toFixed(2)}</span>
        </div>

        {/* Total Fees */}
        <div className="flex justify-between items-center text-gray-700 font-medium">
          <span>Total Fees</span>
          <span className="font-mono">-${totalFees.toFixed(2)}</span>
        </div>

        {showEffectiveRate && (
          <div className="flex justify-between items-center text-sm text-gray-500">
            <span>Effective Rate</span>
            <span className="font-mono">{effectiveFeeRate}%</span>
          </div>
        )}

        <div className="border-t-2 border-gray-400 my-2"></div>

        {/* Net Amount */}
        <div className="flex justify-between items-center font-bold text-lg text-green-600">
          <span>You'll Receive</span>
          <span className="font-mono">${netAmount.toFixed(2)}</span>
        </div>

        {/* Estimated Arrival */}
        <div className="text-xs text-gray-500 text-center mt-3 pt-3 border-t border-gray-200">
          Estimated arrival: 3-5 business days
        </div>
      </div>
    );
  }

  // Default variant
  return (
    <div className={`bg-blue-50 border border-blue-200 rounded-md p-4 space-y-2 ${className}`}>
      <h3 className="font-semibold text-blue-900">Fee Breakdown</h3>

      <div className="space-y-1 text-sm">
        <div className="flex justify-between">
          <span>USDC Amount:</span>
          <span className="font-mono">{usdcAmount.toFixed(2)} USDC</span>
        </div>

        <div className="flex justify-between">
          <span>Exchange Rate:</span>
          <span className="font-mono text-xs">1 USDC = {exchangeRate.toFixed(4)} USD</span>
        </div>

        <div className="flex justify-between">
          <span>USD Before Fees:</span>
          <span className="font-mono">${usdAmountBeforeFees.toFixed(2)}</span>
        </div>

        <div className="flex justify-between text-gray-600">
          <span>Conversion Fee ({conversionFeePercent}%):</span>
          <span className="font-mono">-${conversionFeeAmount.toFixed(2)}</span>
        </div>

        <div className="flex justify-between text-gray-600">
          <span>Payout Fee:</span>
          <span className="font-mono">-${payoutFeeAmount.toFixed(2)}</span>
        </div>

        <div className="flex justify-between font-bold text-blue-900 pt-2 border-t border-blue-300">
          <span>You'll Receive:</span>
          <span className="font-mono">${netAmount.toFixed(2)}</span>
        </div>

        {showEffectiveRate && (
          <div className="flex justify-between text-xs text-gray-600 pt-1">
            <span>Effective Fee Rate:</span>
            <span className="font-mono">{effectiveFeeRate}%</span>
          </div>
        )}
      </div>
    </div>
  );
};

/**
 * Inline Fee Summary Badge
 * Shows just total fees and net amount in a compact badge
 */
export const FeeSummaryBadge: React.FC<{
  totalFees: number;
  netAmount: number;
  className?: string;
}> = ({ totalFees, netAmount, className = '' }) => {
  return (
    <div className={`inline-flex items-center space-x-2 px-3 py-1.5 bg-gray-100 rounded-md ${className}`}>
      <span className="text-xs text-gray-600">Fees: ${totalFees.toFixed(2)}</span>
      <span className="text-gray-400">•</span>
      <span className="text-sm font-semibold text-green-600">Net: ${netAmount.toFixed(2)}</span>
    </div>
  );
};
