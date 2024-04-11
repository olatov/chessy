namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public sealed class QueenTests
{
    private Position _sut = new();

    public QueenTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void GetMoves_WhiteQueenInCenterOfBoard_Returns27Moves()
    {
        // Arrange
        var queen = new Piece { Kind = PieceKind.Queen, Color = PieceColor.White };
        _sut.AddPiece(queen, Coords.Parse("d4"));

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        moves.Should().HaveCount(27);
    }

    // TODO: Add more tests
}