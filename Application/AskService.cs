using Documind.Application.Abstractions;
using Documind.Domain;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Documind.Application;

public class AskService(ISearchService searchService, IChatCompletionService chatCompletionService) : IAskService
{
    private readonly ISearchService _searchService = searchService;
    private readonly IChatCompletionService _chatCompletionService = chatCompletionService;

    public async Task<string> AskAsync(string question)
    {
        List<DocumentRecord> relevantDocuments = [];

        await foreach (var document in _searchService.SearchAsync(question))
        {
            relevantDocuments.Add(document);
        }

        if (relevantDocuments.Count == 0)
        {
            return "I couldn't find any relevant information to answer your question.";
        }

        // 2. Prepare Context for the LLM
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("You are a helpful assistant. Answer the user's question truthfully and concisely, using ONLY the provided context. If the answer cannot be found in the context, respond with 'I cannot answer this question based on the provided information.'. Do not make up answers.");

        // Add document content as context
        var context = string.Join("\n\n---\n\n", relevantDocuments.Select(d => $"Content from source '{d.Source}':\n{d.Content}"));
        chatHistory.AddUserMessage($"Context:\n{context}\n\nQuestion: {question}");

        // 3. Generate Answer using LLM

        var result = await _chatCompletionService.GetChatMessageContentAsync(chatHistory);

        return result.Content ?? "I could not generate an answer based on the provided information.";
    }
}
