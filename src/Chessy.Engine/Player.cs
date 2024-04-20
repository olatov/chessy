namespace Chessy.Engine;

public sealed class Player
{
    public string Name { get; set; } = "Player";

    public PlayerType Type { get; set; } = PlayerType.Human;

    public int? ComputerLevel { get; set; } = 5;

    public bool IsHuman => Type == PlayerType.Human;

    public bool IsComputer => Type == PlayerType.Computer;

    public bool IsRemoteGuest => Type == PlayerType.RemoteGuest;
}
