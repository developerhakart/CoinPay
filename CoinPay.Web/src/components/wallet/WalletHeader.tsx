import React, { useState } from 'react';

interface WalletHeaderProps {
  address: string;
  username?: string;
}

export const WalletHeader: React.FC<WalletHeaderProps> = ({ address, username }) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(address);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error('Failed to copy address:', err);
    }
  };

  return (
    <div className="bg-gradient-to-r from-indigo-600 to-purple-600 rounded-lg shadow-lg p-6 mb-6 text-white">
      <div className="mb-4">
        <h2 className="text-2xl font-bold mb-1">
          {username ? `${username}'s Wallet` : 'My Wallet'}
        </h2>
        <p className="text-indigo-100 text-sm">Your wallet address for receiving USDC</p>
      </div>

      {/* Full Address Display - Prominent */}
      <div className="bg-white/10 backdrop-blur-sm rounded-lg p-4 border border-white/20">
        <div className="flex items-center justify-between mb-2">
          <span className="text-xs font-semibold uppercase tracking-wider text-indigo-100">
            Wallet Address
          </span>
          <button
            onClick={handleCopy}
            className="inline-flex items-center gap-2 px-3 py-1.5 text-sm font-medium bg-white/20 hover:bg-white/30 rounded-md transition-colors border border-white/30"
            title="Copy full address"
          >
            {copied ? (
              <>
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                </svg>
                <span>Copied!</span>
              </>
            ) : (
              <>
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
                <span>Copy Address</span>
              </>
            )}
          </button>
        </div>

        {/* Full address - easy to read and select */}
        <div className="bg-gray-900/50 rounded px-3 py-2 font-mono text-sm break-all select-all">
          {address}
        </div>

        {/* Helper text */}
        <div className="mt-3 flex items-start gap-2 text-xs text-indigo-100">
          <svg className="w-4 h-4 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div>
            <p className="font-medium mb-1">Send testnet USDC to this address:</p>
            <ul className="space-y-0.5 ml-0">
              <li>• Polygon Amoy Faucet: <a href="https://faucet.polygon.technology/" target="_blank" rel="noopener noreferrer" className="underline hover:text-white">faucet.polygon.technology</a></li>
              <li>• Circle USDC Faucet: <a href="https://faucet.circle.com/" target="_blank" rel="noopener noreferrer" className="underline hover:text-white">faucet.circle.com</a></li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};
