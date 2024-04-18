// Auto create by DBClassConvert
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

        public void Convert2(string[] res)
        {
            foreach (var item in res) Debug.Log(item);
            AdrData = ConvertList<DBClassAdrData>("AdrData", res);
            StrData = ConvertList<DBClassStrData>("StrData", res);
            SprData = ConvertList<DBClassSprData>("SprData", res);
        }

        private T[] ConvertList<T>(string key, string[] res)
        {
            return null;
        }
    }
}
