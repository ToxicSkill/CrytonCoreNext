using System;
using System.Collections.Generic;
using System.Linq;

namespace CrytonCoreNext.PDF.Enums
{
    [Flags]
    public enum EPdfRequirements
    {
        Passed = 0,
        Password = 1 << 0,
        Opened = 1 << 2,
        Contains = 1 << 3,
        MoreThanOnePage = 1 << 4
    }

    public static class EPdfRequirementsExtensions
    {
        private static string ToDescriptionString(EPdfRequirements requirements)
        {
            return requirements switch
            {
                EPdfRequirements.Password => $"to has no password",
                EPdfRequirements.MoreThanOnePage => $"to has more than one page",
                EPdfRequirements.Opened => $"being opened",
                EPdfRequirements.Contains => $"not already being in collection",
                _ => string.Empty,
            };
        }

        public static string ToSentence(this EPdfRequirements requierements)
        {
            var values = requierements.GetIndividualFlags().Select(x => ToDescriptionString((EPdfRequirements)x));
            if (values.Count() > 1)
            {
                var joined = string.Join(", ", values);
                var index = joined.LastIndexOf(", ");
                return joined.Remove(index, 1).Insert(index, " and");
            }
            else if (values.Any())
            {
                return values.First().ToString();
            }
            return string.Empty;
        }
        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            ulong bits = Convert.ToUInt64(value);
            List<Enum> results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }

    }
}
