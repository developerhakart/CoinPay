/**
 * FAQ Data for CoinPay Help Center
 * Contains comprehensive FAQs organized by category
 */

export interface FAQ {
  id: string;
  category: string;
  question: string;
  answer: string;
  tags: string[];
}

export const faqData: FAQ[] = [
  // ============ GETTING STARTED (5 FAQs) ============
  {
    id: 'gs-001',
    category: 'getting-started',
    question: 'What is CoinPay and how does it work?',
    answer: `CoinPay is a comprehensive cryptocurrency payment platform that enables you to send, receive, swap, and invest in digital assets.

The platform operates on the Polygon Amoy testnet, allowing you to manage USDC and other cryptocurrencies securely. Your wallet is automatically created when you register, and you can start transacting immediately after funding your account.

Key Features:
- Non-custodial wallet management
- Instant token swaps
- Automated investment strategies
- Real-time transaction tracking
- Secure authentication`,
    tags: ['platform', 'overview', 'getting-started'],
  },
  {
    id: 'gs-002',
    category: 'getting-started',
    question: 'How do I get started with CoinPay?',
    answer: `Getting started with CoinPay is simple and straightforward:

1. Create an account by clicking "Register" and providing your details
2. Your wallet will be automatically created
3. Copy your wallet address from the dashboard
4. Get testnet tokens from the provided faucets
5. Start sending, receiving, or swapping tokens

The entire setup process takes less than 5 minutes. No additional verification is required for testnet usage.`,
    tags: ['setup', 'registration', 'onboarding'],
  },
  {
    id: 'gs-003',
    category: 'getting-started',
    question: 'How do I fund my wallet with testnet tokens?',
    answer: `You can fund your wallet using these testnet faucets:

Polygon MATIC:
- Visit faucet.polygon.technology
- Enter your wallet address
- Request testnet MATIC tokens
- Tokens arrive within 1-2 minutes

USDC Testnet:
- Visit faucet.circle.com
- Select Polygon Amoy network
- Enter your wallet address
- Request USDC tokens
- Instant receipt

Important Note: These are testnet tokens with no real monetary value. They are used exclusively for testing and development purposes.`,
    tags: ['funding', 'faucet', 'testnet', 'tokens'],
  },
  {
    id: 'gs-004',
    category: 'getting-started',
    question: 'Is my wallet secure on CoinPay?',
    answer: `Yes, CoinPay implements industry-standard security practices to protect your assets:

Security Measures:
- Encrypted private key storage with AES-256
- Secure authentication with password hashing
- HTTPS encryption for all communications
- Regular security audits and updates
- No private keys transmitted to frontend

Best Practices to Stay Secure:
- Never share your password or private keys
- Use strong, unique passwords (12+ characters)
- Be cautious of phishing attempts
- Don't access from public WiFi
- Keep your device software updated

Note: CoinPay is a testnet platform. For mainnet usage, consider hardware wallets for maximum security.`,
    tags: ['security', 'wallet', 'safety', 'encryption'],
  },
  {
    id: 'gs-005',
    category: 'getting-started',
    question: 'Can I use CoinPay on multiple devices?',
    answer: `Yes, you can access CoinPay from any device where you can log in with your credentials.

Important Considerations:
- Your wallet is accessed via secure login
- Each device will show your same wallet and balance
- You can log in from desktop, tablet, or mobile
- Sessions are secure and encrypted
- If you change password, all sessions remain valid

Device Recommendations:
- Use secure devices free from malware
- Log out on shared computers
- Enable browser security features
- Clear cached data periodically on shared devices
- Keep browser and OS updated`,
    tags: ['multi-device', 'access', 'security'],
  },

  // ============ SEND & RECEIVE (5 FAQs) ============
  {
    id: 'sr-001',
    category: 'send-receive',
    question: 'How do I send cryptocurrency to someone?',
    answer: `Sending crypto on CoinPay is simple and secure:

Step-by-Step Guide:
1. Go to the "Transfer" or "Wallet" page
2. Click "Send" button
3. Enter the recipient's wallet address
4. Select the token type (USDC, MATIC, etc.)
5. Enter the amount you want to send
6. Review transaction details and gas fees
7. Click "Confirm" to send
8. Wait for blockchain confirmation (usually 1-2 minutes)

Important Reminders:
- Always double-check the recipient address
- Transactions are irreversible once confirmed
- Ensure you have enough tokens + gas fees
- Copy-paste addresses to avoid typos
- Start with small amounts to verify addresses work`,
    tags: ['transfer', 'send', 'transaction'],
  },
  {
    id: 'sr-002',
    category: 'send-receive',
    question: 'How do I receive cryptocurrency?',
    answer: `Receiving crypto is even easier than sending:

Step-by-Step Guide:
1. Go to your "Wallet" page
2. Find your wallet address displayed prominently
3. Click "Copy Address" to copy to clipboard
4. Alternatively, display the QR code for scanning
5. Share your address or QR code with the sender
6. Wait for the transaction to confirm on blockchain

Transaction Status:
- "Pending": Waiting for blockchain confirmation
- "Confirmed": Transaction is complete
- Balance updates automatically once confirmed

Notes:
- You can receive funds while not logged in
- Your address is permanent and reusable
- No fees charged for receiving tokens
- Keep your address private (safer)`,
    tags: ['receive', 'address', 'wallet'],
  },
  {
    id: 'sr-003',
    category: 'send-receive',
    question: 'What are gas fees and why do I need to pay them?',
    answer: `Gas fees are transaction costs paid to blockchain validators for processing your transaction.

Why Do Gas Fees Exist?
- Compensate validators for computational work
- Prevent spam and abuse on the network
- Incentivize transaction processing
- Essential for blockchain security

Understanding Gas Fees on Polygon:
- Denominated in MATIC tokens
- Typically very low (fractions of a cent)
- Vary based on network congestion
- Cannot be refunded even if transaction fails
- Included in total transaction cost

Gas Estimation:
- CoinPay automatically estimates gas
- Based on current network conditions
- More complex operations = higher gas
- Shown before confirmation

Important Tips:
- Always ensure sufficient MATIC for gas
- Get MATIC from testnet faucet first
- Gas is consumed even if transaction fails
- Cannot negotiate or reduce gas prices`,
    tags: ['gas', 'fees', 'blockchain', 'costs'],
  },
  {
    id: 'sr-004',
    category: 'send-receive',
    question: 'How long does a transaction take?',
    answer: `Transaction timing depends on network conditions:

Typical Timeframes (Polygon Amoy):
- Average: 1-2 minutes
- Standard: 2-5 minutes
- During congestion: 5-15 minutes
- Rarely: 15-30 minutes

Transaction Status Tracking:
1. "Pending": Submitted to blockchain, waiting for inclusion
2. "Confirming": In a block, needs confirmations
3. "Confirmed": Finalized on blockchain
4. "Completed": All updates processed

How to Monitor Progress:
- Check "Transactions" page for live status
- Click transaction for detailed info
- View transaction hash on blockchain explorer
- Search hash at https://amoy.polygonscan.com

Factors Affecting Speed:
- Network congestion level
- Gas price offered
- Block time variations
- RPC node responsiveness

Note: Blockchain transactions are irreversible once confirmed, so timing won't affect security.`,
    tags: ['timing', 'confirmation', 'pending', 'blockchain'],
  },
  {
    id: 'sr-005',
    category: 'send-receive',
    question: 'Can I cancel or modify a transaction after sending it?',
    answer: `Unfortunately, cryptocurrency transactions cannot be canceled once submitted to the blockchain. This is a fundamental feature of blockchain technology that ensures security and immutability.

Why Cancellation Is Impossible:
- Blockchain provides permanent records
- Decentralized nature prevents reversals
- Transaction finality is guaranteed
- Network participants can't tamper with records

What You CAN Do If You Make a Mistake:
1. Verify the transaction can't be reversed
2. Contact recipient if you sent to wrong address
3. Report to authorities if fraudulent
4. Document the error for records

Prevention Best Practices:
- Always verify recipient address
- Double-check the amount before sending
- Review all transaction details carefully
- Pause and confirm before clicking send
- Start with small test transactions for new recipients
- Use address books for frequent contacts

Important:
- Be extremely careful with cryptocurrency
- Transactions are final and irreversible
- Take time to verify every detail
- Copy-paste addresses to avoid typos`,
    tags: ['cancellation', 'reversal', 'final', 'irreversible'],
  },

  // ============ SWAP TOKENS (5 FAQs) ============
  {
    id: 'sw-001',
    category: 'swap',
    question: 'How does the token swap feature work?',
    answer: `CoinPay\'s swap feature allows you to instantly exchange one cryptocurrency for another at market rates.

How Token Swaps Work:
1. Select the token you want to swap FROM
2. Select the token you want to receive
3. Enter the amount to swap
4. Review the exchange rate shown
5. Check the estimated fees
6. Confirm the swap

The Swap Process:
- Real-time price quotes updated every 10 seconds
- Exchange rate locked for 30 seconds after quote
- Execution happens on decentralized exchange
- Blockchain confirms within 1-2 minutes

What You Pay:
- Platform fee: 0.3% of swap amount
- Gas fee: Blockchain transaction cost
- Price impact: Varies based on liquidity

Example:
- Swap 100 USDC to MATIC
- Platform fee: $0.30
- Gas fee: ~$0.05
- Receive: ~480 MATIC (minus fees)`,
    tags: ['swap', 'exchange', 'trading', 'dex'],
  },
  {
    id: 'sw-002',
    category: 'swap',
    question: 'What is slippage and how do I set it?',
    answer: `Slippage is the difference between the expected price and the actual execution price of your swap. It occurs due to market volatility and liquidity conditions.

Understanding Slippage:
- Price can move between quote and execution
- Greater volatility = higher potential slippage
- Larger swaps have higher slippage risk
- Expressed as percentage (0.1%, 1%, 2%, etc.)

Slippage Settings Guide:
- 0.1-0.5%: Strict, may fail on volatile markets
- 0.5-1%: Balanced for most swaps (recommended)
- 1-2%: Higher chance of execution
- 2-3%: Very permissive, guarantees execution

How to Set Slippage:
1. Click settings during swap
2. Enter your slippage tolerance percentage
3. Higher = more likely to execute
4. Lower = better final price (if it executes)

Trade-offs:
- Low slippage: Better rates, higher failure risk
- High slippage: Guaranteed execution, worse rates

Example:
- Quote: 100 USDC = 480 MATIC
- 0.5% slippage: Accept 477.6-482.4 MATIC
- Transaction fails if market moves beyond range`,
    tags: ['slippage', 'tolerance', 'volatility', 'settings'],
  },
  {
    id: 'sw-003',
    category: 'swap',
    question: 'What fees are charged for swaps?',
    answer: `Token swaps involve multiple fee components that are transparent and clearly displayed before confirmation.

Fee Breakdown:

1. Platform Fee
   - Amount: 0.3% of swap amount
   - Purpose: Maintains CoinPay infrastructure
   - Goes to: CoinPay treasury

2. Gas Fee
   - Amount: Varies (typically $0.05-$0.50)
   - Purpose: Blockchain transaction cost
   - Goes to: Network validators

3. Price Impact
   - Amount: Varies based on liquidity
   - Purpose: Market adjustment for large orders
   - Goes to: Liquidity providers

Complete Example:
- Swap: 1000 USDC
- Platform fee (0.3%): $3
- Gas fee: $0.10
- Price impact: $2.50
- Total cost: $5.60
- Net received: $994.40 worth of tokens

Fee Display:
- All fees shown before confirmation
- Final amount is pre-calculated
- You see exact tokens received
- No hidden charges

Tips to Minimize Fees:
- Swap during low congestion times
- Make smaller, more frequent swaps
- Use stable pairs when possible
- Plan swaps during off-peak hours`,
    tags: ['fees', 'cost', 'charges', 'pricing'],
  },
  {
    id: 'sw-004',
    category: 'swap',
    question: 'Why did my swap fail?',
    answer: `Swaps can fail for several reasons. Understanding these helps you execute successful swaps.

Common Failure Reasons:

1. Insufficient Balance
   - Symptom: "Insufficient funds" error
   - Cause: Not enough tokens for swap + gas
   - Fix: Verify balance includes gas fee

2. Slippage Too Low
   - Symptom: "Slippage exceeded" error
   - Cause: Price moved beyond tolerance
   - Fix: Increase slippage tolerance

3. Insufficient Liquidity
   - Symptom: "Not enough liquidity" error
   - Cause: Swap amount too large
   - Fix: Reduce swap amount

4. Network Issues
   - Symptom: "RPC error" or timeout
   - Cause: Network congestion
   - Fix: Retry after a few minutes

5. Token Pairing Issues
   - Symptom: "Pair not available" error
   - Cause: Tokens not tradeable together
   - Fix: Use USDC as intermediate token

Failed Swap Implications:
- Gas fees ARE consumed (blockchain cost)
- Your tokens remain in wallet
- Transaction is recorded as failed
- You can retry with different settings

How to Recover from Failures:
1. Wait 1-2 minutes
2. Verify wallet balance
3. Adjust settings (higher slippage, smaller amount)
4. Retry the swap
5. Contact support if persistent`,
    tags: ['failure', 'error', 'troubleshooting', 'failed-swap'],
  },
  {
    id: 'sw-005',
    category: 'swap',
    question: 'How do I get the best exchange rates?',
    answer: `Getting the best rates requires strategy and timing. Here are proven tactics.

Rate Optimization Strategies:

1. Timing
   - Swap during low congestion (off-peak hours)
   - Avoid peak trading times (9-10 AM UTC)
   - Check network activity before swapping
   - Off-peak rates are often better

2. Liquidity Considerations
   - Swap smaller amounts for better rates
   - Use popular token pairs (USDC/MATIC)
   - Avoid obscure or new tokens
   - Check liquidity depth in swap preview

3. Route Selection
   - Direct swaps cheaper than multi-hop
   - USDC is excellent intermediate token
   - Multi-hop may offer better rates
   - CoinPay auto-selects best route

4. Market Conditions
   - Watch price charts before swapping
   - Avoid high volatility periods
   - Set price alerts for targets
   - Be patient for better prices

5. Multiple Venues
   - Compare CoinPay rates
   - Check external DEX aggregators
   - Consider total cost (slippage + fees)
   - Verify final received amount

Rate Comparison Tools:
- CoinGecko: Track real-time prices
- 1inch: Aggregate best rates
- DEXTools: Analyze liquidity

Pro Tips:
- Batch swaps to reduce gas costs
- Use limit orders when available
- Monitor gas prices (high = higher costs)
- Accept good rates rather than waiting for perfect`,
    tags: ['rates', 'optimization', 'liquidity', 'best-price'],
  },

  // ============ INVESTMENTS (5 FAQs) ============
  {
    id: 'inv-001',
    category: 'investments',
    question: 'What are investment strategies in CoinPay?',
    answer: `CoinPay offers automated investment strategies that allow hands-off crypto investing based on your preferences.

Available Strategies:

1. Dollar-Cost Averaging (DCA)
   - Regular purchases at set intervals
   - Example: Buy $100 USDC every week
   - Reduces impact of price volatility
   - Best for long-term investors
   - Automates emotion-free investing

2. Grid Trading
   - Place buy/sell orders in price grid
   - Automatically buy low, sell high
   - Profit from price fluctuations
   - Suitable for sideways markets
   - Requires close monitoring

3. Portfolio Rebalancing
   - Maintain target asset allocations
   - Example: Keep 60% BTC, 40% ETH
   - Automatically rebalance when drifts
   - Reduces concentration risk
   - Enforces disciplined investing

How Strategies Work:
- Connect WhiteBIT exchange account
- Define strategy parameters
- CoinPay executes trades automatically
- You maintain complete asset custody
- Monitor performance in real-time

Strategy Benefits:
- Eliminates emotional decisions
- Hands-off passive investing
- Consistent execution
- Detailed performance tracking
- Full transparency and control`,
    tags: ['strategy', 'automation', 'investing', 'trading'],
  },
  {
    id: 'inv-002',
    category: 'investments',
    question: 'How do I connect my WhiteBIT account?',
    answer: `Connecting WhiteBIT is secure and straightforward. Follow these steps:

Step-by-Step Setup:

1. WhiteBIT Account Setup
   - Log in to WhiteBIT (whitebit.com)
   - Go to Security/API settings
   - Generate new API keys
   - Select permissions: Read, Trade (not Withdraw)

2. CoinPay Integration
   - Go to "Investments" section
   - Click "Connect WhiteBIT"
   - Paste your API key
   - Paste your API secret
   - Click "Verify Connection"

3. Verification
   - CoinPay tests the connection
   - Shows connected account info
   - Displays available balance
   - Ready to use immediately

Security Best Practices:
- NEVER share your API secret
- Regenerate keys if compromised
- Use read-only mode for viewing
- Trade permissions required for strategies
- DO NOT enable withdrawal permission
- Regularly review connected apps

API Key Permissions (Correct Setup):
- Read: Yes (view balances)
- Trade: Yes (execute trades)
- Withdraw: No (extra security)
- Deposit: No (not needed)

If Connection Fails:
1. Verify API key copied correctly
2. Check API secret is accurate
3. Ensure permissions enabled
4. Try revoking and regenerating keys
5. Contact WhiteBIT support if issues

Important Notes:
- CoinPay cannot withdraw funds
- Funds stay in your WhiteBIT account
- You maintain full custody
- Connection is encrypted
- Revoke anytime in settings`,
    tags: ['whitebit', 'api', 'connection', 'exchange'],
  },
  {
    id: 'inv-003',
    category: 'investments',
    question: 'What are the risks of automated investment strategies?',
    answer: `Automated strategies offer convenience but carry inherent risks. Understand them before investing.

Market Risks:
- Crypto prices highly volatile
- Can lose significant amounts
- Past performance ≠ future results
- Market crashes can happen quickly
- Strategies continue through crashes

Strategy-Specific Risks:
- DCA Risk: Continuous buying during downtrend
- Grid Risk: Gets whipsawed in choppy markets
- Rebalancing Risk: Sells winners, buys losers

Operational Risks:
- Exchange API connectivity issues
- Network delays or outages
- Slippage on large trades
- Gas fees consume profits on small trades
- Strategy bugs (rare but possible)

Cryptocurrency Risks:
- Regulatory changes
- Security vulnerabilities
- Smart contract bugs
- Exchange insolvency
- Liquidity crises

Financial Risks:
- Total loss of invested capital possible
- No insurance or guarantees
- Leverage multiplies losses
- Margin calls (if using margin)

Mitigation Strategies:
1. Start with small amounts
2. Test strategies in off-hours first
3. Monitor performance regularly
4. Set stop-loss limits
5. Diversify across multiple strategies
6. Keep majority in manual control
7. Use only what you can afford to lose
8. Regular review and adjustment

Important Disclaimers:
- CoinPay is testnet, not for real money
- Consult financial advisor
- Do your own research (DYOR)
- Understand strategy before using
- Read all documentation`,
    tags: ['risk', 'investment', 'losses', 'volatility'],
  },
  {
    id: 'inv-004',
    category: 'investments',
    question: 'Can I stop or modify my investment strategy?',
    answer: `Yes, you have complete control over your strategies at all times.

Strategy Management Options:

1. Pause Strategy
   - Stops executing new trades
   - Doesn't close existing positions
   - Keeps funds in WhiteBIT
   - Can resume anytime

2. Resume Strategy
   - Restarts trading execution
   - Applies current parameters
   - No setup required
   - Immediate effect

3. Modify Parameters
   - Change buy amount
   - Adjust frequency
   - Update target prices (grid)
   - Modify allocations (rebalancing)
   - Changes apply on next execution

4. Stop Completely
   - Terminates strategy permanently
   - Leaves positions as-is
   - Funds remain in exchange
   - Must manually manage afterward

5. Disconnect Exchange
   - Revokes API access
   - Strategies stop immediately
   - Funds safe in WhiteBIT account
   - Can reconnect anytime

When Changes Take Effect:
- Pause/Resume: Immediate
- Parameter changes: Next execution cycle
- Disconnect: Immediate

Managing Active Positions:
- Existing trades complete normally
- Stop doesn't cancel pending orders
- Must manually close if needed
- Check WhiteBIT for open orders

Important Notes:
- Always verify changes saved
- Review parameters before resuming
- Keep strategy goals in mind
- Document why you changed settings
- Test changes with small amounts first`,
    tags: ['management', 'modifications', 'pause', 'resume'],
  },
  {
    id: 'inv-005',
    category: 'investments',
    question: 'How do I view my investment performance?',
    answer: `CoinPay provides detailed performance metrics and analytics for your strategies.

Performance Metrics:

1. Overall Returns
   - Total gain/loss in USD
   - Percentage return
   - Annualized return rate
   - Time-weighted returns

2. Trade History
   - Complete transaction list
   - Entry and exit prices
   - Profit/loss per trade
   - Execution fees
   - Timestamps

3. Strategy Metrics
   - Win rate percentage
   - Average win/loss
   - Largest win/loss
   - Number of trades
   - Trade frequency

4. Holdings Overview
   - Current positions
   - Cost basis
   - Current value
   - Unrealized P&L
   - Allocation percentages

Dashboard Features:
- Real-time balance updates
- Performance charts
- Trade logs with details
- Fee summaries
- Strategy statistics

How to View Reports:
1. Go to Investments section
2. Select strategy to analyze
3. Choose time period (1mo, 3mo, YTD, All)
4. View detailed breakdown
5. Export data if needed

Understanding Results:
- Realized P&L: Closed trades only
- Unrealized P&L: Open positions
- Net return: After all fees
- Win rate: Profitable trades %

Interpreting Performance:
- Compare to buy-hold baseline
- Account for market conditions
- Consider strategy objectives
- Review fee impact
- Look at consistency, not just returns

Data Export:
- Download CSV format
- Tax reporting compatible
- Suitable for accountants
- Timestamp all entries
- Keep backups`,
    tags: ['performance', 'analytics', 'returns', 'reports'],
  },

  // ============ SECURITY (3 FAQs) ============
  {
    id: 'sec-001',
    category: 'security',
    question: 'How do I keep my account secure?',
    answer: `Security is your responsibility. Follow these practices to protect your account.

Essential Security Practices:

1. Password Security
   - Use 12+ characters (16+ is better)
   - Mix uppercase, lowercase, numbers, symbols
   - Never reuse passwords across sites
   - Change if you suspect compromise
   - Use password manager

2. Two-Factor Authentication (2FA)
   - Enable 2FA on account
   - Use authenticator app (not SMS if possible)
   - Backup recovery codes securely
   - Store backup codes offline
   - Update keys if lost device

3. Device Security
   - Keep OS and software updated
   - Use antivirus/anti-malware software
   - Avoid public WiFi for transactions
   - Use VPN on public networks
   - Regular security updates

4. Phishing Awareness
   - Verify URLs before login (bookmark site)
   - Never click links in emails
   - Hover over links to see actual URL
   - Check email sender carefully
   - Be suspicious of urgency

5. Account Activity
   - Regularly review login history
   - Check connected devices/sessions
   - Sign out from unfamiliar locations
   - Disconnect unused integrations
   - Monitor withdrawal addresses

6. Information Protection
   - Never share password or private keys
   - Never take screenshots of seeds/keys
   - Don't discuss amounts with others
   - Be careful on social media
   - Document recovery codes securely

CoinPay Will Never:
- Ask for your password via email
- Request private keys
- Ask for seed phrases
- Message about account issues
- Need 2FA codes in email

What to Do If Compromised:
1. Change password immediately
2. Review recent activity
3. Check for unauthorized changes
4. Disconnect exchange integrations
5. Monitor account closely
6. Contact support if needed`,
    tags: ['security', 'password', '2fa', 'protection'],
  },
  {
    id: 'sec-002',
    category: 'security',
    question: 'What should I do if I suspect unauthorized access?',
    answer: `Act immediately if you suspect your account is compromised. Fast action limits damage.

Immediate Actions (First 5 Minutes):

1. Change Your Password
   - Click "Change Password" in settings
   - Use new, strong password
   - Don't reuse any previous password
   - Log out all sessions afterward

2. Review Recent Activity
   - Go to "Activity" or "Login History"
   - Look for unfamiliar locations/times
   - Note any suspicious transactions
   - Save screenshot of activity

3. Check Connected Apps
   - Go to "Integrations" or "Connected Apps"
   - Disconnect WhiteBIT if suspicious
   - Review all active connections
   - Enable reconnection only from safe device

4. Verify Email
   - Check for password change confirmations
   - Look for new session alerts
   - Verify email address hasn't changed
   - Check for forwarding rules added

Next Steps (Within Hours):

1. Secure Your Devices
   - Run antivirus scan
   - Check for malware
   - Update all software
   - Consider reinstalling OS if severe

2. Monitor Accounts
   - Check all connected exchanges
   - Monitor your email account
   - Watch for unusual activity
   - Set up fraud alerts

3. Document Evidence
   - Screenshot activity logs
   - Save transaction records
   - Note timeline of events
   - Gather any suspicious emails

Reporting the Incident:

1. Contact CoinPay Support
   - Email support@coinpay.example.com
   - Provide activity screenshots
   - Explain what happened
   - Request account review

2. If Funds Stolen
   - Local law enforcement
   - IC3.gov (if in USA)
   - Your country's cyber crime unit
   - Bank/payment processor

3. Follow-Up
   - Keep records of all communications
   - Monitor for recovery options
   - Check darknet monitoring services
   - Regular monitoring of accounts

Preventive Measures Going Forward:
- Enable strict 2FA
- Use authenticator app
- Change passwords monthly
- Monitor account weekly
- Use VPN always
- Update devices regularly`,
    tags: ['compromise', 'unauthorized', 'breach', 'recovery'],
  },
  {
    id: 'sec-003',
    category: 'security',
    question: 'Are my funds insured or protected?',
    answer: `CoinPay is currently a testnet platform for demonstration. Insurance and protection differ between testnet and mainnet.

Testnet (Current Status):
- CoinPay uses Polygon Amoy testnet
- Test tokens have zero real value
- No real funds at risk
- Not subject to real insurance
- Perfect for learning and testing
- No financial loss possible

Mainnet Considerations (Future):
If CoinPay deploys to mainnet with real assets:

Smart Contract Security:
- Code audits by reputable firms
- Public security review possible
- Insurance coverage available
- Risk mitigation strategies
- Progressive rollout recommended

Asset Protection Options:
1. Self-Custody
   - You control private keys
   - No third-party risk
   - Requires your security
   - Backup responsibility on you

2. Custodial Insurance
   - Optional third-party insurance
   - Covers smart contract bugs
   - Does not cover user errors
   - Typical cost: 0.5-1% annually
   - Subject to policy limits

3. Insurance Providers
   - Nexus Mutual: DeFi-specific
   - Armor: Protocol-level coverage
   - Yearn Insurance: Yield-bearing tokens
   - Terms vary by provider

4. Diversification
   - Don't keep all funds here
   - Multiple wallets/exchanges
   - Risk distribution
   - Reduces exposure to any platform

Blockchain Reality:
- Transactions are irreversible
- Once sent, cannot be recovered
- User error not covered by insurance
- Market losses not covered
- Personal security is critical

Current Security on Testnet:
- Regular updates and improvements
- Best practices implementation
- Community feedback integration
- Transparent development
- Educational focus

Recommendations:
- Never use mainnet keys on testnet
- Only use test tokens
- Treat seriously for experience
- Learn security best practices
- Understand blockchain immutability
- Research insurance thoroughly before mainnet`,
    tags: ['insurance', 'protection', 'custody', 'risk'],
  },

  // ============ TROUBLESHOOTING (2 FAQs) ============
  {
    id: 'ts-001',
    category: 'troubleshooting',
    question: 'Why isn\'t my balance updating or balance looks incorrect?',
    answer: `Balance display issues can occur for several reasons. Most resolve quickly.

Quick Fixes to Try First:

1. Refresh the Page
   - Press Ctrl+R (Windows) or Cmd+R (Mac)
   - Hard refresh: Ctrl+Shift+R
   - Close and reopen browser tab
   - Try a different browser

2. Clear Browser Cache
   - Chrome: Ctrl+Shift+Delete
   - Firefox: Ctrl+Shift+Delete
   - Safari: Develop → Empty Web Caches
   - Select "All time" and clear

3. Check Transaction Status
   - Go to "Transactions" page
   - Look for pending transactions
   - Click to see detailed status
   - Wait for "Confirmed" status

4. Verify Network
   - Ensure you're on Polygon Amoy
   - Check wallet address is correct
   - Confirm you're logged in
   - Try logging out and in

If Balance Still Wrong:

1. Check Explorer
   - Visit https://amoy.polygonscan.com
   - Search your wallet address
   - View actual blockchain balance
   - Compare with CoinPay balance

2. Common Issues
   - Recent transaction not confirmed yet
   - Browser cache showing old balance
   - Network connection interrupted
   - RPC node temporarily out of sync

3. Wait for Confirmation
   - Transactions take 1-2 minutes
   - Network congestion slows it down
   - Balance updates after confirmation
   - Explorer is always the source of truth

4. Check Each Token
   - Balance might show total not tokens
   - USDC balance separate from MATIC
   - Both needed to send transactions
   - View token individually

Advanced Troubleshooting:

1. Network Sync Issues
   - RPC node might be lagging
   - Wait 5-10 minutes
   - Try refreshing again
   - RPC will eventually sync

2. Browser Issues
   - Try incognito/private mode
   - Disable browser extensions
   - Test on different browser
   - Update to latest version

3. Connection Issues
   - Check internet connection
   - Try different network
   - Restart router
   - Check firewall settings

4. Account Issues
   - Verify correct account logged in
   - Check account name matches
   - Multiple accounts? (check all)
   - Different wallet per account?

When to Contact Support:
- Issue persists after all above steps
- Balance differs from explorer significantly
- Funds mysteriously missing
- Unable to complete transactions
- Repeated update failures

Information to Provide Support:
- Wallet address
- Expected vs actual balance
- Screenshots of issue
- Steps you already tried
- Browser and OS information
- Recent transaction hashes`,
    tags: ['balance', 'update', 'incorrect', 'sync'],
  },
  {
    id: 'ts-002',
    category: 'troubleshooting',
    question: 'I\'m getting "Insufficient funds" error. What should I do?',
    answer: `"Insufficient funds" error means you don't have enough tokens to complete your transaction. Resolve it by understanding what's needed.

What "Insufficient Funds" Means:
- Not enough of the token you're trying to send
- Not enough MATIC for gas fees
- Combined total (amount + gas) exceeds balance
- Pending transactions holding funds

Breaking Down Token Amounts:

Example: Sending 100 USDC
- Needed: 100 USDC + gas fee (MATIC)
- If you have: 100 USDC, 0 MATIC = FAIL
- If you have: 100 USDC, 0.05 MATIC = SUCCESS

Diagnosing the Issue:

1. Check Exact Balances
   - View Wallet page
   - Note USDC balance
   - Note MATIC balance
   - View both token amounts

2. Calculate What You Need
   - Amount to send: 100 USDC
   - Estimated gas: 0.05 MATIC
   - Total USDC needed: 100
   - Total MATIC needed: 0.05

3. Identify the Gap
   - Do you have 100 USDC? Yes/No
   - Do you have 0.05 MATIC? Yes/No
   - Which is missing?

Solutions by Scenario:

Scenario 1: Need More Token to Send
   - Have 50 USDC, want to send 100
   - Solution: Reduce send amount to 50
   - Or: Get more USDC from faucet

Scenario 2: Need More MATIC for Gas
   - Have 100 USDC, 0 MATIC
   - Solution: Get testnet MATIC first
   - Go to: faucet.polygon.technology
   - Paste wallet address
   - Request MATIC tokens

Scenario 3: Pending Transactions
   - Funds locked in pending transaction
   - Solution: Wait for confirmation
   - Check Transactions page
   - Usually resolves in 2-5 minutes

How to Get Testnet Tokens:

MATIC Faucet:
1. Visit faucet.polygon.technology
2. Select "Mumbai Testnet" or "Amoy Testnet"
3. Paste your wallet address
4. Complete CAPTCHA
5. Receive MATIC within 1-2 minutes

USDC Faucet:
1. Visit faucet.circle.com
2. Select Polygon Amoy network
3. Enter your wallet address
4. Request USDC
5. Instant credit

Best Practices:
- Always get MATIC before other tokens
- Keep 0.5 MATIC buffer for gas
- Check balance before large transactions
- Request testnet tokens frequently
- Don't wait until balance is zero

Prevention Tips:
- Fund wallet with more MATIC than needed
- Set transaction amount below balance
- Account for gas in every calculation
- Review displayed balance before sending
- Double-check after faucet requests`,
    tags: ['insufficient', 'funds', 'balance', 'gas'],
  },
];

export const categoryInfo = {
  'getting-started': {
    name: 'Getting Started',
    description: 'Learn the basics of CoinPay and get started quickly',
    color: 'primary',
  },
  'send-receive': {
    name: 'Send & Receive',
    description: 'How to send and receive cryptocurrencies',
    color: 'blue',
  },
  'swap': {
    name: 'Swap Tokens',
    description: 'Exchange cryptocurrencies at the best rates',
    color: 'purple',
  },
  'investments': {
    name: 'Investments',
    description: 'Automated investment strategies and portfolio management',
    color: 'green',
  },
  'security': {
    name: 'Security',
    description: 'Protect your account and assets',
    color: 'red',
  },
  'troubleshooting': {
    name: 'Troubleshooting',
    description: 'Common issues and how to resolve them',
    color: 'amber',
  },
};

export function searchFAQs(query: string, faqs: FAQ[] = faqData): FAQ[] {
  if (!query.trim()) return faqs;

  const queryLower = query.toLowerCase();
  return faqs.filter(
    faq =>
      faq.question.toLowerCase().includes(queryLower) ||
      faq.answer.toLowerCase().includes(queryLower) ||
      faq.tags.some(tag => tag.toLowerCase().includes(queryLower))
  );
}

export function filterFAQsByCategory(category: string, faqs: FAQ[] = faqData): FAQ[] {
  if (category === 'all') return faqs;
  return faqs.filter(faq => faq.category === category);
}

export function getFAQsByCategory(faqs: FAQ[] = faqData): Record<string, FAQ[]> {
  const grouped: Record<string, FAQ[]> = {};
  faqs.forEach(faq => {
    if (!grouped[faq.category]) {
      grouped[faq.category] = [];
    }
    const categoryArray = grouped[faq.category];
    if (categoryArray) {
      categoryArray.push(faq);
    }
  });
  return grouped;
}
