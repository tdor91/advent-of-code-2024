namespace Common;

public static class CharExtensions
{
    public static int ToInt(this char c) => (int)char.GetNumericValue(c);
}