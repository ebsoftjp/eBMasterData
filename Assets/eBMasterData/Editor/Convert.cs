using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace eBMasterData.Editor
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
                $"[{cur + 1}/{max}] {text}",
                (float)(cur + 0) / (float)max);
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

        public async void WithSave(System.Func<Task> action)
        {
            try
            {
                await action();
                Save();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public async Task ConvertDBClasses()
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
        }

        public async Task DumpData()
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
        }

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
