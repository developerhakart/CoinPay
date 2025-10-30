import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { payoutApi, type PayoutStatusResponse } from '../api/payoutApi';

export const PayoutStatusPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [status, setStatus] = useState<PayoutStatusResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;

    const fetchStatus = async () => {
      try {
        const data = await payoutApi.getPayoutStatus(id);
        setStatus(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load payout status');
      } finally {
        setIsLoading(false);
      }
    };

    fetchStatus();

    // Auto-refresh every 30 seconds if pending or processing
    const interval = setInterval(() => {
      if (status?.status === 'pending' || status?.status === 'processing') {
        fetchStatus();
      }
    }, 30000);

    return () => clearInterval(interval);
  }, [id, status?.status]);

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error || !status) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-lg shadow-lg p-6 max-w-md w-full">
          <div className="text-center">
            <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg className="w-8 h-8 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </div>
            <h2 className="text-xl font-bold text-gray-900 mb-2">Error</h2>
            <p className="text-gray-600">{error || 'Payout not found'}</p>
          </div>
        </div>
      </div>
    );
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return 'text-green-600 bg-green-100';
      case 'failed':
        return 'text-red-600 bg-red-100';
      case 'processing':
        return 'text-blue-600 bg-blue-100';
      default:
        return 'text-yellow-600 bg-yellow-100';
    }
  };

  const getStageProgress = (stage: string) => {
    const stages = ['initiated', 'converting', 'transferring', 'completed'];
    return ((stages.indexOf(stage) + 1) / stages.length) * 100;
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl mx-auto">
        {/* Header */}
        <div className="bg-white rounded-lg shadow-lg p-6 mb-6">
          <div className="flex items-center justify-between mb-4">
            <h1 className="text-2xl font-bold text-gray-900">Payout Status</h1>
            <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(status.status)}`}>
              {status.status.charAt(0).toUpperCase() + status.status.slice(1)}
            </span>
          </div>

          <p className="text-sm text-gray-500">
            Payout ID: <span className="font-mono">{status.id}</span>
          </p>
        </div>

        {/* Progress Bar */}
        {(status.status === 'pending' || status.status === 'processing') && (
          <div className="bg-white rounded-lg shadow-lg p-6 mb-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">Progress</h2>
            <div className="relative">
              <div className="overflow-hidden h-2 mb-4 text-xs flex rounded bg-gray-200">
                <div
                  style={{ width: `${getStageProgress(status.stage)}%` }}
                  className="shadow-none flex flex-col text-center whitespace-nowrap text-white justify-center bg-blue-600 transition-all duration-500"
                ></div>
              </div>
              <p className="text-sm text-gray-600 capitalize">{status.stage.replace('_', ' ')}</p>
            </div>
          </div>
        )}

        {/* Status Details */}
        <div className="bg-white rounded-lg shadow-lg p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Details</h2>
          <div className="space-y-3">
            <div className="flex justify-between py-2 border-b border-gray-200">
              <span className="text-gray-600">Initiated:</span>
              <span className="font-medium">{new Date(status.initiatedAt).toLocaleString()}</span>
            </div>

            {status.completedAt && (
              <div className="flex justify-between py-2 border-b border-gray-200">
                <span className="text-gray-600">Completed:</span>
                <span className="font-medium">{new Date(status.completedAt).toLocaleString()}</span>
              </div>
            )}

            {status.estimatedArrival && !status.completedAt && (
              <div className="flex justify-between py-2 border-b border-gray-200">
                <span className="text-gray-600">Estimated Arrival:</span>
                <span className="font-medium">{new Date(status.estimatedArrival).toLocaleDateString()}</span>
              </div>
            )}

            {status.failureReason && (
              <div className="py-2">
                <span className="text-gray-600 block mb-2">Failure Reason:</span>
                <div className="bg-red-50 border border-red-200 rounded-md p-3 text-red-800 text-sm">
                  {status.failureReason}
                </div>
              </div>
            )}

            <div className="flex justify-between py-2">
              <span className="text-gray-600">Last Updated:</span>
              <span className="font-medium">{new Date(status.lastUpdated).toLocaleString()}</span>
            </div>
          </div>
        </div>

        {/* Timeline */}
        <div className="bg-white rounded-lg shadow-lg p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Timeline</h2>
          <div className="space-y-4">
            {status.events.map((event, idx) => (
              <div key={idx} className="flex">
                <div className="flex flex-col items-center mr-4">
                  <div className={`w-3 h-3 rounded-full ${
                    idx === status.events.length - 1 ? 'bg-blue-600' : 'bg-gray-400'
                  }`}></div>
                  {idx < status.events.length - 1 && (
                    <div className="w-0.5 h-full bg-gray-300 mt-1"></div>
                  )}
                </div>
                <div className="flex-1 pb-4">
                  <div className="flex items-center justify-between">
                    <h3 className="font-medium text-gray-900">{event.event}</h3>
                    <span className="text-sm text-gray-500">
                      {new Date(event.timestamp).toLocaleTimeString()}
                    </span>
                  </div>
                  {event.description && (
                    <p className="text-sm text-gray-600 mt-1">{event.description}</p>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Auto-refresh indicator */}
        {(status.status === 'pending' || status.status === 'processing') && (
          <div className="mt-4 text-center text-sm text-gray-500">
            <p>Auto-refreshing every 30 seconds...</p>
          </div>
        )}
      </div>
    </div>
  );
};
