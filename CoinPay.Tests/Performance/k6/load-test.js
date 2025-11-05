/**
 * QA-205: Performance Testing with K6
 * Load Test Script for CoinPay Application
 *
 * This script simulates 100+ concurrent users performing typical operations
 * Target: P95 response time < 1 second for critical endpoints
 */

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Counter, Rate, Trend } from 'k6/metrics';

// Custom metrics
const loginFailures = new Counter('login_failures');
const transferSuccessRate = new Rate('transfer_success_rate');
const balanceCheckDuration = new Trend('balance_check_duration');
const transactionListDuration = new Trend('transaction_list_duration');

// Configuration
const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';
const FRONTEND_URL = __ENV.FRONTEND_URL || 'http://localhost:3000';

// Load test stages
export const options = {
  stages: [
    // Ramp-up: 0 to 50 users over 2 minutes
    { duration: '2m', target: 50 },

    // Steady state: 50 users for 3 minutes
    { duration: '3m', target: 50 },

    // Peak load: 50 to 100 users over 2 minutes
    { duration: '2m', target: 100 },

    // Peak steady state: 100 users for 5 minutes
    { duration: '5m', target: 100 },

    // Spike: 100 to 150 users over 1 minute
    { duration: '1m', target: 150 },

    // Spike duration: 150 users for 2 minutes
    { duration: '2m', target: 150 },

    // Ramp-down: 150 to 0 users over 2 minutes
    { duration: '2m', target: 0 },
  ],

  // Performance thresholds
  thresholds: {
    // HTTP request duration (P95 < 1s for critical endpoints)
    'http_req_duration{endpoint:balance}': ['p(95)<1000'],
    'http_req_duration{endpoint:transactions}': ['p(95)<1500'],
    'http_req_duration{endpoint:transfer}': ['p(95)<2000'],

    // HTTP request success rate (>99%)
    'http_req_failed': ['rate<0.01'],

    // Custom metrics
    'transfer_success_rate': ['rate>0.95'],
    'balance_check_duration': ['p(95)<1000'],
    'transaction_list_duration': ['p(95)<1500'],
  },
};

// Test data
const testUsers = [
  { email: 'user1@test.com', password: 'testpass' },
  { email: 'user2@test.com', password: 'testpass' },
  { email: 'user3@test.com', password: 'testpass' },
  { email: 'user4@test.com', password: 'testpass' },
  { email: 'user5@test.com', password: 'testpass' },
];

// Helper function to get random user
function getRandomUser() {
  return testUsers[Math.floor(Math.random() * testUsers.length)];
}

// Helper function to login
function login() {
  const user = getRandomUser();

  const payload = JSON.stringify({
    email: user.email,
    password: user.password,
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: { endpoint: 'login' },
  };

  const res = http.post(`${BASE_URL}/api/auth/login`, payload, params);

  const success = check(res, {
    'login status is 200': (r) => r.status === 200,
    'login returns token': (r) => r.json('token') !== undefined,
  });

  if (!success) {
    loginFailures.add(1);
    return null;
  }

  return res.json('token');
}

// Main test scenario
export default function () {
  // Login
  const token = login();
  if (!token) {
    sleep(1);
    return;
  }

  const authHeaders = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  };

  // User workflow simulation
  group('User Workflow', function () {

    // 1. Check wallet balance (most frequent operation)
    group('Check Balance', function () {
      const startTime = Date.now();

      const balanceRes = http.get(
        `${BASE_URL}/api/wallet/balance`,
        {
          ...authHeaders,
          tags: { endpoint: 'balance' },
        }
      );

      const duration = Date.now() - startTime;
      balanceCheckDuration.add(duration);

      check(balanceRes, {
        'balance check status is 200': (r) => r.status === 200,
        'balance is number': (r) => typeof r.json('balance') === 'number',
        'balance response time < 1s': (r) => r.timings.duration < 1000,
      });
    });

    sleep(1);

    // 2. View transaction history (frequent operation)
    group('View Transactions', function () {
      const startTime = Date.now();

      const txRes = http.get(
        `${BASE_URL}/api/transactions/history?page=1&pageSize=20`,
        {
          ...authHeaders,
          tags: { endpoint: 'transactions' },
        }
      );

      const duration = Date.now() - startTime;
      transactionListDuration.add(duration);

      check(txRes, {
        'transactions status is 200': (r) => r.status === 200,
        'transactions is array': (r) => Array.isArray(r.json('transactions')),
        'transactions response time < 1.5s': (r) => r.timings.duration < 1500,
      });
    });

    sleep(2);

    // 3. Make a transfer (less frequent, but critical)
    group('Submit Transfer', function () {
      const transferPayload = JSON.stringify({
        recipientAddress: '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb0',
        amount: 10.0,
        currency: 'USDC',
        note: 'Load test transfer',
      });

      const transferRes = http.post(
        `${BASE_URL}/api/transfer/submit`,
        transferPayload,
        {
          ...authHeaders,
          tags: { endpoint: 'transfer' },
        }
      );

      const success = check(transferRes, {
        'transfer status is 200 or 201': (r) =>
          r.status === 200 || r.status === 201,
        'transfer returns transaction ID': (r) =>
          r.json('transactionId') !== undefined,
        'transfer response time < 2s': (r) => r.timings.duration < 2000,
      });

      transferSuccessRate.add(success);

      // If transfer succeeded, check status
      if (success) {
        const txId = transferRes.json('transactionId');

        sleep(1);

        // Check transaction status
        const statusRes = http.get(
          `${BASE_URL}/api/transactions/${txId}/status`,
          {
            ...authHeaders,
            tags: { endpoint: 'transaction-status' },
          }
        );

        check(statusRes, {
          'status check status is 200': (r) => r.status === 200,
          'status is valid': (r) =>
            ['Pending', 'Confirmed', 'Failed'].includes(r.json('status')),
        });
      }
    });

    sleep(3);

    // 4. Check transaction details (occasional operation)
    group('View Transaction Details', function () {
      // Get first transaction from history
      const txListRes = http.get(
        `${BASE_URL}/api/transactions/history?page=1&pageSize=1`,
        authHeaders
      );

      if (txListRes.status === 200) {
        const transactions = txListRes.json('transactions');

        if (transactions && transactions.length > 0) {
          const txId = transactions[0].transactionId;

          const detailsRes = http.get(
            `${BASE_URL}/api/transactions/${txId}/details`,
            {
              ...authHeaders,
              tags: { endpoint: 'transaction-details' },
            }
          );

          check(detailsRes, {
            'details status is 200': (r) => r.status === 200,
            'details has transaction ID': (r) =>
              r.json('transactionId') !== undefined,
            'details response time < 1s': (r) => r.timings.duration < 1000,
          });
        }
      }
    });

    sleep(2);
  });

  // Random think time between user sessions (1-5 seconds)
  sleep(Math.random() * 4 + 1);
}

// Setup function (runs once per VU at start)
export function setup() {
  console.log('Starting load test...');
  console.log(`Base URL: ${BASE_URL}`);
  console.log(`Target: 150 concurrent users (peak)`);
  console.log(`Duration: ~17 minutes total`);

  // Verify API is reachable
  const healthRes = http.get(`${BASE_URL}/health`, {
    timeout: '5s',
  });

  if (healthRes.status !== 200) {
    console.error('API health check failed!');
    console.error(`Status: ${healthRes.status}`);
    throw new Error('API not reachable');
  }

  console.log('API health check passed ✓');

  return {
    startTime: Date.now(),
  };
}

// Teardown function (runs once after all VUs finish)
export function teardown(data) {
  const duration = (Date.now() - data.startTime) / 1000;
  console.log(`\nLoad test completed in ${duration.toFixed(2)} seconds`);
}
