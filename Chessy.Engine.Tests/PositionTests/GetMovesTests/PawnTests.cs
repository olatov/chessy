namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public class PawnTests
{
    private Position _sut = new();

    public PawnTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMoveOneOrTwoSquaresForward_WhenWhite_AndOnSecondRank()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d2"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(2);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("d3");
        notations.Should().Contain("d4");
    }

    [Fact]
    public void ShouldMoveOneOrTwoSquaresForward_WhenBlack_AndOnSeventhRank()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e7"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(2);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("e6");
        notations.Should().Contain("e5");
    }

    [Fact]
    public void ShouldMoveOneSquareForward_WhenWhite_AndNotOnSecondRank()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d3"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(1);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("d4");
    }

    [Fact]
    public void ShouldMoveOneSquareForward_WhenBlack_AndNotOnSeventhRank()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e6"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(1);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("e5");
    }

    [Fact]
    public void ShouldNotMove_WhenWhite_AndBlockedByOwnPiece()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d2"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White },
            Coords.Parse("d3"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldNotMove_WhenBlack_AndBlockedByOwnPiece()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e7"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black },
            Coords.Parse("e6"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldNotMoveForward_WhenWhite_AndBlockedByEnemyPiece()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d2"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black },
            Coords.Parse("d3"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldNotMoveForward_WhenBlack_AndBlockedByEnemyPiece()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e7"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White },
            Coords.Parse("e6"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldCaptureDiagonally_WhenWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d5"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black },
            Coords.Parse("c6"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black },
            Coords.Parse("e6"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(3);

        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("dxc6");
        notations.Should().Contain("dxe6");
        notations.Should().Contain("d6");
    }

    [Fact]
    public void ShouldCaptureDiagonally_WhenBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e4"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White },
            Coords.Parse("d3"));
        _sut.AddPiece(
            new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White },
            Coords.Parse("f3"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(3);

        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("exd3");
        notations.Should().Contain("exf3");
        notations.Should().Contain("e3");
    }

    [Fact]
    public void ShouldPromote_WhenWhite_AndOnEighthRank()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d7"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(4);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("d8=Q");
        notations.Should().Contain("d8=R");
        notations.Should().Contain("d8=B");
        notations.Should().Contain("d8=N");
    }

    //[Fact]
    public void ShouldPromote_WhenBlack_AndOnFirstRank()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e2"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(4);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("e1=Q");
        notations.Should().Contain("e1=R");
        notations.Should().Contain("e1=B");
        notations.Should().Contain("e1=N");
    }
}