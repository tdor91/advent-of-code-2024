namespace Common;

public static class MatrixExtensions
{
    public static char GetSymbol(this char[][] source, Point p) => source[p.Y][p.X];

    public static bool Contains(this char[][] source, Point p) =>
        p.X >= 0 &&
        p.Y >= 0 &&
        p.Y < source.Length &&
        p.X < source[p.Y].Length;

    public static IEnumerable<Point> Points(this char[][] source)
    {
        for (int y = 0; y < source.Length; y++)
        for (int x = 0; x < source[y].Length; x++)
            yield return new(x, y);
    }

    public static void Print(this char[][] source)
    {
        for (int y = 0; y < source.Length; y++)
        {
            for (int x = 0; x < source[y].Length; x++)
            {
                Console.Write(source[y][x]);
            }
            Console.WriteLine();
        }
    }

    public static IEnumerable<string> StringRows(this char[][] source)
    {
        return source.Select(arr => new string(arr));
    }

    public static IEnumerable<string> StringColumns(this char[][] source)
    {
        for (int col = 0; col < source[0].Length; col++)
        {
            var chars = Enumerable.Range(0, source.Length).Select(row => source[row][col]).ToArray();
            yield return new string(chars);
        }
    }
}