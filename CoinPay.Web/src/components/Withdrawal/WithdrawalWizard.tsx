import React, { useState, useEffect } from 'react';
import { payoutApi, type ConversionPreviewResponse } from '../../api/payoutApi';
import { bankAccountApi, type BankAccount } from '../../api/bankAccountApi';
import { FeeBreakdown } from '../Fees/FeeBreakdown';

interface WithdrawalWizardProps {
  userBalance: number; // USDC balance
  onComplete: (payoutId: string) => void;
  onCancel: () => void;
}

type WizardStep = 'amount' | 'bank' | 'review' | 'confirm';

export const WithdrawalWizard: React.FC<WithdrawalWizardProps> = ({
  userBalance,
  onComplete,
  onCancel,
}) => {
  const [currentStep, setCurrentStep] = useState<WizardStep>('amount');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form data
  const [usdcAmount, setUsdcAmount] = useState<string>('');
  const [selectedBankAccount, setSelectedBankAccount] = useState<BankAccount | null>(null);
  const [conversionPreview, setConversionPreview] = useState<ConversionPreviewResponse | null>(null);
  const [payoutId, setPayoutId] = useState<string | null>(null);

  // Bank accounts
  const [bankAccounts, setBankAccounts] = useState<BankAccount[]>([]);
  const [loadingBankAccounts, setLoadingBankAccounts] = useState(false);

  // Fetch bank accounts
  useEffect(() => {
    if (currentStep === 'bank') {
      fetchBankAccounts();
    }
  }, [currentStep]);

  const fetchBankAccounts = async () => {
    setLoadingBankAccounts(true);
    try {
      const response = await bankAccountApi.getAll();
      setBankAccounts(response.bankAccounts);

      // Auto-select primary account
      const primary = response.bankAccounts.find(acc => acc.isPrimary);
      if (primary) {
        setSelectedBankAccount(primary);
      }
    } catch (err) {
      setError('Failed to load bank accounts');
    } finally {
      setLoadingBankAccounts(false);
    }
  };

  // Fetch conversion preview when amount changes
  useEffect(() => {
    const fetchPreview = async () => {
      const amount = parseFloat(usdcAmount);
      if (amount > 0 && amount <= userBalance) {
        try {
          const preview = await payoutApi.getConversionPreview({ usdcAmount: amount });
          setConversionPreview(preview);
          setError(null);
        } catch (err) {
          console.error('Failed to fetch conversion preview:', err);
        }
      } else {
        setConversionPreview(null);
      }
    };

    const timer = setTimeout(fetchPreview, 500);
    return () => clearTimeout(timer);
  }, [usdcAmount, userBalance]);

  const handleNext = () => {
    setError(null);

    if (currentStep === 'amount') {
      const amount = parseFloat(usdcAmount);
      if (!amount || amount <= 0) {
        setError('Please enter a valid amount');
        return;
      }
      if (amount > userBalance) {
        setError('Insufficient balance');
        return;
      }
      setCurrentStep('bank');
    } else if (currentStep === 'bank') {
      if (!selectedBankAccount) {
        setError('Please select a bank account');
        return;
      }
      setCurrentStep('review');
    } else if (currentStep === 'review') {
      handleSubmit();
    }
  };

  const handleBack = () => {
    setError(null);
    if (currentStep === 'bank') {
      setCurrentStep('amount');
    } else if (currentStep === 'review') {
      setCurrentStep('bank');
    }
  };

  const handleSubmit = async () => {
    if (!selectedBankAccount || !conversionPreview) return;

    setIsSubmitting(true);
    setError(null);

    try {
      const payout = await payoutApi.initiatePayout({
        bankAccountId: selectedBankAccount.id,
        usdcAmount: parseFloat(usdcAmount),
      });

      setPayoutId(payout.id);
      setCurrentStep('confirm');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to initiate payout');
      setIsSubmitting(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto bg-white rounded-lg shadow-lg p-6">
      {/* Progress Steps */}
      <div className="mb-8">
        <div className="flex items-center justify-between">
          {['Amount', 'Bank Account', 'Review', 'Confirm'].map((label, idx) => {
            const stepKey = ['amount', 'bank', 'review', 'confirm'][idx];
            const isActive = currentStep === stepKey;
            const isCompleted = ['amount', 'bank', 'review', 'confirm'].indexOf(currentStep) > idx;

            return (
              <React.Fragment key={label}>
                <div className="flex flex-col items-center">
                  <div className={`w-10 h-10 rounded-full flex items-center justify-center font-semibold ${
                    isCompleted ? 'bg-green-500 text-white' :
                    isActive ? 'bg-blue-600 text-white' :
                    'bg-gray-300 text-gray-600'
                  }`}>
                    {isCompleted ? '✓' : idx + 1}
                  </div>
                  <span className={`mt-2 text-sm ${isActive ? 'text-blue-600 font-medium' : 'text-gray-600'}`}>
                    {label}
                  </span>
                </div>
                {idx < 3 && (
                  <div className={`flex-1 h-1 mx-2 ${
                    isCompleted ? 'bg-green-500' : 'bg-gray-300'
                  }`} />
                )}
              </React.Fragment>
            );
          })}
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-md text-red-800 text-sm">
          {error}
        </div>
      )}

      {/* Step 1: Amount */}
      {currentStep === 'amount' && (
        <div className="space-y-6">
          <h2 className="text-2xl font-bold text-gray-900">Enter Withdrawal Amount</h2>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Amount (USDC)
            </label>
            <input
              type="number"
              value={usdcAmount}
              onChange={(e) => setUsdcAmount(e.target.value)}
              placeholder="0.00"
              step="0.01"
              min="1"
              max={userBalance}
              className="w-full px-4 py-3 text-lg border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <p className="mt-2 text-sm text-gray-500">
              Available balance: {userBalance.toFixed(2)} USDC
            </p>
          </div>

          {conversionPreview && (
            <div className="bg-blue-50 border border-blue-200 rounded-md p-4 space-y-2">
              <h3 className="font-semibold text-blue-900">Conversion Preview</h3>
              <div className="space-y-1 text-sm">
                <div className="flex justify-between">
                  <span>Exchange Rate:</span>
                  <span className="font-mono">1 USDC = {conversionPreview.exchangeRate.toFixed(4)} USD</span>
                </div>
                <div className="flex justify-between">
                  <span>USD Before Fees:</span>
                  <span className="font-mono">${conversionPreview.usdAmountBeforeFees.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Conversion Fee ({conversionPreview.conversionFeePercent}%):</span>
                  <span className="font-mono">-${conversionPreview.conversionFeeAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Payout Fee:</span>
                  <span className="font-mono">-${conversionPreview.payoutFeeAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between font-bold text-blue-900 pt-2 border-t border-blue-300">
                  <span>You'll Receive:</span>
                  <span className="font-mono">${conversionPreview.netUsdAmount.toFixed(2)}</span>
                </div>
              </div>
            </div>
          )}
        </div>
      )}

      {/* Step 2: Bank Account */}
      {currentStep === 'bank' && (
        <div className="space-y-6">
          <h2 className="text-2xl font-bold text-gray-900">Select Bank Account</h2>

          {loadingBankAccounts ? (
            <div className="flex justify-center py-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            </div>
          ) : bankAccounts.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-gray-600">No bank accounts found. Please add one first.</p>
            </div>
          ) : (
            <div className="space-y-3">
              {bankAccounts.map((account) => (
                <div
                  key={account.id}
                  onClick={() => setSelectedBankAccount(account)}
                  className={`p-4 border-2 rounded-lg cursor-pointer transition-all ${
                    selectedBankAccount?.id === account.id
                      ? 'border-blue-500 bg-blue-50'
                      : 'border-gray-200 hover:border-blue-300'
                  }`}
                >
                  <div className="flex items-center justify-between">
                    <div>
                      <div className="font-semibold text-gray-900">{account.accountHolderName}</div>
                      <div className="text-sm text-gray-600">
                        {account.bankName && `${account.bankName} • `}
                        {account.accountType} •••• {account.lastFourDigits}
                      </div>
                    </div>
                    {account.isPrimary && (
                      <span className="px-2 py-1 text-xs font-medium bg-blue-100 text-blue-800 rounded">
                        Primary
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Step 3: Review */}
      {currentStep === 'review' && conversionPreview && selectedBankAccount && (
        <div className="space-y-6">
          <h2 className="text-2xl font-bold text-gray-900">Review & Confirm</h2>

          <div className="bg-gray-50 rounded-lg p-4 space-y-4">
            <div>
              <h3 className="text-sm font-medium text-gray-500 mb-2">Withdrawal Amount</h3>
              <p className="text-2xl font-bold text-gray-900">{usdcAmount} USDC</p>
            </div>

            <div className="border-t border-gray-200 pt-4">
              <h3 className="text-sm font-medium text-gray-500 mb-2">Bank Account</h3>
              <p className="font-semibold">{selectedBankAccount.accountHolderName}</p>
              <p className="text-sm text-gray-600">
                {selectedBankAccount.bankName && `${selectedBankAccount.bankName} • `}
                {selectedBankAccount.accountType} •••• {selectedBankAccount.lastFourDigits}
              </p>
            </div>

            <div className="border-t border-gray-200 pt-4">
              <h3 className="text-sm font-medium text-gray-500 mb-3">Transaction Breakdown</h3>
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span>USDC Amount:</span>
                  <span className="font-mono">{conversionPreview.usdcAmount.toFixed(2)} USDC</span>
                </div>
                <div className="flex justify-between">
                  <span>Exchange Rate:</span>
                  <span className="font-mono">1 USDC = {conversionPreview.exchangeRate.toFixed(4)} USD</span>
                </div>
                <div className="flex justify-between">
                  <span>USD Value:</span>
                  <span className="font-mono">${conversionPreview.usdAmountBeforeFees.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Conversion Fee:</span>
                  <span className="font-mono">-${conversionPreview.conversionFeeAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Payout Fee:</span>
                  <span className="font-mono">-${conversionPreview.payoutFeeAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between font-bold text-lg pt-2 border-t border-gray-300">
                  <span>Net Amount:</span>
                  <span className="font-mono text-green-600">${conversionPreview.netUsdAmount.toFixed(2)}</span>
                </div>
              </div>
            </div>

            <div className="border-t border-gray-200 pt-4">
              <p className="text-sm text-gray-600">
                Estimated arrival: 3-5 business days
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Step 4: Confirmation */}
      {currentStep === 'confirm' && conversionPreview && selectedBankAccount && payoutId && (
        <div className="space-y-6">
          {/* Success Header */}
          <div className="text-center py-4">
            <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-10 h-10 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <h2 className="text-3xl font-bold text-gray-900 mb-2">Withdrawal Initiated!</h2>
            <p className="text-gray-600">
              Your payout has been successfully submitted and is being processed.
            </p>
          </div>

          {/* Payout ID */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-blue-900">Payout ID</p>
                <p className="font-mono text-sm text-blue-700 mt-1">{payoutId.slice(0, 8)}...{payoutId.slice(-8)}</p>
              </div>
              <span className="px-3 py-1 bg-yellow-100 text-yellow-800 text-sm font-medium rounded-full">
                Pending
              </span>
            </div>
          </div>

          {/* Fee Breakdown */}
          <FeeBreakdown
            usdcAmount={parseFloat(usdcAmount)}
            usdAmountBeforeFees={conversionPreview.usdAmountBeforeFees}
            exchangeRate={conversionPreview.exchangeRate}
            conversionFeePercent={conversionPreview.conversionFeePercent}
            conversionFeeAmount={conversionPreview.conversionFeeAmount}
            payoutFeeAmount={conversionPreview.payoutFeeAmount}
            totalFees={conversionPreview.totalFees}
            netAmount={conversionPreview.netUsdAmount}
            variant="detailed"
            showEffectiveRate={true}
          />

          {/* Bank Account Details */}
          <div className="bg-gray-50 rounded-lg p-4">
            <h3 className="text-sm font-medium text-gray-900 mb-3">Destination Bank Account</h3>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Account Holder:</span>
                <span className="font-medium">{selectedBankAccount.accountHolderName}</span>
              </div>
              {selectedBankAccount.bankName && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Bank:</span>
                  <span className="font-medium">{selectedBankAccount.bankName}</span>
                </div>
              )}
              <div className="flex justify-between">
                <span className="text-gray-600">Account:</span>
                <span className="font-medium font-mono">
                  {selectedBankAccount.accountType} •••• {selectedBankAccount.lastFourDigits}
                </span>
              </div>
            </div>
          </div>

          {/* Next Steps */}
          <div className="bg-green-50 border border-green-200 rounded-lg p-4">
            <h3 className="text-sm font-medium text-green-900 mb-3">What happens next?</h3>
            <ul className="space-y-2 text-sm text-green-800">
              <li className="flex items-start">
                <svg className="w-5 h-5 text-green-600 mr-2 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>Your payout will be processed within 1-2 business days</span>
              </li>
              <li className="flex items-start">
                <svg className="w-5 h-5 text-green-600 mr-2 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>Funds should arrive in your bank account within 3-5 business days</span>
              </li>
              <li className="flex items-start">
                <svg className="w-5 h-5 text-green-600 mr-2 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>You'll receive email notifications for status updates</span>
              </li>
              <li className="flex items-start">
                <svg className="w-5 h-5 text-green-600 mr-2 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>You can track your payout status in the Payout History page</span>
              </li>
            </ul>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-3 pt-4">
            <button
              onClick={() => onComplete(payoutId)}
              className="flex-1 px-6 py-3 bg-blue-600 text-white font-semibold rounded-md hover:bg-blue-700 transition-colors"
            >
              View Payout History
            </button>
            <button
              onClick={() => {
                // Reset wizard for new withdrawal
                setCurrentStep('amount');
                setUsdcAmount('');
                setSelectedBankAccount(null);
                setConversionPreview(null);
                setPayoutId(null);
                setError(null);
              }}
              className="flex-1 px-6 py-3 bg-white text-blue-600 font-semibold border-2 border-blue-600 rounded-md hover:bg-blue-50 transition-colors"
            >
              New Withdrawal
            </button>
          </div>
        </div>
      )}

      {/* Navigation Buttons */}
      {currentStep !== 'confirm' && (
        <div className="flex justify-between mt-8 pt-6 border-t border-gray-200">
          <button
            onClick={currentStep === 'amount' ? onCancel : handleBack}
            disabled={isSubmitting}
            className="px-6 py-2 text-gray-700 border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50"
          >
            {currentStep === 'amount' ? 'Cancel' : 'Back'}
          </button>
          <button
            onClick={handleNext}
            disabled={isSubmitting || (currentStep === 'amount' && (!conversionPreview || parseFloat(usdcAmount) <= 0))}
            className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSubmitting ? (
              <span className="flex items-center">
                <svg className="animate-spin -ml-1 mr-2 h-5 w-5" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Processing...
              </span>
            ) : currentStep === 'review' ? 'Confirm & Withdraw' : 'Next'}
          </button>
        </div>
      )}
    </div>
  );
};
