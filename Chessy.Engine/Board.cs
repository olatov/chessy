using Chessy.Engine.Extensions;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public record Board
{
    public IPiece?[,] Squares = new IPiece?[8, 8];

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
                    if (piece is not null)
                    {
                        double value = piece.GetValue();
                        result += value; // + (8 - Math.Abs(4 - file) - Math.Abs(4 - rank)) * 0.01 * Math.Sign(value);
                        result += (4 - Math.Abs(4 - file)) * 0.002 * Math.Sign(value);
                        result += (4 - Math.Abs(4 - rank)) * 0.002 * Math.Sign(value);
                        //result += value + (8.0 - Math.Abs(3.5 - file) - Math.Abs(3.5 - rank)) * 0.001 * Math.Sign(value);
                        if (piece.Kind == PieceKind.Pawn)
                        {
                            result += (piece.Color == PieceColor.White)
                                ? rank * 0.01 * 0.01 * (3.5 - Math.Abs(3.5 - file))
                                : (rank - 7) * 0.01 * (3.5 - Math.Abs(3.5 - file));
                            //result += Math.Abs(3.5 - rank);
                        }
                    }
                }
            }

            return result;
        }
    }
}
