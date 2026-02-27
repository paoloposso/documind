# Documind API Documentation

This document outlines the available API endpoints for the Documind application, including their methods, paths, and expected parameters.

---

## 1. Ingestion Endpoints

### 1.1 Ingest Text

Ingests new text content into the knowledge base.

*   **Method:** `POST`
*   **Path:** `/api/ingest`
*   **Description:** Accepts a plain text body for ingestion and a source identifier.
*   **Parameters:**
    *   `text` (string): The plain text content to be ingested. (Provided in the request body)
    *   `source` (string): An identifier for the origin of the text. (Provided as an HTTP header)
*   **Example Usage (using .http client):**
    ```http
    POST {{Documind_HostAddress}}/api/ingest
    Content-Type: text/plain
    source: sample-source

    This is a sample document to be ingested. It can be a long text.
    ```

---

## 2. File Ingestion Endpoints

### 2.1 Ingest File

Asynchronously ingests a file (PDF, Word, etc.) into the knowledge base.

*   **Method:** `POST`
*   **Path:** `/api/ingest-file`
*   **Description:** Uploads a file for background processing. Returns a `JobId` that can be used to track progress.
*   **Parameters:**
    *   `file` (IFormFile): The file to upload (e.g., PDF, DOCX, TXT).
*   **Response:** `202 Accepted` with a `Guid` (JobId).
*   **Example Usage:**
    ```http
    POST {{Documind_HostAddress}}/api/ingest-file
    Content-Type: multipart/form-data; boundary=boundary

    --boundary
    Content-Disposition: form-data; name="file"; filename="sample.pdf"
    Content-Type: application/pdf

    <binary content here>
    --boundary--
    ```

### 2.2 Get Ingestion Status

Retrieves the current status of a background ingestion job.

*   **Method:** `GET`
*   **Path:** `/api/ingest-file/status/{jobId}`
*   **Description:** Returns the status of the job from Redis. Statuses are stored for 24 hours.
*   **Parameters:**
    *   `jobId` (Guid): The identifier returned by the Ingest File endpoint.
*   **Response:** `200 Ok` with a `JobStatusResponse` object.
    *   `Status`: `Pending`, `Processing`, `Completed`, or `Failed`.
*   **Example Usage:**
    ```http
    GET {{Documind_HostAddress}}/api/ingest-file/status/{{jobId}}
    ```

---

## 3. Search Endpoints

### 3.1 Search

Searches the knowledge base for documents relevant to a given query.

*   **Method:** `GET`
*   **Path:** `/api/search`
*   **Description:** Returns a list of `DocumentRecord` objects that match the query.
*   **Parameters:**
    *   `query` (string): The search query string. (Provided as a query parameter)
*   **Example Usage (using .http client):**
    ```http
    GET {{Documind_HostAddress}}/api/search?query=sample%20query
    ```

---

## 4. Seed Endpoints

### 4.1 Seed Data

Triggers the seeding of initial knowledge into the system.

*   **Method:** `POST`
*   **Path:** `/api/seed`
*   **Description:** Initiates a process to populate the knowledge base with predefined data.
*   **Parameters:** None required in the request.
*   **Example Usage (using .http client):**
    ```http
    POST {{Documind_HostAddress}}/api/seed
    Content-Type: application/json
    ```

---

## 5. Ask Endpoints

### 5.1 Ask

Asks a question to the knowledge base and receives an answer.

*   **Method:** `GET`
*   **Path:** `/api/ask`
*   **Description:** Processes a natural language question against the knowledge base to provide a relevant answer.
*   **Parameters:**
    *   `question` (string): The question to ask. (Provided as a query parameter)
*   **Example Usage (using .http client):**
    ```http
    GET {{Documind_HostAddress}}/api/ask?question=What%20is%20Documind%3F
    ```
