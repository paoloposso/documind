namespace Documind.Application.Abstractions;

public interface IAskService
{
    Task<string> AskAsync(string question);
}
