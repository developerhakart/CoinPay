# estimation-mvp

## 📘 Purpose

This document defines the **MVP estimation framework and collaboration flow**.  
It establishes a structured process for:
- Reviewing the scope of the MVP,
- Breaking it down into **epics, stories, and tasks**, and
- Producing **3-point estimations** across backend, frontend, and QA streams.

The goal is to produce a detailed estimation file (`estimation-mvp.xlsx`) that reflects the **collective input of all technical and delivery roles** — ensuring transparency, traceability, and data-driven planning.

---

## 👥 Team & Roles

| Role | Responsibility in Estimation |
|------|-------------------------------|
| **Team Lead** | Facilitate breakdown, validate architecture, consolidate backend estimates |
| **Backend Engineer** | Provide effort estimates for APIs, integrations, and database logic |
| **Frontend Engineer** | Estimate React/TypeScript UI and integration work |
| **QA Engineer** | Define test coverage, automation scope, and 3-point QA estimates |
| **AI Agents (Support)** | Generate initial breakdowns, suggest complexity categories, pre-fill estimation templates, summarize risks |

---

## 🧩 Scope of Review
 
All participants review and extract:
- Core functional flows  
- Dependencies between modules  
- Non-functional and integration aspects (security, scalability, performance)  
- Assumptions, constraints, and open questions  

---

## 🗂 Breakdown Structure

The MVP must be decomposed into:

1. **Epics** – major functional or architectural modules  
2. **Stories** – deliverable user-level features within each epic  
3. **Tasks** – implementable engineering items (backend, frontend, QA)  

Each story must include:
- Each work item should have 3 seperate rows for each stream (backend, frontend, QA) and general estimation which is sum of them stream estimations
- Description and acceptance criteria  
- Dependencies and integration notes  
- Links to `*-mvp.md` requirements  
- Assigned owner (BE/FE/QA)  
- 3-point estimation
- each estimation should be estimated per man day (if minimal blockers or task work item is clear then 0.5d etc)

---

## ⏱ 3-Point Estimation Model

Each story and task will have **three estimates per stream (BE, FE, QA)**:

| Type | Description | Example |
|------|--------------|----------|
| **Optimistic (O)** | Best-case with minimal blockers | 0.5d |
| **Most Likely (M)** | Realistic effort considering expected complexities | 1d |
| **Pessimistic (P)** | Worst-case including potential unknowns | 2d |

### QA Considerations
- QA estimates must include:
  - Functional coverage and regression validation.
  - Automation effort (Playwright for UI, Cypress for E2E).
  - Load and stress validation using Grafana K6.
- QA should collaborate with BE/FE to define test data and environment readiness before execution.


## Output structure 

| Epic | Story | Stream | O | M | P | Expected | Notes |
|------|--------|---------|---|---|---|-----------|-------|
| Wallet Core | Create Wallet | Backend | 0.5 | 1 | 2 | 1.08 | API endpoints |
| Wallet Core | Create Wallet | Frontend | 0.5 | 1 | 1.5 | 1.00 | UI form |
| Wallet Core | Create Wallet | QA | 0.5 | 0.75 | 1.5 | 0.88 | Playwright/Cypress tests |
| Wallet Core | Create Wallet | **General (sum)** | **1.5** | **2.75** | **5.0** | **2.96** |  |


**Formula (PERT Estimate):**  
- The **General Estimation** row must aggregate O/M/P values from BE, FE, and QA,  
  and calculate Expected using the same PERT formula `(O + 4M + P) / 6`.
  
  
## Delivery Range Calculation

## 🧮 Delivery Range Calculation

For each epic and the overall MVP:
- **Optimistic Total** = Σ(O)
- **Most Likely Total** = Σ(M)
- **Pessimistic Total** = Σ(P)
- **Expected Total** = Σ(Expected)

These totals provide a **delivery range**:
> MVP Delivery = **Σ(O) to Σ(P)** man-days  
> **Expected Mean** = Σ(Expected) (used for planning)


