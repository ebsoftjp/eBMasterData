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
    public class ReaderForRuntime : Reader
    {
        public ReaderForRuntime(System.Func<int, int, string, bool> indicatorFunc) : base(indicatorFunc)
        {
        }

        protected override async Task<List<LoadData>> CreateFileListFromCustomAPI(SettingsDataSource src) => await CreateFileListDummy(src);
        protected override async Task<List<LoadData>> CreateFileListFromResources(SettingsDataSource src) => await CreateFileListDummy(src);
        protected override async Task<List<LoadData>> CreateFileListFromAddressables(SettingsDataSource src) => await CreateFileListDummy(src);
        protected override async Task<List<LoadData>> CreateFileListFromStreamingAssets(SettingsDataSource src) => await CreateFileListDummy(src);
        protected override async Task<List<LoadData>> CreateFileListFromGoogleSpreadSheet(SettingsDataSource src) => await CreateFileListDummy(src);

        protected override async Task<LoadedText> ReadFromCustomAPI(LoadData item)
        {
            var dl = new DownloaderText(item.Path, false);
            var text = await dl.Get();
            return null;
        }

        protected override async Task<LoadedText> ReadFromAddressables(LoadData item)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(item.Path);
            await handle.Task;

            return new()
            {
                Name = item.Path,
                Text = handle.Result.text,
            };
        }

        protected override async Task<LoadedText> ReadFromStreamingAssets(LoadData item)
        {
            var dl = new DownloaderText(item.Path, true);
            var text = await dl.Get();
            return null;
        }

        protected override async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item)
        {
            var dl = new DownloaderText(item.Path, false)
            {
                ResponseAction = req =>
                {
                    var file = Regex.Match(req.GetResponseHeaders()?.GetValueOrDefault("Content-Disposition") ?? "",
                        @"([a-zA-Z0-9_]+)\.csv"";").Groups[1].Value;
                },
            };
            var text = await dl.Get();
            return null;
        }
    }
}
