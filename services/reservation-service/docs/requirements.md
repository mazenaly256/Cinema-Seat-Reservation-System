# Reservation Service Requirements

## Responsibility

Handles seat reservations for showtimes. Furthermore, provides a representation for the seats layout in the hall with availability status.

## Functional Requirements

### Customer

- Get available seats
- Reserve seats for a showtime
- Cancel a reservation

### Admin

- Get reservations according to specific criteria

### System

- Prevent double booking
- Check the availability of the showtime and it is in future
- Check the availability of a seat

## Non-Functional Requirements

- Service is independently deployable
- Stateless service
