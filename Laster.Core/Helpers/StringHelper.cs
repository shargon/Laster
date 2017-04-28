using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Web;

namespace Laster.Core.Helpers
{
    public class StringHelper
    {
        public static bool LikeString(string input, string pattern)
        {
            if (input == pattern) return true;
            if (string.IsNullOrEmpty(pattern)) return false;

            return Operators.LikeString(input, pattern, CompareMethod.Text);
        }

        public static void Split(string palabra, char sep, out string izq, out string dr)
        {
            if (string.IsNullOrEmpty(palabra)) { izq = ""; dr = ""; return; }
            int fi = palabra.IndexOf(sep);
            if (fi == -1) { izq = palabra; dr = ""; return; }

            izq = palabra.Substring(0, fi);
            dr = palabra.Substring(fi + 1, palabra.Length - fi - 1);
        }

        public static string GetTextFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var v = doc.DocumentNode.SelectSingleNode("//body");
            if (v != null) return HttpUtility.HtmlDecode(v.InnerText);

            return HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
        }
    }
}