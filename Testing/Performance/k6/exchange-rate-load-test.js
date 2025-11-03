/**
 * K6 Load Test - Exchange Rate API
 * Sprint N03 - Phase 3: Fiat Off-Ramp
 *
 * Test Scenario:
 * - High-frequency exchange rate fetching
 * - Tests caching effectiveness
 * - Validates rate refresh performance
 *
 * Performance Targets:
 * - P95 < 500ms (cached)
 * - P95 < 1000ms (fresh fetch)
 * - Error rate < 0.1%
 *
 * Run: k6 run exchange-rate-load-test.js
 */

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const cachedRateDuration = new Trend('cached_rate_duration');
const freshRateDuration = new Trend('fresh_rate_duration');
const cacheHitRate = new Rate('cache_hit_rate');

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5100';

export const options = {
  stages: [
    { duration: '30s', target: 50 },   // Ramp up
    { duration: '2m', target: 200 },   // High load
    { duration: '30s', target: 0 },    // Ramp down
  ],
  thresholds: {
    'cached_rate_duration': ['p(95)<500'],
    'fresh_rate_duration': ['p(95)<1000'],
    'http_req_failed': ['rate<0.001'],
    'cache_hit_rate': ['rate>0.9'],  // >90% cache hits expected
  },
};

export function setup() {
  console.log('=== Exchange Rate Load Test ===');

  const healthRes = http.get(`${BASE_URL}/health`);
  check(healthRes, {
    'API healthy': (r) => r.status === 200,
  });

  return { baseUrl: BASE_URL };
}

export default function (data) {
  const startTime = Date.now();

  const rateRes = http.get(`${data.baseUrl}/api/rates/usdc-usd`, {
    tags: { operation: 'rate_fetch' },
  });

  const duration = Date.now() - startTime;

  const rateCheck = check(rateRes, {
    'rate fetch successful': (r) => r.status === 200,
    'valid rate': (r) => r.json('rate') > 0 && r.json('rate') < 2,
    'has expiration': (r) => r.json('expiresAt') !== undefined,
  });

  // Track if response was cached
  const isCached = rateRes.json('isCached');

  if (isCached) {
    cachedRateDuration.add(duration);
    cacheHitRate.add(1);
  } else {
    freshRateDuration.add(duration);
    cacheHitRate.add(0);
  }

  sleep(0.5); // Minimal think time for high-frequency test
}
