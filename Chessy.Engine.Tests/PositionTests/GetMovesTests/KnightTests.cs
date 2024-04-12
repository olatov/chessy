namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public sealed class KnightTests
{
    private Position _sut = new();

    public KnightTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMove_WhenInCenter()
    {
        // Arrange
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("c4"));
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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("a1"));
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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("a1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("b3"));
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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("h8"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("g7"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("h7"));
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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("a1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("b3"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("c2"));

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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("c4"));
        var enemyPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(enemyPawn, Coords.Parse("b6"));

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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("b5"));
        var enemyKing = new Piece { Kind = PieceKind.King, Color = PieceColor.Black };
        _sut.AddPiece(enemyKing, Coords.Parse("e8"));

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
        var knight = new Piece { Kind = PieceKind.Knight, Color = PieceColor.White };
        _sut.AddPiece(knight, Coords.Parse("e4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.King, Color = PieceColor.White }, Coords.Parse("e1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black }, Coords.Parse("e8"));

        // Act
        var moves = _sut.GetMoves(knight.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == knight);
    }
}