# Architecture Decision Record (ADR): Dedicated Identity Service

## Date

2026-05-21

## Context

The system requires an approach to handle authentication and authorization without bloating business microservices or violating Aspect-Oriented Programming (AOP).

---

## Decision

Isolate all credentials handling and token issuing into a standalone **Identity Service** backed by its own database. The security boundaries are strictly distributed over:

1. **Identity Service:** Responsible for _credential authentication_, it validates user credentials against its isolated database and issues signed JWTs.
2. **API Gateway:** Responsible for _edge authentication_, which is just verifying the JWT's cryptographic signature and determining its validity. It blocks invalid tokens and forwards valid ones.
3. **Downstream Services:** Movie and Reservation services are responsible for authorization, they accept the _trusted_ claims from the Gateway (after the JWT has been successfully validated) and determine whether to allow access to the requested endpoint or not, depending on the received JWT.

---

## Alternatives Considered

- **Full Authentication in Gateway:** Rejected because the database connections and the overhead of token issuance and password-hashing may degrade gateway throughput and exhaust container resource limits, causing it to significantly diverge from its main responsibility.

- **Full Authentication in Each Service:** Rejected because it duplicates all authentication logic across every service.

---

## Consequences

- **Positive:** Strict separation of concerns, optimized resource consumption, network-level zero-trust for credentials, and microservice autonomy.

- **Negative:** Requires an additional HTTP request to the _Identity Service_ with credentials during login and registration flows.

---

## Key Insight

> The Identity Service acts as the _factory_ that issues JWTs.
