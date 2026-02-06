namespace Documind.Domain;

public interface IAskService
{
    Task<string> Ask(string question);
}
