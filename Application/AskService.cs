using System.Runtime.CompilerServices;
using Documind.Application.Abstractions;
using Documind.Domain;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Documind.Application;

public class AskService(ISearchService searchService, IChatCompletionService chatCompletionService) : IAskService
{
    private readonly ISearchService _searchService = searchService;
    private readonly IChatCompletionService _chatCompletionService = chatCompletionService;

    public async IAsyncEnumerable<string> AskStreamingAsync(
        string question,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        List<DocumentRecord> relevantDocuments = [];

        await foreach (var document in _searchService.SearchAsync(question, ct))
        {
            relevantDocuments.Add(document);
        }

        if (relevantDocuments.Count == 0)
        {
            yield return "I couldn't find any relevant information to answer your question.";
        }

        // Prepare Context for the LLM
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("You are a helpful assistant. Answer the user's question truthfully and concisely, using ONLY the provided context. If the answer cannot be found in the context, respond with 'I cannot answer this question based on the provided information.'. Do not make up answers.");

        // Add document content as context
        var context = string.Join("\n\n---\n\n", relevantDocuments.Select(d => $"Content from source '{d.Source}':\n{d.Content}"));
        chatHistory.AddUserMessage($"Context:\n{context}\n\nQuestion: {question}");

        var streamingResults = _chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, cancellationToken: ct);

        await foreach (var chunk in streamingResults.WithCancellation(ct))
        {
            if (chunk.Content is not null)
            {
                yield return chunk.Content;
            }
        }
    }
}
