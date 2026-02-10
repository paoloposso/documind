using Documind.Domain;

namespace Documind.Application.Abstractions;

public interface ISearchService
{
    Task<List<DocumentRecord>> SearchAsync(string userQuery);
}
