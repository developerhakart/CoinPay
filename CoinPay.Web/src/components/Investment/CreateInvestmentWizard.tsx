import React, { useState, useEffect } from 'react';
import { useInvestmentStore } from '@/store/investmentStore';
import { investmentService } from '@/services';
import type { InvestmentPlan, InvestmentCalculation } from '@/types';

interface CreateInvestmentWizardProps {
  onSuccess?: (investmentId: string) => void;
  onCancel?: () => void;
}

type WizardStep = 'plan' | 'amount' | 'confirm';

export const CreateInvestmentWizard: React.FC<CreateInvestmentWizardProps> = ({
  onSuccess,
  onCancel,
}) => {
  const {
    plans,
    selectedPlan,
    selectPlan,
    setPlans,
    isCreatingInvestment,
    setCreatingInvestment,
    addPosition,
    setError,
    toggleCreateWizard,
  } = useInvestmentStore();

  const [currentStep, setCurrentStep] = useState<WizardStep>('plan');
  const [amount, setAmount] = useState<number>(1000);
  const [selectedAsset, setSelectedAsset] = useState<string>('USDC');
  const [demoTokenBalances, setDemoTokenBalances] = useState<any[]>([]);
  const [calculation, setCalculation] = useState<InvestmentCalculation | null>(null);
  const [validationError, setValidationError] = useState<string>('');

  // Fetch plans if not loaded
  useEffect(() => {
    const fetchPlans = async () => {
      if (plans.length === 0) {
        try {
          const fetchedPlans = await investmentService.getWhiteBitPlans();
          setPlans(fetchedPlans);
        } catch (error) {
          console.error('Failed to load plans:', error);
        }
      }
    };
    fetchPlans();
  }, [plans.length, setPlans]);

  // Fetch demo token balances
  useEffect(() => {
    const fetchDemoBalances = async () => {
      try {
        const balances = await investmentService.getDemoTokenBalances();
        setDemoTokenBalances(balances);
      } catch (error) {
        console.error('Failed to load demo token balances:', error);
      }
    };
    fetchDemoBalances();
  }, []);

  // Calculate projections when amount or plan changes
  useEffect(() => {
    if (selectedPlan && amount > 0) {
      const calc = investmentService.calculateProjections(amount, selectedPlan.apy);
      setCalculation(calc);
    }
  }, [selectedPlan, amount]);

  // Validate amount
  const validateAmount = (value: number): string => {
    if (!selectedPlan) return '';

    if (value <= 0) {
      return 'Amount must be greater than zero';
    }

    if (value < selectedPlan.minAmount) {
      return `Minimum amount is ${investmentService.formatCurrency(selectedPlan.minAmount)}`;
    }

    if (selectedPlan.maxAmount && value > selectedPlan.maxAmount) {
      return `Maximum amount is ${investmentService.formatCurrency(selectedPlan.maxAmount)}`;
    }

    return '';
  };

  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseFloat(e.target.value) || 0;
    setAmount(value);
    const error = validateAmount(value);
    setValidationError(error);
  };

  const handlePlanSelect = (plan: InvestmentPlan) => {
    selectPlan(plan);
    // Validate amount with new plan
    const error = validateAmount(amount);
    setValidationError(error);
  };

  const handleNext = () => {
    if (currentStep === 'plan' && selectedPlan) {
      setCurrentStep('amount');
    } else if (currentStep === 'amount' && !validationError) {
      setCurrentStep('confirm');
    }
  };

  const handleBack = () => {
    if (currentStep === 'amount') {
      setCurrentStep('plan');
    } else if (currentStep === 'confirm') {
      setCurrentStep('amount');
    }
  };

  const handleSubmit = async () => {
    if (!selectedPlan || !calculation) return;

    setCreatingInvestment(true);
    setError(null);

    try {
      const response = await investmentService.createInvestment({
        planId: selectedPlan.planId,
        amount: amount,
        asset: selectedAsset,
        walletId: '00000000-0000-0000-0000-000000000000',
      });

      // Add position to store
      addPosition({
        id: response.investmentId,
        planId: response.planId,
        asset: response.asset,
        principalAmount: response.amount,
        currentValue: response.amount,
        accruedRewards: 0,
        apy: response.apy,
        status: 'Active',
        startDate: response.createdAt,
        daysHeld: 0,
        estimatedDailyReward: response.estimatedDailyReward,
        estimatedMonthlyReward: response.estimatedMonthlyReward,
        estimatedYearlyReward: response.estimatedYearlyReward,
      });

      // Success callback
      if (onSuccess) {
        onSuccess(response.investmentId);
      }

      // Close wizard
      toggleCreateWizard();

      // Reset wizard
      setCurrentStep('plan');
      selectPlan(null);
      setAmount(1000);
    } catch (error: any) {
      const errorMessage = error.response?.data?.error || 'Failed to create investment';
      setError(errorMessage);
    } finally {
      setCreatingInvestment(false);
    }
  };

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    }
    toggleCreateWizard();

    // Reset wizard
    setCurrentStep('plan');
    selectPlan(null);
    setAmount(1000);
  };

  // Step indicators
  const steps = [
    { id: 'plan', label: 'Select Plan' },
    { id: 'amount', label: 'Enter Amount' },
    { id: 'confirm', label: 'Confirm' },
  ];

  const currentStepIndex = steps.findIndex((s) => s.id === currentStep);

  return (
    <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
      {/* Header */}
      <div className="px-6 py-4 border-b border-gray-200">
        <h2 className="text-2xl font-bold text-gray-900">Create Investment</h2>
        <p className="text-sm text-gray-600 mt-1">
          Follow the steps to create your investment position
        </p>
      </div>

      {/* Step Indicator */}
      <div className="px-6 py-4 bg-gray-50">
        <div className="flex items-center justify-between">
          {steps.map((step, index) => (
            <React.Fragment key={step.id}>
              <div className="flex items-center">
                <div
                  className={`w-10 h-10 rounded-full flex items-center justify-center font-semibold ${
                    index <= currentStepIndex
                      ? 'bg-blue-600 text-white'
                      : 'bg-gray-300 text-gray-600'
                  }`}
                >
                  {index + 1}
                </div>
                <span
                  className={`ml-3 font-medium ${
                    index <= currentStepIndex ? 'text-gray-900' : 'text-gray-500'
                  }`}
                >
                  {step.label}
                </span>
              </div>
              {index < steps.length - 1 && (
                <div
                  className={`flex-1 h-1 mx-4 ${
                    index < currentStepIndex ? 'bg-blue-600' : 'bg-gray-300'
                  }`}
                />
              )}
            </React.Fragment>
          ))}
        </div>
      </div>

      {/* Content */}
      <div className="px-6 py-6 min-h-[400px]">
        {/* Step 1: Plan Selection */}
        {currentStep === 'plan' && (
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Choose an Investment Plan
            </h3>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {plans.map((plan) => (
                <div
                  key={plan.planId}
                  onClick={() => handlePlanSelect(plan)}
                  className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
                    selectedPlan?.planId === plan.planId
                      ? 'border-blue-600 bg-blue-50'
                      : 'border-gray-200 hover:border-blue-300'
                  }`}
                >
                  <div className="flex items-start justify-between mb-3">
                    <div>
                      <h4 className="text-lg font-bold text-gray-900">{plan.asset}</h4>
                      <p className="text-sm text-gray-600">{plan.term}</p>
                    </div>
                    <div className="text-right">
                      <div className="text-2xl font-bold text-blue-600">
                        {plan.apyFormatted}
                      </div>
                      <div className="text-xs text-gray-500">APY</div>
                    </div>
                  </div>

                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Min Amount:</span>
                      <span className="font-medium">
                        {investmentService.formatCurrency(plan.minAmount)}
                      </span>
                    </div>
                    {plan.maxAmount && (
                      <div className="flex justify-between">
                        <span className="text-gray-600">Max Amount:</span>
                        <span className="font-medium">
                          {investmentService.formatCurrency(plan.maxAmount)}
                        </span>
                      </div>
                    )}
                  </div>

                  {selectedPlan?.planId === plan.planId && (
                    <div className="mt-3 flex items-center text-blue-600">
                      <svg className="w-5 h-5 mr-1" fill="currentColor" viewBox="0 0 20 20">
                        <path
                          fillRule="evenodd"
                          d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                          clipRule="evenodd"
                        />
                      </svg>
                      <span className="text-sm font-medium">Selected</span>
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Step 2: Amount Input */}
        {currentStep === 'amount' && selectedPlan && (
          <div className="space-y-6">
            <h3 className="text-lg font-semibold text-gray-900">
              Enter Investment Amount
            </h3>

            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <div className="flex justify-between items-center">
                <div>
                  <p className="text-sm text-gray-600">Selected Plan</p>
                  <p className="text-lg font-bold text-gray-900">{selectedPlan.asset}</p>
                </div>
                <div className="text-right">
                  <p className="text-sm text-gray-600">APY</p>
                  <p className="text-2xl font-bold text-blue-600">
                    {selectedPlan.apyFormatted}
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Select Asset
              </label>
              <div className="grid grid-cols-3 gap-3 mb-6">
                {['USDC', 'DUSDT', 'DBTC'].map((asset) => {
                  const balance = demoTokenBalances.find(b => b.tokenSymbol === asset);
                  const isDemoToken = asset !== 'USDC';

                  return (
                    <button
                      key={asset}
                      type="button"
                      onClick={() => setSelectedAsset(asset)}
                      className={`p-4 border-2 rounded-lg text-center transition-all ${
                        selectedAsset === asset
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-200 hover:border-blue-300'
                      }`}
                    >
                      <div className="font-bold text-lg">{asset}</div>
                      {isDemoToken && (
                        <div className="text-xs mt-1">
                          {balance ? (
                            <span className="text-green-600">
                              Balance: {balance.balance.toFixed(2)}
                            </span>
                          ) : (
                            <span className="text-gray-500">No balance</span>
                          )}
                        </div>
                      )}
                      {isDemoToken && (
                        <div className="text-xs text-blue-600 mt-1">Demo Token</div>
                      )}
                    </button>
                  );
                })}
              </div>
            </div>

            <div>
              <label htmlFor="investAmount" className="block text-sm font-medium text-gray-700 mb-2">
                Amount ({selectedAsset})
              </label>
              <div className="relative">
                <span className="absolute left-4 top-3 text-gray-500 text-lg">$</span>
                <input
                  type="number"
                  id="investAmount"
                  value={amount}
                  onChange={handleAmountChange}
                  min={selectedPlan.minAmount}
                  max={selectedPlan.maxAmount || undefined}
                  step="100"
                  className={`w-full pl-10 pr-4 py-3 text-lg border rounded-lg focus:outline-none focus:ring-2 ${
                    validationError
                      ? 'border-red-500 focus:ring-red-500'
                      : 'border-gray-300 focus:ring-blue-500'
                  }`}
                />
              </div>
              {validationError && (
                <p className="mt-2 text-sm text-red-600">{validationError}</p>
              )}
              <p className="mt-2 text-sm text-gray-500">
                Min: {investmentService.formatCurrency(selectedPlan.minAmount)}
                {selectedPlan.maxAmount && ` | Max: ${investmentService.formatCurrency(selectedPlan.maxAmount)}`}
              </p>
            </div>

            {calculation && !validationError && (
              <div className="space-y-3">
                <h4 className="text-sm font-semibold text-gray-900">Projected Earnings</h4>
                <div className="grid grid-cols-3 gap-3">
                  <div className="bg-gray-50 rounded-lg p-3 text-center">
                    <p className="text-xs text-gray-600 mb-1">Daily</p>
                    <p className="text-lg font-bold text-blue-600">
                      {investmentService.formatCurrency(calculation.dailyReward, 4)}
                    </p>
                  </div>
                  <div className="bg-gray-50 rounded-lg p-3 text-center">
                    <p className="text-xs text-gray-600 mb-1">Monthly</p>
                    <p className="text-lg font-bold text-green-600">
                      {investmentService.formatCurrency(calculation.monthlyReward)}
                    </p>
                  </div>
                  <div className="bg-gray-50 rounded-lg p-3 text-center">
                    <p className="text-xs text-gray-600 mb-1">Yearly</p>
                    <p className="text-lg font-bold text-purple-600">
                      {investmentService.formatCurrency(calculation.yearlyReward)}
                    </p>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}

        {/* Step 3: Confirmation */}
        {currentStep === 'confirm' && selectedPlan && calculation && (
          <div className="space-y-6">
            <h3 className="text-lg font-semibold text-gray-900">
              Confirm Your Investment
            </h3>

            <div className="bg-gradient-to-br from-blue-50 to-purple-50 rounded-lg p-6 border border-blue-200">
              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <span className="text-gray-700">Investment Plan:</span>
                  <span className="font-bold text-gray-900">{selectedPlan.asset}</span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-700">Asset:</span>
                  <span className="font-bold text-gray-900">
                    {selectedAsset}
                    {selectedAsset !== 'USDC' && <span className="text-xs ml-2 text-blue-600">(Demo)</span>}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-700">APY:</span>
                  <span className="font-bold text-blue-600 text-xl">
                    {selectedPlan.apyFormatted}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-700">Investment Amount:</span>
                  <span className="font-bold text-gray-900 text-2xl">
                    {investmentService.formatCurrency(amount)}
                  </span>
                </div>
              </div>
            </div>

            <div className="bg-white border border-gray-200 rounded-lg p-4">
              <h4 className="text-sm font-semibold text-gray-900 mb-3">
                Expected Returns
              </h4>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-sm text-gray-600">Daily Reward:</span>
                  <span className="font-medium text-blue-600">
                    {investmentService.formatCurrency(calculation.dailyReward, 4)}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-sm text-gray-600">30-Day Reward:</span>
                  <span className="font-medium text-green-600">
                    {investmentService.formatCurrency(calculation.monthlyReward)}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-sm text-gray-600">365-Day Reward:</span>
                  <span className="font-medium text-purple-600">
                    {investmentService.formatCurrency(calculation.yearlyReward)}
                  </span>
                </div>
                <div className="pt-3 border-t border-gray-200">
                  <div className="flex justify-between">
                    <span className="text-sm font-semibold text-gray-900">
                      1-Year Total Value:
                    </span>
                    <span className="font-bold text-gray-900 text-lg">
                      {investmentService.formatCurrency(calculation.totalReturn365Days)}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <div className="flex">
                <svg className="w-5 h-5 text-yellow-600 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
                <div className="text-sm text-yellow-800">
                  <p className="font-medium mb-1">Important Notice:</p>
                  <p>These are projected earnings based on current APY. Actual returns may vary due to market conditions.</p>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Footer */}
      <div className="px-6 py-4 bg-gray-50 border-t border-gray-200 flex justify-between">
        <button
          onClick={handleCancel}
          className="px-6 py-2 text-gray-700 hover:text-gray-900 font-medium"
          disabled={isCreatingInvestment}
        >
          Cancel
        </button>

        <div className="flex space-x-3">
          {currentStep !== 'plan' && (
            <button
              onClick={handleBack}
              className="px-6 py-2 border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50"
              disabled={isCreatingInvestment}
            >
              Back
            </button>
          )}

          {currentStep !== 'confirm' ? (
            <button
              onClick={handleNext}
              disabled={
                (currentStep === 'plan' && !selectedPlan) ||
                (currentStep === 'amount' && (!!validationError || amount <= 0))
              }
              className={`px-6 py-2 rounded-md font-medium text-white transition-colors ${
                (currentStep === 'plan' && !selectedPlan) ||
                (currentStep === 'amount' && (!!validationError || amount <= 0))
                  ? 'bg-gray-400 cursor-not-allowed'
                  : 'bg-blue-600 hover:bg-blue-700'
              }`}
            >
              Next
            </button>
          ) : (
            <button
              onClick={handleSubmit}
              disabled={isCreatingInvestment}
              className={`px-6 py-2 rounded-md font-medium text-white transition-colors ${
                isCreatingInvestment
                  ? 'bg-gray-400 cursor-not-allowed'
                  : 'bg-green-600 hover:bg-green-700'
              }`}
            >
              {isCreatingInvestment ? (
                <span className="flex items-center">
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Creating...
                </span>
              ) : (
                'Create Investment'
              )}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default CreateInvestmentWizard;
