# Load Test Metrics — Get All Showtimes Endpoint (Within a Week From Today)

## Test Configuration

- **Endpoint:** `GET /api/showtimes?from=...&to=...`
- **Script:** `~/tests/performance/movie-service-showtimes-read.js`
- **Load Profile:** 50 max VUs — warmup (30s) → ramp up to 50 VUs (1m) → plateau (2m) → ramp down (30s)
- **Thresholds:** P95 < 2000ms, error rate < 5%
- **Environment:** k6 running locally (Egypt) → Azure VM (South Africa) → Neon PostgreSQL (EU Central, Frankfurt)

---

## Phase 1 — Before Indexes (Baseline)

| Run | P95    | Avg   | Min   | Max    | Throughput |
| --- | ------ | ----- | ----- | ------ | ---------- |
| 1   | 1190ms | 685ms | 333ms | 3490ms | 21.5 req/s |
| 2   | 825ms  | 671ms | 477ms | 2690ms | 21.6 req/s |
| 4   | 804ms  | 672ms | 485ms | 2620ms | 21.6 req/s |
| 5   | 797ms  | 670ms | 476ms | 3270ms | 21.6 req/s |
| 6   | 869ms  | 778ms | 484ms | 7120ms | 20.3 req/s |
| 7   | 793ms  | 671ms | 477ms | 2690ms | 21.6 req/s |
| 8   | 790ms  | 709ms | 581ms | 1880ms | 21.2 req/s |
| 9   | 847ms  | 712ms | 572ms | 1920ms | 21.1 req/s |
| 10  | 847ms  | 712ms | 572ms | 1920ms | 21.1 req/s |
| 11  | 813ms  | 748ms | 578ms | 2300ms | 20.7 req/s |
| 12  | 721ms  | 620ms | 468ms | 5480ms | 22.3 req/s |
| 14  | 814ms  | 661ms | 475ms | 1860ms | 21.7 req/s |
| 15  | 947ms  | 706ms | 339ms | 3340ms | 21.1 req/s |

**Averages:**

- P95: **~847ms**
- Avg: ~702ms
- Min: ~487ms
- Throughput ~21.3 req/s

---

## Phase 2 — After Indexes

| Run | P95    | Avg   | Min   | Max    | Throughput |
| --- | ------ | ----- | ----- | ------ | ---------- |
| 1   | 835ms  | 715ms | 552ms | 2130ms | 21.1 req/s |
| 2   | 1000ms | 776ms | 571ms | 3940ms | 20.3 req/s |
| 3   | 860ms  | 743ms | 574ms | 2140ms | 20.8 req/s |
| 4   | 803ms  | 610ms | 475ms | 2640ms | 22.5 req/s |
| 5   | 748ms  | 608ms | 480ms | 2710ms | 22.5 req/s |
| 6   | 888ms  | 648ms | 476ms | 3080ms | 22.0 req/s |

**Averages:**

- P95: **~856ms**
- Avg: ~683ms
- Min: ~521ms
- Throughput ~21.5 req/s

---

## Phase 3 — After Pagination

| Run | P95   | Avg   | Min   | Max    | Throughput |
| --- | ----- | ----- | ----- | ------ | ---------- |
| 1   | 506ms | 364ms | 301ms | 1740ms | 26.5 req/s |
| 2   | 505ms | 360ms | 313ms | 1550ms | 26.6 req/s |
| 3   | 505ms | 363ms | 302ms | 1680ms | 26.5 req/s |
| 4   | 503ms | 359ms | 312ms | 1480ms | 26.6 req/s |
| 5   | 502ms | 359ms | 312ms | 1640ms | 26.6 req/s |
| 6   | 507ms | 368ms | 311ms | 2000ms | 26.5 req/s |
| 7   | 607ms | 390ms | 307ms | 2320ms | 26.0 req/s |
| 8   | 510ms | 384ms | 312ms | 3040ms | 26.1 req/s |
| 9   | 513ms | 371ms | 315ms | 1520ms | 26.4 req/s |
| 10  | 502ms | 354ms | 309ms | 1410ms | 26.7 req/s |

**Averages:**

- P95: **~516ms**
- Avg: ~367ms
- Min: ~309ms
- Throughput ~26.5 req/s

---

## Final Comparison

| Metric      | Before Indexes | After Indexes | Delta (Indexes)      | After Pagination | Delta (Pagination vs Baseline) |
| ----------- | -------------- | ------------- | -------------------- | ---------------- | ------------------------------ |
| P95 Latency | ~847ms         | ~856ms        | +1% (no improvement) | **~516ms**       | **-39%**                       |
| Avg Latency | ~702ms         | ~683ms        | -3% (negligible)     | **~367ms**       | **-48%**                       |
| Min Latency | ~487ms         | ~521ms        | +7% (no improvement) | **~309ms**       | **-37%**                       |
| Throughput  | ~21.3 req/s    | ~21.5 req/s   | +1% (negligible)     | **~26.5 req/s**  | **+24%**                       |
| Error Rate  | 0%             | 0%            | —                    | **0%**           | —                              |

---

## Conclusion

- The minimum latency observed before pagination suggests a significant latency portion is due to cross-region communication between the client, application server, and database.
- Before pagination, the endpoint returned unbounded result sets — all matching showtimes for the queried date range with no page limit.
- Before Pagination, network latency dominates over request execution time, it is a huge portion from it.
- Indexes added on `StartTime` column in `Showtimes` table. EXPLAIN ANALYZE confirmed index adoption. No actual latency improvement observed — at 10,000 rows, the query execution optimization in time is atually invisible.
- Pagination (page size = 10) was implemented on `GET /api/showtimes` and demonstrated measurable performance improvements under load.
