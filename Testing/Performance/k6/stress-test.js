/**
 * QA-205: Performance Testing with K6
 * Stress Test Script for CoinPay Application
 *
 * This script pushes the system beyond normal load to find breaking points
 * Gradually increases load until system shows signs of stress
 */

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Counter, Rate } from 'k6/metrics';

// Custom metrics
const errors = new Counter('errors');
const errorRate = new Rate('error_rate');

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

// Stress test configuration
export const options = {
  stages: [
    // Ramp to normal load
    { duration: '2m', target: 50 },
    { duration: '3m', target: 50 },

    // Ramp beyond normal capacity
    { duration: '2m', target: 100 },
    { duration: '3m', target: 100 },

    // Push to stress level
    { duration: '2m', target: 200 },
    { duration: '5m', target: 200 },

    // Push further
    { duration: '2m', target: 300 },
    { duration: '5m', target: 300 },

    // Find breaking point
    { duration: '2m', target: 400 },
    { duration: '3m', target: 400 },

    // Recovery test
    { duration: '3m', target: 0 },
  ],

  thresholds: {
    // More lenient thresholds for stress test
    'http_req_duration': ['p(99)<5000'], // 99th percentile < 5s
    'http_req_failed': ['rate<0.1'], // <10% failures acceptable in stress test
    'error_rate': ['rate<0.15'], // <15% error rate at peak stress
  },
};

const testUser = {
  email: 'stress-test@test.com',
  password: 'testpass',
};

function login() {
  const payload = JSON.stringify({
    email: testUser.email,
    password: testUser.password,
  });

  const res = http.post(`${BASE_URL}/api/auth/login`, payload, {
    headers: { 'Content-Type': 'application/json' },
    tags: { endpoint: 'login' },
  });

  const success = check(res, {
    'login successful': (r) => r.status === 200,
  });

  if (!success) {
    errors.add(1);
    errorRate.add(1);
    return null;
  }

  errorRate.add(0);
  return res.json('token');
}

export default function () {
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

  // Aggressive testing - more rapid requests
  group('Stress Load Operations', function () {
    // Rapid balance checks
    for (let i = 0; i < 3; i++) {
      const res = http.get(`${BASE_URL}/api/wallet/balance`, {
        ...authHeaders,
        tags: { endpoint: 'balance' },
      });

      const success = check(res, {
        'balance check successful': (r) => r.status === 200,
      });

      if (!success) {
        errors.add(1);
        errorRate.add(1);
      } else {
        errorRate.add(0);
      }

      sleep(0.1); // Very short sleep
    }

    // Rapid transaction list requests
    for (let i = 0; i < 2; i++) {
      const res = http.get(`${BASE_URL}/api/transactions/history?page=${i + 1}&pageSize=50`, {
        ...authHeaders,
        tags: { endpoint: 'transactions' },
      });

      const success = check(res, {
        'transactions successful': (r) => r.status === 200,
      });

      if (!success) {
        errors.add(1);
        errorRate.add(1);
      } else {
        errorRate.add(0);
      }

      sleep(0.1);
    }
  });

  sleep(Math.random() * 0.5 + 0.5); // 0.5-1s think time
}

export function setup() {
  console.log('Starting stress test...');
  console.log('This test will push the system to its limits');
  console.log(`Target: Up to 400 concurrent users`);

  const healthRes = http.get(`${BASE_URL}/health`);
  if (healthRes.status !== 200) {
    throw new Error('API not reachable');
  }

  return { startTime: Date.now() };
}

export function teardown(data) {
  const duration = (Date.now() - data.startTime) / 1000;
  console.log(`\nStress test completed in ${duration.toFixed(2)} seconds`);
  console.log('Review metrics to identify system breaking points');
}
