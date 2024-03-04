using System;
using Chessy.Engine;

namespace Chessy.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");

            var position = new Position();
            position.ResetToStartingPosition();

            //var moves = position.GetMoves().ToList();

            //System.Console.WriteLine(moves.Count);
            //foreach (var move in moves)
            //{
            //    System.Console.WriteLine(move.Notation);
            //}
        }
    }
}
