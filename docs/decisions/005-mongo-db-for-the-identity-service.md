# Architecture Decision Record (ADR): MongoDB (Non-Relational DBMS) for the Identity Service

## Date

2026-05-21

## Context

### Login Traffic Analysis

The **Identity Service** is a low-traffic component relative to the rest of the cluster. The use of **JWTs** drastically shifts runtime read-demands away from the identity database:

- **Single-Read Login:** A login triggers exactly one read, fetching the user document by email.
- **Token Self-Sufficiency:** Once a signed JWT is issued, the user is authenticated across all subsequent operations. The identity database is completely bypassed as all required claims are already encrypted inside the token.

---

## Decision

Use **MongoDB** as the database for the Identity Service.

---

## Alternatives Considered

### SQL Server (Relational)

| Metric               | MongoDB-Based                                                | SQL Server                                                                        |
| -------------------- | ------------------------------------------------------------ | --------------------------------------------------------------------------------- |
| **Query Complexity** | Zero joins. Entire user claims are fetched in one operation. | JOIN queries needed to fetch claims for the user.                                 |
| **Schema Evolution** | New fields added freely without migrations.                  | Every new field requires a schema migration, and backward compatability problems. |

---

## Consequences

- **Minimum Overhead at Login:** No joins overhead when retrieving user's data.
- **Schema Flexibility:** User documents can evolve freely (new claims and roles) without migrations.
- **Tradeoff:** No database-level referential integrity, but this is cceptable in this case as user data is self-contained with no relational dependencies.

---

## Key Insight

> The Identity Service does not need a relational model — it needs a **flexible, high-speed identity store**. MongoDB's document model ensures fast retrieving without joins.
