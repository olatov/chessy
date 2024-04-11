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

    public bool IsGameOver
    {
        get
        {
            if (!Moves.Any()) { return false; }
            var lastMove = Moves[^1];
            return lastMove.IsCheckmate || lastMove.IsStalemate;
        }
    }

    public bool[] CastlingState { get; set; } = Array.Empty<bool>();

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

        foreach (int File in new[] { 0, 7 })
        {
            Board.Squares[File, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Rook };
            Board.Squares[File, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Rook };
        }

        foreach (int File in new[] { 1, 6 })
        {
            Board.Squares[File, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Knight };
            Board.Squares[File, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Knight };
        }

        foreach (int File in new[] { 2, 5 })
        {
            Board.Squares[File, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Bishop };
            Board.Squares[File, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Bishop };
        }

        Board.Squares[3, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.Queen };
        Board.Squares[3, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Queen };

        Board.Squares[4, 0] = new Piece { Color = PieceColor.White, Kind = PieceKind.King };
        Board.Squares[4, 7] = new Piece { Color = PieceColor.Black, Kind = PieceKind.King };

        foreach (int File in Enumerable.Range(0, 8))
        {
            Board.Squares[File, 1] = new Piece { Color = PieceColor.White, Kind = PieceKind.Pawn };
            Board.Squares[File, 6] = new Piece { Color = PieceColor.Black, Kind = PieceKind.Pawn };
        }

        ColorToMove = PieceColor.White;

        CastlingState = [true, true, true, true];
    }

    public void ResetToEmptyBoard()
    {
        Board = new Board();
        Moves = new List<Move>();
        ColorToMove = PieceColor.White;
        CastlingState = [true, true, true, true];
    }

    public void AddPiece(IPiece piece, Coords position)
    {
        if (Board.Squares[position.File, position.Rank] is not null)
        {
            throw new InvalidOperationException("Square is not empty");
        }

        Board.Squares[position.File, position.Rank] = piece;
    }

    public void MakeMove(Move move)
    {
        Board.Squares[move.To.File, move.To.Rank] = move.Piece;
        Board.Squares[move.From.File, move.From.Rank] = null;

        if (move.IsPromotion)
        {
            Trace.Assert(move.PromotionPieceKind is not null);
            move.Piece.Kind = move.PromotionPieceKind.Value;
        }

        if (move.Piece.Kind == PieceKind.King)
        {
            if (move.IsCastlingShort)
            {
                Board.Squares[5, move.From.Rank] = move.CastlingRook;
                Board.Squares[7, move.From.Rank] = null;
            }
            else if (move.IsCastlingLong)
            {
                Board.Squares[3, move.From.Rank] = move.CastlingRook;
                Board.Squares[0, move.From.Rank] = null;
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
                if (move.From.File == 0)
                {
                    CastlingState[1] = false;
                }
                else if (move.From.File == 7)
                {
                    CastlingState[0] = false;
                }
            }
            else
            {
                if (move.From.File == 0)
                {
                    CastlingState[3] = false;
                }
                else if (move.From.File == 7)
                {
                    CastlingState[2] = false;
                }
            }
        }
    }

    public void UndoMove(Move move)
    {
        Board.Squares[move.From.File, move.From.Rank] = move.Piece;
        Board.Squares[move.To.File, move.To.Rank] = move.IsCapture ? move.CapturedPiece : null;

        if (move.IsPromotion)
        {
            move.Piece.Kind = PieceKind.Pawn;
        }

        if (move.IsCastlingShort)
        {
            Board.Squares[5, move.From.Rank] = null;
            Board.Squares[7, move.From.Rank] = move.CastlingRook;
        }
        else if (move.IsCastlingLong)
        {
            Board.Squares[3, move.From.Rank] = null;
            Board.Squares[0, move.From.Rank] = move.CastlingRook;
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

    public async Task<Move?> FindBestMoveABAsync(PieceColor playerColor, int depth = 1)
    {
        Console.WriteLine("Finding best move...");
        var moves = GetMoves(playerColor, true, true).ToArray();
        Console.WriteLine($"Moves: {moves.Length}");
        if (moves.Length == 0)
        {
            return null;
        }
        else if (moves.Length == 1)
        {
            Console.WriteLine();
            Console.WriteLine($"[Forced]\t{moves.Single().GetNotationVariants().First()}");
            return moves.Single();
        }

        var checkmateMove = Array.Find(moves, x => x.IsCheckmate);
        if (checkmateMove is not null)
        {
            return checkmateMove;
        }

        _nodesCounter = 0;
        var sw = Stopwatch.StartNew();
        var (bestMove, _) = await FindMoveABAsync(
            depth,
            isMaximising: playerColor == PieceColor.White,
            debug: true,
            legalChecks: true);
        sw.Stop();

        Console.WriteLine($"Nodes: {_nodesCounter / 1.0e+6:0.0}m ({_nodesCounter / sw.Elapsed.TotalSeconds / 1000.0:0.0}k / sec)");
        Console.WriteLine($"Time: {sw.Elapsed}");

        if (bestMove is null) { return null; }

        return moves.Single(x =>
            x.From.File == bestMove.From.File
            && x.From.Rank == bestMove.From.Rank
            && x.To.File == bestMove.To.File
            && x.To.Rank == bestMove.To.Rank
            && x.PromotionPieceKind == bestMove.PromotionPieceKind);
    }

    public async Task<(Move? move, double score)> FindMoveABAsync(int depth, double alpha = double.MinValue, double beta = double.MaxValue, bool isMaximising = true, bool legalChecks = false, bool debug = false)
    {
        if (depth == 0)
        {
            _nodesCounter++;
            return (null, isMaximising ? Board.MaterialValue : -Board.MaterialValue);
        }

        var moves = GetMoves(isMaximising ? PieceColor.White : PieceColor.Black, legalChecks, fullInfo: debug);
        if (!moves.Any())
        {
            return (null, 0);
        }

        double bestScore = double.MinValue;
        Move? bestMove = null;

        var moveScores = moves.Select(x => (Move: x, Score: 0.0)).ToArray();
        foreach (int index in Enumerable.Range(0, moves.Count()))
        {
            var move = moveScores[index].Move;
            MakeMove(move);
            //var (_, res) = await FindMoveABAsync(0, -beta, -alpha, !isMaximising);
            moveScores[index].Score = isMaximising ? Board.MaterialValue : -Board.MaterialValue;
            _nodesCounter++;
            UndoMove(move);
        }

        moves = moveScores.OrderByDescending(x => x.Score).Select(x => x.Move);

        int total = moves.Count();
        foreach (var (move, counter) in moves.Zip(Enumerable.Range(1, total)))
        {
            if (debug)
            {
                Console.WriteLine($"[{counter} / {total}]\t");
                FindMoveProgress?.Invoke(this, new FindMoveProgressEventArgs { Current = counter, Total = total });
                await Task.Delay(1);
            }

            double moveScore;
            if (move.IsStalemate)
            {
                moveScore = 0;
            }
            else
            {
                if (move.CapturedPiece?.Kind == PieceKind.King)
                {
                    moveScore = -1.0e+6 - (depth * 1000);
                    _nodesCounter++;
                }
                else
                {
                    MakeMove(move);
                    (_, moveScore) = await FindMoveABAsync(depth - 1, -beta, -alpha, !isMaximising);
                    UndoMove(move);
                }
                moveScore = -moveScore;

                if (move.IsCastlingShort || move.IsCastlingLong)
                {
                    moveScore += 0.3;
                }

                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestMove = move;
                }
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
    }


    public IEnumerable<Move> GetMoves(PieceColor playerColor, bool legalChecks = true, bool fullInfo = false)
    {
        bool isLegal = false;

        foreach (int fromFile in Enumerable.Range(0, 8))
        {
            foreach (int fromRank in Enumerable.Range(0, 8))
            {
                var fromSquare = Board.Squares[fromFile, fromRank];
                if (fromSquare?.Color != playerColor) { continue; }

                foreach (int toFile in Enumerable.Range(0, 8))
                {
                    foreach (int toRank in Enumerable.Range(0, 8))
                    {
                        var toSquare = Board.Squares[toFile, toRank];
                        if (fromSquare.Color == toSquare?.Color) { continue; }

                        var move = new Move(
                            piece: fromSquare,
                            from: new(fromFile, fromRank),
                            to: new(toFile, toRank))
                        {
                            CapturedPiece = toSquare,
                        };

                        if (move.Piece.Kind == PieceKind.Pawn && move.To.Rank is 0 or 7)
                        {
                            move.IsPromotion = true;
                            move.PromotionPieceKind = PieceKind.Queen;
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
                            isLegal = !opponentMoves.Any(x => Board.Squares[x.To.File, x.To.Rank]?.Kind == PieceKind.King);
                            UndoMove(move);
                        }

                        if (isLegal && legalChecks && (move.IsCastlingShort || move.IsCastlingLong))
                        {
                            var opponentMoves = GetMoves(playerColor.OpponentColor(), false);
                            isLegal = !opponentMoves.Any(x => Board.Squares[x.To.File, x.To.Rank]?.Kind == PieceKind.King);
                            if (move.IsCastlingShort)
                            {
                                isLegal = isLegal && !opponentMoves.Any(x => x.To.File == (move.To.File - 1) && x.To.Rank == move.To.Rank);
                            }
                            else if (move.IsCastlingLong)
                            {
                                isLegal = isLegal && !opponentMoves.Any(x => x.To.File == (move.To.File + 1) && x.To.Rank == move.To.Rank);
                            }
                        }

                        if (isLegal && fullInfo)
                        {
                            MakeMove(move);
                            var nextMoves = GetMoves(playerColor, false, false);
                            move.IsCheck = nextMoves.Any(x => x.CapturedPiece?.Kind == PieceKind.King);

                            bool opponentHasMoves = GetMoves(playerColor.OpponentColor(), true, false).Any();
                            if (!opponentHasMoves)
                            {
                                move.IsCheckmate = move.IsCheck;
                                move.IsStalemate = !move.IsCheck;
                            }
                            UndoMove(move);
                        }

                        if (isLegal)
                        {
                            yield return move;

                            if (move.IsPromotion)
                            {
                                foreach (var promotionPieceKind in
                                    new[] { PieceKind.Rook, PieceKind.Bishop, PieceKind.Knight })
                                {
                                    var moveCopy = move.Copy();
                                    moveCopy.PromotionPieceKind = promotionPieceKind;
                                    yield return moveCopy;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public bool IsLegalMove(Move move)
    {
        var piece = Board.Squares[move.From.File, move.From.Rank];
        if (piece is null || piece != move.Piece) { return false; }

        var destinationSquare = Board.Squares[move.To.File, move.To.Rank];
        if (destinationSquare?.Color == piece.Color) { return false; }

        switch (piece.Kind)
        {
            case PieceKind.King:
                if (((Math.Abs(move.From.File - move.To.File) == 1)
                    && (Math.Abs(move.From.Rank - move.To.Rank) <= 1))
                    || ((Math.Abs(move.From.File - move.To.File) <= 1)
                    && (Math.Abs(move.From.Rank - move.To.Rank) == 1)))
                {
                    return true;
                }

                if (move.IsCastlingShort
                    && Board.IsClearBetween(move.From, new Coords(7, move.From.Rank))
                    && ((ColorToMove == PieceColor.White && CastlingState[0])
                        || (ColorToMove == PieceColor.Black && CastlingState[2])))
                {
                    return true;
                }
                else if (move.IsCastlingLong
                    && Board.IsClearBetween(move.From, new Coords(0, move.From.Rank))
                    && ((ColorToMove == PieceColor.White && CastlingState[1])
                        || (ColorToMove == PieceColor.Black && CastlingState[3]))
                    )
                {
                    return true;
                }

                return false;

            case PieceKind.Rook:
                if (move.From.File == move.To.File || move.From.Rank == move.To.Rank)
                {
                    return Board.IsClearBetween(move.From, move.To);
                }
                break;

            case PieceKind.Bishop:
                if (Math.Abs(move.From.File - move.To.File) == Math.Abs(move.From.Rank - move.To.Rank))
                {
                    return Board.IsClearBetween(move.From, move.To);
                }
                break;

            case PieceKind.Queen:
                if (move.From.File == move.To.File || move.From.Rank == move.To.Rank || Math.Abs(move.From.File - move.To.File) == Math.Abs(move.From.Rank - move.To.Rank))
                {
                    return Board.IsClearBetween(move.From, move.To);
                }
                break;

            case PieceKind.Knight:
                if ((Math.Abs(move.From.File - move.To.File) == 1
                     && Math.Abs(move.From.Rank - move.To.Rank) == 2)
                    || (Math.Abs(move.From.File - move.To.File) == 2
                    && Math.Abs(move.From.Rank - move.To.Rank) == 1))
                {
                    return true;
                }
                return false;

            case PieceKind.Pawn:
                switch (piece.Color)
                {
                    case PieceColor.White:
                        if ((move.To.Rank - move.From.Rank == 1)
                            && ((move.From.File == move.To.File && Board.Squares[move.To.File, move.To.Rank] is null)
                                || (Math.Abs(move.From.File - move.To.File) == 1 && Board.Squares[move.To.File, move.To.Rank]?.Color == PieceColor.Black)))
                        {
                            return true;
                        }

                        if ((move.From.Rank == 1)
                            && (move.To.Rank == 3)
                            && (move.From.File == move.To.File)
                            && Board.Squares[move.From.File, 2] is null
                            && Board.Squares[move.From.File, 3] is null)
                        {
                            return true;
                        }

                        return false;

                    case PieceColor.Black:
                        if ((move.To.Rank - move.From.Rank == -1)
                            && ((move.From.File == move.To.File && Board.Squares[move.To.File, move.To.Rank] is null)
                                || (Math.Abs(move.From.File - move.To.File) == 1 && Board.Squares[move.To.File, move.To.Rank]?.Color == PieceColor.White)))
                        {
                            return true;
                        }
                        if ((move.From.Rank == 6)
                            && (move.To.Rank == 4)
                            && (move.From.File == move.To.File)
                            && Board.Squares[move.From.File, 5] is null
                            && Board.Squares[move.From.File, 4] is null)
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
