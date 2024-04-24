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
        public readonly List<KeysData2> data3 = new();

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

        public ReaderForEditor(System.Func<int, int, string, bool> indicatorFunc) : base(indicatorFunc) { }

        public void CreateData()
        {
            data3.Clear();
            for (int i = 0; i < data2.Count; i++)
            {
                data3.Add(TextToData(data2[i]));
            }
        }

        protected KeysData2 TextToData(LoadedText data)
        {
            var parser = new Parser(settings.LineSplitString, settings.FieldSplitString);
            parser.IsOutputLog = true;
            var lines = parser.Exec(data.Text);

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

        protected override async Task<List<LoadData>> CreateFileListFromCustomAPI(SettingsDataSource src) => await CreateCustomAPI(src);
        protected override async Task<List<LoadData>> CreateFileListFromResources(SettingsDataSource src) => await CreateFileListIO(src);
        protected override async Task<List<LoadData>> CreateFileListFromAddressables(SettingsDataSource src) => await CreateFileListIO(src);
        protected override async Task<List<LoadData>> CreateFileListFromStreamingAssets(SettingsDataSource src) => await CreateFileListIO(src);
        protected override async Task<List<LoadData>> CreateFileListFromGoogleSpreadSheet(SettingsDataSource src) => await CreateSpreadSheet(src);

        private async Task<List<LoadData>> CreateFileListIO(SettingsDataSource src)
        {
            await Task.CompletedTask;
            return Directory.GetFiles(src.Path, "*.csv")
                .Select(v => new LoadData
                {
                    Path = v,
                    Src = src,
                }).ToList();
        }

        protected override async Task<LoadedText> ReadFromCustomAPI(LoadData item) => await ReadFromCache(item);
        protected override async Task<LoadedText> ReadFromAddressables(LoadData item) => await ReadFromFile(item);
        protected override async Task<LoadedText> ReadFromStreamingAssets(LoadData item) => await ReadFromFile(item);
        protected override async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item) => await ReadFromCache(item);

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
    }
}
