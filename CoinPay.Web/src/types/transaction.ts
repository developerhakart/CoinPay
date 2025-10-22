export interface Transaction {
  id: number;
  transactionId: string;
  amount: number;
  currency: string;
  type: 'Payment' | 'Transfer' | 'Refund';
  status: 'Pending' | 'Completed' | 'Failed';
  senderName: string;
  receiverName: string;
  description: string;
  createdAt: string;
  completedAt?: string;
}

export interface CreateTransactionRequest {
  amount: number;
  currency: string;
  type: 'Payment' | 'Transfer' | 'Refund';
  status: 'Pending' | 'Completed' | 'Failed';
  senderName: string;
  receiverName: string;
  description: string;
}
