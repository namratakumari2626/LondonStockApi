# London Stock API – Solution Overview

## Introduction

This repository contains a simple implementation of a stock trading API designed as a **Minimum Viable Product (MVP)**.  
The solution ingests trade events, stores them in a relational database, and exposes aggregated stock prices through RESTful endpoints.

The implementation prioritises:
- Correctness of financial calculations
- Transactional consistency
- Clear separation of concerns
- Extensibility for future enhancements

---

## Solution Summary

The system accepts trade notifications from brokers and maintains an aggregated view of stock prices.  
Each trade updates the corresponding stock’s total traded volume and average price, which can then be queried via the API.

Stock prices are updated incrementally to avoid recalculating values from all historical trades.

---

## Technology Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI

---

## Scalability & Enhancements

The current implementation is suitable for low to moderate traffic and prioritises correctness and simplicity.  
To support higher throughput, lower latency, and increased reliability, the following enhancements can be applied.

---

### 1. CQRS (Command Query Responsibility Segregation)

A CQRS approach separates **write operations** (trade ingestion) from **read operations** (stock price queries).

**Write Side**
- Optimised for high-throughput trade ingestion
- Responsible for persisting trades and updating aggregates

**Read Side**
- Optimised for fast queries
- Uses pre-aggregated stock prices
- Can be backed by a separate read model or database

**Benefits**
- Independent scaling of read and write workloads
- Reduced contention on shared resources
- Clear separation of responsibilities

---

### 2. Response Compression

Response compression can be enabled using middleware (e.g. Gzip or Brotli).

- Reduces payload size for stock list responses
- Improves network efficiency and client latency
- Particularly beneficial for large result sets

**Benefits**
- Faster response times
- Reduced network bandwidth usage
- Improved client performance

---

### 3. Event-Driven Architecture

Introduce a message broker (e.g. Kafka, Azure Service Bus, RabbitMQ) to decouple trade ingestion from processing.

- Trades are published as events
- Consumers process trades asynchronously
- Stock aggregation becomes event-driven

**Benefits**
- Higher ingestion throughput
- Improved fault tolerance
- Natural support for downstream consumers (analytics, reporting, auditing)

---

### 4. Caching Strategy

Cache stock price queries to reduce database load.

- Apply **read-through caching** for GET stock endpoints
- Use **cache invalidation or update** when new trades are processed
- Redis is a suitable choice for distributed caching

**Benefits**
- Lower read latency
- Reduced database pressure
- Improved scalability for read-heavy workloads

---

### 5. Database Scaling & Load Balancing

As traffic grows, the database layer can be scaled by:

- Using read replicas for read queries
- Partitioning large trade tables (e.g. by ticker or time range)
- Tuning connection pooling
- Placing API instances behind a load balancer

**Benefits**
- Horizontal scalability
- Improved availability and fault tolerance

---

### 6. EF Core Query Optimisation

#### No-Tracking Queries
- All read-only queries use `AsNoTracking()`
- Disables change tracking for entities that are not updated

**Benefits**
- Reduced memory usage
- Faster query execution
- Improved scalability for read-heavy workloads

---

#### Projection Instead of Entity Materialisation
- Queries project directly into DTOs using `Select`
- Avoids loading full entity graphs when only a subset of fields is required

**Benefits**
- Smaller memory footprint
- Reduced data transfer from the database
- Faster response times

---

### 7. Authentication & Authorization

JWT-based authentication can be added to secure the API.

- Validate JWTs using authentication middleware
- Apply authorization filters or attributes at controller/action level
- Restrict trade ingestion to authorised brokers

**Benefits**
- Centralised security enforcement
- Clear separation between authentication and business logic

---

### 8. EF Core Code-First Approach

The solution uses a **code-first** approach with Entity Framework Core.

- Enables version-controlled schema changes via migrations
- Keeps domain model and database schema in sync
- Supports incremental evolution of the data model

**Benefits**
- Controlled schema evolution
- Improved maintainability
- Easier collaboration across teams

---

### 9. Database Indexing

Indexes can be added to improve query performance.

- Unique index on stock ticker symbols
- Indexes on foreign keys and frequently filtered columns

**Benefits**
- Faster lookups
- Reduced query latency
- Better scalability under load

---

### 10. Testing Strategy

Automated tests improve reliability and maintainability.

- **Unit tests**
  - Validate calculation logic (average price, volume updates)
  - Validate input normalization and edge cases

**Benefits**
- Early detection of defects
- Safer refactoring
- Improved code quality

---

### 11. Rate Limiting

Apply rate limiting at the API or gateway level to control request volume.

**Benefits**
- Protects the system from abuse and traffic spikes
- Improves availability
- Prevents resource exhaustion

---

### 12. Idempotency Keys

Use idempotency keys for trade creation requests to ensure duplicate requests do not create duplicate trades.

**Benefits**
- Prevents duplicate data
- Improves reliability during retries or network failures
- Critical for financial transaction safety

---

### 13. Structured Logging and Metrics

Capture structured logs and key metrics such as request latency, error rates, and throughput.

**Benefits**
- Improved observability
- Faster debugging and incident response
- Effective monitoring and alerting

---

### 14. Health Checks

Implement readiness and liveness health checks to validate database connectivity and application state.

**Benefits**
- Enables safe deployments
- Supports automated scaling
- Early detection of system issues

---

### Summary

These enhancements provide a clear path for evolving the solution from a simple MVP into a scalable, resilient, and production-ready architecture while preserving the original functional goals.


