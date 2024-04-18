using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
//using Newtonsoft.Json;

namespace EbMasterData.Editor
{
    public class ReaderForEditor : Reader
    {
        public ReaderForEditor(System.Func<int, int, string, bool> indicatorFunc) : base(indicatorFunc) { }

        public async Task CreateFileList()
        {
            foreach (var src in settings.Sources ?? new SettingsDataSource[0])
            {
                Debug.Log($"{src.DataType}, {src.Format}, {src.Path}");
                var res = src.DataType switch
                {
                    DsDataType.Addressables => await CreateFileListCommon(src),
                    DsDataType.StreamingAssets => await CreateFileListCommon(src),
                    DsDataType.GoogleSpreadSheet => await CreateFileListFromSpreadSheet(src),
                    _ => null,
                };

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

        private async Task<List<LoadData>> CreateFileListCommon(SettingsDataSource src)
        {
            await Task.CompletedTask;
            return Directory.GetFiles(src.Path, "*.csv")
                .Select(v => new LoadData
                {
                    Path = v,
                    Src = src,
                }).ToList();
        }

        private async Task<List<LoadData>> CreateFileListFromSpreadSheet(SettingsDataSource src)
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

        protected override async Task<LoadedText> ReadFromAddressables(LoadData item)
            => await ReadFromFile(item);

        protected override async Task<LoadedText> ReadFromStreamingAssets(LoadData item)
            => await ReadFromFile(item);

        protected override async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item)
        {
            await Task.CompletedTask;
            return new()
            {
                Name = item.Path,
                Text = loadCache.GetValueOrDefault(item.Path) ?? "",
            };
        }

        //protected override async Task<LoadedText> ReadFromAddressables(SettingsDataSource item)
        //    => await ReadFromFile(item);

        //protected override async Task<LoadedText> ReadFromStreamingAssets(SettingsDataSource item)
        //    => await ReadFromFile(item);

        //protected override async Task<LoadedText> ReadFromGoogleSpreadSheet(SettingsDataSource item)
        //    => await ReadFromFile(item);
    }
}
