using System.Net.NetworkInformation;
using System.Text.RegularExpressions;


namespace GetItDone.Utils
{
    public class TaskNormalizer
    {
        private static readonly Regex Ws = new(@"\s+", RegexOptions.Compiled);

        public static string? NormalizeTitle(string? title)
        {
            if (title == null) return null;
            return Ws.Replace(title.Trim(), " ");
        }
    }
}
