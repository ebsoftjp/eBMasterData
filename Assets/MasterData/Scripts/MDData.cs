// Auto create by DBClassConvert
using System.Linq;
using UnityEngine;

namespace MasterData
{
    public class Data : ScriptableObject
    {
        protected const string returnCode = "\r\n";
        protected const string commaCode = ",";

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

        public void Convert2(string[][] res)
        {
            foreach (var item in res) Debug.Log(item);
            AdrData = ConvertList<DBClassAdrData>(res.FirstOrDefault(v => v[0] == "AdrData"));
            StrData = ConvertList<DBClassStrData>(res.FirstOrDefault(v => v[0] == "StrData"));
            SprData = ConvertList<DBClassSprData>(res.FirstOrDefault(v => v[0] == "SprData"));
        }

        private T[] ConvertList<T>(string[] res) where T : DBClassBase
        {
            return res[1]
                .Split(returnCode)
                .Skip(3)
                .Select(v => System.Activator.CreateInstance(typeof(T), new object[] { v.Split(commaCode) }) as T)
                .ToArray();
        }
    }
}
