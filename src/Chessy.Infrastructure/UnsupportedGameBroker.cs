using Chessy.Infrastructure.GameBrokerMessages;

namespace Chessy.Infrastructure;

public sealed class UnsupportedGameBroker : IGameBroker
{
    public Dictionary<Guid, Func<Move, Task>> OnRemoveMakeMove { get => throw new PlatformNotSupportedException(); set => throw new NotImplementedException(); }
    public Dictionary<Guid, Action<string>> OnChatMessageSend { get => throw new PlatformNotSupportedException(); set => throw new NotImplementedException(); }
    public Dictionary<Guid, Action<IGameBrokerMessage>> OnGameBrokerMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    Dictionary<Guid, Func<IGameBrokerMessage, Task>> IGameBroker.OnGameBrokerMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public (Guid WhiteKey, Guid BlackKey) CreateNewGame()
    {
        throw new PlatformNotSupportedException();
    }

    public List<string> GetChatMessages(Guid key)
    {
        throw new NotImplementedException();
    }

    public Game? GetGame(Guid key)
    {
        throw new PlatformNotSupportedException();
    }

    public void ProcessMessage(Guid key, IGameBrokerMessage message)
    {
        throw new NotImplementedException();
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
