// Application constants

export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  REGISTER: '/register',
  DASHBOARD: '/dashboard',
  WALLET: '/wallet',
  TRANSFER: '/transfer',
} as const;

export const TRANSACTION_STATUS = {
  PENDING: 'pending',
  PROCESSING: 'processing',
  COMPLETED: 'completed',
  FAILED: 'failed',
  CANCELLED: 'cancelled',
} as const;

export const CURRENCIES = {
  USDC: 'USDC',
  ETH: 'ETH',
  BTC: 'BTC',
} as const;

export const API_ENDPOINTS = {
  // Auth
  AUTH_CHECK_USERNAME: '/auth/check-username',
  AUTH_REGISTER_INITIATE: '/auth/register/initiate',
  AUTH_REGISTER_COMPLETE: '/auth/register/complete',
  AUTH_LOGIN_INITIATE: '/auth/login/initiate',
  AUTH_LOGIN_COMPLETE: '/auth/login/complete',
  AUTH_PROFILE: '/me',

  // Wallet
  WALLET_CREATE: '/wallet',
  WALLET_BY_USER: '/wallet/user',
  WALLET_BALANCE: '/wallet/:address/balance',

  // Transactions
  TRANSACTIONS: '/transactions',
  TRANSACTION_BY_ID: '/transactions/:id',
  TRANSACTION_BY_STATUS: '/transactions/status/:status',
} as const;

export const LOCAL_STORAGE_KEYS = {
  AUTH_TOKEN: 'auth_token',
  AUTH_USER: 'auth_user',
} as const;

export const DEFAULT_VALUES = {
  CURRENCY: 'USDC',
  PAGE_SIZE: 10,
  REQUEST_TIMEOUT: 30000,
} as const;
