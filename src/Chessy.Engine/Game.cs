using System.Diagnostics;
using Chessy.Engine.Events;
using Chessy.Engine.Extensions;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public sealed class Game
{
    public Player WhitePlayer { get; set; } = new() { Type = PlayerType.Human };

    public Player BlackPlayer { get; set; } = new() { Type = PlayerType.Human };

    public Board Board { get; set; } = new Board();

    public bool IsRemote => WhitePlayer.IsRemoteGuest || BlackPlayer.IsRemoteGuest;

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

    public CastlingRights CastlingRights { get; set; } = new();

    public Coords? EnPassantTarget { get; set; } = null;

    private long _nodesCounter { get; set; }

    public event EventHandler<FindMoveProgressEventArgs>? FindMoveProgress;

    private Stack<Coords?> EnPassantTargets { get; set; } = new();

    public static Game FromMoves(IEnumerable<Move> moves)
    {
        var result = new Game();
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
            Board.Squares[File, 0] = Piece.CreateRook(PieceColor.White);
            Board.Squares[File, 7] = Piece.CreateRook(PieceColor.Black);
        }

        foreach (int File in new[] { 1, 6 })
        {
            Board.Squares[File, 0] = Piece.CreateKnight(PieceColor.White);
            Board.Squares[File, 7] = Piece.CreateKnight(PieceColor.Black);
        }

        foreach (int File in new[] { 2, 5 })
        {
            Board.Squares[File, 0] = Piece.CreateBishop(PieceColor.White);
            Board.Squares[File, 7] = Piece.CreateBishop(PieceColor.Black);
        }

        Board.Squares[3, 0] = Piece.CreateQueen(PieceColor.White);
        Board.Squares[3, 7] = Piece.CreateQueen(PieceColor.Black);

        Board.Squares[4, 0] = Piece.CreateKing(PieceColor.White);
        Board.Squares[4, 7] = Piece.CreateKing(PieceColor.Black);

        foreach (int File in Enumerable.Range(0, 8))
        {
            Board.Squares[File, 1] = Piece.CreatePawn(PieceColor.White);
            Board.Squares[File, 6] = Piece.CreatePawn(PieceColor.Black);
        }

        ColorToMove = PieceColor.White;

        CastlingRights = new();
    }

    public void ResetToEmptyBoard()
    {
        Board = new Board();
        Moves = new List<Move>();
        ColorToMove = PieceColor.White;
        CastlingRights = new();
    }

    public void MakeMove(Move move)
    {
        Board.Squares[move.To.File, move.To.Rank] = move.Piece;
        Board.Squares[move.From.File, move.From.Rank] = null;

        if (move.IsPromotion)
        {
            // Trace.Assert(move.Piece.Kind == PieceKind.Pawn); // This breaks on position re-creation from moves
            Trace.Assert(move.PromotionPieceKind is not null);
            move.Piece.Kind = move.PromotionPieceKind.Value;
        }

        if (move.IsEnPassantCapture)
        {
            var rank = move.Piece.Color == PieceColor.White ? 4 : 3;
            Board.Squares[move.To.File, rank] = null;
        }

        EnPassantTargets.Push(EnPassantTarget);
        EnPassantTarget = move.EnPassantTarget;

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
                CastlingRights.WhiteShort = CastlingRights.WhiteLong = false;
            }
            else
            {
                CastlingRights.BlackShort = CastlingRights.BlackLong = false;
            }
        }
        else if (move.Piece.Kind == PieceKind.Rook)
        {
            if (move.Piece.Color == PieceColor.White)
            {
                if (move.From.File == 0)
                {
                    CastlingRights.WhiteLong = false;
                }
                else if (move.From.File == 7)
                {
                    CastlingRights.WhiteShort = false;
                }
            }
            else
            {
                if (move.From.File == 0)
                {
                    CastlingRights.BlackLong = false;
                }
                else if (move.From.File == 7)
                {
                    CastlingRights.BlackShort = false;
                }
            }
        }
    }

    public void UndoMove(Move move)
    {
        Board.Squares[move.From.File, move.From.Rank] = move.Piece;

        if (move.IsCapture)
        {
            if (move.IsEnPassantCapture)
            {
                var rank = move.Piece.Color == PieceColor.White ? 4 : 3;
                Board.Squares[move.To.File, move.To.Rank] = null;
                Board.Squares[move.To.File, rank] = move.CapturedPiece;
            }
            else
            {
                Board.Squares[move.To.File, move.To.Rank] = move.CapturedPiece;
            }
        }
        else
        {
            // TODO: Board[move.To] bugs out in wasm
            Board.Squares[move.To.File, move.To.Rank] = null;
        }

        EnPassantTarget = EnPassantTargets.Pop();

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
            CastlingRights.WhiteShort = move.CouldCastleShort;
            CastlingRights.WhiteLong = move.CouldCastleLong;
        }
        else
        {
            CastlingRights.BlackShort = move.CouldCastleShort;
            CastlingRights.BlackLong = move.CouldCastleLong;
        }
    }

    public async Task<Move?> FindBestMoveABAsync(
        PieceColor playerColor,
        int depth = 1,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Finding best move...");
        var moves = GetMoves(playerColor).ToArray();
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
            isTopLevel: true,
            cancellationToken:cancellationToken);
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

    public async Task<(Move? move, double score)> FindMoveABAsync(
        int depth,
        double alpha = double.MinValue,
        double beta = double.MaxValue,
        bool isMaximising = true,
        bool isTopLevel = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (depth == 0)
        {
            _nodesCounter++;
            return (null, isMaximising ? Board.MaterialValue : -Board.MaterialValue);
        }

        var moves = GetMoves(
            isMaximising ? PieceColor.White : PieceColor.Black,
            skipChecks: !isTopLevel);
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
            if (isTopLevel)
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
                    (_, moveScore) = await FindMoveABAsync(
                        depth - 1, -beta, -alpha, !isMaximising, isTopLevel: false,
                        cancellationToken: cancellationToken);
                    UndoMove(move);
                }
                moveScore = -moveScore;

                if (move.IsCastlingShort || move.IsCastlingLong)
                {
                    moveScore += 0.05;
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

    public IEnumerable<Coords> YieldPieceTargets(Coords from)
    {
        var piece = Board.Squares[from.File, from.Rank];
        Coords coords;

        if (piece.Kind == PieceKind.Pawn)
        {
            switch (piece.Color)
            {
                case PieceColor.White:
                    coords = new Coords(from.File, from.Rank + 1);
                    if (coords.IsValid && Board.Squares[coords.File, coords.Rank] is null)
                    {
                        yield return coords;
                        if (from.Rank == 1)
                        {
                            coords = new Coords(from.File, 3);
                            if (coords.IsValid && Board.Squares[coords.File, coords.Rank] is null)

                            {
                                yield return coords;
                            }
                        }
                    }

                    coords = new Coords(from.File - 1, from.Rank + 1);
                    if (coords.IsValid && Board.Squares[coords.File, coords.Rank]?.Color == piece.Color.OpponentColor())
                    {
                        yield return coords;
                    }
                    if (from.Rank == 4 && coords == EnPassantTarget)
                    {
                        yield return coords;
                    }

                    coords = new Coords(from.File + 1, from.Rank + 1);
                    if (coords.IsValid && Board.Squares[coords.File, coords.Rank]?.Color == piece.Color.OpponentColor())
                    {
                        yield return coords;
                    }
                    if (from.Rank == 4 && coords == EnPassantTarget)
                    {
                        yield return coords;
                    }

                    break;

                case PieceColor.Black:
                    coords = new Coords(from.File, from.Rank - 1);
                    if (coords.IsValid && Board.Squares[coords.File, coords.Rank] is null)
                    {
                        yield return coords;
                        if (from.Rank == 6)
                        {
                            coords = new Coords(from.File, 4);
                            if (Board.Squares[coords.File, coords.Rank] is null)
                            {
                                yield return coords;
                            }
                        }
                    }

                    coords = new Coords(from.File - 1, from.Rank - 1);
                    if (coords.IsValid && Board.Squares[coords.File, coords.Rank]?.Color == piece.Color.OpponentColor())
                    {
                        yield return coords;
                    }
                    if (from.Rank == 3 && coords == EnPassantTarget)
                    {
                        yield return coords;
                    }

                    coords = new Coords(from.File + 1, from.Rank - 1);
                    if (coords.IsValid && Board.Squares[coords.File, coords.Rank]?.Color == piece.Color.OpponentColor())
                    {
                        yield return coords;
                    }
                    if (from.Rank == 3 && coords == EnPassantTarget)
                    {
                        yield return coords;
                    }

                    break;
            }
            yield break;
        }

        if (piece.Kind == PieceKind.King)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) { continue; }
                    coords = new Coords(from.File + i, from.Rank + j);
                    if (coords.IsValid) { yield return coords; }
                }
            }

            if ((piece.Color == PieceColor.White && CastlingRights.WhiteLong)
                || (piece.Color == PieceColor.Black && CastlingRights.BlackLong))
            {
                yield return new Coords(2, from.Rank);
            }

            if ((piece.Color == PieceColor.White && CastlingRights.WhiteShort)
                || (piece.Color == PieceColor.Black && CastlingRights.BlackShort))
            {
                yield return new Coords(6, from.Rank);
            }
        }

        if (piece.Kind == PieceKind.Rook || piece.Kind == PieceKind.Queen)
        {
            foreach (var (i, j) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                coords = new Coords(from.File + i, from.Rank + j);
                while (coords.IsValid)
                {
                    if (Board.Squares[coords.File, coords.Rank]?.Color == piece.Color) { break; }
                    yield return coords;
                    if (Board.Squares[coords.File, coords.Rank] is not null) { break; }
                    coords = new Coords(coords.File + i, coords.Rank + j);
                }
            }
        }

        if (piece.Kind == PieceKind.Bishop || piece.Kind == PieceKind.Queen)
        {
            foreach (var (i, j) in new[] { (-1, -1), (-1, 1), (1, -1), (1, 1) })
            {
                coords = new Coords(from.File + i, from.Rank + j);
                while (coords.IsValid)
                {
                    if (Board.Squares[coords.File, coords.Rank]?.Color == piece.Color) { break; }
                    yield return coords;
                    if (Board.Squares[coords.File, coords.Rank] is not null) { break; }
                    coords = new Coords(coords.File + i, coords.Rank + j);
                }
            }
        }

        if (piece.Kind ==PieceKind.Knight)
        {
            foreach (var (i, j) in new[] { (-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1) })
            {
                coords = new Coords(from.File + i, from.Rank + j);
                if (coords.IsValid && Board.Squares[coords.File, coords.Rank]?.Color != piece.Color)
                {
                    yield return coords;
                }
            }
            yield break;
        }
    }


    public IEnumerable<Move> GetMoves(PieceColor playerColor, bool skipChecks = false)
    {
        // TODO: find a better approach

        foreach (Coords from in Board.GetPiecesCoords(playerColor))
        {
            var piece = Board.Squares[from.File, from.Rank];
            Trace.Assert(piece is not null);

            IEnumerable<Coords> targetSquares = YieldPieceTargets(from);
            foreach (Coords to in targetSquares)
            {
                if (from == to || Board.Squares[to.File, to.Rank]?.Color == playerColor) { continue; }

                IEnumerable<PieceKind?> promotionPieceKinds =
                    Board.Squares[from.File, from.Rank]?.Kind == PieceKind.Pawn && (to.Rank is 0 or 7)
                        ? [PieceKind.Queen, PieceKind.Rook, PieceKind.Bishop, PieceKind.Knight]
                        : new[] { null as PieceKind? };

                foreach (var promotionPieceKind in promotionPieceKinds)
                {
                    var move = new Move(
                        piece: Board.Squares[from.File, from.Rank]!,
                        from: from,
                        to: to)
                    {
                        CapturedPiece = Board[to],
                        PromotionPieceKind = promotionPieceKind
                    };

                    if (move.Piece.Kind == PieceKind.Pawn && move.To == EnPassantTarget)
                    {
                        move.IsEnPassantCapture = true;
                        var rank = playerColor == PieceColor.White ? 4 : 3;
                        move.CapturedPiece = Board.Squares[move.To.File, rank];
                    }

                    if (move.IsCastlingShort)
                    {
                        if (Board.Squares[5, from.Rank] is not null) { break; }
                        move.CastlingRook = Board.Squares[7, from.Rank];
                    }
                    else if (move.IsCastlingLong)
                    {
                        if (Board.Squares[3, from.Rank] is not null) { break; }
                        move.CastlingRook = Board.Squares[0, from.Rank];
                    }

                    if (playerColor == PieceColor.White)
                    {
                        move.CouldCastleShort = CastlingRights.WhiteShort;
                        move.CouldCastleLong = CastlingRights.WhiteLong;
                    }
                    else
                    {
                        move.CouldCastleShort = CastlingRights.BlackShort;
                        move.CouldCastleLong = CastlingRights.BlackLong;
                    }

                    if (!IsLegalMove(move)) { break; }

                    if (!skipChecks)
                    {
                        var opponentMoves = GetMoves(playerColor.OpponentColor(), true);
                        bool isUnderCheckNow = opponentMoves.Any(x => x.CapturedPiece?.Kind == PieceKind.King);

                        MakeMove(move);

                        opponentMoves = GetMoves(playerColor.OpponentColor(), true);
                        bool isUnderCheckAfterMove = opponentMoves.Any(x => x.CapturedPiece?.Kind == PieceKind.King);

                        if (isUnderCheckAfterMove
                            || (isUnderCheckNow && (move.IsCastlingShort || move.IsCastlingLong))
                            || move.IsCastlingShort && opponentMoves.Any(x => x.To == new Coords(5, move.To.Rank))
                            || move.IsCastlingLong && opponentMoves.Any(x => x.To == new Coords(3, move.To.Rank)))
                        {
                            UndoMove(move);
                            break;
                        }

                        var nextMoves = GetMoves(playerColor, true);
                        move.IsCheck = nextMoves.Any(x => x.CapturedPiece?.Kind == PieceKind.King);
                        if (move.IsCheck)
                        {
                            // TODO: fix this
                            bool opponentHasMoves = GetMoves(playerColor.OpponentColor(), skipChecks: false).Any();
                            if (!opponentHasMoves)
                            {
                                move.IsCheckmate = true;
                            }

                            // TODO: stalemate
                        }

                        UndoMove(move);


                    }

                    yield return move;
                }
            }
        }
    }

    public bool IsLegalMove(Move move)
    {
        // TODO: find a better approach

        var piece = Board.Squares[move.From.File, move.From.Rank];
        if (piece is null || piece != move.Piece) { return false; }

        var destinationSquare = Board.Squares[move.To.File, move.To.Rank];
        if (destinationSquare?.Color == piece.Color) { return false; }

        switch (piece.Kind)
        {
            case PieceKind.King:
                if ((Math.Abs(move.From.File - move.To.File) == 1
                        && Math.Abs(move.From.Rank - move.To.Rank) <= 1)
                    || (Math.Abs(move.From.File - move.To.File) <= 1
                        && Math.Abs(move.From.Rank - move.To.Rank) == 1))
                {
                    return true;
                }

                if (move.IsCastlingShort
                    && Board.IsClearBetween(move.From, new Coords(7, move.From.Rank))
                    && ((piece.Color == PieceColor.White && CastlingRights.WhiteShort)
                        || (piece.Color == PieceColor.Black && CastlingRights.BlackShort)))
                {
                    return true;
                }
                else if (move.IsCastlingLong
                    && Board.IsClearBetween(move.From, new Coords(0, move.From.Rank))
                    && ((piece.Color == PieceColor.White && CastlingRights.WhiteLong)
                        || (piece.Color == PieceColor.Black && CastlingRights.BlackLong)))
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
                        if (move.To.Rank - move.From.Rank == 1
                            && ((move.From.File == move.To.File && Board.Squares[move.To.File, move.To.Rank] is null)
                                || (Math.Abs(move.From.File - move.To.File) == 1
                                    && (Board.Squares[move.To.File, move.To.Rank]?.Color == PieceColor.Black
                                        || move.To == EnPassantTarget))))
                        {
                            return true;
                        }

                        if (move.From.Rank == 1
                            && move.To.Rank == 3
                            && move.From.File == move.To.File
                            && Board.Squares[move.From.File, 2] is null
                            && Board.Squares[move.From.File, 3] is null)
                        {
                            return true;
                        }

                        return false;

                    case PieceColor.Black:
                        if (move.To.Rank - move.From.Rank == -1
                            && ((move.From.File == move.To.File && Board.Squares[move.To.File, move.To.Rank] is null)
                                || (Math.Abs(move.From.File - move.To.File) == 1
                                    && (Board.Squares[move.To.File, move.To.Rank]?.Color == PieceColor.White
                                        || move.To == EnPassantTarget))))
                        {
                            return true;
                        }
                        if (move.From.Rank == 6
                            && move.To.Rank == 4
                            && move.From.File == move.To.File
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

    public string GetShortNotation(Move move)
    {
        var moves = GetMoves(move.Piece.Color);
        return move.GetNotationVariants()
            .FirstOrDefault(x => moves.Count(
                y => y.GetNotationVariants().Contains(x)) == 1) ?? string.Empty;

    }
}
