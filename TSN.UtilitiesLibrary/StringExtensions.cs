using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TSN.UtilitiesLibrary.Exceptions;

namespace TSN.UtilitiesLibrary
{
    public static class StringExtensions
    {
        static StringExtensions()
        {
            _regexFalse = new("^(false|f|no|0)$");
            _regexTrue = new("^(true|t|yes|1)$");
            _regexWhiteSpace = new(@"\s");
            _regexEmailAddress = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            _regexIpAddress = new(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }


        private const string _randomStringCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string SpaceCharacter = " ";
        public const string SpaceCharacterHtml = "%20";
        
        private static readonly Regex _regexTrue;
        private static readonly Regex _regexFalse;
        private static readonly Regex _regexWhiteSpace;
        private static readonly Regex _regexIpAddress;
        private static readonly Regex _regexEmailAddress;



        public static string GenerateRandomString(int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length);
            if (length == 0)
                return string.Empty;
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str += _randomStringCharacters[Shared.Random.Next(_randomStringCharacters.Length)];
            return str;
        }
        public static string GenerateRandomString(int minLengthInclusive, int maxLengthInclusive) => GenerateRandomString(Shared.Random.Next(minLengthInclusive, maxLengthInclusive + 1));
        public static string GenerateRandomPassword(int minLengthInclusive, int maxLengthInclusive)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(minLengthInclusive, 4);
            const string alphaLower = "abcdefghijklmnopqrstuvwxyz"; // 45%
            const string alphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 25%
            const string numeric = "0123456789";                    // 20%
            const string special = @".+#$%&/*-,\!=@_|<>";           // 10%
            double length = Shared.Random.Next(minLengthInclusive, maxLengthInclusive + 1);
            char[] a = [.. alphaLower.Shuffle().Take((int)Math.Ceiling(length * 45D / 100D))];
            char[] b = [.. alphaUpper.Shuffle().Take((int)Math.Ceiling(length * 25D / 100D))];
            char[] c = [.. numeric.Shuffle().Take((int)Math.Ceiling(length * 20D / 100D))];
            char[] d = [.. special.Shuffle().Take((int)Math.Ceiling(length * 10D / 100D))];
            StringBuilder password = new();
            password.Append(a[0]);
            password.Append(b[0]);
            password.Append(c[0]);
            password.Append(d[0]);
            if (length > 4)
                password.Append(a.Skip(1).Union(b.Skip(1)).Union(c.Skip(1)).Union(d.Skip(1)).ToArray().ShuffleList().Take((int)length - 4));
            return password.ToString().Shuffle();
        }

        public static bool IsEmptyWhiteSpace(this string s) => s.Equals(_regexWhiteSpace.Replace(s, string.Empty));
        public static string Capitalize(this string s, CultureInfo culture)
        {
            StringBuilder sb = new();
            bool progressing = false;
            for (int i = 0; i < s.Length; i++)
                if (char.IsWhiteSpace(s[i]))
                {
                    progressing = false;
                    sb.Append(s[i]);
                }
                else if (!progressing)
                {
                    progressing = true;
                    sb.Append(char.ToUpper(s[i], culture));
                }
                else
                    sb.Append(char.ToLower(s[i], culture));
            return sb.ToString();
        }
        public static string CapitalizeInvariant(this string s) => Capitalize(s, CultureInfo.InvariantCulture);
        public static string Capitalize(this string s) => Capitalize(s, CultureInfo.CurrentCulture);
        public static string TrimDeeper(this string s) => string.Join(SpaceCharacter, _regexWhiteSpace.Split(s.Trim()).Where(x => !x.Equals(string.Empty)));
        public static StringStates TryToTrim(this string? s, out string? trimmed)
        {
            if (s is null)
            {
                trimmed = null;
                return StringStates.Null;
            }
            return (trimmed = s.Trim()).Equals(string.Empty) ? StringStates.Empty : StringStates.Valued;
        }
        public static StringStates TryToTrimDeeper(this string? s, out string? trimmed)
        {
            if (s is null)
            {
                trimmed = null;
                return StringStates.Null;
            }
            return (trimmed = s.TrimDeeper()).Equals(string.Empty) ? StringStates.Empty : StringStates.Valued;
        }
        public static StringStates GetState(this string? s, bool considerWhiteSpaces) => s is null ? StringStates.Null : ((considerWhiteSpaces ? s.Trim() : s).Equals(string.Empty) ? StringStates.Empty : StringStates.Valued);
        public static void ThrowExceptionByState(this StringStates state, string? argumentName = null)
        {
            switch (state)
            {
                case StringStates.Null:
                    throw new ArgumentNullException(argumentName);
                case StringStates.Empty:
                    throw new ArgumentEmptyException(argumentName);
                default:
                    break;
            }
        }
        public static string RemoveDiacritics(this string s)
        {
            Encoding srcEncoding = Encoding.UTF8;
            Encoding destEncoding = Encoding.GetEncoding(1252);
            string normalizedString = destEncoding.GetString(Encoding.Convert(srcEncoding, destEncoding, srcEncoding.GetBytes(s))).Normalize(NormalizationForm.FormD);
            StringBuilder result = new();
            for (int i = 0; i < normalizedString.Length; i++)
                if (!CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]).Equals(UnicodeCategory.NonSpacingMark))
                    result.Append(normalizedString[i]);
            return result.ToString();
        }
        public static string ToUpperLatin(this string s) => s.ToUpperInvariant().RemoveDiacritics();
        public static string ToLowerLatin(this string s) => s.ToLowerInvariant().RemoveDiacritics();

        public static string Shuffle(this string s) => new([.. s.ToCharArray().ShuffleList()]);
        public static string Shuffle(this string s, out IList<int> indices) => new([.. s.ToCharArray().ShuffleList(out indices)]);

        public static bool IsValidIpAddressV4(this string ipAddress)
        {
#if DEBUG
            return true;
#endif
            return _regexIpAddress.IsMatch(ipAddress);
        }
        public static bool IsValidEmailAddress(this string emailAddress) => _regexEmailAddress.IsMatch(emailAddress);

        public static bool TryParseToBool(this string s, out bool value)
        {
            string str = s.Trim().ToLowerInvariant();
            return (value = _regexTrue.IsMatch(str)) || _regexFalse.IsMatch(str);
        }
    }
}