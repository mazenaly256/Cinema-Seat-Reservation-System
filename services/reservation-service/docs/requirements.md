# Reservation Service Requirements

## Responsibility

Handles seat reservations for showtimes and provides a representation for the seats layout in the hall with availability status. Furthermore, communicate with Movie Service over HTTPS to validate showtime data.

## Functional Requirements

### Customer

- Get reserved or available seats for a specific showtime
- Reserve seats for a showtime

### System

- Prevent double booking
- Check the availability of the showtime and it is in future (via Movie Service)
- Check the availability of a seat

## Non-Functional Requirements

- Service is independently deployable
- Stateless service
- Communicate with movie service
