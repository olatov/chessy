namespace Chessy.Infrastructure;


public interface IGameBroker
{
    public Dictionary<Guid, Func<Move, Task>> OnRemoveMakeMove { get; set; }

    public Dictionary<Guid, Action<string>> OnChatMessageSend { get; set; }

    public void RemoteMakeMove(Guid key, Move move);

    public void RemoteChatMessageSend(Guid key, string message);

    public (Guid WhiteKey, Guid BlackKey) CreateNewGame();

    public Game? GetGame(Guid key);
}