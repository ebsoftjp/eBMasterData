// Auto create by DBClassConvert
using System.Linq;
using UnityEngine;

namespace MasterData
{
    [System.Serializable]
    public abstract class MDClassBase
    {
        public string Id; // ID
    }

    [System.Serializable]
    public class MDClassAdrData : MDClassBase
    {
        public int Value; // Value of data
        public string OrderText; // Display order text

        public MDClassAdrData(params string[] lines)
        {
            Id = lines[0];
            Value = int.Parse(lines[1]);
            OrderText = lines[2];
        }
    }

    [System.Serializable]
    public class MDClassStrData : MDClassBase
    {
        public string Name; // Name of data

        public MDClassStrData(params string[] lines)
        {
            Id = lines[0];
            Name = lines[1];
        }
    }

    [System.Serializable]
    public class MDClassSprData : MDClassBase
    {
        public string Sub1; // Index of Sub1
        public string Sub2; // Index of Sub2

        public MDClassSprData(params string[] lines)
        {
            Id = lines[0];
            Sub1 = lines[1];
            Sub2 = lines[2];
        }
    }
}
