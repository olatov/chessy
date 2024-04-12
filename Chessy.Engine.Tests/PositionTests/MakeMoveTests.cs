namespace Chessy.Engine.Tests.PositionTests;

public sealed class MakeMoveTests
{
    private readonly Position _sut = new Position();

    public MakeMoveTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenPawnMovesOneSquare_AndWhite()
    {
        // Arrange
        var pawn = _sut.Board["e2"] = Piece.CreatePawn(PieceColor.White);
        var from = "e2";
        var to = "e3";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }
}