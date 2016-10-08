using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public static class SplunkExtensions
    {
        private const string DATE_FORMAT = "MM/dd/yyyy HH:mm:ss";

        public static string ToSplunkLog(this IDictionary<string, object> dictionary)
        {
            StringBuilder str = new StringBuilder();
            foreach(var pair in dictionary)
            {
                str.Append($"{pair.Key}=\"{pair.Value.ToString().Replace("\"", "")}\" ");
            }
            return str.ToString();
        }

        public static string ToSplunkLogEndLine(this IDictionary<string, object> dictionary, DateTimeOffset time)
        {
            return $"{time.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)} {dictionary.ToSplunkLog()}\r\n";
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB)
    where TValue : class
        {
            return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }
    }
}