export interface User {
  id: number;
  username: string;
  email?: string;
  walletAddress?: string;
  circleUserId?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  walletAddress?: string;
  user?: User;
}

export interface LoginRequest {
  username: string;
  password?: string;
}

export interface RegisterRequest {
  username: string;
  email?: string;
  password?: string;
}
