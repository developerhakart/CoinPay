# CoinPay Project Backlog

## Priority Levels
- **Critical** - Blocking issues, security vulnerabilities
- **High** - Important features, major bugs
- **Medium** - Nice-to-have features, minor bugs
- **Low** - Future enhancements, optimizations

---

## Low Priority Items

### Configure Circle Webhooks for Automatic Transaction Status Updates
**Type**: Enhancement
**Priority**: Low
**Estimated Effort**: 30 minutes
**Created**: November 4, 2025

**Description**:
Currently, Circle API transactions are created successfully but require manual status updates. Webhooks have been implemented in the codebase but need to be configured in the Circle Console to enable automatic transaction status updates.

**Technical Details**:
- ✅ Webhook endpoint implemented: `POST /api/webhooks/circle`
- ✅ Webhook handler service ready (`CircleWebhookHandler.cs`)
- ✅ Webhook models created (`CircleWebhookModels.cs`)
- ❌ Webhook NOT configured in Circle Console (manual setup required)

**Setup Instructions**:
1. Expose API publicly (using ngrok or production domain)
2. Go to Circle Console → Developer Settings → Webhooks
3. Add Subscription:
   - URL: `https://your-domain.com/api/webhooks/circle`
   - Event: `transactions.updated`
4. Test with a POL transfer

**Current Workaround**:
Transaction statuses are manually updated based on Circle dashboard verification.

**Benefits When Implemented**:
- Real-time automatic transaction status updates (30 seconds)
- No manual intervention required
- Better user experience
- Eliminates need for polling background service

**Documentation**:
See `CIRCLE-WEBHOOK-SETUP.md` for detailed setup instructions.

**Related Files**:
- `CoinPay.Api/Services/Circle/CircleWebhookHandler.cs`
- `CoinPay.Api/Services/Circle/Models/CircleWebhookModels.cs`
- `CoinPay.Api/Program.cs` (webhook endpoint)
- `CIRCLE-WEBHOOK-SETUP.md`

**Dependencies**:
- None (code is ready, only configuration needed)

**Acceptance Criteria**:
- [ ] Webhook configured in Circle Console
- [ ] Test transaction automatically updates from Pending to Completed
- [ ] Webhook logs appear in API logs
- [ ] No manual status updates required

---

## Medium Priority Items

*(Add future medium priority items here)*

---

## High Priority Items

*(Add future high priority items here)*

---

## Critical Items

*(Add future critical items here)*

---

**Last Updated**: November 4, 2025
