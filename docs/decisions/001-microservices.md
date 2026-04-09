# Architecture Decision Record (ADR): Microservices for Cinema Reservation System

## Context
The Cinema Reservation System needs to support future growth, independent development of modules, and scalability.  
A monolithic approach could be simpler to start with but would tightly couple components, making future maintenance and scaling difficult.

## Decision
Use a **microservices architecture**, splitting the system into at least two services:  

- **movie-service** – handles movies and showtimes.  
- **reservation-service** – handles reservations and user interactions.  

## Alternatives Considered
### Monolith / Modular Monolith
- **Pros:**  
  - Simpler to run locally.  
  - Easier initial development.  
  - Easier deployment.  
- **Cons:**  
  - Tightly coupled system.  
  - Difficult to scale or develop individual parts independently.  
  - Harder to maintain long-term.

### Microservices
- **Pros:**  
  - **Scalable:** Each service can scale independently (e.g., reservation-service may need more resources than movie-service).  
  - **Decoupled:** Each service is isolated, having its own database and cloud resources, reducing interdependencies.  
  - **Maintainable:** Services can evolve independently, allowing isolated growth and easier updates.  
- **Cons:**  
  - **Complex local setup:** Requires running multiple processes concurrently, managing ports, and service discovery.  
  - **Network latency:** Inter-service communication is slower than direct communication between sections in a monolith application.  

## Consequences
- Deployment will require orchestration (e.g., Docker Compose or Kubernetes).  
- Testing and monitoring will need to handle multiple services.  
- The system becomes more resilient to failure in individual services but requires proper error handling and retries.