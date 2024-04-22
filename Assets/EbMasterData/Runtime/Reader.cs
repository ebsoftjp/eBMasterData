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
    public abstract class Reader
    {
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

        [System.Serializable]
        public class LoadedText
        {
            public string Name;
            public string Text;
        }

        public string DBClassesPath => $"{settings.OutputPath}/{settings.ClassesFileName}.cs";
        public string DBDataPath => $"{settings.OutputPath}/{settings.DataFileName}.cs";

        protected readonly System.Func<int, int, string, bool> indicatorFunc;
        protected readonly Settings settings;
        protected readonly Dictionary<string, string> tmpTextList = new();

        public Reader(System.Func<int, int, string, bool> indicatorFunc)
        {
            this.indicatorFunc = indicatorFunc;
            settings = Resources.Load<Settings>(Paths.SettingsPath);
        }

        public async Task CreateFileList()
        {
            foreach (var src in settings.Sources ?? new SettingsDataSource[0])
            {
                Debug.Log($"{src.DataType}, {src.Format}, {src.Path}");
                var res = src.DataType switch
                {
                    DsDataType.CustomAPI => await CreateFileListFromCustomAPI(src),
                    DsDataType.Resources => await CreateFileListFromResources(src),
                    DsDataType.Addressables => await CreateFileListFromAddressables(src),
                    DsDataType.StreamingAssets => await CreateFileListFromStreamingAssets(src),
                    DsDataType.GoogleSpreadSheet => await CreateFileListFromGoogleSpreadSheet(src),
                    _ => null,
                };

                // exclude null data
                if (res != null)
                {
                    files2.AddRange(res);
                }
            }

            for (int i = 0; i < files2.Count; i++)
            {
                Debug.Log($"[files2 {i + 1}/{files2.Count}] {files2[i].Path}");
            }
            for (int i = 0; i < loadCache.Count; i++)
            {
                Debug.Log($"[loadCache {i + 1}/{loadCache.Count}] {loadCache.Keys.ElementAt(i)}");
            }
        }

        public async Task ReadText()
        {
            data2.Clear();
            foreach (var item in files2)
            {
                var res = item.Src.DataType switch
                {
                    DsDataType.CustomAPI => await ReadFromCustomAPI(item),
                    DsDataType.Resources => await ReadFromResources(item),
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

        protected async Task<List<LoadData>> CreateFileListDummy(SettingsDataSource _)
        {
            await Task.CompletedTask;
            return null;
        }

        protected virtual async Task<List<LoadData>> CreateFileListFromCustomAPI(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromResources(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromAddressables(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromStreamingAssets(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromGoogleSpreadSheet(SettingsDataSource src) => await CreateFileListDummy(src);

        protected async Task<LoadedText> ReadTextDummy(LoadData _)
        {
            await Task.CompletedTask;
            return null;
        }

        protected virtual async Task<LoadedText> ReadFromCustomAPI(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromResources(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromAddressables(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromStreamingAssets(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item) => await ReadTextDummy(item);

        protected async Task<LoadedText> ReadFromFile(LoadData item)
        {
            using var sr = new StreamReader(item.Path);
            var res = await sr.ReadToEndAsync();
            sr.Close();

            return new()
            {
                Name = PathToTableName(item.Path),
                Text = res,
            };
        }

        protected async Task<LoadedText> ReadFromCache(LoadData item)
        {
            await Task.CompletedTask;

            return new()
            {
                Name = item.Path,
                Text = loadCache.GetValueOrDefault(item.Path) ?? "",
            };
        }

        private string PathToTableName(string str)
            => Regex.Match(str, @"([^/]+)\.[a-zA-Z0-9]+$").Groups[1].Value;
    }
}
