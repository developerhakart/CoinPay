import { useState, useRef } from 'react';
import { TransactionList } from './components/TransactionList';
import { TransactionForm } from './components/TransactionForm';

function App() {
  const [refreshKey, setRefreshKey] = useState(0);
  const listRef = useRef<HTMLDivElement>(null);

  const handleTransactionCreated = () => {
    // Trigger refresh of transaction list
    setRefreshKey((prev) => prev + 1);

    // Scroll to top of the list to see new transaction
    setTimeout(() => {
      listRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 100);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-500 to-purple-600 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="text-center mb-8">
          <h1 className="text-5xl font-bold text-white mb-2 drop-shadow-lg">
            CoinPay
          </h1>
          <p className="text-blue-100 text-lg">
            Cryptocurrency Payment Platform
          </p>
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

export default App;
