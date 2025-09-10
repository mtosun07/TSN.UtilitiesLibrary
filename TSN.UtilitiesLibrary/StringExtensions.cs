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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _latinLetters = new(() =>
            {
                HashSet<char> set = [];
                for (int i = 65; i <= 90; i++)
                    set.Add((char)i);
                for (int i = 97; i <= 122; i++)
                    set.Add((char)i);
                for (int i = 192; i <= 591; i++)
                    set.Add((char)i);
                return set;
            });
            _latinDigits = new(() => new HashSet<char>("0123456789"));
            _latinNonAlphanumerics = new(() =>
            {
                HashSet<char> set = [];
                char c;
                for (int i = 32; i <= 126; i++)
                    if (!char.IsWhiteSpace(c = (char)i) && !_latinLetters.Value.Contains(c) && !_latinDigits.Value.Contains(c))
                        set.Add(c);
                for (int i = 160; i <= 255; i++)
                    if (!char.IsWhiteSpace(c = (char)i) && !_latinLetters.Value.Contains(c) && !_latinDigits.Value.Contains(c))
                        set.Add(c);
                return set;
            });
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
        private static readonly Lazy<IReadOnlySet<char>> _latinLetters;
        private static readonly Lazy<IReadOnlySet<char>> _latinDigits;
        private static readonly Lazy<IReadOnlySet<char>> _latinNonAlphanumerics;

        public static IReadOnlySet<char> LatinLetters => _latinLetters.Value;
        public static Lazy<IReadOnlySet<char>> LatinDigits => _latinDigits;
        public static IReadOnlySet<char> LatinNonAlphaNumerics => _latinNonAlphanumerics.Value;



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
        private static IEnumerable<char> RemoveCharactersInternal(this IEnumerable<char> s, CharacterKinds canContain, IReadOnlySet<char>? letterCharSet = null, IReadOnlySet<char>? digitCharSet = null, IReadOnlySet<char>? otherCharSet = null)
        {
            /*
             * The if / else-if / else tree precomputes the character filter predicate
             * based on the flags before iteration, eliminating per-character conditional checks
             * and minimizing runtime overhead.
             */
            if (!canContain.IsFlagDefined())
                throw new ArgumentOutOfRangeException(nameof(canContain));
            if (canContain == CharacterKinds.None)
                return string.Empty;
            if (canContain == CharacterKinds.All)
                return s ?? string.Empty;
            Func<char, bool> isLetter = letterCharSet is null ? char.IsLetter : letterCharSet.Contains;
            Func<char, bool> isDigit = digitCharSet is null ? char.IsDigit : digitCharSet.Contains;
            Func<char, bool> predicate;
            if ((canContain & CharacterKinds.Letter) != 0)
            {
                if ((canContain & CharacterKinds.Digit) != 0)
                {
                    if ((canContain & CharacterKinds.Other) != 0)
                    {
                        if ((canContain & CharacterKinds.WhiteSpace) != 0)
                            throw new InvalidOperationException();
                        else if (otherCharSet is null)
                            predicate = x => !char.IsWhiteSpace(x);
                        else
                            predicate = otherCharSet.Contains;
                    }
                    else if ((canContain & CharacterKinds.WhiteSpace) != 0)
                        predicate = x => char.IsWhiteSpace(x) || isLetter(x) || isDigit(x);
                    else
                        predicate = x => isLetter(x) || isDigit(x);
                }
                else if ((canContain & CharacterKinds.Other) != 0)
                {
                    if ((canContain & CharacterKinds.WhiteSpace) != 0)
                    {
                        if (otherCharSet is null)
                            predicate = x => !isDigit(x);
                        else
                            predicate = x => isLetter(x) || char.IsWhiteSpace(x) || otherCharSet.Contains(x);
                    }
                    else if (otherCharSet is null)
                        predicate = x => !isDigit(x) && !char.IsWhiteSpace(x);
                    else
                        predicate = x => isLetter(x) || otherCharSet.Contains(x);
                }
                else if ((canContain & CharacterKinds.WhiteSpace) != 0)
                    predicate = x => char.IsWhiteSpace(x) || isLetter(x);
                else
                    predicate = isLetter;
            }
            else if ((canContain & CharacterKinds.Digit) != 0)
            {
                if ((canContain & CharacterKinds.Other) != 0)
                {
                    if ((canContain & CharacterKinds.WhiteSpace) != 0)
                    {
                        if (otherCharSet is null)
                            predicate = x => !isLetter(x);
                        else
                            predicate = x => isDigit(x) || char.IsWhiteSpace(x) || otherCharSet.Contains(x);
                    }
                    else if (otherCharSet is null)
                        predicate = x => !isLetter(x) && !char.IsWhiteSpace(x);
                    else
                        predicate = x => isDigit(x) || otherCharSet.Contains(x);
                }
                else if ((canContain & CharacterKinds.WhiteSpace) != 0)
                    predicate = x => char.IsWhiteSpace(x) || isDigit(x);
                else
                    predicate = isDigit;
            }
            else if ((canContain & CharacterKinds.Other) != 0)
            {
                if ((canContain & CharacterKinds.WhiteSpace) != 0)
                {
                    if (otherCharSet is null)
                        predicate = x => !isLetter(x) && !isDigit(x);
                    else
                        predicate = x => char.IsWhiteSpace(x) && otherCharSet.Contains(x);
                }
                else if (otherCharSet is null)
                    predicate = x => !isLetter(x) && !isDigit(x) && !char.IsWhiteSpace(x);
                else
                    predicate = otherCharSet.Contains;
            }
            else if ((canContain & CharacterKinds.WhiteSpace) != 0)
                predicate = char.IsWhiteSpace;
            else
                throw new InvalidOperationException();
            return s.Where(predicate);
        }
        public static string RemoveCharacters(this string s, CharacterKinds canContain)
        {
            StringBuilder sb = new();
            sb.Append([.. RemoveCharactersInternal(s, canContain)]);
            return sb.ToString();
        }
        public static string RemoveNonLatinChars(this string s, CharacterKinds canContain = CharacterKinds.All)
        {
            StringBuilder sb = new();
            sb.Append([.. RemoveCharactersInternal(s, canContain, _latinLetters.Value, _latinDigits.Value, _latinNonAlphanumerics.Value)]);
            return sb.ToString();
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