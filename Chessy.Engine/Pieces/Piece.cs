namespace Chessy.Engine.Pieces;

public enum PieceKind
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King,
}

public enum PieceColor
{
    White,
    Black,
}

public class Piece : IPiece
{
    public PieceColor Color { get; set; }
    public PieceKind Kind { get; set; }

    public override string ToString()
    {
        return $"{Color} {Kind}";
    }

    public string Identifier
    {
        get => Color switch
        {
            PieceColor.White => Kind switch
            {
                PieceKind.King => "♔",
                PieceKind.Queen => "♕",
                PieceKind.Rook => "♖",
                PieceKind.Bishop => "♗",
                PieceKind.Knight => "♘",
                PieceKind.Pawn => "♙",
                _ => throw new NotImplementedException(),
            },
            PieceColor.Black => Kind switch
            {
                PieceKind.King => "♚",
                PieceKind.Queen => "♛",
                PieceKind.Rook => "♜",
                PieceKind.Bishop => "♝",
                PieceKind.Knight => "♞",
                PieceKind.Pawn => "♟︎",
                _ => throw new NotImplementedException(),
            },
            _ => string.Empty,
        };
    }
}
