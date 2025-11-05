import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig, AxiosResponse } from 'axios';
import { env } from '@/config';

// Create axios instance with base configuration
const apiClient: AxiosInstance = axios.create({
  baseURL: `${env.apiBaseUrl}/api`,
  timeout: env.apiTimeout,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - Add JWT token to requests
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('auth_token');

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    if (env.enableLogging) {
      console.log('[API Request]', {
        method: config.method?.toUpperCase(),
        url: config.url,
        data: config.data,
      });
    }

    return config;
  },
  (error: AxiosError) => {
    if (env.enableLogging) {
      console.error('[API Request Error]', error);
    }
    return Promise.reject(error);
  }
);

// Response interceptor - Handle errors globally
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    if (env.enableLogging) {
      console.log('[API Response]', {
        status: response.status,
        url: response.config.url,
        data: response.data,
      });
    }
    return response;
  },
  async (error: AxiosError) => {
    if (env.enableLogging) {
      console.error('[API Response Error]', {
        status: error.response?.status,
        url: error.config?.url,
        message: error.message,
        data: error.response?.data,
      });
    }

    // Handle specific error cases
    if (error.response) {
      const status = error.response.status;

      switch (status) {
        case 401:
          // Unauthorized - Clear token and redirect to login
          localStorage.removeItem('auth_token');
          localStorage.removeItem('auth_user');

          // Only redirect if not already on login page
          if (!window.location.pathname.includes('/login')) {
            window.location.href = '/login';
          }
          break;

        case 403:
          // Forbidden - Access denied
          break;

        case 404:
          // Resource not found
          break;

        case 500:
        case 502:
        case 503:
          // Server errors
          break;

        default:
          break;
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
