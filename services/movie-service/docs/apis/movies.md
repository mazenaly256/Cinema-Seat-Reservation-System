# Movies API

## Base URL

`/api/movies`

---

## Endpoints

### Get all movies

`GET /api/movies`

Supports query parameters:

- status (optional) → now-showing (have future upcoming showtimes)

#### Examples

`GET /api/movies` → get all movies on the system

`GET /api/movies?status=now-showing` → get all movies that have upcoming showtimes

### Get movie by id

`GET /api/movies/{movieId}`

### Create a new movie

`POST /api/movies`

### Update a movie

`PUT /api/movies/{movieId}`

### Delete a movie

`DELETE /api/movies/{movieId}`
