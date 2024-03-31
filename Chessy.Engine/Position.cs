using System.Diagnostics;
using Chessy.Engine.Events;
using Chessy.Engine.Extensions;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public class Position
{
    public Board Board { get; set; } = new Board();

    public IList<Move> Moves { get; set; } = new List<Move>();

    public PieceColor ColorToMove { get; set; }

    public bool[] CastlingState { get; set; }

    private long _nodesCounter { get; set; }

    public event EventHandler<FindMoveProgressEventArgs>? FindMoveProgress;

    public static Position FromMoves(IEnumerable<Move> moves)
    {
        var result = new Position();
        result.ResetToStartingPosition();
        foreach (var move in moves)
        {
            result.MakeMove(move);
            result.Moves.Add(move);
            result.ColorToMove = result.ColorToMove.OpponentColor();
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

    public Move? FindBestMoveAB(PieceColor playerColor, int depth = 1)
    {
        Console.WriteLine();
        var sw = Stopwatch.StartNew();
        var moves = GetMoves(playerColor, true, true).ToArray();
        if (moves.Length == 0)
        {
            return null;
        }
        else if (moves.Length == 1)
        {
            Console.WriteLine();
            Console.WriteLine($"[Forced]\t{moves.Single().NotationVariants.First()}");
            return moves.Single();
        }

        _nodesCounter = 0;
        var (bestMove, _) = FindMoveAB(
            depth,
            isMaximising: playerColor == PieceColor.White,
            debug: true,
            legalChecks: true);
        sw.Stop();

        Console.WriteLine($"Nodes: {_nodesCounter / 1.0e+6:0.0}m ({_nodesCounter / sw.Elapsed.TotalSeconds / 1000.0:0.0}k / sec)");
        Console.WriteLine($"Time: {sw.Elapsed}");

        if (bestMove is null) { return null; }

        return moves.Single(x =>
            x.From.file == bestMove.From.file
            && x.From.rank == bestMove.From.rank
            && x.To.file == bestMove.To.file
            && x.To.rank == bestMove.To.rank
            && x.PromotionPiece == bestMove.PromotionPiece);
    }

    public (Move? move, double score) FindMoveAB(int depth, double alpha = double.MinValue, double beta = double.MaxValue, bool isMaximising = true, bool legalChecks = false, bool debug = false)
    {
        if (depth == 0)
        {
            _nodesCounter++;
            return (null, isMaximising ? Board.MaterialValue : -Board.MaterialValue);
        }

        var moves = GetMoves(isMaximising ? PieceColor.White : PieceColor.Black, legalChecks);
        if (!moves.Any())
        {
            return (null, 0);
        }

        double bestScore = double.MinValue;
        Move? bestMove = null;

        if (depth > 2)
        {
            moves = moves.OrderBy(x =>
            {
                MakeMove(x);
                var (_, res) = FindMoveAB(1, -beta, -alpha, !isMaximising);
                UndoMove(x);
                return res;
            }).ToList();
        }

        int total = moves.Count();

        if (debug)
        {
            FindMoveProgress?.Invoke(this, new FindMoveProgressEventArgs { Current = 0, Total = total });
        }
        int counter = 0;

        foreach (var move in moves)
        {
            counter++;
            // Stopwatch? sw = null;
            // if (debug)
            // {
            //     var shortNotation = move.NotationVariants
            //         .First(x => moves.Count(y => y.NotationVariants.Contains(x)) == 1);
            //     Console.Write($"[{counter} / {total}]\t{shortNotation,-8}\t");
            //     sw = Stopwatch.StartNew();
            // }
            MakeMove(move);
            double moveScore;
            if (move.CapturedPiece?.Kind == PieceKind.King)
            {
                moveScore = -1.0e+6 - (depth * 1000);
                _nodesCounter++;
            }
            else
            {
                (_, moveScore) = FindMoveAB(depth - 1, -beta, -alpha, !isMaximising);
            }
            moveScore = -moveScore;
            UndoMove(move);

            if (move.IsCastlingShort || move.IsCastlingLong)
            {
                moveScore += 0.3;
            }

            if (moveScore > bestScore)
            {
                bestScore = moveScore;
                bestMove = move;
            }

            if (debug)
            {
                // sw?.Stop();
                // Console.Write($"{moveScore,6:0.00}\t");
                // Console.Write(
                //     (sw?.Elapsed.TotalMilliseconds >= 100)
                //         ? $"{sw?.Elapsed.TotalSeconds,5:0.0}s\t"
                //         : $"{"-",6}\t");

                // if (bestMove == move)
                // {
                //     Console.Write("<-");
                // }
                // Console.WriteLine();

                FindMoveProgress?.Invoke(this, new FindMoveProgressEventArgs { Current = counter, Total = total });
            }

            alpha = Math.Max(alpha, bestScore);
            if (alpha >= beta)
            {
                break;
            }
        }

        if (!moves.Any())
        {
            return (null, isMaximising ? Board.MaterialValue : -Board.MaterialValue);
        }

        return (bestMove, bestScore);

        //return isMaximising ? Board.MaterialValue : -Board.MaterialValue;

        //    value := max(value, −negamax(child, depth − 1, −β, −α, −color))
        //α:= max(α, value)
        //if α ≥ β then
        //    break (*cut - off *)
    }


    public IEnumerable<Move> GetMoves(PieceColor playerColor, bool legalChecks = true, bool fullInfo = false)
    {
        bool isLegal = false;

        foreach (int fromFile in Enumerable.Range(0, 8))
        {
            foreach (int fromRank in Enumerable.Range(0, 8))
            {
                var fromSquare = Board.Squares[fromFile, fromRank];
                if (fromSquare?.Color == playerColor)
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

                            isLegal = IsLegalMove(move);

                            if (isLegal && legalChecks)
                            {
                                MakeMove(move);
                                var opponentMoves = GetMoves(playerColor.OpponentColor(), false, false);
                                isLegal = !opponentMoves.Any(x => Board.Squares[x.To.file, x.To.rank]?.Kind == PieceKind.King);
                                UndoMove(move);
                            }

                            if (isLegal && legalChecks && (move.IsCastlingShort || move.IsCastlingLong))
                            {
                                var opponentMoves = GetMoves(playerColor.OpponentColor(), false);
                                isLegal = !opponentMoves.Any(x => Board.Squares[x.To.file, x.To.rank]?.Kind == PieceKind.King);
                                if (move.IsCastlingShort)
                                {
                                    isLegal = isLegal && !opponentMoves.Any(x => x.To.file == (move.To.file + 1) && x.To.rank == move.To.rank);
                                }
                                else if (move.IsCastlingLong)
                                {
                                    isLegal = isLegal && !opponentMoves.Any(x => x.To.file == (move.To.file - 1) && x.To.rank == move.To.rank);
                                }
                            }

                            if (fullInfo)
                            {
                                MakeMove(move);
                                var nextMoves = GetMoves(playerColor, false, false);
                                move.IsCheck = nextMoves.Any(x => x.CapturedPiece?.Kind == PieceKind.King);
                                nextMoves = GetMoves(playerColor.OpponentColor(), true, false);
                                if (move.IsCheck)
                                {
                                    move.IsCheckmate = !nextMoves.Any();
                                }
                                else if (!nextMoves.Any())
                                {
                                    move.IsStalemate = true;
                                }
                                UndoMove(move);
                            }

                            if (isLegal)
                            {
                                yield return move;
                            }
                        }
                    }
                }
            }
        }
    }

    public bool IsLegalMove(Move move)
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
                    && Board.IsClearBetween(move.From, (7, move.From.rank))
                    && ((ColorToMove == PieceColor.White && CastlingState[0])
                        || (ColorToMove == PieceColor.Black && CastlingState[2])))
                {
                    return true;
                }
                else if (move.IsCastlingLong
                    && Board.IsClearBetween(move.From, (0, move.From.rank))
                    && ((ColorToMove == PieceColor.White && CastlingState[1])
                        || (ColorToMove == PieceColor.Black && CastlingState[3]))
                    )
                {
                    return true;
                }

                return false;

            case PieceKind.Rook:
                if (move.From.file == move.To.file || move.From.rank == move.To.rank)
                {
                    return Board.IsClearBetween(move.From, move.To);
                }
                break;

            case PieceKind.Bishop:
                if (Math.Abs(move.From.file - move.To.file) == Math.Abs(move.From.rank - move.To.rank))
                {
                    return Board.IsClearBetween(move.From, move.To);
                }
                break;

            case PieceKind.Queen:
                if (move.From.file == move.To.file || move.From.rank == move.To.rank || Math.Abs(move.From.file - move.To.file) == Math.Abs(move.From.rank - move.To.rank))
                {
                    return Board.IsClearBetween(move.From, move.To);
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
