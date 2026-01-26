CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE IF NOT EXISTS documents (
    id UUID PRIMARY KEY,
    content TEXT NOT NULL,
    source TEXT,
    embedding vector(768)
);

CREATE INDEX ON documents 
USING hnsw (embedding vector_cosine_ops);