/**
 * Error Service
 * Maps error codes to user-friendly messages with template variable support
 */

const ERROR_MESSAGES: Record<string, string> = {
  // Balance and Transaction Errors
  INSUFFICIENT_BALANCE: 'Insufficient {token} balance. You need {required} but only have {available}.',
  INSUFFICIENT_GAS_BALANCE: 'Insufficient MATIC balance for gas fees. Please fund your wallet with testnet MATIC.',
  INVALID_ADDRESS: 'Invalid recipient address. Please check the address and try again.',
  INVALID_SENDER_ADDRESS: 'Invalid sender address. Please verify your wallet address.',
  SAME_ADDRESS_TRANSFER: 'Cannot send to yourself. Please enter a different recipient address.',

  // Network and Connection Errors
  NETWORK_ERROR: 'Connection issue. Please check your internet connection and try again.',
  RPC_ERROR: 'Network service temporarily unavailable. Please try again in a moment.',
  TIMEOUT_ERROR: 'Request timed out. Please check your connection and try again.',
  SERVICE_UNAVAILABLE: 'The service is temporarily unavailable. Please try again later.',

  // Transaction Errors
  TRANSACTION_FAILED: 'Transaction failed. {reason}',
  TRANSACTION_REVERTED: 'Transaction was reverted on the blockchain. {reason}',
  TRANSACTION_PENDING: 'A transaction with similar parameters is still pending. Please wait before retrying.',
  GAS_ESTIMATION_FAILED: 'Unable to estimate gas. Transaction parameters may be invalid.',
  INVALID_GAS_PRICE: 'Gas price is too low. Please try again.',

  // Token and Swap Errors
  SWAP_FAILED: 'Swap failed. {reason}',
  SLIPPAGE_EXCEEDED: 'Slippage exceeded. The token price moved beyond your tolerance. Try increasing slippage or submit a new quote.',
  INVALID_TOKEN_PAIR: 'Invalid token pair for swapping. Please select valid tokens.',
  TOKEN_NOT_FOUND: 'Token "{token}" not found. Please select a different token.',
  INSUFFICIENT_LIQUIDITY: 'Insufficient liquidity for this swap. Try swapping a smaller amount.',
  PRICE_IMPACT_TOO_HIGH: 'Price impact is too high. Your swap amount is too large for current liquidity.',

  // Investment Errors
  INVALID_INVESTMENT_STRATEGY: 'Invalid investment strategy configuration. Please check your settings.',
  WHITEBIT_CONNECTION_FAILED: 'Failed to connect to WhiteBIT. Please verify your API credentials.',
  INVALID_API_CREDENTIALS: 'Invalid WhiteBIT API credentials. Please check your API key and secret.',
  STRATEGY_ALREADY_RUNNING: 'This strategy is already running. Stop it before starting again.',
  INSUFFICIENT_EXCHANGE_BALANCE: 'Insufficient balance on WhiteBIT. Please fund your exchange account.',
  STRATEGY_EXECUTION_FAILED: 'Strategy execution failed. {reason}',

  // Authentication Errors
  AUTHENTICATION_FAILED: 'Authentication failed. Please log in again.',
  SESSION_EXPIRED: 'Your session has expired. Please log in again.',
  INVALID_CREDENTIALS: 'Invalid email or password. Please check and try again.',
  USER_NOT_FOUND: 'User account not found.',
  ACCOUNT_LOCKED: 'Your account has been locked due to multiple failed login attempts. Please try again later.',
  EMAIL_NOT_VERIFIED: 'Please verify your email address before logging in.',

  // Wallet Errors
  WALLET_NOT_FOUND: 'Wallet not found. Please ensure your wallet is properly created.',
  WALLET_CREATION_FAILED: 'Failed to create wallet. Please try again.',
  INVALID_PRIVATE_KEY: 'Invalid private key format.',
  WALLET_LOCKED: 'Wallet is locked. Please unlock it to continue.',

  // API and Data Errors
  INVALID_REQUEST: 'Invalid request. Please check your input and try again.',
  MISSING_REQUIRED_FIELD: 'Missing required field: {field}',
  INVALID_EMAIL: 'Invalid email address. Please enter a valid email.',
  INVALID_PASSWORD: 'Invalid password. Password must be at least 8 characters.',
  DUPLICATE_EMAIL: 'Email already registered. Please use a different email or log in.',
  VALIDATION_ERROR: 'Validation error: {message}',

  // Generic Errors
  UNKNOWN_ERROR: 'An unexpected error occurred. Please try again.',
  INTERNAL_SERVER_ERROR: 'Internal server error. Our team has been notified. Please try again later.',
  BAD_REQUEST: 'Invalid request format. Please check your input.',
  UNAUTHORIZED: 'Unauthorized. Please log in to continue.',
  FORBIDDEN: 'You don\'t have permission to perform this action.',
  NOT_FOUND: 'The requested resource was not found.',
  CONFLICT: 'Operation conflicts with existing data. Please refresh and try again.',
  RATE_LIMITED: 'Too many requests. Please wait a moment and try again.',
};

/**
 * Get user-friendly error message for an error code
 * @param code - Error code to look up
 * @param context - Object with variables to interpolate (e.g., {token: 'USDC', required: '100'})
 * @returns User-friendly error message
 */
export function getErrorMessage(code: string, context?: Record<string, string>): string {
  const baseMessage = ERROR_MESSAGES[code] ?? ERROR_MESSAGES['UNKNOWN_ERROR'];
  let message: string = baseMessage ?? 'An unexpected error occurred. Please try again.';

  // Replace template variables with context values
  if (context && message) {
    Object.entries(context).forEach(([key, value]) => {
      message = message.replace(new RegExp(`\\{${key}\\}`, 'g'), value);
    });
  }

  return message;
}

/**
 * Extract error code and message from various error formats
 * @param error - Error object or string
 * @returns Object with code and message
 */
export function parseError(error: unknown): { code: string; message: string; originalError: unknown } {
  if (typeof error === 'string') {
    return {
      code: 'UNKNOWN_ERROR',
      message: getErrorMessage('UNKNOWN_ERROR'),
      originalError: error,
    };
  }

  if (error instanceof Error) {
    // Check if it's an API error with a code
    const apiError = error as any;
    if (apiError.response?.data?.code) {
      const code = apiError.response.data.code;
      return {
        code,
        message: getErrorMessage(code, apiError.response.data.context),
        originalError: error,
      };
    }

    // Check for standard HTTP error codes
    if (apiError.response?.status) {
      const statusCode = apiError.response.status;
      const statusErrorMap: Record<number, string> = {
        400: 'BAD_REQUEST',
        401: 'UNAUTHORIZED',
        403: 'FORBIDDEN',
        404: 'NOT_FOUND',
        409: 'CONFLICT',
        429: 'RATE_LIMITED',
        500: 'INTERNAL_SERVER_ERROR',
        502: 'SERVICE_UNAVAILABLE',
        503: 'SERVICE_UNAVAILABLE',
      };
      const code = statusErrorMap[statusCode] || 'UNKNOWN_ERROR';
      return {
        code,
        message: getErrorMessage(code),
        originalError: error,
      };
    }

    // Check for network errors
    if (apiError.code === 'ECONNREFUSED' || apiError.code === 'ENOTFOUND') {
      return {
        code: 'NETWORK_ERROR',
        message: getErrorMessage('NETWORK_ERROR'),
        originalError: error,
      };
    }

    // Check for timeout errors
    if (apiError.code === 'ECONNABORTED' || error.message?.includes('timeout')) {
      return {
        code: 'TIMEOUT_ERROR',
        message: getErrorMessage('TIMEOUT_ERROR'),
        originalError: error,
      };
    }

    // Use the error message if no specific code found
    return {
      code: 'UNKNOWN_ERROR',
      message: error.message ?? getErrorMessage('UNKNOWN_ERROR'),
      originalError: error,
    };
  }

  return {
    code: 'UNKNOWN_ERROR',
    message: getErrorMessage('UNKNOWN_ERROR'),
    originalError: error,
  };
}

/**
 * Format error for display in UI
 * @param error - Error to format
 * @returns Formatted error object ready for display
 */
export function formatErrorForDisplay(error: unknown): {
  title: string;
  message: string;
  type: 'error' | 'warning' | 'info';
  code: string;
} {
  const { code, message } = parseError(error);

  // Determine error type based on code
  const warningCodes = ['RATE_LIMITED', 'TIMEOUT_ERROR', 'TRANSACTION_PENDING'];
  const infoCodes = ['SESSION_EXPIRED'];

  return {
    title: 'Error',
    message,
    type: infoCodes.includes(code) ? 'info' : warningCodes.includes(code) ? 'warning' : 'error',
    code,
  };
}
