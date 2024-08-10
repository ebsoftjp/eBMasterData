#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace eBMasterData.Editor
{
    public class ConvertClasses
    {
        private string tablePrefix => settings.ClassNamePrefix;
        private string classBase0 => settings.ClassNameBase;
        private string primaryKey => settings.ClassPrimaryKey;
        private static readonly string[] vecArray = new string[]
        {
            "VecData",
        };

        private string classBase1 => "Rate";
        private string rateKey => "Rate";

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
                $"        protected int[] ToIntArray(string s) => s.Replace(\" \", \"\").Split(\",\").Where(v => v != \"\").Select(v => ToType<int>(v)).ToArray();",
                $"        protected bool[] ToBoolArray(string s) => s.Replace(\" \", \"\").Split(\",\").Where(v => v != \"\").Select(v => ToType<bool>(v)).ToArray();",
                $"        protected float[] ToFloatArray(string s) => s.Replace(\" \", \"\").Split(\",\").Where(v => v != \"\").Select(v => ToType<float>(v)).ToArray();",
                $"",
                $"        protected T ToType<T>(string s)",
                $"        {{",
                $"            try",
                $"            {{",
                $"                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));",
                $"                return (T)converter?.ConvertFromString(s) ?? default;",
                $"            }}",
                $"            catch",
                $"            {{",
                $"                Debug.LogError($\"{{s}}: not {{typeof(T).Name}}\");",
                $"                return default;",
                $"            }}",
                $"        }}",
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
                $"        protected Color ToColor(string s)",
                $"        {{",
                $"            var b = ColorUtility.TryParseHtmlString(s, out var v);",
                $"            return b ? v : Color.white;",
                $"        }}",
                $"",
                $"        protected T ToEnum<T>(string s)",
                $"        {{",
                $"            return string.IsNullOrEmpty(s) ? (T)System.Enum.ToObject(typeof(T), 0) : (T)System.Enum.Parse(typeof(T), s);",
                $"        }}",
                $"    }}",
                $"",
                $"    [System.Serializable]",
                $"    public abstract class {tablePrefix}{classBase1} : {tablePrefix}{classBase0}",
                $"    {{",
                $"        public int {rateKey}; // Rate",
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

            // rate class
            if (keys2.FirstOrDefault(v => v.key == rateKey) != null)
            {
                useBase = classBase1;
            }

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
                if (key == rateKey) continue;
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
                    "int" => $"ToType<int>(lines[{n}])",
                    "bool" => $"ToType<bool>(lines[{n}])",
                    "float" => $"ToType<float>(lines[{n}])",
                    "Vector2" => $"ToVector2(lines[{n}])",
                    "Vector2Int" => $"ToVector2Int(lines[{n}])",
                    "Vector3" => $"ToVector3(lines[{n}])",
                    "Vector3Int" => $"ToVector3Int(lines[{n}])",
                    "Color" => $"ToColor(lines[{n}])",
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
    }
}
#endif
