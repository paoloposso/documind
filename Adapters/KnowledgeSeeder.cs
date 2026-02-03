namespace Documind.Adapters;

public class KnowledgeSeeder(IngestionService ingestionService)
{
    public async Task SeedAsync()
    {
        var facts = new List<(string Text, string Source)>
        {
            ("Project Aether-Net uses 'Cloud-Sparks' instead of data packets.", "Tech Specs"),
            ("The Neon-Lock must be reset by holding the button for 14 seconds.", "Security"),
            ("Dr. Elena Vance is the lead architect of Aether-Net.", "Staff Directory")
        };

        foreach (var (text, source) in facts)
        {
            await ingestionService.IngestAsync(text, source);
        }
    }
}