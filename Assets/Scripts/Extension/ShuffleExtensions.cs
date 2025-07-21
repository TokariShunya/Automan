using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

/// <summary>
/// シーケンスの拡張
/// </summary>
public static class ShuffleExtensions
{
    /// <summary>
    /// シーケンスをシャッフルしたものを返す
    /// </summary>
    public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
    {
        TSource[] array = source.ToArray();

        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = Random.Range(0, i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }

        return array;
    }
}