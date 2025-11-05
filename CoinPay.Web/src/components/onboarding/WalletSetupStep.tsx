/**
 * WalletSetupStep Component (Step 2 of Onboarding)
 *
 * Displays wallet setup instructions and security best practices
 * for protecting cryptocurrency assets.
 */

export function WalletSetupStep() {
  return (
    <div>
      {/* Header */}
      <div className="text-center mb-8">
        <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gradient-to-br from-secondary-500 to-secondary-600 mb-6">
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
              d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
            />
          </svg>
        </div>
        <h2 className="text-3xl font-bold text-gray-900 mb-4">
          Secure Your Wallet
        </h2>
        <p className="text-lg text-gray-600">
          Follow these best practices to keep your crypto safe
        </p>
      </div>

      {/* Security Tips */}
      <div className="space-y-4">
        {/* Tip 1: Protect Private Keys */}
        <div className="flex items-start gap-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <div className="flex-shrink-0 w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
            <svg
              className="w-5 h-5 text-primary-600"
              fill="currentColor"
              viewBox="0 0 20 20"
              aria-hidden="true"
            >
              <path
                fillRule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clipRule="evenodd"
              />
            </svg>
          </div>
          <div className="flex-1">
            <h3 className="font-semibold text-gray-900 mb-1">
              Protect Your Private Keys
            </h3>
            <p className="text-sm text-gray-600">
              Never share your private keys or seed phrase with anyone. CoinPay
              will never ask for this information.
            </p>
          </div>
        </div>

        {/* Tip 2: Use Strong Passwords */}
        <div className="flex items-start gap-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <div className="flex-shrink-0 w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
            <svg
              className="w-5 h-5 text-primary-600"
              fill="currentColor"
              viewBox="0 0 20 20"
              aria-hidden="true"
            >
              <path
                fillRule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clipRule="evenodd"
              />
            </svg>
          </div>
          <div className="flex-1">
            <h3 className="font-semibold text-gray-900 mb-1">
              Use Strong Passwords
            </h3>
            <p className="text-sm text-gray-600">
              Create a unique, strong password for your CoinPay account and
              enable two-factor authentication when available.
            </p>
          </div>
        </div>

        {/* Tip 3: Verify Addresses */}
        <div className="flex items-start gap-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <div className="flex-shrink-0 w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
            <svg
              className="w-5 h-5 text-primary-600"
              fill="currentColor"
              viewBox="0 0 20 20"
              aria-hidden="true"
            >
              <path
                fillRule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clipRule="evenodd"
              />
            </svg>
          </div>
          <div className="flex-1">
            <h3 className="font-semibold text-gray-900 mb-1">
              Verify Addresses
            </h3>
            <p className="text-sm text-gray-600">
              Always double-check wallet addresses before sending transactions.
              Crypto transactions are irreversible.
            </p>
          </div>
        </div>

        {/* Tip 4: Stay Informed */}
        <div className="flex items-start gap-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <div className="flex-shrink-0 w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
            <svg
              className="w-5 h-5 text-primary-600"
              fill="currentColor"
              viewBox="0 0 20 20"
              aria-hidden="true"
            >
              <path
                fillRule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clipRule="evenodd"
              />
            </svg>
          </div>
          <div className="flex-1">
            <h3 className="font-semibold text-gray-900 mb-1">Stay Informed</h3>
            <p className="text-sm text-gray-600">
              Be aware of phishing attempts and only access CoinPay through
              official channels.
            </p>
          </div>
        </div>
      </div>

      {/* Important Warning */}
      <div className="mt-6 p-4 bg-warning-50 border border-warning-200 rounded-lg">
        <div className="flex gap-3">
          <svg
            className="flex-shrink-0 w-5 h-5 text-warning-600 mt-0.5"
            fill="currentColor"
            viewBox="0 0 20 20"
            aria-hidden="true"
          >
            <path
              fillRule="evenodd"
              d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
              clipRule="evenodd"
            />
          </svg>
          <p className="text-sm text-warning-800">
            <strong className="font-semibold">Important:</strong> CoinPay support
            will never ask for your password or private keys via email, phone, or
            chat.
          </p>
        </div>
      </div>
    </div>
  );
}
