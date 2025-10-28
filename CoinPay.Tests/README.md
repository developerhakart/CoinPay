# CoinPay Test Suite

Comprehensive testing infrastructure for the CoinPay cryptocurrency payment platform.

## Overview

This test suite includes:
- **Unit Tests** (xUnit): Testing individual components and services
- **Integration Tests** (xUnit + Testcontainers): Testing API endpoints with real database
- **E2E Tests** (Playwright): Testing complete user workflows
- **Component Tests** (Cypress): Testing React components in isolation
- **Load Tests** (K6): Performance and stress testing

## Project Structure

```
CoinPay.Tests/
├── CoinPay.Api.Tests/          # Unit tests for API
│   ├── Services/                # Service layer tests
│   ├── Controllers/             # Controller tests
│   └── Repositories/            # Repository tests
├── CoinPay.Integration.Tests/   # Integration tests
│   ├── Api/                     # API endpoint tests
│   └── Database/                # Database integration tests
├── e2e/                         # Playwright E2E tests
│   ├── auth.spec.ts             # Authentication flows
│   ├── transactions.spec.ts     # Transaction management
│   └── wallet.spec.ts           # Wallet operations
├── cypress/                     # Cypress component tests
│   ├── e2e/                     # End-to-end scenarios
│   └── component/               # Component tests
├── k6/                          # K6 load tests
│   ├── load-test.js             # Load testing scenarios
│   └── stress-test.js           # Stress testing scenarios
└── docs/                        # Test documentation
    ├── test-strategy.md         # Testing strategy
    ├── test-plan.md             # Test plan
    └── coverage-report.md       # Coverage reports
```

## Prerequisites

- .NET 9.0 SDK
- Node.js 20+ and npm
- Docker (for Testcontainers)
- K6 (for load testing)

## Setup

### Install Dependencies

```bash
# .NET dependencies (automatic with restore)
dotnet restore

# Node.js dependencies
npm install
```

### Install Playwright Browsers

```bash
npx playwright install
```

### Install K6 (Load Testing)

**Windows (using Chocolatey):**
```bash
choco install k6
```

**macOS:**
```bash
brew install k6
```

**Linux:**
```bash
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

## Running Tests

### Unit Tests

```bash
# Run all unit tests
dotnet test CoinPay.Api.Tests/CoinPay.Api.Tests.csproj

# Run with coverage
dotnet test CoinPay.Api.Tests/CoinPay.Api.Tests.csproj --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~AuthServiceTests"
```

### Integration Tests

```bash
# Run all integration tests
dotnet test CoinPay.Integration.Tests/CoinPay.Integration.Tests.csproj

# Requires Docker running for Testcontainers
```

### E2E Tests (Playwright)

```bash
# Run all E2E tests
npx playwright test

# Run specific test file
npx playwright test e2e/auth.spec.ts

# Run in headed mode (see browser)
npx playwright test --headed

# Run in debug mode
npx playwright test --debug

# Generate HTML report
npx playwright show-report
```

### Component Tests (Cypress)

```bash
# Open Cypress Test Runner
npx cypress open

# Run Cypress tests headless
npx cypress run

# Run specific spec
npx cypress run --spec cypress/e2e/transactions.cy.ts
```

### Load Tests (K6)

```bash
# Run load test
k6 run k6/load-test.js

# Run stress test
k6 run k6/stress-test.js

# Run with specific VUs and duration
k6 run --vus 100 --duration 30s k6/load-test.js
```

## Test Scripts

The following npm scripts are available:

```json
{
  "test": "npm run test:unit && npm run test:e2e",
  "test:unit": "dotnet test",
  "test:e2e": "playwright test",
  "test:e2e:ui": "playwright test --ui",
  "test:e2e:headed": "playwright test --headed",
  "test:cypress": "cypress run",
  "test:cypress:open": "cypress open",
  "test:load": "k6 run k6/load-test.js",
  "test:all": "npm run test:unit && npm run test:e2e && npm run test:cypress"
}
```

## Test Coverage

### Viewing Coverage Reports

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Install report generator (one time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Open report
start coveragereport/index.html
```

### Coverage Goals

- **Unit Tests**: > 80% code coverage
- **Integration Tests**: All API endpoints covered
- **E2E Tests**: All critical user workflows covered

## CI/CD Integration

### GitHub Actions

Tests run automatically on:
- Pull requests
- Pushes to main branch
- Daily scheduled runs

See `.github/workflows/test.yml` for configuration.

### Test Environment Variables

For CI/CD, set these environment variables:

```bash
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/coinpay_test

# API
API_BASE_URL=http://localhost:7777
GATEWAY_URL=http://localhost:5000

# Frontend
WEB_BASE_URL=http://localhost:3000

# Circle API (test credentials)
CIRCLE_API_KEY=test_key
CIRCLE_ENTITY_SECRET=test_secret
```

## Writing Tests

### Unit Test Example

```csharp
using Xunit;
using FluentAssertions;
using Moq;

public class ServiceTests
{
    [Fact]
    public async Task MethodName_ShouldReturnExpectedResult_WhenCondition()
    {
        // Arrange
        var mock = new Mock<IDependency>();
        mock.Setup(x => x.Method()).Returns(expectedValue);
        var service = new Service(mock.Object);

        // Act
        var result = await service.MethodUnderTest();

        // Assert
        result.Should().Be(expectedValue);
    }
}
```

### E2E Test Example

```typescript
import { test, expect } from '@playwright/test';

test('feature should work correctly', async ({ page }) => {
  // Arrange
  await page.goto('/feature');

  // Act
  await page.getByRole('button', { name: 'Submit' }).click();

  // Assert
  await expect(page).toHaveURL('/success');
});
```

## Test Data

### Test Users

```
Username: testuser
Password: (passkey-based, simulated in dev mode)
```

### Test Wallets

```
Wallet Address: 0xtest123...
Balance: 1000 USDC
```

## Troubleshooting

### Playwright Tests Failing

- Ensure web server is running on port 3000
- Run `npx playwright install` to install browsers
- Check `playwright.config.ts` for correct baseURL

### Testcontainers Issues

- Ensure Docker is running
- Check Docker Desktop settings for sufficient resources
- Verify network connectivity

### K6 Not Found

- Install K6 using package manager (see Setup section)
- Verify installation: `k6 version`

## Best Practices

1. **Follow AAA Pattern**: Arrange, Act, Assert
2. **Use Descriptive Names**: `MethodName_ShouldExpectedBehavior_WhenCondition`
3. **One Assert Per Test**: Focus on single behavior
4. **Mock External Dependencies**: Use Moq for isolation
5. **Clean Up Resources**: Dispose DbContext, close connections
6. **Use FluentAssertions**: More readable assertions
7. **Tag Tests**: Use `[Trait]` for categorization
8. **Avoid Test Interdependence**: Each test should be independent

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Playwright Documentation](https://playwright.dev/)
- [Cypress Documentation](https://www.cypress.io/)
- [K6 Documentation](https://k6.io/docs/)
- [Testcontainers .NET](https://dotnet.testcontainers.org/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4)
