// Auto create by EbMasterData.ConvertClasses
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
    public class MDClassResData : MDClassBase
    {
        public string Group; // Group name
        public string Type; // Type of value
        public string Value; // Value

        public MDClassResData(params string[] lines)
        {
            Id = lines[0];
            Group = lines[1];
            Type = lines[2];
            Value = lines[3];
        }
    }

    [System.Serializable]
    public class MDClassStrData : MDClassBase
    {
        public StrEnum Name; // Name of data

        public MDClassStrData(params string[] lines)
        {
            Id = lines[0];
            Name = (StrEnum)System.Enum.Parse(typeof(StrEnum), lines[1]);
        }
    }

    [System.Serializable]
    public class MDClassSprData : MDClassBase
    {
        public string Sub1Id; // Index of Sub1
        public string Sub2Id; // Index of Sub2

        public MDClassSprSub1 Sub1 => MDData.Tables.SprSub1.FirstOrDefault(v => v.Id == Sub1Id);
        public MDClassSprSub2[] Sub2 => MDData.Tables.SprSub2.Where(v => v.Id == Sub2Id).ToArray();

        public MDClassSprData(params string[] lines)
        {
            Id = lines[0];
            Sub1Id = lines[1];
            Sub2Id = lines[2];
        }
    }

    [System.Serializable]
    public class MDClassSprSub1 : MDClassBase
    {
        public string Value; // Value of",data

        public MDClassSprSub1(params string[] lines)
        {
            Id = lines[0];
            Value = lines[1];
        }
    }

    [System.Serializable]
    public class MDClassSprSub2 : MDClassBase
    {
        public int Value; // Value of data

        public MDClassSprSub2(params string[] lines)
        {
            Id = lines[0];
            Value = int.Parse(lines[1]);
        }
    }
}
