import { env } from '../config/env';

export interface ExchangeRateResponse {
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

export interface FeeConfiguration {
  conversionFeePercent: number;
  payoutFlatFee: number;
  minimumPayoutAmount: number;
  maximumPayoutAmount?: number;
  feeTier: string;
  description: string;
}

export interface FeeBreakdown {
  usdAmountBeforeFees: number;
  conversionFeePercent: number;
  conversionFeeAmount: number;
  payoutFeeAmount: number;
  totalFees: number;
  netAmount: number;
  effectiveFeeRate: number;
}

class RatesApi {
  private baseUrl: string;

  constructor() {
    this.baseUrl = env.apiBaseUrl;
  }

  private async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const token = localStorage.getItem('authToken');

    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({
        error: { code: 'UNKNOWN_ERROR', message: 'An unexpected error occurred' },
      }));
      throw new Error(error.error?.message || 'Request failed');
    }

    return response.json();
  }

  /**
   * Get current USDC to USD exchange rate
   */
  async getUsdcToUsdRate(): Promise<ExchangeRateResponse> {
    return this.request<ExchangeRateResponse>('/api/rates/usdc-usd');
  }

  /**
   * Get fee configuration
   */
  async getFeeConfiguration(): Promise<FeeConfiguration> {
    return this.request<FeeConfiguration>('/api/rates/fees');
  }

  /**
   * Calculate fee breakdown for a specific amount
   */
  async calculateFees(usdAmount: number): Promise<FeeBreakdown> {
    return this.request<FeeBreakdown>(`/api/rates/fees/calculate?usdAmount=${usdAmount}`);
  }

  /**
   * Force refresh exchange rate (clears cache)
   */
  async refreshRate(): Promise<ExchangeRateResponse> {
    return this.request<ExchangeRateResponse>('/api/rates/refresh', {
      method: 'POST',
    });
  }

  /**
   * Check if rate service is healthy
   */
  async checkHealth(): Promise<{ isAvailable: boolean; status: string; timestamp: string }> {
    return this.request('/api/rates/health');
  }
}

export const ratesApi = new RatesApi();
