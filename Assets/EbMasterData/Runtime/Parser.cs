using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EbMasterData
{
    public class Parser
    {
        private const char dqCode = '\"';
        private const char retCode = '\n';
        private readonly string lineSplit;
        private readonly string fieldSplit;

        public bool IsOutputLog;

        /// <summary>
        /// Constructor
        /// </summary>
        public Parser(string lineSplit, string fieldSplit)
        {
            this.lineSplit = ReplaceEscape(lineSplit);
            this.fieldSplit = ReplaceEscape(fieldSplit);
        }

        /// <summary>
        /// Convert text to data
        /// </summary>
        public string[][] Exec(string src)
        {
            var res = MultiItemSplit(src.Split(lineSplit), $"{retCode}")
                .Select(v => MultiItemSplit(v.Split(fieldSplit), fieldSplit)
                    .Select(s => s[0] == dqCode && s[^1] == dqCode ? s[1..^1] : s)
                    .Select(s => s.Replace($"{dqCode}{dqCode}", $"{dqCode}"))
                    .ToArray())
                .ToArray();

            if (IsOutputLog)
            {
                Debug.Log($"[{res.Length}, {res[0].Length}]\n" + string.Join(
                    retCode,
                    res.Select(line => string.Join(fieldSplit, line
                        .Select(v => v.Replace($"{retCode}", "\\n"))))));
            }

            return res;
        }

        /// <summary>
        /// Convert lines
        /// </summary>
        private static List<string> MultiItemSplit(string[] src, string joinText)
        {
            var res = new List<string>();
            var tmp = new List<string>();
            var dqCount = 0;
            foreach (var item in src)
            {
                dqCount += item.Count(v => v == dqCode);
                tmp.Add(item);
                if (dqCount % 2 == 0)
                {
                    res.Add(string.Join(joinText, tmp));
                    tmp.Clear();
                }
            }
            Debug.Assert(tmp.Count == 0 && dqCount % 2 == 0, $"error: {tmp}, {dqCount}");
            return res;
        }

        private static string ReplaceEscape(string str) => str
            .Replace("\\t", "\t")
            .Replace("\\r", "\r")
            .Replace("\\n", "\n");
    }
}
