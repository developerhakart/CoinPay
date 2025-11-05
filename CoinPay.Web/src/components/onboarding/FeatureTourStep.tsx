/**
 * FeatureTourStep Component (Step 3 of Onboarding)
 *
 * Guides users through the key features of CoinPay:
 * Send & Receive, Swap Tokens, and Investment Strategies.
 */

export function FeatureTourStep() {
  return (
    <div>
      {/* Header */}
      <div className="text-center mb-8">
        <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gradient-to-br from-accent-500 to-accent-600 mb-6">
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
              d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z"
            />
          </svg>
        </div>
        <h2 className="text-3xl font-bold text-gray-900 mb-4">
          Explore Key Features
        </h2>
        <p className="text-lg text-gray-600">
          Everything you need to manage your cryptocurrency
        </p>
      </div>

      {/* Feature List */}
      <div className="space-y-5">
        {/* Send & Receive Feature */}
        <div className="p-5 bg-white border-2 border-primary-200 rounded-xl hover:border-primary-300 transition-colors">
          <div className="flex items-start gap-4">
            <div className="flex-shrink-0 w-12 h-12 bg-gradient-to-br from-primary-500 to-primary-600 rounded-xl flex items-center justify-center">
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
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <div className="flex-1">
              <h3 className="font-semibold text-gray-900 mb-1 text-lg">
                Send & Receive
              </h3>
              <p className="text-sm text-gray-600 mb-3">
                Transfer crypto to anyone, anywhere in the world. Generate QR
                codes for easy receiving.
              </p>
              <span className="inline-flex items-center text-xs font-medium text-primary-600 bg-primary-50 px-2.5 py-1 rounded-full">
                Available in Wallet
              </span>
            </div>
          </div>
        </div>

        {/* Swap Tokens Feature */}
        <div className="p-5 bg-white border-2 border-secondary-200 rounded-xl hover:border-secondary-300 transition-colors">
          <div className="flex items-start gap-4">
            <div className="flex-shrink-0 w-12 h-12 bg-gradient-to-br from-secondary-500 to-secondary-600 rounded-xl flex items-center justify-center">
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
            <div className="flex-1">
              <h3 className="font-semibold text-gray-900 mb-1 text-lg">
                Swap Tokens
              </h3>
              <p className="text-sm text-gray-600 mb-3">
                Exchange one cryptocurrency for another instantly with transparent
                pricing and low fees.
              </p>
              <span className="inline-flex items-center text-xs font-medium text-secondary-600 bg-secondary-50 px-2.5 py-1 rounded-full">
                Available in Swap
              </span>
            </div>
          </div>
        </div>

        {/* Investment Strategies Feature */}
        <div className="p-5 bg-white border-2 border-accent-200 rounded-xl hover:border-accent-300 transition-colors">
          <div className="flex items-start gap-4">
            <div className="flex-shrink-0 w-12 h-12 bg-gradient-to-br from-accent-500 to-accent-600 rounded-xl flex items-center justify-center">
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
            <div className="flex-1">
              <h3 className="font-semibold text-gray-900 mb-1 text-lg">
                Investment Strategies
              </h3>
              <p className="text-sm text-gray-600 mb-3">
                Connect your exchange accounts and automate investment strategies to
                grow your portfolio.
              </p>
              <span className="inline-flex items-center text-xs font-medium text-accent-600 bg-accent-50 px-2.5 py-1 rounded-full">
                Available in Investment
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Help Center Info */}
      <div className="mt-8 p-5 bg-gradient-to-r from-primary-50 to-accent-50 rounded-xl border border-primary-100">
        <div className="flex items-start gap-3">
          <svg
            className="flex-shrink-0 w-6 h-6 text-primary-600 mt-0.5"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <div className="flex-1">
            <h4 className="font-semibold text-gray-900 mb-1">Need Help?</h4>
            <p className="text-sm text-gray-600">
              Visit our Help Center anytime for detailed guides, FAQs, and
              support. You can also access tutorials from your dashboard.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
