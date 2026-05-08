# Critical Path's Latency Baseline Metrics

## 1. Summary

This report documents the baseline performance of the Cinema System MVP, and shows the effect of request timeouts and rate limiting on the user experience and the sanity of the system.

---

## 2. Test

Testing Script: `/tests/performance/critical-path-latency-baseline.js`

### Test Parameters

- **Load Profile:** 1500 Virtual Users (VUs) sending requests with rate around 100 request/second.
- **Environment:** Docker-constrained (0.5 CPU / 256MB RAM per service).
- **Target:** `GET /api/seats` (Gateway -> Reservation -> Movie Service sync call).

---

## 3. Core Performance Metrics (Average over multiple consecutive testings)

### 3.1 Without Request Timeouts and Rate Limiting

| Metric            | Result      | Analysis                                         |
| :---------------- | :---------- | :----------------------------------------------- |
| **Throughput**    | 100 req/sec | The system's current maximum processing capacity |
| **p(95) Latency** | 16s         | 95% of requests wait under 16 seconds.           |

### 3.2 After Applying a 5-second Request Timeout and Limiting Rate of Requests to 1000 Request Each 5 Seconds

| Metric            | Result    | Analysis                                          |
| :---------------- | :-------- | :------------------------------------------------ |
| **Throughput**    | 800 req/s | The system's current maximum processing capacity. |
| **p(95) Latency** | 7s        | 95% of requests wait under 7 seconds.             |

---

## 4. Technical Interpretations

### 4.1 Trade-off between Throughput & Latency and Correctness of the System

Without rate limiting and timeouts, the system throughput is relatively small (100 req/sec) due to the long time required to process all the requests without any limiting or timeouts. After applying the rate limiting and request timeouts, the throughput increased, due to the fast fails that made the system repond to many reuests without full processing, just by ignoring them due to rate limiting or timeouts.
Here we compromise the correctness of the system (that is responding with the expected data, not rate limited or timed out), in order to achive reasonable response time that improves the user experience, as from user's presepective a 16-second delay may means a system failure.

### 4.2 The "Sync Chain" Tax

One of the reasons for the relatively high latency (int both conditions) is the synchronous HTTP jumps between the Gateway, Reservation, Movie and SQL Server services. In a tightly-constrained environment, the overhead of opening connections and waiting for serialized JSON responses causes delay.

### 4.3 Thread Starvation

**The Cause:** Under the relatively high load with only 0.5 CPU and 256MB RAM, the requests pile up because there is no available threads (even if no threads are blocked due to the Async programming, the load is more than the container limitations, so no idle/blocked thread and also there is not enough threads), and each request has to wait for synchronous HTTP response from the next service in the chain, and another requests are queued waiting for a thread to become available.
