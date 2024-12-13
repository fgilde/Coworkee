using System;
using System.Collections.Generic;
using System.Linq;
using GptInvoke.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace Coworkee.Application.Common;

public class OllamaAIHandler : IAIHandler
{
    private readonly OllamaApiClient _ollama;

    public OllamaAIHandler(OllamaApiClient ollama)
    {
        _ollama = ollama;
    }

    public async Task<string> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        await foreach (var response in _ollama.ChatAsync(ChatRequestFor(request), cancellationToken))
        {
            sb.Append(response.Message.Content);
        }

        return sb.ToString();
    }

    private ChatRequest ChatRequestFor(AIRequest request)
    {
        return new ChatRequest
        {
            //Stream = true,
            Model = request.Model,
            Messages = CreateMessages(request.Messages)
        };
    }

    private IEnumerable<Message> CreateMessages(List<AIMessage> requestMessages)
    {
        return requestMessages.Select(message => new Message()
        {
            Content = message.Content,
            Role = message.Role
        });
    }

    public async Task StreamCompletionAsync(AIRequest request, Action<string> responseHandler, CancellationToken cancellationToken = default)
    {
        await foreach (var response in _ollama.ChatAsync(ChatRequestFor(request), cancellationToken))
        {
            responseHandler(response.Message.Content);
        }

    }
}
