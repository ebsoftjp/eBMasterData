using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;
//using Newtonsoft.Json;

namespace eBMasterData
{
    public class ReaderForRuntime : Reader
    {
        // Constructor ================================================================

        public ReaderForRuntime(System.Func<int, int, string, bool> indicatorFunc) : base(indicatorFunc) { }

        // File list ================================================================

        protected override async Task<List<LoadData>> CreateFileListFromCustomAPI(SettingsDataSource src) => await CreateCustomAPI(src);

        protected override async Task<List<LoadData>> CreateFileListFromResources(SettingsDataSource src)
        {
            await Task.CompletedTask;
            var texts = Resources.LoadAll<TextAsset>(src.Path);

            foreach (var item in texts)
            {
                loadCache[item.name] = item.text;
            }

            return texts
                .Select(v => new LoadData()
                {
                    Src = src,
                    Path = v.name,
                })
                .ToList();
        }

        protected override async Task<List<LoadData>> CreateFileListFromAddressables(SettingsDataSource src)
        {
            var handle = Addressables.LoadAssetsAsync<TextAsset>(src.AddressablesLabel, null, true);
            await handle.Task;

            foreach (var item in handle.Result)
            {
                loadCache[item.name] = item.text;
            }

            return handle.Result
                .Select(v => new LoadData()
                {
                    Src = src,
                    Path = v.name,
                })
                .ToList();
        }

        protected override async Task<List<LoadData>> CreateFileListFromStreamingAssets(SettingsDataSource src)
        {
            await Task.CompletedTask;
            var path1 = src.Path;
            var path2 = "";
            if (path1.StartsWith(DownloaderText.streamingAssetsPath))
            {
                path2 = path1.Substring(DownloaderText.streamingAssetsPath.Length);
                path1 = DownloaderText.ToStreamingPath(path2);
            }
            return Directory.GetFiles(path1, "*.csv")
                .Select(v => new LoadData
                {
                    Path = path2 + v.Substring(path1.Length),
                    Src = src,
                }).ToList();
        }

        protected override async Task<List<LoadData>> CreateFileListFromGoogleSpreadSheet(SettingsDataSource src) => await CreateSpreadSheet(src);

        // Read text ================================================================

        protected override async Task<LoadedText> ReadFromCustomAPI(LoadData item) => await ReadFromCache(item);
        protected override async Task<LoadedText> ReadFromResources(LoadData item) => await ReadFromCache(item);
        protected override async Task<LoadedText> ReadFromAddressables(LoadData item) => await ReadFromCache(item);

        protected override async Task<LoadedText> ReadFromStreamingAssets(LoadData item)
        {
            var dl = new DownloaderText(item.Path, true);
            var text = await dl.Get();

            return new(
                PathToTableName(item.Path),
                text,
                settings);
        }

        protected override async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item) => await ReadFromCache(item);
    }
}
