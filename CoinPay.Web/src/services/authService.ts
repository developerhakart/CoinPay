import apiClient from './apiClient';
import { AuthResponse, User } from '@/types';

export const authService = {
  // Check if username exists
  async checkUsername(username: string): Promise<{ exists: boolean }> {
    const response = await apiClient.post('/auth/check-username', { username });
    return response.data;
  },

  // Register - Initiate
  async registerInitiate(username: string): Promise<{
    challengeId: string;
    challenge: string;
    userId: number;
  }> {
    const response = await apiClient.post('/auth/register/initiate', { username });
    return response.data;
  },

  // Register - Complete
  async registerComplete(data: {
    username: string;
    challengeId: string;
    credentialId: string;
    publicKey: string;
    authenticatorData: string;
  }): Promise<{ message: string }> {
    const response = await apiClient.post('/auth/register/complete', data);
    return response.data;
  },

  // Login - Initiate
  async loginInitiate(username: string): Promise<{
    challengeId: string;
    challenge: string;
  }> {
    const response = await apiClient.post('/auth/login/initiate', { username });
    return response.data;
  },

  // Login - Complete
  async loginComplete(data: {
    username: string;
    challengeId: string;
    credentialId: string;
    authenticatorData: string;
    signature: string;
  }): Promise<AuthResponse> {
    const response = await apiClient.post('/auth/login/complete', data);
    return response.data;
  },

  // Get current user profile
  async getProfile(): Promise<User> {
    const response = await apiClient.get('/me');
    return response.data;
  },

  // Logout
  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
  },
};
