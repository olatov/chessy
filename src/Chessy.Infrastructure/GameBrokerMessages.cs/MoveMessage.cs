namespace Chessy.Infrastructure.GameBrokerMessages;

public sealed class MoveMessage : IGameBrokerMessage
{
    public Move Move { get; set; }
}
