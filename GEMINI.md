# Gemini LLM Instructions

# Project: DocuMind (RAG System)

## Technology Stack
- **Framework:** .NET 10 (Targeting Native AOT)
- **AI Integration:** Semantic Kernel + Microsoft.Extensions.AI
- **Database:** PostgreSQL with pgvector
- **Cloud:** Google Gemini (1.5 Flash for Chat, text-embedding-004 for Embeddings)

## Coding Guidelines
- **Native AOT:** Never use Reflection. Always use Source Generators and `JsonSerializerContext`.
- **API Style:** Use Minimal APIs with `TypedResults` and `Results<T1, T2>` union types for status codes.
- **Naming:** Use File-scoped namespaces. Use `ReadOnlyMemory<float>` for vector embeddings.
- **Architecture:** Follow "Ports and Adapters." Services should be registered as `Scoped` in `Program.cs`.

## RAG Specifics
- Always use **Cosine Similarity** for vector comparisons.
- When generating answers, always cite the source content from the `DocumentRecord`.
- Our embedding dimension is strictly **768**.
