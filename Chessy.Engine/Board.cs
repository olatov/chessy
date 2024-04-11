using Chessy.Engine.Extensions;
using Chessy.Engine.Pieces;

namespace Chessy.Engine;

public record Board
{
    public IPiece?[,] Squares = new IPiece?[8, 8];

    public bool IsClearBetween((int file, int rank) square1, (int file, int rank) square2)
    {
        if (square1.file == square2.file)
        {
            int file = square1.file;
            for (var rank = Math.Min(square1.rank, square2.rank) + 1;
                rank < Math.Max(square1.rank, square2.rank);
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
        else if (square1.rank == square2.rank)
        {
            // Vertical
            int rank = square1.rank;
            for (var file = Math.Min(square1.file, square2.file) + 1;
                file < Math.Max(square1.file, square2.file);
                file++)
            {
                if (Squares[file, rank] is not null)
                {
                    return false;
                }
            }

            return true;
        }
        else if (Math.Abs(square1.file - square2.file) == Math.Abs(square1.rank - square2.rank))
        {
            int fileSign = Math.Sign(square2.file - square1.file);
            int rankSign = Math.Sign(square2.rank - square1.rank);

            int rank = square1.rank + rankSign;
            int file = square1.file + fileSign;
            while (rank != square2.rank)
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
