namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public sealed class PawnTests
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
    public void ShouldNotCaptureOwnPiece_WhenWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        var otherPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d5"));
        _sut.AddPiece(otherPawn, Coords.Parse("c6"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(moves => moves.CapturedPiece == otherPawn);
    }

    [Fact]
    public void ShouldNotCaptureOwnPiece_WhenBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        var otherPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e4"));
        _sut.AddPiece(otherPawn, Coords.Parse("d3"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(moves => moves.CapturedPiece == otherPawn);
    }

    [Fact]
    public void ShouldGiveCheck_WhenAttackingEnemyKing()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d6"));
        var enemyKing = new Piece { Kind = PieceKind.King, Color = PieceColor.Black };
        _sut.AddPiece(enemyKing, Coords.Parse("e8"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == pawn)
            .Select(move => move.GetNotationVariants().First());
        moves.Should().Contain(m => m.IsCheck);
        notations.Should().Contain("d7+");
    }

    [Fact]
    public void ShouldNotMove_WhenWouldPutOwnKingInCheck()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("e4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.King, Color = PieceColor.White }, Coords.Parse("b4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black }, Coords.Parse("h4"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == pawn);
    }

    [Fact]
    public void EnPassantTargetIsSet_WhenDoubleMove_AndWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d2"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        var move = moves.First(move => move.To == Coords.Parse("d4"));
        move.EnPassantTarget.Should().Be(Coords.Parse("d3"));
    }

    [Fact]
    public void EnPassantTargetIsSet_WhenDoubleMove_AndBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e7"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().Contain(m => m.IsPawnDoubleMove);

        var move = moves.First(move => move.To == Coords.Parse("e5"));
        move.EnPassantTarget.Should().Be(Coords.Parse("e6"));
    }

    [Fact]
    public void EnPassantTargetIsNotSet_WhenNotDoubleMove_AndWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d2"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        var move = moves.First(move => move.To == Coords.Parse("d3"));
        move.EnPassantTarget.Should().BeNull();
    }

    [Fact]
    public void EnPassantTargetIsNotSet_WhenNotDoubleMove_AndBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e7"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        var move = moves.First(move => move.To == Coords.Parse("e6"));
        move.EnPassantTarget.Should().BeNull();
    }

    [Fact] void EnPassantTargetIsSavedOnPosition_WhenDoubleMove_AndWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("d2"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);
        _sut.MakeMove(new Move(pawn, Coords.Parse("d2"), Coords.Parse("d4")));

        // Assert
        _sut.EnPassantTarget.Should().Be(Coords.Parse("d3"));
    }

    [Fact]
    void EnPassantTargetIsSavedOnPosition_WhenDoubleMove_AndBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("e7"));

        // Act
        var moves = _sut.GetMoves(pawn.Color);
        _sut.MakeMove(new Move(pawn, Coords.Parse("e7"), Coords.Parse("e5")));

        // Assert
        _sut.EnPassantTarget.Should().Be(Coords.Parse("e6"));
    }

    [Fact]
    void EnPassantIsResetAfterMove_WhenWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        var enemyPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("d2"));
        _sut.AddPiece(enemyPawn, Coords.Parse("e6"));

        // Act
        _sut.MakeMove(new Move(pawn, Coords.Parse("d2"), Coords.Parse("d4")));
        _sut.MakeMove(new Move(enemyPawn, Coords.Parse("e6"), Coords.Parse("e5")));
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        _sut.EnPassantTarget.Should().BeNull();
    }

    [Fact]
    void EnPassantIsResetAfterMove_WhenBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        var enemyPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("e7"));
        _sut.AddPiece(enemyPawn, Coords.Parse("d3"));

        // Act
        _sut.MakeMove(new Move(pawn, Coords.Parse("e7"), Coords.Parse("e5")));
        _sut.MakeMove(new Move(enemyPawn, Coords.Parse("d3"), Coords.Parse("d4")));
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        _sut.EnPassantTarget.Should().BeNull();
    }

    [Fact]
    public void ShouldCaptureEnPassant_WhenWhite()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        var enemyPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };

        _sut.AddPiece(pawn, Coords.Parse("d5"));
        _sut.AddPiece(enemyPawn, Coords.Parse("c7"));

        // Act
        _sut.MakeMove(new Move(enemyPawn, Coords.Parse("c7"), Coords.Parse("c5")));
        var moves = _sut.GetMoves(pawn.Color);

        var move = moves.First(m => m.GetNotationVariants().First() == "dxc6");
        _sut.MakeMove(move);

        // Assert
        move.Piece.Should().Be(pawn);
        move.IsEnPassantCapture.Should().BeTrue();
        move.CapturedPiece.Should().Be(enemyPawn);

        var enemyCoords = Coords.Parse("c5");
        _sut.Board.Squares[enemyCoords.File, enemyCoords.Rank].Should().BeNull();
    }

    [Fact]
    public void ShouldCaptureEnPassant_WhenBlack()
    {
        // Arrange
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        var enemyPawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };

        _sut.AddPiece(pawn, Coords.Parse("e4"));
        _sut.AddPiece(enemyPawn, Coords.Parse("d2"));

        // Act
        _sut.MakeMove(new Move(enemyPawn, Coords.Parse("d2"), Coords.Parse("d4")));
        var moves = _sut.GetMoves(pawn.Color);

        var move = moves.First(m => m.GetNotationVariants().First() == "exd3");
        _sut.MakeMove(move);

        // Assert
        move.Piece.Should().Be(pawn);
        move.IsEnPassantCapture.Should().BeTrue();
        move.CapturedPiece.Should().Be(enemyPawn);

        var enemyCoords = Coords.Parse("d4");
        _sut.Board.Squares[enemyCoords.File, enemyCoords.Rank].Should().BeNull();
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

    [Fact]
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