// Auto create by DBClassConvert
using System.Linq;
using UnityEngine;

namespace MasterData
{
    [System.Serializable]
    public class DBClassBase
    {
        public string Id; // ID

        public DBClassBase(string text)
        {
        }
    }

    [System.Serializable]
    public class DBClassRate : DBClassBase
    {
        public string Rate;

        public DBClassRate(string text) : base(text)
        {
        }
    }

    [System.Serializable]
    public class DBClassAdrData : DBClassBase
    {
        public int Value; // Value of data
        public string OrderText; // Display order text

        public DBClassAdrData(string text) : base(text)
        {
            var lines = text.Split(",");
            Id = lines[0];
            Value = int.Parse(lines[1]);
            OrderText = lines[2];
        }
    }

    [System.Serializable]
    public class DBClassStrData : DBClassBase
    {
        public string Name; // Name of data

        public DBClassStrData(string text) : base(text)
        {
            var lines = text.Split(",");
            Id = lines[0];
            Name = lines[1];
        }
    }

    [System.Serializable]
    public class DBClassSprData : DBClassBase
    {
        public int Value; // Value of data

        public DBClassSprData(string text) : base(text)
        {
            var lines = text.Split(",");
            Id = lines[0];
            Value = int.Parse(lines[1]);
        }
    }
}
