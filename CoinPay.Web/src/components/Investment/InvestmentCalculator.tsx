import React, { useState, useEffect } from 'react';
import { investmentService } from '@/services';
import type { InvestmentCalculation } from '@/types';

interface InvestmentCalculatorProps {
  initialAmount?: number;
  initialApy?: number;
  onCalculationChange?: (calculation: InvestmentCalculation) => void;
}

export const InvestmentCalculator: React.FC<InvestmentCalculatorProps> = ({
  initialAmount = 1000,
  initialApy = 8.5,
  onCalculationChange,
}) => {
  const [amount, setAmount] = useState<number>(initialAmount);
  const [apy, setApy] = useState<number>(initialApy);
  const [calculation, setCalculation] = useState<InvestmentCalculation | null>(null);

  // Calculate projections whenever amount or APY changes
  useEffect(() => {
    if (amount > 0 && apy >= 0) {
      const newCalculation = investmentService.calculateProjections(amount, apy);
      setCalculation(newCalculation);

      if (onCalculationChange) {
        onCalculationChange(newCalculation);
      }
    } else {
      setCalculation(null);
    }
  }, [amount, apy, onCalculationChange]);

  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseFloat(e.target.value) || 0;
    setAmount(value);
  };

  const handleApyChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseFloat(e.target.value) || 0;
    setApy(value);
  };

  // Quick amount presets
  const quickAmounts = [500, 1000, 5000, 10000];

  const handleQuickAmount = (preset: number) => {
    setAmount(preset);
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          Investment Calculator
        </h3>
        <p className="text-sm text-gray-600">
          Calculate potential earnings based on your investment amount and APY.
        </p>
      </div>

      <div className="space-y-6">
        {/* Amount Input */}
        <div>
          <div className="flex justify-between items-center mb-2">
            <label
              htmlFor="amount"
              className="block text-sm font-medium text-gray-700"
            >
              Investment Amount
            </label>
            <span className="text-sm font-semibold text-blue-600">
              {investmentService.formatCurrency(amount)}
            </span>
          </div>

          <input
            type="range"
            id="amount"
            min="100"
            max="50000"
            step="100"
            value={amount}
            onChange={handleAmountChange}
            className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-blue-600"
          />

          <div className="flex justify-between text-xs text-gray-500 mt-1">
            <span>$100</span>
            <span>$50,000</span>
          </div>

          {/* Quick Amount Buttons */}
          <div className="grid grid-cols-4 gap-2 mt-3">
            {quickAmounts.map((preset) => (
              <button
                key={preset}
                onClick={() => handleQuickAmount(preset)}
                className={`py-1 px-2 text-xs rounded-md font-medium transition-colors ${
                  amount === preset
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                ${preset.toLocaleString()}
              </button>
            ))}
          </div>

          {/* Custom Amount Input */}
          <div className="mt-3">
            <label htmlFor="customAmount" className="sr-only">
              Custom Amount
            </label>
            <div className="relative">
              <span className="absolute left-3 top-2 text-gray-500">$</span>
              <input
                type="number"
                id="customAmount"
                value={amount}
                onChange={handleAmountChange}
                min="0"
                step="100"
                className="w-full pl-8 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter custom amount"
              />
            </div>
          </div>
        </div>

        {/* APY Input */}
        <div>
          <div className="flex justify-between items-center mb-2">
            <label
              htmlFor="apy"
              className="block text-sm font-medium text-gray-700"
            >
              Annual Percentage Yield (APY)
            </label>
            <span className="text-sm font-semibold text-green-600">
              {investmentService.formatAPY(apy)}
            </span>
          </div>

          <input
            type="range"
            id="apy"
            min="0"
            max="20"
            step="0.1"
            value={apy}
            onChange={handleApyChange}
            className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-green-600"
          />

          <div className="flex justify-between text-xs text-gray-500 mt-1">
            <span>0%</span>
            <span>20%</span>
          </div>

          {/* Custom APY Input */}
          <div className="mt-3">
            <label htmlFor="customApy" className="sr-only">
              Custom APY
            </label>
            <div className="relative">
              <input
                type="number"
                id="customApy"
                value={apy}
                onChange={handleApyChange}
                min="0"
                max="100"
                step="0.1"
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
                placeholder="Enter custom APY"
              />
              <span className="absolute right-3 top-2 text-gray-500">%</span>
            </div>
          </div>
        </div>

        {/* Projection Results */}
        {calculation && (
          <div className="pt-6 border-t border-gray-200">
            <h4 className="text-sm font-semibold text-gray-900 mb-4">
              Projected Earnings
            </h4>

            <div className="space-y-4">
              {/* Daily Rewards */}
              <div className="flex items-center justify-between p-4 bg-blue-50 rounded-lg">
                <div>
                  <p className="text-sm text-gray-600">Daily Rewards</p>
                  <p className="text-xs text-gray-500 mt-1">
                    Every 24 hours
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-lg font-bold text-blue-600">
                    {investmentService.formatCurrency(
                      calculation.dailyReward,
                      8
                    )}
                  </p>
                </div>
              </div>

              {/* Monthly Rewards */}
              <div className="flex items-center justify-between p-4 bg-green-50 rounded-lg">
                <div>
                  <p className="text-sm text-gray-600">Monthly Rewards</p>
                  <p className="text-xs text-gray-500 mt-1">
                    After 30 days
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-lg font-bold text-green-600">
                    {investmentService.formatCurrency(
                      calculation.monthlyReward,
                      2
                    )}
                  </p>
                  <p className="text-xs text-gray-600 mt-1">
                    Total: {investmentService.formatCurrency(
                      calculation.totalReturn30Days,
                      2
                    )}
                  </p>
                </div>
              </div>

              {/* Yearly Rewards */}
              <div className="flex items-center justify-between p-4 bg-purple-50 rounded-lg">
                <div>
                  <p className="text-sm text-gray-600">Yearly Rewards</p>
                  <p className="text-xs text-gray-500 mt-1">
                    After 365 days
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-lg font-bold text-purple-600">
                    {investmentService.formatCurrency(
                      calculation.yearlyReward,
                      2
                    )}
                  </p>
                  <p className="text-xs text-gray-600 mt-1">
                    Total: {investmentService.formatCurrency(
                      calculation.totalReturn365Days,
                      2
                    )}
                  </p>
                </div>
              </div>
            </div>

            {/* ROI Summary */}
            <div className="mt-6 p-4 bg-gradient-to-r from-blue-600 to-purple-600 rounded-lg text-white">
              <p className="text-sm font-medium mb-2">
                1-Year Return on Investment
              </p>
              <div className="flex items-baseline">
                <span className="text-3xl font-bold">
                  +{((calculation.yearlyReward / amount) * 100).toFixed(2)}%
                </span>
                <span className="ml-2 text-sm opacity-90">
                  ({investmentService.formatCurrency(calculation.yearlyReward)})
                </span>
              </div>
            </div>
          </div>
        )}

        {/* Disclaimer */}
        <div className="pt-4 border-t border-gray-200">
          <div className="flex items-start">
            <svg
              className="w-5 h-5 text-yellow-600 mr-2 flex-shrink-0 mt-0.5"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                clipRule="evenodd"
              />
            </svg>
            <div className="text-xs text-gray-600">
              <p className="font-medium text-gray-700 mb-1">
                Important Disclaimer:
              </p>
              <p>
                These are estimated projections based on the current APY. Actual
                rewards may vary due to market conditions, APY changes, and other
                factors. Past performance does not guarantee future results.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default InvestmentCalculator;
