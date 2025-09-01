namespace TSN.UtilitiesLibrary
{
    public static class Delegates
    {
        public delegate T Parser<T>(string s);
        public delegate bool TryParser<T>(string s, out T value);
    }
}