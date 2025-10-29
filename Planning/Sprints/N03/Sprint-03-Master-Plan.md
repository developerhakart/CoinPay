# CoinPay Wallet MVP - Sprint N03 Master Plan

**Version**: 1.0
**Sprint Duration**: 2 weeks (10 working days)
**Sprint Dates**: February 3 - February 14, 2025
**Document Status**: Ready for Execution
**Last Updated**: 2025-10-29
**Owner**: Team Lead

---

## Executive Summary

### Sprint Overview

Sprint N03 is a **Critical Feature Sprint** focused on implementing Phase 3 (Fiat Off-Ramp) to enable crypto-to-fiat conversion and bank account payouts. This sprint delivers the core value proposition: seamless conversion of cryptocurrency to traditional fiat currency with direct bank transfers.

**Sprint Goal**: Enable users to convert USDC to USD and withdraw funds directly to their bank accounts, completing the crypto-to-fiat user journey.

### Sprint N02 Achievement Summary

✅ **97% Complete** (32/33 tasks completed)
- Phase 1: Core Wallet Foundation (100% complete)
- Phase 2: Transaction History & Monitoring (95% complete)
- Phase 2: UI Polish & Enhancements (100% complete)
- QA Testing: All critical and optional tests (100% complete)
- Outstanding: BE-208 (Blockchain Event Listener) - deferred to future sprint

### Total Team Capacity

| Team | Engineers | Available Days | Planned Effort | Utilization |
|------|-----------|---------------|----------------|-------------|
| Backend | 2-3 | 20-30 days | ~28 days | 93% |
| Frontend | 2-3 | 20-30 days | ~22 days | 73% |
| QA | 2-3 | 20-30 days | ~20 days | 67% |
| **Total** | **6-9** | **60-90 days** | **~70 days** | **78%** |

### Sprint Dates

- **Start Date**: Monday, February 3, 2025
- **End Date**: Friday, February 14, 2025
- **Working Days**: 10 days
- **Mid-Sprint Checkpoint**: Friday, February 7, 2025 (Day 5)
- **Sprint Review**: Friday, February 14, 2025 (Day 10)

### Critical Success Factors

1. **Backend**: Bank account management with secure encrypted storage
2. **Backend**: Fiat gateway integration with real-time exchange rates
3. **Backend**: Payout APIs with status tracking and webhooks
4. **Frontend**: Bank account management UI with validation
5. **Frontend**: Fiat withdrawal flow with conversion calculator
6. **QA**: Phase 3 test coverage (unit, integration, E2E)
7. **Cross-Team**: No blockers preventing Phase 4 (Exchange Investment) in Sprint N04

---

## Team Goals

### Backend Team Sprint Goal

**Primary Goal**: Implement secure bank account management and fiat gateway integration for crypto-to-fiat payouts.

**Key Deliverables**:
- Bank account CRUD operations with encryption
- Fiat gateway integration (RedotPay/Bridge/Wyre)
- Real-time USDC/USD exchange rate API
- Payout initiation and tracking endpoints
- Webhook handling for payout status updates
- Conversion fee calculation engine
- Payout transaction history APIs
- KYC/AML basic compliance (user verification)

**Success Metrics**:
- All bank account operations secured with encryption
- Fiat gateway successfully processes test payouts
- Exchange rates updated every 30 seconds
- Payout status tracked in real-time
- API response time < 2s for payout operations
- 100% of payouts have audit trail
- All integration tests pass

### Frontend Team Sprint Goal

**Primary Goal**: Implement intuitive bank account management and fiat withdrawal user experience.

**Key Deliverables**:
- Bank account add/edit/delete interface
- Bank account validation (routing/account numbers)
- Fiat withdrawal flow with step-by-step wizard
- USDC to USD conversion calculator
- Exchange rate display with refresh
- Fee transparency (conversion + payout fees)
- Payout confirmation screen
- Payout status tracking page
- Payout history with filtering
- Success/failure notifications

**Success Metrics**:
- Bank account form validates correctly (US format)
- Conversion calculator updates in real-time
- Withdrawal flow completes in < 5 clicks
- Mobile responsiveness verified on 3+ devices
- Zero accessibility violations (WCAG AA)
- Component tests cover critical flows
- No console errors in production build

### QA Team Sprint Goal

**Primary Goal**: Achieve comprehensive test coverage for Phase 3 fiat off-ramp functionality.

**Key Deliverables**:
- Phase 3 functional test plan (bank accounts, payouts)
- Security testing (encryption, data protection)
- Integration testing with fiat gateway (sandbox)
- Automated E2E tests for withdrawal flow
- Negative testing (invalid bank accounts, insufficient balance)
- Performance testing (payout API load)
- Compliance testing (KYC/AML workflows)
- Regression testing (Phases 1-2 features)
- Bug triage and resolution support

**Success Metrics**:
- Unit test coverage > 80%
- All critical E2E tests pass
- Security audit: zero Critical/High vulnerabilities
- Performance tests meet thresholds (<2s API)
- Zero Critical bugs, < 3 High bugs at sprint end
- All test documentation updated
- Compliance checks pass

---

## Consolidated Task List

### Phase 3: Fiat Off-Ramp - Bank Account Management (Backend) (8.00 days)

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| BE-301 | Bank Account Model & Repository | Backend | 1.50 | BE-1 | - | 1 |
| BE-302 | Encryption Service for Sensitive Data | Backend | 2.00 | Senior BE | - | 1 |
| BE-303 | POST /api/bank-account - Add Bank Account | Backend | 1.00 | BE-1 | BE-301, BE-302 | 1 |
| BE-304 | GET /api/bank-account - List Bank Accounts | Backend | 0.75 | BE-1 | BE-301 | 1 |
| BE-305 | PUT /api/bank-account/{id} - Update Bank Account | Backend | 0.75 | BE-1 | BE-301 | 1 |
| BE-306 | DELETE /api/bank-account/{id} - Remove Bank Account | Backend | 0.50 | BE-1 | BE-301 | 1 |
| BE-307 | Bank Account Validation Service | Backend | 1.50 | BE-2 | BE-301 | 1 |

**Phase 3 Bank Account Subtotal**: ~8.00 days

### Phase 3: Fiat Off-Ramp - Gateway Integration (Backend) (10.00 days)

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| BE-308 | Fiat Gateway Service Interface | Backend | 1.50 | Senior BE | - | 1 |
| BE-309 | RedotPay/Bridge API Client Integration | Backend | 3.00 | Senior BE | BE-308 | 1-2 |
| BE-310 | Exchange Rate Service (USDC/USD) | Backend | 1.50 | BE-1 | BE-309 | 2 |
| BE-311 | Conversion Fee Calculator Service | Backend | 1.00 | BE-1 | BE-310 | 2 |
| BE-312 | GET /api/rates/usdc-usd - Exchange Rate | Backend | 0.50 | BE-1 | BE-310 | 2 |
| BE-313 | POST /api/payout/initiate - Initiate Payout | Backend | 2.00 | BE-2 | BE-309, BE-311 | 2 |
| BE-314 | Webhook Handler for Payout Status | Backend | 1.50 | BE-2 | BE-313 | 2 |

**Phase 3 Gateway Integration Subtotal**: ~10.00 days

### Phase 3: Fiat Off-Ramp - Payout Management (Backend) (10.00 days)

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| BE-315 | Payout Transaction Model & Repository | Backend | 1.50 | BE-1 | - | 1 |
| BE-316 | Payout Status Update Service | Backend | 1.50 | BE-2 | BE-315, BE-314 | 2 |
| BE-317 | GET /api/payout/history - Payout History | Backend | 2.00 | BE-1 | BE-315 | 2 |
| BE-318 | GET /api/payout/{id}/status - Payout Status | Backend | 1.00 | BE-1 | BE-315 | 2 |
| BE-319 | GET /api/payout/{id}/details - Payout Details | Backend | 1.00 | BE-1 | BE-315 | 2 |
| BE-320 | POST /api/payout/{id}/cancel - Cancel Payout | Backend | 1.00 | BE-1 | BE-315 | 2 |
| BE-321 | Payout Audit Trail & Logging | Backend | 2.00 | Senior BE | BE-315 | 2 |

**Phase 3 Payout Management Subtotal**: ~10.00 days

**Phase 3 Backend Total**: ~28.00 days

### Phase 3: Fiat Off-Ramp - Frontend (22.00 days)

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| FE-301 | Bank Account Form Component | Frontend | 2.00 | FE-1 | BE-303 | 1 |
| FE-302 | Bank Account List Component | Frontend | 1.50 | FE-1 | BE-304 | 1 |
| FE-303 | Bank Account Validation (Client-Side) | Frontend | 1.50 | FE-1 | FE-301 | 1 |
| FE-304 | Bank Account Management Page | Frontend | 2.00 | FE-1 | FE-301, FE-302 | 1 |
| FE-305 | Fiat Withdrawal Wizard (Multi-Step) | Frontend | 3.00 | FE-2 | BE-313 | 1-2 |
| FE-306 | USDC to USD Conversion Calculator | Frontend | 2.00 | FE-1 | BE-310, BE-311 | 2 |
| FE-307 | Exchange Rate Display Component | Frontend | 1.00 | FE-1 | BE-312 | 2 |
| FE-308 | Payout Confirmation Screen | Frontend | 1.50 | FE-1 | FE-305 | 2 |
| FE-309 | Payout Status Tracking Page | Frontend | 2.00 | FE-2 | BE-318 | 2 |
| FE-310 | Payout History Page | Frontend | 2.50 | FE-2 | BE-317 | 2 |
| FE-311 | Payout Detail Modal | Frontend | 1.50 | FE-1 | BE-319 | 2 |
| FE-312 | Fee Transparency UI (Breakdown) | Frontend | 1.50 | FE-1 | BE-311 | 2 |

**Phase 3 Frontend Total**: ~22.00 days

### Phase 3: QA Testing (20.00 days)

| Task ID | Task Name | Team | Effort (days) | Owner | Dependencies | Week |
|---------|-----------|------|---------------|-------|--------------|------|
| QA-301 | Phase 3 Functional Test Plan | QA | 1.00 | QA Lead | - | 1 |
| QA-302 | Bank Account Management Testing | QA | 3.00 | QA-1 | Backend/Frontend | 1 |
| QA-303 | Fiat Gateway Integration Testing | QA | 3.00 | QA-2 | BE-309 | 1-2 |
| QA-304 | Withdrawal Flow E2E Tests (Cypress) | QA | 3.00 | QA-1 | Frontend | 2 |
| QA-305 | Security Testing (Encryption, Data Protection) | QA | 2.50 | QA Lead | BE-302 | 2 |
| QA-306 | Negative Testing (Invalid Data, Edge Cases) | QA | 2.00 | QA-2 | All APIs | 2 |
| QA-307 | Performance Testing (Payout API Load) | QA | 1.50 | QA-1 | K6 | 2 |
| QA-308 | Compliance Testing (KYC/AML) | QA | 1.50 | QA Lead | Backend | 2 |
| QA-309 | Regression Testing (Phases 1-2) | QA | 2.00 | QA-2 | - | 2 |
| QA-310 | Bug Triage & Resolution Support | QA | 0.50 | QA Lead | Ongoing | 1-2 |

**Phase 3 QA Total**: ~20.00 days

### Grand Total: ~70.00 days (Backend: 28, Frontend: 22, QA: 20)

---

## Week-by-Week Plan

### Week 1 (Days 1-5): February 3-7, 2025

#### Backend Team - Week 1

**Days 1-2 (Foundation)**:
- BE-301: Bank account model & repository
- BE-302: Encryption service (started)
- BE-308: Fiat gateway service interface
- **Deliverable**: Bank account infrastructure ready

**Days 3-4 (Bank Account APIs)**:
- BE-302: Encryption service (completed)
- BE-303: Add bank account endpoint
- BE-304: List bank accounts endpoint
- BE-305: Update bank account endpoint
- BE-306: Delete bank account endpoint
- BE-307: Bank account validation (started)
- **Deliverable**: All bank account CRUD operations functional

**Day 5 (Mid-Sprint Checkpoint)**:
- BE-307: Validation service (completed)
- BE-309: Gateway integration (started)
- **Checkpoint Demo**: Bank account management working end-to-end

#### Frontend Team - Week 1

**Days 1-2 (Bank Account Forms)**:
- FE-301: Bank account form component
- FE-303: Client-side validation
- **Deliverable**: Bank account form validates correctly

**Days 3-4 (Bank Account Management)**:
- FE-302: Bank account list component
- FE-304: Bank account management page
- FE-305: Withdrawal wizard (started)
- **Deliverable**: Users can manage bank accounts

**Day 5 (Mid-Sprint Checkpoint)**:
- FE-305: Withdrawal wizard (continued)
- **Checkpoint Demo**: Bank account management UI complete

#### QA Team - Week 1

**Day 1 (Sprint Planning)**:
- Sprint N03 planning meeting
- QA-301: Phase 3 test plan
- Test environment setup

**Days 2-3 (Bank Account Testing)**:
- QA-302: Bank account management tests
- Security validation (encryption)
- Bug logging

**Days 4-5 (Gateway Testing Begins)**:
- QA-302: Bank account tests (continued)
- QA-303: Gateway integration tests (started)
- **Checkpoint Demo**: Test results and coverage report

### Week 2 (Days 6-10): February 10-14, 2025

#### Backend Team - Week 2

**Days 6-7 (Gateway Integration)**:
- BE-309: Gateway integration (completed)
- BE-310: Exchange rate service
- BE-311: Fee calculator service
- BE-312: Exchange rate endpoint
- **Deliverable**: Gateway integration complete with live rates

**Days 8-9 (Payout APIs)**:
- BE-313: Initiate payout endpoint
- BE-314: Webhook handler
- BE-315: Payout model & repository
- BE-316: Payout status update service
- BE-317: Payout history endpoint
- **Deliverable**: Payout flow functional end-to-end

**Day 10 (Sprint Completion)**:
- BE-318: Payout status endpoint
- BE-319: Payout details endpoint
- BE-320: Cancel payout endpoint
- BE-321: Audit trail implementation
- Code reviews and documentation
- **Sprint Review**: Demo complete fiat off-ramp flow

#### Frontend Team - Week 2

**Days 6-7 (Withdrawal Flow)**:
- FE-305: Withdrawal wizard (completed)
- FE-306: Conversion calculator
- FE-307: Exchange rate display
- FE-308: Payout confirmation screen
- **Deliverable**: Complete withdrawal flow

**Days 8-9 (Payout Tracking)**:
- FE-309: Payout status tracking page
- FE-310: Payout history page
- FE-311: Payout detail modal
- FE-312: Fee transparency UI
- **Deliverable**: Full payout tracking experience

**Day 10 (Sprint Completion)**:
- UI polish and responsive fixes
- Performance optimization
- Accessibility audit
- **Sprint Review**: Demo complete user journey

#### QA Team - Week 2

**Days 6-7 (Integration & E2E)**:
- QA-303: Gateway testing (completed)
- QA-304: E2E withdrawal tests
- QA-305: Security testing

**Days 8-9 (Comprehensive Testing)**:
- QA-306: Negative testing
- QA-307: Performance testing
- QA-308: Compliance testing
- QA-309: Regression testing

**Day 10 (Sprint Completion)**:
- Final test execution
- Bug verification and closure
- Test report generation
- **Sprint Review**: Present test results and metrics

---

## Technical Dependencies

### External Services

1. **Fiat Gateway** (RedotPay/Bridge/Wyre)
   - Sandbox account required
   - API credentials (API key, secret)
   - Webhook URL configuration
   - Test bank accounts for validation

2. **Encryption Service**
   - AWS KMS or Azure Key Vault (recommended)
   - Or local encryption with secure key management
   - FIPS 140-2 compliant (recommended)

3. **KYC/AML Provider** (Optional for MVP)
   - Onfido, Jumio, or Persona
   - Basic identity verification
   - Document upload and validation

### Internal Dependencies

1. **Database**
   - New tables: bank_accounts, payout_transactions
   - Encrypted columns for sensitive data
   - Audit logging tables

2. **Circle SDK**
   - USDC transfer capabilities (already implemented)
   - Balance checking before payouts

3. **Redis Cache**
   - Exchange rate caching (30-second TTL)
   - Session management

---

## Risk Assessment

### High Risk Items

| Risk | Impact | Mitigation | Owner |
|------|--------|------------|-------|
| Fiat gateway API downtime | High | Use fallback provider, implement retry logic | Backend Lead |
| Bank account validation complexity (US only) | Medium | Focus on ACH format only (routing + account) | Backend |
| Exchange rate volatility | Medium | Lock rates for 30 seconds during checkout | Backend |
| Encryption key management | High | Use managed service (AWS KMS), backup keys | DevOps + Backend |
| PCI DSS compliance for bank data | High | Encrypt at rest + in transit, audit logging | Security + Backend |

### Medium Risk Items

| Risk | Impact | Mitigation | Owner |
|------|--------|------------|-------|
| Gateway sandbox limitations | Medium | Test with multiple test accounts | QA |
| Payout processing delays | Medium | Set clear expectations (24-48h) | Product + Frontend |
| Mobile responsiveness for forms | Low | Test early, iterate on design | Frontend |

---

## Definition of Done

### Backend DoD

- [ ] All API endpoints implemented and documented
- [ ] Bank account data encrypted at rest
- [ ] Fiat gateway integration tested in sandbox
- [ ] Exchange rates update every 30 seconds
- [ ] All payout statuses tracked and logged
- [ ] Webhook handler processes updates correctly
- [ ] Unit tests: > 80% coverage
- [ ] Integration tests: All critical paths pass
- [ ] API response time: < 2s
- [ ] Code reviewed and approved
- [ ] Swagger documentation updated
- [ ] No security vulnerabilities (Critical/High)

### Frontend DoD

- [ ] Bank account management UI complete
- [ ] Withdrawal wizard functional (5-step max)
- [ ] Conversion calculator accurate and real-time
- [ ] Fee breakdown clearly displayed
- [ ] Payout tracking updates in real-time
- [ ] Mobile responsive (tested on 3 devices)
- [ ] Accessibility score > 90 (Lighthouse)
- [ ] Zero console errors
- [ ] Component tests: critical flows covered
- [ ] Code reviewed and approved
- [ ] User acceptance testing passed

### QA DoD

- [ ] Test plan reviewed and approved
- [ ] All functional tests executed
- [ ] E2E tests automated (Cypress)
- [ ] Security testing: zero Critical bugs
- [ ] Performance tests meet thresholds
- [ ] Regression tests pass (Phases 1-2)
- [ ] Bug triage completed
- [ ] Test report published
- [ ] All Critical/High bugs resolved
- [ ] Sprint N03 Test Report completed

---

## Sprint Ceremonies

### Daily Standup
- **Time**: 9:00 AM daily
- **Duration**: 15 minutes
- **Format**: What did you do? What will you do? Any blockers?

### Mid-Sprint Checkpoint (Day 5)
- **Date**: Friday, February 7, 2025
- **Duration**: 1 hour
- **Agenda**: Demo bank account management, review progress

### Sprint Review (Day 10)
- **Date**: Friday, February 14, 2025
- **Duration**: 1.5 hours
- **Agenda**: Demo complete fiat off-ramp flow, gather feedback

### Sprint Retrospective (Day 10)
- **Date**: Friday, February 14, 2025
- **Duration**: 1 hour
- **Agenda**: What went well? What can improve? Action items

---

## Success Metrics

### Product Metrics
- Users can add bank account (100%)
- USDC converts to USD with fees < 3%
- Payouts initiated successfully (sandbox)
- Payout status tracked in real-time

### Engineering Metrics
- Code coverage: > 80%
- API response time: < 2s
- Zero production incidents
- All tests passing

### Quality Metrics
- Zero Critical bugs
- < 3 High priority bugs
- Accessibility score > 90
- Security audit passed

---

## Appendix

### Glossary

- **ACH**: Automated Clearing House (US bank transfer network)
- **AML**: Anti-Money Laundering
- **KYC**: Know Your Customer
- **Payout**: Transfer of fiat currency to bank account
- **RedotPay**: Fiat gateway service provider (example)
- **USDC**: USD Coin (stablecoin)

### References

- Sprint N01 Master Plan: `Planning/Sprints/N01/Sprint-1-Master-Plan.md`
- Sprint N02 Master Plan: `Planning/Sprints/N02/Sprint-02-Master-Plan.md`
- Wallet MVP PRD: `Planning/wallet-mvp.md`
- Wallet V2 PRD: `Planning/wallet-v2.md`

---

**Document Version History**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-29 | Team Lead | Initial Sprint N03 Master Plan |

**Approval**

- [ ] Product Owner: ___________________ Date: __________
- [ ] Tech Lead: _______________________ Date: __________
- [ ] Backend Lead: ____________________ Date: __________
- [ ] Frontend Lead: ___________________ Date: __________
- [ ] QA Lead: _________________________ Date: __________
