/**
 * QA-205: Performance Testing with K6
 * Spike Test Script for CoinPay Application
 *
 * Tests system behavior during sudden traffic spikes
 * Simulates viral marketing campaign or media coverage
 */

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('error_rate');
const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

// Spike test configuration
export const options = {
  stages: [
    // Normal load
    { duration: '30s', target: 20 },

    // SPIKE! Sudden 10x increase
    { duration: '10s', target: 200 },

    // Hold spike
    { duration: '2m', target: 200 },

    // Drop back to normal
    { duration: '10s', target: 20 },

    // Recovery period
    { duration: '2m', target: 20 },

    // Second spike
    { duration: '10s', target: 300 },
    { duration: '1m', target: 300 },

    // Ramp down
    { duration: '30s', target: 0 },
  ],

  thresholds: {
    'http_req_duration': ['p(95)<3000'], // Lenient during spike
    'http_req_failed': ['rate<0.05'], // <5% failures
    'error_rate': ['rate<0.1'], // <10% errors during spike
  },
};

export default function () {
  // Quick login
  const loginRes = http.post(
    `${BASE_URL}/api/auth/login`,
    JSON.stringify({
      email: 'spike-test@test.com',
      password: 'testpass',
    }),
    {
      headers: { 'Content-Type': 'application/json' },
      tags: { endpoint: 'login' },
    }
  );

  if (loginRes.status !== 200) {
    errorRate.add(1);
    sleep(1);
    return;
  }

  const token = loginRes.json('token');
  const authHeaders = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  };

  // Critical path only during spike
  const balanceRes = http.get(`${BASE_URL}/api/wallet/balance`, {
    ...authHeaders,
    tags: { endpoint: 'balance' },
  });

  const success = check(balanceRes, {
    'balance check successful': (r) => r.status === 200,
    'response time acceptable': (r) => r.timings.duration < 5000,
  });

  if (!success) {
    errorRate.add(1);
  } else {
    errorRate.add(0);
  }

  sleep(0.5);
}

export function setup() {
  console.log('Starting spike test...');
  console.log('Simulating sudden traffic surge');
  console.log('20 → 200 → 20 → 300 → 0 users');

  const healthRes = http.get(`${BASE_URL}/health`);
  if (healthRes.status !== 200) {
    throw new Error('API not reachable');
  }

  return { startTime: Date.now() };
}

export function teardown(data) {
  const duration = (Date.now() - data.startTime) / 1000;
  console.log(`\nSpike test completed in ${duration.toFixed(2)} seconds`);
  console.log('Check if system recovered after spike');
}
