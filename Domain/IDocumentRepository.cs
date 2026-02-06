namespace Documind.Domain;

public interface IDocumentRepository
{
    Task AddAsync(DocumentRecord record);
    IAsyncEnumerable<DocumentRecord> SearchAsync(ReadOnlyMemory<float> embedding, int limit);
}
