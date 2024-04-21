namespace Chessy.Infrastructure.GameBrokerMessages;

public sealed class ChatMessage : IGameBrokerMessage
{
    public string Message { get; set; }
}
