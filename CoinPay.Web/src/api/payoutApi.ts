import { env } from '../config/env';

export interface PayoutInitiationRequest {
  bankAccountId: string;
  usdcAmount: number;
}

export interface BankAccountSummary {
  id: string;
  accountHolderName: string;
  lastFourDigits: string;
  accountType: string;
  bankName?: string;
}

export interface Payout {
  id: string;
  bankAccountId: string;
  gatewayTransactionId?: string;
  usdcAmount: number;
  usdAmount: number;
  exchangeRate: number;
  conversionFee: number;
  payoutFee: number;
  totalFees: number;
  netAmount: number;
  status: string;
  initiatedAt: string;
  completedAt?: string;
  estimatedArrival?: string;
  failureReason?: string;
  bankAccount?: BankAccountSummary;
}

export interface PayoutHistoryResponse {
  payouts: Payout[];
  total: number;
  offset: number;
  limit: number;
}

export interface PayoutStatusEvent {
  event: string;
  timestamp: string;
  description?: string;
}

export interface PayoutStatusResponse {
  id: string;
  status: string;
  stage: string;
  initiatedAt: string;
  completedAt?: string;
  estimatedArrival?: string;
  failureReason?: string;
  lastUpdated: string;
  events: PayoutStatusEvent[];
}

export interface ConversionPreviewRequest {
  usdcAmount: number;
}

export interface ConversionPreviewResponse {
  usdcAmount: number;
  exchangeRate: number;
  usdAmountBeforeFees: number;
  conversionFeePercent: number;
  conversionFeeAmount: number;
  payoutFeeAmount: number;
  totalFees: number;
  netUsdAmount: number;
  expiresAt: string;
}

class PayoutApi {
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

  async initiatePayout(request: PayoutInitiationRequest): Promise<Payout> {
    return this.request<Payout>('/api/payout/initiate', {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }

  async getPayoutHistory(limit = 20, offset = 0): Promise<PayoutHistoryResponse> {
    return this.request<PayoutHistoryResponse>(
      `/api/payout/history?limit=${limit}&offset=${offset}`
    );
  }

  async getPayoutStatus(id: string): Promise<PayoutStatusResponse> {
    return this.request<PayoutStatusResponse>(`/api/payout/${id}/status`);
  }

  async getConversionPreview(request: ConversionPreviewRequest): Promise<ConversionPreviewResponse> {
    return this.request<ConversionPreviewResponse>('/api/payout/preview', {
      method: 'POST',
      body: JSON.stringify(request),
    });
  }
}

export const payoutApi = new PayoutApi();
