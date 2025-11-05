# CoinPay Test Suite & QA Infrastructure

**Comprehensive Testing & Quality Assurance Repository**

This is the complete testing infrastructure for the CoinPay cryptocurrency payment platform, including unit tests, integration tests, E2E tests, performance tests, security audits, and QA documentation.

---

## 📋 Overview

This test suite provides comprehensive coverage across all layers:
- **Unit Tests** (.NET/xUnit): Testing individual components and services
- **Integration Tests** (xUnit + Testcontainers): Testing API endpoints with real database
- **E2E Tests** (Playwright/Cypress): Testing complete user workflows
- **Performance Tests** (K6): Load, stress, and spike testing
- **Security Testing**: OWASP Top 10 validation and penetration testing
- **Compliance Testing**: Regulatory and accessibility compliance
- **QA Documentation**: Test plans, bug trackers, and test reports

---

## 📁 Complete Directory Structure

```
CoinPay.Tests/
├── CoinPay.Api.Tests/              # .NET Unit tests for API
│   ├── Services/                    # Service layer tests
│   ├── Controllers/                 # Controller tests
│   └── Repositories/                # Repository tests
├── CoinPay.Integration.Tests/       # .NET Integration tests
│   ├── Api/                         # API endpoint integration tests
│   └── Database/                    # Database integration tests
├── E2E/                             # End-to-end test automation
│   ├── playwright/                  # Playwright tests (27 test cases)
│   ├── cypress/                     # Cypress tests
│   └── reports/                     # Test execution reports
├── Performance/                     # Performance and load testing
│   └── k6/                          # K6 load/stress/spike tests
├── QA/                              # QA test plans and documentation
├── Accessibility/                   # WCAG 2.1 AA compliance testing
├── Compliance/                      # Regulatory compliance testing
├── Security/                        # Security testing artifacts
├── Sprint-N05/                      # Current sprint test artifacts
├── Archive/                         # Archived test artifacts
│   ├── Sprint-N04/                  # Sprint N04 completed tests
│   ├── cypress-old/                 # Legacy Cypress tests
│   ├── e2e-old/                     # Legacy E2E tests
│   └── k6-old/                      # Legacy K6 tests
├── docs/                            # Test documentation
├── COMPREHENSIVE_TEST_REPORT_2025-11-05.md
└── README.md                        # This file
```

---

## ⚡ Prerequisites

- **.NET 9.0 SDK** - For unit and integration tests
- **Node.js 20+** and **npm** - For E2E and performance tests
- **Docker** - For Testcontainers in integration tests
- **K6** - For load and performance testing (optional)

---

## 🚀 Quick Start

```bash
# 1. Install dependencies
dotnet restore
npm install

# 2. Install Playwright browsers
npx playwright install

# 3. Start services
docker-compose up -d

# 4. Run tests
dotnet test                     # Unit tests
cd E2E/playwright && npm test   # E2E tests
```

---

## 🧪 Running Tests

### Unit Tests (.NET/xUnit)

```bash
# Run all unit tests
dotnet test CoinPay.Api.Tests/CoinPay.Api.Tests.csproj

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~AuthServiceTests"
```

### Integration Tests

```bash
# Run all integration tests (requires Docker)
dotnet test CoinPay.Integration.Tests/CoinPay.Integration.Tests.csproj
```

### E2E Tests (Playwright)

```bash
cd E2E/playwright

# Run all tests
npx playwright test

# Run in headed mode
npx playwright test --headed

# Run in UI mode
npx playwright test --ui

# View report
npx playwright show-report
```

### E2E Tests (Cypress)

```bash
cd E2E/cypress

# Interactive mode
npx cypress open

# Headless mode
npx cypress run
```

### Performance Tests (K6)

```bash
cd Performance/k6

# Run load test
k6 run load-test.js

# Run with specific VUs
k6 run --vus 100 --duration 30s load-test.js
```

---

## 📦 NPM Scripts

```bash
npm test              # Run all tests
npm run test:unit     # Unit tests only
npm run test:e2e      # E2E tests
npm run test:e2e:ui   # E2E with UI
npm run test:cypress  # Cypress tests
npm run test:load     # K6 load tests
npm run test:all      # All test types
```

---

## 📊 Test Coverage

### Generating Coverage Reports

```bash
# Generate .NET coverage
dotnet test --collect:"XPlat Code Coverage"

# Install report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

### Coverage Goals

| Test Type | Target | Status |
|-----------|--------|--------|
| Unit Tests | > 80% | 🎯 Target |
| Integration Tests | All endpoints | ✅ In Progress |
| E2E Tests | All critical flows | ⚠️ Partial (40.7%) |
| Security Tests | OWASP Top 10 | ✅ Passed (8/10) |

---

## 📝 Test Categories

### 1. Unit Tests (CoinPay.Api.Tests)
- Service layer logic
- Repository operations
- Controller actions
- **Framework**: xUnit + Moq + FluentAssertions

### 2. Integration Tests (CoinPay.Integration.Tests)
- API endpoint integration
- Database operations
- External service integration
- **Framework**: xUnit + Testcontainers

### 3. E2E Tests (E2E/)
- Authentication flows
- Wallet management
- Transaction processing
- **Frameworks**: Playwright + Cypress

### 4. Performance Tests (Performance/k6/)
- Load testing
- Stress testing
- Spike testing
- **Framework**: K6

### 5. Security Testing
- OWASP Top 10 validation
- Penetration testing
- **Results**: See `Archive/Sprint-N04/security-audit.md`

### 6. Accessibility Testing
- WCAG 2.1 AA compliance
- Keyboard navigation
- Screen reader compatibility

---

## 🔄 Sprint Testing Workflow

1. **Planning**: Create test plan in `Sprint-N0X/QA-50X-Test-Plan.md`
2. **Development**: Prepare test automation
3. **Testing**: Execute tests and document bugs
4. **Reporting**: Generate test reports
5. **Completion**: Archive to `Archive/Sprint-N0X/`

---

## 📈 Quality Metrics

### Current Status
- **Test Pass Rate**: 42.9%
- **Unit Tests**: ❌ Build Errors
- **Integration Tests**: ✅ 100% (1/1)
- **E2E Tests**: ⚠️ 40.7% (11/27)
- **Security Score**: ✅ 8/10 OWASP

### Test Reports
- Latest: `COMPREHENSIVE_TEST_REPORT_2025-11-05.md`
- Sprint N04: `Archive/Sprint-N04/SPRINT_N04_FINAL_TEST_REPORT.md`
- Sprint N05: `Sprint-N05/SPRINT_N05_QA_FINAL_REPORT.md`

---

## 🔧 CI/CD Integration

Tests run automatically on:
- Pull requests
- Pushes to main/development
- Daily scheduled runs

See example GitHub Actions workflow in the comprehensive README section.

---

## 📚 Writing Tests

### Unit Test Example

```csharp
[Fact]
public async Task GetBalance_ShouldReturnCorrectBalance_WhenWalletExists()
{
    // Arrange
    var mockRepo = new Mock<IWalletRepository>();
    mockRepo.Setup(x => x.GetWalletAsync(It.IsAny<Guid>()))
        .ReturnsAsync(new Wallet { Balance = 1000.50m });

    var service = new WalletService(mockRepo.Object);

    // Act
    var result = await service.GetBalanceAsync(Guid.NewGuid());

    // Assert
    result.Should().Be(1000.50m);
}
```

### E2E Test Example

```typescript
test('user should transfer funds successfully', async ({ page }) => {
  await page.goto('/transfer');
  await page.fill('[data-testid="recipient"]', '0x123...');
  await page.fill('[data-testid="amount"]', '10.00');
  await page.click('[data-testid="submit"]');

  await expect(page.locator('[data-testid="success"]')).toBeVisible();
});
```

---

## 🎯 Best Practices

1. ✅ Follow AAA pattern (Arrange-Act-Assert)
2. ✅ Use descriptive test names
3. ✅ One assertion per test
4. ✅ Mock external dependencies
5. ✅ Use data-testid for stable selectors
6. ✅ Keep tests independent
7. ✅ Clean up resources

---

## 🐛 Troubleshooting

**Unit Tests Won't Build**
- Fix project references: `dotnet restore`

**E2E Tests Fail**
- Ensure backend running: `cd CoinPay.Api && dotnet run`
- Ensure frontend running: `cd CoinPay.Web && npm run dev`

**Playwright Issues**
- Install browsers: `npx playwright install --with-deps`

**Docker Issues**
- Restart Docker: `docker-compose down && docker-compose up -d`

---

## 📖 Resources

- [xUnit Documentation](https://xunit.net/)
- [Playwright Documentation](https://playwright.dev/)
- [Cypress Documentation](https://docs.cypress.io/)
- [K6 Documentation](https://k6.io/docs/)
- [OWASP Testing Guide](https://owasp.org/www-project-web-security-testing-guide/)

---

## 🤝 Contributing

1. Identify test category (Unit, Integration, E2E, etc.)
2. Create test files with proper naming
3. Follow best practices
4. Ensure tests pass locally
5. Update documentation
6. Submit pull request

---

## 📞 Support

For testing questions:
1. Check test documentation in sprint folders
2. Review test reports
3. Check troubleshooting section
4. Contact QA team lead
5. Create issue with details

---

## 📋 Current Status

**Last Updated**: 2025-11-05
**Version**: 3.0 (Consolidated)
**Status**: Active - Unified Test Infrastructure

### Recent Changes
- **2025-11-05**: Consolidated `Testing/` into `CoinPay.Tests/`
- **2025-11-05**: Moved Sprint-N04 to Archive
- **2025-11-05**: Merged README documentation

### Known Issues
- ❌ Unit tests have build errors
- ⚠️ E2E tests 59.3% failure rate
- ⏸️ K6 not installed

### Next Steps
1. Fix unit test build errors (P0)
2. Update E2E tests (P1)
3. Install K6 and run performance tests (P2)
4. Expand integration test coverage (P2)

---

**For detailed information, see the full documentation in the comprehensive test report.**
