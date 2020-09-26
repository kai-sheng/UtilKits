using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilKits.Serializer
{
    public class INIHelper : ISerializer
    {
        string pattern = @"
            ^(?:\[)                  # Section Start
                (?<Section>[^\]]*)   # Actual Section text into Section Group
             (?:\])                  # Section End then EOL/EOB
                 (?:[\r\n]{0,2}|\Z)  # Match but don't capture the CRLF or EOB
             (?<KVPs>                # Begin working on the Key Value Pairs
               (?!\[)                # Stop if a [ is found
               (?<Key>[^=]*?)        # Any text before the =, matched few as possible
                  (?:=)              # Anchor the = now but don't capture it
               (?<Value>[^=\[]*)     # Get everything that is not an =
               (?=^[^=]*?=|\Z|\[)    # Look Ahead, Capture but don't consume
             )+                      # Multiple values
        ";

        private static Lazy<INIHelper> _serializer = new Lazy<INIHelper>(() => new INIHelper());

        /// <summary>
        /// 取得處理Xml序列化的執行個體
        /// </summary>
        public static INIHelper Instance => _serializer.Value;

        public List<INIModel> Deserialize(string data)
        {

            MatchCollection matches = Regex.Matches(data, pattern,
                RegexOptions.Compiled |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Multiline);

            if (matches == null || matches.Count == 0) return null;

            List<INIModel> result = new List<INIModel>();

            foreach (Match match in matches)
            {
                var maxContent = match.Groups["KVPs"].Captures.Count;
                var keys = match.Groups["Key"].Captures.Cast<Capture>().Select(c => c.Value.Trim()).ToArray();
                var values = match.Groups["Value"].Captures.Cast<Capture>().Select(c => c.Value.Trim()).ToArray();

                var model = new INIModel()
                {
                    Section = match.Groups["Section"].Value
                };

                for (var i = 0; i < maxContent; i++)
                {
                    model.Content.Add(keys[i], values[i]);
                }

                result.Add(model);
            }

            return result;

        }

        public string Serialize(INIModel model)
        {
            return Serialize(new List<INIModel>() { model });
        }

        public string Serialize(IEnumerable<INIModel> collection)
        {
            var result = new StringBuilder();

            foreach (var model in collection)
            {
                result.Append(model.ToString());
            }

            return result.ToString();
        }

        public string Serialize<T>(T obj)
        {
            return Serialize(obj);
        }

        public T Deserialize<T>(string data)
        {
            if (typeof(T) == typeof(List<INIModel>))
            {
                return (T)(object)Deserialize(data);
            }

            throw new Exception("Must use List<INIModel>");
        }
    }

    public class INIModel
    {
        public INIModel()
        {
            Content = new Dictionary<string, string>();
        }

        public string Section { get; set; }
        public Dictionary<string, string> Content { get; set; }

        public override string ToString()
        {
            if (Content.Count == 0)
                return string.Empty;

            var result = new StringBuilder();

            result.AppendLine($"[{Section}]");

            foreach (var item in Content)
            {
                result.AppendLine($"{item.Key}={item.Value}");
            }

            return result.ToString();
        }
    }
}