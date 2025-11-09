import { useAuthStore } from '@/store';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import { OnboardingWizard, useOnboarding } from '@/components/onboarding/OnboardingWizard';

export function DashboardPage() {
  const { user, logout } = useAuthStore();
  const [copied, setCopied] = useState(false);
  const { shouldShowOnboarding, markAsCompleted } = useOnboarding();
  const [isOnboardingOpen, setIsOnboardingOpen] = useState(false);

  const handleCopyAddress = async () => {
    if (!user?.walletAddress) return;
    try {
      await navigator.clipboard.writeText(user.walletAddress);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error('Failed to copy address:', err);
    }
  };

  const handleOnboardingComplete = () => {
    markAsCompleted();
    setIsOnboardingOpen(false);
  };

  const handleShowOnboarding = () => {
    setIsOnboardingOpen(true);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold text-gray-900">CoinPay Dashboard</h1>
          <div className="flex items-center gap-4">
            <span className="text-gray-600">Welcome, {user?.username}</span>
            <button
              onClick={handleShowOnboarding}
              className="px-4 py-2 bg-primary-500 text-white rounded-md hover:bg-primary-600 transition-colors"
              aria-label="Show onboarding tutorial"
            >
              Tutorial
            </button>
            <button
              onClick={logout}
              className="px-4 py-2 bg-danger-500 text-white rounded-md hover:bg-danger-600 transition-colors"
            >
              Logout
            </button>
          </div>
        </div>
      </header>

      <main id="main-content" className="container mx-auto px-4 py-8">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <Link
            to="/wallet"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Wallet</h3>
            <p className="text-gray-600">View your wallet balance and details</p>
          </Link>

          <Link
            to="/transfer"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Transfer</h3>
            <p className="text-gray-600">Send cryptocurrency to other users</p>
          </Link>

          <Link
            to="/transactions"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Transactions</h3>
            <p className="text-gray-600">View your transaction history</p>
          </Link>

          <Link
            to="/investment"
            className="bg-gradient-to-br from-blue-600 to-purple-600 p-6 rounded-lg shadow hover:shadow-lg transition-shadow"
          >
            <h3 className="text-lg font-semibold text-white mb-2">Investment</h3>
            <p className="text-blue-100">Earn rewards on your crypto holdings</p>
          </Link>

          <Link
            to="/swap"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Swap</h3>
            <p className="text-gray-600">Exchange tokens at competitive rates</p>
          </Link>

          <Link
            to="/withdraw"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Withdraw</h3>
            <p className="text-gray-600">Cash out to your bank account</p>
          </Link>

          <Link
            to="/bank-accounts"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Bank Accounts</h3>
            <p className="text-gray-600">Manage your linked bank accounts</p>
          </Link>

          <Link
            to="/docs"
            className="bg-gradient-to-br from-emerald-600 to-teal-600 p-6 rounded-lg shadow hover:shadow-lg transition-shadow"
          >
            <h3 className="text-lg font-semibold text-white mb-2">Development</h3>
            <p className="text-emerald-100">API docs and integration guides for developers</p>
          </Link>

          <Link
            to="/help"
            className="bg-white p-6 rounded-lg shadow hover:shadow-md transition-shadow"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Help Center</h3>
            <p className="text-gray-600">Get answers and learn how to use CoinPay</p>
          </Link>
        </div>

        {/* Wallet Address Card - Prominent */}
        {user?.walletAddress && (
          <div className="bg-gradient-to-r from-indigo-600 to-purple-600 rounded-lg shadow-lg p-6 mb-6 text-white">
            <div className="mb-3">
              <h2 className="text-xl font-bold mb-1">Your Wallet Address</h2>
              <p className="text-indigo-100 text-sm">Use this address to receive testnet USDC</p>
            </div>

            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-4 border border-white/20">
              <div className="flex items-center justify-between mb-2">
                <span className="text-xs font-semibold uppercase tracking-wider text-indigo-100">
                  Address
                </span>
                <button
                  onClick={handleCopyAddress}
                  className="inline-flex items-center gap-2 px-3 py-1.5 text-sm font-medium bg-white/20 hover:bg-white/30 rounded-md transition-colors border border-white/30"
                >
                  {copied ? (
                    <>
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                      </svg>
                      <span>Copied!</span>
                    </>
                  ) : (
                    <>
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                      </svg>
                      <span>Copy Address</span>
                    </>
                  )}
                </button>
              </div>

              <div className="bg-gray-900/50 rounded px-3 py-2 font-mono text-sm break-all select-all mb-3">
                {user.walletAddress}
              </div>

              <div className="flex items-start gap-2 text-xs text-indigo-100">
                <svg className="w-4 h-4 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <div>
                  <p className="font-medium mb-1">Get testnet USDC from these faucets:</p>
                  <ul className="space-y-0.5 ml-0">
                    <li>• Polygon Amoy MATIC: <a href="https://faucet.polygon.technology/" target="_blank" rel="noopener noreferrer" className="underline hover:text-white">faucet.polygon.technology</a></li>
                    <li>• Circle USDC: <a href="https://faucet.circle.com/" target="_blank" rel="noopener noreferrer" className="underline hover:text-white">faucet.circle.com</a></li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        )}

        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Account Information</h2>
          <dl className="space-y-2">
            <div className="flex">
              <dt className="font-medium text-gray-700 w-32">Username:</dt>
              <dd className="text-gray-900">{user?.username}</dd>
            </div>
            <div className="flex">
              <dt className="font-medium text-gray-700 w-32">User ID:</dt>
              <dd className="text-gray-900">{user?.id}</dd>
            </div>
          </dl>
        </div>
      </main>

      <OnboardingWizard
        isOpen={shouldShowOnboarding || isOnboardingOpen}
        onClose={() => setIsOnboardingOpen(false)}
        onComplete={handleOnboardingComplete}
      />
    </div>
  );
}
