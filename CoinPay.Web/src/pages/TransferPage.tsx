import { Link } from 'react-router-dom';

export function TransferPage() {
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
        <h1 className="text-3xl font-bold text-gray-900 mb-8">Transfer Funds</h1>

        <div className="max-w-2xl mx-auto bg-white rounded-lg shadow p-6">
          <div className="bg-primary-50 border border-primary-200 rounded-lg p-4">
            <p className="text-primary-800">
              Transfer functionality will be implemented in the next sprint.
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
