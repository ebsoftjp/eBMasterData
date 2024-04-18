// Auto create by DBClassConvert
using System.Linq;
using UnityEngine;

namespace MasterData
{
    [System.Serializable]
    public class DBClassBase
    {
        public string Id; // ID
    }

    [System.Serializable]
    public class DBClassRate : DBClassBase
    {
        public string Rate;
    }

    [System.Serializable]
    public class DBClassAdrData : DBClassBase
    {
        public int Value; // Value of data
        public string OrderText; // Display order text
    }

    [System.Serializable]
    public class DBClassStrData : DBClassBase
    {
        public string Name; // Name of data
    }

    [System.Serializable]
    public class DBClassSprData : DBClassBase
    {
        public int Value; // Value of data
    }
}
