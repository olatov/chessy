namespace Chessy.Engine.Tests.GameTests.GetMovesTests;

public sealed class BishopTests
{
    private Game _sut = new();

    public BishopTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMove_WhenInCenter()
    {
        // Arrange
        var bishop = _sut.Board["d4"] = Piece.CreateBishop(PieceColor.White);
        string[] expected = [
            "Ba1", "Bb2", "Bc3", "Be5", "Bf6", "Bg7", "Bh8",
            "Bc5", "Bb6", "Ba7", "Be3", "Bf2", "Bg1",
        ];

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenInCorner()
    {
        // Arrange
        var bishop = _sut.Board["a1"] = Piece.CreateBishop(PieceColor.White);
        string[] expected = ["Bb2", "Bc3", "Bd4", "Be5", "Bf6", "Bg7", "Bh8"];

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        var notations = moves.Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldMove_WhenInCornerAndPartiallyBlocked()
    {
        // Arrange
        var bishop = _sut.Board["a1"] = Piece.CreateBishop(PieceColor.White);
        _sut.Board["e5"] = Piece.CreatePawn(PieceColor.White);
        string[] expected = ["Bb2", "Bc3", "Bd4"];

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == bishop)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldNotMove_WhenBlocked()
    {
        // Arrange
        var bishop = _sut.Board["d1"] = Piece.CreateBishop(PieceColor.White);
        _sut.Board["c2"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["e2"] = Piece.CreatePawn(PieceColor.White);

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == bishop);
    }

    [Fact]
    public void ShouldCapture_WhenOpponentPieceInDiagonal()
    {
        // Arrange
        var bishop = _sut.Board["d4"] = Piece.CreateBishop(PieceColor.White);
        _sut.Board["c3"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["c5"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["e3"] = Piece.CreatePawn(PieceColor.White);
        _sut.Board["g7"] = Piece.CreatePawn(PieceColor.Black);
        string[] expected = ["Be5", "Bf6", "Bxg7"];

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        var notations = moves.Where(move => move.Piece == bishop)
            .Select(move => move.GetNotationVariants().First());
        notations.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldNotMove_WhenWouldPutOwnKingInCheck()
    {
        // Arrange
        var bishop = _sut.Board["e4"] = Piece.CreateBishop(PieceColor.White);
        _sut.Board["e1"] = Piece.CreateKing(PieceColor.White);
        _sut.Board["e8"] = Piece.CreateRook(PieceColor.Black);

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == bishop);
    }
}