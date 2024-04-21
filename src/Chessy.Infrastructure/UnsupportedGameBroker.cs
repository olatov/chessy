namespace Chessy.Infrastructure;

public sealed class UnsupportedGameBroker : IGameBroker
{
    public Dictionary<Guid, Func<Move, Task>> OnRemoveMakeMove { get => throw new PlatformNotSupportedException(); set => throw new NotImplementedException(); }
    public Dictionary<Guid, Action<string>> OnChatMessageSend { get => throw new PlatformNotSupportedException(); set => throw new NotImplementedException(); }

    public (Guid WhiteKey, Guid BlackKey) CreateNewGame()
    {
        throw new PlatformNotSupportedException();
    }

    public Game? GetGame(Guid key)
    {
        throw new PlatformNotSupportedException();
    }

    public void RemoteChatMessageSend(Guid key, string message)
    {
        throw new PlatformNotSupportedException();
    }

    public void RemoteMakeMove(Guid key, Move move)
    {
        throw new PlatformNotSupportedException();
    }
}
