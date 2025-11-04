import { createBrowserRouter } from 'react-router-dom';
import { ProtectedRoute } from '@/components/common/ProtectedRoute';
import { HomePage } from '@/pages/HomePage';
import { LoginPage } from '@/pages/LoginPage';
import { RegisterPage } from '@/pages/RegisterPage';
import { DashboardPage } from '@/pages/DashboardPage';
import { WalletPage } from '@/pages/WalletPage';
import { TransferPage } from '@/pages/TransferPage';
import { TransactionsPage } from '@/pages/TransactionsPage';
// Phase 3: Bank Account Management
import { BankAccountsPage } from '@/pages/BankAccountsPage';
// Phase 3: Fiat Off-Ramp
import { WithdrawPage } from '@/pages/WithdrawPage';
import { PayoutHistoryPage } from '@/pages/PayoutHistoryPage';
import { PayoutStatusPage } from '@/pages/PayoutStatusPage';
// Phase 4: Exchange Investment
import { InvestmentPage } from '@/pages/InvestmentPage';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <HomePage />,
  },
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/register',
    element: <RegisterPage />,
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        path: '/dashboard',
        element: <DashboardPage />,
      },
      {
        path: '/wallet',
        element: <WalletPage />,
      },
      {
        path: '/transfer',
        element: <TransferPage />,
      },
      {
        path: '/transactions',
        element: <TransactionsPage />,
      },
      // Phase 3: Bank Account Management Routes
      {
        path: '/bank-accounts',
        element: <BankAccountsPage />,
      },
      // Phase 3: Fiat Off-Ramp Routes
      {
        path: '/withdraw',
        element: <WithdrawPage />,
      },
      {
        path: '/payout/history',
        element: <PayoutHistoryPage />,
      },
      {
        path: '/payout/:id/status',
        element: <PayoutStatusPage />,
      },
      // Phase 4: Exchange Investment Routes
      {
        path: '/investment',
        element: <InvestmentPage />,
      },
    ],
  },
  {
    path: '*',
    element: (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">404 - Page Not Found</h1>
          <p className="text-gray-600 mb-4">The page you're looking for doesn't exist.</p>
          <a href="/" className="text-primary-500 hover:text-primary-600 font-semibold">
            Go back home
          </a>
        </div>
      </div>
    ),
  },
]);
