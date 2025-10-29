import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store';
import { WalletHeader } from '@/components/wallet/WalletHeader';
import { BalanceCard } from '@/components/wallet/BalanceCard';
import { QuickActions } from '@/components/wallet/QuickActions';
import { RecentTransactions } from '@/components/wallet/RecentTransactions';
import { QRCodeModal } from '@/components/wallet/QRCodeModal';
import { walletService } from '@/services/walletService';
import { transactionService } from '@/services/transactionService';
import type { Transaction } from '@/types/transaction';

export function WalletPage() {
  const { user } = useAuthStore();
  const [balance, setBalance] = useState(0);
  const [isLoadingBalance, setIsLoadingBalance] = useState(true);
  const [isLoadingTransactions, setIsLoadingTransactions] = useState(true);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [lastUpdated, setLastUpdated] = useState<Date | undefined>(undefined);
  const [isQRModalOpen, setIsQRModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch balance
  const fetchBalance = async () => {
    if (!user?.walletAddress) return;

    try {
      setIsLoadingBalance(true);
      setError(null);
      const response = await walletService.getBalance(user.walletAddress);
      setBalance(response.balance || 0);
      setLastUpdated(new Date());
    } catch (err) {
      console.error('Failed to fetch balance:', err);
      setError('Failed to load balance');
      setBalance(0);
    } finally {
      setIsLoadingBalance(false);
    }
  };

  // Fetch recent transactions
  const fetchTransactions = async () => {
    if (!user?.id) return;

    try {
      setIsLoadingTransactions(true);
      const response = await transactionService.getUserTransactions(user.id);
      setTransactions(response || []);
    } catch (err) {
      console.error('Failed to fetch transactions:', err);
      setTransactions([]);
    } finally {
      setIsLoadingTransactions(false);
    }
  };

  // Initial data fetch
  useEffect(() => {
    fetchBalance();
    fetchTransactions();
  }, [user?.walletAddress, user?.id]);

  // Auto-refresh balance every 30 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      if (!isLoadingBalance) {
        fetchBalance();
      }
    }, 30000); // 30 seconds

    return () => clearInterval(interval);
  }, [user?.walletAddress, isLoadingBalance]);

  const handleRefresh = async () => {
    await Promise.all([fetchBalance(), fetchTransactions()]);
  };

  const handleReceiveClick = () => {
    setIsQRModalOpen(true);
  };

  const handleCloseQRModal = () => {
    setIsQRModalOpen(false);
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
              You need to create a wallet to use CoinPay features.
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

      <main className="container mx-auto px-4 py-8 max-w-4xl">
        {error && (
          <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
            <div className="flex items-center gap-2">
              <svg className="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <span className="text-sm text-red-800">{error}</span>
              <button
                onClick={handleRefresh}
                className="ml-auto text-sm text-red-600 hover:text-red-700 font-medium"
              >
                Retry
              </button>
            </div>
          </div>
        )}

        <WalletHeader address={user.walletAddress} username={user.username} />

        <BalanceCard
          balance={balance}
          currency="USDC"
          isLoading={isLoadingBalance}
          onRefresh={fetchBalance}
          lastUpdated={lastUpdated}
        />

        <QuickActions
          onReceiveClick={handleReceiveClick}
          walletAddress={user.walletAddress}
        />

        <RecentTransactions
          transactions={transactions}
          isLoading={isLoadingTransactions}
        />

        <QRCodeModal
          isOpen={isQRModalOpen}
          onClose={handleCloseQRModal}
          address={user.walletAddress}
        />
      </main>
    </div>
  );
}
