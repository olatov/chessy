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

    public Piece(PieceColor color, PieceKind kind)
    {
        Color = color;
        Kind = kind;
    }

    public static Piece CreatePawn(PieceColor color) => new Piece(color, PieceKind.Pawn);

    public static Piece CreateKnight(PieceColor color) => new Piece(color, PieceKind.Knight);

    public static Piece CreateBishop(PieceColor color) => new Piece(color, PieceKind.Bishop);

    public static Piece CreateRook(PieceColor color) => new Piece(color, PieceKind.Rook);

    public static Piece CreateQueen(PieceColor color) => new Piece(color, PieceKind.Queen);

    public static Piece CreateKing(PieceColor color) => new Piece(color, PieceKind.King);

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
