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
        public readonly List<EnumData> data4 = new();

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

        [System.Serializable]
        public class EnumData
        {
            public string name;
            public string[] values;
        }

        // Constructor ================================================================

        public ReaderForEditor(System.Func<int, int, string, bool> indicatorFunc) : base(indicatorFunc) { }

        // File list ================================================================

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

        // Read text ================================================================

        protected override async Task<LoadedText> ReadFromCustomAPI(LoadData item) => await ReadFromCache(item);
        protected override async Task<LoadedText> ReadFromResources(LoadData item) => await ReadFromFile(item);
        protected override async Task<LoadedText> ReadFromAddressables(LoadData item) => await ReadFromFile(item);
        protected override async Task<LoadedText> ReadFromStreamingAssets(LoadData item) => await ReadFromFile(item);
        protected override async Task<LoadedText> ReadFromGoogleSpreadSheet(LoadData item) => await ReadFromCache(item);

        protected async Task<LoadedText> ReadFromFile(LoadData item)
        {
            using var sr = new StreamReader(item.Path);
            var res = await sr.ReadToEndAsync();
            sr.Close();

            return new(
                PathToTableName(item.Path),
                res,
                settings);
        }

        // Create header data ================================================================

        public void CreateHeaderData()
        {
            data3.Clear();
            var tables = ParsedTables;
            var values1 = ParsedValuesWithHeader;
            for (int i = 0; i < tables.Length; i++)
            {
                data3.Add(TextToData(tables[i], values1[i]));
            }

            data4.Clear();
            var values2 = ParsedValues;
            for (int i = 0; i < settings.Enums.Length; i++)
            {
                var enumName = settings.Enums[i];
                var enumValues = new List<string>();

                for (int j = 0; j < tables.Length; j++)
                {
                    var keyIndex = System.Array.IndexOf(values1[j].ElementAtOrDefault(1), enumName);
                    if (keyIndex > -1)
                    {
                        enumValues.AddRange(values2[j].Select(v => v[keyIndex]));
                    }
                }

                data4.Add(new()
                {
                    name = enumName,
                    values = enumValues.OrderBy(v => v).Distinct().Where(v => v != "").ToArray(),
                });
            }
        }

        protected KeysData2 TextToData(string tableName, string[][] lines)
        {
            return new()
            {
                name = tableName,
                keys = Enumerable.Repeat(0, lines.FirstOrDefault()?.Count() ?? 0).Select((_, n) => new KeysData3()
                {
                    key = lines.ElementAtOrDefault(0)?.ElementAtOrDefault(n) ?? "",
                    type = lines.ElementAtOrDefault(1)?.ElementAtOrDefault(n) ?? "",
                    comment = lines.ElementAtOrDefault(2)?.ElementAtOrDefault(n) ?? "",
                }).ToArray(),
            };
        }
    }
}
