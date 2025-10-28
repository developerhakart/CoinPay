import { useState, useRef } from 'react';
import { useAuthStore } from '@/store';
import { TransactionList } from './TransactionList';
import { TransactionForm } from './TransactionForm';

export function Dashboard() {
  const { user, logout } = useAuthStore();
  const [refreshKey, setRefreshKey] = useState(0);
  const listRef = useRef<HTMLDivElement>(null);

  const handleTransactionCreated = () => {
    setRefreshKey((prev) => prev + 1);
    setTimeout(() => {
      listRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 100);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-500 to-purple-600 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        {/* Header with User Info */}
        <div className="bg-white/90 backdrop-blur rounded-lg shadow-lg p-6 mb-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-800 mb-1">
                Welcome, {user?.username}!
              </h1>
              <p className="text-gray-600 text-sm">
                {user?.walletAddress ? (
                  <>
                    Wallet: <span className="font-mono text-xs">{user.walletAddress}</span>
                  </>
                ) : (
                  'No wallet connected'
                )}
              </p>
            </div>
            <button
              onClick={logout}
              className="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors font-medium"
            >
              Sign Out
            </button>
          </div>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
          <div className="bg-white/90 backdrop-blur rounded-lg p-4 shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-500 text-sm">Total Transactions</p>
                <p className="text-2xl font-bold text-gray-800">
                  {refreshKey >= 0 ? '—' : '0'}
                </p>
              </div>
              <div className="text-4xl">💰</div>
            </div>
          </div>
          <div className="bg-white/90 backdrop-blur rounded-lg p-4 shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-500 text-sm">Active</p>
                <p className="text-2xl font-bold text-yellow-600">—</p>
              </div>
              <div className="text-4xl">⏳</div>
            </div>
          </div>
          <div className="bg-white/90 backdrop-blur rounded-lg p-4 shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-500 text-sm">Completed</p>
                <p className="text-2xl font-bold text-green-600">—</p>
              </div>
              <div className="text-4xl">✅</div>
            </div>
          </div>
        </div>

        {/* Transaction Form */}
        <div className="mb-6">
          <TransactionForm onSuccess={handleTransactionCreated} />
        </div>

        {/* Transaction List */}
        <div ref={listRef} className="bg-white/95 backdrop-blur rounded-lg shadow-xl p-6">
          <h2 className="text-2xl font-bold text-gray-800 mb-4">
            Recent Transactions
          </h2>
          <TransactionList key={refreshKey} />
        </div>

        {/* Footer */}
        <div className="text-center mt-8 text-white/80 text-sm">
          <p>Powered by CoinPay API • Secure Cryptocurrency Payments</p>
        </div>
      </div>
    </div>
  );
}
