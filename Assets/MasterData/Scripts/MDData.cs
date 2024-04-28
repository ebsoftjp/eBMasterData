// Auto create by EbMasterData.ConvertData
using System.Linq;
using UnityEngine;

namespace MasterData
{
    public class MDData : ScriptableObject
    {
        public MDClassAdrData[] AdrData;
        public MDClassResData[] ResData;
        public MDClassStrData[] StrData;
        public MDClassSprData[] SprData;
        public MDClassSprSub1[] SprSub1;
        public MDClassSprSub2[] SprSub2;

        public MDClassBase[] GetItemsFromTableName(string tableName) => tableName switch
        {
            "AdrData" => AdrData,
            "ResData" => ResData,
            "StrData" => StrData,
            "SprData" => SprData,
            "SprSub1" => SprSub1,
            "SprSub2" => SprSub2,
            _ => null,
        };

        public void Convert2(string[] names, string[][][] data)
        {
            Debug.Assert(names.Length == data.Length, $"Convert2: {names.Length} != {data.Length}");
            AdrData = ConvertList<MDClassAdrData>("AdrData", names, data);
            ResData = ConvertList<MDClassResData>("ResData", names, data);
            StrData = ConvertList<MDClassStrData>("StrData", names, data);
            SprData = ConvertList<MDClassSprData>("SprData", names, data);
            SprSub1 = ConvertList<MDClassSprSub1>("SprSub1", names, data);
            SprSub2 = ConvertList<MDClassSprSub2>("SprSub2", names, data);
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
