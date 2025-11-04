// Investment-related type definitions for Sprint N04 - Phase 4: Exchange Investment

export interface ExchangeConnection {
  connectionId: string;
  exchangeName: string;
  status: 'active' | 'inactive';
  connectedAt: string;
  lastValidated?: string;
}

export interface ConnectExchangeRequest {
  apiKey: string;
  apiSecret: string;
}

export interface ExchangeConnectionStatus {
  connected: boolean;
  connectionId?: string;
  exchangeName?: string;
  connectedAt?: string;
  lastValidated?: string;
}

export interface InvestmentPlan {
  planId: string;
  asset: string;
  apy: number;
  apyFormatted: string;
  minAmount: number;
  maxAmount?: number;
  term: string;
  description: string;
}

export interface CreateInvestmentRequest {
  planId: string;
  amount: number;
}

export interface CreateInvestmentResponse {
  investmentId: string;
  planId: string;
  asset: string;
  amount: number;
  apy: number;
  status: string;
  estimatedDailyReward: number;
  estimatedMonthlyReward: number;
  estimatedYearlyReward: number;
  createdAt: string;
}

export interface InvestmentPosition {
  id: string;
  planId: string;
  asset: string;
  principalAmount: number;
  currentValue: number;
  accruedRewards: number;
  apy: number;
  status: 'Active' | 'Closed' | 'Failed';
  startDate?: string;
  lastSyncedAt?: string;
  daysHeld: number;
  estimatedDailyReward: number;
  estimatedMonthlyReward: number;
  estimatedYearlyReward: number;
}

export interface InvestmentTransaction {
  id: string;
  type: 'Create' | 'Withdraw' | 'AccrueRewards';
  amount: number;
  status: 'Pending' | 'Confirmed' | 'Failed';
  createdAt: string;
}

export interface ProjectedRewards {
  daily: number;
  monthly: number;
  yearly: number;
}

export interface InvestmentPositionDetail {
  id: string;
  planId: string;
  planName: string;
  asset: string;
  principalAmount: number;
  currentValue: number;
  accruedRewards: number;
  apy: number;
  status: string;
  startDate?: string;
  endDate?: string;
  lastSyncedAt?: string;
  daysHeld: number;
  estimatedDailyReward: number;
  estimatedMonthlyReward: number;
  estimatedYearlyReward: number;
  transactions: InvestmentTransaction[];
  projectedRewards: ProjectedRewards;
}

export interface WithdrawInvestmentRequest {
  // No body parameters needed - position ID is in URL
}

export interface WithdrawInvestmentResponse {
  investmentId: string;
  withdrawalAmount: number;
  principal: number;
  rewards: number;
  status: string;
  estimatedCompletionTime: string;
}

export interface InvestmentCalculation {
  amount: number;
  apy: number;
  dailyReward: number;
  monthlyReward: number;
  yearlyReward: number;
  totalReturn30Days: number;
  totalReturn365Days: number;
}
