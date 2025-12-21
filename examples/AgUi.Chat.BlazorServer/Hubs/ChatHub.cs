using Microsoft.AspNetCore.SignalR;

namespace AgUi.Chat.BlazorServer.Hubs;

/// <summary>
/// SignalR hub for real-time chat messaging.
/// </summary>
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Send a message to the chat. Currently echoes back as a demo.
    /// In production, this would integrate with the OpenCode backend.
    /// </summary>
    public async Task SendMessage(string user, string message)
    {
        _logger.LogInformation("Message received from {User}: {Message}", user, message);

        // For demo purposes, simulate a streaming AI response
        // In production, this would call the OpenCode backend
        await SimulateStreamingResponse(message);
    }

    /// <summary>
    /// Join a chat room (for future multi-session support).
    /// </summary>
    public async Task JoinChat(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogInformation("Client {ConnectionId} joined session {SessionId}", Context.ConnectionId, sessionId);
    }

    /// <summary>
    /// Leave a chat room.
    /// </summary>
    public async Task LeaveChat(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogInformation("Client {ConnectionId} left session {SessionId}", Context.ConnectionId, sessionId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}, Exception: {Exception}",
            Context.ConnectionId, exception?.Message ?? "None");
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Simulates a streaming response for demo purposes.
    /// Replace with actual OpenCode backend integration.
    /// </summary>
    private async Task SimulateStreamingResponse(string userMessage)
    {
        // Simulate thinking delay
        await Task.Delay(500);

        // Generate a demo response
        var response = GenerateDemoResponse(userMessage);

        // Stream the response word by word
        var words = response.Split(' ');
        foreach (var word in words)
        {
            await Clients.Caller.SendAsync("ReceiveStreamingChunk", word + " ");
            await Task.Delay(50); // Simulate typing delay
        }

        // Signal streaming complete
        await Clients.Caller.SendAsync("StreamingComplete");
    }

    private static string GenerateDemoResponse(string userMessage)
    {
        // Simple demo responses based on keywords
        var lowerMessage = userMessage.ToLower();

        if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi"))
        {
            return "Hello! I'm a demo assistant in the OpenCode Chat Blazor Server sample. I can demonstrate streaming responses and real-time updates via SignalR. How can I help you today?";
        }

        if (lowerMessage.Contains("help"))
        {
            return "This is a demo chat application showcasing:\n\n1. Blazor Server with interactive components\n2. MudBlazor UI framework with OpenCode theming\n3. SignalR for real-time communication\n4. Streaming responses\n\nIn a production environment, this would be connected to the OpenCode AI backend for actual AI-powered conversations.";
        }

        if (lowerMessage.Contains("code") || lowerMessage.Contains("example"))
        {
            return "Here's an example of how streaming works:\n\n```csharp\nawait foreach (var chunk in GetStreamingResponse())\n{\n    await Clients.Caller.SendAsync(\"ReceiveStreamingChunk\", chunk);\n}\n```\n\nThe response is streamed word by word to demonstrate real-time updates.";
        }

        return $"Thank you for your message: \"{userMessage}\"\n\nThis is a demo response from the OpenCode Chat sample. The actual AI integration would process your request through the OpenCode backend and stream back intelligent responses.";
    }
}
