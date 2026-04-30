# MVP Baseline Performance Metrics

## 1. Executive Summary

This report documents the baseline performance of the Cinema System MVP under stress. While the system maintains 100% integrity (zero failures), it exhibits severe latency degradation when resources are constrained, identifying a critical bottleneck in the synchronous communication chain.

---

## 2. Test

Testing Script: `/tests/performance/mvp-baseline-performance-test.js`

### Test Parameters

- **Load Profile:** 100 Virtual Users (VUs) over a 4-minute duration.
- **Environment:** Docker-constrained (0.5 CPU / 256MB RAM per service).
- **Target:** `GET /api/seats` (Reservation -> Movie Service sync call).

---

## 3. Core Performance Metrics

| Metric            | Result                   | Analysis                                            |
| :---------------- | :----------------------- | :-------------------------------------------------- |
| **Throughput**    | 22.88 req/s              | The system's current maximum processing capacity.   |
| **p(95) Latency** | 3.39s                    | 95% of request wait over 3 seconds                  |
| **Latency**       | Max: 48.79s - Avg: 1.96s | Indicates extreme thread starvation and DB queuing. |

---

## 4. Technical Findings

### 4.1 The "Sync Chain" Tax

The `http_req_duration` average of **1.96s** indicates that the synchronous HTTP jumps between the Reservation, Movie and SQL Server services is a major reason for the latency. In a resource-starved environment, the overhead of opening connections and waiting for serialized JSON responses causes delay.

### 4.2 Thread Starvation & The "Invisible Wait"

The huge difference between the Average (1.96s) and Max Latency (48.79s) is the most critical finding.

**The Cause:** Under the relatively high load with only 0.5 CPU, the requests pile up because there is no available threads (even with non-blocking, the load is more than the CPU limitation, so no idle thread and also not enough threads, so the request must wait), and each request still waiting for synchronous HTTP response from the next service in the chain, and another requests are queued waiting for a thread to become available.
