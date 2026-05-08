# API Gateway Service

## Overview

The API Gateway is the centralized entry point and primary defense mechanism for the Cinema Seat Reservation System. Built using **YARP (Yet Another Reverse Proxy)**, it transforms a collection of independent services/containers into a cohesive, resilient ecosystem.

## Impact on the system

- **Latency Limiting:** Limit peak system-wide hang times to a predictable **10s timeout**.
- **Resource Efficiency:** Protected **256MB RAM**-constrained containers from resource saturation and cascading failures by proactively reclaiming the resource from stale incompletely processed requests that are not completed within the 10 seconds.
