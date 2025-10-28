import { useAuthStore } from '@/store';
import { Link } from 'react-router-dom';
import { TransactionList } from '@/components/TransactionList';

export function TransactionsPage() {
  const { user, logout } = useAuthStore();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <div className="flex items-center gap-4">
            <Link
              to="/dashboard"
              className="text-gray-600 hover:text-gray-900 transition-colors"
            >
              ← Back to Dashboard
            </Link>
            <h1 className="text-2xl font-bold text-gray-900">Transactions</h1>
          </div>
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
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-6">Transaction History</h2>
          <TransactionList />
        </div>
      </main>
    </div>
  );
}
