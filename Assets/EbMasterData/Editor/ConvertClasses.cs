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

            // reference
            var refClasses = new List<Reader.KeysData3>();

            // local functions
            bool isClassType(string v) => allClasses.Contains(v) || allClasses.Contains(v.Replace("[]", ""));
            string getRefKey(string v) => $"{v}{settings.ClassPrimaryKey}";

            // base define
            foreach (var v in keys2)
            {
                var type = v.type;
                var key = v.key;
                var comment = v.comment.Replace("\n", "");
                if (comment == "") comment = "(no comment)";

                if (isClassType(type))
                {
                    type = "string";
                    key = getRefKey(key);
                    refClasses.Add(v);
                }

                res.Add($"        public {type} {key}; // {comment}");
            }

            if (refClasses.Count > 0)
            {
                res.Add($"");
                res.AddRange(refClasses
                    .SelectMany(v =>
                    {
                        var org = v.key;
                        var key = org;
                        var pmkey = $"{org}{settings.ClassPrimaryKey}";
                        var key2 = v.type;
                        var key3 = v.type.Replace("[]", "");
                        var table = $"{settings.DataFileName}.Tables.{key3}";

                        var add = new List<string>();

                        if (key2 == key3)
                        {
                            add.Add($"        public {tablePrefix}{key2} {key} => {table}.FirstOrDefault(v => v.{primaryKey} == {pmkey});");
                        }
                        else
                        {
                            add.Add($"        public {tablePrefix}{key2} {key} => {table}.Where(v => v.{primaryKey} == {pmkey}).ToArray();");
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
                var key = v.key;
                if (isClassType(v.type))
                {
                    key = getRefKey(key);
                }
                return $"            {key} = {parse};";
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
