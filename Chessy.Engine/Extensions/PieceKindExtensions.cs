using Chessy.Engine.Pieces;

namespace Chessy.Engine.Extensions;

public static class PieceKindExtensions
{
    public static double Value(this PieceKind pieceKind)
    {
        return pieceKind switch
        {
            PieceKind.Pawn => 1.00,
            PieceKind.Knight => 3.05,
            PieceKind.Bishop => 3.33,
            PieceKind.Rook => 5.63,
            PieceKind.Queen => 9.50,
            PieceKind.King => 1.0e+6,
            _ => throw new NotImplementedException(),
        };
    }

    public static string Figurine(this PieceKind pieceKind)
    {
        return pieceKind switch
        {
            PieceKind.King => "♔",
            PieceKind.Queen => "♕",
            PieceKind.Rook => "♖",
            PieceKind.Bishop => "♗",
            PieceKind.Knight => "♘",
            PieceKind.Pawn => "",
            _ => string.Empty
        };
    }
}