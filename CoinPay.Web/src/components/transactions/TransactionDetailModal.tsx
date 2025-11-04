import { useState } from 'react';
import { StatusBadge } from '@/components/StatusBadge';
import type { Transaction } from '@/types/transaction';

interface TransactionDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  transaction: Transaction | null;
}

export const TransactionDetailModal = ({ isOpen, onClose, transaction }: TransactionDetailModalProps) => {
  const [copied, setCopied] = useState<string | null>(null);

  if (!isOpen || !transaction) return null;

  const handleCopy = async (text: string, field: string) => {
    try {
      await navigator.clipboard.writeText(text);
      setCopied(field);
      setTimeout(() => setCopied(null), 2000);
    } catch (err) {
      console.error('Failed to copy:', err);
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
      month: 'long',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  };

  const formatAmount = (amount: number, currency: string) => {
    // Use more decimals for cryptocurrencies, fewer for fiat
    const decimals = currency === 'USD' ? 2 : 6;
    return `${amount.toFixed(decimals)} ${currency}`;
  };

  const getTypeIcon = (type: string) => {
    switch (type) {
      case 'Payment':
        return (
          <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
            <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
          </div>
        );
      case 'Transfer':
        return (
          <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
            <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
            </svg>
          </div>
        );
      case 'Refund':
        return (
          <div className="w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center">
            <svg className="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h10a8 8 0 018 8v2M3 10l6 6m-6-6l6-6" />
            </svg>
          </div>
        );
      default:
        return (
          <div className="w-12 h-12 bg-gray-100 rounded-full flex items-center justify-center">
            <svg className="w-6 h-6 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </div>
        );
    }
  };

  const CopyButton = ({ text, field }: { text: string; field: string }) => (
    <button
      onClick={() => handleCopy(text, field)}
      className="p-1 text-gray-400 hover:text-gray-600 transition-colors"
      title="Copy to clipboard"
    >
      {copied === field ? (
        <svg className="w-4 h-4 text-green-600" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
        </svg>
      ) : (
        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
        </svg>
      )}
    </button>
  );

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div className="flex min-h-screen items-center justify-center p-4">
        {/* Background overlay */}
        <div
          className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
          onClick={onClose}
        ></div>

        {/* Modal panel */}
        <div className="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all w-full max-w-2xl">
          {/* Header */}
          <div className="bg-gradient-to-r from-indigo-500 to-purple-600 px-6 py-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                {getTypeIcon(transaction.type)}
                <div>
                  <h3 className="text-lg font-semibold text-white">Transaction Details</h3>
                  <p className="text-sm text-indigo-100">{transaction.type}</p>
                </div>
              </div>
              <button
                onClick={onClose}
                className="text-white hover:text-indigo-100 transition-colors"
              >
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          </div>

          {/* Body */}
          <div className="px-6 py-4 max-h-[calc(100vh-200px)] overflow-y-auto">
            {/* Amount and Status */}
            <div className="mb-6 p-4 bg-gray-50 rounded-lg">
              <div className="flex items-center justify-between mb-2">
                <span className="text-sm text-gray-500">Amount</span>
                <StatusBadge status={transaction.status as any} />
              </div>
              <p className="text-3xl font-bold text-gray-900">
                {formatAmount(transaction.amount, transaction.currency)}
              </p>
            </div>

            {/* Transaction Information */}
            <div className="space-y-4">
              {/* Transaction ID */}
              {transaction.transactionId && (
                <div>
                  <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Transaction ID</label>
                  <div className="mt-1 flex items-center gap-2 p-2 bg-gray-50 rounded">
                    <code className="flex-1 text-sm font-mono text-gray-900 break-all">{transaction.transactionId}</code>
                    <CopyButton text={transaction.transactionId} field="txId" />
                  </div>
                </div>
              )}

              {/* Sender */}
              {transaction.senderName && (
                <div>
                  <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">From</label>
                  <div className="mt-1 flex items-center gap-2 p-2 bg-gray-50 rounded">
                    <p className="flex-1 text-sm text-gray-900">{transaction.senderName}</p>
                    <CopyButton text={transaction.senderName} field="sender" />
                  </div>
                </div>
              )}

              {/* Receiver */}
              {transaction.receiverName && (
                <div>
                  <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">To</label>
                  <div className="mt-1 flex items-center gap-2 p-2 bg-gray-50 rounded">
                    <p className="flex-1 text-sm text-gray-900">{transaction.receiverName}</p>
                    <CopyButton text={transaction.receiverName} field="receiver" />
                  </div>
                </div>
              )}

              {/* Description */}
              {transaction.description && (
                <div>
                  <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Description</label>
                  <p className="mt-1 text-sm text-gray-900 p-2 bg-gray-50 rounded">{transaction.description}</p>
                </div>
              )}

              <div className="border-t border-gray-200 pt-4"></div>

              {/* Created Date */}
              <div>
                <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Created</label>
                <p className="mt-1 text-sm text-gray-900">{formatDate(transaction.createdAt)}</p>
              </div>

              {/* Completed Date */}
              {transaction.completedAt && (
                <div>
                  <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Completed</label>
                  <p className="mt-1 text-sm text-gray-900">{formatDate(transaction.completedAt)}</p>
                </div>
              )}

              {/* Network Info */}
              <div>
                <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Network</label>
                <p className="mt-1 text-sm text-gray-900">Polygon Amoy Testnet</p>
              </div>

              {/* Transaction Type */}
              <div>
                <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Type</label>
                <p className="mt-1 text-sm text-gray-900">{transaction.type}</p>
              </div>

              {/* Currency */}
              <div>
                <label className="text-xs font-medium text-gray-500 uppercase tracking-wider">Currency</label>
                <p className="mt-1 text-sm text-gray-900">{transaction.currency}</p>
              </div>
            </div>
          </div>

          {/* Footer */}
          <div className="bg-gray-50 px-6 py-4 flex justify-end gap-3">
            <button
              onClick={onClose}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
