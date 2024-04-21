using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using Chessy.Engine;
using Chessy.Engine.Pieces;

namespace Chessy.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.NativeAot80)]

public class GameBenchmarks
{
    private Game _sut = new();

    [GlobalSetup]
    public void Setup()
    {
        _sut.ResetToStartingPosition();
    }

    [Benchmark]
    public void GetMovesWithoutChecks()
    {
        _sut.GetMoves(PieceColor.White, skipChecks: true).ToArray();
    }

    [Benchmark]
    public void GetMovesWithChecks()
    {
        _sut.GetMoves(PieceColor.White, skipChecks: false).ToArray();
    }

    [Benchmark]
    public async Task FindBestMove()
    {
        var bestMove = await _sut.FindBestMoveABAsync(PieceColor.White, depth: 3);
    }

    [Benchmark]
    public void BoardValue()
    {
        var _ = _sut.Board.MaterialValue;
    }
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<GameBenchmarks>();
    }
}