using System.Collections.ObjectModel;

namespace Hashi.Gui.Extensions;

public static class ObservableCollectionExtensions
{
    /// <summary>
    ///     Adds a range of items to the ObservableCollection.
    /// </summary>
    /// <typeparam name="T">The type of object to handle.</typeparam>
    /// <param name="collection">The collection to add the range to.</param>
    /// <param name="items">The items to add.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if collection or items are null.</exception>
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        foreach (var item in items) collection.Add(item);
    }

    /// <summary>
    ///    Removes all items that match the predicate from the ObservableCollection.
    /// </summary>
    /// <typeparam name="T">The type of object to handle.</typeparam>
    /// <param name="collection">The collection to add the range to.</param>
    /// <param name="predicate">The predicate to use.</param>
    /// <exception cref="ArgumentNullException">Throws an exception if collection or items are null.</exception>
    public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var itemsToRemove = collection.Where(predicate).ToList();
        foreach (var item in itemsToRemove)
        {
            collection.Remove(item);
        }
    }
}