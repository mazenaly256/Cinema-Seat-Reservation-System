# Reservation Service API

## Base URL

`/api`

---

## Reservations Endpoints

### Create a new reservation (only after a successful payment)

`POST /api/reservations`

---

## Seats Endpoints

### Get seats by status and showtime

`GET /api/seats`
Supports query parameters:

- status (required)
  - available → seats that are not reserved (free seats)
  - reserved → already reserved seats
- showtimeId (required) → specify the showtime of the queried seats

#### Examples

`GET /api/seats?status=available&showtimeId=30000000-0000-0000-0000-000000000001`

`GET /api/seats?status=reserved&showtimeId=30000000-0000-0000-0000-000000000003`
