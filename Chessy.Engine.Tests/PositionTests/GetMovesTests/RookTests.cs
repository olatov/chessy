namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public sealed class RookTests
{
    private Position _sut = new();

    public RookTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMove_WhenInCenter()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("d4"));
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
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("a1"));
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
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("a1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White }, Coords.Parse("e1"));
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
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("d4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("d5"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("d3"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("e4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("c4"));

        // Act
        var moves = _sut.GetMoves(rook.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == rook);
    }

    [Fact]
    public void ShouldMove_WhenCanCapture()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("d4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black }, Coords.Parse("d6"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black }, Coords.Parse("d2"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black }, Coords.Parse("a4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black }, Coords.Parse("f4"));
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
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("f4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.King, Color = PieceColor.Black }, Coords.Parse("d8"));

        // Act
        var moves = _sut.GetMoves(rook.Color, true, true);

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
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("d2"));
        _sut.AddPiece(new Piece { Kind = PieceKind.King, Color = PieceColor.White }, Coords.Parse("e1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Queen, Color = PieceColor.Black }, Coords.Parse("a5"));

        // Act
        var moves = _sut.GetMoves(rook.Color, true, true);

        // Assert
        moves.Should().NotContain(move => move.Piece == rook);
    }

    [Fact]
    public void CastlingShortRightRetains_WhenRookNotMoved_AndWhite()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("h1"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.White };
        _sut.AddPiece(king, Coords.Parse("e1"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("f2"));

        // Act
        _sut.MakeMove(new Move(pawn, Coords.Parse("f2"), Coords.Parse("f2")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteShort.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingShortRightRetains_WhenRookNotMoved_AndBlack()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black };
        _sut.AddPiece(rook, Coords.Parse("h8"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.Black };
        _sut.AddPiece(king, Coords.Parse("e8"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("f7"));

        // Act
        _sut.MakeMove(new Move(pawn, Coords.Parse("f7"), Coords.Parse("f6")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackShort.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingShortRightLost_WhenRookMoved_AndWhite()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("h1"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.White };
        _sut.AddPiece(king, Coords.Parse("e1"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("f2"));

        // Act
        _sut.MakeMove(new Move(rook, Coords.Parse("h1"), Coords.Parse("h2")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteShort.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingShortRightLost_WhenRookMoved_AndBlack()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black };
        _sut.AddPiece(rook, Coords.Parse("h8"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.Black };
        _sut.AddPiece(king, Coords.Parse("e8"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("f7"));

        // Act
        _sut.MakeMove(new Move(rook, Coords.Parse("h8"), Coords.Parse("h7")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackShort.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingShort);
    }

    [Fact]
    public void CastlingStateLongRightRetains_WhenRookNotMoved_AndWhite()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("a1"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.White };
        _sut.AddPiece(king, Coords.Parse("e1"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("b2"));

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
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black };
        _sut.AddPiece(rook, Coords.Parse("a8"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.Black };
        _sut.AddPiece(king, Coords.Parse("e8"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("b7"));

        // Act
        _sut.MakeMove(new Move(pawn, Coords.Parse("b7"), Coords.Parse("b6")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackLong.Should().BeTrue();
        moves.Should().Contain(move => move.IsCastlingLong);
    }

    [Fact]
    public void CastlingLongRightLost_WhenRookMoved_AndWhite()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.White };
        _sut.AddPiece(rook, Coords.Parse("a1"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.White };
        _sut.AddPiece(king, Coords.Parse("e1"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White };
        _sut.AddPiece(pawn, Coords.Parse("b2"));

        // Act
        _sut.MakeMove(new Move(rook, Coords.Parse("a1"), Coords.Parse("a2")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.WhiteLong.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingLong);
    }

    [Fact]
    public void CastlingLongRightLost_WhenRookMoved_AndBlack()
    {
        // Arrange
        var rook = new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black };
        _sut.AddPiece(rook, Coords.Parse("a8"));
        var king = new Piece { Kind = PieceKind.King, Color = PieceColor.Black };
        _sut.AddPiece(king, Coords.Parse("e8"));
        var pawn = new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black };
        _sut.AddPiece(pawn, Coords.Parse("b7"));

        // Act
        _sut.MakeMove(new Move(rook, Coords.Parse("a8"), Coords.Parse("a7")));
        var moves = _sut.GetMoves(king.Color);

        // Assert
        _sut.CastlingRights.BlackLong.Should().BeFalse();
        moves.Should().NotContain(move => move.IsCastlingLong);
    }
}