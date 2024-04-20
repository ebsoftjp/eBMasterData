// Auto create by DBClassConvert
using System.Linq;
using UnityEngine;

namespace MasterData
{
    [System.Serializable]
    public abstract class DBClassBase
    {
        public string Id; // ID

        protected string Parse_string(string v) => v;
        protected int Parse_int(string v) => int.Parse(v);
    }

    [System.Serializable]
    public class DBClassAdrData : DBClassBase
    {
        public int Value; // Value of data
        public string OrderText; // Display order text

        public DBClassAdrData(params string[] lines)
        {
            Id = Parse_string(lines[0]);
            Value = Parse_int(lines[1]);
            OrderText = Parse_string(lines[2]);
        }
    }

    [System.Serializable]
    public class DBClassStrData : DBClassBase
    {
        public string Name; // Name of data

        public DBClassStrData(params string[] lines)
        {
            Id = Parse_string(lines[0]);
            Name = Parse_string(lines[1]);
        }
    }

    [System.Serializable]
    public class DBClassSprData : DBClassBase
    {
        //public SprSub1 Sub1; // Index of Sub1
        //public SprSub2[] Sub2; // Index of Sub2

        public DBClassSprData(params string[] lines)
        {
            Id = Parse_string(lines[0]);
            //Sub1 = Parse_SprSub1(lines[1]);
            //Sub2 = Parse_SprSub2Array(lines[2]);
        }
    }
}
