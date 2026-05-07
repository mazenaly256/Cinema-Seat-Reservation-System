# Architecture Decision Record (ADR): API Gateway Implementation

## Context

In the MVP version, the **Reservation Service** communicates directly with the **Movie Service**. While this is functional, it comes with several problems:

- **Client Complexity:** Frontend applications must manage multiple base URLs and internal ports for different services.
- **Cascading Failures:** If one service fails or takes a long time to respond, the calling services will also have to wait or maybe fail not due to bug but due to the dependency, leading to the observed latency that spikes up to **48.79s** during the baseline performance measuring. Without a central "Fail-Fast" layer, one slow service can cause slowing down of the entire system.
- **Security Surface:** Every service must individually manage CORS, SSL, and authentication, increasing the headache of configurations and repetitive logic.

---

## Decision

Implement **YARP (Yet Another Reverse Proxy)** as a centralized **API Gateway** to act as the single entry point for all external traffic.

**The Flow:**

1.  **Client Request:** All requests hit the Gateway (e.g., `:8000/api/movies/*` or `:8000/api/reservations/*`) not separate services.
2.  **Reverse Proxying:** YARP routes the request to the appropriate internal service (Docker container).
3.  **Resilience Enforcement:** The Gateway applies a strict **Request Timeout** (e.g., 5 seconds) to prevent the 48-second hang observed in MVP in baseline metrics.

---

## Alternatives Considered

### Ocelot

- **Pros:** Mature, widely used in the .NET community.
- **Cons:** Configuration-heavy and slower performance benchmarks compared to YARP in high-concurrency .NET 10 environments.

### Nginx / Kong

- **Pros:** Industry standards for high-performance proxying.
- **Cons:** Moves logic outside the .NET ecosystem. Using YARP allows us to keep our "Quality Gate" within C#, enabling us to write custom middleware or authentication logic using familiar tools.

---

## Consequences

- **Centralized Resilience:** We can now implement Rate Limiting and Timeouts in only one place to protect all the services
- **Simplified Security:** Future JWT validation will happen at the Gateway, so internal services can trust the headers they receive.
- **Operational Overhead:** We have added a third service to the `docker-compose.yml`, slightly increasing the infrastructure management load.

---

## Key Insight

> The Gateway isn't just a router; it's a **shield**. It transforms our collection of services into a single, cohesive 'System' by centralizing cross-cutting concerns like security and failure management.
