import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { WithdrawalWizard } from '../components/Withdrawal/WithdrawalWizard';

export const WithdrawPage: React.FC = () => {
  const navigate = useNavigate();

  // Mock balance - in production this would come from wallet context/API
  const [userBalance] = useState(1000.00); // Mock 1000 USDC balance

  const handleComplete = (payoutId: string) => {
    // Navigate to payout status page
    navigate(`/payout/${payoutId}/status`);
  };

  const handleCancel = () => {
    // Navigate back to wallet or dashboard
    navigate('/wallet');
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-4xl mx-auto">
        <div className="mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Withdraw to Bank Account</h1>
          <p className="mt-2 text-sm text-gray-600">
            Convert your USDC to USD and withdraw to your bank account
          </p>
        </div>

        <WithdrawalWizard
          userBalance={userBalance}
          onComplete={handleComplete}
          onCancel={handleCancel}
        />
      </div>
    </div>
  );
};
