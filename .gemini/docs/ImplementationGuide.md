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

The process of adding new knowledge to the system involves generating an embedding for the content and storing it in the document repository.

**Steps:**

1.  **Receive Content:** The `IngestionService` receives raw `text` and its `source` (e.g., from an endpoint).
2.  **Generate Embedding:**
    *   The `IngestionService` uses an `IEmbeddingGenerator<string, Embedding<float>>` (e.g., provided by `Microsoft.Extensions.AI`) to convert the text content into a numerical vector (embedding).
    *   It specifies `Dimensions` (e.g., 768) and `task_type` (`RETRIEVAL_DOCUMENT`) to guide the embedding model.
    *   A manual slicing step (`if (vector.Length > 768) { vector = vector[..768]; }`) ensures the embedding vector matches the expected dimension for the underlying vector store (e.g., PostgreSQL with `pgvector`).
3.  **Create Document Record:** A `DocumentRecord` object is created, encapsulating the `Id`, `Content`, generated `Embedding`, and `Source`.
4.  **Store Document:** The `IngestionService` then uses an `IDocumentRepository` (e.g., `VectorStoreDocumentRepository`) to persist the `DocumentRecord` into the chosen data store (e.g., PostgreSQL with `pgvector`).

**Components Involved:**

*   `IngestionService` (Application Layer)
*   `IIngestionService` (Application/Abstractions - *to be created*)
*   `IEmbeddingGenerator` (from `Microsoft.Extensions.AI`)
*   `IDocumentRepository` (Domain Layer)
*   `VectorStoreDocumentRepository` (Infrastructure Layer)
*   `DocumentRecord` (Domain Layer)
*   `IngestionEndpoints` (Endpoints Layer)

### 3.2. Question Answering

*(To be elaborated later)*

### 3.3. Knowledge Seeding

*(To be elaborated later)*

## 4. Tooling and Libraries

*   **ASP.NET Core Minimal APIs:** Used for building lightweight web APIs with minimal boilerplate.
*   **Semantic Kernel (`Microsoft.SemanticKernel`):** An SDK for integrating Large Language Models (LLMs) with traditional programming languages. Used for orchestrating AI interactions, chat completion, and potentially planning.
*   **Microsoft.Extensions.AI:** Provides abstractions for AI components, including `IEmbeddingGenerator`.
*   **`pgvector` for PostgreSQL:** A PostgreSQL extension that enables efficient storage and similarity search of vector embeddings.
