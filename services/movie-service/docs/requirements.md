# Movie Service Requirements

## Responsibilities

- Role-based authorization according to the request headers that added by the Gateway
- Manage movies and showtimes.
- Provides information to other services like Reservation Service.

## Functional Requirements

### Customer

- Get movies
- Get showtimes

### Admin

- Manage movies
- Manage showtimes

### General

- Provide showtime info to Reservation Service
- Validate movie and showtime existence

## Non-Functional Requirements

- Independently deployable
