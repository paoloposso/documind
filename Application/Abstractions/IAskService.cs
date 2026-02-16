namespace Documind.Application.Abstractions;

public interface IAskService
{
    IAsyncEnumerable<string> AskStreamingAsync(string question, CancellationToken ct = default);
}
