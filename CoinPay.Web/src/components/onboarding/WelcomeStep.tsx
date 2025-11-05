/**
 * WelcomeStep Component (Step 1 of Onboarding)
 *
 * Displays the welcome message and feature highlights for new users.
 * Shows CoinPay value proposition with Send, Swap, and Invest features.
 */

export function WelcomeStep() {
  return (
    <div className="text-center">
      {/* Icon */}
      <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gradient-to-br from-primary-500 to-primary-600 mb-6">
        <svg
          className="w-10 h-10 text-white"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          aria-hidden="true"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
      </div>

      {/* Welcome Message */}
      <h2 className="text-3xl font-bold text-gray-900 mb-4">
        Welcome to CoinPay
      </h2>
      <p className="text-lg text-gray-600 mb-8 max-w-lg mx-auto">
        Your secure gateway to cryptocurrency payments and management
      </p>

      {/* Feature Highlights Grid */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 text-left">
        {/* Send Feature */}
        <div className="p-5 bg-primary-50 rounded-lg border border-primary-100">
          <div className="w-10 h-10 bg-primary-500 rounded-lg flex items-center justify-center mb-3">
            <svg
              className="w-6 h-6 text-white"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M13 10V3L4 14h7v7l9-11h-7z"
              />
            </svg>
          </div>
          <h3 className="font-semibold text-gray-900 mb-1">Fast Transactions</h3>
          <p className="text-sm text-gray-600">
            Send and receive crypto instantly with low fees
          </p>
        </div>

        {/* Swap Feature */}
        <div className="p-5 bg-secondary-50 rounded-lg border border-secondary-100">
          <div className="w-10 h-10 bg-secondary-500 rounded-lg flex items-center justify-center mb-3">
            <svg
              className="w-6 h-6 text-white"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"
              />
            </svg>
          </div>
          <h3 className="font-semibold text-gray-900 mb-1">Token Swaps</h3>
          <p className="text-sm text-gray-600">
            Exchange tokens seamlessly at competitive rates
          </p>
        </div>

        {/* Investment Feature */}
        <div className="p-5 bg-accent-50 rounded-lg border border-accent-100">
          <div className="w-10 h-10 bg-accent-500 rounded-lg flex items-center justify-center mb-3">
            <svg
              className="w-6 h-6 text-white"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"
              />
            </svg>
          </div>
          <h3 className="font-semibold text-gray-900 mb-1">Invest & Grow</h3>
          <p className="text-sm text-gray-600">
            Earn returns through automated investment strategies
          </p>
        </div>
      </div>
    </div>
  );
}
