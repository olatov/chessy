namespace Chessy.Engine.Pieces;

public interface IPiece
{
    public PieceColor Color { get; set; }

    public PieceKind Kind { get; set; }

    public string Identifier { get; }
}
