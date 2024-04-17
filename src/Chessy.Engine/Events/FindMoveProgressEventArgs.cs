namespace Chessy.Engine.Events;

public class FindMoveProgressEventArgs : EventArgs
{
    public int Current { get; set; }

    public int Total { get; set;}     
}