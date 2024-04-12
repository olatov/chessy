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
        var pawn = _sut.Board["d2"] = Piece.CreatePawn(PieceColor.White);

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
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e7")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d3")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e6")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d2")] = pawn;
        _sut.Board[Coords.Parse("d3")] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldNotMove_WhenBlack_AndBlockedByOwnPiece()
    {
        // Arrange
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e7")] = pawn;
        _sut.Board[Coords.Parse("e6")] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldNotMoveForward_WhenWhite_AndBlockedByEnemyPiece()
    {
        // Arrange
        var pawn = _sut.Board["d2"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["d3"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldNotMoveForward_WhenBlack_AndBlockedByEnemyPiece()
    {
        // Arrange
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e7")] = pawn;
        _sut.Board[Coords.Parse("e6")] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(m => m.Piece == pawn);
    }

    [Fact]
    public void ShouldCaptureDiagonally_WhenWhite()
    {
        // Arrange
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d5")] = pawn;
        _sut.Board[Coords.Parse("c6")] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e6")] = Piece.CreatePawn(PieceColor.Black);

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
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e4")] = pawn;
        _sut.Board[Coords.Parse("d3")] = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("f3")] = Piece.CreatePawn(PieceColor.White);

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
        var pawn = Piece.CreatePawn(PieceColor.White);
        var otherPawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d5")] = pawn;
        _sut.Board[Coords.Parse("c6")] = otherPawn;

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(moves => moves.CapturedPiece == otherPawn);
    }

    [Fact]
    public void ShouldNotCaptureOwnPiece_WhenBlack()
    {
        // Arrange
        var pawn = Piece.CreatePawn(PieceColor.Black);
        var otherPawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e4")] = pawn;
        _sut.Board[Coords.Parse("d3")] = otherPawn;

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(moves => moves.CapturedPiece == otherPawn);
    }

    [Fact]
    public void ShouldGiveCheck_WhenAttackingEnemyKing()
    {
        // Arrange
        var pawn = _sut.Board[Coords.Parse("d6")] = Piece.CreatePawn(PieceColor.White);
        var enemyKing = _sut.Board[Coords.Parse("e8")] = Piece.CreateKing(PieceColor.Black);

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
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("e4")] = pawn;
        _sut.Board[Coords.Parse("b4")] = Piece.CreateKing(PieceColor.White);
        _sut.Board[Coords.Parse("h4")] = Piece.CreateRook(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == pawn);
    }

    [Fact]
    public void EnPassantTargetIsSet_WhenDoubleMove_AndWhite()
    {
        // Arrange
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d2")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e7")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d2")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e7")] = pawn;

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        var move = moves.First(move => move.To == Coords.Parse("e6"));
        move.EnPassantTarget.Should().BeNull();
    }

    [Fact] void EnPassantTargetIsSavedOnPosition_WhenDoubleMove_AndWhite()
    {
        // Arrange
        var pawn = Piece.CreatePawn(PieceColor.White);
        _sut.Board[Coords.Parse("d2")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("e7")] = pawn;

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
        var pawn = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = Piece.CreatePawn(PieceColor.Black);
        _sut.Board[Coords.Parse("d2")] = pawn;
        _sut.Board[Coords.Parse("e6")] = enemyPawn;

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
        var pawn = _sut.Board[Coords.Parse("e7")] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board[Coords.Parse("d3")] = Piece.CreatePawn(PieceColor.White);

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
        var pawn = _sut.Board[Coords.Parse("d5")] = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = _sut.Board[Coords.Parse("c7")] = Piece.CreatePawn(PieceColor.Black);

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
        var pawn = _sut.Board[Coords.Parse("e4")] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board[Coords.Parse("d2")] = Piece.CreatePawn(PieceColor.White);

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
        var pawn = _sut.Board[Coords.Parse("d7")] = Piece.CreatePawn(PieceColor.White);
        var enemyKing = _sut.Board[Coords.Parse("a8")] = Piece.CreateKing(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(4);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("d8=Q+");
        notations.Should().Contain("d8=R+");
        notations.Should().Contain("d8=B");
        notations.Should().Contain("d8=N");
    }

    [Fact]
    public void ShouldPromote_WhenBlack_AndOnFirstRank()
    {
        // Arrange
        var pawn = _sut.Board[Coords.Parse("e2")] = Piece.CreatePawn(PieceColor.Black);
        var enemyKing = _sut.Board[Coords.Parse("h1")] = Piece.CreateKing(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(pawn.Color);

        // Assert
        moves.Should().HaveCount(4);
        var notations = moves.Select(m => m.GetNotationVariants().First());
        notations.Should().Contain("e1=Q+");
        notations.Should().Contain("e1=R+");
        notations.Should().Contain("e1=B");
        notations.Should().Contain("e1=N");
    }
}