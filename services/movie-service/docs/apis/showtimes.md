# Showtimes API

## Base URL

`/api/showtimes`

---

## Endpoints

### Check existence of an upcoming showtime by id

`HEAD /api/showtimes/{showtimeId}`

Returns:

- `200 OK` — exists
- `404 Not Found` — has already started or does not exist (no _upcoming_ showtime with the given id)

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
