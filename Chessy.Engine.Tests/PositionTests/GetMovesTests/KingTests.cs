namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public sealed class KingTests
{
    private Position _sut = new();

    public KingTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void GetMoves_WhiteKingInCenterOfBoard_Returns8Moves()
    {
        // Arrange
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.White };
        _sut.AddPiece(king, Coords.Parse("d4"));
        _sut.CastlingRights = new()
        {
            WhiteShort = false,
            WhiteLong = false,
            BlackShort = false,
            BlackLong = false
        };
        string[] expectedMoves = ["Kc3", "Kd3", "Ke3", "Kc4", "Ke4", "Kc5", "Kd5", "Ke5"];

        // Act
        var moves = _sut.GetMoves(king.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expectedMoves);
    }

    // TODO: Add more tests
}
