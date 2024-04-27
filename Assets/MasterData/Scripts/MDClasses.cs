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
        public int FrameRate; // Frame rate
        public string Caution; // Caution message
        public StrEnum EnumTest; // Enum test value

        public MDClassResData(params string[] lines)
        {
            FrameRate = int.Parse(lines[0]);
            Caution = lines[1];
            EnumTest = (StrEnum)System.Enum.Parse(typeof(StrEnum), lines[2]);
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
}
