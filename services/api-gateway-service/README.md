# API Gateway Service

## Overview

The API Gateway is the centralized entry point and primary defense mechanism for the Cinema Seat Reservation System. Built using **YARP (Yet Another Reverse Proxy)**, it transforms a collection of independent services/containers into a cohesive, resilient ecosystem.

## Impact on the system

- **Latency Mitigation:** Reduced peak system-wide hang times from **48s** to a predictable **5s timeout**.
- **Resource Efficiency:** Protected **512MB RAM**-constrained containers from resource saturation and cascading failures by proactively reclaiming memory from stale asynchronous tasks that are not completed within the 5 seconds.
