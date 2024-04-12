using System.Diagnostics;
using System.Text;
using Chessy.Engine.Extensions;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public class Move(IPiece piece, Coords from, Coords to, PieceKind? promitionPieceKind = null)
{
    public Coords From { get; set; } = from;

    public Coords To { get; set; } = to;

    public IPiece Piece { get; set; } = piece;

    public bool IsCheck { get; set; }

    public bool IsCheckmate { get; set; }

    public bool IsStalemate { get; set; }

    public IPiece? CapturedPiece { get; set; }

    public bool IsCapture { get => CapturedPiece is not null; }

    public bool IsEnPassantCapture { get; set; }

    public Coords? EnPassantTarget => IsPawnDoubleMove ?
        new Coords(To.File, Piece.Color == PieceColor.White ? 2 : 5)
        : null;

    public static Move For(IPiece piece, Coords from, Coords to, PieceKind? promitionPieceKind = null) =>
        new(piece, from, to) { PromotionPieceKind = promitionPieceKind };

    public static Move For(IPiece piece, string from, string to, PieceKind? promitionPieceKind = null) =>
        new(piece, Coords.Parse(from), Coords.Parse(to), promitionPieceKind);

    public bool IsPawnDoubleMove => Piece?.Kind == PieceKind.Pawn && Math.Abs(From.Rank - To.Rank) == 2;

    public bool IsCastlingShort
    {
        get => Piece?.Kind == PieceKind.King
            && From.File == 4
            && To.File == 6
            && From.Rank == To.Rank;
    }

    public bool IsCastlingLong
    {
        get => Piece?.Kind == PieceKind.King
            && From.File == 4
            && To.File == 2
            && From.Rank == To.Rank;
    }

    public IPiece? CastlingRook { get; set; }

    public PieceKind? PromotionPieceKind { get; set; } = promitionPieceKind;

    public bool IsPromotion => PromotionPieceKind is not null;

    public bool CouldCastleShort { get; set; }

    public bool CouldCastleLong { get; set; }

    public Move Copy()
    {
        return new Move(Piece, From, To)
        {
            IsCheck = IsCheck,
            IsCheckmate = IsCheckmate,
            IsStalemate = IsStalemate,
            CapturedPiece = CapturedPiece,
            IsEnPassantCapture = IsEnPassantCapture,
            CastlingRook = CastlingRook,
            PromotionPieceKind = PromotionPieceKind,
            CouldCastleShort = CouldCastleShort,
            CouldCastleLong = CouldCastleLong
        };
    }

    public IList<string> GetNotationVariants()
    {
        var variants = new List<string>();

        var result = new StringBuilder();

        foreach (var variant in Enumerable.Range(1, 4))
        {
            result.Clear();

            if (IsCastlingShort)
            {
                result.Append("O-O");
            }
            else if (IsCastlingLong)
            {
                result.Append("O-O-O");
            }
            else
            {
                var fromFile = (char)((char)From.File + 'a');
                var fromRank = (char)((char)From.Rank + '1');
                var toFile = (char)((char)To.File + 'a');
                var toRank = (char)((char)To.Rank + '1');

                result.Append(Piece!.Kind.Algebraic());

                if (variant == 2 || variant == 4 || (Piece?.Kind == PieceKind.Pawn && IsCapture))
                {
                    result.Append(fromFile);
                }

                if (variant == 3 || variant == 4)
                {
                    result.Append(fromRank);
                }

                if (IsCapture)
                {
                    result.Append('x');
                }

                result.Append(toFile).Append(toRank);

                if (IsPromotion)
                {
                    Trace.Assert(PromotionPieceKind is not null);
                    result.Append('=').Append(PromotionPieceKind.Value.Algebraic());
                }
            }

            if (IsCheckmate)
            {
                result.Append('#');
            }
            else if (IsCheck)
            {
                result.Append('+');
            }

            variants.Add(result.ToString());
        }

        return variants.Distinct().OrderBy(x => x.Length).ToList();
    }

    public override string ToString() => GetNotationVariants().Last();
}
