import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 20 },  // Ramp up to 20 users
    { duration: '1m', target: 20 },   // Stay at 20 users
    { duration: '30s', target: 50 },  // Ramp up to 50 users
    { duration: '1m', target: 50 },   // Stay at 50 users
    { duration: '30s', target: 0 },   // Ramp down to 0 users
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
    http_req_failed: ['rate<0.01'],   // Error rate should be less than 1%
  },
};

const BASE_URL = 'http://localhost:5000';

export default function () {
  // Test 1: Get all transactions
  let res = http.get(`${BASE_URL}/api/transactions`);
  check(res, {
    'get transactions status is 200': (r) => r.status === 200,
    'get transactions response time < 500ms': (r) => r.timings.duration < 500,
  });

  sleep(1);

  // Test 2: Get transaction by status
  res = http.get(`${BASE_URL}/api/transactions/status/Pending`);
  check(res, {
    'get by status is 200': (r) => r.status === 200 || r.status === 404,
  });

  sleep(1);

  // Test 3: Health check
  res = http.get(`${BASE_URL}/health`);
  check(res, {
    'health check is 200': (r) => r.status === 200,
    'health check is healthy': (r) => r.body.includes('Healthy'),
  });

  sleep(1);
}

export function handleSummary(data) {
  return {
    'summary.json': JSON.stringify(data),
    stdout: textSummary(data, { indent: ' ', enableColors: true }),
  };
}

function textSummary(data, options) {
  const { indent = '', enableColors = false } = options || {};

  let summary = `
${indent}scenarios: (100.00%) 1 scenario, ${data.metrics.vus_max.values.max} max VUs, ${Math.ceil(data.state.testRunDurationMs / 1000)}s max duration
${indent}iterations: ${data.metrics.iterations.values.count} (${data.metrics.iterations.values.rate.toFixed(2)}/s)

${indent}checks: ${data.metrics.checks.values.passes} / ${data.metrics.checks.values.fails + data.metrics.checks.values.passes} (${((data.metrics.checks.values.passes / (data.metrics.checks.values.fails + data.metrics.checks.values.passes)) * 100).toFixed(2)}%)

${indent}http_req_duration: avg=${data.metrics.http_req_duration.values.avg.toFixed(2)}ms min=${data.metrics.http_req_duration.values.min.toFixed(2)}ms med=${data.metrics.http_req_duration.values.med.toFixed(2)}ms max=${data.metrics.http_req_duration.values.max.toFixed(2)}ms p(90)=${data.metrics['http_req_duration{expected_response:true}'].values['p(90)'].toFixed(2)}ms p(95)=${data.metrics['http_req_duration{expected_response:true}'].values['p(95)'].toFixed(2)}ms

${indent}http_reqs: ${data.metrics.http_reqs.values.count} (${data.metrics.http_reqs.values.rate.toFixed(2)}/s)
  `;

  return summary;
}
