namespace Chessy.Engine.Tests.PositionTests.GetMovesTests;

public sealed class BishopTests
{
    private Position _sut = new();

    public BishopTests()
    {
        _sut.ResetToEmptyBoard();
    }

    [Fact]
    public void ShouldMove_WhenInCenter()
    {
        // Arrange
        var bishop = new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White };
        _sut.AddPiece(bishop, Coords.Parse("d4"));
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
        var bishop = new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White };
        _sut.AddPiece(bishop, Coords.Parse("a1"));
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
        var bishop = new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White };
        _sut.AddPiece(bishop, Coords.Parse("a1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("e5"));
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
        var bishop = new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White };
        _sut.AddPiece(bishop, Coords.Parse("d1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("c2"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("e2"));

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == bishop);
    }

    [Fact]
    public void ShouldCapture_WhenOpponentPieceInDiagonal()
    {
        // Arrange
        var bishop = new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White };
        _sut.AddPiece(bishop, Coords.Parse("d4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("c3"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("c5"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.White }, Coords.Parse("e3"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Pawn, Color = PieceColor.Black }, Coords.Parse("g7"));
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
        var bishop = new Piece { Kind = PieceKind.Bishop, Color = PieceColor.White };
        _sut.AddPiece(bishop, Coords.Parse("e4"));
        _sut.AddPiece(new Piece { Kind = PieceKind.King, Color = PieceColor.White }, Coords.Parse("e1"));
        _sut.AddPiece(new Piece { Kind = PieceKind.Rook, Color = PieceColor.Black }, Coords.Parse("e8"));

        // Act
        var moves = _sut.GetMoves(bishop.Color);

        // Assert
        moves.Should().NotContain(move => move.Piece == bishop);
    }
}