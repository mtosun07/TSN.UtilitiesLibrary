namespace TSN.UtilitiesLibrary
{
    internal static class Shared
    {
        static Shared()
        {
            _random = new Lazy<Random>(() => new Random());
        }


        private static readonly Lazy<Random> _random;

        public static Random Random => _random.Value;
    }
}