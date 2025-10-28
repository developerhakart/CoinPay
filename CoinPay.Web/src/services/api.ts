import type { Transaction, CreateTransactionRequest } from '../types/transaction';
import apiClient from './apiClient';

export const transactionApi = {
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

  // Create a new transaction
  async create(transaction: CreateTransactionRequest): Promise<Transaction> {
    const response = await apiClient.post('/transactions', transaction);
    return response.data;
  },

  // Update transaction status
  async updateStatus(id: number, status: string): Promise<void> {
    await apiClient.patch(`/transactions/${id}/status?status=${status}`);
  },

  // Delete transaction
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/transactions/${id}`);
  },
};
