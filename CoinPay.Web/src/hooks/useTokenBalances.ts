import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/store/authStore';
import { getTokenBalances } from '@/api/swapApi';
import { TokenBalance } from '@/types/swap';

/**
 * Hook to fetch and manage token balances for the current user
 * Auto-refreshes every 30 seconds
 */
export const useTokenBalances = () => {
  const { user } = useAuthStore();
  const walletAddress = user?.walletAddress;

  return useQuery({
    queryKey: ['tokenBalances', walletAddress],
    queryFn: async () => {
      if (!walletAddress) {
        return new Map<string, number>();
      }

      const balances = await getTokenBalances(walletAddress);

      // Convert array to Map for easier lookup
      const balanceMap = new Map<string, number>();
      balances.forEach((balance: TokenBalance) => {
        balanceMap.set(balance.tokenAddress.toLowerCase(), balance.balance);
      });

      return balanceMap;
    },
    enabled: !!walletAddress,
    refetchInterval: 30000, // Refresh every 30 seconds
    staleTime: 10000, // Consider data stale after 10 seconds
  });
};
