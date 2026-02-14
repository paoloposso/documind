using Documind.Domain;

namespace Documind.Application.Abstractions;

public interface ISearchService
{
    IAsyncEnumerable<DocumentRecord> SearchAsync(string userQuery);
}
