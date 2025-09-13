namespace TSN.UtilitiesLibrary
{
    public sealed class CommonEqualityComparer<T> : IEqualityComparer<T>
    {
        static CommonEqualityComparer()
        {
            _default = new CommonEqualityComparer<T>((x, y) => ReferenceEquals(x, y) || (x is null ? y is null : (y is not null && x.Equals(y))));
        }
        public CommonEqualityComparer(Func<T?, T?, bool>? equalityComparer, Func<T, int>? hashCodeCalculator = null)
        {
            _equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            _hashCodeCalculator = hashCodeCalculator ?? new Func<T, int>(o => o?.GetHashCode() ?? 0);
        }


        private static readonly CommonEqualityComparer<T> _default;
        
        private readonly Func<T?, T?, bool> _equalityComparer;
        private readonly Func<T, int> _hashCodeCalculator;

        public static CommonEqualityComparer<T> Default => _default;



        public bool Equals(T? x, T? y) => _equalityComparer.Invoke(x, y);
        public int GetHashCode(T obj) => _hashCodeCalculator.Invoke(obj);
    }
}