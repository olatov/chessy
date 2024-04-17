namespace Chessy.Engine.Tests.PositionTests;

public sealed class UndoMoveTests
{
    private readonly Position _sut = new Position();

    public UndoMoveTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndUndoPawnMovesOneSquare()
    {
        // Arrange
        var (from, to) = ("e2", "e3");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndUndoPawnMovesOneSquare()
    {
        // Arrange
        var (from, to) = ("e7", "e6");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndUndoPawnMovesTwoSquares()
    {
        // Arrange
        var (from, to) = ("e2", "e4");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndPawnMovesTwoSquares()
    {
        // Arrange
        var (from, to) = ("e7", "e5");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var move = Move.For(pawn, from, to);

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndUndoPawnCaptures()
    {
        // Arrange
        var (from, to) = ("e2", "d3");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = _sut.Board[to] = Piece.CreatePawn(PieceColor.Black);
        var move = Move.For(pawn, from, to);
        move.CapturedPiece = _sut.Board[to];

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().Be(enemyPawn);
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndUndoPawnCaptures()
    {
        // Arrange
        var (from, to) = ("e7", "d6");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board[to] = Piece.CreatePawn(PieceColor.White);
        var move = Move.For(pawn, from, to);
        move.CapturedPiece = _sut.Board[to];

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().Be(enemyPawn);
    }

    [Fact]
    public void ShouldUpdateBoard_WhenWhite_AndUndoPawnCapturesEnPassant()
    {
        // Arrange
        var (from, to) = ("e5", "d6");
        var enemyPawnCoords = "d5";
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = _sut.Board[enemyPawnCoords] = Piece.CreatePawn(PieceColor.Black);
        _sut.EnPassantTarget = Coords.Parse(to);
        var move = Move.For(pawn, from, to);
        move.CapturedPiece = _sut.Board[enemyPawnCoords];
        move.IsEnPassantCapture = true;

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().BeNull();
        _sut.Board[enemyPawnCoords].Should().Be(enemyPawn);
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndUndoPawnCapturesEnPassant()
    {
        // Arrange
        var (from, to) = ("e4", "d3");
        var enemyPawnCoords = "d4";
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board[enemyPawnCoords] = Piece.CreatePawn(PieceColor.White);
        _sut.EnPassantTarget = Coords.Parse(to);
        var move = Move.For(pawn, from, to);
        move.CapturedPiece = _sut.Board[enemyPawnCoords];
        move.IsEnPassantCapture = true;

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[to].Should().BeNull();
        _sut.Board[enemyPawnCoords].Should().Be(enemyPawn);
    }

    [Theory]
    [InlineData(PieceKind.Queen)]
    [InlineData(PieceKind.Rook)]
    [InlineData(PieceKind.Bishop)]
    [InlineData(PieceKind.Knight)]
    public void ShouldUpdateBoard_WhenWhite_AndUndoPawnPromotes(PieceKind promotionPieceKind)
    {
        // Arrange
        var (from, to) = ("a7", "a8");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var move = Move.For(pawn, from, to, promotionPieceKind);

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[from]!.Kind.Should().Be(PieceKind.Pawn);
        _sut.Board[from]!.Color.Should().Be(PieceColor.White);
        _sut.Board[to].Should().BeNull();
    }

    [Theory]
    [InlineData(PieceKind.Queen)]
    [InlineData(PieceKind.Rook)]
    [InlineData(PieceKind.Bishop)]
    [InlineData(PieceKind.Knight)]
    public void ShouldUpdateBoard_WhenBlack_AndUndoPawnPromotes(PieceKind promotionPieceKind)
    {
        // Arrange
        var (from, to) = ("a2", "a1");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var move = Move.For(pawn, from, to, promotionPieceKind);

        // Act
        _sut.MakeMove(move);
        _sut.UndoMove(move);

        // Assert
        _sut.Board[from].Should().Be(pawn);
        _sut.Board[from]!.Kind.Should().Be(PieceKind.Pawn);
        _sut.Board[from]!.Color.Should().Be(PieceColor.Black);
        _sut.Board[to].Should().BeNull();
    }
}
