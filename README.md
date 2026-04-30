# Cinema Seat Reservation System

_A distributed microservices architecture built on ASP.NET and Docker. Designed for modularity and scalability_

## Architectural Overview

This system is a multi-service ecosystem designed to demonstrate scalable backend patterns, automated infrastructure management, and performance optimization in resource-constrained environments.

### Core Services

- **Movie Service:** Orchestration of cinema catalogs, metadata, and showtime scheduling.
- **Reservation Service:** Manages high-concurrency seat locking and booking transactions.
- **User Service:** Acts as the centralized Identity Provider (IdP) for the ecosystem.

## Phase 1: Infrastructure & DevOps (Completed)

The primary focus of this phase was establishing a stable, reproducible foundation and a professional "Quality Gate".

- **Containerization:** Full orchestration via **Docker Compose** to ensure environment parity between development and CI/CD runners.
- **CI/CD Pipeline:** Implementation of **GitHub Actions** for automated build validation, .slnx solution linting, and Docker configuration verification.
- **Modern Tooling:** Migration to the **.slnx solution format** for streamlined project management within the .NET 10 ecosystem.

## Performance Baseline & Metrics

To drive architectural decisions, we established a data-driven baseline under simulated production pressure:

- **Test Scenario:** 100 Virtual Users (VUs) executing 5,000 total requests via k6.
- **System Constraints:** Services restricted to **0.5 CPU** and **512MB RAM**.
- **Key Finding:** Maximum latency reached **48.79s** due to Thread Starvation and Task Scheduler saturation.
- **Optimization Target:** This bottleneck will be mitigated in Phase 2 via Gateway-level timeouts and resilience patterns.

## Development Roadmap

1.  **Phase 1 (Completed):** Infrastructure, CI/CD, and Performance Benchmarking.
2.  **Phase 2 (Current):** System Hardening via **YARP Gateway**, Request Timeouts, and JWT Security.
3.  **Phase 3 (Planned):** Distributed Caching (Redis), Observability (Serilog), and Metric-driven tuning.
4.  **Phase 4 (Planned):** Advanced Patterns including Domain-Driven Design (DDD) and Payment Integration.
