namespace Documind.Application.Abstractions;

public interface IAskService
{
    Task<string> Ask(string question);
}
