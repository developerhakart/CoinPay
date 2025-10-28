import { useAuthStore } from '@/store';
import { Link } from 'react-router-dom';

export function WalletPage() {
  const { user } = useAuthStore();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow">
        <div className="container mx-auto px-4 py-4">
          <Link to="/dashboard" className="text-primary-500 hover:text-primary-600">
            ← Back to Dashboard
          </Link>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-8">My Wallet</h1>

        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Wallet Details</h2>
          <dl className="space-y-3">
            <div>
              <dt className="text-sm font-medium text-gray-500">Wallet Address</dt>
              <dd className="mt-1 text-sm text-gray-900 font-mono break-all">
                {user?.walletAddress || 'No wallet created'}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Balance</dt>
              <dd className="mt-1 text-2xl font-bold text-gray-900">
                0.00 USDC
              </dd>
            </div>
          </dl>
        </div>

        <div className="bg-primary-50 border border-primary-200 rounded-lg p-4">
          <p className="text-primary-800">
            Wallet functionality will be implemented in the next sprint.
          </p>
        </div>
      </main>
    </div>
  );
}
