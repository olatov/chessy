namespace Chessy.Engine.Tests.PositionTests;

public sealed class MakeMoveTests
{
    private readonly Position _sut = new Position();

    public MakeMoveTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndPawnMovesOneSquare()
    {
        // Arrange
        var pawn = _sut.Board["e2"] = Piece.CreatePawn(PieceColor.White);
        var from = "e2";
        var to = "e3";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndPawnMovesOneSquare()
    {
        // Arrange
        var pawn = _sut.Board["e7"] = Piece.CreatePawn(PieceColor.Black);
        var from = "e7";
        var to = "e6";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndPawnMovesTwoSquares()
    {
        // Arrange
        var pawn = _sut.Board["e2"] = Piece.CreatePawn(PieceColor.White);
        var from = "e2";
        var to = "e4";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndPawnMovesTwoSquares()
    {
        // Arrange
        var pawn = _sut.Board["e7"] = Piece.CreatePawn(PieceColor.Black);
        var from = "e7";
        var to = "e5";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndPawnCaptures()
    {
        // Arrange
        var pawn = _sut.Board["e2"] = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = _sut.Board["d3"] = Piece.CreatePawn(PieceColor.Black);
        var from = "e2";
        var to = "d3";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndPawnCaptures()
    {
        // Arrange
        var pawn = _sut.Board["e7"] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board["f6"] = Piece.CreatePawn(PieceColor.White);
        var from = "e7";
        var to = "f6";
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndPawnCapturesEnPassant()
    {
        // Arrange
        var pawn = _sut.Board["e5"] = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = _sut.Board["d5"] = Piece.CreatePawn(PieceColor.Black);
        _sut.EnPassantTarget = Coords.Parse("d6");
        var from = "e5";
        var to = "d6";
        var move = Move.For(pawn, from, to);
        move.IsEnPassantCapture = true;

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
        _sut.Board["d5"].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndPawnCapturesEnPassant()
    {
        // Arrange
        var pawn = _sut.Board["e4"] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board["d4"] = Piece.CreatePawn(PieceColor.White);
        _sut.EnPassantTarget = Coords.Parse("d3");
        var from = "e4";
        var to = "d3";
        var move = Move.For(pawn, from, to);
        move.IsEnPassantCapture = true;

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
        _sut.Board["d4"].Should().BeNull();
    }

    [Theory]
    [InlineData(PieceKind.Queen)]
    [InlineData(PieceKind.Rook)]
    [InlineData(PieceKind.Bishop)]
    [InlineData(PieceKind.Knight)]
    public void ShouldUpdateBoard_WhenWhite_AndPawnPromotes(PieceKind promotionPieceKind)
    {
        // Arrange
        var pawn = _sut.Board["a7"] = Piece.CreatePawn(PieceColor.White);
        var from = "a7";
        var to = "a8";
        var move = Move.For(pawn, from, to, promotionPieceKind);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Kind.Should().Be(promotionPieceKind);
        _sut.Board[to].Color.Should().Be(PieceColor.White);
        _sut.Board[from].Should().BeNull();
    }


}