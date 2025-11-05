/**
 * K6 Load Test - Payout Initiation API
 * Sprint N03 - Phase 3: Fiat Off-Ramp
 *
 * Test Scenario:
 * - Simulates 100+ concurrent users initiating payouts
 * - Tests API performance under load
 * - Validates response times and success rates
 *
 * Performance Targets:
 * - P95 < 2000ms (payout initiation)
 * - Error rate < 1%
 * - Success rate > 95%
 *
 * Run: k6 run payout-load-test.js
 */

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';

// Custom metrics
const payoutInitiationDuration = new Trend('payout_initiation_duration');
const payoutSuccessRate = new Rate('payout_success_rate');
const insufficientBalanceErrors = new Counter('insufficient_balance_errors');
const authenticationErrors = new Counter('authentication_errors');

// Configuration
const BASE_URL = __ENV.BASE_URL || 'http://localhost:5100';
const TEST_USER_EMAIL = __ENV.TEST_USER_EMAIL || 'testuser@test.com';
const TEST_BANK_ACCOUNT_ID = __ENV.TEST_BANK_ACCOUNT_ID || 'test-bank-account-id';

// Load test configuration
export const options = {
  stages: [
    { duration: '1m', target: 20 },   // Ramp up to 20 users
    { duration: '2m', target: 50 },   // Ramp up to 50 users
    { duration: '3m', target: 100 },  // Ramp up to 100 users
    { duration: '5m', target: 100 },  // Stay at 100 users for 5 minutes
    { duration: '2m', target: 50 },   // Ramp down to 50 users
    { duration: '1m', target: 0 },    // Ramp down to 0 users
  ],
  thresholds: {
    // Performance thresholds
    'payout_initiation_duration': [
      'p(95)<2000',  // 95% of requests must complete within 2s
      'p(99)<3000',  // 99% of requests must complete within 3s
    ],
    'http_req_failed': ['rate<0.01'],  // <1% errors
    'payout_success_rate': ['rate>0.95'],  // >95% success
    'http_req_duration': ['p(95)<2500'],  // Overall P95 < 2.5s
  },
};

/**
 * Setup function - runs once before tests
 */
export function setup() {
  console.log('=== Payout Load Test Setup ===');
  console.log(`BASE_URL: ${BASE_URL}`);
  console.log(`Test User: ${TEST_USER_EMAIL}`);

  // Health check
  const healthRes = http.get(`${BASE_URL}/health`);
  if (healthRes.status !== 200) {
    console.error('API health check failed!');
    return null;
  }

  console.log('API health check passed');
  return { baseUrl: BASE_URL };
}

/**
 * Main test function - runs for each virtual user
 */
export default function (data) {
  if (!data) {
    console.error('Setup failed, skipping test');
    return;
  }

  // Step 1: Login (Development endpoint)
  const loginRes = http.post(
    `${data.baseUrl}/api/auth/login/dev`,
    JSON.stringify({
      username: TEST_USER_EMAIL
    }),
    {
      headers: {
        'Content-Type': 'application/json',
      },
    }
  );

  const loginSuccess = check(loginRes, {
    'login successful': (r) => r.status === 200,
    'token received': (r) => r.json('token') !== undefined,
  });

  if (!loginSuccess) {
    authenticationErrors.add(1);
    return;
  }

  const token = loginRes.json('token');

  // Step 2: Get exchange rate
  const rateRes = http.get(
    `${data.baseUrl}/api/rates/usdc-usd`,
    {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    }
  );

  check(rateRes, {
    'rate fetch successful': (r) => r.status === 200,
    'valid rate returned': (r) => r.json('rate') > 0,
  });

  // Step 3: Get conversion preview
  const previewAmount = Math.random() * 900 + 100; // Random amount 100-1000 USDC
  const previewRes = http.post(
    `${data.baseUrl}/api/payout/preview`,
    JSON.stringify({
      usdcAmount: previewAmount,
    }),
    {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    }
  );

  check(previewRes, {
    'preview successful': (r) => r.status === 200,
    'fees calculated': (r) => r.json('totalFees') !== undefined,
  });

  // Step 4: Initiate payout (main test operation)
  const payoutStartTime = Date.now();

  const payoutRes = http.post(
    `${data.baseUrl}/api/payout/initiate`,
    JSON.stringify({
      bankAccountId: TEST_BANK_ACCOUNT_ID,
      usdcAmount: previewAmount,
    }),
    {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      tags: { operation: 'payout_initiation' },
    }
  );

  const payoutDuration = Date.now() - payoutStartTime;
  payoutInitiationDuration.add(payoutDuration);

  const payoutSuccess = check(payoutRes, {
    'payout initiated': (r) => r.status === 201 || r.status === 200,
    'payout ID received': (r) => r.json('id') !== undefined || r.status === 400,
    'response time acceptable': (r) => payoutDuration < 2000,
  });

  // Track specific error types
  if (payoutRes.status === 400) {
    const errorCode = payoutRes.json('error.code');
    if (errorCode === 'INSUFFICIENT_BALANCE') {
      insufficientBalanceErrors.add(1);
    }
  }

  payoutSuccessRate.add(payoutSuccess);

  // Step 5: Check payout status (if initiation succeeded)
  if (payoutRes.status === 201) {
    const payoutId = payoutRes.json('id');

    const statusRes = http.get(
      `${data.baseUrl}/api/payout/${payoutId}/status`,
      {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      }
    );

    check(statusRes, {
      'status fetch successful': (r) => r.status === 200,
      'status is pending': (r) => r.json('status') === 'pending',
    });
  }

  // Simulate user think time (1-3 seconds)
  sleep(Math.random() * 2 + 1);
}

/**
 * Teardown function - runs once after all tests
 */
export function teardown(data) {
  console.log('=== Payout Load Test Complete ===');
}

/**
 * Handle summary - custom results output
 */
export function handleSummary(data) {
  return {
    'stdout': textSummary(data, { indent: ' ', enableColors: true }),
    'payout-load-test-results.json': JSON.stringify(data),
  };
}

function textSummary(data, options) {
  const summary = [];

  summary.push('\n=== PAYOUT LOAD TEST RESULTS ===\n');

  // Test duration
  const duration = (data.state.testRunDurationMs / 1000).toFixed(2);
  summary.push(`Test Duration: ${duration}s`);

  // Request stats
  const reqCount = data.metrics.http_reqs.values.count;
  const reqRate = data.metrics.http_reqs.values.rate.toFixed(2);
  summary.push(`Total Requests: ${reqCount}`);
  summary.push(`Request Rate: ${reqRate} req/s`);

  // Payout initiation performance
  if (data.metrics.payout_initiation_duration) {
    const p95 = data.metrics.payout_initiation_duration.values['p(95)'].toFixed(2);
    const p99 = data.metrics.payout_initiation_duration.values['p(99)'].toFixed(2);
    const avg = data.metrics.payout_initiation_duration.values.avg.toFixed(2);

    summary.push(`\nPayout Initiation Performance:`);
    summary.push(`  Average: ${avg}ms`);
    summary.push(`  P95: ${p95}ms ${p95 < 2000 ? '✓' : '✗ FAILED'}`);
    summary.push(`  P99: ${p99}ms ${p99 < 3000 ? '✓' : '✗ FAILED'}`);
  }

  // Success rate
  if (data.metrics.payout_success_rate) {
    const successRate = (data.metrics.payout_success_rate.values.rate * 100).toFixed(2);
    summary.push(`\nSuccess Rate: ${successRate}% ${successRate > 95 ? '✓' : '✗ FAILED'}`);
  }

  // Error breakdown
  const failedReqs = data.metrics.http_req_failed.values.rate;
  const errorRate = (failedReqs * 100).toFixed(2);
  summary.push(`\nError Rate: ${errorRate}% ${failedReqs < 0.01 ? '✓' : '✗ FAILED'}`);

  if (data.metrics.insufficient_balance_errors) {
    summary.push(`  Insufficient Balance Errors: ${data.metrics.insufficient_balance_errors.values.count}`);
  }

  if (data.metrics.authentication_errors) {
    summary.push(`  Authentication Errors: ${data.metrics.authentication_errors.values.count}`);
  }

  summary.push('\n=== END OF REPORT ===\n');

  return summary.join('\n');
}
