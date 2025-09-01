namespace TSN.UtilitiesLibrary
{
    public static class LinqExtensions
    {
        public static IList<T> ShuffleList<T>(this IList<T> collection)
        {
            var temp = collection.ToArray();
            temp.Shuffle();
            return temp;
        }
        public static IEnumerable<T> ShuffleList<T>(this IList<T> collection, out IList<int> indices)
        {
            if (collection == null)
            {
                indices = Array.Empty<int>();
                return [];
            }
            indices = Enumerable.Range(0, collection.Count).ToList();
            indices.Shuffle();
            return indices.Select(x => collection[x]);
        }
        public static IEnumerable<T> UndoShuffle<T>(this IEnumerable<T> collection, IList<int> indices) => collection.Select((x, i) => new { Index = indices[i], Char = x }).OrderBy(x => x.Index).Select(x => x.Char);
        public static IEnumerable<T[]> Split<T>(this IList<T> collection, int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length);
            var div = Math.DivRem(collection.Count, length, out var rem);
            for (int i = 0; i < div; i++)
            {
                var array = new T[length];
                for (int j = i * length, x = 0; x < length; j++)
                    array[x++] = collection[j];
                yield return array;
            }
            if (rem > 0)
            {
                var array = new T[rem];
                for (int i = div, x = 0; x < rem; i++)
                    array[x++] = collection[i];
                yield return array;
            }
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = [];
            foreach (var element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }
    }
}