using Chessy.Infrastructure.GameBrokerMessages;

namespace Chessy.Infrastructure;


public interface IGameBroker
{
    public Dictionary<Guid, Func<IGameBrokerMessage, Task>> OnGameBrokerMessage { get; set; }

    public void ProcessMessage(Guid key, IGameBrokerMessage message);

    public (Guid WhiteKey, Guid BlackKey) CreateNewGame();

    public Game? GetGame(Guid key);

    public List<string> GetChatMessages(Guid key);
}