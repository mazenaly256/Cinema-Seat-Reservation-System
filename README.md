# Cinema Seat Reservation System

_A distributed microservices architecture built on ASP.NET and Docker. Designed for modularity and scalability_

## Architectural Overview

This system is a multi-service ecosystem designed to demonstrate scalable backend patterns, automated infrastructure management, and performance optimization in resource-constrained environments.

![Architecture Diagram](docs/system-architecture-diagram.png)

## Services

- **Movie Service:** Responsible for movies and showtimes.
- **Reservation Service:** Responsible for seats layout representation and seat reservation.
- **Identity Service:** Acts as the centralized Identity Provider (IdP) for the system.
- **API Gateway:** Centralizes the logic of authentication and apply rate limits and timeouts for requests.

## API Documentation
https://www.postman.com/mazenaly256-3830648/workspace/cinema-seat-reservation-system-api-documentation
