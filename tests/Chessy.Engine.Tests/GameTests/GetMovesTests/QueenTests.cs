namespace Chessy.Engine.Tests.GameTests.GetMovesTests;

public sealed class QueenTests
{
    private readonly Game _sut = new();

    public QueenTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void GetMoves_WhiteQueenInCenterOfBoard_Returns27Moves()
    {
        // Arrange
        var queen = _sut.Board["d4"] = Piece.CreateQueen(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        moves.Should().HaveCount(27);
    }


    [Fact]
    public void ShouldCaptureDiagonally_WhenWhite()
    {
        // Arrange
        var queen = _sut.Board["e5"] = Piece.CreateQueen(PieceColor.White);
        _sut.Board["d6"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["f6"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qxd6");
        notations.Should().Contain("Qxf6");
    }

    [Fact]
    public void ShouldCaptureDiagonally_WhenBlack()
    {
        // Arrange
        var queen = _sut.Board["e4"] = Piece.CreateQueen(PieceColor.Black);
        _sut.Board["d5"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["f5"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qxd5");
        notations.Should().Contain("Qxf5");
    }

    [Fact]
    public void ShouldCaptureAlongRows_WhenWhite()
    {
        // Arrange
        var queen = _sut.Board["e4"] = Piece.CreateQueen(PieceColor.White);
        _sut.Board["e5"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["e3"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qxe5");
        notations.Should().Contain("Qxe3");
    }

    [Fact]
    public void ShouldCaptureAlongRows_WhenBlack()
    {
        // Arrange
        var queen = _sut.Board["e4"] = Piece.CreateQueen(PieceColor.Black);
        _sut.Board["e5"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["e3"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qxe5");
        notations.Should().Contain("Qxe3");
    }

    [Fact]
    public void ShouldCaptureAlongFiles_WhenWhite()
    {
        // Arrange
        var queen = _sut.Board["d4"] = Piece.CreateQueen(PieceColor.White);
        _sut.Board["e4"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["c4"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qxe4");
        notations.Should().Contain("Qxc4");
    }

    [Fact]
    public void ShouldCaptureAlongFiles_WhenBlack()
    {
        // Arrange
        var queen = _sut.Board["d4"] = Piece.CreateQueen(PieceColor.Black);
        _sut.Board["e4"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["c4"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qxe4");
        notations.Should().Contain("Qxc4");
    }

    [Fact]
    public void ShouldGiveCheck_WhenWhite()
    {
        // Arrange
        var queen = _sut.Board["a1"] = Piece.CreateQueen(PieceColor.White);
        _sut.Board["e3"] = Piece.CreateKing(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qe1+");
        moves.Should().Contain(m => m.IsCheck);
    }

    [Fact]
    public void ShouldGiveCheck_WhenBlack()
    {
        // Arrange
        var queen = _sut.Board["a8"] = Piece.CreateQueen(PieceColor.Black);
        _sut.Board["e6"] = Piece.CreateKing(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qe8+");
        moves.Should().Contain(m => m.IsCheck);
    }

    [Fact]
    public void ShouldGiveCheckmate_WhenWhite()
    {
        // Arrange
        var queen = _sut.Board["a1"] = Piece.CreateQueen(PieceColor.White);
        _sut.Board["h8"] = Piece.CreateKing(PieceColor.Black);
        _sut.Board["g7"] = Piece.CreatePawn(PieceColor.Black);
        _sut.Board["h7"] = Piece.CreatePawn(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qa8#");
        moves.Should().Contain(m => m.IsCheckmate);
    }

    [Fact]
    public void ShouldGiveCheckmate_WhenBlack()
    {
        // Arrange
        var queen = _sut.Board["a8"] = Piece.CreateQueen(PieceColor.Black);
        _sut.Board["h1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["g2"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["h2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(queen.Color);

        // Assert
        var notations = moves.Select(m => m.GetNotationVariants()[0]);
        notations.Should().Contain("Qa1#");
        moves.Should().Contain(m => m.IsCheckmate);
    }
}
