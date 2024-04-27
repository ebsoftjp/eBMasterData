using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace EbMasterData.Editor
{
    public class Convert
    {
        private Settings settings;

        private class ConfigData
        {
            public string type;
            public string group;
            public string key;
        }

        public Convert()
        {
            if (File.Exists(Paths.SettingsFullPath))
            {
                settings = Resources.Load<Settings>(Paths.SettingsPath);
            }
        }

        private bool DownloadIndicator(int cur, int max, string text)
        {
            var isCancel = EditorUtility.DisplayCancelableProgressBar(
                text,
                $"{cur + 1}/{max} {text}",
                (float)(cur + 1) / (float)max);
            return isCancel;
        }

        private void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        public void InitData()
        {
            CreateDir(Paths.SettingsFullPath);
            if (File.Exists(Paths.SettingsFullPath)) return;
            settings = ScriptableObject.CreateInstance<Settings>();
            AssetDatabase.CreateAsset(settings, Paths.SettingsFullPath);

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

        public async void ConvertDBClasses()
        {
            // get data
            var reader = new ReaderForEditor(DownloadIndicator);

            await reader.CreateFileList();
            await reader.ReadText();
            reader.ParseData();
            reader.CreateHeaderData();

            // create base
            var cb = new ConvertBase(settings);
            WriteFile(cb.Create(reader.data3), reader.DBBasePath);

            // create classes
            var cc = new ConvertClasses(settings);
            WriteFile(cc.CreateMasterDataDBClasses(reader.data3), reader.DBClassesPath);

            // create enums
            var ce = new ConvertEnums(settings);
            WriteFile(ce.CreateMasterDataEnums(reader.data4), reader.DBEnumsPath);

            // create data
            var cd = new ConvertData(settings);
            WriteFile(cd.CreateMasterDataData(reader.data3), reader.DBDataPath);

            Save();
            EditorUtility.ClearProgressBar();
        }

        public async void DumpData()
        {
            // get data
            var reader = new ReaderForEditor(DownloadIndicator);

            await reader.CreateFileList();
            await reader.ReadText();
            reader.ParseData();

            // create data
            var obj = ScriptableObject.CreateInstance($"{settings.NamespaceName}.{settings.DataFileName}");
            obj.GetType().GetMethod("Convert2").Invoke(obj, new object[] { reader.ParsedTables, reader.ParsedValues } );
            AssetDatabase.CreateAsset(obj, Paths.DataFullPath);

            Save();
            EditorUtility.ClearProgressBar();
        }

        //private void CreateDBConfig()
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

        private void CreateDir(string path)
        {
            var dir = Regex.Replace(path, @"[^/]+?$", "");
            if (Directory.Exists(dir)) return;
            Directory.CreateDirectory(dir);
        }

        private void CreateFile(string path)
        {
            if (File.Exists(path)) return;

            CreateDir(path);
            var fs = File.Create(path);
            fs.Close();
            AssetDatabase.ImportAsset(path);
        }

        private void WriteFile(List<string> res, string path)
        {
            CreateFile(path);

            var sw = new StreamWriter(path, false);
            sw.Write(string.Join("\n", res));
            sw.Flush();
            sw.Close();

            var text = AssetDatabase.LoadAssetAtPath<Object>(path);
            EditorUtility.SetDirty(text);
        }
    }
}
