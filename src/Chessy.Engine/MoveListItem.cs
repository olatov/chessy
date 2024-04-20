namespace Chessy.Engine;

public record MoveListItem(
    int Number, string?
    White = null,
    string? Black = null,
    string? Misc = null);
