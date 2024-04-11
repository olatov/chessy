namespace Chessy.Engine;

public sealed record Coords(int File, int Rank)
{
    public static Coords Parse(string coords)
    {
        if (coords.Length != 2)
        {
            throw new ArgumentException("Coords must be 2 characters long");
        }

        var file = coords[0] switch
        {
            'a' => 0,
            'b' => 1,
            'c' => 2,
            'd' => 3,
            'e' => 4,
            'f' => 5,
            'g' => 6,
            'h' => 7,
            _ => throw new ArgumentException("Invalid file")
        };

        var rank = coords[1] switch
        {
            '1' => 0,
            '2' => 1,
            '3' => 2,
            '4' => 3,
            '5' => 4,
            '6' => 5,
            '7' => 6,
            '8' => 7,
            _ => throw new ArgumentException("Invalid rank")
        };

        return new Coords(file, rank);
    }

    public override string ToString() => $"{(char)('a' + File)}{Rank + 1}";
}