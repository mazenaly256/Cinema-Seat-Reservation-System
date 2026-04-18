# Architecture Decision Record (ADR): Concurrent Reservation Handling

## Context

In the reservation service, multiple users may attempt to reserve the same seat concurrently (at the same instant).

Under concurrent access, multiple users may observe the same seat as available, leading to a race condition where more than one user proceeds to payment for the same seat, resulting in inconsistent system state and failed reservation handling as users may pay for seats they do not receive.

---

## Decision

Use **seat locking/holding _before_ payment** to handle the race condition at a safe stage.

**Flow:**

1. User selects a seat
2. System attempts to save it as a temporary seat hold for a some minutes:
   1. **Seat hold succeeds** → Proceed to payment
   2. **Seat hold fails (duplicate)** → Reject the reservation request, preventing the user from proceeding to payment
3. Payment succeeds (if the seat hold succeeded) → Create a reservation record in database

---

## Alternatives Considered

### Store Only Final Paid Reservations

- **Pros:**
  - Simpler schema (one table)

- **Cons:**
  - Two users can read "available" at same instant
  - Both pay, database rejects one insert to reservations table
  - **Race condition handled at wrong stage, Loser paid for nothing**
  - Refunds, angry users, broken trust

---

## Consequences

- Race condition still exists but is **moved to a safe stage** where it does not affect correctness or user experience
- System behavior is consistent and predictable under concurrency
- Requires background cleanup of expired holds

---

## Key Insight

> _The system does not eliminate race conditions — it moves them to a safe stage where they do not affect data integrity or user experience._
