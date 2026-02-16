using System.Runtime.CompilerServices;
using Documind.Domain;

namespace Documind.Application.Abstractions;

public interface ISearchService
{
    IAsyncEnumerable<DocumentRecord> SearchAsync(string userQuery, CancellationToken ct = default);
}
