export interface SwapQuote {
  fromToken: string;
  toToken: string;
  fromTokenSymbol: string;
  toTokenSymbol: string;
  fromAmount: number;
  toAmount: number;
  exchangeRate: number;
  slippageTolerance: number;
  minimumReceived: number;
  platformFee: number;
  platformFeePercentage: number;
  estimatedGasCost: string;
  priceImpact: number;
  provider: string;
  quoteValidUntil: string;
  route?: string[];
}

export interface SwapExecutionRequest {
  fromToken: string;
  toToken: string;
  fromAmount: number;
  slippageTolerance: number;
}

export interface SwapExecutionResponse {
  swapId: string;
  transactionHash?: string;
  status: SwapStatus;
  fromToken: string;
  toToken: string;
  fromAmount: number;
  toAmount: number;
  createdAt: string;
}

export interface SwapDetails {
  id: string;
  userId: string;
  fromToken: string;
  toToken: string;
  fromTokenSymbol: string;
  toTokenSymbol: string;
  fromAmount: number;
  toAmount: number;
  exchangeRate: number;
  slippageTolerance: number;
  minimumReceived: number;
  platformFee: number;
  estimatedGasCost: string;
  actualGasUsed?: string;
  priceImpact: number;
  status: SwapStatus;
  transactionHash?: string;
  errorMessage?: string;
  createdAt: string;
  confirmedAt?: string;
  failedAt?: string;
}

export type SwapStatus = 'pending' | 'confirmed' | 'failed';

export interface SwapHistoryFilters {
  status?: SwapStatus | 'all';
  page?: number;
  pageSize?: number;
}

export interface SwapHistoryResponse {
  swaps: SwapDetails[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface TokenBalance {
  tokenAddress: string;
  symbol: string;
  balance: number;
  decimals: number;
}

export interface SlippagePreset {
  label: string;
  value: number;
}

export const SLIPPAGE_PRESETS: SlippagePreset[] = [
  { label: '0.5%', value: 0.5 },
  { label: '1%', value: 1.0 },
  { label: '3%', value: 3.0 },
];

export const DEFAULT_SLIPPAGE = 1.0;
export const MIN_SLIPPAGE = 0.1;
export const MAX_SLIPPAGE = 50;
export const HIGH_SLIPPAGE_THRESHOLD = 5;

export interface PriceImpactLevel {
  level: 'low' | 'medium' | 'high';
  color: 'green' | 'yellow' | 'red';
  threshold: number;
}

export const PRICE_IMPACT_LEVELS: PriceImpactLevel[] = [
  { level: 'low', color: 'green', threshold: 1 },
  { level: 'medium', color: 'yellow', threshold: 3 },
  { level: 'high', color: 'red', threshold: Infinity },
];
