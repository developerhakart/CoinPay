// Supported tokens for swap on Polygon Amoy Testnet
export interface Token {
  address: string;
  symbol: string;
  name: string;
  decimals: number;
  logoUrl: string;
}

export const SUPPORTED_TOKENS: Record<string, Token> = {
  USDC: {
    address: '0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582',
    symbol: 'USDC',
    name: 'USD Coin',
    decimals: 6,
    logoUrl: '/tokens/usdc.svg',
  },
  WETH: {
    address: '0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9',
    symbol: 'WETH',
    name: 'Wrapped Ether',
    decimals: 18,
    logoUrl: '/tokens/eth.svg',
  },
  WMATIC: {
    address: '0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270',
    symbol: 'WMATIC',
    name: 'Wrapped Matic',
    decimals: 18,
    logoUrl: '/tokens/matic.svg',
  },
};

export const SUPPORTED_TOKENS_ARRAY = Object.values(SUPPORTED_TOKENS);

// Helper function to get token by address
export const getTokenByAddress = (address: string): Token | undefined => {
  return SUPPORTED_TOKENS_ARRAY.find(
    (token) => token.address.toLowerCase() === address.toLowerCase()
  );
};

// Helper function to get token by symbol
export const getTokenBySymbol = (symbol: string): Token | undefined => {
  return SUPPORTED_TOKENS[symbol.toUpperCase()];
};
