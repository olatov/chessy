﻿using System.Text;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public class Move
{
    public (int file, int rank) From { get; set; }

    public (int file, int rank) To { get; set; }

    public IPiece Piece { get; set; }

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

    public string Notation
    {
        get
        {
            var result = new StringBuilder();


            if (IsCastlingShort)
            {
                result.Append("0-0");
            }
            else if (IsCastlingLong)
            {
                result.Append("0-0-0");
            }
            else
            {
                var piecePrefix = Piece.Kind switch
                {
                    PieceKind.King => "K",
                    PieceKind.Queen => "Q",
                    PieceKind.Rook => "R",
                    PieceKind.Bishop => "B",
                    PieceKind.Knight => "N",
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
                        PieceKind.King => "K",
                        PieceKind.Queen => "Q",
                        PieceKind.Rook => "R",
                        PieceKind.Bishop => "B",
                        PieceKind.Knight => "N",
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
