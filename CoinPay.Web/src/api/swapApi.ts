import apiClient from '@/services/apiClient';
import {
  SwapQuote,
  SwapExecutionRequest,
  SwapExecutionResponse,
  SwapDetails,
  SwapHistoryResponse,
  SwapHistoryFilters,
  TokenBalance,
} from '@/types/swap';

/**
 * Get a swap quote with pricing and fee information
 */
export const getSwapQuote = async (
  fromToken: string,
  toToken: string,
  amount: number,
  slippage: number
): Promise<SwapQuote> => {
  const response = await apiClient.get<SwapQuote>('/swap/quote', {
    params: {
      fromToken,
      toToken,
      amount,
      slippage,
    },
  });
  return response.data;
};

/**
 * Execute a token swap
 */
export const executeSwap = async (
  request: SwapExecutionRequest
): Promise<SwapExecutionResponse> => {
  const response = await apiClient.post<SwapExecutionResponse>(
    '/swap/execute',
    request
  );
  return response.data;
};

/**
 * Get swap history with optional filters
 */
export const getSwapHistory = async (
  filters: SwapHistoryFilters = {}
): Promise<SwapHistoryResponse> => {
  const { status = 'all', page = 1, pageSize = 20 } = filters;

  const params: Record<string, string | number> = {
    page,
    pageSize,
  };

  if (status !== 'all') {
    params.status = status;
  }

  const response = await apiClient.get<SwapHistoryResponse>('/swap/history', {
    params,
  });
  return response.data;
};

/**
 * Get detailed information about a specific swap
 */
export const getSwapDetails = async (swapId: string): Promise<SwapDetails> => {
  const response = await apiClient.get<SwapDetails>(`/swap/${swapId}/details`);
  return response.data;
};

/**
 * Get token balances for the current user
 */
export const getTokenBalances = async (
  walletAddress: string
): Promise<TokenBalance[]> => {
  const response = await apiClient.get<{ balances: TokenBalance[] }>(
    `/wallet/${walletAddress}/balances`
  );
  return response.data.balances;
};
