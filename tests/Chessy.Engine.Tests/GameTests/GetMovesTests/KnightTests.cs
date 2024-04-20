namespace Chessy.Engine.Tests.GameTests.GetMovesTests;

public sealed class KnightTests
{
    private Game _sut = new();

    public KnightTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMove_WhenInCenter()
    {
        // Arrange
        var knight = _sut.Board["c4"] = Piece.CreateKnight(PieceColor.White);
        string[] expected = ["Na3", "Nb2", "Nb6", "Na5", "Ne3", "Nd2", "Nd6", "Ne5"];

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenInCorner()
    {
        // Arrange
        var knight = _sut.Board["a1"] = Piece.CreateKnight(PieceColor.White);
        string[] expected = ["Nb3", "Nc2"];

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenInCornerAndPartiallyBlocked()
    {
        // Arrange
        var knight = _sut.Board["a1"] = Piece.CreateKnight(PieceColor.White);
        _sut.Board["b3"] = Piece.CreatePawn(PieceColor.White);
        string[] expected = ["Nc2"];

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == knight)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenBehindPieces()
    {
        // Arrange
        var knight = _sut.Board["h8"] = Piece.CreateKnight(PieceColor.White);
        _sut.Board["g7"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["h7"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["g8"] = Piece.CreateKing(PieceColor.White);
        string[] expected = ["Nf7", "Ng6"];

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == knight)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldNotMove_WhenBlocked()
    {
        // Arrange
        var knight = _sut.Board["a1"] = Piece.CreateKnight(PieceColor.White);
        _sut.Board["b3"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["c2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == knight)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().BeEmpty();
    }

    [Fact]
    public void ShouldCapture_WhenEnemyPieceIsInDestination()
    {
        // Arrange
        var knight = _sut.Board["c4"] = Piece.CreateKnight(PieceColor.White);
        var enemyPawn = _sut.Board["b6"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == knight)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().Contain("Nxb6");
    }

    [Fact]
    public void ShouldGiveCheck_WhenAttackingEnemyKing()
    {
        // Arrange
        var knight = _sut.Board["b5"] = Piece.CreateKnight(PieceColor.White);
        var enemyKing = _sut.Board["e8"] = Piece.CreateKing(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == knight)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().Contain("Nc7+");
        notations.Should().Contain("Nd6+");
    }

    [Fact]
    public void ShouldNotMove_WhenWouldPutOwnKingInCheck()
    {
        // Arrange
        var knight = _sut.Board["e4"] = Piece.CreateKnight(PieceColor.White);
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["e8"] = Piece.CreateRook(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == knight);
    }
}