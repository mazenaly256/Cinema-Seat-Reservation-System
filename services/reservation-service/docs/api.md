# Reservation Service API

## Base URL

`/api`

---

## Reservations Endpoints

### Get a reservation by id

`GET /api/reservations/{reservationId}`

### Create a new reservation (only after a successful payment)

`POST /api/reservations`

### Delete a reservation

`DELETE /api/reservations/{reservationId}`

---

## Seats Endpoints

### Get available seats

`GET /api/seats`
Supports query parameters:

- showtimeId (required) → gets the available seats for a specific showtime
