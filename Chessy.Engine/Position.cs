using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public class Position
{
    public Board Board { get; set; } = new Board();

    public IList<Move> Moves { get; set; } = new List<Move>();

    public PieceColor ColorToMove { get; set; }

    public bool[] CastlingState { get; set; }

    public static Position FromMoves(IEnumerable<Move> moves)
    {
        var result = new Position();
        result.ResetToStartingPosition();
        foreach (var move in moves)
        {
            result.MakeMove(move);
            result.Moves.Add(move);
            result.SwitchColor();
        }

        return result;
    }

    public void ResetToStartingPosition()
    {
        Board = new Board();
        Moves = new List<Move>();

        foreach (int file in new[] { 0, 7 })
        {
            Board.Squares[file, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Rook };
            Board.Squares[file, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Rook };
        }

        foreach (int file in new[] { 1, 6 })
        {
            Board.Squares[file, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Knight };
            Board.Squares[file, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Knight };
        }

        foreach (int file in new[] { 2, 5 })
        {
            Board.Squares[file, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Bishop };
            Board.Squares[file, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Bishop };
        }

        Board.Squares[3, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Queen };
        Board.Squares[3, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Queen };

        Board.Squares[4, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.King };
        Board.Squares[4, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.King };

        foreach (int file in Enumerable.Range(0, 8))
        {
            Board.Squares[file, 1] = new Piece { Color = PieceColor.White, Kind = PieceKind.Pawn };
            Board.Squares[file, 6] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Pawn };
        }

        ColorToMove = PieceColor.White;

        CastlingState = [true, true, true, true];
    }

    public void MakeMove(Move move)
    {
        Board.Squares[move.To.file, move.To.rank] = move.Piece;
        Board.Squares[move.From.file, move.From.rank] = null;

        if (move.IsPromotion)
        {
            move.Piece.Kind = move.PromotionPiece!.Value;
        }

        if (move.Piece.Kind == PieceKind.King)
        {
            if (move.IsCastlingShort)
            {
                Board.Squares[5, move.From.rank] = move.CastlingRook;
                Board.Squares[7, move.From.rank] = null;
            }
            else if (move.IsCastlingLong)
            {
                Board.Squares[3, move.From.rank] = move.CastlingRook;
                Board.Squares[0, move.From.rank] = null;
            }

            if (move.Piece.Color == PieceColor.White)
            {
                CastlingState[0] = false;
                CastlingState[1] = false;
            }
            else
            {
                CastlingState[2] = false;
                CastlingState[3] = false;
            }
        }
        else if (move.Piece.Kind == PieceKind.Rook)
        {
            if (move.Piece.Color == PieceColor.White)
            {
                if (move.From.file == 0)
                {
                    CastlingState[1] = false;
                }
                else if (move.From.file == 7)
                {
                    CastlingState[0] = false;
                }
            }
            else
            {
                if (move.From.file == 0)
                {
                    CastlingState[3] = false;
                }
                else if (move.From.file == 7)
                {
                    CastlingState[2] = false;
                }
            }
        }
    }

    public void UndoMove(Move move)
    {
        Board.Squares[move.From.file, move.From.rank] = move.Piece;
        Board.Squares[move.To.file, move.To.rank] = move.IsCapture ? move.CapturedPiece : null;

        if (move.IsPromotion)
        {
            move.Piece.Kind = PieceKind.Pawn;
        }

        if (move.IsCastlingShort)
        {
            Board.Squares[5, move.From.rank] = null;
            Board.Squares[7, move.From.rank] = move.CastlingRook;
        }
        else if (move.IsCastlingLong)
        {
            Board.Squares[3, move.From.rank] = null;
            Board.Squares[0, move.From.rank] = move.CastlingRook;
        }

        if (move.Piece.Color == PieceColor.White)
        {
            CastlingState[0] = move.CouldCastleShort;
            CastlingState[1] = move.CouldCastleLong;
        }
        else
        {
            CastlingState[2] = move.CouldCastleShort;
            CastlingState[3] = move.CouldCastleLong;
        }
    }

    public void SwitchColor()
    {
        ColorToMove = (ColorToMove == PieceColor.White)
            ? PieceColor.Black
            : PieceColor.White;
    }

    public (Move? move, double score) FindBestMove(int depth = 1, bool isFullInfo = false)
    {
        const int maxDepth = 3;

        Move? result = null;
        double bestScore = (ColorToMove == PieceColor.White) ? -1.0e+9 : 1.0e+9;

        if (depth == maxDepth)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {ColorToMove} ---");
        }

        foreach (var move in GetValidMoves(false, fullInfo: isFullInfo))
        {
            if (Board.Squares[move.To.file, move.To.rank]?.Kind == PieceKind.King)
            {
                return (move, (ColorToMove == PieceColor.White) ? 1.0e+9 : -1.0e+9);
            }

            MakeMove(move);
            double score = 0;

            if (depth > 0)
            {
                SwitchColor();
                (_, score) = FindBestMove(depth - 1);
                SwitchColor();
            }
            else
            {
                score = Board.MaterialValue;
            }

            if ((ColorToMove == PieceColor.White && score > bestScore) 
                || (ColorToMove == PieceColor.Black && score < bestScore))
            {
                result = move;
                bestScore = score;
            }

            if (depth == maxDepth)
            {
                Console.WriteLine($"{move.Notation}\t {score: +0.00}");
            }
            
            UndoMove(move);
        }

        if (result is null)
        {
            // Stalemate?
            //return (result, 0);
        }

        return (result, bestScore);
    }

    public IEnumerable<Move> GetValidMoves(bool checks = true, bool fullInfo = false)
    {
        bool isValid = false;

        foreach (int fromFile in Enumerable.Range(0, 8))
        {
            foreach (int fromRank in Enumerable.Range(0, 8))
            {
                var fromSquare = Board.Squares[fromFile, fromRank];
                if (fromSquare?.Color == ColorToMove)
                {
                    foreach (int toFile in Enumerable.Range(0, 8))
                    {
                        foreach (int toRank in Enumerable.Range(0, 8))
                        {
                            var toSquare = Board.Squares[toFile, toRank];
                            if (fromSquare.Color == toSquare?.Color)
                            {
                                continue;
                            }

                            var move = new Move
                            {
                                Piece = fromSquare,
                                CapturedPiece = toSquare,
                                From = (fromFile, fromRank),
                                To = (toFile, toRank),
                            };

                            if ((fromSquare.Kind == PieceKind.Pawn) && (toRank == 0 || toRank == 7))
                            {
                                move.PromotionPiece = PieceKind.Queen;
                            }

                            if (move.IsCastlingShort)
                            {
                                if (Board.Squares[5, fromRank] is not null)
                                {
                                    continue;
                                }
                                move.CastlingRook = Board.Squares[7, fromRank];
                            }
                            else if (move.IsCastlingLong)
                            {
                                if (Board.Squares[3, fromRank] is not null)
                                {
                                    continue;
                                }
                                move.CastlingRook = Board.Squares[0, fromRank];
                            }

                            if (fromSquare.Color == PieceColor.White)
                            {
                                move.CouldCastleShort = CastlingState[0];
                                move.CouldCastleLong = CastlingState[1];
                            }
                            else
                            {
                                move.CouldCastleShort = CastlingState[2];
                                move.CouldCastleLong = CastlingState[3];
                            }

                            isValid = IsValidMove(move);

                            if (isValid && checks)
                            {
                                MakeMove(move);
                                SwitchColor();
                                var opponentMoves = GetValidMoves(false);
                                isValid = !opponentMoves.Any(x => Board.Squares[x.To.file, x.To.rank]?.Kind == PieceKind.King);
                                UndoMove(move);
                                SwitchColor();
                            }

                            if (fullInfo)
                            {
                                MakeMove(move);
                                var nextMoves = GetValidMoves(false, false);
                                move.IsCheck = nextMoves.Any(x => x.CapturedPiece?.Kind == PieceKind.King);
                                if (move.IsCheck)
                                {
                                    SwitchColor();
                                    nextMoves = GetValidMoves(true, false);
                                    move.IsCheckmate = !nextMoves.Any();
                                    SwitchColor();
                                }
                                UndoMove(move);
                            }
                                
                            if (isValid)
                            {
                                yield return move;
                            }
                        }
                    }
                }
            }
        }
    }

    public bool IsValidMove(Move move)
    {
        var piece = Board.Squares[move.From.file, move.From.rank];
        if (piece is null || piece != move.Piece) { return false; }

        var destinationSquare = Board.Squares[move.To.file, move.To.rank];
        if (destinationSquare?.Color == piece.Color) { return false; }

        switch (piece.Kind)
        {
            case PieceKind.King:
                if (((Math.Abs(move.From.file - move.To.file) == 1)
                    && (Math.Abs(move.From.rank - move.To.rank) <= 1))
                    || ((Math.Abs(move.From.file - move.To.file) <= 1)
                    && (Math.Abs(move.From.rank - move.To.rank) == 1)))
                {
                    return true;
                }

                    if (move.IsCastlingShort
                        && Board.IsClearBetween(move.From, (7, move.From.rank)))
                    {
                        return true;
                    }
                    else if (move.IsCastlingLong
                        && Board.IsClearBetween(move.From, (0, move.From.rank)))
                    {
                        return true;
                    }

                return false;

            case PieceKind.Rook:
                if (move.From.file == move.To.file || move.From.rank == move.To.rank)
                {
                    if (!Board.IsClearBetween(move.From, move.To))
                    {
                        return false;
                    }

                    return true;
                }
                break;

            case PieceKind.Bishop:
                if (Math.Abs(move.From.file - move.To.file) == Math.Abs(move.From.rank - move.To.rank))
                {
                    if (!Board.IsClearBetween(move.From, move.To))
                    {
                        return false;
                    }

                    return true;
                }
                break;

            case PieceKind.Queen:
                if (move.From.file == move.To.file || move.From.rank == move.To.rank)
                {
                    if (!Board.IsClearBetween(move.From, move.To))
                    {
                        return false;
                    }

                    return true;
                }
                else if (Math.Abs(move.From.file - move.To.file) == Math.Abs(move.From.rank - move.To.rank))
                {
                    if (!Board.IsClearBetween(move.From, move.To))
                    {
                        return false;
                    }

                    return true;
                }
                break;

            case PieceKind.Knight:
                if ((Math.Abs(move.From.file - move.To.file) == 1
                     && Math.Abs(move.From.rank - move.To.rank) == 2)
                    || (Math.Abs(move.From.file - move.To.file) == 2
                    && Math.Abs(move.From.rank - move.To.rank) == 1))
                {
                    return true;
                }
                return false;

            case PieceKind.Pawn:
                switch (piece.Color)
                {
                    case PieceColor.White:
                        if ((move.To.rank - move.From.rank == 1)
                            && ((move.From.file == move.To.file && Board.Squares[move.To.file, move.To.rank] is null)
                                || (Math.Abs(move.From.file - move.To.file) == 1 && Board.Squares[move.To.file, move.To.rank]?.Color == PieceColor.Black)))
                        {
                            return true;
                        }

                        if ((move.From.rank == 1)
                            && (move.To.rank == 3)
                            && (move.From.file == move.To.file)
                            && Board.Squares[move.From.file, 2] is null
                            && Board.Squares[move.From.file, 3] is null)
                        {
                            return true;
                        }

                        return false;

                    case PieceColor.Black:
                        if ((move.To.rank - move.From.rank == -1)
                            && ((move.From.file == move.To.file && Board.Squares[move.To.file, move.To.rank] is null)
                                || (Math.Abs(move.From.file - move.To.file) == 1 && Board.Squares[move.To.file, move.To.rank]?.Color == PieceColor.White)))
                        {
                            return true;
                        }
                        if ((move.From.rank == 6)
                            && (move.To.rank == 4)
                            && (move.From.file == move.To.file)
                            && Board.Squares[move.From.file, 5] is null
                            && Board.Squares[move.From.file, 4] is null)
                        {
                            return true;
                        }


                        return false;

                }
                break;
        }

        return false;
    }
}
