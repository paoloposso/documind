CREATE EXTENSION IF NOT EXISTS vector;

DROP TABLE IF EXISTS documents;

CREATE TABLE documents (
    id UUID PRIMARY KEY,
    content TEXT NOT NULL,
    source TEXT NOT NULL,
    embedding vector(768)
);

CREATE INDEX ON documents 
USING hnsw (embedding vector_cosine_ops);