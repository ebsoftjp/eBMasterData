using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EbMasterData;

public class ParseCSV
{
    private const string lineSplit = "\r\n";
    private const string fieldSplit = ",";

    [Test]
    public void T101_DoubleQuote() => Common(
        new string[]
        {
            "Id of data,\"Value of",
            "\"\",data\"",
        },
        new string[][]
        {
            new string[] { "Id of data", "Value of\n\",data" },
        });

    [Test]
    public void T102_DoubleQuote() => Common(
        new string[]
        {
            "\"\"Value\"\"",
            "\"\"\"Value\"\"\"",
        },
        new string[][]
        {
            new string[] { "\"Value\"" },
            new string[] { "\"Value\"" },
        });

    [Test]
    public void T201_Comma() => Common(
        new string[]
        {
            "\"1,2\",\"3,4\"",
        },
        new string[][]
        {
            new string[] { "1,2", "3,4" },
        });

    private void Common(string[] before, string[][] results)
    {
        foreach (var line in before)
        {
            Debug.Log(line);
        }

        var res = 0;
        var parser = new Parser(lineSplit, fieldSplit);
        var after = parser.Exec(string.Join(lineSplit, before));

        for (int i = 0; i < Mathf.Max(after.Length, results.Length); i++)
        {
            var r1 = after?.ElementAtOrDefault(i);
            var r2 = results?.ElementAtOrDefault(i);
            for (int j = 0; j < Mathf.Max(r1?.Length ?? 0, r2?.Length ?? 0); j++)
            {
                var c1 = r1?.ElementAtOrDefault(j) ?? "(null)";
                var c2 = r2?.ElementAtOrDefault(j) ?? "(null)";
                if (c1 != c2) res++;
                Debug.Log($"({j},{i}) [{c1}] => [{c2}]");
            }
        }
        Assert.That(res == 0, $"error count: {res}");
    }
}
