using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace TSN.UtilitiesLibrary
{
    public static class ObjectExtensions
    {
        public static bool EqualsDefault<T>(this T value) where T : struct => value.Equals(default(T));
        public static bool IsFlagDefined(this Enum e) => !decimal.TryParse(e.ToString(), out _);
        public static IEnumerable<Enum> ResolveFlags(this Enum e)
        {
            var t = e.GetType();
            var zero = Convert.ChangeType(0, Enum.GetUnderlyingType(t));
            return Enum.GetValues(t).Cast<Enum>().Where(x => !x.Equals(zero) && e.HasFlag(x));
        }
        public static string EnumToString(this BigInteger value, IList<(string Name, BigInteger Value)>? allValues, string seperator = ", ")
        {
            if (allValues is null || allValues.Count == 0)
                return value.ToString();
            var sep = seperator ?? string.Empty;
            var num = value;
            int num2 = allValues.Count - 1;
            var stringBuilder = new StringBuilder();
            bool flag = true;
            var num3 = num;
            while (num2 >= 0 && (num2 != 0 || allValues[num2].Value != BigInteger.Zero))
            {
                if ((num & allValues[num2].Value) == allValues[num2].Value)
                {
                    num -= allValues[num2].Value;
                    if (!flag)
                        stringBuilder.Insert(0, sep);
                    stringBuilder.Insert(0, allValues[num2].Name);
                    flag = false;
                }
                num2--;
            }
            if (num != BigInteger.Zero)
                return value.ToString();
            if (num3 == BigInteger.Zero)
            {
                if (allValues.Count != 0 && allValues[0].Value == BigInteger.Zero)
                    return allValues[0].Name;
                return "0";
            }
            return stringBuilder.ToString();
        }
        public static DisplayAttribute? GetDisplayAttribute(this Enum e) => e.GetType().GetMember(e.ToString()).FirstOrDefault()?.GetCustomAttributes(false)?.Select(x => x as DisplayAttribute).Where(x => x is not null).FirstOrDefault();

        public static int GetCombinedHashCode(params object?[]? collection)
        {
            if (collection is null)
                return 0;
            HashCode hash = new();
            foreach (var item in collection)
                hash.Add(item);
            return hash.ToHashCode();
        }

        public static bool TryParseToCultureInfo(string s, out CultureInfo? ci)
        {
            try
            {
                return (ci = CultureInfo.GetCultureInfo(s)) is not null;
            }
            catch (CultureNotFoundException)
            {
                ci = null;
                return false;
            }
        }
    }
}