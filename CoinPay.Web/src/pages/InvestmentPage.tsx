import { Link } from 'react-router-dom';
import { InvestmentDashboard } from '@/components/Investment';

export function InvestmentPage() {
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
        <div className="mb-6">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">Exchange Investment</h1>
          <p className="text-gray-600">
            Connect your WhiteBit account and earn rewards on your cryptocurrency holdings.
          </p>
        </div>

        <InvestmentDashboard />
      </main>
    </div>
  );
}
