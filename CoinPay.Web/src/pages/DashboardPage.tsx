import { useAuthStore } from '@/store';
import { Link } from 'react-router-dom';

export function DashboardPage() {
  const { user, logout } = useAuthStore();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold text-gray-900">CoinPay Dashboard</h1>
          <div className="flex items-center gap-4">
            <span className="text-gray-600">Welcome, {user?.username}</span>
            <button
              onClick={logout}
              className="px-4 py-2 bg-danger-500 text-white rounded-md hover:bg-danger-600 transition-colors"
            >
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
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
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Account Information</h2>
          <dl className="space-y-2">
            <div className="flex">
              <dt className="font-medium text-gray-700 w-32">Username:</dt>
              <dd className="text-gray-900">{user?.username}</dd>
            </div>
            <div className="flex">
              <dt className="font-medium text-gray-700 w-32">Wallet Address:</dt>
              <dd className="text-gray-900 font-mono text-sm">
                {user?.walletAddress || 'Not created yet'}
              </dd>
            </div>
          </dl>
        </div>
      </main>
    </div>
  );
}
