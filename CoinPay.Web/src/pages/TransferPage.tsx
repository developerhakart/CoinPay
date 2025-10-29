import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/store';
import { walletService } from '@/services/walletService';
import { transactionService } from '@/services/transactionService';

interface TransferFormData {
  toAddress: string;
  amount: string;
  description: string;
}

export function TransferPage() {
  const { user } = useAuthStore();
  const navigate = useNavigate();

  const [formData, setFormData] = useState<TransferFormData>({
    toAddress: '',
    amount: '',
    description: ''
  });

  const [balance, setBalance] = useState<number>(0);
  const [isLoadingBalance, setIsLoadingBalance] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showPreview, setShowPreview] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errors, setErrors] = useState<Partial<TransferFormData>>({});

  // Fetch balance on mount
  useEffect(() => {
    const fetchBalance = async () => {
      if (!user?.walletAddress) return;

      try {
        setIsLoadingBalance(true);
        const response = await walletService.getBalance(user.walletAddress);
        setBalance(response.balance || 0);
      } catch (err) {
        console.error('Failed to fetch balance:', err);
        setBalance(0);
      } finally {
        setIsLoadingBalance(false);
      }
    };

    fetchBalance();
  }, [user?.walletAddress]);

  // Validate Ethereum address
  const isValidAddress = (address: string): boolean => {
    return /^0x[a-fA-F0-9]{40}$/.test(address);
  };

  // Validate form
  const validateForm = (): boolean => {
    const newErrors: Partial<TransferFormData> = {};

    // Validate recipient address
    if (!formData.toAddress) {
      newErrors.toAddress = 'Recipient address is required';
    } else if (!isValidAddress(formData.toAddress)) {
      newErrors.toAddress = 'Invalid Ethereum address format';
    } else if (formData.toAddress.toLowerCase() === user?.walletAddress?.toLowerCase()) {
      newErrors.toAddress = 'Cannot send to your own address';
    }

    // Validate amount
    if (!formData.amount) {
      newErrors.amount = 'Amount is required';
    } else {
      const amountNum = parseFloat(formData.amount);
      if (isNaN(amountNum) || amountNum <= 0) {
        newErrors.amount = 'Amount must be greater than 0';
      } else if (amountNum > balance) {
        newErrors.amount = `Insufficient balance. You have ${balance.toFixed(2)} USDC`;
      } else if (amountNum < 0.000001) {
        newErrors.amount = 'Minimum amount is 0.000001 USDC';
      } else if (amountNum > 1000000) {
        newErrors.amount = 'Maximum amount is 1,000,000 USDC';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle input changes
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear error for this field
    if (errors[name as keyof TransferFormData]) {
      setErrors(prev => ({ ...prev, [name]: undefined }));
    }
    setError(null);
  };

  // Handle amount input with USDC formatting
  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let value = e.target.value;
    // Allow only numbers and decimal point
    value = value.replace(/[^0-9.]/g, '');
    // Allow only one decimal point
    const parts = value.split('.');
    if (parts.length > 2) {
      value = parts[0]! + '.' + parts.slice(1).join('');
    }
    // Limit to 6 decimal places
    if (parts.length === 2 && parts[1]!.length > 6) {
      value = parts[0]! + '.' + parts[1]!.substring(0, 6);
    }

    setFormData(prev => ({ ...prev, amount: value }));
    if (errors.amount) {
      setErrors(prev => ({ ...prev, amount: undefined }));
    }
    setError(null);
  };

  // Set max amount
  const handleMaxClick = () => {
    setFormData(prev => ({ ...prev, amount: balance.toString() }));
    if (errors.amount) {
      setErrors(prev => ({ ...prev, amount: undefined }));
    }
  };

  // Handle preview
  const handlePreview = () => {
    if (validateForm()) {
      setShowPreview(true);
    }
  };

  // Handle submit
  const handleSubmit = async () => {
    if (!validateForm()) return;

    setIsSubmitting(true);
    setError(null);

    try {
      const response = await transactionService.create({
        amount: parseFloat(formData.amount),
        currency: 'USDC',
        type: 'Transfer',
        status: 'Pending',
        senderName: user?.username || 'Unknown',
        receiverName: formData.toAddress.slice(0, 8) + '...',
        description: formData.description || `Transfer ${formData.amount} USDC to ${formData.toAddress.slice(0, 8)}...`
      });

      // Success - navigate to transactions page
      navigate('/transactions', {
        state: {
          message: 'Transfer initiated successfully!',
          transactionId: response.transaction.id
        }
      });
    } catch (err: any) {
      console.error('Transfer failed:', err);
      setError(err.response?.data?.message || 'Failed to initiate transfer. Please try again.');
      setShowPreview(false);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancelPreview = () => {
    setShowPreview(false);
  };

  if (!user?.walletAddress) {
    return (
      <div className="min-h-screen bg-gray-50">
        <header className="bg-white shadow">
          <div className="container mx-auto px-4 py-4">
            <Link to="/dashboard" className="text-indigo-600 hover:text-indigo-700 font-medium">
              ← Back to Dashboard
            </Link>
          </div>
        </header>

        <main className="container mx-auto px-4 py-8">
          <div className="max-w-md mx-auto text-center py-12">
            <svg className="w-16 h-16 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
            </svg>
            <h2 className="text-2xl font-bold text-gray-900 mb-2">No Wallet Found</h2>
            <p className="text-gray-600 mb-6">
              You need a wallet to transfer funds.
            </p>
            <Link
              to="/dashboard"
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700"
            >
              Go to Dashboard
            </Link>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow">
        <div className="container mx-auto px-4 py-4">
          <Link to="/dashboard" className="text-indigo-600 hover:text-indigo-700 font-medium">
            ← Back to Dashboard
          </Link>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8 max-w-2xl">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Send USDC</h1>
        <p className="text-gray-600 mb-8">Transfer USDC to any Ethereum address on Polygon Amoy</p>

        {/* Balance Card */}
        <div className="bg-white rounded-lg shadow-sm p-4 mb-6 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-500 mb-1">Available Balance</p>
              {isLoadingBalance ? (
                <div className="h-6 w-32 bg-gray-200 rounded animate-pulse"></div>
              ) : (
                <p className="text-2xl font-bold text-gray-900">
                  {balance.toFixed(6)} <span className="text-lg font-normal text-gray-500">USDC</span>
                </p>
              )}
            </div>
            <div className="text-right">
              <span className="inline-flex items-center gap-1 px-2 py-1 text-xs font-medium text-green-700 bg-green-100 rounded">
                <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                </svg>
                Gasless
              </span>
            </div>
          </div>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
            <div className="flex items-start gap-2">
              <svg className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <span className="text-sm text-red-800">{error}</span>
            </div>
          </div>
        )}

        {!showPreview ? (
          /* Transfer Form */
          <div className="bg-white rounded-lg shadow-sm p-6">
            <form onSubmit={(e) => { e.preventDefault(); handlePreview(); }} className="space-y-6">
              {/* Recipient Address */}
              <div>
                <label htmlFor="toAddress" className="block text-sm font-medium text-gray-700 mb-2">
                  Recipient Address
                </label>
                <input
                  type="text"
                  id="toAddress"
                  name="toAddress"
                  value={formData.toAddress}
                  onChange={handleChange}
                  placeholder="0x..."
                  className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-colors font-mono text-sm ${
                    errors.toAddress ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                />
                {errors.toAddress && (
                  <p className="mt-1 text-sm text-red-600">{errors.toAddress}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Enter a valid Ethereum address (42 characters starting with 0x)
                </p>
              </div>

              {/* Amount */}
              <div>
                <label htmlFor="amount" className="block text-sm font-medium text-gray-700 mb-2">
                  Amount (USDC)
                </label>
                <div className="relative">
                  <input
                    type="text"
                    id="amount"
                    name="amount"
                    value={formData.amount}
                    onChange={handleAmountChange}
                    placeholder="0.00"
                    className={`w-full px-4 py-3 pr-20 border rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-colors text-lg ${
                      errors.amount ? 'border-red-300 bg-red-50' : 'border-gray-300'
                    }`}
                  />
                  <button
                    type="button"
                    onClick={handleMaxClick}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 px-3 py-1 text-xs font-medium text-indigo-600 bg-indigo-50 rounded hover:bg-indigo-100 transition-colors"
                  >
                    MAX
                  </button>
                </div>
                {errors.amount && (
                  <p className="mt-1 text-sm text-red-600">{errors.amount}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Minimum: 0.000001 USDC • Maximum: 1,000,000 USDC
                </p>
              </div>

              {/* Description (Optional) */}
              <div>
                <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-2">
                  Note (Optional)
                </label>
                <textarea
                  id="description"
                  name="description"
                  value={formData.description}
                  onChange={handleChange}
                  rows={3}
                  placeholder="Add a note for this transfer..."
                  maxLength={500}
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-colors resize-none"
                />
                <p className="mt-1 text-xs text-gray-500 text-right">
                  {formData.description.length}/500
                </p>
              </div>

              {/* Gas Fee Info */}
              <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                <div className="flex items-start gap-3">
                  <svg className="w-5 h-5 text-green-600 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                  </svg>
                  <div>
                    <p className="text-sm font-medium text-green-900">Gasless Transaction</p>
                    <p className="text-xs text-green-700 mt-1">
                      No gas fees required! This transaction is sponsored by CoinPay's paymaster service.
                    </p>
                  </div>
                </div>
              </div>

              {/* Submit Button */}
              <button
                type="submit"
                disabled={isLoadingBalance}
                className="w-full bg-indigo-600 text-white py-3 px-4 rounded-lg font-medium hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Review Transfer
              </button>
            </form>
          </div>
        ) : (
          /* Transaction Preview */
          <div className="bg-white rounded-lg shadow-sm p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-6">Review Transaction</h2>

            <div className="space-y-4 mb-6">
              <div className="flex justify-between py-3 border-b border-gray-200">
                <span className="text-sm font-medium text-gray-500">From</span>
                <span className="text-sm font-mono text-gray-900">{user.walletAddress?.slice(0, 10)}...{user.walletAddress?.slice(-8)}</span>
              </div>

              <div className="flex justify-between py-3 border-b border-gray-200">
                <span className="text-sm font-medium text-gray-500">To</span>
                <span className="text-sm font-mono text-gray-900">{formData.toAddress.slice(0, 10)}...{formData.toAddress.slice(-8)}</span>
              </div>

              <div className="flex justify-between py-3 border-b border-gray-200">
                <span className="text-sm font-medium text-gray-500">Amount</span>
                <span className="text-lg font-bold text-gray-900">{parseFloat(formData.amount).toFixed(6)} USDC</span>
              </div>

              {formData.description && (
                <div className="py-3 border-b border-gray-200">
                  <p className="text-sm font-medium text-gray-500 mb-1">Note</p>
                  <p className="text-sm text-gray-700">{formData.description}</p>
                </div>
              )}

              <div className="flex justify-between py-3 border-b border-gray-200">
                <span className="text-sm font-medium text-gray-500">Network</span>
                <span className="text-sm text-gray-900">Polygon Amoy Testnet</span>
              </div>

              <div className="flex justify-between py-3">
                <span className="text-sm font-medium text-gray-500">Gas Fee</span>
                <span className="inline-flex items-center gap-1 text-sm font-medium text-green-600">
                  <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                  </svg>
                  FREE (Sponsored)
                </span>
              </div>
            </div>

            <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
              <div className="flex gap-2">
                <svg className="w-5 h-5 text-yellow-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                <div>
                  <p className="text-sm font-medium text-yellow-900">Confirm transaction details</p>
                  <p className="text-xs text-yellow-700 mt-1">
                    Please verify all information before confirming. Transactions cannot be reversed.
                  </p>
                </div>
              </div>
            </div>

            <div className="flex gap-3">
              <button
                type="button"
                onClick={handleCancelPreview}
                disabled={isSubmitting}
                className="flex-1 bg-white text-gray-700 py-3 px-4 rounded-lg font-medium border border-gray-300 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Back
              </button>
              <button
                type="button"
                onClick={handleSubmit}
                disabled={isSubmitting}
                className="flex-1 bg-indigo-600 text-white py-3 px-4 rounded-lg font-medium hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
              >
                {isSubmitting ? (
                  <>
                    <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Processing...
                  </>
                ) : (
                  'Confirm & Send'
                )}
              </button>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
