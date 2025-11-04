import React, { useState } from 'react';
import { BankAccountList } from '../components/BankAccounts/BankAccountList';
import { BankAccountForm } from '../components/BankAccounts/BankAccountForm';
import { bankAccountApi, type BankAccount } from '../api/bankAccountApi';
import type { BankAccountFormData } from '../utils/bankAccountValidation';

type ModalMode = 'add' | 'edit' | null;

export const BankAccountsPage: React.FC = () => {
  const [modalMode, setModalMode] = useState<ModalMode>(null);
  const [editingAccount, setEditingAccount] = useState<BankAccount | null>(null);
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  // Open add modal
  const handleAdd = () => {
    setModalMode('add');
    setEditingAccount(null);
  };

  // Open edit modal
  const handleEdit = (account: BankAccount) => {
    setModalMode('edit');
    setEditingAccount(account);
  };

  // Close modal
  const handleCancel = () => {
    setModalMode(null);
    setEditingAccount(null);
  };

  // Handle form submission
  const handleSubmit = async (data: BankAccountFormData) => {
    if (modalMode === 'add') {
      await bankAccountApi.add(data);
    } else if (modalMode === 'edit' && editingAccount) {
      // For edit mode, only send updatable fields
      await bankAccountApi.update(editingAccount.id, {
        accountHolderName: data.accountHolderName,
        bankName: data.bankName,
        isPrimary: data.isPrimary,
      });
    }

    // Close modal and refresh list
    setModalMode(null);
    setEditingAccount(null);
    setRefreshTrigger(prev => prev + 1);
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        {/* Page Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Bank Accounts</h1>
          <p className="mt-2 text-sm text-gray-600">
            Manage your bank accounts for fiat withdrawals. All sensitive information is encrypted.
          </p>
        </div>

        {/* Bank Account List */}
        <BankAccountList
          onAdd={handleAdd}
          onEdit={handleEdit}
          refreshTrigger={refreshTrigger}
        />

        {/* Modal Overlay */}
        {modalMode && (
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4">
            <div className="relative bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
              {/* Modal Header */}
              <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 rounded-t-lg">
                <div className="flex items-center justify-between">
                  <h2 className="text-xl font-semibold text-gray-900">
                    {modalMode === 'add' ? 'Add Bank Account' : 'Edit Bank Account'}
                  </h2>
                  <button
                    onClick={handleCancel}
                    className="text-gray-400 hover:text-gray-600 focus:outline-none"
                  >
                    <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              </div>

              {/* Modal Body */}
              <div className="px-6 py-4">
                <BankAccountForm
                  mode={modalMode}
                  initialData={editingAccount ? {
                    accountHolderName: editingAccount.accountHolderName,
                    routingNumber: '', // Never send back to client
                    accountNumber: '', // Never send back to client
                    accountType: editingAccount.accountType as 'checking' | 'savings',
                    bankName: editingAccount.bankName,
                    isPrimary: editingAccount.isPrimary,
                  } : undefined}
                  onSubmit={handleSubmit}
                  onCancel={handleCancel}
                />
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
