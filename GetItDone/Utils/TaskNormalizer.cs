using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;


namespace GetItDone.Utils
{
    public class TaskNormalizer
    {
        private static readonly Regex Ws = new(@"\s+", RegexOptions.Compiled);

        public static string? NormalizeTitle(string? title)
        {
            if (title == null) return null;

            // remove white space
            title = Ws.Replace(title.Trim(), " ");

            // unicode normalization
            title = title.Normalize(NormalizationForm.FormC);

            // remove invisible control characters to prevent bugs
            title = Regex.Replace(title, @"[\p{Cc}\p{Cf}]", "");

            return title;
        }
    }
}
