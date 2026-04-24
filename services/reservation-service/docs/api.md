# Reservation Service API

## Base URL

`/api`

---

## Reservations Endpoints

### Create a new reservation (only after a successful payment)

`POST /api/reservations`

---

## Seats Endpoints

### Get seats by showtime and availability

`GET /api/seats`
Supports query parameters:

- showtimeId (required) → specify the showtime of the queried seats
- available (required)
  - available → seats that are neither reserved nor locked/held (free seats)
  - reserved → seats that are either reserved or locked/held

#### Examples

`GET /api/seats?status=available&showtimeId=30000000-0000-0000-0000-000000000001`

`GET /api/seats?status=reserved&showtimeId=30000000-0000-0000-0000-000000000003`
