namespace Chessy.Engine.Tests.GameTests.GetMovesTests;

public sealed class KingTests
{
    private Game _sut = new();

    public KingTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void GetMoves_WhiteKingInCenterOfBoard_Returns8Moves()
    {
        // Arrange
        var king = _sut.Board["d4"] = Piece.CreateKing(PieceColor.White);
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
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().BeEquivalentTo(expectedMoves);
    }


    [Fact]
    public void GetMoves_WhiteKingCannotMoveIntoCheck_ReturnsCorrectMoves()
    {
        // Arrange
        _sut.Board["e4"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["d5"] = Piece.CreateQueen(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("Ke5");
    }

    [Fact]
    public void GetMoves_BlackKingCannotMoveIntoCheck_ReturnsCorrectMoves()
    {
        // Arrange
        _sut.Board["e4"] = Piece.CreateKing(PieceColor.Black);
        _sut.Board["d5"] = Piece.CreateQueen(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(PieceColor.Black);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("Ke5");
    }


    [Fact]
    public void GetMoves_WhiteKingCanCastleShort_WhenCastlingRightsAreSet()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["a1"] = null;
        _sut.Board["f1"] = null;
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = false,
            WhiteShort = true
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("O-O");
    }

    [Fact]
    public void GetMoves_WhiteKingCanCastleLong_WhenCastlingRightsAreSet()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["h1"] = null;
        _sut.Board["f1"] = null;
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = true,
            WhiteShort = false
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("O-O-O");
    }

    [Fact]
    public void GetMoves_WhiteKingCannotCastle_WhenCastlingRightsAreNotSet()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = false,
            WhiteShort = false
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O");
        notations.Should().NotContain("O-O-O");
    }


    [Fact]
    public void GetMoves_WhiteKingCannotCastle_WhenKingMoved()
    {
        // Arrange
        var king = _sut.Board["e2"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = true,
            WhiteShort = false
        };

        _sut.MakeMove(Move.For(king, Coords.Parse("e2"), Coords.Parse("e1")));

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O-O");
        notations.Should().NotContain("O-O");
    }


    [Fact]
    public void GetMoves_WhiteKingCannotCastle_WhenInCheck()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["e8"] = Piece.CreateQueen(PieceColor.Black);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = true,
            WhiteShort = true
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O");
        notations.Should().NotContain("O-O-O");
    }


    [Fact]
    public void GetMoves_WhiteKingCannotCastleShort_WhenFSquareAttacked()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["f1"] = null;
        _sut.Board["f8"] = Piece.CreateRook(PieceColor.Black);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = false,
            WhiteShort = true
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O");
    }


    [Fact]
    public void GetMoves_WhiteKingCannotCastleLong_WhenDSquareAttacked()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["d1"] = null;
        _sut.Board["d8"] = Piece.CreateRook(PieceColor.Black);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = true,
            WhiteShort = false
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O-O");
    }


    [Fact]
    public void GetMoves_WhiteKingCannotCastleLong_IntoCheck()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["a1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["c8"] = Piece.CreateQueen(PieceColor.Black);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = true,
            WhiteShort = false
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O-O");
    }


    [Fact]
    public void GetMoves_WhiteKingCannotCastleShort_IntoCheck()
    {
        // Arrange
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["h1"] = Piece.CreateRook(PieceColor.White);
        _sut.Board["f1"] = null;
        _sut.Board["f8"] = Piece.CreateQueen(PieceColor.Black);
        _sut.CastlingRights = new CastlingRights
        {
            WhiteLong = false,
            WhiteShort = true
        };

        // Act
        var moves = _sut.GetMoves(PieceColor.White);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().NotContain("O-O");
    }
}
