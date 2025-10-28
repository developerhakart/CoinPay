import apiClient from './apiClient';
import { Transaction, CreateTransactionRequest, TransactionResponse } from '@/types';

export const transactionService = {
  // Get all transactions
  async getAll(): Promise<Transaction[]> {
    const response = await apiClient.get('/transactions');
    return response.data;
  },

  // Get transaction by ID
  async getById(id: number): Promise<Transaction> {
    const response = await apiClient.get(`/transactions/${id}`);
    return response.data;
  },

  // Get transactions by status
  async getByStatus(status: string): Promise<Transaction[]> {
    const response = await apiClient.get(`/transactions/status/${status}`);
    return response.data;
  },

  // Create a new transaction (transfer)
  async create(data: CreateTransactionRequest): Promise<TransactionResponse> {
    const response = await apiClient.post('/transactions', data);
    return response.data;
  },

  // Update transaction status
  async updateStatus(id: number, status: string): Promise<void> {
    await apiClient.patch(`/transactions/${id}/status`, null, {
      params: { status },
    });
  },

  // Delete transaction
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/transactions/${id}`);
  },

  // Get user's transaction history
  async getUserTransactions(userId: number): Promise<Transaction[]> {
    const response = await apiClient.get(`/transactions/user/${userId}`);
    return response.data;
  },
};
