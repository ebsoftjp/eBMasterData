using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EbMasterData
{
    public class Parser
    {
        private const char dqChar = '\"';
        private readonly string lineSplit;
        private readonly string fieldSplit;

        public Parser(string lineSplit, string fieldSplit)
        {
            this.lineSplit = lineSplit;
            this.fieldSplit = fieldSplit;
        }

        public string[][] Exec(string src)
        {
            var res = MultiItemSplit(src.Split(lineSplit), "\n")
                .Select(v => MultiItemSplit(v.Split(fieldSplit), fieldSplit)
                    .Select(s => s[0] == dqChar && s[^1] == dqChar ? s[1..^1] : s)
                    .Select(s => s.Replace($"{dqChar}{dqChar}", $"{dqChar}"))
                    .ToArray())
                .ToArray();
            return res;
        }

        private List<string> MultiItemSplit(string[] src, string joinText)
        {
            var res = new List<string>();
            var tmp = new List<string>();
            var dqCount = 0;
            foreach (var item in src)
            {
                dqCount += item.Count(v => v == dqChar);
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
    }
}
