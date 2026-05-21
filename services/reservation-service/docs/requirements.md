# Reservation Service Requirements

## Responsibilities

- Authorization
- Handle seat reservations for showtimes.
- Provide a representation for the seats layout in the hall with availability status.
- Communicate with Movie Service to validate showtime data.

## Functional Requirements

### Customer

- Get reserved or available seats for a specific showtime
- Reserve seats for a showtime

### General

- Prevent double booking
- Check the availability of the showtime and it is in future (via Movie Service)
- Check the availability of a seat

## Non-Functional Requirements

- Service is independently deployable
- Stateless service
- Communicate with movie service
