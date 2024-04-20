namespace Chessy.Infractructure;

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
}

public sealed class GameBroker
{
    const int MaxItems = 5;

    private List<Game> _games = new();

    private Queue<GameRegistryItem> _registry = new();

    public Dictionary<Guid, Func<Move, Task>> OnRemoveMakeMove { get; set; } = new();

    public void RemoteMakeMove(Guid key, Move move)
    {
        var whiteItem = _registry.FirstOrDefault(x => x.WhiteKey == key);
        if (whiteItem is not null)
        {
            if (OnRemoveMakeMove.TryGetValue(whiteItem.BlackKey, out var handler))
            {
                handler.Invoke(move);
            }
            return;
        }

        var blackItem = _registry.FirstOrDefault(x => x.BlackKey == key);
        if (blackItem is not null)
        {
            if (OnRemoveMakeMove.TryGetValue(blackItem.WhiteKey, out var handler))
            {
                handler.Invoke(move);
            }
            return;
        }

        throw new InvalidOperationException("This is wrong");
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
            OnRemoveMakeMove.Remove(oldItem.WhiteKey);
            OnRemoveMakeMove.Remove(oldItem.BlackKey);
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
}