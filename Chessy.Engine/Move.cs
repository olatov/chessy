using System.Text;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public class Move
{
    public (int file, int rank) From { get; set; }

    public (int file, int rank) To { get; set; }

    public IPiece? Piece { get; set; }

    public bool IsCheck { get; set; }

    public bool IsCheckmate { get; set; }

    public bool IsStalemate { get; set; }

    public IPiece? CapturedPiece { get; set; }

    public bool IsCapture { get => CapturedPiece is not null; }

    public bool IsEnPassantCapture { get; set; }

    public bool IsCastlingShort
    {
        get => Piece.Kind == PieceKind.King
            && From.file == 4
            && To.file == 6
            && From.rank == To.rank;
    }

    public bool IsCastlingLong
    {
        get => Piece.Kind == PieceKind.King
            && From.file == 4
            && To.file == 2
            && From.rank == To.rank;
    }

    public IPiece? CastlingRook { get; set; }

    public PieceKind? PromotionPiece { get; set; }

    public bool IsPromotion { get => PromotionPiece is not null; }

    public bool CouldCastleShort { get; set; }

    public bool CouldCastleLong { get; set; }

    public IEnumerable<string> NotationVariants
    {
        get
        {
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
                    var piecePrefix = Piece.Kind switch
                    {
                        PieceKind.King => "♔",
                        PieceKind.Queen => "♕",
                        PieceKind.Rook => "♖",
                        PieceKind.Bishop => "♗",
                        PieceKind.Knight => "♘",
                        _ => string.Empty
                    };

                    var fromFile = (char)((char)From.file + 'a');
                    var fromRank = (char)((char)From.rank + '1');
                    var toFile = (char)((char)To.file + 'a');
                    var toRank = (char)((char)To.rank + '1');

                    result.Append(piecePrefix);

                    if (variant == 2 || variant == 4 || (Piece.Kind == PieceKind.Pawn && IsCapture))
                    {
                        result.Append(fromFile);
                    }

                    if (variant == 3 || variant == 4)
                    {
                        result.Append(fromRank);
                    }

                    if (IsCapture)
                    {
                        result.Append("x");
                    }

                    result.Append(toFile)
                        .Append(toRank);

                    if (IsPromotion)
                    {
                        var promotionPiecePrefix = PromotionPiece switch
                        {
                            PieceKind.King => "♔",
                            PieceKind.Queen => "♕",
                            PieceKind.Rook => "♖",
                            PieceKind.Bishop => "♗",
                            PieceKind.Knight => "♘",
                            _ => string.Empty
                        };

                        result.Append("=")
                            .Append(promotionPiecePrefix);
                    }
                }

                if (IsCheckmate)
                {
                    result.Append("#");
                }
                else if (IsCheck)
                {
                    result.Append("+");
                }

                yield return result.ToString();
            }
        }
    }

    public string Notation
    {
        get
        {
            var result = new StringBuilder();

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
                var piecePrefix = Piece.Kind switch
                {
                    PieceKind.King => "♔",
                    PieceKind.Queen => "♕",
                    PieceKind.Rook => "♖",
                    PieceKind.Bishop => "♗",
                    PieceKind.Knight => "♘",
                    _ => string.Empty
                };

                var fromFile = (char)((char)From.file + 'a');
                var fromRank = (char)((char)From.rank + '1');
                var toFile = (char)((char)To.file + 'a');
                var toRank = (char)((char)To.rank + '1');

                result.Append(piecePrefix)
                    .Append(fromFile)
                    .Append(fromRank);

                if (IsCapture)
                {
                    result.Append("x");
                }

                result.Append(toFile)
                    .Append(toRank);

                if (IsPromotion)
                {
                    var promotionPiecePrefix = PromotionPiece switch
                    {
                        PieceKind.King => "♔",
                        PieceKind.Queen => "♕",
                        PieceKind.Rook => "♖",
                        PieceKind.Bishop => "♗",
                        PieceKind.Knight => "♘",
                        _ => string.Empty
                    };

                    result.Append("=")
                        .Append(promotionPiecePrefix);
                }
            }

            if (IsCheckmate)
            {
                result.Append("#");
            }
            else if (IsCheck)
            {
                result.Append("+");
            }

            return result.ToString();
        }
    }

    public override string ToString()
    {
        return Notation;
    }
}
