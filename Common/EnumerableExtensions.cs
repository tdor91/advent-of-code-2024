﻿namespace Common;

public static class EnumerableExtensions
{
    public static IEnumerable<(T A, T B)> Pairwise<T>(this IEnumerable<T> source)
    {
        return source.Zip(source.Skip(1));
    }

    public static IEnumerable<int> ToInts(this IEnumerable<string> source) => source.Select(int.Parse);
}