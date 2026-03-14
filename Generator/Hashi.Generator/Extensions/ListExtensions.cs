namespace Hashi.Generator.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IList{T}" />.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    ///     Shuffles the elements of the list in place using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="collection">The list to shuffle.</param>
    public static void Shuffle<T>(this IList<T> collection)
    {
        for (var i = collection.Count - 1; i > 0; i--)
        {
            var j = Random.Shared.Next(i + 1);
            (collection[i], collection[j]) = (collection[j], collection[i]);
        }
    }
}
