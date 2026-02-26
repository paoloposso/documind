namespace Documind.Domain;

public interface IDocumentRepository
{
    Task AddAsync(DocumentRecord record, CancellationToken ct = default);
    IAsyncEnumerable<DocumentRecord> SearchAsync(ReadOnlyMemory<float> embedding, int limit);
}
