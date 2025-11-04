import React, { useState, useEffect } from 'react';

interface ExchangeRateInfo {
  rate: number;
  baseCurrency: string;
  quoteCurrency: string;
  timestamp: string;
  validForSeconds: number;
  source: string;
  isCached: boolean;
  expiresAt: string;
  isValid: boolean;
  secondsUntilExpiration: number;
}

interface ExchangeRateDisplayProps {
  /**
   * Show full details including source and expiration
   * @default false
   */
  showDetails?: boolean;

  /**
   * Display size variant
   * @default 'medium'
   */
  size?: 'small' | 'medium' | 'large';

  /**
   * Auto-refresh when rate expires
   * @default true
   */
  autoRefresh?: boolean;

  /**
   * Callback when rate updates
   */
  onRateUpdate?: (rate: ExchangeRateInfo) => void;

  /**
   * Custom CSS classes
   */
  className?: string;
}

/**
 * Exchange Rate Display Component
 * Fetches and displays real-time USDC to USD exchange rate
 * Auto-refreshes when rate expires
 */
export const ExchangeRateDisplay: React.FC<ExchangeRateDisplayProps> = ({
  showDetails = false,
  size = 'medium',
  autoRefresh = true,
  onRateUpdate,
  className = '',
}) => {
  const [rateInfo, setRateInfo] = useState<ExchangeRateInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [secondsRemaining, setSecondsRemaining] = useState(0);

  // Fetch exchange rate from API
  const fetchRate = async () => {
    try {
      setError(null);
      const token = localStorage.getItem('authToken');
      const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5100'}/api/rates/usdc-usd`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });

      if (!response.ok) {
        throw new Error('Failed to fetch exchange rate');
      }

      const data: ExchangeRateInfo = await response.json();
      setRateInfo(data);
      setSecondsRemaining(data.secondsUntilExpiration);

      if (onRateUpdate) {
        onRateUpdate(data);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load exchange rate');
    } finally {
      setIsLoading(false);
    }
  };

  // Initial fetch
  useEffect(() => {
    fetchRate();
  }, []);

  // Auto-refresh when rate expires
  useEffect(() => {
    if (!autoRefresh || !rateInfo) return;

    const interval = setInterval(() => {
      const newSecondsRemaining = Math.max(
        0,
        Math.floor((new Date(rateInfo.expiresAt).getTime() - Date.now()) / 1000)
      );

      setSecondsRemaining(newSecondsRemaining);

      // Refresh when expired
      if (newSecondsRemaining <= 0) {
        fetchRate();
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [rateInfo, autoRefresh]);

  // Size variants
  const sizeClasses = {
    small: {
      container: 'text-sm',
      rate: 'text-lg font-semibold',
      label: 'text-xs',
    },
    medium: {
      container: 'text-base',
      rate: 'text-2xl font-bold',
      label: 'text-sm',
    },
    large: {
      container: 'text-lg',
      rate: 'text-3xl font-bold',
      label: 'text-base',
    },
  };

  const classes = sizeClasses[size];

  if (isLoading) {
    return (
      <div className={`flex items-center space-x-2 ${className}`}>
        <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
        <span className="text-gray-500">Loading rate...</span>
      </div>
    );
  }

  if (error || !rateInfo) {
    return (
      <div className={`text-red-600 ${classes.container} ${className}`}>
        <span>{error || 'Rate unavailable'}</span>
      </div>
    );
  }

  return (
    <div className={`${classes.container} ${className}`}>
      {/* Main Rate Display */}
      <div className="flex items-baseline space-x-2">
        <span className={`${classes.rate} text-gray-900`}>
          {rateInfo.rate.toFixed(4)}
        </span>
        <span className={`${classes.label} text-gray-600`}>
          {rateInfo.baseCurrency}/{rateInfo.quoteCurrency}
        </span>
      </div>

      {/* Details Section */}
      {showDetails && (
        <div className={`mt-2 space-y-1 ${classes.label} text-gray-500`}>
          <div className="flex items-center justify-between">
            <span>Source:</span>
            <span className="font-medium text-gray-700">{rateInfo.source}</span>
          </div>

          <div className="flex items-center justify-between">
            <span>Status:</span>
            <span className={`font-medium ${rateInfo.isCached ? 'text-blue-600' : 'text-green-600'}`}>
              {rateInfo.isCached ? 'Cached' : 'Live'}
            </span>
          </div>

          {autoRefresh && (
            <div className="flex items-center justify-between">
              <span>Refreshes in:</span>
              <span className={`font-medium font-mono ${secondsRemaining <= 5 ? 'text-orange-600' : 'text-gray-700'}`}>
                {secondsRemaining}s
              </span>
            </div>
          )}

          <div className="flex items-center justify-between">
            <span>Updated:</span>
            <span className="font-medium text-gray-700">
              {new Date(rateInfo.timestamp).toLocaleTimeString()}
            </span>
          </div>
        </div>
      )}

      {/* Expiration Indicator (Compact) */}
      {!showDetails && autoRefresh && (
        <div className="mt-1">
          <div className="flex items-center space-x-2">
            <div className="flex-1 bg-gray-200 rounded-full h-1.5">
              <div
                className={`h-1.5 rounded-full transition-all duration-1000 ${
                  secondsRemaining <= 5 ? 'bg-orange-500' : 'bg-blue-600'
                }`}
                style={{
                  width: `${(secondsRemaining / rateInfo.validForSeconds) * 100}%`,
                }}
              ></div>
            </div>
            <span className="text-xs text-gray-500 font-mono w-8 text-right">
              {secondsRemaining}s
            </span>
          </div>
        </div>
      )}
    </div>
  );
};

/**
 * Compact Exchange Rate Badge
 * Shows rate in a small badge format
 */
export const ExchangeRateBadge: React.FC<{ className?: string }> = ({ className = '' }) => {
  const [rateInfo, setRateInfo] = useState<ExchangeRateInfo | null>(null);

  useEffect(() => {
    const fetchRate = async () => {
      try {
        const token = localStorage.getItem('authToken');
        const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5100'}/api/rates/usdc-usd`, {
          headers: token ? { Authorization: `Bearer ${token}` } : {},
        });
        if (response.ok) {
          const data: ExchangeRateInfo = await response.json();
          setRateInfo(data);
        }
      } catch (error) {
        console.error('Failed to fetch exchange rate:', error);
      }
    };

    fetchRate();
    const interval = setInterval(fetchRate, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, []);

  if (!rateInfo) return null;

  return (
    <div className={`inline-flex items-center px-2.5 py-1 bg-blue-50 text-blue-700 text-xs font-medium rounded-md ${className}`}>
      <span className="mr-1">1 USDC =</span>
      <span className="font-semibold">{rateInfo.rate.toFixed(4)} USD</span>
    </div>
  );
};
