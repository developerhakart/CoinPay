# TECH-001: Eliminate Hardcoded Localhost URLs - Configuration Management

**Status**: NOT STARTED ⏳
**Priority**: HIGH 🔴
**Type**: Technical Task - Infrastructure
**Sprint**: N02
**Effort Estimate**: 1.5 days
**Owner**: Backend Engineer + Frontend Engineer

---

## 📋 Executive Summary

Eliminate all hardcoded localhost URLs and port numbers from both frontend and backend codebases by moving them to environment-specific configuration files. This improves maintainability, environment portability, and deployment flexibility.

---

## 🎯 Goal

Move all hardcoded localhost URLs and port configurations to:
- **Backend**: `appsettings.json` / `appsettings.Development.json` / `appsettings.Production.json`
- **Frontend**: `.env.development` / `.env.production` / `env.ts`

---

## 🔍 Current State Analysis

### Hardcoded URLs Found:

#### Frontend (CoinPay.Web)
**File**: `src/store/authStore.ts`
- Line 70: `http://localhost:5000/api/auth/login/initiate`
- Line 90: `http://localhost:5000/api/auth/login/complete`
- Line 110: `http://localhost:5000/api/me`
- Line 142: `http://localhost:5000/api/auth/check-username`
- Line 158: `http://localhost:5000/api/auth/register/initiate`
- Line 178: `http://localhost:5000/api/auth/register/complete`

**Note**: Frontend already has `env.ts` with `apiBaseUrl` configuration, but it's not being used in `authStore.ts`.

#### Backend (CoinPay.Api)
**File**: `Program.cs`
- Lines 72-78: CORS DevelopmentPolicy with hardcoded origins:
  - `http://localhost:3000` (React default)
  - `http://localhost:3001` (Vite alternate)
  - `http://localhost:5173` (Vite default)
  - `http://localhost:5100` (Custom frontend)
  - `http://localhost:5174` (Additional Vite)
  - `http://localhost:4200` (Angular default)

**Note**: Backend CORS allows multiple frontend ports but they're hardcoded.

#### Test Files
Multiple test files (Playwright, Cypress, K6) also have hardcoded localhost URLs.

---

## 🛠️ Implementation Plan

### Phase 1: Backend Configuration (BE) - 0.75 days

#### Task BE-TECH-001.1: Add CORS Configuration to appsettings.json
**Effort**: 0.25 days (2 hours)

**Changes Required**:

1. **Update**: `CoinPay.Api/appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ApiSettings": {
    "BaseUrl": "http://localhost:5100",
    "Port": 5100
  },
  "CorsSettings": {
    "AllowedOrigins": []
  }
}
```

2. **Update**: `CoinPay.Api/appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=coinpay;Username=postgres;Password=root"
  },
  "ApiSettings": {
    "BaseUrl": "http://localhost:5100",
    "Port": 5100
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:3001",
      "http://localhost:5173",
      "http://localhost:5100",
      "http://localhost:5174",
      "http://localhost:4200"
    ]
  },
  "Circle": {
    "ApiUrl": "https://api.circle.com/v1/w3s",
    "ApiKey": "TEST_API_KEY_your_actual_key_here",
    "EntitySecret": "TEST_ENTITY_SECRET_your_actual_secret_here",
    "AppId": "TEST_APP_ID_your_actual_app_id_here"
  },
  "Jwt": {
    "Issuer": "CoinPayApi",
    "Audience": "CoinPayClient",
    "SecretKey": "your-super-secret-key-min-32-characters-long-for-HS256",
    "ExpirationMinutes": 1440,
    "RefreshTokenExpirationDays": 7
  }
}
```

3. **Create**: `CoinPay.Api/appsettings.Production.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ApiSettings": {
    "BaseUrl": "https://api.coinpay.com",
    "Port": 443
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "https://app.coinpay.com",
      "https://www.coinpay.com"
    ]
  }
}
```

#### Task BE-TECH-001.2: Update Program.cs to Use Configuration
**Effort**: 0.5 days (4 hours)

**Changes Required**:

**File**: `CoinPay.Api/Program.cs`

1. Add configuration models at the top of `Program.cs`:
```csharp
// Configuration Models
public class ApiSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5100";
    public int Port { get; set; } = 5100;
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
```

2. Read configuration in Program.cs (after line 36):
```csharp
// Load configuration settings
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();

Log.Information("API Settings: BaseUrl={BaseUrl}, Port={Port}", apiSettings.BaseUrl, apiSettings.Port);
Log.Information("CORS Settings: AllowedOrigins={AllowedOrigins}", string.Join(", ", corsSettings.AllowedOrigins));
```

3. Replace hardcoded CORS configuration (lines 66-96):
```csharp
// Add CORS with environment-specific policies
builder.Services.AddCors(options =>
{
    // Development policy - read from configuration
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        var origins = corsSettings.AllowedOrigins;
        if (origins.Length > 0)
        {
            policy.WithOrigins(origins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Correlation-ID");
            Log.Information("CORS configured with {Count} allowed origins", origins.Length);
        }
        else
        {
            // Fallback: Allow all origins in development if not configured
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("X-Correlation-ID");
            Log.Warning("CORS configured to allow ANY origin (no origins specified in configuration)");
        }
    });

    // Production policy - read from configuration
    options.AddPolicy("ProductionPolicy", policy =>
    {
        var origins = corsSettings.AllowedOrigins;
        if (origins.Length > 0)
        {
            policy.WithOrigins(origins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Correlation-ID");
            Log.Information("CORS configured with {Count} allowed origins", origins.Length);
        }
        else
        {
            // Production MUST have origins configured
            throw new InvalidOperationException("CORS AllowedOrigins must be configured in Production");
        }
    });
});
```

**Acceptance Criteria**:
- [ ] Configuration models created
- [ ] CORS origins read from appsettings files
- [ ] API logs configured origins on startup
- [ ] Production throws error if no origins configured
- [ ] Development falls back to allow-all if not configured

---

### Phase 2: Frontend Configuration (FE) - 0.5 days

#### Task FE-TECH-001.1: Update authStore.ts to Use Environment Configuration
**Effort**: 0.5 days (4 hours)

**Changes Required**:

**File**: `CoinPay.Web/src/store/authStore.ts`

1. Import env configuration at the top:
```typescript
import { env } from '@/config/env';
```

2. Replace all hardcoded URLs with `env.apiBaseUrl`:
   - Line 70: `${env.apiBaseUrl}/api/auth/login/initiate`
   - Line 90: `${env.apiBaseUrl}/api/auth/login/complete`
   - Line 110: `${env.apiBaseUrl}/api/me`
   - Line 142: `${env.apiBaseUrl}/api/auth/check-username`
   - Line 158: `${env.apiBaseUrl}/api/auth/register/initiate`
   - Line 178: `${env.apiBaseUrl}/api/auth/register/complete`

**Example**:
```typescript
// BEFORE
const initiateResponse = await fetch('http://localhost:5000/api/auth/login/initiate', {

// AFTER
const initiateResponse = await fetch(`${env.apiBaseUrl}/api/auth/login/initiate`, {
```

**Acceptance Criteria**:
- [ ] All hardcoded URLs in authStore.ts replaced
- [ ] Import statement added
- [ ] Template literals used correctly
- [ ] No TypeScript errors

---

### Phase 3: Environment Files Setup (FE) - 0.25 days

#### Task FE-TECH-001.2: Create Environment-Specific Configuration Files
**Effort**: 0.25 days (2 hours)

**Changes Required**:

1. **Create**: `CoinPay.Web/.env.development`
```bash
# CoinPay Frontend - Development Environment

# API Configuration
VITE_API_BASE_URL=http://localhost:5100
VITE_API_TIMEOUT=30000

# Application Configuration
VITE_APP_NAME=CoinPay
VITE_APP_VERSION=1.0.0

# Feature Flags
VITE_ENABLE_LOGGING=true
VITE_ENABLE_MOCK_API=false

# Environment
VITE_NODE_ENV=development
```

2. **Create**: `CoinPay.Web/.env.production`
```bash
# CoinPay Frontend - Production Environment

# API Configuration
VITE_API_BASE_URL=https://api.coinpay.com
VITE_API_TIMEOUT=30000

# Application Configuration
VITE_APP_NAME=CoinPay
VITE_APP_VERSION=1.0.0

# Feature Flags
VITE_ENABLE_LOGGING=false
VITE_ENABLE_MOCK_API=false

# Environment
VITE_NODE_ENV=production
```

3. **Update**: `CoinPay.Web/.gitignore` (ensure .env files are handled correctly)
```
# Environment files
.env
.env.local
.env.*.local

# Keep example and environment-specific files
!.env.example
!.env.development
!.env.production
```

**Acceptance Criteria**:
- [ ] .env.development created with correct values
- [ ] .env.production created with production URLs
- [ ] .gitignore updated to protect sensitive .env files
- [ ] .env.example kept as template

---

### Phase 4: Documentation & Testing - 0.5 days

#### Task TECH-001.3: Update Documentation
**Effort**: 0.25 days (2 hours)

**Changes Required**:

1. **Update**: `CoinPay.Api/CLAUDE.md`
   - Document new configuration structure
   - Add section on "Port & CORS Configuration"
   - Add examples of changing ports for different environments

2. **Update**: `CoinPay.Web/CLAUDE.md`
   - Document environment variable usage
   - Add section on "API Configuration"
   - Add examples of setting up .env files

3. **Update**: Root `README.md`
   - Document configuration management approach
   - Add "Configuration" section with examples

#### Task TECH-001.4: Testing & Validation
**Effort**: 0.25 days (2 hours)

**Test Cases**:

1. **Backend Configuration Test**:
   - [ ] Start backend with default configuration (reads from appsettings.Development.json)
   - [ ] Verify CORS logs show configured origins
   - [ ] Test CORS preflight from allowed origin
   - [ ] Test CORS rejection from non-allowed origin

2. **Frontend Configuration Test**:
   - [ ] Start frontend with .env.development
   - [ ] Verify API calls go to configured backend URL
   - [ ] Test authentication flow (register/login)
   - [ ] Test wallet operations

3. **Integration Test**:
   - [ ] Start both frontend and backend
   - [ ] Change backend port in appsettings.json
   - [ ] Change frontend API URL in .env.development
   - [ ] Verify full end-to-end flow works

4. **Production Configuration Test**:
   - [ ] Build frontend for production
   - [ ] Verify production environment uses production API URL
   - [ ] Verify backend rejects request if no CORS origins configured in production

**Acceptance Criteria**:
- [ ] All test cases pass
- [ ] Documentation updated
- [ ] Configuration validated in multiple scenarios

---

## 📊 Effort Breakdown

| Task | Owner | Effort | Details |
|------|-------|--------|---------|
| BE-TECH-001.1 | Backend Engineer | 0.25 days | Add CORS config to appsettings |
| BE-TECH-001.2 | Backend Engineer | 0.5 days | Update Program.cs |
| FE-TECH-001.1 | Frontend Engineer | 0.5 days | Update authStore.ts |
| FE-TECH-001.2 | Frontend Engineer | 0.25 days | Create env files |
| TECH-001.3 | Backend/Frontend | 0.25 days | Documentation |
| TECH-001.4 | Backend/Frontend | 0.25 days | Testing |
| **TOTAL** | | **2.0 days** | **Rounded estimate: 1.5-2 days** |

---

## 🎯 Acceptance Criteria

### Backend (CoinPay.Api)
- [ ] No hardcoded localhost URLs in Program.cs
- [ ] CORS origins read from appsettings.json
- [ ] appsettings.Development.json has development origins
- [ ] appsettings.Production.json has production origins
- [ ] API logs configured CORS origins on startup
- [ ] Production mode throws error if CORS not configured

### Frontend (CoinPay.Web)
- [ ] No hardcoded localhost URLs in authStore.ts
- [ ] All API calls use env.apiBaseUrl
- [ ] .env.development exists with correct values
- [ ] .env.production exists with production URLs
- [ ] .gitignore properly handles env files

### Testing
- [ ] Backend starts successfully with configured CORS
- [ ] Frontend connects to backend using configured URL
- [ ] CORS allows requests from configured origins
- [ ] Full authentication flow works end-to-end
- [ ] Configuration can be changed without code changes

### Documentation
- [ ] CLAUDE.md files updated with configuration guide
- [ ] README.md includes configuration section
- [ ] Comments added to configuration files

---

## 🔗 Related Files

### Backend Files to Modify:
- `CoinPay.Api/Program.cs`
- `CoinPay.Api/appsettings.json`
- `CoinPay.Api/appsettings.Development.json`
- `CoinPay.Api/appsettings.Production.json` (create)
- `CoinPay.Api/CLAUDE.md`

### Frontend Files to Modify:
- `CoinPay.Web/src/store/authStore.ts`
- `CoinPay.Web/.env.development` (create)
- `CoinPay.Web/.env.production` (create)
- `CoinPay.Web/.env.example` (update)
- `CoinPay.Web/.gitignore` (verify)
- `CoinPay.Web/CLAUDE.md`

### Documentation Files:
- `README.md`

---

## 🚧 Potential Blockers

1. **Breaking Changes**: This task changes how configuration works. Ensure all developers update their local configuration files.
2. **Test Updates**: Test files (Playwright, Cypress, K6) may also need configuration updates (stretch goal).
3. **Docker Configuration**: Docker compose files may need environment variable updates (stretch goal).

---

## 📚 Additional Improvements (Future/Stretch Goals)

1. **Test Configuration**: Update Playwright, Cypress, and K6 test files to use environment variables
2. **Docker Configuration**: Update docker-compose.yml to use environment variables
3. **Gateway Configuration**: Add configuration management to CoinPay.Gateway if needed
4. **Validation**: Add startup validation to ensure all required configuration values are present
5. **Secret Management**: Consider using Azure Key Vault or similar for production secrets

---

## ✅ Definition of Done

- [ ] All hardcoded localhost URLs eliminated from backend
- [ ] All hardcoded localhost URLs eliminated from frontend
- [ ] Configuration files properly structured
- [ ] Environment-specific configuration working (Dev/Prod)
- [ ] All tests passing
- [ ] Documentation updated
- [ ] Code reviewed by Team Lead
- [ ] Tested in local development environment
- [ ] Configuration validated for production readiness

---

**Created**: 2025-10-29
**Last Updated**: 2025-10-29
**Version**: 1.0
