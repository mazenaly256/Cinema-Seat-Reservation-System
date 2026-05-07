# Gateway Service Requirements

## Responsibility

Receives all the requests that come to the system, acting as a single entry point for all client requests. It abstracts the internal microservice structure from the outside world, providing a unified API surface that can apply centralized logic and constraints.

## Functional Requirements

- **Routing:** It must analyze incoming HTTP request paths (e.g., /movies/ or /reservations/) and forward them to the correct service (hence it is called "Proxy")
- **Rate Limiting:** Protects the system from buggy clients by limiting each user to a maximum number of requests per time window.
- **Authentication & Authorization:** The Gateway validates the JWT and user permissions before the request ever hits the internal services. If the token is fake or expired or even correct but not authorized to use that api, the request is killed at the edge.

## Non-Functional Requirements

- **Independently Deployable:** Must be containerized and capable of being deployed or updated without impacting the availability of the Movie or Reservation services.
- **Low Latency:** As the entry point for every request, the routing logic must have minimal overhead (target <10ms).
- **Horizontally Scalable:** The service must be _stateless_ (does not store user-related data locally) to allow multiple instances to run behind a Load Balancer without any inconsitencies.
- **High Availability:** Must support health checks (e.g., /health) so an orchestrator can automatically restart or replace failing instances.
