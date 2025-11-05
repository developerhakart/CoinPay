import React, { useState, Fragment } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import { Token, SUPPORTED_TOKENS_ARRAY } from '@/constants/tokens';
import { useTokenBalances } from '@/hooks/useTokenBalances';

interface TokenSelectionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectToken: (token: Token) => void;
  excludeToken?: string;
}

export const TokenSelectionModal: React.FC<TokenSelectionModalProps> = ({
  isOpen,
  onClose,
  onSelectToken,
  excludeToken,
}) => {
  const [searchQuery, setSearchQuery] = useState('');
  const { data: balances, isLoading } = useTokenBalances();

  const filteredTokens = SUPPORTED_TOKENS_ARRAY.filter((token) => {
    // Exclude the token if specified
    if (excludeToken && token.address.toLowerCase() === excludeToken.toLowerCase()) {
      return false;
    }

    // Filter by search query
    if (searchQuery) {
      const query = searchQuery.toLowerCase();
      return (
        token.symbol.toLowerCase().includes(query) ||
        token.name.toLowerCase().includes(query)
      );
    }

    return true;
  });

  const handleSelectToken = (token: Token) => {
    onSelectToken(token);
    onClose();
    setSearchQuery('');
  };

  const formatBalance = (balance: number) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 6,
    }).format(balance);
  };

  return (
    <Transition appear show={isOpen} as={Fragment}>
      <Dialog as="div" className="relative z-50" onClose={onClose}>
        <Transition.Child
          as={Fragment}
          enter="ease-out duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-200"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-black bg-opacity-25" />
        </Transition.Child>

        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center p-4 text-center">
            <Transition.Child
              as={Fragment}
              enter="ease-out duration-300"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-200"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 text-left align-middle shadow-xl transition-all">
                <Dialog.Title
                  as="h3"
                  className="text-lg font-medium leading-6 text-gray-900 mb-4 flex items-center justify-between"
                >
                  <span>Select a token</span>
                  <button
                    onClick={onClose}
                    className="text-gray-400 hover:text-gray-600 focus:outline-none"
                    aria-label="Close modal"
                  >
                    <svg
                      className="w-6 h-6"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M6 18L18 6M6 6l12 12"
                      />
                    </svg>
                  </button>
                </Dialog.Title>

                <div className="mb-4">
                  <input
                    type="text"
                    placeholder="Search by name or symbol"
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                    aria-label="Search tokens"
                  />
                </div>

                <div className="space-y-1 max-h-96 overflow-y-auto">
                  {filteredTokens.length === 0 ? (
                    <div className="text-center py-8 text-gray-500">
                      No tokens found
                    </div>
                  ) : (
                    filteredTokens.map((token) => {
                      const balance =
                        balances?.get(token.address.toLowerCase()) || 0;

                      return (
                        <button
                          key={token.address}
                          onClick={() => handleSelectToken(token)}
                          className="w-full flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
                        >
                          <div className="flex items-center space-x-3">
                            <div className="w-8 h-8 rounded-full bg-gray-200 flex items-center justify-center">
                              <span className="text-sm font-medium text-gray-600">
                                {token.symbol.charAt(0)}
                              </span>
                            </div>
                            <div className="text-left">
                              <div className="font-medium text-gray-900">
                                {token.symbol}
                              </div>
                              <div className="text-sm text-gray-500">
                                {token.name}
                              </div>
                            </div>
                          </div>
                          <div className="text-right">
                            {isLoading ? (
                              <div className="w-16 h-4 bg-gray-200 animate-pulse rounded" />
                            ) : (
                              <span className="text-sm font-medium text-gray-900">
                                {formatBalance(balance)}
                              </span>
                            )}
                          </div>
                        </button>
                      );
                    })
                  )}
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
};
