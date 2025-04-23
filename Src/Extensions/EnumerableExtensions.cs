namespace Extensions
{
    public static class EnumerableExtensions
    {
        private static Random Random = new();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(_ => Random.Next());
        }
    }
}
