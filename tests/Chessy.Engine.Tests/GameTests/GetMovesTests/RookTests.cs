namespace Chessy.Engine.Tests.GameTests.GetMovesTests;

public sealed class RookTests
{
    private Game _sut = new();

    public RookTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMove_WhenInCenter()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["d4"] = rook;
        string[] expected = [
            "Rd1", "Rd2", "Rd3", "Rd5", "Rd6", "Rd7", "Rd8",
            "Ra4", "Rb4", "Rc4", "Re4", "Rf4", "Rg4", "Rh4"
        ];

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenInCorner()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["a1"] = rook;
        string[] expected = [
            "Ra2", "Ra3", "Ra4", "Ra5", "Ra6", "Ra7", "Ra8",
            "Rb1", "Rc1", "Rd1", "Re1", "Rf1", "Rg1", "Rh1"
        ];

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenInCornerAndPartiallyBlocked()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["a1"] = rook;
        _sut.Board["e1"] = Piece.CreateBishop(PieceColor.White);
        string[] expected = [
            "Ra2", "Ra3", "Ra4", "Ra5", "Ra6", "Ra7", "Ra8",
            "Rb1", "Rc1", "Rd1"
        ];

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == rook)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldNotMove_WhenBlocked()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["d4"] = rook;
        _sut.Board["d5"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["d3"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["e4"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["c4"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == rook);
    }

    [Fact]
    public void ShouldMove_WhenCanCapture()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["d4"] = rook;
        _sut.Board["d6"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["d2"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["a4"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["f4"] = Piece.CreatePawn(PieceColor.Black);
        string[] expected = [
            "Rxa4", "Rxd2", "Rd3", "Rd5", "Rxd6",
            "Rb4", "Rc4", "Re4", "Rxf4"
        ];

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldGiveCheck_WhenAttackingEnemyKing()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["f4"] = rook;
        _sut.Board["d8"] = Piece.CreateKing(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        moves.Should().Contain(move => move.IsCheck);
        var notations = moves.Where(move => move.IsCheck)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().Contain("Rd4+");
        notations.Should().Contain("Rf8+");
    }

    [Fact]
    public void ShouldNotMove_WhenWouldPutOwnKingInCheck()
    {
        // Arrange
        var rook = Piece.CreateRook(PieceColor.White);
        _sut.Board["d2"] = rook;
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a5"] = Piece.CreateQueen(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == rook);
    }

    [Fact]
    public void CastlingShortRightRetains_WhenRookNotMoved_AndWhite()
    {
        // Arrange
        var rook = _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        var king = _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        var pawn = _sut.Board["f2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        _sut.MakeMove(Move.For(pawn, "f2", "f3"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteShort.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingShortRightRetains_WhenRookNotMoved_AndBlack()
    {
        // Arrange

        var rook = _sut.Board["h8"] = Piece.CreateRook(PieceColor.Black);
        var king = _sut.Board["e8"] = Piece.CreateKing(PieceColor.Black);
        var pawn = _sut.Board["f7"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        _sut.MakeMove(Move.For(pawn, "f7", "f6"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackShort.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingShortRightLost_WhenRookMoved_AndWhite()
    {
        // Arrange
        var rook = _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        var king = _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        var pawn = _sut.Board["f2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        _sut.MakeMove(Move.For(rook, "h1", "h2"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteShort.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingShortRightLost_WhenRookMoved_AndBlack()
    {
        // Arrange
        var rook = _sut.Board["h8"] = Piece.CreateRook(PieceColor.Black);
        var king = _sut.Board["e8"] = Piece.CreateKing(PieceColor.Black);
        var pawn = _sut.Board["f7"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        _sut.MakeMove(Move.For(rook, "h8", "h7"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackShort.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingStateLongRightRetains_WhenRookNotMoved_AndWhite()
    {
        // Arrange
        var rook = _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        var king = _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        var pawn = _sut.Board["b2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        _sut.MakeMove(new Move(pawn, Coords.Parse("b2"), Coords.Parse("b3")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteLong.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingLong);
    }

    [Fact]
    public void CastlingLongRightRetains_WhenRookNotMoved_AndBlack()
    {
        // Arrange
        var rook = _sut.Board["a8"] = Piece.CreateRook(PieceColor.Black);
        var king = _sut.Board["e8"] = Piece.CreateKing(PieceColor.Black);
        var pawn = _sut.Board["b7"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        _sut.MakeMove(Move.For(pawn, "b7", "b6"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackLong.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingLong);
    }

    [Fact]
    public void CastlingLongRightLost_WhenRookMoved_AndWhite()
    {
        // Arrange
        var rook = _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        var king = _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        var pawn = _sut.Board["b2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        _sut.MakeMove(Move.For(rook, "a1", "a2"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteLong.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingLong);
    }

    [Fact]
    public void CastlingLongRightLost_WhenRookMoved_AndBlack()
    {
        // Arrange
        var rook = _sut.Board["a8"] = Piece.CreateRook(PieceColor.Black);
        var king = _sut.Board["e8"] = Piece.CreateKing(PieceColor.Black);
        var pawn = _sut.Board["b7"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        _sut.MakeMove(Move.For(rook, "a8", "a7"));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackLong.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingLong);
    }
}