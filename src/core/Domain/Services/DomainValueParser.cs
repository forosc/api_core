using System.Text.RegularExpressions;
using System.Globalization;

namespace Domain.Services
{
    public static class DomainValueParser
    {
        private static readonly Regex DateRegex =
            new(@"^\d{1,2}([./-])\d{1,2}\1\d{2,4}$", RegexOptions.Compiled);

        private static readonly Regex NumberRegex =
            new(@"^-?[\d,]+([.,]\d+)?-?$", RegexOptions.Compiled);

        public static bool IsDate(string? value)
            => value != null && DateRegex.IsMatch(value);

        public static bool IsNumber(string? value)
            => value != null && NumberRegex.IsMatch(value);

        public static string NormalizeDate(string raw)
        {
            raw = raw.Replace(".", "/").Replace("-", "/");
            var parts = raw.Split('/');

            // dd/MM/yyyy
            int day = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int year = int.Parse(parts[2]);

            if (year < 100)
                year += 2000;

            return new DateTime(year, month, day)
                .ToString("yyyy-MM-dd");
        }

        public static string NormalizeNumber(string raw)
        {
            raw = raw.Replace(",", "").Replace(" ", "");
            return decimal.Parse(raw, CultureInfo.InvariantCulture)
                         .ToString(CultureInfo.InvariantCulture);
        }
    }
}

