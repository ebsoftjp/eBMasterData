// Auto create by EbMasterData.ConvertData
using System.Linq;
using UnityEngine;

namespace MasterData
{
    public class MDData : ScriptableObject
    {
        public static MDData Tables;

        public MDClassAdrData[] AdrData;
        public MDClassResData[] ResData;
        public MDClassStrData[] StrData;

        public MDClassBase[] GetItemsFromTableName(string tableName) => tableName switch
        {
            "AdrData" => AdrData,
            "ResData" => ResData,
            "StrData" => StrData,
            _ => null,
        };

        public void Convert2(string[] names, string[][][] data)
        {
            Debug.Assert(names.Length == data.Length, $"Convert2: {names.Length} != {data.Length}");
            AdrData = ConvertList<MDClassAdrData>("AdrData", names, data);
            ResData = ConvertList<MDClassResData>("ResData", names, data);
            StrData = ConvertList<MDClassStrData>("StrData", names, data);
        }

        private T[] ConvertList<T>(string title, string[] titles, string[][][] data) where T : MDClassBase
        {
            return data?
                .ElementAtOrDefault(System.Array.IndexOf(titles, title))?
                .Select(v => System.Activator.CreateInstance(typeof(T), new object[] { v }) as T)?
                .ToArray() ?? new T[0];
        }
    }
}
