# Implementation Guide

This document outlines key aspects of the Documind project to assist with understanding and implementing new features.

## 1. Project Architecture Overview

*   **Endpoints Layer:** Handles incoming HTTP requests and routes them to the appropriate application services. Uses Minimal APIs.
*   **Application Layer:** Contains the business logic and orchestrates operations. Services here depend on domain interfaces and infrastructure implementations.
*   **Domain Layer:** Defines the core business entities, value objects, and interfaces that represent the business concepts. It is the heart of the application.
*   **Infrastructure Layer:** Provides the concrete implementations for domain interfaces, such as data access (e.g., `VectorStoreDocumentRepository`).

**Data Flow Diagram (Conceptual):**

```
User Request -> Endpoints -> Application Service -> Domain Interface -> Infrastructure Implementation -> External Service/DB
```

## 2. Domain Model Explanation

*   **`DocumentRecord`:** Represents a piece of knowledge stored in the system. It typically includes:
    *   `Id`: Unique identifier.
    *   `Content`: The raw text of the document.
    *   `Embedding`: A vector representation of the content, used for semantic search.
    *   `Source`: Origin of the document (e.g., file path, URL).

*   **`Embedding`:** A numerical vector (array of floats) that captures the semantic meaning of text. Documents with similar meanings have embeddings that are close to each other in a multi-dimensional space.

## 3. Key Workflows

### 3.1. File Ingestion

The process of adding knowledge from files is handled asynchronously to ensure a responsive user experience and efficient processing.

**Overview:**
Documind uses a **Background Service** architecture combined with **Redis** for state tracking. This prevents long-running ingestion tasks (like parsing a large book) from blocking the API.

**Architecture Components:**
*   **`FileIngestionService`:** Orchestrates the ingestion. It saves uploaded files to a temporary location, generates a `JobId`, and queues an `IngestionJob`.
*   **`IngestionJobQueue`:** A `System.Threading.Channels` based queue that manages the flow of work between the API and the background worker.
*   **`IngestionBackgroundService`:** A hosted worker that continuously dequeues jobs and processes them sequentially.
*   **`RedisJobStatusService`:** Stores the current status of ingestion jobs (Pending, Processing, Completed, Failed). Redis is used for its **TTL (Time-To-Live)** feature, ensuring that ephemeral job statuses are automatically cleaned up after 24 hours.

**Processing Steps:**
1.  **Queue Job:** The user uploads a file. The API returns a `202 Accepted` with a `JobId` immediately.
2.  **Extract & Chunk:** The background worker uses **Apache Tika** to extract text and a streaming `IEnumerable` chunker to break the text into manageable pieces (e.g., 512 characters).
3.  **Batch Embedding:** Chunks are processed in **batches** (e.g., 10 at a time) to minimize network overhead when calling the AI Embedding API.
4.  **Store:** Generated vector embeddings and raw content are stored in PostgreSQL using `pgvector`.
5.  **Clean up:** The temporary file is deleted, and the job status in Redis is updated to `Completed`.

**Components Involved:**
*   `FileIngestionService` (Application Layer)
*   `IngestionBackgroundService` (Background Worker)
*   `IngestionJobQueue` (Messaging)
*   `RedisJobStatusService` (Job Tracking)
*   `IIngestionService` (Batch Processing logic)
*   `IDocumentRepository` (Domain Layer)
*   `VectorStoreDocumentRepository` (Infrastructure Layer)
*   `FileIngestionEndpoints` (Endpoints Layer)

### 3.2. Batch Ingestion Performance

To optimize throughput, `IIngestionService.IngestBatchAsync` is used. This allows the application to send multiple text chunks in a single request to the embedding generator, which is significantly faster than sequential single-chunk requests.

### 3.3. Question Answering

*(To be elaborated later)*

### 3.4. Knowledge Seeding

*(To be elaborated later)*

## 4. Tooling and Libraries

*   **ASP.NET Core Minimal APIs:** Used for building lightweight web APIs with minimal boilerplate.
*   **Redis:** Used for tracking ephemeral ingestion job states with automatic 24-hour expiration.
*   **System.Threading.Channels:** Provides an efficient, thread-safe producer-consumer queue for background work.
*   **High-Performance Logging:** Uses `.NET Source Generators` (`[LoggerMessage]`) to minimize allocations and maximize performance in high-throughput services.
*   **Semantic Kernel (`Microsoft.SemanticKernel`):** An SDK for integrating Large Language Models (LLMs) with traditional programming languages. Used for orchestrating AI interactions, chat completion, and potentially planning.
*   **Microsoft.Extensions.AI:** Provides abstractions for AI components, including `IEmbeddingGenerator`.
*   **`pgvector` for PostgreSQL:** A PostgreSQL extension that enables efficient storage and similarity search of vector embeddings.

