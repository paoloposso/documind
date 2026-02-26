using Documind.Application.Abstractions;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;

namespace Documind.Application
{
    public partial class FileIngestionService(IIngestionService ingestionService) : IFileIngestionService
    {
        private readonly IIngestionService _ingestionService = ingestionService;
        private const int MaxChunkSize = 512;
        private const int Overlap = 128;

        public async Task IngestFileAsync(IFormFile file, CancellationToken ct = default)
        {
            await using var stream = file.OpenReadStream();
            var text = await ExtractTextAsync(stream);

            var chunks = ChunkText(text, MaxChunkSize, Overlap);

            for (var i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                var source = $"{file.FileName}#chunk-{i + 1}";
                await _ingestionService.IngestAsync(chunk, source, ct);
            }
        }

        private static async Task<string> ExtractTextAsync(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            var textExtractor = new TextExtractor();
            var result = await Task.Run(() => textExtractor.Extract(bytes));
            return result.Text;
        }

        private static List<string> ChunkText(string text, int chunkSize, int overlap)
        {
            var chunks = new List<string>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            var sentences = MyRegex().Split(text).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            var currentChunk = "";
            for (int i = 0; i < sentences.Count; i++)
            {
                var sentence = sentences[i];
                if (currentChunk.Length + sentence.Length > chunkSize && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk);

                    var words = currentChunk.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var overlapWords = words.Skip(Math.Max(0, words.Length - overlap));
                    currentChunk = string.Join(" ", overlapWords) + " ";
                }

                currentChunk += sentence + " ";
            }

            if (!string.IsNullOrWhiteSpace(currentChunk))
            {
                chunks.Add(currentChunk.Trim());
            }

            return chunks;
        }

        [GeneratedRegex(@"(?<=[\.!\?])\s+")]
        private static partial Regex MyRegex();
    }
}
