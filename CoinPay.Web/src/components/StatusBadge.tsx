import { TransactionStatus } from '@/types';

interface StatusBadgeProps {
  status: TransactionStatus | 'Pending' | 'Completed' | 'Failed';
}

export function StatusBadge({ status }: StatusBadgeProps) {
  const getStatusStyles = () => {
    const statusLower = status.toLowerCase();
    if (statusLower === 'completed') {
      return 'bg-green-100 text-green-800 border-green-200';
    } else if (statusLower === 'pending') {
      return 'bg-yellow-100 text-yellow-800 border-yellow-200';
    } else if (statusLower === 'failed' || statusLower === 'cancelled') {
      return 'bg-red-100 text-red-800 border-red-200';
    } else if (statusLower === 'processing') {
      return 'bg-blue-100 text-blue-800 border-blue-200';
    } else {
      return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  return (
    <span className={`px-3 py-1 rounded-full text-xs font-semibold border ${getStatusStyles()}`}>
      {status}
    </span>
  );
}
