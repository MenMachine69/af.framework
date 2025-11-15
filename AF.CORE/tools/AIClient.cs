using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AF.CORE;

/// <summary>
/// Client um AI-Funktionen zu integrieren.
/// </summary>
public class AIClient
{
    private readonly Kernel? kernel;
    
    
    /// <summary>
    /// Einen AI-Client erzeugen
    /// </summary>
    /// <param name="model">zu verwendendes Model</param>
    /// <param name="endpoint">Endpoint URL</param>
    /// <param name="key">API Key</param>
    public AIClient(string model, string endpoint, string key)
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(model, endpoint, key);

        kernel = builder.Build();
    }

    /// <summary>
    /// Kernel der für den Client verwendet wird.
    /// </summary>
    public Kernel Kernel => kernel!;


    /// <summary>
    /// Eine Nachricht senden und auf die Antwort warten.
    /// </summary>
    /// <param name="message">Nachricht</param>
    /// <returns>Antwort</returns>
    public string Send(string message)
    {
        var result = kernel!.InvokePromptAsync(message).Result;
        return result.GetValue<string>() ?? "";
    }
    
}

/// <summary>
/// Ein AI Chat-Client
/// </summary>
public class AIChatClient
{
    private readonly ChatCompletionAgent agent;
    private readonly ChatHistory history;
    private readonly AgentThread agentThread;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="client">AIClient, der den CHat zur Verfügung stellt</param>
    /// <param name="name">Name des Chats</param>
    /// <param name="instructions">Anweisungen (Systemnachrichten)</param>
    public AIChatClient(AIClient client, string name, string instructions)
    {
        agent = new() { Name = name, Kernel = client.Kernel, Instructions = instructions };
        history = new();
        agentThread = new ChatHistoryAgentThread();
    }

    /// <summary>
    /// AssistentMessage hinzufügen
    /// </summary>
    /// <param name="msg"></param>
    public void AddAssistentMessage(string msg)
    {
        history.AddAssistantMessage(msg);
    }

    /// <summary>
    /// SystemMessage hinzufügen
    /// </summary>
    /// <param name="msg"></param>
    public void AddSystemMessage(string msg)
    {
        history.AddSystemMessage(msg);
    }

    /// <summary>
    /// DeveloperMessage hinzufügen
    /// </summary>
    /// <param name="msg"></param>
    public void AddDeveloperMessage(string msg)
    {
        history.AddDeveloperMessage(msg);
    }

    /// <summary>
    /// Setzt den Chat zurück und startet eine neue Konversation.
    /// </summary>
    public void StartNewConversation()
    {
        history.Clear();
    }

    /// <summary>
    /// Nachricht senden und Antwort empfangen.
    /// </summary>
    /// <param name="message">Nachricht</param>
    /// <returns>Antwort</returns>
    public async Task<string> Send(string message)
    {
        StringBuilder sb = StringBuilderPool.GetStringBuilder();

        history.AddUserMessage(message);

        await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(history, agentThread))
        {
            sb.Append(response.Message);
        }

        var ret = sb.ToString();
        StringBuilderPool.ReturnStringBuilder(sb);

        return ret;
    }
}

