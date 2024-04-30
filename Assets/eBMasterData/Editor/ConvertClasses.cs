using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace eBMasterData.Editor
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

        public List<string> CreateMasterDataDBClasses(List<ReaderForEditor.KeysData2> data)
        {
            var res = new List<string>
            {
                $"// Auto create by eBMasterData.ConvertClasses",
                $"using System.Linq;",
                $"using UnityEngine;",
                $"",
                $"namespace {settings.NamespaceName}",
                $"{{",
                $"    [System.Serializable]",
                $"    public abstract class {tablePrefix}{classBase0}",
                $"    {{",
                $"        public string {primaryKey}; // ID",
                $"",
                $"        protected int[] ToIntArray(string s) => s.Replace(\" \", \"\").Split(\",\").Where(v => v != \"\").Select(v => int.Parse(v)).ToArray();",
                $"        protected float[] ToFloatArray(string s) => s.Replace(\" \", \"\").Split(\",\").Where(v => v != \"\").Select(v => float.Parse(v)).ToArray();",
                $"",
                $"        protected Vector2 ToVector2(string s)",
                $"        {{",
                $"            var v = ToFloatArray(s);",
                $"            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1));",
                $"        }}",
                $"",
                $"        protected Vector2Int ToVector2Int(string s)",
                $"        {{",
                $"            var v = ToIntArray(s);",
                $"            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1));",
                $"        }}",
                $"",
                $"        protected Vector3 ToVector3(string s)",
                $"        {{",
                $"            var v = ToFloatArray(s);",
                $"            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1), v.ElementAtOrDefault(2));",
                $"        }}",
                $"",
                $"        protected Vector3Int ToVector3Int(string s)",
                $"        {{",
                $"            var v = ToIntArray(s);",
                $"            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1), v.ElementAtOrDefault(2));",
                $"        }}",
                $"",
                $"        protected T ToEnum<T>(string s)",
                $"        {{",
                $"            return string.IsNullOrEmpty(s) ? (T)System.Enum.ToObject(typeof(T), 0) : (T)System.Enum.Parse(typeof(T), s);",
                $"        }}",
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

        private List<string> CreateClassFileEach1(ReaderForEditor.KeysData2 data, string[] allClasses)
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
            var refClasses = new List<ReaderForEditor.KeysData3>();

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
                        var table = $"{settings.BaseFileName}.{key3}";

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

            //// reference
            //var inhClasses = allClasses
            //    .Where(v => v != data.name && v.StartsWith(data.name))
            //    .ToArray();

            //if (inhClasses.Length > 0)
            //{
            //    res.Add($"");
            //    res.AddRange(inhClasses
            //        .Select(v => $"        public {tablePrefix}{v}[] {v.Replace(data.name, "")}Array"
            //            + $" => {settings.BaseFileName}.{v}.Where(v => v.{primaryKey} == {primaryKey}).ToArray();"));
            //}

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
                    "float" => $"float.Parse(lines[{n}])",
                    "bool" => $"bool.Parse(lines[{n}])",
                    "Vector2" => $"ToVector2(lines[{n}])",
                    "Vector2Int" => $"ToVector2Int(lines[{n}])",
                    "Vector3" => $"ToVector3(lines[{n}])",
                    "Vector3Int" => $"ToVector3Int(lines[{n}])",
                    _ => $"lines[{n}]",
                };

                // enum
                if (settings.Enums.Contains(v.type))
                {
                    parse = $"ToEnum<{v.type}>(lines[{n}])";
                }

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
