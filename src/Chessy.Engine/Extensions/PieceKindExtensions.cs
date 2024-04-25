using Chessy.Engine.Pieces;

namespace Chessy.Engine.Extensions;

public static class PieceKindExtensions
{
    public static int Value(this PieceKind pieceKind)
    {
        return pieceKind switch
        {
            PieceKind.Pawn => 100,
            PieceKind.Knight => 305,
            PieceKind.Bishop => 333,
            PieceKind.Rook => 563,
            PieceKind.Queen => 950,
            PieceKind.King => 100_000_000,
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

    public static string Algebraic(this PieceKind pieceKind)
    {
        return pieceKind switch
        {
            PieceKind.King => "K",
            PieceKind.Queen => "Q",
            PieceKind.Rook => "R",
            PieceKind.Bishop => "B",
            PieceKind.Knight => "N",
            PieceKind.Pawn => "",
            _ => string.Empty
        };
    }
}