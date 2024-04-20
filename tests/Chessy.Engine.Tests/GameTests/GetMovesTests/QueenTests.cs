namespace Chessy.Engine.Tests.GameTests.GetMovesTests;

public sealed class QueenTests
{
    private Game _sut = new();

    public QueenTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void GetMoves_WhiteQueenInCenterOfBoard_Returns27Moves()
    {
        // Arrange
        var queen = _sut.Board["d4"] = Piece.CreateQueen(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        moves.Should().HaveCount(27);
    }

    // TODO: Add more tests
}