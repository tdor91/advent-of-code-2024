namespace Common;

public readonly record struct Point(int X, int Y)
{
    public static Point Parse(string str)
    {
        var parts = str.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    public static Point operator *(Point a, int b) => new(a.X * b, a.Y * b);

    public Point Invert() => new(X * -1, Y * -1);

    public (int X, int Y) DistanceTo(Point other) => (Math.Abs(X - other.X), Math.Abs(Y - other.Y));

    public Point PathTo(Point other) => new (other.X - X, other.Y - Y);

    public Point CenterTo(Point other) => new((X + other.X) / 2, (Y + other.Y) / 2);

    public Point Add(int x, int y) => new(X + x, Y + y);
}