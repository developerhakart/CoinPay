import apiClient from './apiClient';
import { Wallet, WalletBalance, CreateWalletRequest } from '@/types';

export const walletService = {
  // Create a new wallet
  async createWallet(data: CreateWalletRequest): Promise<Wallet> {
    const response = await apiClient.post('/wallet', data);
    return response.data;
  },

  // Get wallet by user ID
  async getWalletByUserId(userId: number): Promise<Wallet> {
    const response = await apiClient.get(`/wallet/user/${userId}`);
    return response.data;
  },

  // Get wallet balance
  async getBalance(walletAddress: string): Promise<WalletBalance> {
    const response = await apiClient.get(`/wallet/${walletAddress}/balance`);
    return response.data;
  },

  // Get wallet by address
  async getWalletByAddress(address: string): Promise<Wallet> {
    const response = await apiClient.get(`/wallet/${address}`);
    return response.data;
  },
};
