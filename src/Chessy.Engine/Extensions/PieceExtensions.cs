using Chessy.Engine.Pieces;

namespace Chessy.Engine.Extensions;

public static class PieceExtensions
{
    public static int GetValue(this IPiece piece)
    {
        int value = piece.Kind switch
        {
            PieceKind.Pawn => 100,
            PieceKind.Knight => 305,
            PieceKind.Bishop => 333,
            PieceKind.Rook => 563,
            PieceKind.Queen => 950,
            PieceKind.King => 100_000_000,
            _ => throw new NotImplementedException(),
        };

        return (piece.Color == PieceColor.White) ? value : -value;
    }
}

public static class PieceColorExtensions
{
    public static PieceColor OpponentColor(this PieceColor pieceColor)
    {
        return pieceColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
}
