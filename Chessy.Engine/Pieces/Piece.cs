using System.Web;
using Microsoft.AspNetCore.Html;
using static System.Net.Mime.MediaTypeNames;

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

    public string Icon
    {
        get
        {
            switch (Color)
            {
                case PieceColor.White:
                    return Kind switch
                    {
                        PieceKind.King => "♔",
                        PieceKind.Queen => "♕",
                        PieceKind.Rook => "♖",
                        PieceKind.Bishop => "♗",
                        PieceKind.Knight => "♘",
                        PieceKind.Pawn => "♙",
                        _ => throw new NotImplementedException(),
                    };

                case PieceColor.Black:
                    return Kind switch
                    {
                        PieceKind.King => "♚",
                        PieceKind.Queen => "♛",
                        PieceKind.Rook => "♜",
                        PieceKind.Bishop => "♝",
                        PieceKind.Knight => "♞",
                        PieceKind.Pawn => "♟︎",
                        _ => throw new NotImplementedException(),
                    };
            }

            return string.Empty;
        }
    }
}
