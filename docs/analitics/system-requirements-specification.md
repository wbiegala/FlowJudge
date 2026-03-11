[↩️](./../../README.md)

# Software Requirements Specification (SRS)
 - Project name: __FlowJudge__
 - Version: 0.1 (Draft - MVP)
 - Date: 2026-02-18

# 1. Introduction
## 1.1. Purpose
This document describes the functional and non-functional requirements of the FlowJudge application.
The intended audience included:
 - Developers
 - Architects
 - Product stakeholders
 - Potential investors
 - Technical reviewers
This SRS defines the MVP scope for an AI-powered automated code review system.

## 1.2. Scope
FlowJudge is a SaaS application that integrates with source control platforms (e.g., GitHub, GitLab, Azure DevOps) and performs automated AI-based code reviews on Pull/Merge Requests (PR).

The system:
 - Automatically triggers review on PR creation or update
 - Analyzes changed code (diff-based review)
 - Publishes inline comments and a structured summary
 - Optionally sets a PR status check

The system does not replace static analysis tools or CI pipelines.

## 1.3. Definitions
| Term           | Definition                                               |
| -------------- | ---------------------------------------------------------|
| PR             | Pull Request / Merge Request                             |
| VCS            | Version Control System                                   |
| Inline comment | Comment attached to a specific line in PR diff           |
| Review Job     | Background process that performs analysis                |
| Tenant         | Logical isolation unit per repository or organization    |

# 2. Overall description
# 2.1. Product perpsective
FlowJudge is a standalone SaaS platform integrating with external VCS providers.

High-level architecture:
 - Webhook Receiver (API)
 - Review Processing Engine
 - AI Review Engine
 - Comment Publisher
 - Admin Web Panel
 - Database (multi-tenant)

The system depends on:
 - VCS APIs
 - External AI provider (e.g., OpenAI / Azure OpenAI)
 - Cloud infrastructure

## 2.2. Product functions (High level)
The system shall:
 - Integrate with VCS repositories.
 - Receive PR-related webhook events.
 - Retrieve PR metadata and diff.
 - Analyze changes using an AI engine.
 - Publish inline comments and summary.
 - Optionally set PR status check.
 - Enforce usage and cost limits.
 - Maintain audit metadata.

## 2.3. User classes
### Repository Administrator
 - Installs integration
 - Configures review settings
 - Views usage and review history

### Developer (PR Author / Reviewer)
 - Triggers manual review
 - Receives review comments
 - Requests re-run

## 2.4. Operating environment
 - Azure cloud-hosted
 - Backend: .NET 10+
 - Frontend: Angular 20+
 - Database: PostgreSql
 - AI Provider: OpenAI-compatible API
 - Identity server: Keycloak + social login
 - VCS APIs: GitHub, Azure DevOps

## 2.5. Design constraints
 - Must use diff-based review (not full repo analysis in MVP)
 - Must limit token usage per review
 - Must not permanently store source code diffs
 - Must support multi-tenant isolation
 - Must handle API rate limits from VCS providers

## 2.6. Assumptions and dependencies
Assumptions:
 - VCS providers expose stable webhook and PR APIs
 - AI provider supports structured JSON output
 - Internet connectivity is required

Dependencies:
 - External AI provider availability
 - VCS API rate limits and permissions

# 3. Specific requirements
## 3.1. Functional requirements
### 3.1.1. Integration
__FR-001__ - The system shall allow repository integration via supported authentication methods.  
__FR-002__ - The system shall validate required permissions before enabling review.  
__FR-003__ - The system shall display integration status.  
__FR-004__ - The system shall allow the selection of a repository for reviews.  
### 3.1.2. Review triggering
__FR-005__ - The system shall allow set triggering review policy per repository.  
__FR-006__ - The system shall automatically trigger review when a PR is created.  
__FR-007__ - The system shall trigger review when a PR is updated with new commits.  
__FR-008__ - The system shall allow manual review trigger via label or command.  
__FR-009__ - The system shall allow re-running a review.  
### 3.1.3. PR processing
__FR-010__ - The system shall retrieve PR metadata.  
__FR-011__ - The system shall retrieve changed files and diff.  
__FR-012__ - The system shall filter files based on: glob exclusions, file size limits, supported languages.  
__FR-013__ - The system shall enforce configurable limits for: maximum files, maximum changed lines.  
### 3.1.4. AI review engine
__FR-014__ - The system shall submit only changed code to the AI provider.  
__FR-015__ - The AI engine shall return structured output.  
__FR-016__ - The system shall classify findings by severity (High, Medium, Low).  
__FR-017__ - The system shall deduplicate findings.  
__FR-018__ - The system shall prioritize findings before publishing.  
### 3.1.5. Comment publishing
__FR-019__ - The system shall publish inline comments mapped to correct diff lines.  
__FR-020__ - The system shall publish a single structured summary comment.  
__FR-021__ - The system shall enforce maximum inline comment count.  
__FR-022__ - The system shall optionally publish a PR status check.  
### 3.1.6. Configuration
__FR-023__ - The system shall support predefined review modes: balanced, strict, security-first.  
__FR-024__ - The system shall allow exclusion of file patterns.  
__FR-025__ - The system shall allow configuration of severity thresholds for failure. 
### 3.1.7. Usage and cost control 
__FR-026__ - The system shall track number of reviews per repository.  
__FR-027__ - The system shall block reviews when limits are exceeded.  
__FR-028__ - The system shall estimate AI token usage per review.  
### 3.1.8. Authentication and authorization
__FR-029__ - The system shall allow create user account.  
__FR-030__ - The system shall allow change user account password.  
__FR-031__ - The system shall allow social login (google, github, microsoft).
__FR-032__ - The system shall logout user when session expires.  
__FR-033__ - The system shall block using when user did not accept EULA and GDPR.  
### 3.1.9. Audit and logging
__FR-034__ - The system shall store metadata of each review: PR ID, timestamp, status, numebr of findings  
__FR-035__ - The system shall log errors and retries.  

## 3.2. Non-functional requirements
### 3.2.1. Performance
__NFR-001__ - Review initiation shall occur within 5 seconds of webhook receipt.  
__NFR-002__ - For PRs under 1,000 changed lines, review completion should not exceed 60 seconds (under normal load).  
### 3.2.2. Scalability
__NFR-003__ - The system shall support horizontal scaling of review workers.  
__NFR-004__ - Review processing shall be asynchronous via queue-based architecture.  
### 3.2.3. Reliability
__NFR-005__ - Webhook processing shall be idempotent.  
__NFR-006__ - Transient API failures shall trigger automatic retry.  
### 3.2.4. Security
__NFR-007__ - The system shall isolate tenant data.  
__NFR-008__ - The system shall not permanently store full code diffs.  
__NFR-009__ - Secrets shall be masked before AI submission when detectable.  
__NFR-010__ - All communication shall use HTTPS.  
### 3.2.5. Maintainability
__NFR-011__ - The AI prompt configuration shall be versioned.  
__NFR-012__ - The review engine shall be modular to allow new review policies.  
### 3.2.6. Observability
__NFR-013__ - The system shall provide structured logs.  
__NFR-014__ - The system shall expose review metrics: review duration, failure rate, average token usage.  

# 4. Future scope (Post-MVP)
 - Multi-agent review (Security / Performance / Maintainability)
 - IDE plugin integration
 - Full repository architectural analysis
 - Team-level analytics dashboard
 - Self-hosted deployment option

# 5. Out of scope
 - Static code analysis replacement
 - CI/CD orchestration
 - Real-time collaborative review
 - Enterprise SSO (optional for later stage)

# 6. Approval
This document defines the MVP-level requirements and may evolve during iterative development.
