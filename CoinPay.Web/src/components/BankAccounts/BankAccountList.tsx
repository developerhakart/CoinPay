import React, { useState, useEffect } from 'react';
import { bankAccountApi, type BankAccount } from '../../api/bankAccountApi';

interface BankAccountListProps {
  onEdit?: (bankAccount: BankAccount) => void;
  onAdd?: () => void;
  refreshTrigger?: number; // Used to trigger refresh from parent
}

export const BankAccountList: React.FC<BankAccountListProps> = ({
  onEdit,
  onAdd,
  refreshTrigger = 0,
}) => {
  const [bankAccounts, setBankAccounts] = useState<BankAccount[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  // Fetch bank accounts
  const fetchBankAccounts = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await bankAccountApi.getAll();
      setBankAccounts(response.bankAccounts);
    } catch (err) {
      console.error('Failed to fetch bank accounts:', err);
      setError(err instanceof Error ? err.message : 'Failed to load bank accounts');
    } finally {
      setIsLoading(false);
    }
  };

  // Fetch on mount and when refreshTrigger changes
  useEffect(() => {
    fetchBankAccounts();
  }, [refreshTrigger]);

  // Handle delete
  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this bank account? This action cannot be undone.')) {
      return;
    }

    setDeletingId(id);

    try {
      await bankAccountApi.delete(id);
      // Remove from local state
      setBankAccounts(prev => prev.filter(account => account.id !== id));
    } catch (err) {
      console.error('Failed to delete bank account:', err);
      alert(err instanceof Error ? err.message : 'Failed to delete bank account');
    } finally {
      setDeletingId(null);
    }
  };

  // Loading state
  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-md p-4">
        <div className="flex items-center">
          <svg className="h-5 w-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
          </svg>
          <p className="text-sm text-red-800">{error}</p>
        </div>
        <button
          onClick={fetchBankAccounts}
          className="mt-3 text-sm text-red-600 hover:text-red-800 font-medium"
        >
          Try again
        </button>
      </div>
    );
  }

  // Empty state
  if (bankAccounts.length === 0) {
    return (
      <div className="text-center py-12">
        <svg
          className="mx-auto h-12 w-12 text-gray-400"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
          />
        </svg>
        <h3 className="mt-2 text-sm font-medium text-gray-900">No bank accounts</h3>
        <p className="mt-1 text-sm text-gray-500">
          Get started by adding your first bank account.
        </p>
        {onAdd && (
          <button
            onClick={onAdd}
            className="mt-4 inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <svg className="-ml-1 mr-2 h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clipRule="evenodd" />
            </svg>
            Add Bank Account
          </button>
        )}
      </div>
    );
  }

  // List view
  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex justify-between items-center">
        <h3 className="text-lg font-medium text-gray-900">
          Your Bank Accounts ({bankAccounts.length})
        </h3>
        {onAdd && (
          <button
            onClick={onAdd}
            className="inline-flex items-center px-3 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <svg className="-ml-0.5 mr-2 h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z" clipRule="evenodd" />
            </svg>
            Add Bank Account
          </button>
        )}
      </div>

      {/* Bank Accounts List */}
      <div className="grid gap-4 sm:grid-cols-1 lg:grid-cols-2">
        {bankAccounts.map((account) => (
          <div
            key={account.id}
            className={`relative bg-white border rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow ${
              account.isPrimary ? 'border-blue-500 ring-2 ring-blue-500 ring-opacity-50' : 'border-gray-200'
            }`}
          >
            {/* Primary Badge */}
            {account.isPrimary && (
              <div className="absolute top-3 right-3">
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                  Primary
                </span>
              </div>
            )}

            {/* Account Holder Name */}
            <div className="mb-3">
              <h4 className="text-lg font-medium text-gray-900">{account.accountHolderName}</h4>
              {account.bankName && (
                <p className="text-sm text-gray-500">{account.bankName}</p>
              )}
            </div>

            {/* Account Details */}
            <div className="space-y-2">
              <div className="flex items-center text-sm">
                <span className="text-gray-500 w-24">Account:</span>
                <span className="font-mono text-gray-900">•••• {account.lastFourDigits}</span>
              </div>
              <div className="flex items-center text-sm">
                <span className="text-gray-500 w-24">Type:</span>
                <span className="text-gray-900 capitalize">{account.accountType}</span>
              </div>
              <div className="flex items-center text-sm">
                <span className="text-gray-500 w-24">Status:</span>
                {account.isVerified ? (
                  <span className="inline-flex items-center text-green-700">
                    <svg className="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                    </svg>
                    Verified
                  </span>
                ) : (
                  <span className="text-gray-500">Unverified</span>
                )}
              </div>
            </div>

            {/* Actions */}
            <div className="mt-4 pt-4 border-t border-gray-200 flex justify-end space-x-2">
              {onEdit && (
                <button
                  onClick={() => onEdit(account)}
                  className="inline-flex items-center px-3 py-1.5 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                  <svg className="-ml-0.5 mr-1.5 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                  Edit
                </button>
              )}
              <button
                onClick={() => handleDelete(account.id)}
                disabled={deletingId === account.id}
                className="inline-flex items-center px-3 py-1.5 border border-red-300 text-sm font-medium rounded-md text-red-700 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {deletingId === account.id ? (
                  <>
                    <svg className="animate-spin -ml-0.5 mr-1.5 h-4 w-4" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Deleting...
                  </>
                ) : (
                  <>
                    <svg className="-ml-0.5 mr-1.5 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    Delete
                  </>
                )}
              </button>
            </div>

            {/* Added Date */}
            <div className="mt-3 text-xs text-gray-400">
              Added {new Date(account.createdAt).toLocaleDateString()}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
