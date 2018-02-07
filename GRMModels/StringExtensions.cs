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
    }
}