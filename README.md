# Documind

Documind is a document intelligence system built with .NET, designed to ingest, store, and query information from various documents using advanced AI capabilities. It leverages vector embeddings for efficient semantic search and integrates with large language models (LLMs) to provide conversational answers based on the retrieved context.

## Technical Stack & Architecture

- **Backend:** C# / .NET 10 (ASP.NET Core)
- **AI Integration:** Microsoft Semantic Kernel for orchestrating LLM interactions and embedding generation.
- **Vector Database:** Qdrant (or similar vector store) for storing document embeddings and performing semantic search.
- **Relational Database:** PostgreSQL for managing document metadata and application-specific data.
- **Docker Compose:** For easy setup and orchestration of services (PostgreSQL, Qdrant).

## Core Components:

- **Ingestion Service:** Handles the processing and embedding of documents, storing them in both the relational database and the vector store.
- **Search Service:** Performs semantic searches against the vector database to retrieve relevant document chunks based on a given query.
- **Ask Service:** Utilizes the retrieved context from the Search Service and an LLM (via Semantic Kernel) to generate natural language answers to user questions.
- **Endpoints:** ASP.NET Core Minimal APIs for interacting with the system (ingestion, search, ask).

## Setup and Execution

To get Documind up and running, follow these steps:

### Prerequisites

- .NET 8 SDK
- Docker Desktop (for PostgreSQL and Qdrant)
- A Google Gemini API Key

### Environment Variables

The application requires the following environment variables to be set:

- `Gemini:ApiKey`: Your Google Gemini API Key. This can be set in `appsettings.json`, `appsettings.Development.json`, or as an environment variable in your system.
- PostgreSQL Connection String: The connection string for PostgreSQL is configured via `ConnectionStrings:Postgres` in `appsettings.json` or `appsettings.Development.json`, or as an environment variable. The default Docker Compose setup expects:
    - `Host=localhost`
    - `Port=5432`
    - `Database=documind_db`
    - `Username=postgres`
    - `Password=secret1`

A typical connection string for development would look like: `Host=localhost;Port=5432;Database=documind_db;Username=postgres;Password=secret1`

### Running with Docker Compose

1.  **Start Docker services:**
    Navigate to the project root directory where `docker-compose.yaml` is located and run:
    ```bash
    docker compose up -d
    ```
    This will start the PostgreSQL and Qdrant containers.

### Running the .NET Application

1.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```
2.  **Run the application:**
    ```bash
    dotnet run
    ```
    The application will typically run on `http://localhost:5000` or `https://localhost:7000`. You can find the exact URLs in `Properties/launchSettings.json`.

### Seeding Knowledge (Optional)

After starting the application, you can seed initial knowledge into the system. This will ingest predefined facts about C# programming into your vector store.

1.  **Trigger the seed endpoint:**
    Send a POST request to `/seed`. For example, using `curl`:
    ```bash
    curl -X POST "http://localhost:5000/seed"
    ```
    Or visit the Swagger UI and execute the `/seed` endpoint.

### Checking DB

```shell
docker exec -it documind-db psql -U postgres -d documind_db -c "SELECT source, count(*) FROM documents GROUP BY source;"
```