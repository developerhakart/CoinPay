export interface Transaction {
  id: number;
  transactionId?: string;
  fromAddress: string;
  toAddress: string;
  amount: number;
  currency: string;
  status: TransactionStatus;
  transactionHash?: string;
  circleTransferId?: string;
  description?: string;
  createdAt: string;
  updatedAt?: string;
  completedAt?: string;
}

export enum TransactionStatus {
  Pending = 'pending',
  Processing = 'processing',
  Completed = 'completed',
  Failed = 'failed',
  Cancelled = 'cancelled',
}

export interface CreateTransactionRequest {
  fromAddress: string;
  toAddress: string;
  amount: number;
  currency?: string;
  description?: string;
}

export interface TransactionResponse {
  transaction: Transaction;
  message?: string;
}
