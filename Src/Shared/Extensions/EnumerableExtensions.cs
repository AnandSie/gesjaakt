namespace Extensions
{
    public static class EnumerableExtensions
    {
        private readonly static Random random = new();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(_ => random.Next());
        }
    }
}
