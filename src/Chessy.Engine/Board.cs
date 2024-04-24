using Chessy.Engine.Extensions;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public record Board
{
    public IPiece?[,] Squares { get; set; } = new IPiece?[8, 8];

    public IPiece? this[string coords]
    {
        get => this[Coords.Parse(coords)];
        set => this[Coords.Parse(coords)] = value;
    }

    public IPiece? this[Coords coords]
    {
        get => Squares[coords.File, coords.Rank];
        set => Squares[coords.File, coords.Rank] = value;
    }

    private readonly double[,] _pieceBonusTable = new double[8, 8];

    private readonly double[,] _pawnBonusTable = new double[8, 8];

    public Board()
    {
        FillPeceBonusTable();
        FillPawnBounsTable();
    }

    public IEnumerable<Coords> GetPiecesCoords(PieceColor color)
    {
        foreach (var file in Enumerable.Range(0, 8))
        {
            foreach (var rank in Enumerable.Range(0, 8))
            {
                var piece = Squares[file, rank];
                if (piece is not null && piece.Color == color)
                {
                    yield return new Coords(file, rank);
                }
            }
        }
    }

    public static IEnumerable<Coords> GetAllSquares()
    {
        foreach (var file in Enumerable.Range(0, 8))
        {
            foreach (var rank in Enumerable.Range(0, 8))
            {
                yield return new Coords(file, rank);
            }
        }
    }

    public bool IsClearBetween(Coords square1, Coords square2)
    {
        if (square1.File == square2.File)
        {
            int file = square1.File;
            for (var rank = Math.Min(square1.Rank, square2.Rank) + 1;
                rank < Math.Max(square1.Rank, square2.Rank);
                rank++)
            {
                if (Squares[file, rank] is not null)
                {
                    return false;
                }
            }
            // Horizontal
            return true;
        }
        else if (square1.Rank == square2.Rank)
        {
            // Vertical
            int rank = square1.Rank;
            for (var file = Math.Min(square1.File, square2.File) + 1;
                file < Math.Max(square1.File, square2.File);
                file++)
            {
                if (Squares[file, rank] is not null)
                {
                    return false;
                }
            }

            return true;
        }
        else if (Math.Abs(square1.File - square2.File) == Math.Abs(square1.Rank - square2.Rank))
        {
            int fileSign = Math.Sign(square2.File - square1.File);
            int rankSign = Math.Sign(square2.Rank - square1.Rank);

            int rank = square1.Rank + rankSign;
            int file = square1.File + fileSign;
            while (rank != square2.Rank)
            {
                if (Squares[file, rank] is not null)
                {
                    return false;
                }
                file += fileSign;
                rank += rankSign;
            }

            // Diagonal
            return true;
        }

        // Invalid
        return false;
    }

    public double MaterialValue
    {
        get
        {
            double result = 0;

            foreach (var file in Enumerable.Range(0, 8))
            {
                foreach (var rank in Enumerable.Range(0, 8))
                {
                    var piece = Squares[file, rank];
                    if (piece is null) { continue; }

                    double value = piece.GetValue();
                    result += value;
                    int sign = Math.Sign(value);

                    if (piece.Kind != PieceKind.Pawn)
                    {
                        result += _pieceBonusTable[file, rank] * sign;
                    }
                    else
                    {
                        result += (piece.Color == PieceColor.White)
                            ? _pawnBonusTable[file, rank]
                            : -_pawnBonusTable[file, 7 - rank];
                    }
                }
            }

            return result;
        }
    }

    private void FillPeceBonusTable()
    {
        for (var file = 0; file < 8; file++)
        {
            for (var rank = 0; rank < 8; rank++)
            {
                _pieceBonusTable[file, rank] =
                    (8 - Math.Abs(3.5 - file) * 0.5 - Math.Abs(3.5 - rank) * 2) * 0.05
                    + (Random.Shared.NextDouble() * 0.01 - 0.005);
            }
        }
    }

    private void FillPawnBounsTable()
    {
        for (var rank = 0; rank < 8; rank++)
        {
            for (var file = 0; file < 8; file++)
            {
                _pawnBonusTable[file, rank] =
                    (4 - Math.Abs(3.5 - file) + rank * 3) * 0.02
                    + (Random.Shared.NextDouble() * 0.01 - 0.005);
            }
        }
    }
}
