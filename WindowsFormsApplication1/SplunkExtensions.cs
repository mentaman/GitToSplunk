using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public static class SplunkExtensions
    {
        public static string ToSplunkLog(this Dictionary<string, object> dictionary)
        {
            var str = "";
            foreach(var pair in dictionary)
            {
                str += $"{pair.Key}=\"{pair.Value.ToString().Replace("\"", "")}\" ";
            }
            return str;
        }
        public static string ToSplunkLogEndLine(this Dictionary<string, object> dictionary)
        {
            return dictionary.ToSplunkLog()+"\r\n";
        }
    }
}