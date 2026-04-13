# Movies API

## Base URL

`/api/movies`

---

## Endpoints

### Get all movies

GET /api/movies?status=all

### Get movies that have upcoming showtimes

GET /api/movies?status=now-showing

### Get movie by id

GET /api/movies/{movieId}

### Create a new movie

POST /api/movies

### Update a movie

PUT /api/movies/{movieId}

### Delete a movie

DELETE /api/movies/{movieId}

### Get all showtimes for a movie

GET /api/movies/{movieId}/showtimes

### Get upcoming showtimes for a movie

GET /api/movies/{movieId}/showtimes?status=upcoming
