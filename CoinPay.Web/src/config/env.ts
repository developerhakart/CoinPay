// Environment configuration with type safety
export const env = {
  // API Configuration
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5100',
  apiTimeout: parseInt(import.meta.env.VITE_API_TIMEOUT || '30000', 10),

  // Application Configuration
  appName: import.meta.env.VITE_APP_NAME || 'CoinPay',
  appVersion: import.meta.env.VITE_APP_VERSION || '1.0.0',

  // Feature Flags
  enableLogging: import.meta.env.VITE_ENABLE_LOGGING === 'true',
  enableMockApi: import.meta.env.VITE_ENABLE_MOCK_API === 'true',

  // Environment
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD,
  nodeEnv: import.meta.env.VITE_NODE_ENV || 'development',
} as const;

// Type-safe environment configuration
export type Environment = typeof env;

// Validate required environment variables
export function validateEnv(): void {
  const required = ['VITE_API_BASE_URL'] as const;
  const missing = required.filter((key) => !import.meta.env[key]);

  if (missing.length > 0) {
    console.warn('Missing environment variables:', missing);
  }

  if (env.enableLogging) {
    console.log('Environment configuration:', {
      apiBaseUrl: env.apiBaseUrl,
      nodeEnv: env.nodeEnv,
      isDevelopment: env.isDevelopment,
    });
  }
}
