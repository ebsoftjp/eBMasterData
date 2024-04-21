using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace EbMasterData.Editor
{
    public class ConvertClasses
    {
        private string tablePrefix => settings.ClassNamePrefix;
        private string classBase0 => settings.ClassNameBase;
        private string primaryKey => settings.ClassPrimaryKey;
        private string idSuffix => primaryKey;
        //private string configKey = "Config";
        private static readonly string[] vecArray = new string[]
        {
            "VecData",
        };

        private readonly Settings settings;

        public ConvertClasses(Settings settings)
        {
            this.settings = settings;
        }

        public List<string> CreateMasterDataDBClasses(List<Reader.KeysData2> data)
        {
            var res = new List<string>
            {
                $"// Auto create by EbMasterData.ConvertClasses",
                $"using System.Linq;",
                $"using UnityEngine;",
                $"",
                $"namespace {settings.NamespaceName}",
                $"{{",
                $"    [System.Serializable]",
                $"    public abstract class {tablePrefix}{classBase0}",
                $"    {{",
                $"        public string {primaryKey}; // ID",
                $"    }}",
            };

            // create reference list
            var allClasses = data.Select(v => v.name).OrderByDescending(v => v.Length).ToArray();

            // for reference
            foreach (var v in data)
            {
                var list = CreateClassFileEach1(v, allClasses);
                res.AddRange(list);
            }

            res.AddRange(new List<string>
            {
                $"}}",
                $"",
            });

            return res;
        }

        private List<string> CreateClassFileEach1(Reader.KeysData2 data, string[] allClasses)
        {
            var keys2 = data.keys.Where(v => v.key != primaryKey).ToArray();

            var useBase = classBase0;

            // begin
            var res = new List<string>()
            {
                $"",
                $"    [System.Serializable]",
                $"    public class {tablePrefix}{data.name} : {tablePrefix}{useBase}",
                $"    {{",
            };

            res.AddRange(keys2.Select(v => $"        public {v.type} {v.key}; // {v.comment.Replace("\n", "")}"));

            // reference
            var refClasses = keys2
                .Where(v => Regex.IsMatch(v.key, $"{idSuffix}$"))
                .ToArray();

            if (refClasses.Length > 0)
            {
                res.Add($"");
                res.AddRange(refClasses
                    .SelectMany(v =>
                    {
                        var org = v.key;
                        var key = Regex.Replace(org, $"{idSuffix}$", "");
                        var key2 = allClasses.FirstOrDefault(r => r == key)
                            ?? allClasses.FirstOrDefault(r => Regex.IsMatch(key, $"{r}$")) ?? key;

                        var add = new List<string>
                        {
                            $"        public {tablePrefix}{key2} {key} => Data.Tables.{key2}.FirstOrDefault(v => v.{primaryKey} == {org});",
                            $"        public {tablePrefix}{key2}[] {key}Array => Data.Tables.{key2}.Where(v => v.{primaryKey} == {org}).ToArray();",
                        };

                        if (vecArray.Contains(key2))
                        {
                            add.Add($"        public Vector3Int[] {key}VecArray => {key}Array.Select(v => v.Vec).ToArray();");
                        }

                        return add;
                    }));
            }

            // reference
            var inhClasses = allClasses
                .Where(v => v != data.name && v.StartsWith(data.name))
                .ToArray();

            if (inhClasses.Length > 0)
            {
                res.Add($"");
                res.AddRange(inhClasses
                    .Select(v => $"        public {tablePrefix}{v}[] {v.Replace(data.name, "")}Array"
                        + $" => Data.Tables.{v}.Where(v => v.{primaryKey} == {primaryKey}).ToArray();"));
            }

            // vector
            var vecClasses = keys2
                .Where(v => Regex.IsMatch(v.key, @"X$"))
                .ToArray();

            if (vecClasses.Length > 0)
            {
                res.Add($"");
                res.AddRange(vecClasses
                    .SelectMany(v =>
                    {
                        var org = v.key;
                        var key = Regex.Replace(org, @"X$", "");
                        var key2 = key == "" ? "Vec" : key;
                        return new string[]
                        {
                            $"        public Vector3Int {key2} => new({key}X, {key}Y, {key}Z);",
                        };
                    }));
            }

            // constructor
            res.AddRange(new List<string>()
            {
                $"",
                $"        public {tablePrefix}{data.name}(params string[] lines)",
                $"        {{",
            });

            res.AddRange(data.keys.Select((v, n) =>
            {
                var parse = v.type switch
                {
                    "int" => $"int.Parse(lines[{n}])",
                    _ => $"lines[{n}]",
                };
                return $"            {v.key} = {parse};";
            }));

            // end
            res.AddRange(new List<string>()
            {
                $"        }}",
                $"    }}",
            });

            return res;
        }

        private List<string> CreateClassFileEach2(string key1, string[] keys2)
        {
            // param
            var res = new List<string>()
            {
                $"",
                $"    [System.Serializable]",
                $"    public class {tablePrefix}{key1}",
                $"    {{",
            };

            res.AddRange(keys2.Select(v => $"// {v}"));

            res.AddRange(new List<string>
            {
                $"}}",
            });

            return res;
        }
    }
}
