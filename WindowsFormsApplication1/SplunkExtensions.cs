using System.Collections.Generic;
using System.Text;

namespace WindowsFormsApplication1
{
    public static class SplunkExtensions
    {
        public static string ToSplunkLog(this Dictionary<string, object> dictionary)
        {
            StringBuilder str = new StringBuilder();
            foreach(var pair in dictionary)
            {
                str.Append($"{pair.Key}=\"{pair.Value.ToString().Replace("\"", "")}\" ");
            }
            return str.ToString();
        }
        public static string ToSplunkLogEndLine(this Dictionary<string, object> dictionary)
        {
            return $"{dictionary.ToSplunkLog()}\r\n";
        }
    }
}