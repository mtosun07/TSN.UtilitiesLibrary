using System.Numerics;
using System.Text;
using static TSN.UtilitiesLibrary.Delegates;

namespace TSN.UtilitiesLibrary
{
    public static class CryptographyExtensions
    {
        private const string _encodingSeperator = "###";
        private const string _encodingSaltFormat = "D";


        private static string Encode(this string s, Func<BigInteger, string> converter, Encoding encoding) => converter.Invoke(new BigInteger(encoding.GetBytes($"{Guid.NewGuid().ToString(_encodingSaltFormat)}{_encodingSeperator}{s}")));
        public static string EncodeBase36(this string s) => Encode(s, MathExtensions.ConvertToBase36, Encoding.UTF8);
        public static string EncodeBase100(this string s) => Encode(s, MathExtensions.ConvertToBase100, Encoding.UTF8);
        private static bool TryToDecode(this string s, TryParser<BigInteger> parser, Encoding encoding, out string decoded)
        {
            if (!parser.Invoke(s, out var i))
            {
                decoded = string.Empty;
                return false;
            }
            var splitted = encoding.GetString(i.ToByteArray()).Split([_encodingSeperator], StringSplitOptions.None);
            if (splitted.Length == 1 || !Guid.TryParseExact(splitted[0], _encodingSaltFormat, out _))
            {
                decoded = string.Empty;
                return false;
            }
            decoded = string.Join(string.Empty, splitted.Skip(1));
            return true;
        }
        public static bool TryToDecodeBase36(this string s, out string decoded) => TryToDecode(s, MathExtensions.TryParseFromBase36, Encoding.UTF8, out decoded);
        public static bool TryToDecodeBase100(this string s, out string decoded) => TryToDecode(s, MathExtensions.TryParseFromBase100, Encoding.UTF8, out decoded);
    }
}