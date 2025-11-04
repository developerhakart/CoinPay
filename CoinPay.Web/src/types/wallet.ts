export interface Wallet {
  id: string;
  userId: number;
  walletAddress: string;
  circleWalletId?: string;
  balance: number;
  currency: string;
  createdAt: string;
  updatedAt: string;
}

export interface WalletBalance {
  walletAddress: string;
  balance: number;
  nativeBalance: number; // POL/MATIC balance
  currency: string;
}

export interface CreateWalletRequest {
  userId: number;
  currency?: string;
}
