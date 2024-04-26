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

        protected readonly List<LoadData> files2 = new();
        public readonly List<LoadedText> data2 = new();
        public readonly List<string[][]> parsedValues = new();

        [System.Serializable]
        public class LoadedText
        {
            public string Name;
            public string Text;
        }

        public string DBClassesPath => $"{settings.OutputPath}/{settings.ClassesFileName}.cs";
        public string DBEnumsPath => $"{settings.OutputPath}/{settings.EnumsFilePath}.cs";
        public string DBDataPath => $"{settings.OutputPath}/{settings.DataFileName}.cs";

        protected readonly System.Func<int, int, string, bool> indicatorFunc;
        protected readonly Settings settings;
        protected readonly Parser parser;
        protected readonly Dictionary<string, string> tmpTextList = new();

        // Constructor ================================================================

        public Reader(System.Func<int, int, string, bool> indicatorFunc)
        {
            this.indicatorFunc = indicatorFunc;
            settings = Resources.Load<Settings>(Paths.SettingsPath);
            parser = new(settings.LineSplitString, settings.FieldSplitString);
            parser.IsOutputLog = true;
        }

        // File list ================================================================

        public async Task CreateFileList()
        {
            foreach (var src in settings.Sources ?? new SettingsDataSource[0])
            {
                Debug.Log($"{src.DataType}, {src.Path}");
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

            return new()
            {
                Name = item.Path,
                Text = loadCache.GetValueOrDefault(item.Path) ?? "",
            };
        }

        // Parse data ================================================================

        public void ParseData()
        {
            parsedValues.Clear();
            foreach (var v in data2)
            {
                parsedValues.Add(parser.Exec(v.Text));
            }
        }

        // Convert args ================================================================

        public string[] ParsedTables => data2.Select(v => v.Name).ToArray();
        public string[][][] ParsedValuesWithHeader => parsedValues.ToArray();
        public string[][][] ParsedValues => parsedValues.Select(v => v.Skip(settings.HeaderLines).ToArray()).ToArray();

        // Etc ================================================================

        protected string PathToTableName(string str)
            => Regex.Match(str, @"([^/]+)\.[a-zA-Z0-9]+$").Groups[1].Value;
    }
}
