// Auto create by DBClassConvert
using System.Linq;
using UnityEngine;

namespace MasterData
{
    public class Data : ScriptableObject
    {
        public static Data Tables;

        public DBClassAdrData[] AdrData;
        public DBClassStrData[] StrData;
        public DBClassSprData[] SprData;

        public DBClassBase[] GetItemsFromTableName(string tableName) => tableName switch
        {
            "AdrData" => AdrData,
            "StrData" => StrData,
            "SprData" => SprData,
            _ => null,
        };

        public void Convert2(string[] names, string[][][] data)
        {
            Debug.Assert(names.Length == data.Length, $"Convert2: {names.Length} != {data.Length}");
            AdrData = ConvertList<DBClassAdrData>("AdrData", names, data);
            StrData = ConvertList<DBClassStrData>("StrData", names, data);
            SprData = ConvertList<DBClassSprData>("SprData", names, data);
        }

        private T[] ConvertList<T>(string title, string[] titles, string[][][] data) where T : DBClassBase
        {
            return data?
                .ElementAtOrDefault(System.Array.IndexOf(titles, title))?
                .Select(v => System.Activator.CreateInstance(typeof(T), new object[] { v }) as T)?
                .ToArray() ?? new T[0];
        }
    }
}
