# CoinPay Wallet MVP - Sprint 1 Master Plan

**Version**: 1.0
**Sprint Duration**: 2 weeks (10 working days)
**Sprint Dates**: January 6-17, 2025
**Document Status**: Ready for Execution
**Last Updated**: 2025-10-26
**Owner**: Team Lead

---

## Executive Summary

### Sprint Overview

Sprint 1 is a **Foundation Sprint** focused on establishing the core technical infrastructure and QA capabilities required for the CoinPay Wallet MVP. This sprint sets the groundwork for all subsequent development phases.

**Sprint Goal**: Establish a fully operational backend infrastructure with Circle SDK integration for passkey-based wallet creation and gasless USDC transfers, while simultaneously building comprehensive QA testing infrastructure.

### Total Team Capacity

| Team | Engineers | Available Days | Planned Effort | Utilization |
|------|-----------|---------------|----------------|-------------|
| Backend | 2-3 | 20-30 days | ~29 days | 97% |
| Frontend | TBD | TBD | **PLAN MISSING** | - |
| QA | 2-3 | 20-30 days | ~22 days | 73% |
| **Total** | **4-6** | **40-60 days** | **~51 days** | **85%** |

### Sprint Dates

- **Start Date**: Monday, January 6, 2025
- **End Date**: Friday, January 17, 2025
- **Working Days**: 10 days
- **Mid-Sprint Checkpoint**: Friday, January 10, 2025 (Day 5)
- **Sprint Review**: Friday, January 17, 2025 (Day 10)

### Critical Success Factors

1. **Backend**: Circle SDK integration operational with passkey authentication
2. **Backend**: Gasless USDC transfer capability functional on Polygon Amoy testnet
3. **QA**: Test automation infrastructure established and operational
4. **Cross-Team**: API contracts defined and documented for Frontend integration
5. **Critical Path**: No blockers preventing Phase 1 (Core Wallet Foundation) implementation in Sprint 2

---

## CRITICAL ISSUE: Missing Frontend Plan

**STATUS**: The Frontend Sprint 1 plan is **MISSING**. This creates a coordination risk for the sprint.

**Impact**:
- Frontend team capacity and tasks unknown
- Frontend-Backend integration points undefined
- API contract review cannot be scheduled without Frontend plan
- Sprint capacity planning incomplete

**Immediate Actions Required**:
1. Frontend Engineer must create Sprint 1 Frontend Plan immediately (Day 1)
2. Schedule emergency alignment meeting: Backend Lead + Frontend Lead + Team Lead
3. Define minimum Frontend deliverables for Sprint 1:
   - React project setup
   - Vite/CRA configuration
   - Basic component structure
   - API client setup
   - WebAuthn passkey integration (client-side)
   - UI mockups for wallet creation and transfer flows

**Mitigation**:
- Backend team proceeds with API development as planned
- Backend documents API contracts in Swagger (self-sufficient)
- QA team prepares mock-based testing approach
- Frontend plan must be ready by Day 2 to minimize disruption

---

## Team Goals

### Backend Team Sprint Goal

**Primary Goal**: Establish core backend infrastructure and integrate Circle SDK for passkey-based wallet creation and gasless USDC transfers on Polygon Amoy testnet.

**Key Deliverables**:
- ASP.NET Core API with clean architecture
- PostgreSQL database with EF Core migrations
- Hashicorp Vault integration for secrets management
- Circle SDK integration with passkey authentication
- Wallet creation endpoint operational
- Basic USDC transfer capability (gasless)
- YARP API Gateway configured
- Health checks and structured logging
- Comprehensive API documentation (Swagger)

**Success Metrics**:
- All infrastructure tasks (Phase 0: 9.88 days) completed
- Core wallet foundation tasks (Phase 1: 19.08 days) - 65% complete minimum
- API endpoints documented and accessible via Swagger
- Integration tests pass for Circle SDK calls
- At least one end-to-end gasless transfer demonstrated on testnet

### Frontend Team Sprint Goal

**STATUS**: **PLAN MISSING - TO BE DEFINED**

**Assumed Minimum Deliverables** (to be confirmed):
- React application setup (Vite or CRA)
- TypeScript configuration
- Tailwind CSS or UI framework setup
- WebAuthn passkey integration (client-side)
- Basic authentication flow UI
- API client with axios/fetch configuration
- Environment configuration for dev/staging

### QA Team Sprint Goal

**Primary Goal**: Establish comprehensive QA infrastructure and testing foundation to support CoinPay Wallet MVP development.

**Key Deliverables**:
- Test environment configuration with Circle testnet and Polygon Amoy access
- Test automation framework installation (Playwright, Cypress, K6)
- Test data management strategy and implementation
- CI/CD integration for automated testing
- QA strategy and test plan documentation
- Quality gates and Definition of Done defined
- Team training on test frameworks

**Success Metrics**:
- All test environments accessible and documented
- Minimum 10 test wallets funded with testnet USDC
- CI/CD pipeline runs tests automatically on PR
- Test strategy documented and approved
- All 16 QA infrastructure tasks completed
- Team trained on Playwright, Cypress, K6

---

## Consolidated Task List

### Phase 0: Infrastructure Setup

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| BE-001 | Initialize ASP.NET Core Project | Backend | 0.54 | Senior BE | None | 1 |
| BE-002 | Configure Development Environment | Backend | 0.54 | BE-1 | BE-001 | 1 |
| BE-003 | Setup PostgreSQL Database | Backend | 1.08 | BE-1 | BE-002 | 1 |
| BE-004 | Configure Hashicorp Vault | Backend | 2.00 | Senior BE | BE-002 | 1 |
| BE-005 | Setup Structured Logging (Serilog) | Backend | 1.00 | BE-1 | BE-001 | 1 |
| BE-006 | Implement Health Check Endpoints | Backend | 1.00 | BE-1 | BE-003, BE-004 | 1 |
| BE-007 | Configure YARP Gateway | Backend | 2.00 | Senior BE | BE-001 | 2 |
| BE-008 | Configure CORS Policies | Backend | 0.54 | BE-1 | BE-007 | 2 |
| BE-009 | Global Exception Handling Middleware | Backend | 1.08 | BE-1 | BE-005 | 2 |
| QA-001 | Configure test environments | QA | 1.67 | QA Lead | DevOps access | 1 |
| QA-002 | Test data management setup | QA | 1.08 | QA-1 | Polygon Amoy | 1 |
| QA-003 | CI/CD test integration | QA | 2.00 | QA-2 | GitHub Actions | 1-2 |
| QA-004 | Configure test reporting | QA | 1.00 | QA-2 | QA-003 | 2 |
| QA-005 | Setup mock services | QA | 1.58 | QA-1 | Backend API specs | 1-2 |
| **FE-TBD** | **Frontend Infrastructure Tasks** | **Frontend** | **TBD** | **TBD** | **TBD** | **TBD** |

**Phase 0 Subtotal**: ~17.11 days (Backend: 9.88 days, QA: 7.23 days, Frontend: TBD)

### Phase 1: Circle SDK Integration (Backend Focus)

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| BE-101 | Configure Circle Modular Wallets SDK | Backend | 2.00 | Senior BE | BE-004 | 1-2 |
| BE-102 | Environment Configuration Service | Backend | 1.00 | BE-1 | BE-004, BE-101 | 2 |
| BE-103 | POST /api/auth/register Endpoint | Backend | 2.00 | BE-1 | BE-102 | 2 |
| BE-104 | POST /api/auth/verify Endpoint | Backend | 2.17 | BE-1 | BE-103, BE-105 | 2 |
| BE-105 | Session Management (JWT) | Backend | 2.00 | Senior BE | BE-001 | 2 |
| BE-106 | POST /api/wallet/create Endpoint | Backend | 3.08 | Senior BE | BE-101, BE-105 | 2 |
| BE-107 | Wallet Repository | Backend | 1.08 | BE-1 | BE-003, BE-106 | 2 |
| BE-108 | GET /api/wallet/{address}/balance | Backend | 2.00 | BE-1 | BE-112, BE-107 | 2 |
| BE-109 | POST /api/transactions/transfer | Backend | 4.00 | Senior BE | BE-113, BE-114 | 2 |
| BE-110 | GET /api/transactions/{id}/status | Backend | 2.00 | BE-1 | BE-109, BE-111 | 2 |
| BE-111 | Transaction Repository | Backend | 1.00 | BE-1 | BE-003 | 2 |
| BE-112 | Blockchain RPC Service | Backend | 2.17 | BE-1 | BE-102 | 2 |
| BE-113 | UserOperation Service | Backend | 3.17 | Senior BE | BE-112, BE-101 | 2 |
| BE-114 | Paymaster Integration | Backend | 2.17 | Senior BE | BE-113 | 2 |

**Phase 1 Subtotal**: ~29.84 days (Backend only, Frontend tasks TBD)

### QA Infrastructure Tasks

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| QA-101 | Install Playwright | QA | 1.50 | QA-1 | Node.js | 1 |
| QA-102 | Configure Playwright for passkey testing | QA | 1.50 | QA-1 | QA-101 | 1-2 |
| QA-103 | Install Cypress | QA | 1.00 | QA-2 | Node.js | 1 |
| QA-104 | Configure Cypress base setup | QA | 1.50 | QA-2 | QA-103 | 1 |
| QA-105 | Install Grafana K6 | QA | 0.75 | QA Lead | K6 binaries | 1 |
| QA-106 | Create K6 baseline scripts | QA | 1.25 | QA Lead | QA-105 | 1-2 |
| QA-107 | Test strategy documentation | QA | 2.00 | QA Lead | Team alignment | 1 |
| QA-108 | Test plan template creation | QA | 1.50 | QA Lead | QA-107 | 2 |
| QA-109 | Quality gates definition | QA | 1.00 | QA Lead | Team collab | 2 |
| QA-110 | Bug reporting standards | QA | 0.75 | QA-1 | GitHub Issues | 2 |
| QA-111 | Test case repository setup | QA | 1.00 | QA-2 | Test tool | 2 |

**QA Infrastructure Subtotal**: ~14.75 days

### Grand Total: ~61.70 days (without Frontend tasks)

---

## Week-by-Week Plan

### Week 1 (Days 1-5): January 6-10, 2025

#### Backend Team - Week 1

**Days 1-2 (Infrastructure Foundation)**:
- BE-001: ASP.NET Core project initialization
- BE-002: Development environment setup (Docker Compose, PostgreSQL)
- BE-003: PostgreSQL database with EF Core migrations
- BE-005: Structured logging with Serilog
- **Deliverable**: Team can run API locally with database connectivity

**Days 3-4 (Security & Observability)**:
- BE-004: Hashicorp Vault integration (2 days)
- BE-006: Health check endpoints
- BE-008: CORS configuration
- **Deliverable**: Secrets managed securely, health checks operational

**Day 5 (Mid-Sprint Checkpoint)**:
- BE-007: YARP gateway configuration (started)
- BE-009: Global exception handling middleware
- BE-101: Circle SDK client wrapper (started)
- **Checkpoint Demo**: Show project structure, health checks, logging with correlation IDs

#### QA Team - Week 1

**Day 1 (Kickoff & Environment)**:
- Sprint planning meeting
- QA-001: Request Circle Console access, configure Polygon Amoy RPC
- QA-101: Install Playwright
- QA-103: Install Cypress

**Day 2 (Environment & Framework Setup)**:
- QA-001: Complete test environment setup
- QA-002: Generate test wallets
- QA-101: Configure Playwright base setup
- QA-104: Configure Cypress base setup

**Day 3 (Test Data & Mocks)**:
- QA-002: Fund wallets, create test users
- QA-005: Setup mock server infrastructure
- QA-102: Configure Playwright for WebAuthn
- QA-105: Install Grafana K6
- QA-107: Begin test strategy documentation

**Day 4 (CI/CD & Mocks)**:
- QA-003: Configure GitHub Actions workflow
- QA-005: Create WhiteBit API mocks
- QA-106: Create K6 baseline scripts
- QA-107: Complete test strategy documentation

**Day 5 (Mid-Sprint Checkpoint)**:
- QA-004: Configure Allure reporting
- QA-108: Create test plan templates
- QA-110: Create bug report template
- **Checkpoint Demo**: Show test strategy, CI/CD pipeline, test frameworks

#### Frontend Team - Week 1

**STATUS**: **PLAN MISSING**

**Recommended Minimum Tasks** (to be confirmed by Frontend Lead):
- FE-001: Initialize React project (Vite/CRA)
- FE-002: TypeScript and ESLint configuration
- FE-003: Tailwind CSS or UI framework setup
- FE-004: Project folder structure and routing
- FE-005: Environment configuration (.env files)
- FE-006: API client service setup
- **Deliverable**: Frontend app runs locally, can make test API calls

---

### Week 2 (Days 6-10): January 13-17, 2025

#### Backend Team - Week 2

**Days 6-7 (Gateway & Authentication)**:
- BE-007: YARP gateway (completed)
- BE-009: Exception handling (completed)
- BE-101: Circle SDK client (completed)
- BE-102: Circle environment configuration service
- BE-103: Passkey registration endpoint
- BE-104: Passkey verification endpoint
- BE-105: JWT session management
- **Deliverable**: Users can register and login with passkeys

**Days 8-9 (Wallet & Transaction Core)**:
- BE-106: Wallet creation endpoint
- BE-107: Wallet repository
- BE-112: Blockchain RPC service
- BE-113: UserOperation service
- BE-114: Paymaster integration
- BE-109: Transfer endpoint (started)
- **Deliverable**: Wallet creation works, transaction infrastructure ready

**Day 10 (Sprint Review)**:
- BE-109: Transfer endpoint (completed - stretch goal)
- BE-110: Transaction status endpoint (stretch goal)
- BE-111: Transaction repository (stretch goal)
- **Sprint Demo**: Show wallet creation, gasless USDC transfer end-to-end
- **Deliverable**: Functional gasless transfer demonstrated on testnet

#### QA Team - Week 2

**Day 6 (Quality Gates & Repository)**:
- QA-109: Define quality gates and DoD
- QA-111: Setup test case repository
- QA-003: Complete CI/CD integration
- QA-005: Complete Fiat Gateway mocks

**Day 7 (Test Cases & Polish)**:
- QA-111: Create Phase 1 test cases (passkey, wallet, transfer)
- QA-102: Complete Playwright WebAuthn configuration
- QA-106: Complete K6 baseline scripts
- QA-108: Create test plan examples

**Day 8 (Documentation & Validation)**:
- Document all frameworks (Playwright, Cypress, K6)
- QA-110: Complete bug reporting guide
- QA-004: Validate test reporting dashboard
- Run end-to-end validation of all test infrastructure

**Day 9 (Team Training)**:
- Conduct Playwright training session
- Conduct Cypress training session
- Conduct K6 training session
- Knowledge transfer to Backend/Frontend teams

**Day 10 (Sprint Review & Retrospective)**:
- **Sprint Demo**: Show test infrastructure, CI/CD pipeline, test frameworks
- Sprint retrospective
- Sprint 2 planning preparation
- Create Sprint 1 success report

#### Frontend Team - Week 2

**STATUS**: **PLAN MISSING**

**Recommended Minimum Tasks** (to be confirmed):
- FE-007: WebAuthn passkey integration (client-side)
- FE-008: Authentication context and state management
- FE-009: Login/Register UI components
- FE-010: Wallet creation UI
- FE-011: Transfer UI mockup
- FE-012: Integration with Backend API
- **Deliverable**: Basic UI flows functional, can register/login with passkeys

---

## Critical Path Analysis

### Critical Path Items (Must Complete for Sprint Success)

**Week 1 Critical Path**:
1. BE-001 → BE-002 → BE-003 (Database foundation) - **3 days**
2. BE-001 → BE-004 (Vault secrets management) - **2.5 days**
3. BE-004 → BE-101 (Circle SDK client) - **2 days**
4. QA-001 (Test environments) → QA-002 (Test data) - **2.75 days**

**Week 2 Critical Path**:
1. BE-101 → BE-102 → BE-106 (Wallet creation endpoint) - **6 days**
2. BE-101 → BE-112 → BE-113 → BE-114 → BE-109 (Transfer capability) - **11.34 days**
3. BE-105 (JWT sessions) → BE-104 (Passkey verification) - **4.17 days**

**Total Critical Path Duration**: ~13.34 days (Backend only)

**Critical Path Risk**: The transfer capability path (11.34 days) exceeds the 10-day sprint duration by 1.34 days. This is mitigated by parallel work streams and the fact that BE-109/110/111 are stretch goals.

### Blocking Dependencies

**Hard Blockers** (will stop all progress):
1. **Circle Console Access** (Day 1) - Blocks passkey domain configuration
2. **Circle API Keys** (Day 1-2) - Blocks all Circle SDK integration
3. **PostgreSQL Setup** (Day 1-2) - Blocks all database operations
4. **Vault Configuration** (Day 2-3) - Blocks secrets management

**Soft Blockers** (can be worked around):
1. Frontend plan missing - Backend can proceed independently with Swagger docs
2. Polygon Amoy RPC downtime - Can use alternative RPC endpoints
3. GitHub Actions access - Can run tests locally until CI/CD is configured

### Buffer Analysis

**Planned vs Available Capacity**:
- Backend: 29 days planned vs 20-30 days available = **97% utilization** (minimal buffer)
- QA: 22 days planned vs 20-30 days available = **73% utilization** (good buffer)
- Frontend: **Unknown** (plan missing)

**Risk Mitigation**:
- Backend has stretch goals (BE-110, BE-111) that can be deferred to Sprint 2
- QA has ~8 days of buffer for unexpected issues
- Frontend buffer unknown - needs immediate planning

---

## Integration Points & Coordination

### Day 3: Backend + Frontend API Contract Review

**Participants**: Backend Lead, Frontend Lead, Team Lead
**Duration**: 1 hour
**Agenda**:
- Review Swagger API documentation for authentication endpoints
- Define request/response DTO formats
- Agree on error handling conventions
- Define WebAuthn passkey credential format
- Review JWT token structure and claims
- Agree on API versioning strategy

**Prerequisites**:
- BE-001, BE-005 completed (Swagger documentation available)
- Frontend plan created
- Frontend API client design ready

**Deliverables**:
- API contract document
- Error response format standardized
- Passkey credential format agreed upon

---

### Day 5: Mid-Sprint Checkpoint (All Teams)

**Participants**: All team members
**Duration**: 1.5 hours
**Agenda**:
1. **Backend Demo** (20 min):
   - Show project structure and clean architecture
   - Demonstrate health check endpoints
   - Show structured logging with correlation IDs
   - Show Vault integration

2. **Frontend Demo** (20 min):
   - Show React app structure
   - Demonstrate basic routing
   - Show API client configuration
   - **STATUS**: Cannot demo without Frontend plan

3. **QA Demo** (20 min):
   - Show test strategy document
   - Demonstrate CI/CD pipeline (initial version)
   - Show test frameworks installed (Playwright, Cypress, K6)
   - Show test environment access

4. **Blockers & Risks** (20 min):
   - Identify any blockers
   - Escalate external dependencies (Circle access, etc.)
   - Adjust Week 2 plans if needed

5. **Sprint Health Check** (10 min):
   - Velocity tracking (tasks completed vs planned)
   - Capacity adjustment if needed
   - Re-prioritize tasks if behind

**Success Criteria**:
- All infrastructure tasks on track
- No critical blockers identified
- Team aligned on Week 2 priorities

---

### Day 7: Backend + QA Test Environment Handoff

**Participants**: Backend Engineers, QA Engineers
**Duration**: 1 hour
**Agenda**:
- Backend demonstrates API endpoints in dev environment
- QA validates API accessibility from test environment
- Review test data requirements
- Handoff Swagger documentation
- Review API authentication flow
- Setup test user accounts

**Prerequisites**:
- BE-103, BE-104, BE-105 completed (authentication endpoints ready)
- QA-001, QA-002 completed (test environment and data ready)

**Deliverables**:
- QA can access Backend API
- Test user credentials shared
- Postman collection with example requests
- Test data inventory updated

---

### Day 8: Frontend + QA UI Testing Handoff

**Participants**: Frontend Engineers, QA Engineers
**Duration**: 1 hour
**Agenda**:
- Frontend demonstrates UI components in dev environment
- QA validates UI accessibility from test environment
- Review data-testid conventions for test selectors
- Review accessibility requirements (ARIA labels, keyboard nav)
- Handoff component documentation
- Review error handling and loading states

**Prerequisites**:
- Frontend UI components deployed to dev environment
- QA-101, QA-102, QA-103, QA-104 completed (test frameworks ready)

**Deliverables**:
- QA can access Frontend app
- data-testid conventions documented
- Accessibility requirements reviewed
- Page object models started

**STATUS**: Cannot schedule without Frontend plan

---

### Day 10: Sprint Review and Demo

**Participants**: All team members, Product Owner, Stakeholders
**Duration**: 2 hours
**Agenda**:

**1. Sprint Overview** (10 min) - Team Lead:
- Sprint goal recap
- Team capacity and utilization
- Key achievements

**2. Backend Demo** (30 min) - Backend Lead:
- Project structure and architecture
- Health check endpoints (/health, /health/ready)
- Structured logging with correlation IDs
- Vault integration for secrets
- Passkey registration (POST /api/auth/register)
- Passkey login (POST /api/auth/verify)
- Wallet creation (POST /api/wallet/create)
- USDC balance query (GET /api/wallet/{address}/balance)
- **Highlight**: Gasless USDC transfer (POST /api/transactions/transfer)
- Transaction status (GET /api/transactions/{id}/status) - if completed
- Swagger documentation walkthrough

**3. Frontend Demo** (20 min) - Frontend Lead:
- React app structure
- WebAuthn passkey integration (client-side)
- Authentication flow (register/login UI)
- Wallet creation UI
- Transfer UI
- **STATUS**: Cannot demo without Frontend plan

**4. QA Demo** (20 min) - QA Lead:
- Test strategy overview
- CI/CD pipeline demonstration
- Test frameworks (Playwright, Cypress, K6)
- Test environment and test data
- Mock services for external APIs
- Test reporting dashboard
- Quality gates and DoD

**5. Metrics & Retrospective Preview** (10 min) - Team Lead:
- Sprint metrics (velocity, task completion)
- Challenges encountered
- Lessons learned
- Preview Sprint 2 goals

**6. Q&A** (20 min) - All:
- Stakeholder questions
- Feedback collection
- Next steps

**Success Criteria**:
- End-to-end gasless USDC transfer demonstrated
- All infrastructure operational
- QA testing framework ready for Sprint 2
- Stakeholders confident in technical foundation

---

## Cross-Team Dependencies

### Frontend Dependencies on Backend

| Dependency | Required By | Impact if Delayed | Status |
|------------|-------------|-------------------|--------|
| Swagger API documentation | Day 3 | Cannot design API client | Day 2-3 |
| Authentication endpoints | Day 7 | Cannot build login/register UI | Day 7-8 |
| Wallet creation endpoint | Day 8 | Cannot build wallet UI | Day 8-9 |
| Transfer endpoint | Day 9 | Cannot build transfer UI | Day 9-10 |
| CORS configuration | Day 3 | API calls blocked | Day 2 |
| JWT token format | Day 7 | Cannot store sessions | Day 7 |
| Error response format | Day 3 | Cannot handle errors | Day 2 |

**Mitigation**: Backend to prioritize API documentation (Swagger) and share early.

### QA Dependencies on Backend

| Dependency | Required By | Impact if Delayed | Status |
|------------|-------------|-------------------|--------|
| Backend API dev environment | Day 2 | Cannot test API integration | Day 2 |
| API specifications (Swagger) | Day 3 | Cannot create accurate mocks | Day 3 |
| Test database access | Day 3 | Cannot seed test data | Day 3 |
| Sample API responses | Day 4 | Mock services incomplete | Day 4 |
| Health check endpoints | Day 2 | Cannot monitor environments | Day 1-2 |

**Mitigation**: QA to use mock data until real APIs available.

### QA Dependencies on Frontend

| Dependency | Required By | Impact if Delayed | Status |
|------------|-------------|-------------------|--------|
| Frontend dev environment | Day 2 | Cannot test UI automation | **UNKNOWN** |
| Component structure | Day 5 | Cannot create page objects | **UNKNOWN** |
| data-testid attributes | Day 7 | Test selectors fragile | **UNKNOWN** |
| Sample UI flows | Day 8 | Cannot validate E2E scenarios | **UNKNOWN** |

**Mitigation**: QA to use static HTML mockups for initial framework setup.

**STATUS**: All Frontend dependencies at risk due to missing plan.

### Backend Dependencies on External Services

| Dependency | Required By | Impact if Delayed | Mitigation |
|------------|-------------|-------------------|------------|
| Circle Console Access | Day 1 | Blocker - Cannot create Client Key | Escalate to PM immediately |
| Circle API Keys | Day 2 | Blocker - SDK won't work | Contact Circle support Day 1 |
| Polygon Amoy RPC | Day 8 | Balance queries fail | Use multiple RPC endpoints |
| Vault Setup | Day 3 | Store secrets in appsettings temporarily | Setup Vault locally first |

**Critical**: Circle Console access and API keys are hard blockers. Must be obtained by Day 1-2.

---

## Risk Summary

### High Risks (Priority 1)

| Risk ID | Risk | Probability | Impact | Mitigation Strategy | Owner |
|---------|------|-------------|--------|---------------------|-------|
| R-001 | **Frontend plan missing** | **100%** | **CRITICAL** | Frontend Lead creates plan Day 1, emergency alignment meeting | Team Lead |
| R-002 | Circle Console access delayed | Medium | Critical | Request access Day 1, escalate to PM if delayed >24 hours | Backend Lead |
| R-003 | Circle API keys unavailable | Medium | Critical | Contact Circle support immediately, escalate if no response | Backend Lead |
| R-004 | ERC-4337 UserOperation construction difficult | High | High | Reference Circle SDK examples, allocate senior engineer, consult Circle support | Backend Lead |
| R-005 | Backend capacity insufficient (97% utilization) | Medium | High | Defer BE-110, BE-111 to Sprint 2 if needed | Backend Lead |

### Medium Risks (Priority 2)

| Risk ID | Risk | Probability | Impact | Mitigation Strategy | Owner |
|---------|------|-------------|--------|---------------------|-------|
| R-006 | WebAuthn passkey integration unclear | Medium | Medium | Research WebAuthn .NET libraries Day 1, consider frontend-first approach | Backend Lead |
| R-007 | Polygon Amoy RPC downtime | Low | Medium | Use multiple RPC endpoints, implement retry logic | Backend Lead |
| R-008 | CI/CD pipeline configuration complex | Medium | Medium | Start with simple workflow, iterate | QA Lead |
| R-009 | Playwright WebAuthn simulation issues | Medium | Medium | Research Playwright docs, engage community | QA Lead |
| R-010 | Frontend-Backend integration delays | High | Medium | Define API contracts early, use mock data | Team Lead |

### Low Risks (Priority 3)

| Risk ID | Risk | Probability | Impact | Mitigation Strategy | Owner |
|---------|------|-------------|--------|---------------------|-------|
| R-011 | Database migration issues | Low | Low | Document migration steps, provide docker-compose setup | Backend Lead |
| R-012 | Team member unavailable | Low | Medium | Cross-train on critical tasks, pair programming | Team Lead |
| R-013 | Test reporting dashboard setup issues | Low | Low | Use console output temporarily | QA Lead |
| R-014 | K6 performance baseline unclear | Low | Low | Start with conservative targets, adjust | QA Lead |

### Contingency Plans

**If Circle Console Access Delayed >2 Days**:
1. Backend focuses on infrastructure tasks (BE-001 to BE-009)
2. Frontend implements WebAuthn independently with test credentials
3. QA prepares mock-based testing
4. Re-prioritize wallet creation and transfer to Sprint 2
5. Extend Sprint 1 by 2-3 days if critical

**If Frontend Plan Not Ready by Day 2**:
1. Backend proceeds independently with Swagger documentation
2. QA focuses on backend API testing only
3. Schedule emergency meeting: Frontend Lead + Team Lead + Product Owner
4. Consider allocating Backend engineer to assist Frontend setup
5. Defer frontend-dependent integration points to Sprint 2

**If Backend Behind Schedule by Day 5**:
1. Identify bottleneck tasks
2. Allocate additional engineer to critical path
3. Defer stretch goals (BE-110, BE-111, BE-108) to Sprint 2
4. Increase pair programming on complex tasks (BE-113, BE-114)
5. Consider reducing scope: focus on wallet creation only, defer transfers

---

## Success Criteria

### Overall Sprint Success

Sprint 1 is considered **SUCCESSFUL** when:

1. **Infrastructure Operational**:
   - [ ] API runs in local and dev environments
   - [ ] PostgreSQL database with migrations executable
   - [ ] Hashicorp Vault stores and retrieves secrets
   - [ ] Health checks return status of all dependencies
   - [ ] Structured logging with correlation IDs operational
   - [ ] YARP gateway routes requests correctly

2. **Authentication Functional**:
   - [ ] Users can register with passkeys (POST /api/auth/register)
   - [ ] Users can login with passkeys (POST /api/auth/verify)
   - [ ] JWT tokens generated and validated correctly
   - [ ] Session management functional

3. **Wallet Creation Operational**:
   - [ ] POST /api/wallet/create returns deterministic address
   - [ ] Circle Smart Account created successfully
   - [ ] Wallet record stored in database

4. **Transaction Capability** (Stretch Goal):
   - [ ] POST /api/transactions/transfer submits gasless USDC transfer
   - [ ] Transaction demonstrates 100% gas sponsorship
   - [ ] Transaction confirmed on Polygon Amoy testnet
   - [ ] Transaction status queryable

5. **QA Infrastructure Ready**:
   - [ ] Test environments configured and accessible
   - [ ] 10+ test wallets funded with testnet USDC
   - [ ] CI/CD pipeline runs tests on PR
   - [ ] Test frameworks installed (Playwright, Cypress, K6)
   - [ ] Test strategy and quality gates documented

6. **Documentation Complete**:
   - [ ] All API endpoints documented in Swagger
   - [ ] API integration guide for Frontend
   - [ ] Test strategy and test plans documented
   - [ ] Environment setup guide for team
   - [ ] Architecture decision records for key decisions

7. **Quality Gates Met**:
   - [ ] Unit test coverage >60% for backend services
   - [ ] Integration tests pass for Circle SDK calls
   - [ ] No Critical or High severity bugs
   - [ ] Code review completed for all critical components
   - [ ] Security review passed for authentication flow

### Minimum Viable Sprint (If Behind Schedule)

**Absolute Minimum Deliverables** (cannot ship Sprint 1 without these):
1. Backend API runs with database connectivity
2. Hashicorp Vault integration functional
3. Health checks operational
4. Passkey registration endpoint (POST /api/auth/register)
5. Wallet creation endpoint (POST /api/wallet/create)
6. Swagger documentation for all implemented endpoints
7. QA test strategy documented
8. Test environments configured

**Nice-to-Have** (can defer to Sprint 2):
- Passkey login endpoint (use registration only for MVP)
- Transfer endpoint (defer to Sprint 2)
- Transaction status endpoint
- Balance query endpoint
- Full CI/CD pipeline (can run tests locally)
- Complete test framework setup (prioritize one framework)

---

## Sprint Review Demo Agenda

**Date**: Friday, January 17, 2025
**Time**: 2:00 PM - 4:00 PM
**Location**: Conference Room / Zoom
**Attendees**: All team members, Product Owner, Stakeholders

### Demo Sequence

**1. Introduction** (10 min) - Team Lead
- Sprint 1 goal recap
- Team composition and capacity
- High-level achievements
- Metrics overview

**2. Backend Demonstration** (30 min) - Backend Lead

**Infrastructure**:
- Show project structure (Solution Explorer: API, Application, Domain, Infrastructure)
- Demonstrate clean architecture pattern
- Show Swagger UI at /swagger

**Observability**:
- Health check endpoints:
  - GET /health - Liveness
  - GET /health/ready - Readiness with dependency status
- Structured logging demonstration:
  - Show correlation IDs in logs
  - Demonstrate request tracing

**Secrets Management**:
- Show Vault integration (retrieve Circle API keys)
- Demonstrate configuration service loading secrets on startup

**Authentication Flow**:
- POST /api/auth/register - Register new user with passkey
  - Show request/response in Swagger
  - Verify user record in PostgreSQL database
- POST /api/auth/verify - Login with passkey
  - Show JWT token in response
  - Decode JWT to show claims (sub, username, exp)

**Wallet Creation**:
- POST /api/wallet/create - Create Circle Smart Account
  - Show deterministic address generation
  - Verify wallet record in database
  - Show wallet on Polygon Amoy block explorer

**USDC Balance** (if completed):
- GET /api/wallet/{address}/balance - Query USDC balance
  - Show balance from Polygon RPC
  - Demonstrate caching (30-second TTL)

**Gasless Transfer** (HIGHLIGHT - if completed):
- POST /api/transactions/transfer - Submit gasless USDC transfer
  - Show request: toAddress, amount
  - Show response: userOpHash, status
  - Verify transaction on Polygon Amoy block explorer
  - **Prove 100% gas sponsorship** (user paid 0 gas)
  - Show transaction record in database

**Transaction Status** (if completed):
- GET /api/transactions/{id}/status - Query transaction status
  - Show status: pending → confirmed
  - Show transaction hash and confirmations

**API Documentation**:
- Walkthrough Swagger documentation
- Show request/response examples
- Show error response format
- Show authentication flow

**3. Frontend Demonstration** (20 min) - Frontend Lead

**STATUS**: **CANNOT DEMO - PLAN MISSING**

**Expected Demo** (if plan existed):
- React application structure
- WebAuthn passkey integration (client-side)
- Register/Login UI flow
- Wallet creation UI
- Transfer UI
- Error handling and loading states

**4. QA Demonstration** (20 min) - QA Lead

**Test Strategy**:
- Show test strategy document
- Explain test pyramid (unit 70%, integration 20%, E2E 10%)
- Explain automation vs manual split (80/20)
- Show risk-based testing approach

**Test Infrastructure**:
- Demonstrate CI/CD pipeline (GitHub Actions)
  - Show workflow file
  - Show test execution on PR
  - Show test results artifact
- Show test frameworks installed:
  - Playwright (with WebAuthn virtual authenticator)
  - Cypress (with custom commands)
  - Grafana K6 (with baseline scripts)

**Test Environment**:
- Show test environment access (Circle, Polygon Amoy, Backend API)
- Show test data inventory:
  - 10+ test wallets with USDC
  - 20+ test user accounts
  - Test bank account data
- Show mock services (WhiteBit API, Fiat Gateway API)

**Test Reporting**:
- Show Allure dashboard (or test report format)
- Show test results with pass/fail rates
- Show screenshots on failure

**Quality Gates**:
- Show quality gates document
- Show Definition of Done for Backend/Frontend/QA
- Show bug severity/priority matrix
- Show bug report template

**Test Cases**:
- Show test case repository structure
- Show Phase 1 test cases (passkey, wallet, transfer)
- Show test tagging (smoke, regression, critical)

**5. Metrics & Lessons Learned** (10 min) - Team Lead

**Sprint Metrics**:
- Planned vs actual effort
- Task completion rate
- Velocity
- Team utilization

**Challenges Encountered**:
- Frontend plan missing (impact and resolution)
- Circle Console access delays (if any)
- Technical challenges (e.g., ERC-4337 UserOperation construction)

**Lessons Learned**:
- What went well
- What could be improved
- Action items for Sprint 2

**6. Q&A and Feedback** (20 min) - All

- Stakeholder questions
- Feedback collection
- Concerns or risks
- Approval to proceed to Sprint 2

**7. Sprint 2 Preview** (10 min) - Team Lead

- Sprint 2 focus areas (Phase 1 completion)
- Expected deliverables
- Estimated effort
- Sprint 2 planning timeline

---

## Next Sprint Preview (Sprint 2)

### Sprint 2 Focus Areas

**Backend Team**:
- **Complete Phase 1**: Core Wallet Foundation
  - Transaction monitoring service (background worker)
  - Transaction history endpoint (GET /api/transactions)
  - Wallet balance refresh (with caching)
  - Complete BE-110, BE-111 if deferred from Sprint 1
- **Start Phase 2**: Transaction History & Monitoring
  - Transaction list with pagination
  - Transaction detail endpoint
  - Webhook for transaction status updates
  - Blockchain event listener

**Frontend Team**:
- **Complete Phase 1**: Basic Wallet UI
  - Wallet dashboard
  - Transaction history display
  - Transfer form with validation
  - Transaction status tracking
  - Error handling and loading states
- **Start Phase 2**: Enhanced UX
  - Transaction filters and search
  - QR code generation
  - Copy address to clipboard
  - Responsive design refinements

**QA Team**:
- **Phase 1 Testing**:
  - Passkey authentication testing (QA-101 to QA-106)
  - Wallet creation testing (QA-107 to QA-109)
  - Gasless transfer testing (QA-110 to QA-114)
  - Transaction monitoring testing (QA-115 to QA-118)
- **Phase 1 Regression Testing**:
  - Automated E2E tests for critical paths
  - Performance testing (load tests with K6)
  - Security testing (OWASP Top 10)
  - Accessibility testing (WCAG 2.1 AA)

### Estimated Effort for Sprint 2

| Team | Estimated Effort | Key Deliverables |
|------|------------------|------------------|
| Backend | ~18 days | Phase 1 completion, Phase 2 start |
| Frontend | ~20 days | Phase 1 completion, Phase 2 start |
| QA | ~21 days | Phase 1 testing, regression suite |
| **Total** | **~59 days** | **Phase 1 fully tested and operational** |

### Sprint 2 Planning Timeline

- **Sprint 2 Planning Meeting**: Monday, January 20, 2025 (9:00 AM - 11:00 AM)
- **Sprint 2 Dates**: January 20 - January 31, 2025 (2 weeks)
- **Sprint 2 Review**: Friday, January 31, 2025

---

## Recommendations from Team Lead

### 1. Frontend Plan - URGENT

**Recommendation**: Frontend Lead must create Sprint 1 plan **immediately** (Day 1 morning).

**Action Items**:
- [ ] Emergency meeting: Frontend Lead + Team Lead (Day 1, 9:00 AM)
- [ ] Frontend Lead creates plan by Day 1 end of day
- [ ] Team review of Frontend plan (Day 2 morning)
- [ ] Publish Frontend plan to repository

**Rationale**: Cannot coordinate cross-team work without knowing Frontend capacity and tasks.

---

### 2. Circle Access - CRITICAL

**Recommendation**: Obtain Circle Console access and API keys **Day 1**.

**Action Items**:
- [ ] Backend Lead requests Circle Console access (Day 1, 9:00 AM)
- [ ] Backend Lead contacts Circle support for API keys (Day 1, 9:30 AM)
- [ ] Escalate to Product Owner if no response within 24 hours
- [ ] Prepare fallback plan (defer SDK integration to Sprint 2)

**Rationale**: Circle SDK integration is on critical path. Delay >2 days will jeopardize sprint goal.

---

### 3. Capacity Management - Backend

**Recommendation**: Proactively manage Backend team capacity (97% utilization).

**Action Items**:
- [ ] Mark BE-110, BE-111, BE-108 as "Stretch Goals" (can defer to Sprint 2)
- [ ] Monitor daily progress on critical path tasks
- [ ] Increase pair programming on complex tasks (BE-113, BE-114)
- [ ] Consider adding temporary resource if available

**Rationale**: 97% utilization leaves minimal buffer for unknowns. Stretch goals provide flexibility.

---

### 4. Integration Points - Early Coordination

**Recommendation**: Schedule integration meetings early, even before all dependencies ready.

**Action Items**:
- [ ] Day 3 API contract review (Backend + Frontend) - **MANDATORY**
- [ ] Day 5 mid-sprint checkpoint (All teams) - **MANDATORY**
- [ ] Day 7 test environment handoff (Backend + QA) - **MANDATORY**
- [ ] Daily standups at 9:00 AM (15 minutes) - **MANDATORY**

**Rationale**: Early coordination prevents misalignment and rework.

---

### 5. Risk Escalation Protocol

**Recommendation**: Establish clear escalation path for blockers.

**Escalation Levels**:
1. **Team Level** (0-4 hours): Engineer → Team Lead
2. **Cross-Team Level** (4-8 hours): Team Lead → Team Lead coordination
3. **Management Level** (8-24 hours): Team Lead → Product Owner
4. **Executive Level** (>24 hours): Product Owner → VP Engineering

**Blockers Requiring Immediate Escalation**:
- Circle Console access not granted within 24 hours
- Frontend plan not ready by Day 2
- Critical external dependency unavailable
- Team member unexpected absence

**Rationale**: Fast escalation prevents small issues from becoming sprint-level risks.

---

### 6. Definition of Done - Enforce Rigorously

**Recommendation**: Enforce quality gates and DoD to prevent technical debt.

**Action Items**:
- [ ] Code review required for all tasks (no exceptions)
- [ ] Unit tests required before merging (Backend: 80% coverage, Frontend: component tests)
- [ ] API documentation updated in Swagger before handoff to Frontend
- [ ] Test-ready criteria validated before QA handoff
- [ ] No Critical or High bugs in "Done" tasks

**Rationale**: Quality gates prevent shortcuts that create debt in later sprints.

---

### 7. Timeline Adjustments

**Recommendation**: Monitor sprint health at Day 5 checkpoint, adjust if needed.

**Adjustment Options** (if behind schedule):
1. **Defer Stretch Goals**: Move BE-110, BE-111, BE-108 to Sprint 2
2. **Extend Sprint**: Add 2-3 days to Sprint 1 (requires stakeholder approval)
3. **Reduce Scope**: Focus on wallet creation only, defer transfers to Sprint 2
4. **Add Resources**: Temporary contractor for non-critical tasks

**Decision Criteria**:
- If <60% tasks complete by Day 5 → Defer stretch goals
- If <40% tasks complete by Day 5 → Consider scope reduction
- If critical blocker unresolved by Day 5 → Consider sprint extension

**Rationale**: Proactive adjustment prevents sprint failure.

---

### 8. Communication & Transparency

**Recommendation**: Daily updates and transparent communication.

**Communication Cadence**:
- **Daily Standup** (9:00 AM, 15 min): What did I do? What will I do? Blockers?
- **Daily Status Update** (5:00 PM): Update task status in Jira/GitHub, flag risks
- **Weekly Team Sync** (Friday, 4:00 PM): Week review, next week preview
- **Slack Channel**: #coinpay-sprint-1 for async updates and quick questions

**Rationale**: Transparency allows early intervention on issues.

---

## Appendix A: Team Roster

### Backend Team

| Name | Role | Seniority | Capacity (days) | Primary Focus |
|------|------|-----------|-----------------|---------------|
| TBD | Backend Lead | Senior | 10 days | Circle SDK, Architecture |
| TBD | Backend Engineer 1 | Mid-level | 10 days | API endpoints, Repositories |
| TBD | Backend Engineer 2 (optional) | Mid-level | 10 days | Infrastructure, Testing |

**Total Backend Capacity**: 20-30 days

### Frontend Team

**STATUS**: **TEAM COMPOSITION UNKNOWN - PLAN MISSING**

| Name | Role | Seniority | Capacity (days) | Primary Focus |
|------|------|-----------|-----------------|---------------|
| TBD | Frontend Lead | TBD | TBD days | TBD |
| TBD | Frontend Engineer 1 | TBD | TBD days | TBD |

### QA Team

| Name | Role | Seniority | Capacity (days) | Primary Focus |
|------|------|-----------|-----------------|---------------|
| TBD | QA Lead | Senior | 10 days | Strategy, K6, Training |
| TBD | QA Engineer 1 | Mid-level | 10 days | Playwright, Test Data |
| TBD | QA Engineer 2 | Mid-level | 10 days | Cypress, CI/CD |

**Total QA Capacity**: 20-30 days

---

## Appendix B: Key Contacts

| Role | Name | Email | Slack | Escalation |
|------|------|-------|-------|------------|
| Team Lead | TBD | TBD | @team-lead | - |
| Backend Lead | TBD | TBD | @backend-lead | Team Lead |
| Frontend Lead | TBD | TBD | @frontend-lead | Team Lead |
| QA Lead | TBD | TBD | @qa-lead | Team Lead |
| Product Owner | TBD | TBD | @product-owner | VP Product |
| DevOps Contact | TBD | TBD | @devops | Team Lead |
| Circle Support | - | support@circle.com | - | Backend Lead |

---

## Appendix C: Environment URLs

| Environment | Purpose | URL | Credentials |
|-------------|---------|-----|-------------|
| Backend API (Dev) | API development | http://localhost:5000 | Local dev |
| Backend API (Test) | QA testing | https://dev-api.coinpay.local | In Vault |
| Frontend (Dev) | UI development | http://localhost:3000 or :5173 | Local dev |
| Frontend (Test) | QA testing | https://dev.coinpay.local | - |
| YARP Gateway | API gateway | http://localhost:5100 | Local dev |
| PostgreSQL (Dev) | Database | localhost:5432 | postgres/dev_password |
| Hashicorp Vault (Dev) | Secrets | http://localhost:8200 | dev_token |
| Swagger UI | API docs | http://localhost:5000/swagger | - |
| Polygon Amoy RPC | Blockchain | https://rpc-amoy.polygon.technology | Public |
| Circle Console | SDK config | https://console.circle.com | Pending access |
| Allure Dashboard | Test reports | TBD | TBD |
| GitHub Actions | CI/CD | https://github.com/[org]/coinpay/actions | GitHub OAuth |

---

## Appendix D: Success Metrics Dashboard

### Sprint 1 Metrics (To Be Tracked Daily)

| Metric | Target | Actual | Status |
|--------|--------|--------|--------|
| **Capacity** | | | |
| Backend planned effort | ~29 days | ___ | - |
| Frontend planned effort | TBD | ___ | - |
| QA planned effort | ~22 days | ___ | - |
| **Completion** | | | |
| Backend tasks completed | 24/24 (100%) | ___/24 | - |
| Frontend tasks completed | TBD | ___/TBD | - |
| QA tasks completed | 16/16 (100%) | ___/16 | - |
| **Quality** | | | |
| Unit test coverage (Backend) | >60% | ___% | - |
| Critical bugs | 0 | ___ | - |
| High bugs | 0 | ___ | - |
| Code review completion | 100% | ___% | - |
| **Documentation** | | | |
| API endpoints documented | 100% | ___% | - |
| Test strategy complete | Yes | ___ | - |
| Environment setup guide | Yes | ___ | - |
| **Integration** | | | |
| API contracts defined | Yes | ___ | - |
| Test environments configured | 5/5 | ___/5 | - |
| CI/CD pipeline operational | Yes | ___ | - |
| **Functional** | | | |
| Passkey registration works | Yes | ___ | - |
| Passkey login works | Yes | ___ | - |
| Wallet creation works | Yes | ___ | - |
| Gasless transfer works | Yes | ___ | - |

---

## Appendix E: Quick Reference Links

### Documentation
- Project Plan: `D:\Projects\Test\Claude\CoinPay\Planning\CoinPay-Project.md`
- Backend Sprint 1 Plan: `D:\Projects\Test\Claude\CoinPay\Planning\Sprint-1-Backend-Plan.md`
- Frontend Sprint 1 Plan: **MISSING**
- QA Sprint 1 Plan: `D:\Projects\Test\Claude\CoinPay\Planning\sprint-1-qa-plan.md`
- Master Plan: `D:\Projects\Test\Claude\CoinPay\Planning\Sprint-1-Master-Plan.md`

### External Resources
- Circle Modular Wallets Docs: https://developers.circle.com/w3s/docs/modular-wallets-overview
- ERC-4337 Account Abstraction: https://eips.ethereum.org/EIPS/eip-4337
- Polygon Amoy Testnet: https://polygon.technology/blog/introducing-the-amoy-testnet-for-polygon-pos
- WebAuthn .NET: https://github.com/passwordless-lib/fido2-net-lib
- YARP Documentation: https://microsoft.github.io/reverse-proxy/

### Tools
- GitHub Repository: TBD
- Jira Board: TBD
- Slack Channel: #coinpay-sprint-1
- Circle Console: https://console.circle.com
- Polygon Amoy Block Explorer: https://amoy.polygonscan.com/

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-26 | Team Lead | Initial Sprint 1 Master Plan created |
| - | - | - | Frontend plan to be integrated when available |

---

**SPRINT 1 STATUS**: **READY TO START** (pending Frontend plan)

**NEXT STEPS**:
1. Frontend Lead creates Sprint 1 Frontend Plan (Day 1 morning) - **URGENT**
2. Team Lead reviews all three plans and consolidates (Day 1 afternoon)
3. Kickoff meeting (Day 1, 2:00 PM) - All teams
4. Begin execution (Day 1, 3:00 PM onwards)
5. Daily standups at 9:00 AM starting Day 2

---

**End of Sprint 1 Master Plan**
