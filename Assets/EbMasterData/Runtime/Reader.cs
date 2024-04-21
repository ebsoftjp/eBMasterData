using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;
//using Newtonsoft.Json;

namespace EbMasterData
{
    public class Reader
    {
        protected const string commaCode = ",";
        protected const string retCode = "\r\n";
        protected readonly Dictionary<string, string> loadCache = new();

        [System.Serializable]
        protected class LoadData
        {
            public string Path;
            public SettingsDataSource Src;
        }

        [System.Serializable]
        protected class CustomAPIData
        {
            public CustomAPIItem[] List;
        }

        [System.Serializable]
        protected class CustomAPIItem
        {
            public string Name;
            public string Text;
        }

        protected readonly List<LoadData> files2 = new();
        public readonly List<LoadedText> data2 = new();
        public readonly List<KeysData2> data3 = new();

        [System.Serializable]
        public class LoadedText
        {
            public string Name;
            public string Text;
        }

        [System.Serializable]
        public class KeysData2
        {
            public string name;
            public KeysData3[] keys;
        }

        [System.Serializable]
        public class KeysData3
        {
            public string key;
            public string type;
            public string comment;
        }

        public string DBClassesPath => $"{settings.OutputPath}/{settings.DBClassesFileName}.cs";
        public string DBDataPath => $"{settings.OutputPath}/{settings.DataFileName}.cs";

        protected readonly System.Func<int, int, string, bool> indicatorFunc;
        protected readonly Settings settings;
        protected readonly Dictionary<string, string> tmpTextList = new();

        public Reader(System.Func<int, int, string, bool> indicatorFunc)
        {
            this.indicatorFunc = indicatorFunc;
            settings = Resources.Load<Settings>(Paths.SettingsPath);
        }

        public async Task ReadText()
        {
            //var data2 = new List<LoadedText>();
            data2.Clear();
            foreach (var item in files2)
            {
                var res = item.Src.DataType switch
                {
                    DsDataType.CustomAPI => await ReadFromCustomAPI(item),
                    DsDataType.Addressables => await ReadFromAddressables(item),
                    DsDataType.StreamingAssets => await ReadFromStreamingAssets(item),
                    DsDataType.GoogleSpreadSheet => await ReadFromGoogleSpreadSheet(item),
                    _ => null,
                };

                // exclude null data
                if (res != null)
                {
                    data2.Add(res);
                }
            }

            for (int i = 0; i < data2.Count; i++)
            {
                Debug.Log($"[Read {i + 1}/{data2.Count}] {data2[i].Name}\n{data2[i].Text}");
            }
        }

        public void CreateData()
        {
            data3.Clear();
            for (int i = 0; i < data2.Count; i++)
            {
                data3.Add(TextToData(data2[i]));
            }
        }

        protected virtual async Task<LoadedText> ReadFromCustomAPI(LoadData item)
        {
            var dl = new DownloaderText(item.Path, false);
            var text = await dl.Get();
            return null;
        }

        protected virtual async Task<LoadedText> ReadFromAddressables(LoadData item)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(item.Path);
            await handle.Task;

            return new()
            {
                Name = item.Path,
                Text = handle.Result.text,
            };
        }

        protected virtual async Task<LoadedText> ReadFromStreamingAssets(LoadData item)
        {
            var dl = new DownloaderText(item.Path, true);
            var text = await dl.Get();
            return null;
        }

        protected virtual async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item)
        {
            var dl = new DownloaderText(item.Path, false)
            {
                ResponseAction = req =>
                {
                    Debug.Log($"ResponseAction");
                    // Content-Disposition: attachment; filename="EbMasterData-SprData.csv"; filename*=UTF-8''EbMasterData%20-%20SprData.csv
                    var file = Regex.Match(req.GetResponseHeaders()?.GetValueOrDefault("Content-Disposition") ?? "",
                        @"([a-zA-Z0-9_]+)\.csv"";").Groups[1].Value;
                    Debug.Log($"\"{file}\"");
                },
            };
            var text = await dl.Get();
            return null;
        }

        protected virtual async Task<LoadedText> ReadFromFile(LoadData item)
        {
            //var files = item.Src.Format switch
            //{
            //    DsFormat.CSV => Directory.GetFiles(item.Path, "*.csv"),
            //    DsFormat.JSON => Directory.GetFiles(item.Path, "*.json"),
            //    _ => null,
            //};

            //var file = files[i];

            //// cancel from editor
            //var isCancel = indicatorFunc?.Invoke(i, files.Length, file) ?? false;
            //if (isCancel) return null;

            //Debug.Log(file);
            //var item = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
            //Debug.Log(item.name);
            //Debug.Log(item.text);
            //var lines = item.text.Split(retCode).Select(v => v.Split(commaCode));

            using (var sr = new StreamReader(item.Path))
            {
                //data2.Add(TextToData(file, await sr.ReadToEndAsync()));
                var res = await sr.ReadToEndAsync();
                sr.Close();
                return new()
                {
                    Name = PathToTableName(item.Path),
                    Text = res,
                };
            }
        }

        protected KeysData2 TextToData(LoadedText data)
        {
            //Debug.Log(data.Name);
            var lines = data.Text.Split(retCode).Select(v => v.Split(commaCode));
            return new()
            {
                name = data.Name,
                keys = Enumerable.Repeat(0, lines?.FirstOrDefault()?.Count() ?? 0).Select((_, n) => new KeysData3()
                {
                    key = lines?.ElementAtOrDefault(0)?.ElementAtOrDefault(n) ?? "",
                    type = lines?.ElementAtOrDefault(1)?.ElementAtOrDefault(n) ?? "",
                    comment = lines?.ElementAtOrDefault(2)?.ElementAtOrDefault(n) ?? "",
                }).ToArray(),
            };
        }

        private string PathToTableName(string str)
            => Regex.Match(str, @"([^/]+)\.[a-zA-Z0-9]+$").Groups[1].Value;
    }
}
