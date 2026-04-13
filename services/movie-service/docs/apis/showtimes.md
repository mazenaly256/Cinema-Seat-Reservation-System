# Showtimes API

## Base URL

`/api/showtimes`

---

## Endpoints

### Get all showtimes

`GET /api/showtimes`

Supports query parameters:

- movieId (optional) → filter by movie
- status (optional) → upcoming (its start time after the moment of querying)
- from (optional)
- to (optional)

#### Examples

`GET /api/showtimes?status=upcoming`

`GET /api/showtimes?from=2026-04-11T08:00:00&to=2026-04-11T20:00:00`

`GET /api/showtimes?movieId=123&status=upcoming`

### Get showtime by id

`GET /api/showtimes/{showtimeId}`

### Create a new showtime for a movie

`POST /api/showtimes`

### Update a showtime

`PUT /api/showtimes/{showtimeId}`

### Delete a showtime

`DELETE /api/showtimes/{showtimeId}`
