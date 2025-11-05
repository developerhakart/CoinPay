import { useState, useMemo } from 'react';
import { Link } from 'react-router-dom';
import { FAQSection, FAQSearch, CategoryNav, type FAQItem } from '@/components/help/FAQSection';

const faqData: FAQItem[] = [
  // Getting Started
  {
    category: 'getting-started',
    question: 'What is CoinPay and how does it work?',
    answer: 'CoinPay is a cryptocurrency payment platform that enables you to send, receive, swap, and invest in digital assets. The platform operates on the Polygon Amoy testnet, allowing you to manage USDC and other cryptocurrencies securely. Your wallet is automatically created when you register, and you can start transacting immediately after funding your account.'
  },
  {
    category: 'getting-started',
    question: 'How do I get started with CoinPay?',
    answer: 'To get started:\n1. Create an account by clicking "Register" and providing your details\n2. Your wallet will be automatically created\n3. Copy your wallet address from the dashboard\n4. Get testnet tokens from the provided faucets\n5. Start sending, receiving, or swapping tokens'
  },
  {
    category: 'getting-started',
    question: 'How do I fund my wallet with testnet tokens?',
    answer: 'You can fund your wallet using these testnet faucets:\n\n1. Polygon Amoy MATIC: Visit faucet.polygon.technology and paste your wallet address\n2. Circle USDC: Visit faucet.circle.com and request testnet USDC\n\nNote: These are testnet tokens with no real value, used for testing purposes only.'
  },
  {
    category: 'getting-started',
    question: 'Is my wallet secure?',
    answer: 'Yes, CoinPay implements industry-standard security practices:\n- Encrypted private key storage\n- Secure authentication\n- HTTPS encryption for all communications\n- Regular security audits\n\nHowever, always remember to:\n- Never share your password or private keys\n- Use strong, unique passwords\n- Be cautious of phishing attempts'
  },

  // Send & Receive
  {
    category: 'send-receive',
    question: 'How do I send cryptocurrency to someone?',
    answer: 'To send crypto:\n1. Go to the "Transfer" or "Wallet" page\n2. Enter the recipient\'s wallet address\n3. Enter the amount you want to send\n4. Review the transaction details and gas fees\n5. Click "Send" to confirm\n6. Wait for blockchain confirmation (usually 1-2 minutes)\n\nAlways double-check the recipient address before sending, as transactions are irreversible.'
  },
  {
    category: 'send-receive',
    question: 'How do I receive cryptocurrency?',
    answer: 'To receive crypto:\n1. Go to your "Wallet" page\n2. Find your wallet address displayed at the top\n3. Click "Copy Address" or show the QR code\n4. Share your address or QR code with the sender\n5. Wait for the transaction to confirm on the blockchain\n\nYour balance will update automatically once the transaction is confirmed.'
  },
  {
    category: 'send-receive',
    question: 'What are gas fees and why do I need to pay them?',
    answer: 'Gas fees are transaction costs paid to blockchain validators for processing your transaction. These fees:\n- Vary based on network congestion\n- Are paid in the native blockchain token (MATIC for Polygon)\n- Are typically very low on Polygon (fractions of a cent)\n- Cannot be refunded even if a transaction fails\n\nMake sure you have enough native tokens to cover gas fees before sending transactions.'
  },
  {
    category: 'send-receive',
    question: 'How long does a transaction take?',
    answer: 'Transaction times vary by blockchain:\n- Polygon Amoy testnet: 1-2 minutes typically\n- Most transactions confirm within 2-5 minutes\n- During high network congestion, it may take longer\n\nYou can track your transaction status on the "Transactions" page or using the transaction hash on a blockchain explorer.'
  },
  {
    category: 'send-receive',
    question: 'Can I cancel a transaction after sending it?',
    answer: 'No, cryptocurrency transactions cannot be canceled once submitted to the blockchain. This is a fundamental feature of blockchain technology that ensures security and immutability.\n\nTo avoid mistakes:\n- Always verify the recipient address\n- Double-check the amount\n- Review all transaction details before confirming\n- Start with a small test transaction for new recipients'
  },

  // Swap Tokens
  {
    category: 'swap',
    question: 'How does the token swap feature work?',
    answer: 'The swap feature allows you to exchange one cryptocurrency for another:\n1. Select the token you want to swap from\n2. Select the token you want to receive\n3. Enter the amount\n4. Review the exchange rate and fees\n5. Confirm the swap\n\nThe swap executes at the current market rate plus a small fee. Rates are locked for 30 seconds after quote generation.'
  },
  {
    category: 'swap',
    question: 'What is slippage and how do I set it?',
    answer: 'Slippage is the difference between the expected price and the actual execution price of a swap. It occurs due to market volatility.\n\nSlippage settings:\n- Low slippage (0.1-0.5%): May fail during volatile markets\n- Medium slippage (0.5-1%): Balanced for most swaps\n- High slippage (1-3%): Ensures execution but may cost more\n\nYou can adjust slippage in the swap settings. Higher slippage means your transaction is more likely to succeed but you might get a less favorable rate.'
  },
  {
    category: 'swap',
    question: 'What fees are charged for swaps?',
    answer: 'Swap fees include:\n- Platform fee: 0.3% of the swap amount\n- Gas fee: Blockchain transaction cost\n- Price impact: Market price movement for large swaps\n\nAll fees are displayed before you confirm the swap. The final amount you receive is shown after all fees are deducted.'
  },
  {
    category: 'swap',
    question: 'Why did my swap fail?',
    answer: 'Swaps can fail for several reasons:\n- Insufficient balance for the swap amount plus gas fees\n- Slippage tolerance too low during price volatility\n- Token price moved beyond acceptable range\n- Network congestion or RPC errors\n\nIf a swap fails:\n- Gas fees are still consumed (this is a blockchain requirement)\n- Your tokens remain in your wallet\n- You can retry with adjusted settings'
  },

  // Investments
  {
    category: 'investments',
    question: 'What are investment strategies in CoinPay?',
    answer: 'CoinPay offers automated investment strategies:\n- Dollar-Cost Averaging (DCA): Regular purchases at set intervals\n- Grid Trading: Buy low, sell high automatically\n- Portfolio Rebalancing: Maintain target asset allocations\n\nStrategies connect to WhiteBIT exchange for execution. You maintain custody while the platform executes trades according to your strategy parameters.'
  },
  {
    category: 'investments',
    question: 'How do I connect my WhiteBIT account?',
    answer: 'To connect WhiteBIT:\n1. Log in to your WhiteBIT account\n2. Generate API keys (read + trade permissions)\n3. Go to "Investment" in CoinPay\n4. Click "Connect WhiteBIT"\n5. Enter your API key and secret\n6. Save and verify the connection\n\nNever share your API keys with anyone. CoinPay cannot withdraw funds from your exchange account.'
  },
  {
    category: 'investments',
    question: 'What are the risks of automated investment strategies?',
    answer: 'Investment strategies carry risks:\n- Market risk: Crypto prices can be highly volatile\n- Strategy risk: No guarantee of profits\n- API risk: Connection issues may delay trades\n- Exchange risk: Relies on third-party exchange availability\n\nInvest only what you can afford to lose. Past performance doesn\'t guarantee future results. Consider starting with small amounts to test strategies.'
  },
  {
    category: 'investments',
    question: 'Can I stop or modify my investment strategy?',
    answer: 'Yes, you have full control:\n- Pause/resume strategies anytime\n- Modify parameters (amount, frequency, targets)\n- Close positions and withdraw funds\n- Disconnect exchange at any time\n\nChanges take effect immediately. Ongoing trades will complete, then new parameters apply.'
  },

  // Security
  {
    category: 'security',
    question: 'How do I keep my account secure?',
    answer: 'Follow these security best practices:\n1. Use a strong, unique password\n2. Never share your password or private keys\n3. Enable 2FA when available\n4. Verify URLs before logging in\n5. Use secure, updated devices\n6. Be wary of phishing emails and messages\n7. Regularly review your transaction history\n\nCoinPay will never ask for your password or private keys via email or chat.'
  },
  {
    category: 'security',
    question: 'What should I do if I suspect unauthorized access?',
    answer: 'If you suspect your account is compromised:\n1. Change your password immediately\n2. Review recent transactions\n3. Disconnect any exchange connections\n4. Contact CoinPay support\n5. Check for unfamiliar devices or sessions\n\nIf funds were stolen, report to:\n- CoinPay support team\n- Local law enforcement\n- Relevant exchange platforms'
  },
  {
    category: 'security',
    question: 'Are my funds insured?',
    answer: 'CoinPay is a testnet platform for demonstration purposes. Real funds should not be sent to testnet addresses.\n\nFor production deployments:\n- Smart contract security audits are recommended\n- Consider third-party insurance options\n- Maintain secure key management practices\n- Understand that blockchain transactions are irreversible'
  },
  {
    category: 'security',
    question: 'How does CoinPay protect my private keys?',
    answer: 'CoinPay implements multiple security layers:\n- Private keys are encrypted at rest\n- Secure key derivation algorithms\n- No keys are transmitted to frontend\n- Server-side encryption and protection\n- Regular security audits\n\nHowever, as with all custodial solutions, you trust CoinPay to secure your keys. For maximum security, consider using your own wallet with CoinPay integration.'
  },

  // Troubleshooting
  {
    category: 'troubleshooting',
    question: 'Why isn\'t my balance updating?',
    answer: 'If your balance isn\'t updating:\n1. Click the refresh button on the wallet page\n2. Check if the transaction confirmed on blockchain\n3. Clear browser cache and reload\n4. Verify you\'re looking at the correct network (Polygon Amoy)\n5. Wait a few minutes for blockchain confirmation\n\nIf the issue persists, contact support with your wallet address and transaction hash.'
  },
  {
    category: 'troubleshooting',
    question: 'I\'m getting "Insufficient funds" error. What should I do?',
    answer: 'This error means you don\'t have enough tokens:\n- Check you have enough of the token you\'re trying to send\n- Ensure you have native tokens (MATIC) for gas fees\n- Account for the total: amount + fees\n\nGet more testnet tokens from faucets or reduce your transaction amount.'
  },
  {
    category: 'troubleshooting',
    question: 'The transaction is stuck as "Pending". What should I do?',
    answer: 'Pending transactions can occur due to:\n- Network congestion\n- Gas price too low\n- RPC connection issues\n\nTo resolve:\n1. Wait 10-15 minutes (most resolve automatically)\n2. Check blockchain explorer for transaction status\n3. Try refreshing the page\n4. If stuck for >30 minutes, contact support\n\nNote: Transactions cannot be canceled once submitted.'
  },
  {
    category: 'troubleshooting',
    question: 'I forgot my password. How can I recover my account?',
    answer: 'To recover your account:\n1. Click "Forgot Password" on the login page\n2. Enter your registered email address\n3. Check your email for recovery instructions\n4. Follow the link to reset your password\n5. Create a new strong password\n\nIf you don\'t receive the email:\n- Check spam/junk folders\n- Verify you used the correct email\n- Wait a few minutes and try again\n- Contact support if issues persist'
  },
  {
    category: 'troubleshooting',
    question: 'The website is loading slowly or timing out. What can I do?',
    answer: 'Try these troubleshooting steps:\n1. Refresh the page (Ctrl+R or Cmd+R)\n2. Clear browser cache and cookies\n3. Try a different browser\n4. Check your internet connection\n5. Disable browser extensions temporarily\n6. Try incognito/private mode\n\nIf problems persist, the platform may be experiencing high traffic or maintenance. Check our status page or social media for updates.'
  }
];

const categories = [
  {
    id: 'all',
    name: 'All Topics',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
      </svg>
    )
  },
  {
    id: 'getting-started',
    name: 'Getting Started',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
      </svg>
    )
  },
  {
    id: 'send-receive',
    name: 'Send & Receive',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
      </svg>
    )
  },
  {
    id: 'swap',
    name: 'Swap Tokens',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4" />
      </svg>
    )
  },
  {
    id: 'investments',
    name: 'Investments',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
      </svg>
    )
  },
  {
    id: 'security',
    name: 'Security',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
      </svg>
    )
  },
  {
    id: 'troubleshooting',
    name: 'Troubleshooting',
    icon: (
      <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
      </svg>
    )
  }
];

export function HelpPage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeCategory, setActiveCategory] = useState('all');

  const filteredFAQs = useMemo(() => {
    let filtered = faqData;

    if (activeCategory !== 'all') {
      filtered = filtered.filter(faq => faq.category === activeCategory);
    }

    if (searchQuery.trim()) {
      const query = searchQuery.toLowerCase();
      filtered = filtered.filter(
        faq =>
          faq.question.toLowerCase().includes(query) ||
          faq.answer.toLowerCase().includes(query)
      );
    }

    return filtered;
  }, [searchQuery, activeCategory]);

  const groupedFAQs = useMemo(() => {
    const groups: Record<string, FAQItem[]> = {};
    filteredFAQs.forEach(faq => {
      const category = faq.category || 'other';
      if (!groups[category]) {
        groups[category] = [];
      }
      groups[category].push(faq);
    });
    return groups;
  }, [filteredFAQs]);

  const getCategoryInfo = (categoryId: string) => {
    return categories.find(cat => cat.id === categoryId);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm">
        <div className="container mx-auto px-4 py-4">
          <Link to="/dashboard" className="inline-flex items-center text-primary-600 hover:text-primary-700 font-medium transition-colors">
            <svg className="w-5 h-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            Back to Dashboard
          </Link>
        </div>
      </header>

      {/* Hero Section */}
      <div className="bg-gradient-to-br from-primary-500 to-primary-600 text-white py-12">
        <div className="container mx-auto px-4 max-w-4xl text-center">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-white/20 rounded-full mb-4">
            <svg className="w-8 h-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h1 className="text-4xl font-bold mb-4">How can we help you?</h1>
          <p className="text-xl text-primary-100 mb-8">
            Find answers to common questions and learn how to use CoinPay
          </p>
          <div className="max-w-2xl mx-auto">
            <FAQSearch
              value={searchQuery}
              onChange={setSearchQuery}
              placeholder="Search for help..."
            />
          </div>
        </div>
      </div>

      {/* Main Content */}
      <main className="container mx-auto px-4 py-8 max-w-4xl">
        {/* Category Navigation */}
        <div className="mb-8">
          <CategoryNav
            categories={categories}
            activeCategory={activeCategory}
            onCategoryClick={setActiveCategory}
          />
        </div>

        {/* Results Count */}
        {searchQuery && (
          <div className="mb-6">
            <p className="text-gray-600">
              Found <strong>{filteredFAQs.length}</strong> result{filteredFAQs.length !== 1 ? 's' : ''} for "{searchQuery}"
            </p>
          </div>
        )}

        {/* FAQ Sections */}
        {filteredFAQs.length > 0 ? (
          <div>
            {activeCategory === 'all' ? (
              Object.entries(groupedFAQs).map(([categoryId, faqs]) => {
                const categoryInfo = getCategoryInfo(categoryId);
                return (
                  <FAQSection
                    key={categoryId}
                    title={categoryInfo?.name || categoryId}
                    faqs={faqs}
                    icon={
                      categoryInfo?.icon ? (
                        <div className="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center">
                          {categoryInfo.icon}
                        </div>
                      ) : undefined
                    }
                  />
                );
              })
            ) : (
              <FAQSection
                title={getCategoryInfo(activeCategory)?.name || ''}
                faqs={filteredFAQs}
              />
            )}
          </div>
        ) : (
          <div className="text-center py-12">
            <svg className="w-16 h-16 mx-auto text-gray-300 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">No results found</h3>
            <p className="text-gray-600 mb-6">
              We couldn't find any articles matching your search. Try different keywords or browse by category.
            </p>
            <button
              onClick={() => {
                setSearchQuery('');
                setActiveCategory('all');
              }}
              className="text-primary-600 hover:text-primary-700 font-medium"
            >
              Clear search and show all
            </button>
          </div>
        )}

        {/* Contact Support Section */}
        <div className="mt-12 bg-gradient-to-r from-primary-50 to-accent-50 rounded-xl p-8 border border-primary-100">
          <div className="text-center">
            <h2 className="text-2xl font-bold text-gray-900 mb-3">Still need help?</h2>
            <p className="text-gray-600 mb-6">
              Can't find what you're looking for? Our support team is here to help.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <a
                href="mailto:support@coinpay.example.com"
                className="inline-flex items-center justify-center px-6 py-3 bg-primary-500 text-white font-semibold rounded-lg hover:bg-primary-600 transition-colors"
              >
                <svg className="w-5 h-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
                Email Support
              </a>
              <Link
                to="/dashboard"
                className="inline-flex items-center justify-center px-6 py-3 bg-white text-primary-600 font-semibold rounded-lg border-2 border-primary-500 hover:bg-primary-50 transition-colors"
              >
                <svg className="w-5 h-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                </svg>
                Back to Dashboard
              </Link>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
