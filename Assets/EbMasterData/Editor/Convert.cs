﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
//using Newtonsoft.Json;
using EbMasterData;

namespace EbMasterData.Editor
{
    public class Convert
    {
        private const string masterClassName = "Data";
        private const string tablePrefix = "DBClass";
        private const string classBase0 = "Base";
        private const string classBase1 = "Rate";
        //private const string classBase2 = "base2";
        private const string primaryKey = "Id";
        private const string idSuffix = "Id";
        private const string commaCode = ",";
        private const string subKey = "Rate";
        private const string configKey = "Config";
        //private static readonly string[][] subKeys = new string[][]
        //{
        //    new string[] { "Rate" },
        //};
        private static readonly string[] vecArray = new string[]
        {
            "VecData",
        };

        class ConfigData
        {
            public string type;
            public string group;
            public string key;
        }

        private static bool DownloadIndicator(int cur, int max, string text)
        {
            var isCancel = EditorUtility.DisplayCancelableProgressBar(
                text,
                $"{cur + 1}/{max} {text}",
                (float)(cur + 1) / (float)max);
            return isCancel;
        }

        private static void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        public static void InitData()
        {
            var path = Paths.SettingsFullPath;
            CreateDir(path);
            if (File.Exists(path)) return;
            var settings = ScriptableObject.CreateInstance<Settings>();
            AssetDatabase.CreateAsset(settings, path);

            WriteFile(new List<string>
            {
                $"{{",
                $"    \"name\": \"{Paths.NameSpace}\",",
                $"    \"rootNamespace\": \"\",",
                $"    \"references\": [],",
                $"    \"includePlatforms\": [],",
                $"    \"excludePlatforms\": [],",
                $"    \"allowUnsafeCode\": false,",
                $"    \"overrideReferences\": false,",
                $"    \"precompiledReferences\": [],",
                $"    \"autoReferenced\": true,",
                $"    \"defineConstraints\": [],",
                $"    \"versionDefines\": [],",
                $"    \"noEngineReferences\": false",
                $"}}",
            }, Paths.AsmFullPath);

            Save();
        }

        public static async void ConvertDBClasses()
        {
            // get data
            var reader = new ReaderForEditor(DownloadIndicator);

            await reader.CreateFileList();
            await reader.ReadText();
            reader.CreateData();
            EditorUtility.ClearProgressBar();

            // create classes
            CreateMasterDataDBClasses(reader.data3, reader.DBClassesPath);

            // create data
            CreateMasterDataData(reader.data3, reader.DBDataPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        //[MenuItem("Tools/DB download", false, 102)]
        //private static async void DownloadDB()
        //{
        //    // get server data
        //    var d = new MasterData.Downloader();
        //    var res = await d.Download2(DownloadIndicator);
        //    //var res = await d.DownloadDummy2(DownloadIndicator);
        //    EditorUtility.ClearProgressBar();

        //    // add local
        //    var files = Directory.GetFiles("Assets/CSV/", "*.csv");
        //    var append = new List<string>();
        //    foreach (var file in files)
        //    {
        //        var item = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
        //        var lines = item.text.Split(retCode).Where(v => v != "").Select(v => v.Split(commaCode));
        //        append.Add(item.name);
        //        var keys = lines?.FirstOrDefault() ?? new string[0];
        //        append.Add("[" + string.Join(",", lines?.Skip(2)?
        //            .Select(v => $"{{{string.Join(",", v.Select((s, n) => $"\"{keys[n]}\":\"{s}\""))}}}")
        //            ?? new string[0]) + "]");
        //    }
        //    EditorUtility.ClearProgressBar();

        //    var data = AssetDatabase.LoadAssetAtPath<MasterData.Data>("Assets/Resources/DBMasters.asset");
        //    data.Convert2(res.Concat(append).ToArray());
        //    EditorUtility.SetDirty(data);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        //[MenuItem("Tools/DB config", false, 104)]
        //private static void CreateDBConfig()
        //{
        //    // get data
        //    var data = AssetDatabase.LoadAssetAtPath<MasterData.Data>("Assets/Resources/DBMasters.asset");
        //    MasterData.Data.Tables = data;
        //    var path = "Assets/MasterData/Scripts/Config.cs";

        //    foreach (var area in data.Area)
        //    {
        //        Debug.Assert(area.AreaBlockArray.Length > 0, $"Area - {area.Id}: area.AreaBlockArray.Length > {area.AreaBlockArray.Length}");
        //    }

        //    var res = new List<string>
        //    {
        //        $"// Auto create by DBClassConvert",
        //        $"using System.Linq;",
        //        $"using UnityEngine;",
        //        $"using MasterData;",
        //        $"",
        //        $"public class {configKey}",
        //        $"{{",
        //        $"    public static void Reset()",
        //        $"    {{",
        //    };

        //    // group
        //    var groups = data.Config.Select(v => v.Group).Distinct().ToArray();

        //    // reset
        //    foreach (var group in groups)
        //    {
        //        res.Add($"        _{group} = null;");
        //    }

        //    res.AddRange(new List<string>
        //    {
        //        $"    }}",
        //    });

        //    for (int i = 0; i < groups.Length; i++)
        //    {
        //        //if (i > 0) res.Add("");

        //        var group = groups[i];
        //        res.AddRange(new List<string>
        //        {
        //            $"",
        //            $"    public static {configKey}{group} _{group};",
        //            $"    public static {configKey}{group} {group}",
        //            $"    {{",
        //            $"        get",
        //            $"        {{",
        //            $"            if (_{group} == null)",
        //            $"            {{",
        //            $"                var list = DB.Tables.{configKey}.Where(v => v.Group == \"{group}\");",
        //            $"                _{group} = new()",
        //            $"                {{",
        //        });

        //        var list = data.Config.Where(v => v.Group == group).ToArray();
        //        for (int j = 0; j < list.Length; j++)
        //        {
        //            var d = list[j];
        //            res.Add($"                    {d.Id} = To{d.Type}(list.FirstOrDefault(v => v.Id == \"{d.Id}\").Str),");
        //        }

        //        res.AddRange(new List<string>
        //        {
        //            $"                }};",
        //            $"            }}",
        //            $"            return _{group};",
        //            $"        }}",
        //            $"    }}",
        //        });
        //    }

        //    res.AddRange(new List<string>
        //    {
        //        $"",
        //        $"    private static int Toint(string str)",
        //        $"    {{",
        //        $"        int.TryParse(str, out var n);",
        //        $"        return n;",
        //        $"    }}",
        //        $"",
        //        $"    private static string Tostring(string str) => str;",
        //        $"",
        //        $"    private static Vector3 ToVector3(string str)",
        //        $"    {{",
        //        $"        var v = str.Replace(\" \", \"\")",
        //        $"            .Split(\",\")",
        //        $"            .Select(v => {{ float.TryParse(v, out var n); return n; }});",
        //        $"        return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1), v.ElementAtOrDefault(2));",
        //        $"    }}",
        //        $"",
        //        $"    private static Vector3Int ToVector3Int(string str)",
        //        $"    {{",
        //        $"        var v = str.Replace(\" \", \"\")",
        //        $"            .Split(\",\")",
        //        $"            .Select(v => {{ int.TryParse(v, out var n); return n; }});",
        //        $"        return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1), v.ElementAtOrDefault(2));",
        //        $"    }}",
        //        $"}}",
        //        $"",
        //        $"namespace MasterData",
        //        $"{{",
        //    });

        //    for (int i = 0; i < groups.Length; i++)
        //    {
        //        var group = groups[i];
        //        res.AddRange(new List<string>
        //        {
        //            $"",
        //            $"    [System.Serializable]",
        //            $"    public class {configKey}{group}",
        //            $"    {{",
        //        });

        //        res.AddRange(data.Config.Where(v => v.Group == group).SelectMany((v, n) => new List<string>
        //        {
        //            $"",
        //            $"        /// <summary>",
        //            string.Join("\n", v.Description.Split("\n").Select(s => $"        /// {s}")),
        //            $"        /// </summary>",
        //            $"        public {v.Type} {v.Id};"
        //        }.Skip(n == 0 ? 1 : 0)));

        //        res.AddRange(new List<string>
        //        {
        //            $"    }}",
        //        });
        //    }

        //    res.AddRange(new List<string>
        //    {
        //        $"}}",
        //    });

        //    WriteFile(res, path);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        private static void CreateMasterDataData(List<Reader.KeysData2> data, string path)
        {
            var res = new List<string>
            {
                $"// Auto create by DBClassConvert",
                $"using UnityEngine;",
                //$"using Newtonsoft.Json;",
                $"",
                $"namespace MasterData",
                $"{{",
                $"    public class {masterClassName} : ScriptableObject",
                $"    {{",
                $"        public static {masterClassName} Tables;",
                $"",
            };

            foreach (var v in data)
            {
                res.Add($"        public {tablePrefix}{v.name}[] {v.name};");
            }

            res.AddRange(new List<string>
            {
                $"",
                $"        public {tablePrefix}{classBase0}[] GetItemsFromTableName(string tableName) => tableName switch",
                $"        {{",
            });

            foreach (var v in data)
            {
                res.Add($"            \"{v.name}\" => {v.name},");
            }

            res.AddRange(new List<string>
            {
                $"            _ => null,",
                $"        }};",
                $"",
                $"        public void Convert2(string[] res)",
                $"        {{",
                $"            foreach (var item in res) Debug.Log(item);",
            });

            foreach (var v in data)
            {
                res.Add($"            {v.name} = ConvertList<{tablePrefix}{v.name}>(\"{v.name}\", res);");
            }

            res.AddRange(new List<string>
            {
                $"        }}",
                $"",
                $"        private T[] ConvertList<T>(string key, string[] res)",
                $"        {{",
                //$"            var index = System.Array.IndexOf(res, key);",
                //$"            return (index >= 0 && index + 1 < res.Length)",
                //$"                ? JsonConvert.DeserializeObject<T[]>(res[index + 1])",
                //$"                : new T[0];",
                $"            return null;",
                $"        }}",
                $"    }}",
                $"}}",
            });

            WriteFile(res, path);
        }

        private static void CreateMasterDataDBClasses(List<Reader.KeysData2> data, string path)
        {
            var res = new List<string>
            {
                $"// Auto create by DBClassConvert",
                $"using System.Linq;",
                $"using UnityEngine;",
                $"",
                $"namespace MasterData",
                $"{{",
                $"    [System.Serializable]",
                $"    public class {tablePrefix}{classBase0}",
                $"    {{",
                $"        public string {primaryKey}; // ID",
                $"    }}",
                $"",
                $"    [System.Serializable]",
                $"    public class {tablePrefix}{classBase1} : {tablePrefix}{classBase0}",
                $"    {{",
                $"        public string {subKey};",
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
            });

            WriteFile(res, path);
        }

        private static List<string> CreateClassFileEach1(Reader.KeysData2 data, string[] allClasses)
        {
            //return new List<string>();
            //Reader.KeysData2 data = null;
            data.keys = data.keys.Where(v => v.key != primaryKey).ToArray();

            var useBase = classBase0;
            if (data.keys.Length == 2 && data.keys[1].key == subKey)
            {
                useBase = classBase1;
                data.keys = data.keys
                    .Where(v => v.key != subKey)
                    .ToArray();
            }

            // param
            var res = new List<string>()
            {
                $"",
                $"    [System.Serializable]",
                $"    public class {tablePrefix}{data.name} : {tablePrefix}{useBase}",
                $"    {{",
            };

            res.AddRange(data.keys.Select(v => $"        public {v.type} {v.key}; // {v.comment.Replace("\n", "")}"));

            // reference
            var refClasses = data.keys
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
            var vecClasses = data.keys
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

            res.Add($"    }}");

            return res;
        }

        private static List<string> CreateClassFileEach2(string key1, string[] keys2)
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

        private static void CreateDir(string path)
        {
            var dir = Regex.Replace(path, @"[^/]+?$", "");
            if (Directory.Exists(dir)) return;
            Directory.CreateDirectory(dir);
        }

        private static void CreateFile(string path)
        {
            if (File.Exists(path)) return;

            CreateDir(path);
            var fs = File.Create(path);
            fs.Close();
            AssetDatabase.ImportAsset(path);
        }

        private static void WriteFile(List<string> res, string path)
        {
            CreateFile(path);

            var sw = new StreamWriter(path, false);
            sw.WriteLine(string.Join("\n", res));
            sw.Flush();
            sw.Close();

            var text = AssetDatabase.LoadAssetAtPath<Object>(path);
            EditorUtility.SetDirty(text);
        }
    }
}
