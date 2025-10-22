import type { Transaction, CreateTransactionRequest } from '../types/transaction';

const API_BASE_URL = 'http://localhost:5000/api';

export const transactionApi = {
  // Get all transactions
  async getAll(): Promise<Transaction[]> {
    const response = await fetch(`${API_BASE_URL}/transactions`);
    if (!response.ok) {
      throw new Error('Failed to fetch transactions');
    }
    return response.json();
  },

  // Get transaction by ID
  async getById(id: number): Promise<Transaction> {
    const response = await fetch(`${API_BASE_URL}/transactions/${id}`);
    if (!response.ok) {
      throw new Error('Failed to fetch transaction');
    }
    return response.json();
  },

  // Get transactions by status
  async getByStatus(status: string): Promise<Transaction[]> {
    const response = await fetch(`${API_BASE_URL}/transactions/status/${status}`);
    if (!response.ok) {
      throw new Error('Failed to fetch transactions by status');
    }
    return response.json();
  },

  // Create a new transaction
  async create(transaction: CreateTransactionRequest): Promise<Transaction> {
    const response = await fetch(`${API_BASE_URL}/transactions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(transaction),
    });
    if (!response.ok) {
      throw new Error('Failed to create transaction');
    }
    return response.json();
  },

  // Update transaction status
  async updateStatus(id: number, status: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/transactions/${id}/status?status=${status}`, {
      method: 'PATCH',
    });
    if (!response.ok) {
      throw new Error('Failed to update transaction status');
    }
  },

  // Delete transaction
  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/transactions/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) {
      throw new Error('Failed to delete transaction');
    }
  },
};
