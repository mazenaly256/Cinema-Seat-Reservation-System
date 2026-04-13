# Showtimes API

## Base URL

`/api/showtimes`

---

## Endpoints

### Get all showtimes

GET /api/showtimes

### Get upcoming showtimes

GET /api/showtimes?status=upcoming

### Get showtimes by date and time range

GET /api/showtimes?from=2026-04-11T18:00:00&to=2026-04-11T18:00:00

### Get showtime by id

GET /api/showtimes/{showtimeId}

### Create a new showtime

POST /api/showtimes

### Update a showtime

PUT /api/showtimes/{showtimeId}

### Delete a showtime

DELETE /api/showtimes/{showtimeId}
