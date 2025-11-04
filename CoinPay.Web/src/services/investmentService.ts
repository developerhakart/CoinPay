import apiClient from './apiClient';
import {
  ConnectExchangeRequest,
  ExchangeConnection,
  ExchangeConnectionStatus,
  InvestmentPlan,
  CreateInvestmentRequest,
  CreateInvestmentResponse,
  InvestmentPosition,
  InvestmentPositionDetail,
  WithdrawInvestmentResponse,
  InvestmentCalculation
} from '@/types/investment';

/**
 * Investment Service - Sprint N04: Phase 4 Exchange Investment
 * Handles all API calls related to exchange connections and investments
 */
export const investmentService = {
  // ============================================================================
  // EXCHANGE CONNECTION MANAGEMENT
  // ============================================================================

  /**
   * Connect WhiteBit exchange account with API credentials
   */
  async connectWhiteBit(data: ConnectExchangeRequest): Promise<ExchangeConnection> {
    const response = await apiClient.post('/exchange/whitebit/connect', data);
    return response.data;
  },

  /**
   * Get WhiteBit connection status for current user
   */
  async getWhiteBitStatus(): Promise<ExchangeConnectionStatus> {
    const response = await apiClient.get('/exchange/whitebit/status');
    return response.data;
  },

  /**
   * Get available WhiteBit Flex investment plans
   */
  async getWhiteBitPlans(): Promise<InvestmentPlan[]> {
    const response = await apiClient.get('/exchange/whitebit/plans');
    return response.data;
  },

  // ============================================================================
  // INVESTMENT POSITION MANAGEMENT
  // ============================================================================

  /**
   * Create a new investment position
   */
  async createInvestment(data: CreateInvestmentRequest): Promise<CreateInvestmentResponse> {
    const response = await apiClient.post('/investment/create', data);
    return response.data;
  },

  /**
   * Get all investment positions for current user
   */
  async getPositions(): Promise<InvestmentPosition[]> {
    const response = await apiClient.get('/investment/positions');
    return response.data;
  },

  /**
   * Get detailed information for a specific investment position
   */
  async getPositionDetails(id: string): Promise<InvestmentPositionDetail> {
    const response = await apiClient.get(`/investment/${id}/details`);
    return response.data;
  },

  /**
   * Withdraw (close) an investment position
   */
  async withdrawInvestment(id: string): Promise<WithdrawInvestmentResponse> {
    const response = await apiClient.post(`/investment/${id}/withdraw`, {});
    return response.data;
  },

  // ============================================================================
  // UTILITY FUNCTIONS
  // ============================================================================

  /**
   * Calculate investment projections locally (client-side)
   * Formula: Daily Reward = Principal × (APY / 365 / 100)
   */
  calculateProjections(amount: number, apy: number): InvestmentCalculation {
    const dailyReward = amount * (apy / 365 / 100);
    const monthlyReward = dailyReward * 30;
    const yearlyReward = dailyReward * 365;

    return {
      amount,
      apy,
      dailyReward: parseFloat(dailyReward.toFixed(8)),
      monthlyReward: parseFloat(monthlyReward.toFixed(8)),
      yearlyReward: parseFloat(yearlyReward.toFixed(8)),
      totalReturn30Days: parseFloat((amount + monthlyReward).toFixed(8)),
      totalReturn365Days: parseFloat((amount + yearlyReward).toFixed(8))
    };
  },

  /**
   * Format currency with proper decimal places
   */
  formatCurrency(amount: number, decimals: number = 2): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals
    }).format(amount);
  },

  /**
   * Format APY percentage
   */
  formatAPY(apy: number): string {
    return `${apy.toFixed(2)}%`;
  },

  /**
   * Calculate days between two dates
   */
  calculateDaysHeld(startDate: string): number {
    const start = new Date(startDate);
    const now = new Date();
    const diffTime = Math.abs(now.getTime() - start.getTime());
    const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }
};

export default investmentService;
