using Chessy.Engine;
using Chessy.Engine.Pieces;

namespace Chessy.Console;

internal static class Program
{
    static void Main(string[] args)
    {
        var position = new Game();
        position.ResetToStartingPosition();

        var moves = position.GetMoves(PieceColor.White);

        System.Console.WriteLine("Valid moves: ");
        System.Console.WriteLine(string.Join(" ", moves.Select(x => x.ToString())));
    }
}

