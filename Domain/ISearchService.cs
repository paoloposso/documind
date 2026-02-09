using Documind.Domain;

namespace Documind.Domain;

public interface ISearchService
{
    Task<List<DocumentRecord>> SearchAsync(string userQuery);
}
