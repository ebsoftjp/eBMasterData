using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

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

        protected readonly List<LoadData> dataForLoad = new();
        public readonly List<LoadedText> loadedTexts = new();
        public readonly List<string[][]> parsedValues = new();

        public string DBBasePath => $"{settings.OutputPath}/{settings.BaseFileName}.cs";
        public string DBClassesPath => $"{settings.OutputPath}/{settings.ClassesFileName}.cs";
        public string DBEnumsPath => $"{settings.OutputPath}/{settings.EnumsFilePath}.cs";
        public string DBDataPath => $"{settings.OutputPath}/{settings.DataFileName}.cs";

        protected readonly System.Func<int, int, string, bool> indicatorFunc;
        protected readonly Settings settings;
        protected readonly Parser parser;

        // Constructor ================================================================

        public Reader(System.Func<int, int, string, bool> indicatorFunc)
        {
            this.indicatorFunc = indicatorFunc;
            settings = Resources.Load<Settings>(Paths.SettingsPath);
            parser = new(settings.LineSplitString, settings.FieldSplitString);
        }

        // File list ================================================================

        public async Task CreateFileList()
        {
            var max = settings.Sources.Length;
            for (int i = 0; i < max; i++)
            {
                var src = settings.Sources[i];
                if (indicatorFunc?.Invoke(i, max, $"{src.DataType}: {src.Path}") ?? false) break;

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
                    dataForLoad.AddRange(res);
                }
            }

            for (int i = 0; i < dataForLoad.Count; i++)
            {
                Debug.Log($"[files2 {i + 1}/{dataForLoad.Count}] {dataForLoad[i].Path}");
            }
            for (int i = 0; i < loadCache.Count; i++)
            {
                Debug.Log($"[loadCache {i + 1}/{loadCache.Count}] {loadCache.Keys.ElementAt(i)}");
            }
        }

        protected virtual async Task<List<LoadData>> CreateFileListFromCustomAPI(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromResources(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromAddressables(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromStreamingAssets(SettingsDataSource src) => await CreateFileListDummy(src);
        protected virtual async Task<List<LoadData>> CreateFileListFromGoogleSpreadSheet(SettingsDataSource src) => await CreateFileListDummy(src);

        protected async Task<List<LoadData>> CreateFileListDummy(SettingsDataSource _)
        {
            await Task.CompletedTask;
            return null;
        }

        protected async Task<List<LoadData>> CreateCustomAPI(SettingsDataSource src)
        {
            var dl = new DownloaderText(src.Path, false);
            var text = await dl.Get();
            Debug.Log(text);
            var data = JsonUtility.FromJson<CustomAPIData>($"{{\"List\":{text}}}");
            foreach (var item in data.List)
            {
                loadCache[item.Name] = item.Text;
            }
            return data.List
                .Select(v => new LoadData()
                {
                    Path = v.Name,
                    Src = src,
                })
                .ToList();
        }

        protected async Task<List<LoadData>> CreateSpreadSheet(SettingsDataSource src)
        {
            var file = "";
            var dl = new DownloaderText(src.Path, false)
            {
                ResponseAction = req =>
                {
                    Debug.Log($"ResponseAction");
                    // Content-Disposition: attachment; filename="EbMasterData-SprData.csv"; filename*=UTF-8''EbMasterData%20-%20SprData.csv
                    file = Regex.Match(req.GetResponseHeaders()?.GetValueOrDefault("Content-Disposition") ?? "",
                        @"([a-zA-Z0-9_]+)\.csv"";").Groups[1].Value;
                    Debug.Log($"\"{file}\"");
                },
            };
            var text = await dl.Get();
            loadCache[file] = text;
            return new()
            {
                new()
                {
                    Path = file,
                    Src = src,
                },
            };
        }

        // Read text ================================================================

        public async Task ReadText()
        {
            loadedTexts.Clear();
            var max = dataForLoad.Count;
            for (int i = 0; i < max; i++)
            {
                var item = dataForLoad[i];
                if (indicatorFunc?.Invoke(i, max, $"{item.Src.DataType}: {item.Path}") ?? false) break;

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
                    loadedTexts.Add(res);
                }
            }

            for (int i = 0; i < loadedTexts.Count; i++)
            {
                Debug.Log($"[Read {i + 1}/{loadedTexts.Count}] {loadedTexts[i].Name}\n{loadedTexts[i].Text}");
            }
        }

        protected virtual async Task<LoadedText> ReadFromCustomAPI(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromResources(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromAddressables(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromStreamingAssets(LoadData item) => await ReadTextDummy(item);
        protected virtual async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item) => await ReadTextDummy(item);

        protected async Task<LoadedText> ReadTextDummy(LoadData _)
        {
            await Task.CompletedTask;
            return null;
        }

        protected async Task<LoadedText> ReadFromCache(LoadData item)
        {
            await Task.CompletedTask;

            return new(
                item.Path,
                loadCache.GetValueOrDefault(item.Path) ?? "",
                settings);
        }

        // Parse data ================================================================

        public void ParseData()
        {
            parsedValues.Clear();
            foreach (var v in loadedTexts)
            {
                parsedValues.Add(parser.Exec(v.Text, v.Format));
            }
        }

        // Convert args ================================================================

        public string[] ParsedTables => loadedTexts.Select(v => v.Name).ToArray();
        public string[][][] ParsedValuesWithHeader => parsedValues.ToArray();
        public string[][][] ParsedValues => parsedValues.Select(v => v.Skip(settings.HeaderLines).ToArray()).ToArray();

        // Etc ================================================================

        protected string PathToTableName(string str)
            => Regex.Match(str, @"([^/]+)\.[a-zA-Z0-9]+$").Groups[1].Value;
    }
}
