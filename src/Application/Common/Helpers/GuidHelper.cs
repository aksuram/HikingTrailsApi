using System;

namespace HikingTrailsApi.Application.Common.Helpers
{
    public static class GuidHelper
    {
        private const int UrlFriendlyGuidLength = 34;

        public static string ToUrlFriendlyString(Guid guid)
        {
            var base64Guid = Convert.ToBase64String(guid.ToByteArray());

            return base64Guid
                .Replace("+", "-")
                .Replace("/", "_")
                .Substring(0, base64Guid.Length - 2);
        }

        public static Guid FromUrlFriendlyString(string guid)
        {
            if (guid == null || guid.Length != UrlFriendlyGuidLength) return new Guid();

            var byteArray = Convert.FromBase64String(
                guid.Replace("-", "+").Replace("_", "/") + "==");

            return new Guid(byteArray);
        }
    }
}
