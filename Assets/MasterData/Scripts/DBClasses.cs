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
        public string Value; // Value of data
        public string OrderText; // Display order text
    }
}
