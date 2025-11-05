import { useState } from 'react';
import { Link } from 'react-router-dom';

interface ErrorAlertProps {
  /**
   * Error message to display
   */
  message: string;
  /**
   * Optional error title (defaults to 'Error')
   */
  title?: string;
  /**
   * Error type for styling
   * @default 'error'
   */
  type?: 'error' | 'warning' | 'info';
  /**
   * Optional function to call when retry button is clicked
   */
  onRetry?: () => void | Promise<void>;
  /**
   * Whether to show dismiss button
   * @default true
   */
  dismissible?: boolean;
  /**
   * Optional callback when dismissed
   */
  onDismiss?: () => void;
  /**
   * Additional CSS classes
   */
  className?: string;
  /**
   * Error code for debugging
   */
  errorCode?: string;
  /**
   * Show detailed error info (for development)
   */
  showDetails?: boolean;
}

export function ErrorAlert({
  message,
  title = 'Error',
  type = 'error',
  onRetry,
  dismissible = true,
  onDismiss,
  className = '',
  errorCode,
  showDetails = false,
}: ErrorAlertProps) {
  const [isVisible, setIsVisible] = useState(true);
  const [isRetrying, setIsRetrying] = useState(false);

  const handleDismiss = () => {
    setIsVisible(false);
    onDismiss?.();
  };

  const handleRetry = async () => {
    if (!onRetry) return;
    setIsRetrying(true);
    try {
      await onRetry();
    } finally {
      setIsRetrying(false);
    }
  };

  if (!isVisible) return null;

  // Color schemes based on error type
  const colorSchemes = {
    error: {
      container: 'bg-red-50 border-red-200',
      icon: 'text-red-600 bg-red-100',
      title: 'text-red-900',
      message: 'text-red-800',
      button: 'text-red-700 hover:text-red-900',
      link: 'text-red-600 hover:text-red-700',
    },
    warning: {
      container: 'bg-amber-50 border-amber-200',
      icon: 'text-amber-600 bg-amber-100',
      title: 'text-amber-900',
      message: 'text-amber-800',
      button: 'text-amber-700 hover:text-amber-900',
      link: 'text-amber-600 hover:text-amber-700',
    },
    info: {
      container: 'bg-blue-50 border-blue-200',
      icon: 'text-blue-600 bg-blue-100',
      title: 'text-blue-900',
      message: 'text-blue-800',
      button: 'text-blue-700 hover:text-blue-900',
      link: 'text-blue-600 hover:text-blue-700',
    },
  };

  const colors = colorSchemes[type];

  // Icon based on error type
  const getIcon = () => {
    switch (type) {
      case 'warning':
        return (
          <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 9v2m0 4v2m0-6a4 4 0 110-8 4 4 0 010 8zm0 0a4 4 0 100 8 4 4 0 000-8z"
            />
          </svg>
        );
      case 'info':
        return (
          <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        );
      default:
        return (
          <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        );
    }
  };

  return (
    <div
      className={`${colors.container} border rounded-lg p-4 shadow-sm ${className}`}
      role="alert"
      data-testid="error-alert"
    >
      <div className="flex gap-4">
        {/* Icon */}
        <div className={`flex-shrink-0 w-10 h-10 rounded-lg flex items-center justify-center ${colors.icon}`}>
          {getIcon()}
        </div>

        {/* Content */}
        <div className="flex-1 min-w-0">
          {/* Title and Message */}
          <div>
            <h3 className={`text-sm font-semibold ${colors.title}`}>{title}</h3>
            <p className={`text-sm mt-1 ${colors.message} whitespace-pre-wrap break-words`}>{message}</p>
          </div>

          {/* Error Code and Details (Development) */}
          {(errorCode || showDetails) && (
            <div className="mt-2 text-xs text-gray-600 bg-white/50 rounded p-2 font-mono">
              {errorCode && <div>Code: {errorCode}</div>}
              {showDetails && <div className="text-gray-500">Check browser console for more details</div>}
            </div>
          )}

          {/* Actions */}
          <div className="mt-3 flex flex-wrap gap-2">
            {onRetry && (
              <button
                onClick={handleRetry}
                disabled={isRetrying}
                className={`inline-flex items-center gap-1 px-3 py-1.5 text-sm font-medium rounded transition-colors ${
                  isRetrying
                    ? 'opacity-50 cursor-not-allowed'
                    : `${colors.link} hover:underline`
                }`}
                data-testid="error-retry-button"
              >
                {isRetrying ? (
                  <>
                    <svg className="h-4 w-4 animate-spin" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                    </svg>
                    Retrying...
                  </>
                ) : (
                  <>
                    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                    </svg>
                    Try Again
                  </>
                )}
              </button>
            )}

            {/* Help Link */}
            <Link
              to="/help"
              className={`inline-flex items-center gap-1 px-3 py-1.5 text-sm font-medium rounded transition-colors ${colors.link} hover:underline`}
              data-testid="error-help-link"
            >
              <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Need Help?
            </Link>
          </div>
        </div>

        {/* Dismiss Button */}
        {dismissible && (
          <button
            onClick={handleDismiss}
            className={`flex-shrink-0 p-1 rounded hover:bg-white/50 transition-colors ${colors.button}`}
            aria-label="Dismiss"
            data-testid="error-dismiss-button"
          >
            <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        )}
      </div>
    </div>
  );
}

/**
 * Container component for stacking multiple error alerts
 */
export interface ErrorListProps {
  errors: Array<ErrorAlertProps & { id: string }>;
  onDismiss?: (id: string) => void;
}

export function ErrorList({ errors, onDismiss }: ErrorListProps) {
  return (
    <div className="space-y-3">
      {errors.map((error) => (
        <ErrorAlert
          key={error.id}
          {...error}
          onDismiss={() => onDismiss?.(error.id)}
        />
      ))}
    </div>
  );
}
