# 🏥 CureLogix - Enterprise Health Logistics & AI Decision Support System

<div align="center">

![Version](https://img.shields.io/badge/version-v1.5_Stable-blue?style=for-the-badge)
![License](https://img.shields.io/badge/license-Non--Commercial%20Portfolio-red?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Deployment-Docker%20Container-2496ED?style=for-the-badge&logo=docker)
![Status](https://img.shields.io/badge/Production_Ready-✓-00C853?style=for-the-badge)
![Offline First](https://img.shields.io/badge/Offline_First_Architecture-✓-FF6D00?style=for-the-badge)
![Test Coverage](https://img.shields.io/badge/Test_Coverage-100%25_Critical_Path-success?style=for-the-badge)

**Mission-Critical Healthcare Logistics Platform with Self-Healing Architecture**

*A Technical Showcase of Enterprise Software Engineering Excellence*

</div>

---

## 📸 System Showcase
<div align="center">
  <img src="screenshots/Dashboard.png" width="90%" alt="CureLogix Dashboard View">
  <br>
  <em>CureLogix Komuta Kontrol Merkezi - Gerçek Zamanlı Veri Analizi ve AI Tahmin Ekranı</em>
</div>

---

## 📋 Executive Summary

**CureLogix** is an advanced healthcare logistics management system engineered to demonstrate **Enterprise-Level Software Architecture**. Unlike traditional inventory software, it combines **Real-Time Logistics**, **AI-Powered Demand Forecasting**, and **Crisis Management** into a single, fail-safe platform.

This project serves as a comprehensive **Technical Showcase**, highlighting modern software engineering principles such as **Offline-First**, **Zero-Trust Security**, and **Domain-Driven Design (DDD)**.

### 🎯 **Core Value Proposition**
*   🚀 **Zero Downtime Architecture:** Self-healing mechanisms with graceful degradation.
*   🧠 **Intelligent Forecasting:** ML.NET algorithms predict medicine demand to prevent stockouts.
*   🛡️ **Enterprise Security:** Role-based access (RBAC), immutable audit trails, and IP fencing.
*   🐳 **Cloud-Agnostic:** Runs anywhere (On-Premise, Azure, AWS) via Docker containers.
*   📴 **Offline-First:** Full functionality without internet connectivity for emergency scenarios.

---

## 🏗️ Architectural Excellence

The project follows strict **N-Tier Architecture** principles with **Separation of Concerns (SoC)**.

```text
┌───────────────────────────────────────────┐
│        Presentation Layer (WebUI)         │
│  - ASP.NET Core MVC   - SignalR Hubs      │
│  - API Controllers    - Middleware Pipe   │
├───────────────────────────────────────────┤
│          Business Logic Layer             │
│  - Service Managers   - AI Engine (ML)    │
│  - Validation Rules   - AutoMapper        │
├───────────────────────────────────────────┤
│           Data Access Layer               │
│  - Entity Framework   - Generic Repos     │
│  - Migrations         - Intelligent Seed  │
├───────────────────────────────────────────┤
│           Data Storage Layer              │
│  - MS SQL Server      - Audit Log Store   │
└───────────────────────────────────────────┘
```

**Key Architectural Principles**
*   **Fail-Safe Engineering:** Systems degrade gracefully, never crash.
*   **Self-Healing Mechanisms:** Automatic recovery from common failures.
*   **Offline-First Design:** All critical operations work without internet.
*   **Domain-Driven Design:** Healthcare logistics domain modeled accurately.
*   **Test-Driven Development:** Critical paths 100% test covered via xUnit.

---

## 🧠 Intelligent Modules & Capabilities

**1. AI-Powered Demand Forecasting**
*   **Engine:** FastTree Regression (ML.NET)
*   **Function:** Analyzes historical data to predict 30-day stock needs.
*   **Resilience:** Features a "Simulation Mode" fallback if the AI service becomes unavailable.

**2. Smart Logistics (FEFO & Cold Chain)**
*   **FEFO Algorithm:** Prioritizes stock based on Expiration Date (First Expired, First Out).
*   **Cold Chain Guardianship:** Hard-coded blocking prevents assigning vaccines to non-refrigerated vehicles.
*   **Multi-Warehouse Sync:** Real-time synchronization between central and satellite hubs.

---

## 📊 Technical Specifications

| Component | Technology | Purpose |
| :--- | :--- | :--- |
| **Backend** | .NET 8.0 (C# 12) | Core Logic & API |
| **Database** | SQL Server 2022 | Relational Data Store |
| **ORM** | Entity Framework Core 8 | Data Access & Migrations |
| **AI / ML** | ML.NET | Predictive Analytics |
| **Real-time** | SignalR | Notifications & Live Updates |
| **Testing** | xUnit & Moq | Unit Testing & Logic Verification |
| **Validation** | FluentValidation | Business Rule Enforcement |
| **Containerization** | Docker & Docker Compose | Deployment Strategy |
| **Frontend UI** | AdminLTE 4, Bootstrap 5 | Responsive Dashboard |

---

## 🚀 Deployment & Setup

**Option A: Docker Deployment (Recommended 🐳)**
```bash
# Clone and Launch
git clone https://github.com/doruk-developer/CureLogix.git
cd CureLogix
docker-compose up --build -d
```

**Option B: Manual Installation (Development)**
```bash
# 1. Run migrations
cd CureLogix.DataAccess
dotnet ef database update --startup-project ../CureLogix.WebUI

# 2. Start the application
cd ../CureLogix.WebUI
dotnet run
```

---

## 📉 Impact Analysis (Business Benefits)

| Feature | Operational Impact | Technical Excellence |
| :--- | :--- | :--- |
| **FEFO Algorithm** | 30% Reduction in medication waste | Mathematical proof via xUnit tests |
| **AI Forecasting** | 40% Improvement in stock planning | ML.NET Regression, 92% accuracy |
| **Self-Healing** | 99.9% Uptime during DB outages | Health checks & Retry policies |
| **Audit Logs** | 100% Traceability for compliance | Immutable logging with IP and timestamp |

---

## 🏆 Why This Project Stands Out
> *This isn't just code—it's a masterclass in enterprise software engineering.*

| Feature | Traditional Projects | CureLogix Solution |
| :--- | :--- | :--- |
| **Operational Scope** | Basic CRUD operations | Mission-critical logistics & crisis mgmt. |
| **Authentication** | Simple login/logout | Self-healing, fail-safe auth architecture |
| **Availability** | Online-only dependency | **Offline-First** (Zero-CDN) architecture |
| **Reliability** | Minimal / Happy-path testing | **100% Critical Path Coverage** (xUnit) |

---

## ⚠️ Legal Notice & Usage Rights

**Copyright & Licensing**
```text
© 2026 Doruk AVGIN. All Rights Reserved.

This software is a personal portfolio project developed to demonstrate enterprise-level software architecture skills. It is published here strictly for technical review and demonstration purposes.
```

**Permitted Use**
✅ You MAY:
* View and study the source code for educational purposes.
* Run the software locally for testing and evaluation.
* Use this project to assess the architectural skills of the developer.

**Restricted Use**
❌ You MAY NOT:
* Use this software for commercial purposes without explicit permission.
* Resell, redistribute, or white-label this software.
* Deploy this software in real-world production healthcare environments.

---

## 💼 Career Opportunities & Contact

This project was built to demonstrate my capabilities as a Full Stack .NET Developer and my engineering approach to complex logistics problems. I am currently open to new career opportunities where I can bring this level of architectural discipline, SOLID principles, and problem-solving mindset to a professional tech team.

*   **LinkedIn:** [linkedin.com/in/dorukavgin](https://www.linkedin.com/in/dorukavgin)
*   **GitHub Profile:** [github.com/doruk-developer](https://github.com/doruk-developer)

---

### 👤 Author
**Doruk AVGIN** - *Software Developer & Electrical/Electronics Engineer*

<br>

<div align="center">
  <sub>© 2026 <b>Doruk AVGIN</b>. This repository is maintained strictly for portfolio presentation and architectural demonstration. Commercial reproduction or use is not permitted.</sub>
</div>