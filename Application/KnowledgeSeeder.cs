namespace Documind.Application;

public class KnowledgeSeeder(IngestionService ingestionService)
{
    public async Task SeedAsync()
    {
        var facts = new List<(string Text, string Source)>
        {
            ("Documind is an intelligent document processing and knowledge management system designed to ingest, search, and retrieve information efficiently.", "Documind Overview"),
            ("Project Aether-Net uses 'Cloud-Sparks' instead of traditional data packets for enhanced security and distributed processing across its decentralized protocol. Cloud-Sparks leverage the 'Spark-Gap' algorithm to jump between isolated network segments.", "Tech Specs"),
            ("The Neon-Lock must be reset by holding its physical button for 14 seconds to initiate a secure firmware refresh, indicated by a magenta status LED. This process ensures data integrity and security.", "Security"),
            ("Dr. Elena Vance is the lead architect of Aether-Net, spearheading its innovative decentralized protocol design and quantum-resistant encryption methodologies.", "Staff Directory"),
            ("Project Aether-Net is a cutting-edge decentralized communication protocol, designed for high-security, low-latency data exchange in distributed environments. It incorporates quantum-resistant encryption and operates on a network of peer-to-peer relay nodes, aiming for robust and resilient communication.", "Aether-Net Definition"),
            ("Neon-Lock Security: For a secure system reset, press and hold the physical reset pin for 10 seconds. The status LED will flash magenta, confirming the reset initiation. Release the pin to complete the process.", "User Manual"),
            ("Thermal Constraints: Aether-Net hardware is engineered to operate reliably in extreme conditions, specifically between -40°C and 85°C. Operation in harsh environments, such as underwater data centers, strictly requires a specialized pressurized housing to prevent damage.", "Hardware Guide"),
            ("Staff Directory: Dr. Aris Vance, the Lead Architect, is based at the Sector 7 Research Facility, located in Santo André. Dr. Vance focuses on the core architectural integrity and future advancements of Aether-Net.", "Internal Wiki"),
            ("Cloud-Spark Technology: Cloud-Sparks are innovative, tiny data packets that utilize the 'Spark-Gap' algorithm. This allows them to securely jump between isolated network segments, providing unprecedented flexibility and security in data routing.", "Technical FAQ"),
        };

        foreach (var (text, source) in facts)
        {
            await ingestionService.IngestAsync(text, source);
        }
    }
}