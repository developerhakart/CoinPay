import React, { useEffect, useState } from 'react';
import { useInvestmentStore } from '@/store/investmentStore';
import { investmentService } from '@/services';
import type { InvestmentPlan } from '@/types';

interface InvestmentPlansProps {
  onSelectPlan?: (plan: InvestmentPlan) => void;
  selectedPlanId?: string;
}

export const InvestmentPlans: React.FC<InvestmentPlansProps> = ({
  onSelectPlan,
  selectedPlanId,
}) => {
  const {
    plans,
    setPlans,
    selectPlan,
    isLoading,
    setLoading,
    setError,
    isConnected,
  } = useInvestmentStore();

  const [isLoadingPlans, setIsLoadingPlans] = useState(false);

  // Fetch plans on mount
  useEffect(() => {
    const fetchPlans = async () => {
      if (!isConnected) {
        return;
      }

      setIsLoadingPlans(true);
      setLoading(true);
      setError(null);

      try {
        const fetchedPlans = await investmentService.getWhiteBitPlans();
        setPlans(fetchedPlans);
      } catch (error: any) {
        const errorMessage =
          error.response?.data?.error || 'Failed to load investment plans';
        setError(errorMessage);
      } finally {
        setIsLoadingPlans(false);
        setLoading(false);
      }
    };

    fetchPlans();
  }, [isConnected, setPlans, setLoading, setError]);

  const handleSelectPlan = (plan: InvestmentPlan) => {
    selectPlan(plan);
    if (onSelectPlan) {
      onSelectPlan(plan);
    }
  };

  // Not connected state
  if (!isConnected) {
    return (
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="text-center py-8">
          <svg
            className="mx-auto h-12 w-12 text-gray-400 mb-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <h3 className="text-lg font-medium text-gray-900 mb-2">
            Connect Your WhiteBit Account
          </h3>
          <p className="text-sm text-gray-600">
            Please connect your WhiteBit account to view available investment plans.
          </p>
        </div>
      </div>
    );
  }

  // Loading state
  if (isLoadingPlans || isLoading) {
    return (
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="text-center py-8">
          <svg
            className="animate-spin h-8 w-8 text-blue-600 mx-auto mb-4"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              className="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              strokeWidth="4"
            ></circle>
            <path
              className="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            ></path>
          </svg>
          <p className="text-sm text-gray-600">Loading investment plans...</p>
        </div>
      </div>
    );
  }

  // Empty state
  if (plans.length === 0) {
    return (
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="text-center py-8">
          <svg
            className="mx-auto h-12 w-12 text-gray-400 mb-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"
            />
          </svg>
          <h3 className="text-lg font-medium text-gray-900 mb-2">
            No Plans Available
          </h3>
          <p className="text-sm text-gray-600">
            There are currently no investment plans available from WhiteBit.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          Available Investment Plans
        </h3>
        <p className="text-sm text-gray-600">
          Choose a plan to start earning rewards on your crypto holdings.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {plans.map((plan) => {
          const isSelected = selectedPlanId === plan.planId;

          return (
            <div
              key={plan.planId}
              className={`bg-white rounded-lg shadow-md overflow-hidden transition-all cursor-pointer ${
                isSelected
                  ? 'ring-2 ring-blue-500 shadow-lg'
                  : 'hover:shadow-lg'
              }`}
              onClick={() => handleSelectPlan(plan)}
            >
              {/* Header with Asset */}
              <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4">
                <div className="flex items-center justify-between">
                  <h4 className="text-xl font-bold text-white">
                    {plan.asset}
                  </h4>
                  {isSelected && (
                    <svg
                      className="w-6 h-6 text-white"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                    >
                      <path
                        fillRule="evenodd"
                        d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                        clipRule="evenodd"
                      />
                    </svg>
                  )}
                </div>
                <p className="text-blue-100 text-sm mt-1">{plan.term}</p>
              </div>

              {/* APY Highlight */}
              <div className="px-6 py-5 bg-gray-50 border-b border-gray-200">
                <div className="text-center">
                  <div className="text-3xl font-bold text-blue-600">
                    {plan.apyFormatted}
                  </div>
                  <div className="text-sm text-gray-600 mt-1">APY</div>
                </div>
              </div>

              {/* Plan Details */}
              <div className="px-6 py-4 space-y-3">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Minimum:</span>
                  <span className="font-medium text-gray-900">
                    {investmentService.formatCurrency(plan.minAmount)}
                  </span>
                </div>

                {plan.maxAmount && (
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Maximum:</span>
                    <span className="font-medium text-gray-900">
                      {investmentService.formatCurrency(plan.maxAmount)}
                    </span>
                  </div>
                )}

                {plan.description && (
                  <div className="pt-3 border-t border-gray-200">
                    <p className="text-xs text-gray-600">{plan.description}</p>
                  </div>
                )}
              </div>

              {/* Select Button */}
              <div className="px-6 py-4 bg-gray-50">
                <button
                  onClick={() => handleSelectPlan(plan)}
                  className={`w-full py-2 px-4 rounded-md font-medium transition-colors ${
                    isSelected
                      ? 'bg-blue-600 text-white hover:bg-blue-700'
                      : 'bg-white text-blue-600 border border-blue-600 hover:bg-blue-50'
                  }`}
                >
                  {isSelected ? 'Selected' : 'Select Plan'}
                </button>
              </div>
            </div>
          );
        })}
      </div>

      {/* Info Banner */}
      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
        <div className="flex">
          <svg
            className="w-5 h-5 text-blue-600 mr-3 flex-shrink-0 mt-0.5"
            fill="currentColor"
            viewBox="0 0 20 20"
          >
            <path
              fillRule="evenodd"
              d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
              clipRule="evenodd"
            />
          </svg>
          <div className="text-sm text-blue-800">
            <p className="font-medium mb-1">Investment Information:</p>
            <ul className="list-disc list-inside space-y-1">
              <li>Flexible terms - withdraw anytime</li>
              <li>Daily rewards calculated automatically</li>
              <li>Rewards compound when reinvested</li>
              <li>No lock-up period required</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default InvestmentPlans;
