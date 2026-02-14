using Documind.Domain;
using System.Threading.Tasks;

namespace Documind.Application.Abstractions;

public interface IIngestionService
{
    Task IngestAsync(string text, string source);
}
