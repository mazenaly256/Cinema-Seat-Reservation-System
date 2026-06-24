# Architecture Decision Record (ADR): Pagination Over Caching (To Improve Performance and Reduce Latency)

## Status

Accepted

## Date

2026-06-24

## Context

After we reached the production-level product, including all main features, the system is tested to know its bottlenecks, load testing via k6 under 50 concurrent users revealed that `GET /api/showtimes?from=...&to=...` had a P95 latency of \~850ms with a minimum of \~470ms per every single request. The minimum latency represents the fixed network round trip between the Azure VM (South Africa) and the Neon PostgreSQL instance (EU Central, Frankfurt) _that transmits the whole requested data without pagination_, assuming that the code is very efficient.
Testing script can be found at `~/tests/performance/movie-service-showtimes-read.js`.

Database indexes were added on `StartTime` column. EXPLAIN ANALYZE on PostgreSQL confirmed that the query planner adopted the indexing. However, no measurable latency improvement was observed — at 10,000 rows, the sequential scan is fast enough that the index benefit is smaller than measurement noise. The bottleneck is not query performance, it is network latency to the remote database.

The endpoint currently returns all matching showtimes unbounded. As the dataset grows, this produces:

- Increasing response payload size
- Increasing serialization time
- Increasing the data transmission time
- Increasing bandwidth consumption
- High latency under high load

## Decision

Implement pagination on `GET /api/showtimes` and `GET /api/movies`, returning a fixed page size of 10 results per request.

Caching was evaluated and rejected for the following reasons:

**Caching (common problems in both distributed and in memory):**

- Range-based queries (`from/to` date filters) produce highly variable cache keys as if data of 2 days is requested then data of five days that span those two days is requested again, then actually we absolutely need to hit database again as we do not know if there are showtimes in those excess 3 days or not — cache usage rate would be low
- Cache invalidation on create/update/delete for the domain resources adds complexity that it actually does not pay off.
- Full-list caching becomes impractical as the dataset grows to 100k+ rows

**In-memory caching:**

- In a multi-containered deployment, each container instance maintains its own isolated cache, this produces inconsistent responses, as each container will have its own data separately — unacceptable for a booking system.
  Furthermore, syncing all those data will add significant complexity.

**Distributed caching (via Redis or any similar):**

- Introduces its own network round trip between the service and the Redis instance, that introduce more network obverhead that will increase latency.

**Pagination:**

- Returns a fixed, bounded result set regardless of total row count, this will reduces serialization time, consumed bandwidth, and response payload per request that will for sure reduce the latency
- Scales correctly to 100k+ rows _without any architectural changes_
- Works identically across multiple container instances multi-containered deployment — Database is the single source of truth
- No cache invalidation complexity

## Consequences

- Frontend clients must implement pagination support (page number or cursor parameter)
- A single request no longer returns all matching showtimes — clients fetch pages on demand
- P95 latency is expected to decrease due to reduced payload size and serialization time
- The system scales correctly as showtime volume grows, so future scalability without rearchitecting overhead
