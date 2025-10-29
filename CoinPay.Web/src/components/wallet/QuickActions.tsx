import React from 'react';
import { useNavigate } from 'react-router-dom';

interface QuickActionsProps {
  onReceiveClick: () => void;
  walletAddress?: string; // Optional, not currently used but may be needed for future features
}

export const QuickActions: React.FC<QuickActionsProps> = ({ onReceiveClick }) => {
  const navigate = useNavigate();

  const actions = [
    {
      id: 'send',
      label: 'Send',
      icon: (
        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
        </svg>
      ),
      onClick: () => navigate('/transfer'),
      color: 'bg-blue-500 hover:bg-blue-600'
    },
    {
      id: 'receive',
      label: 'Receive',
      icon: (
        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m0 0l-4-4m4 4l4-4" />
        </svg>
      ),
      onClick: onReceiveClick,
      color: 'bg-green-500 hover:bg-green-600'
    },
    {
      id: 'qr',
      label: 'QR Code',
      icon: (
        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v1m6 11h2m-6 0h-2v4m0-11v3m0 0h.01M12 12h4.01M16 20h4M4 12h4m12 0h.01M5 8h2a1 1 0 001-1V5a1 1 0 00-1-1H5a1 1 0 00-1 1v2a1 1 0 001 1zm12 0h2a1 1 0 001-1V5a1 1 0 00-1-1h-2a1 1 0 00-1 1v2a1 1 0 001 1zM5 20h2a1 1 0 001-1v-2a1 1 0 00-1-1H5a1 1 0 00-1 1v2a1 1 0 001 1z" />
        </svg>
      ),
      onClick: onReceiveClick,
      color: 'bg-purple-500 hover:bg-purple-600'
    }
  ];

  return (
    <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
      <h3 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
      <div className="grid grid-cols-3 gap-4">
        {actions.map((action) => (
          <button
            key={action.id}
            onClick={action.onClick}
            className={`flex flex-col items-center justify-center p-4 rounded-lg text-white transition-all transform hover:scale-105 ${action.color}`}
          >
            {action.icon}
            <span className="mt-2 text-sm font-medium">{action.label}</span>
          </button>
        ))}
      </div>
    </div>
  );
};
