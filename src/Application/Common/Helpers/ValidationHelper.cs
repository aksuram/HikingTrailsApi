using System.Globalization;

namespace HikingTrailsApi.Application.Common.Helpers
{
    public static class ValidationHelper
    {
        public static string AlphaRegex { get; } = "^[A-Za-z ĄČĘĖĮŠŲŪŽąčęėįšųūž]+$";
        public static string AlphaNumericRegex { get; } = "^[A-Za-z0-9 ĄČĘĖĮŠŲŪŽąčęėįšųūž]+$";
        public static string CapitalizedAlphaRegex { get; } = "^[A-ZĄČĘĖĮŠŲŪŽ][A-Za-z ĄČĘĖĮŠŲŪŽąčęėįšųūž]+$";
        public static string AlphaNumericSymbolRegex { get; } = @"^[A-Za-z0-9 ĄČĘĖĮŠŲŪŽąčęėįšųūž\-\._/,]+$";
        public static string UserNameRegex { get; } = @"^([A-Za-z0-9@\-\._])+$";


        public static bool IsInt(string value)
        {
            if (value == null) { return false; }

            return int.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.CurrentCulture, out _);
        }

        public static bool IsDouble(string value)
        {
            if (value == null) { return false; }

            return double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.CurrentCulture, out _);
        }
    }
}
