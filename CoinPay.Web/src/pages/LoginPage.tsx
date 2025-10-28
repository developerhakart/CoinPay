import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authService } from '@/services';
import { useAuthStore } from '@/store';

export function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const { login } = useAuthStore();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      // If password is empty or username is testuser, use dev login
      if (!password || username.toLowerCase() === 'testuser') {
        const response = await authService.loginDev(username);

        // Create user object from response
        const user = {
          id: 0, // Will be extracted from token
          username: response.username,
          walletAddress: response.walletAddress || undefined,
        };

        login(user, response.token);
        navigate('/dashboard');
      } else {
        // Regular passkey-based login flow would go here
        throw new Error('Passkey login not yet implemented. Use testuser or leave password empty for dev login.');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md w-full bg-white rounded-lg shadow-lg p-8">
        <h2 className="text-3xl font-bold text-center text-gray-900 mb-2">
          Sign In to CoinPay
        </h2>

        <p className="text-center text-sm text-gray-600 mb-6">
          💡 For testing: Use <strong>testuser</strong> or leave password empty
        </p>

        {error && (
          <div className="mb-4 p-3 bg-danger-50 border border-danger-200 text-danger-700 rounded-md">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="username" className="block text-sm font-medium text-gray-700 mb-2">
              Username
            </label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              placeholder="Enter your username"
            />
          </div>

          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-2">
              Password <span className="text-gray-400 font-normal">(optional for dev mode)</span>
            </label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              placeholder="Leave empty for dev login"
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="w-full bg-primary-500 text-white py-3 rounded-md font-semibold hover:bg-primary-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isLoading ? 'Signing in...' : 'Sign In'}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-gray-600">
          Don't have an account?{' '}
          <Link to="/register" className="text-primary-500 hover:text-primary-600 font-semibold">
            Sign up
          </Link>
        </p>
      </div>
    </div>
  );
}
