# Architecture Decision Record (ADR): API Gateway Implementation

## Context

In the MVP version, the **Reservation Service** communicates directly with the **Movie Service** and both should be called separately. While this is functional, it comes with several problems:

- **Client Complexity:** Frontend applications must manage multiple base URLs and internal ports for different services.
- **Cascading Failures:** If one service takes a long time to respond (which is considered as a failure), the calling services will also have to wait.
- **Security Surface:** Every service must individually manage CORS, SSL, and authentication, increasing the headache of configurations and repetitive logic.

---

## Decision

Implement **YARP (Yet Another Reverse Proxy)** as a centralized **API Gateway** to act as the single entry point for all external traffic.

**The Flow:**

1.  **Client Request:** All requests hit the Gateway container (e.g., `:8080/api/movies/*` or `:8080/api/reservations/*`) not separate services.
2.  **Reverse Proxying:** YARP routes the request to the appropriate internal service (Docker container).
3.  **Resilience Enforcement:** The Gateway applies a strict **Request Timeout** (e.g., 10 seconds) to prevent the long hangs.

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
- **Simplified Security:** Future JWT validation will happen at the Gateway, so internal services can trust the headers they receive, as the services _can not be reached except through the gateway_. If the token is fake or expired or even correct but not authorized to use that endpoint, the request is killed at the edge.
- **Operational Overhead:** We have added a third service to the `docker-compose.yml`, slightly increasing the infrastructure management load.

---

## Key Insight

> The Gateway isn't just a router; it's a **shield**. It transforms our collection of services into a single, cohesive 'System' by centralizing cross-cutting concerns like security and failure management (failure includes here long time to response).
