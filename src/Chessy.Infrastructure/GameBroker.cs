using Chessy.Infrastructure.GameBrokerMessages;

namespace Chessy.Infrastructure;

internal sealed class GameRegistryItem
{
    public Guid WhiteKey { get; set; } = Guid.NewGuid();

    public Guid BlackKey { get; set; } = Guid.NewGuid();

    public Game WhiteGame { get; set; } = new()
    {
        WhitePlayer = new() { Type = PlayerType.Human },
        BlackPlayer = new() { Type = PlayerType.RemoteGuest },
    };

    public Game BlackGame { get; set; } = new()
    {
        WhitePlayer = new() { Type = PlayerType.RemoteGuest },
        BlackPlayer = new() { Type = PlayerType.Human },
    };

    public List<string> ChatMessages { get; private set; } = new List<string>();
}

public sealed class GameBroker : IGameBroker
{
    const int MaxItems = 5;

    private readonly Queue<GameRegistryItem> _registry = new();

    public Dictionary<Guid, Func<IGameBrokerMessage, Task>> OnGameBrokerMessage { get; set; } = new();

    public void ProcessMessage(Guid key, IGameBrokerMessage message)
    {
        if (message is MoveMessage moveMessage)
        {
            ProcessMoveMessage(key, moveMessage);
            return;
        }

        if (message is ChatMessage chatMessage)
        {
            ProcessChatMessage(key, chatMessage);
            return;
        }

        throw new ArgumentException($"Invalid message - {message}");
    }

    private void ProcessMoveMessage(Guid key, MoveMessage message)
    {
        var whiteItem = _registry.FirstOrDefault(x => x.WhiteKey == key);
        if (whiteItem is not null)
        {
            if (OnGameBrokerMessage.TryGetValue(whiteItem.BlackKey, out var handler))
            {
                handler.Invoke(message);
            }
            return;
        }

        var blackItem = _registry.FirstOrDefault(x => x.BlackKey == key);
        if (blackItem is not null)
        {
            if (OnGameBrokerMessage.TryGetValue(blackItem.WhiteKey, out var handler))
            {
                handler.Invoke(message);
            }
            return;
        }

        throw new InvalidOperationException("This is wrong");
    }

    private void ProcessChatMessage(Guid key, ChatMessage message)
    {
        var item = _registry.FirstOrDefault(x => x.WhiteKey == key || x.BlackKey == key);
        if (item is not null)
        {
            item.ChatMessages.Add(message.Message);
            if (OnGameBrokerMessage.TryGetValue(item.WhiteKey, out var handler))
            {
                handler.Invoke(message);
            }

            if (OnGameBrokerMessage.TryGetValue(item.BlackKey, out handler))
            {
                handler.Invoke(message);
            }
        }
    }

    public (Guid WhiteKey, Guid BlackKey) CreateNewGame()
    {
        var item = new GameRegistryItem();
        item.WhiteGame.ResetToStartingPosition();
        item.BlackGame.ResetToStartingPosition();

        _registry.Enqueue(item);

        while (_registry.Count > MaxItems)
        {
            var oldItem = _registry.Dequeue();
            OnGameBrokerMessage.Remove(oldItem.WhiteKey);
        }

        return (item.WhiteKey, item.BlackKey);
    }

    public Game? GetGame(Guid key)
    {
        var whiteItem = _registry.FirstOrDefault(x => x.WhiteKey == key);
        if (whiteItem is not null)
        {
            return whiteItem.WhiteGame;
        }

        var blackItem = _registry.FirstOrDefault(x => x.BlackKey == key);
        if (blackItem is not null)
        {
            return blackItem.BlackGame;
        }

        return null;
    }

    public List<string> GetChatMessages(Guid key)
    {
        var item = _registry.FirstOrDefault(x => x.WhiteKey == key || x.BlackKey == key);
        return item?.ChatMessages ?? new List<string>();
    }
}