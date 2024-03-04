using Chessy.Engine.Pieces;

namespace Chessy.Engine.Extensions;

public static class PieceExtensions
{
    public static double GetValue(this IPiece piece)
    {
        double value = piece.Kind switch
        {
            PieceKind.Pawn => 1.0,
            PieceKind.Knight => 3.0,
            PieceKind.Bishop => 3.0,
            PieceKind.Rook => 5.0,
            PieceKind.Queen => 9.0,
            PieceKind.King => 1.0e+6,
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
