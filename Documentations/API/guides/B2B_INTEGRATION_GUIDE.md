# CoinPay B2B Integration Guide

## Overview

This guide provides step-by-step instructions for integrating CoinPay into your B2B platform to accept cryptocurrency payments, process withdrawals, and manage user wallets.

## Table of Contents

1. [Integration Architecture](#integration-architecture)
2. [Prerequisites](#prerequisites)
3. [Setup & Configuration](#setup--configuration)
4. [Core Integration Flows](#core-integration-flows)
5. [Security Best Practices](#security-best-practices)
6. [Production Checklist](#production-checklist)

## Integration Architecture

### High-Level Architecture

```
┌─────────────────┐
│  Your Platform  │
│   (Frontend)    │
└────────┬────────┘
         │
         ├─ REST API Calls
         │
┌────────▼─────────────┐
│  Your Backend Server │
│                      │
│  - API Key Storage   │
│  - Webhook Handler   │
│  - Business Logic    │
└────────┬─────────────┘
         │
         ├─ JWT Authentication
         ├─ API Requests
         │
┌────────▼────────┐
│  CoinPay API    │
│                 │
│  - Wallet Mgmt  │
│  - Transactions │
│  - Swaps        │
│  - Payouts      │
└──────┬──────────┘
       │
       ├─ Blockchain Transactions
       │
┌──────▼──────────┐
│  Polygon Amoy   │
│  (Testnet)      │
└─────────────────┘
```

## Prerequisites

### 1. Technical Requirements

- Backend server (Node.js, .NET, Python, or similar)
- HTTPS-enabled domain for production
- Database for storing user data and transaction history
- Webhook endpoint for receiving real-time notifications

### 2. CoinPay Account Setup

1. Create a CoinPay account at https://coinpay.com/register
2. Verify your email address
3. Complete KYB (Know Your Business) verification for production
4. Generate API credentials from Settings → API Keys

### 3. Development Tools

- Postman or similar API testing tool
- Code editor (VS Code, Visual Studio, etc.)
- Git for version control
- Docker (optional, for local testing)

## Setup & Configuration

### Step 1: Environment Variables

Create a `.env` file in your backend:

```bash
# CoinPay API Configuration
COINPAY_API_URL=http://localhost:7777
COINPAY_USERNAME=your-username
COINPAY_PASSWORD=your-password
COINPAY_WEBHOOK_SECRET=your-webhook-secret

# Your Application
APP_PORT=3000
DATABASE_URL=postgresql://user:password@localhost:5432/yourdb
```

### Step 2: Initialize API Client

**Node.js Example:**

```javascript
// src/services/coinpay.service.js
const axios = require('axios');

class CoinPayService {
  constructor() {
    this.baseURL = process.env.COINPAY_API_URL;
    this.token = null;
    this.tokenExpiry = null;
  }

  async authenticate() {
    if (this.token && this.tokenExpiry > Date.now()) {
      return this.token; // Token still valid
    }

    const response = await axios.post(`${this.baseURL}/api/auth/login`, {
      username: process.env.COINPAY_USERNAME,
      password: process.env.COINPAY_PASSWORD
    });

    this.token = response.data.token;
    this.tokenExpiry = Date.now() + (response.data.expiresIn * 60 * 1000);

    return this.token;
  }

  async request(method, endpoint, data = null) {
    const token = await this.authenticate();

    const config = {
      method,
      url: `${this.baseURL}${endpoint}`,
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    };

    if (data) {
      config.data = data;
    }

    try {
      const response = await axios(config);
      return response.data;
    } catch (error) {
      console.error('CoinPay API Error:', error.response?.data || error.message);
      throw error;
    }
  }

  // Wallet Methods
  async getWallet() {
    return this.request('GET', '/api/wallet');
  }

  async getBalance() {
    return this.request('GET', '/api/wallet/balance');
  }

  // Transaction Methods
  async createTransaction(transactionData) {
    return this.request('POST', '/api/transactions', transactionData);
  }

  async getTransaction(id) {
    return this.request('GET', `/api/transactions/${id}`);
  }

  async listTransactions(filters = {}) {
    const params = new URLSearchParams(filters).toString();
    return this.request('GET', `/api/transactions?${params}`);
  }
}

module.exports = new CoinPayService();
```

**.NET Example:**

```csharp
// Services/CoinPayService.cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class CoinPayService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _username;
    private readonly string _password;
    private string _token;
    private DateTime _tokenExpiry;

    public CoinPayService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _baseUrl = config["CoinPay:ApiUrl"];
        _username = config["CoinPay:Username"];
        _password = config["CoinPay:Password"];
    }

    public async Task<string> AuthenticateAsync()
    {
        if (!string.IsNullOrEmpty(_token) && _tokenExpiry > DateTime.UtcNow)
        {
            return _token;
        }

        var payload = new { username = _username, password = _password };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            $"{_baseUrl}/api/auth/login",
            content
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(result);

        _token = authResponse.Token;
        _tokenExpiry = DateTime.UtcNow.AddMinutes(authResponse.ExpiresIn);

        return _token;
    }

    public async Task<T> RequestAsync<T>(
        HttpMethod method,
        string endpoint,
        object data = null
    )
    {
        var token = await AuthenticateAsync();

        var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        if (data != null)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"
            );
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseData);
    }
}
```

## Core Integration Flows

### Flow 1: Accept Crypto Payment

**Use Case:** Customer wants to pay for your service with USDC

**Steps:**

1. **Customer initiates payment** on your platform
2. **Your backend creates transaction** via CoinPay API
3. **Display payment details** to customer (amount, recipient address)
4. **Customer sends crypto** from their wallet
5. **Receive webhook notification** when payment is confirmed
6. **Update order status** in your database
7. **Fulfill order** for customer

**Implementation:**

```javascript
// routes/payments.js
const express = require('express');
const router = express.Router();
const coinpay = require('../services/coinpay.service');
const Order = require('../models/Order');

// Create payment
router.post('/api/payments', async (req, res) => {
  try {
    const { orderId, amount, currency } = req.body;

    // 1. Get order from your database
    const order = await Order.findById(orderId);
    if (!order) {
      return res.status(404).json({ error: 'Order not found' });
    }

    // 2. Create transaction in CoinPay
    const transaction = await coinpay.createTransaction({
      amount: amount,
      currency: currency || 'USDC',
      type: 'Payment',
      receiverName: process.env.COINPAY_MERCHANT_ADDRESS,
      description: `Payment for Order #${orderId}`,
      metadata: {
        orderId: orderId,
        customerId: order.customerId
      }
    });

    // 3. Save transaction ID to order
    order.coinpayTransactionId = transaction.transactionId;
    order.paymentStatus = 'pending';
    await order.save();

    // 4. Return payment details to frontend
    res.json({
      transactionId: transaction.transactionId,
      amount: transaction.amount,
      currency: transaction.currency,
      recipientAddress: transaction.receiverName,
      status: transaction.status
    });

  } catch (error) {
    console.error('Payment creation error:', error);
    res.status(500).json({ error: 'Failed to create payment' });
  }
});

// Check payment status
router.get('/api/payments/:transactionId', async (req, res) => {
  try {
    const { transactionId } = req.params;

    // Get transaction from CoinPay
    const transaction = await coinpay.getTransaction(transactionId);

    res.json({
      transactionId: transaction.transactionId,
      status: transaction.status,
      amount: transaction.amount,
      completedAt: transaction.completedAt
    });

  } catch (error) {
    console.error('Payment status error:', error);
    res.status(500).json({ error: 'Failed to get payment status' });
  }
});

module.exports = router;
```

### Flow 2: Handle Webhook Notifications

**Use Case:** Receive real-time updates when payments are confirmed

**Implementation:**

```javascript
// routes/webhooks.js
const express = require('express');
const router = express.Router();
const crypto = require('crypto');
const Order = require('../models/Order');

// Verify webhook signature
function verifyWebhookSignature(payload, signature, secret) {
  const hash = crypto
    .createHmac('sha256', secret)
    .update(JSON.stringify(payload))
    .digest('hex');

  return hash === signature;
}

// Webhook endpoint
router.post('/webhooks/coinpay', async (req, res) => {
  try {
    const signature = req.headers['x-signature'];
    const payload = req.body;

    // 1. Verify webhook signature
    const isValid = verifyWebhookSignature(
      payload,
      signature,
      process.env.COINPAY_WEBHOOK_SECRET
    );

    if (!isValid) {
      console.error('Invalid webhook signature');
      return res.status(401).json({ error: 'Invalid signature' });
    }

    // 2. Handle different event types
    switch (payload.event) {
      case 'transaction.completed':
        await handleTransactionCompleted(payload.data);
        break;

      case 'transaction.failed':
        await handleTransactionFailed(payload.data);
        break;

      case 'payout.completed':
        await handlePayoutCompleted(payload.data);
        break;

      default:
        console.log(`Unhandled event: ${payload.event}`);
    }

    // 3. Acknowledge receipt
    res.status(200).json({ received: true });

  } catch (error) {
    console.error('Webhook error:', error);
    res.status(500).json({ error: 'Webhook processing failed' });
  }
});

async function handleTransactionCompleted(data) {
  const { transactionId, amount, currency, metadata } = data;

  // Find order by transaction ID or metadata
  const order = await Order.findOne({
    coinpayTransactionId: transactionId
  });

  if (!order) {
    console.error(`Order not found for transaction ${transactionId}`);
    return;
  }

  // Update order status
  order.paymentStatus = 'completed';
  order.paidAt = new Date();
  order.paidAmount = amount;
  order.paidCurrency = currency;
  await order.save();

  // Send confirmation email to customer
  await sendPaymentConfirmationEmail(order);

  // Fulfill order
  await fulfillOrder(order);

  console.log(`Payment completed for order ${order.id}`);
}

async function handleTransactionFailed(data) {
  const { transactionId, error } = data;

  const order = await Order.findOne({
    coinpayTransactionId: transactionId
  });

  if (order) {
    order.paymentStatus = 'failed';
    order.paymentError = error;
    await order.save();

    console.log(`Payment failed for order ${order.id}: ${error}`);
  }
}

module.exports = router;
```

### Flow 3: Process Withdrawal (Fiat Off-Ramp)

**Use Case:** Customer wants to withdraw crypto to their bank account

**Implementation:**

```javascript
// routes/withdrawals.js
const express = require('express');
const router = express.Router();
const coinpay = require('../services/coinpay.service');
const User = require('../models/User');

// Create withdrawal
router.post('/api/withdrawals', async (req, res) => {
  try {
    const { userId, amount, currency, bankAccountId } = req.body;

    // 1. Get user and verify bank account
    const user = await User.findById(userId).populate('bankAccounts');
    const bankAccount = user.bankAccounts.find(
      acc => acc.id === bankAccountId
    );

    if (!bankAccount) {
      return res.status(404).json({ error: 'Bank account not found' });
    }

    // 2. Create payout in CoinPay
    const payout = await coinpay.request('POST', '/api/payouts', {
      amount: amount,
      currency: currency,
      bankAccountId: bankAccount.coinpayId,
      description: `Withdrawal for user ${userId}`
    });

    // 3. Save payout record
    const withdrawal = {
      userId: userId,
      coinpayPayoutId: payout.id,
      amount: amount,
      currency: currency,
      status: 'pending',
      createdAt: new Date()
    };

    await user.withdrawals.push(withdrawal);
    await user.save();

    res.json({
      withdrawalId: withdrawal.id,
      payoutId: payout.id,
      status: payout.status,
      estimatedArrival: payout.estimatedArrival
    });

  } catch (error) {
    console.error('Withdrawal error:', error);
    res.status(500).json({ error: 'Failed to create withdrawal' });
  }
});

module.exports = router;
```

## Security Best Practices

### 1. API Key Management

- **Store API keys in environment variables**, never in code
- **Use different keys** for development and production
- **Rotate keys regularly** (every 90 days)
- **Restrict API key permissions** to minimum required

### 2. Webhook Security

- **Verify webhook signatures** for every request
- **Use HTTPS** for webhook endpoints
- **Implement idempotency** to handle duplicate webhooks
- **Log all webhook events** for audit trail

### 3. Transaction Validation

- **Verify transaction amounts** match expected values
- **Check transaction status** before fulfilling orders
- **Implement timeout logic** for pending transactions
- **Handle edge cases** (partial payments, overpayments)

### 4. Error Handling

```javascript
// Implement exponential backoff for retries
async function retryWithBackoff(fn, maxRetries = 3) {
  for (let i = 0; i < maxRetries; i++) {
    try {
      return await fn();
    } catch (error) {
      if (i === maxRetries - 1) throw error;

      const delay = Math.pow(2, i) * 1000; // 1s, 2s, 4s
      console.log(`Retry ${i + 1} after ${delay}ms`);
      await new Promise(resolve => setTimeout(resolve, delay));
    }
  }
}

// Usage
await retryWithBackoff(() => coinpay.createTransaction(data));
```

## Production Checklist

### Pre-Launch

- [ ] Complete KYB verification
- [ ] Generate production API keys
- [ ] Configure production webhook endpoint (HTTPS)
- [ ] Test webhook signature verification
- [ ] Implement error monitoring (Sentry, etc.)
- [ ] Set up logging and alerting
- [ ] Load test your integration
- [ ] Prepare customer support documentation

### Security

- [ ] All API keys stored in secure vault
- [ ] HTTPS enforced on all endpoints
- [ ] Webhook signatures verified
- [ ] Rate limiting implemented
- [ ] SQL injection prevention
- [ ] XSS prevention
- [ ] CSRF protection

### Monitoring

- [ ] Transaction success/failure metrics
- [ ] API latency monitoring
- [ ] Error rate alerting
- [ ] Webhook delivery monitoring
- [ ] Balance monitoring
- [ ] Audit logging

### Compliance

- [ ] KYB documentation complete
- [ ] Privacy policy updated
- [ ] Terms of service updated
- [ ] AML/KYC procedures documented
- [ ] Transaction limits configured

## Support

### Resources

- **Documentation:** https://docs.coinpay.com
- **API Reference:** http://localhost:7777/swagger
- **Email Support:** dev@coinpay.com
- **Discord Community:** https://discord.gg/coinpay

### Getting Help

1. Check documentation first
2. Search GitHub issues
3. Ask in Discord community
4. Email support for urgent issues

---

**Last Updated:** November 7, 2025
**Version:** 1.0.0
