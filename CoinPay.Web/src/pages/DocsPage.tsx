import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store';

export function DocsPage() {
  const { logout } = useAuthStore();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <div className="flex items-center gap-4">
            <Link to="/dashboard" className="text-primary-500 hover:text-primary-600">
              ← Back to Dashboard
            </Link>
            <h1 className="text-2xl font-bold text-gray-900">Developer Documentation</h1>
          </div>
          <button
            onClick={logout}
            className="px-4 py-2 bg-danger-500 text-white rounded-md hover:bg-danger-600 transition-colors"
          >
            Logout
          </button>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        {/* Hero Section */}
        <div className="bg-gradient-to-r from-emerald-600 to-teal-600 rounded-lg shadow-lg p-8 mb-8 text-white">
          <h2 className="text-3xl font-bold mb-3">CoinPay API Documentation</h2>
          <p className="text-emerald-100 text-lg">
            Comprehensive guides and references for integrating CoinPay into your B2B applications
          </p>
        </div>

        {/* Quick Links Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
          <a
            href="/api/swagger"
            target="_blank"
            rel="noopener noreferrer"
            className="bg-white p-6 rounded-lg shadow hover:shadow-lg transition-shadow border-l-4 border-blue-500"
          >
            <div className="flex items-center gap-3 mb-3">
              <svg className="w-8 h-8 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 9l3 3-3 3m5 0h3M5 20h14a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              <h3 className="text-xl font-semibold text-gray-900">API Reference</h3>
            </div>
            <p className="text-gray-600 mb-2">Interactive Swagger/OpenAPI documentation</p>
            <span className="text-blue-500 font-medium text-sm">Open Swagger UI →</span>
          </a>

          <div className="bg-white p-6 rounded-lg shadow hover:shadow-lg transition-shadow border-l-4 border-emerald-500 cursor-pointer">
            <div className="flex items-center gap-3 mb-3">
              <svg className="w-8 h-8 text-emerald-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              <h3 className="text-xl font-semibold text-gray-900">Getting Started</h3>
            </div>
            <p className="text-gray-600 mb-2">Quick start guide for B2B integration</p>
            <span className="text-emerald-500 font-medium text-sm">See below ↓</span>
          </div>

          <a
            href="https://github.com/developerhakart/CoinPay"
            target="_blank"
            rel="noopener noreferrer"
            className="bg-white p-6 rounded-lg shadow hover:shadow-lg transition-shadow border-l-4 border-gray-700"
          >
            <div className="flex items-center gap-3 mb-3">
              <svg className="w-8 h-8 text-gray-700" fill="currentColor" viewBox="0 0 24 24">
                <path fillRule="evenodd" d="M12 2C6.477 2 2 6.477 2 12c0 4.42 2.865 8.17 6.839 9.49.5.092.682-.217.682-.482 0-.237-.008-.866-.013-1.7-2.782.603-3.369-1.34-3.369-1.34-.454-1.156-1.11-1.463-1.11-1.463-.908-.62.069-.608.069-.608 1.003.07 1.531 1.03 1.531 1.03.892 1.529 2.341 1.087 2.91.831.092-.646.35-1.086.636-1.336-2.22-.253-4.555-1.11-4.555-4.943 0-1.091.39-1.984 1.029-2.683-.103-.253-.446-1.27.098-2.647 0 0 .84-.269 2.75 1.025A9.578 9.578 0 0112 6.836c.85.004 1.705.114 2.504.336 1.909-1.294 2.747-1.025 2.747-1.025.546 1.377.203 2.394.1 2.647.64.699 1.028 1.592 1.028 2.683 0 3.842-2.339 4.687-4.566 4.935.359.309.678.919.678 1.852 0 1.336-.012 2.415-.012 2.743 0 .267.18.578.688.48C19.138 20.167 22 16.418 22 12c0-5.523-4.477-10-10-10z" clipRule="evenodd" />
              </svg>
              <h3 className="text-xl font-semibold text-gray-900">GitHub Repository</h3>
            </div>
            <p className="text-gray-600 mb-2">Source code and examples</p>
            <span className="text-gray-700 font-medium text-sm">View on GitHub →</span>
          </a>
        </div>

        {/* Documentation Sections */}
        <div className="space-y-8">
          {/* Getting Started */}
          <section className="bg-white rounded-lg shadow p-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-4 flex items-center gap-3">
              <span className="bg-emerald-100 text-emerald-600 rounded-lg p-2">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
              </span>
              Getting Started with CoinPay API
            </h2>

            <div className="prose max-w-none">
              <h3 className="text-xl font-semibold text-gray-800 mt-6 mb-3">1. Authentication</h3>
              <p className="text-gray-600 mb-4">
                CoinPay uses JWT (JSON Web Token) authentication. Obtain your API credentials from your account settings.
              </p>

              <div className="bg-gray-900 rounded-lg p-4 mb-4 overflow-x-auto">
                <code className="text-sm text-green-400">
                  POST /api/auth/login<br />
                  Content-Type: application/json<br />
                  <br />
                  {'{'}<br />
                  &nbsp;&nbsp;"username": "your-username",<br />
                  &nbsp;&nbsp;"password": "your-password"<br />
                  {'}'}
                </code>
              </div>

              <h3 className="text-xl font-semibold text-gray-800 mt-6 mb-3">2. Base URL</h3>
              <div className="bg-blue-50 border-l-4 border-blue-500 p-4 mb-4">
                <p className="text-blue-900 font-mono">
                  <strong>Production:</strong> https://api.coinpay.com<br />
                  <strong>Testnet:</strong> http://localhost:7777 (Development)
                </p>
              </div>

              <h3 className="text-xl font-semibold text-gray-800 mt-6 mb-3">3. Making Your First Request</h3>
              <div className="bg-gray-900 rounded-lg p-4 mb-4 overflow-x-auto">
                <code className="text-sm text-green-400">
                  GET /api/wallet<br />
                  Authorization: Bearer YOUR_JWT_TOKEN<br />
                  Content-Type: application/json
                </code>
              </div>
            </div>
          </section>

          {/* Core Features */}
          <section className="bg-white rounded-lg shadow p-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-6">Core API Features</h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <a
                href="https://github.com/developerhakart/CoinPay/blob/development/Documentations/API/AUTHENTICATION.md"
                target="_blank"
                rel="noopener noreferrer"
                className="border-l-4 border-gray-700 pl-4 hover:bg-gray-50 p-4 rounded-r-lg transition-colors"
              >
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Authentication</h3>
                  <span className="text-xs text-gray-500">→</span>
                </div>
                <p className="text-gray-600 mb-2">JWT-based authentication system</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• POST /api/auth/login - Obtain JWT token</li>
                  <li>• POST /api/auth/register - Create account</li>
                  <li>• POST /api/auth/refresh - Refresh token</li>
                </ul>
              </a>

              <a
                href="https://github.com/developerhakart/CoinPay/blob/development/Documentations/API/WALLET_API.md"
                target="_blank"
                rel="noopener noreferrer"
                className="border-l-4 border-blue-500 pl-4 hover:bg-gray-50 p-4 rounded-r-lg transition-colors"
              >
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Wallet Management</h3>
                  <span className="text-xs text-gray-500">→</span>
                </div>
                <p className="text-gray-600 mb-2">Create and manage crypto wallets</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• GET /api/wallet - Get wallet details</li>
                  <li>• GET /api/wallet/balance - Check balance</li>
                  <li>• GET /api/wallet/transactions - Transaction history</li>
                </ul>
              </a>

              <a
                href="https://github.com/developerhakart/CoinPay/blob/development/Documentations/API/TRANSACTION_API.md"
                target="_blank"
                rel="noopener noreferrer"
                className="border-l-4 border-emerald-500 pl-4 hover:bg-gray-50 p-4 rounded-r-lg transition-colors"
              >
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Transactions</h3>
                  <span className="text-xs text-gray-500">→</span>
                </div>
                <p className="text-gray-600 mb-2">Send and receive cryptocurrency payments</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• POST /api/transactions - Create transaction</li>
                  <li>• GET /api/transactions - List transactions</li>
                  <li>• GET /api/transactions/{'{id}'} - Get details</li>
                </ul>
              </a>

              <a
                href="https://github.com/developerhakart/CoinPay/blob/development/Documentations/API/SWAP_API.md"
                target="_blank"
                rel="noopener noreferrer"
                className="border-l-4 border-purple-500 pl-4 hover:bg-gray-50 p-4 rounded-r-lg transition-colors"
              >
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Token Swaps</h3>
                  <span className="text-xs text-gray-500">→</span>
                </div>
                <p className="text-gray-600 mb-2">Exchange tokens via 1inch DEX aggregator</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• GET /api/swap/quote - Get swap quote</li>
                  <li>• POST /api/swap/execute - Execute swap</li>
                  <li>• GET /api/swap/history - Swap history</li>
                </ul>
              </a>

              <div className="border-l-4 border-orange-500 pl-4 p-4">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Withdrawals (Payouts)</h3>
                  <span className="text-xs text-gray-400">Coming soon</span>
                </div>
                <p className="text-gray-600 mb-2">Fiat off-ramp to bank accounts</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• POST /api/payouts - Create payout</li>
                  <li>• GET /api/payouts/{'{id}'} - Payout status</li>
                  <li>• GET /api/payouts - Payout history</li>
                </ul>
              </div>

              <div className="border-l-4 border-indigo-500 pl-4 p-4">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Investments</h3>
                  <span className="text-xs text-gray-400">Coming soon</span>
                </div>
                <p className="text-gray-600 mb-2">Exchange staking and yield farming</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• GET /api/investment/plans - Available plans</li>
                  <li>• POST /api/investment/positions - Create position</li>
                  <li>• GET /api/investment/positions - Your positions</li>
                </ul>
              </div>

              <div className="border-l-4 border-pink-500 pl-4 p-4">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">Bank Accounts</h3>
                  <span className="text-xs text-gray-400">Coming soon</span>
                </div>
                <p className="text-gray-600 mb-2">Link and manage bank accounts</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• POST /api/bank-accounts - Add account</li>
                  <li>• GET /api/bank-accounts - List accounts</li>
                  <li>• DELETE /api/bank-accounts/{'{id}'} - Remove</li>
                </ul>
              </div>

              <a
                href="https://github.com/developerhakart/CoinPay/blob/development/Documentations/API/guides/B2B_INTEGRATION_GUIDE.md"
                target="_blank"
                rel="noopener noreferrer"
                className="border-l-4 border-teal-500 pl-4 hover:bg-gray-50 p-4 rounded-r-lg transition-colors"
              >
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-semibold text-gray-900">B2B Integration Guide</h3>
                  <span className="text-xs text-gray-500">→</span>
                </div>
                <p className="text-gray-600 mb-2">Complete integration guide for businesses</p>
                <ul className="text-sm text-gray-600 space-y-1">
                  <li>• Architecture diagrams</li>
                  <li>• Setup instructions</li>
                  <li>• Production checklist</li>
                </ul>
              </a>
            </div>
          </section>

          {/* Webhooks */}
          <section className="bg-white rounded-lg shadow p-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-4 flex items-center gap-3">
              <span className="bg-purple-100 text-purple-600 rounded-lg p-2">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                </svg>
              </span>
              Webhooks & Events
            </h2>

            <p className="text-gray-600 mb-4">
              Subscribe to real-time events for transaction updates, wallet changes, and more.
            </p>

            <div className="bg-gray-50 rounded-lg p-6">
              <h3 className="font-semibold text-gray-900 mb-3">Available Events:</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="flex items-start gap-2">
                  <span className="text-emerald-500 mt-1">✓</span>
                  <div>
                    <p className="font-medium text-gray-900">transaction.created</p>
                    <p className="text-sm text-gray-600">New transaction initiated</p>
                  </div>
                </div>
                <div className="flex items-start gap-2">
                  <span className="text-emerald-500 mt-1">✓</span>
                  <div>
                    <p className="font-medium text-gray-900">transaction.completed</p>
                    <p className="text-sm text-gray-600">Transaction confirmed on blockchain</p>
                  </div>
                </div>
                <div className="flex items-start gap-2">
                  <span className="text-emerald-500 mt-1">✓</span>
                  <div>
                    <p className="font-medium text-gray-900">payout.processed</p>
                    <p className="text-sm text-gray-600">Fiat withdrawal completed</p>
                  </div>
                </div>
                <div className="flex items-start gap-2">
                  <span className="text-emerald-500 mt-1">✓</span>
                  <div>
                    <p className="font-medium text-gray-900">wallet.updated</p>
                    <p className="text-sm text-gray-600">Wallet balance changed</p>
                  </div>
                </div>
              </div>
            </div>
          </section>

          {/* Code Examples */}
          <section className="bg-white rounded-lg shadow p-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Code Examples</h2>

            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold text-gray-800 mb-2">Create Transaction (Node.js)</h3>
                <div className="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre className="text-sm text-green-400">
{`const axios = require('axios');

const createTransaction = async () => {
  const response = await axios.post(
    'http://localhost:7777/api/transactions',
    {
      amount: 100.50,
      currency: 'USDC',
      type: 'Transfer',
      receiverName: '0x1234...abcd',
      description: 'Payment for services'
    },
    {
      headers: {
        'Authorization': 'Bearer YOUR_JWT_TOKEN',
        'Content-Type': 'application/json'
      }
    }
  );

  console.log('Transaction created:', response.data);
};`}
                  </pre>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold text-gray-800 mb-2">Get Swap Quote (Python)</h3>
                <div className="bg-gray-900 rounded-lg p-4 overflow-x-auto">
                  <pre className="text-sm text-green-400">
{`import requests

def get_swap_quote():
    url = "http://localhost:7777/api/swap/quote"
    params = {
        "fromToken": "USDC",
        "toToken": "WETH",
        "amount": "100"
    }
    headers = {
        "Authorization": "Bearer YOUR_JWT_TOKEN"
    }

    response = requests.get(url, params=params, headers=headers)
    quote = response.json()

    print(f"Expected output: {quote['toAmount']} WETH")
    return quote`}
                  </pre>
                </div>
              </div>
            </div>
          </section>

          {/* Support & Resources */}
          <section className="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg border border-blue-200 p-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Support & Resources</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="bg-white rounded-lg p-4">
                <h3 className="font-semibold text-gray-900 mb-2">📧 Email Support</h3>
                <p className="text-gray-600 text-sm mb-2">Get help from our team</p>
                <a href="mailto:dev@coinpay.com" className="text-blue-600 hover:text-blue-700 text-sm font-medium">
                  dev@coinpay.com
                </a>
              </div>
              <div className="bg-white rounded-lg p-4">
                <h3 className="font-semibold text-gray-900 mb-2">💬 Discord Community</h3>
                <p className="text-gray-600 text-sm mb-2">Join other developers</p>
                <a href="#" className="text-blue-600 hover:text-blue-700 text-sm font-medium">
                  Join Discord
                </a>
              </div>
              <div className="bg-white rounded-lg p-4">
                <h3 className="font-semibold text-gray-900 mb-2">📚 Help Center</h3>
                <p className="text-gray-600 text-sm mb-2">Browse knowledge base</p>
                <Link to="/help" className="text-blue-600 hover:text-blue-700 text-sm font-medium">
                  Visit Help Center
                </Link>
              </div>
            </div>
          </section>
        </div>
      </main>

      {/* Footer */}
      <footer className="bg-white border-t border-gray-200 mt-12">
        <div className="container mx-auto px-4 py-6">
          <div className="flex justify-between items-center">
            <p className="text-gray-600 text-sm">
              © 2025 CoinPay. All rights reserved.
            </p>
            <div className="flex gap-6">
              <a href="/api/swagger" target="_blank" className="text-gray-600 hover:text-gray-900 text-sm">
                API Reference
              </a>
              <a href="https://github.com/developerhakart/CoinPay" target="_blank" className="text-gray-600 hover:text-gray-900 text-sm">
                GitHub
              </a>
              <Link to="/help" className="text-gray-600 hover:text-gray-900 text-sm">
                Help Center
              </Link>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}
