# FlowJudge

**FlowJudge** is an AI-powered code review bot that automatically reviews Pull Requests / Merge Requests and adds **inline comments directly in the diff**, just like a human reviewer would.

It is designed primarily for **solo developers, freelancers, and small teams** who want consistent, high-quality code reviews without needing a dedicated reviewer.

> _“Code review where it matters — inside your PR.”_

---

## 🚀 What FlowJudge Does

When a Pull Request (PR/MR) is created or updated, FlowJudge:

1. Fetches the **diff** of the PR
2. Analyzes only the **changed code**
3. Uses an **AI review engine** to detect:
   - bugs and logic issues
   - security risks
   - maintainability problems
4. Posts:
   - **inline comments** on the exact lines in question
   - a **single summary comment** with priorities and next steps
   - an optional **status check** (pass / warn / fail)

FlowJudge focuses on **useful, high-signal feedback**, not lint noise.

---

## 🎯 Target Users

- Solo developers
- Junior / mid-level developers
- Freelancers reviewing their own work
- Small teams without formal code review processes

---

## 🔑 Key Features (MVP)

- 🔗 Native PR integration (GitHub / GitLab / Azure DevOps)
- ⚡ Automatic review on PR creation or update
- 🧠 AI-powered code analysis (context-aware)
- 💬 Inline comments mapped to correct diff lines
- 📋 Human-readable summary review
- 🛠 Configurable review rules per repository
- 🔄 Manual trigger via label or comment command
- 🚦 Optional merge gate via status checks

---

## 🧠 How the AI Review Works (High Level)

FlowJudge is **not a chatbot**. It uses a structured review pipeline:

1. **Deterministic pre-analysis**
   - file type detection
   - language detection
   - security-sensitive area heuristics
2. **AI analysis**
   - reviews diff hunks with contextual prompts
   - returns structured JSON findings
3. **Post-processing**
   - deduplicates issues
   - prioritizes by severity and confidence
4. **Publishing**
   - maps findings to inline comments
   - posts a clean summary comment

This approach keeps reviews predictable, testable, and cost-controlled.

---

## 🏗 Architecture Overview

### Backend
- .NET 10
- Clean Architecture
- Webhook receiver + background job processing
- REST API for configuration and audit

### AI Layer
- OpenAI / Azure OpenAI
- Function calling with strict JSON schema
- Prompt versioning and cost tracking

### Frontend
- Angular 20+
- Lightweight admin panel for:
  - repository configuration
  - review rules
  - limits and usage

### Infrastructure
- Cloud-native (Azure / AWS)
- Queue-based processing
- Token and cost limits per repository
- CI/CD with GitHub Actions

---

## ⚙️ Supported Triggers

- PR / MR opened
- PR / MR updated (new commits)
- Manual trigger:
  - label (e.g. `flowjudge-review`)
  - comment command (e.g. `/flowjudge review`)

---

## 🛡 Security & Privacy

- Reviews **only changed code**, not entire repositories
- No long-term storage of source code diffs
- Secret masking before AI processing
- Support for enterprise-friendly AI providers (e.g. Azure OpenAI)
- Minimal permission scopes

---

## 🧭 Project Status

🚧 **Early development / MVP phase**

Planned roadmap:
- [ ] GitHub / Azure DevOps integration
- [ ] AI review engine v1 (C# + TypeScript)
- [ ] Inline comment publishing
- [ ] Summary & status checks
- [ ] Basic admin UI
- [ ] Usage limits & billing

---

# Documentation
- ### [Software Requirements Specification (SRS)](docs/system-requirements-specification.md)
- ### [Use cases](docs/use-cases.md)
- ### [Architecture](docs/architecture.md)
- ### [Tech stack](docs/techstack.md)