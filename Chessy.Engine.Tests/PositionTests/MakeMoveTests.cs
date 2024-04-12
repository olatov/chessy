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
        var (from, to) = ("e2", "e3");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
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
        var (from, to) = ("e7", "e6");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
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
        var (from, to) = ("e2", "e4");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
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
        var (from, to) = ("e7", "e5");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
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
        var (from, to) = ("e2", "d3");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var enemyPawn = _sut.Board[to] = Piece.CreatePawn(PieceColor.Black);
        var move = Move.For(pawn, from, to);
        move.CapturedPiece = _sut.Board[to];

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
        var (from, to) = ("e7", "d6");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var enemyPawn = _sut.Board[to] = Piece.CreatePawn(PieceColor.White);
        var move = Move.For(pawn, from, to);
        move.CapturedPiece = _sut.Board[to];

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

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
        _sut.Board[enemyPawnCoords].Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateBoard_WhenBlack_AndPawnCapturesEnPassant()
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

        // Assert
        _sut.Board[to].Should().Be(pawn);
        _sut.Board[from].Should().BeNull();
        _sut.Board[enemyPawnCoords].Should().BeNull();
    }

    [Theory]
    [InlineData(PieceKind.Queen)]
    [InlineData(PieceKind.Rook)]
    [InlineData(PieceKind.Bishop)]
    [InlineData(PieceKind.Knight)]
    public void ShouldUpdateBoard_WhenWhite_AndPawnPromotes(PieceKind promotionPieceKind)
    {
        // Arrange
        var (from, to) = ("a7", "a8");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.White);
        var move = Move.For(pawn, from, to, promotionPieceKind);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().NotBeNull();
        _sut.Board[to]!.Kind.Should().Be(promotionPieceKind);
        _sut.Board[to]!.Color.Should().Be(PieceColor.White);
        _sut.Board[from].Should().BeNull();
    }

    [Theory]
    [InlineData(PieceKind.Queen)]
    [InlineData(PieceKind.Rook)]
    [InlineData(PieceKind.Bishop)]
    [InlineData(PieceKind.Knight)]
    public void ShouldUpdateBoard_WhenBlack_AndPawnPromotes(PieceKind promotionPieceKind)
    {
        // Arrange
        var (from, to) = ("a2", "a1");
        var pawn = _sut.Board[from] = Piece.CreatePawn(PieceColor.Black);
        var move = Move.For(pawn, from, to, promotionPieceKind);

        // Act
        _sut.MakeMove(move);

        // Assert
        _sut.Board[to].Should().NotBeNull();
        _sut.Board[to]!.Kind.Should().Be(promotionPieceKind);
        _sut.Board[to]!.Color.Should().Be(PieceColor.Black);
        _sut.Board[from].Should().BeNull();
    }
}