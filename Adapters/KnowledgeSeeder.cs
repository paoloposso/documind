namespace Documind.Adapters;

public class KnowledgeSeeder(IngestionService ingestionService)
{
    public async Task SeedAsync()
    {
        var facts = new List<(string Text, string Source)>
        {
            ("Project Aether-Net uses 'Cloud-Sparks' instead of data packets.", "Tech Specs"),
            ("The Neon-Lock must be reset by holding the button for 14 seconds.", "Security"),
            ("Dr. Elena Vance is the lead architect of Aether-Net.", "Staff Directory"),
            ("Project Aether-Net Specification: Aether-Net is a decentralized protocol designed for 2026, using quantum-resistant encryption and peer-to-peer relay nodes.", "Spec-v1.0"),
            ("Neon-Lock Security: To reset the Neon-Lock, press the physical reset pin for 10 seconds until the status LED flashes magenta.", "User Manual"),
            ("Thermal Constraints: The Aether-Net hardware is rated for temperatures between -40°C and 85°C. It is strictly forbidden to operate in underwater data centers without a pressurized housing.", "Hardware Guide"),
            ("Staff Directory: Dr. Aris Vance is the Lead Architect based at the Sector 7 Research Facility in Santo André.", "Internal Wiki"),
            ("Cloud-Spark Technology: Cloud-Sparks are tiny data packets that use the 'Spark-Gap' algorithm to jump between isolated networks.", "Technical FAQ"),
        };

        foreach (var (text, source) in facts)
        {
            await ingestionService.IngestAsync(text, source);
        }
    }
}