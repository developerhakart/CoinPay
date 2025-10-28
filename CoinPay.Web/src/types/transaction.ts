export interface Transaction {
  id: number;
  transactionId?: string;
  amount: number;
  currency: string;
  type: string; // Payment, Refund, Transfer
  status: string; // Pending, Completed, Failed
  senderName?: string;
  receiverName?: string;
  description?: string;
  createdAt: string;
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
  amount: number;
  currency?: string;
  type?: string; // Payment, Refund, Transfer
  status?: string; // Pending, Completed, Failed
  senderName?: string;
  receiverName?: string;
  description?: string;
}

export interface TransactionResponse {
  transaction: Transaction;
  message?: string;
}
