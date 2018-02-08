using System;
using System.Reflection;
using Humanizer;

namespace GRMModels
{
    public static class StringExtensions
    {
        public static string ReplaceTextInDate(this string date)
        {
            if (date.Contains("st"))
                return date.Replace("st", string.Empty);
            if (date.Contains("nd"))
                return date.Replace("nd", string.Empty);
            if (date.Contains("th"))
                return date.Replace("th", string.Empty);

            return date;
        }

        public static string ToDescription(this DistributionType en)
        {

            var type = en.GetType();

            var memInfo = type.GetMember(en.ToString());

            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(Description), false);

                if (attrs.Length > 0)
                    return ((Description)attrs[0]).Text;

            }

            return en.ToString();

        }


        public static string ToOrdinalDate(this DateTime date)
        {
            var day = date.Day.Ordinalize();

            return $"{day} {date:MMM yyyy}";
        }

    }
}