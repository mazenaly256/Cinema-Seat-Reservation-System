# Cinema Seat Reservation System

_A cloud-native, microservices-based ecosystem developed in ASP.NET Core with REST APIs. This system features deliberate design decisions optimized for modularity, scalability and resilience — implementing proven distributed systems patterns such as Retries and Circuit Breakers and Request Timeouts alongside automated CI/CD pipelines._

---
## Architectural Overview
### Diagram
![System Architecture Diagram](docs/system-architecture-diagram.png)

### Services

- **Movie Service:** Responsible for movies and showtimes.
- **Reservation Service:** Responsible for seats layout representation and seat reservation.
- **Identity Service:** Acts as the centralized Identity Provider (IdP) for the system.
- **API Gateway:** Centralizes the logic of authentication and applies rate limiting, timeouts, and routing policies for all incoming traffic.
---
## API Documentation
[Postman API Workspace](https://www.postman.com/mazenaly256-3830648/workspace/cinema-seat-reservation-system-api-documentation)
