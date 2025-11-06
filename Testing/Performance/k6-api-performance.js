import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';

// Custom metrics
const errorRate = new Rate('errors');
const apiDuration = new Trend('api_duration');
const requestCounter = new Counter('requests_total');

// Test configuration
export const options = {
  scenarios: {
    // Normal Load Test
    normal_load: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '1m', target: 10 },  // Ramp up to 10 users
        { duration: '8m', target: 10 },  // Stay at 10 users
        { duration: '1m', target: 0 },   // Ramp down to 0
      ],
      gracefulRampDown: '30s',
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<2000', 'p(99)<5000'], // 95% under 2s, 99% under 5s
    http_req_failed: ['rate<0.01'],                   // Error rate < 1%
    errors: ['rate<0.1'],                             // Custom error rate < 10%
    api_duration: ['p(95)<2000'],                     // API response P95 < 2s
  },
};

// Environment configuration
const BASE_URL = __ENV.BASE_URL || 'https://api.coinpay.app';
const API_KEY = __ENV.API_KEY || '';

// Test data
const TEST_WALLET_ADDRESS = '0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb1';
const TEST_USER_EMAIL = `loadtest_${__VU}_${Date.now()}@example.com`;
const TEST_PASSWORD = 'LoadTest123!@#';

// Helper function to make authenticated requests
function authenticatedRequest(url, token, method = 'GET', payload = null) {
  const params = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    tags: { name: url },
  };

  let response;
  if (method === 'POST') {
    response = http.post(`${BASE_URL}${url}`, payload ? JSON.stringify(payload) : null, params);
  } else {
    response = http.get(`${BASE_URL}${url}`, params);
  }

  return response;
}

// Setup function - runs once per VU
export function setup() {
  // Optionally create test users or get auth tokens
  return {
    baseUrl: BASE_URL,
  };
}

// Main test function
export default function (data) {
  let authToken;

  group('Authentication Flow', function () {
    // Login test
    const loginPayload = {
      email: 'testuser@example.com', // Use pre-created test user
      password: 'TestPassword123!',
    };

    const loginRes = http.post(`${BASE_URL}/api/auth/login`, JSON.stringify(loginPayload), {
      headers: { 'Content-Type': 'application/json' },
      tags: { name: 'POST_/api/auth/login' },
    });

    const loginSuccess = check(loginRes, {
      'login status 200': (r) => r.status === 200,
      'login response time < 800ms': (r) => r.timings.duration < 800,
      'login returns token': (r) => r.json('token') !== undefined,
    });

    if (!loginSuccess) {
      errorRate.add(1);
      console.error(`Login failed: ${loginRes.status} - ${loginRes.body}`);
      return; // Skip rest of test for this VU iteration
    }

    authToken = loginRes.json('token');
    apiDuration.add(loginRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  if (!authToken) {
    console.error('No auth token, skipping authenticated tests');
    return;
  }

  group('Wallet Operations', function () {
    // Get wallet balance
    const balanceRes = authenticatedRequest(`/api/wallet/balance/${TEST_WALLET_ADDRESS}`, authToken);

    const balanceSuccess = check(balanceRes, {
      'balance status 200': (r) => r.status === 200,
      'balance response time < 1500ms': (r) => r.timings.duration < 1500,
      'balance has USDC': (r) => r.json('balances.USDC') !== undefined,
      'balance has ETH': (r) => r.json('balances.ETH') !== undefined,
      'balance has MATIC': (r) => r.json('balances.MATIC') !== undefined,
    });

    if (!balanceSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(balanceRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  group('Transaction History', function () {
    // Get transaction history with pagination
    const historyRes = authenticatedRequest('/api/transactions/history?page=1&pageSize=20', authToken);

    const historySuccess = check(historyRes, {
      'history status 200': (r) => r.status === 200,
      'history response time < 1000ms': (r) => r.timings.duration < 1000,
      'history returns array': (r) => Array.isArray(r.json('transactions')),
      'history has pagination': (r) => r.json('totalPages') !== undefined,
    });

    if (!historySuccess) {
      errorRate.add(1);
    }

    apiDuration.add(historyRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  group('Swap Operations', function () {
    // Get swap quote
    const quoteParams = 'fromToken=USDC&toToken=ETH&amount=100';
    const quoteRes = authenticatedRequest(`/api/swap/quote?${quoteParams}`, authToken);

    const quoteSuccess = check(quoteRes, {
      'swap quote status 200': (r) => r.status === 200,
      'swap quote response time < 1500ms': (r) => r.timings.duration < 1500,
      'quote has exchange rate': (r) => r.json('exchangeRate') !== undefined,
      'quote has estimated amount': (r) => r.json('estimatedAmount') !== undefined,
      'quote has fees': (r) => r.json('fees') !== undefined,
    });

    if (!quoteSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(quoteRes.timings.duration);
    requestCounter.add(1);

    sleep(2);
  });

  group('Investment Operations', function () {
    // Get investment plans
    const plansRes = authenticatedRequest('/api/exchange/plans', authToken);

    const plansSuccess = check(plansRes, {
      'plans status 200': (r) => r.status === 200,
      'plans response time < 1500ms': (r) => r.timings.duration < 1500,
      'plans returns array': (r) => Array.isArray(r.json('plans')),
    });

    if (!plansSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(plansRes.timings.duration);
    requestCounter.add(1);

    sleep(1);

    // Get investment positions
    const positionsRes = authenticatedRequest('/api/investment/positions', authToken);

    const positionsSuccess = check(positionsRes, {
      'positions status 200': (r) => r.status === 200,
      'positions response time < 1000ms': (r) => r.timings.duration < 1000,
      'positions returns array': (r) => Array.isArray(r.json('positions')),
    });

    if (!positionsSuccess) {
      errorRate.add(1);
    }

    apiDuration.add(positionsRes.timings.duration);
    requestCounter.add(1);

    sleep(1);
  });

  // Think time between iterations
  sleep(Math.random() * 3 + 2); // Random sleep between 2-5 seconds
}

// Teardown function
export function teardown(data) {
  // Cleanup if needed
}

// Generate HTML report
export function handleSummary(data) {
  return {
    'performance-report.html': htmlReport(data),
    'performance-summary.txt': textSummary(data, { indent: ' ', enableColors: true }),
    stdout: textSummary(data, { indent: ' ', enableColors: true }),
  };
}
