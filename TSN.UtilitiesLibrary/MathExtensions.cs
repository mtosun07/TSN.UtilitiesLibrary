using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using TSN.UtilitiesLibrary.Exceptions;

namespace TSN.UtilitiesLibrary
{
    public static class MathExtensions
    {
        static MathExtensions()
        {
            _base36CharIndices = new ReadOnlyDictionary<char, int>(_base36Chars.Select((x, i) => new { Char = x, Index = i }).ToDictionary(x => x.Char, x => x.Index));
            _base100CharIndices = new ReadOnlyDictionary<char, int>(_base100Chars.Select((x, i) => new { Char = x, Index = i }).ToDictionary(x => x.Char, x => x.Index));
        }


        private const char _base36NegativeSign = '_';
        private const char _base100NegativeSign = '¤';
        private const string _base36Chars = "gq906tjbk7nxepslyiawcvu8foh41d32r5zm"; // randomized 0-9 a-z
        private const string _base100Chars = "v=OşHBlhx¥D|ß#+{d3Nz£/4€[ğpPAg);i@9yK&c?E.çaVö*§}¢,M1]RUbts2W8~J-QmS%q:oLC6XkTurZ0(\\$n5fI^_wj!eü7FYG"; // randomized some of UTF8

        private static readonly ReadOnlyDictionary<char, int> _base36CharIndices;
        private static readonly ReadOnlyDictionary<char, int> _base100CharIndices;



        private static string ConvertToBase(string baseChars, char negativeSign, BigInteger i)
        {
            if (i.Sign == 0)
                return baseChars[0].ToString();
            var isNegative = i < BigInteger.Zero;
            var sb = new StringBuilder();
            if (isNegative)
                i = -i;
            while (i >= baseChars.Length)
            {
                i = BigInteger.DivRem(i, baseChars.Length, out var rem);
                sb.Insert(0, baseChars[(int)rem]);
            }
            sb.Insert(0, baseChars[(int)i]);
            if (isNegative)
                sb.Insert(0, negativeSign);
            var s = sb.ToString();
            return s[0] == baseChars[0] ? s.Substring(1) : s;
        }
        public static BigInteger ParseFromBase(IDictionary<char, int> indices, string baseChars, char negativeSign, string s)
        {
            if (s.Equals(string.Empty))
                throw new ArgumentEmptyException(nameof(s));
            var isNegative = s[0] == negativeSign;
            if (isNegative)
                s = s.Substring(1);
            var i = BigInteger.Zero;
            var pow = 0;
            foreach (var c in s.Reverse())
                i += (indices.TryGetValue(c, out var x) ? x : throw new FormatException()) * BigInteger.Pow(baseChars.Length, pow++);
            return isNegative ? -i : i;
        }

        public static string ConvertToBase36(this BigInteger i) => ConvertToBase(_base36Chars, _base36NegativeSign, i);
        public static BigInteger ParseFromBase36(string s) => ParseFromBase(_base36CharIndices, _base36Chars, _base36NegativeSign, s);
        public static bool TryConvertToBase36(this BigInteger i, out string s)
        {
            try
            {
                s = i.ConvertToBase36();
                return true;
            }
            catch
            {
                s = string.Empty;
                return false;
            }
        }
        public static bool TryParseFromBase36(string s, out BigInteger i)
        {
            try
            {
                i = ParseFromBase36(s);
                return true;
            }
            catch
            {
                i = BigInteger.Zero;
                return false;
            }
        }
        public static string ConvertToBase36(this long i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this ulong i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this int i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this uint i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this short i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this ushort i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this sbyte i) => ConvertToBase36((BigInteger)i);
        public static string ConvertToBase36(this byte i) => ConvertToBase36((BigInteger)i);
        public static bool TryConvertToBase36(this long i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this ulong i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this int i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this uint i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this short i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this ushort i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this sbyte i, out string s) => TryConvertToBase36((BigInteger)i, out s);
        public static bool TryConvertToBase36(this byte i, out string s) => TryConvertToBase36((BigInteger)i, out s);

        public static string ConvertToBase100(this BigInteger i) => ConvertToBase(_base100Chars, _base100NegativeSign, i);
        public static BigInteger ParseFromBase100(string s) => ParseFromBase(_base100CharIndices, _base100Chars, _base100NegativeSign, s);
        public static bool TryConvertToBase100(this BigInteger i, out string s)
        {
            try
            {
                s = i.ConvertToBase100();
                return true;
            }
            catch
            {
                s = string.Empty;
                return false;
            }
        }
        public static bool TryParseFromBase100(string s, out BigInteger i)
        {
            try
            {
                i = ParseFromBase100(s);
                return true;
            }
            catch
            {
                i = BigInteger.Zero;
                return false;
            }
        }
        public static string ConvertToBase100(this long i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this ulong i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this int i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this uint i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this short i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this ushort i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this sbyte i) => ConvertToBase100((BigInteger)i);
        public static string ConvertToBase100(this byte i) => ConvertToBase100((BigInteger)i);
        public static bool TryConvertToBase100(this long i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this ulong i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this int i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this uint i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this short i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this ushort i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this sbyte i, out string s) => TryConvertToBase100((BigInteger)i, out s);
        public static bool TryConvertToBase100(this byte i, out string s) => TryConvertToBase100((BigInteger)i, out s);
    }
}