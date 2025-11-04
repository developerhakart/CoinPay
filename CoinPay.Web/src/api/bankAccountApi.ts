import { env } from '../config/env';

export interface BankAccount {
  id: string;
  accountHolderName: string;
  accountType: 'checking' | 'savings';
  bankName?: string;
  lastFourDigits: string;
  isPrimary: boolean;
  isVerified: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface BankAccountFormData {
  accountHolderName: string;
  routingNumber: string;
  accountNumber: string;
  accountType: 'checking' | 'savings';
  bankName?: string;
  isPrimary?: boolean;
}

export interface BankAccountListResponse {
  bankAccounts: BankAccount[];
  total: number;
}

export interface ApiError {
  error: {
    code: string;
    message: string;
    details?: Record<string, unknown>;
  };
}

class BankAccountApi {
  private baseUrl: string;

  constructor() {
    this.baseUrl = env.apiBaseUrl;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
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
      const error: ApiError = await response.json().catch(() => ({
        error: {
          code: 'UNKNOWN_ERROR',
          message: 'An unexpected error occurred',
        },
      }));
      throw new Error(error.error.message);
    }

    // Handle 204 No Content
    if (response.status === 204) {
      return {} as T;
    }

    return response.json();
  }

  /**
   * Get all bank accounts for the authenticated user
   */
  async getAll(): Promise<BankAccountListResponse> {
    return this.request<BankAccountListResponse>('/api/bank-account');
  }

  /**
   * Get bank account by ID
   */
  async getById(id: string): Promise<BankAccount> {
    return this.request<BankAccount>(`/api/bank-account/${id}`);
  }

  /**
   * Add new bank account
   */
  async add(data: BankAccountFormData): Promise<BankAccount> {
    return this.request<BankAccount>('/api/bank-account', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  /**
   * Update existing bank account
   */
  async update(id: string, data: Partial<BankAccountFormData>): Promise<BankAccount> {
    return this.request<BankAccount>(`/api/bank-account/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  /**
   * Delete bank account
   */
  async delete(id: string): Promise<void> {
    return this.request<void>(`/api/bank-account/${id}`, {
      method: 'DELETE',
    });
  }

  /**
   * Validate bank account with detailed result
   */
  async validate(data: {
    accountHolderName: string;
    routingNumber: string;
    accountNumber: string;
    accountType: string;
    bankName?: string;
  }): Promise<BankValidationResponse> {
    return this.request('/api/bank-account/validate', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  /**
   * Lookup bank name from routing number
   */
  async lookupBankName(routingNumber: string): Promise<BankLookupResponse> {
    return this.request(`/api/bank-account/lookup/${routingNumber}`);
  }
}

export interface BankValidationResponse {
  isValid: boolean;
  errors: string[];
  warnings: string[];
  suggestedBankName?: string;
}

export interface BankLookupResponse {
  routingNumber: string;
  bankName: string;
}

export const bankAccountApi = new BankAccountApi();
