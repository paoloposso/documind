namespace Documind.Domain;

public class DocumentRecord
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Source { get; set; } = "Manual Entry";
    public ReadOnlyMemory<float> Embedding { get; set; }
}