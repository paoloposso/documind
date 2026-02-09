using Documind.Domain;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Documind.Application;

public class AskService : IAskService
{
    private readonly ISearchService _searchService;
    private readonly IDocumentRepository _documentRepository; // May not be directly used here if SearchService handles all retrieval
    private readonly Kernel _kernel; // Inject Semantic Kernel

    public AskService(ISearchService searchService, IDocumentRepository documentRepository, Kernel kernel)
    {
        _searchService = searchService;
        _documentRepository = documentRepository;
        _kernel = kernel;
    }

    public async Task<string> Ask(string question)
    {
        // 1. Retrieve Relevant Information using SearchService
        var relevantDocuments = await _searchService.SearchAsync(question);

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
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory);

        return result.Content ?? "I could not generate an answer based on the provided information.";
    }
}
