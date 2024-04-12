namespace Chessy.Engine.Tests.PositionTests;

public sealed class UndoMoveTests
{
    private readonly Position _sut = new Position();

    public UndoMoveTests()
    {
        _sut.ResetToEmptyBoard();
    }
}
