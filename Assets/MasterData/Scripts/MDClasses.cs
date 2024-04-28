// Auto create by EbMasterData.ConvertClasses
using System.Linq;
using UnityEngine;

namespace MasterData
{
    [System.Serializable]
    public abstract class MDClassBase
    {
        public string Id; // ID

        protected int[] ToIntArray(string s) => s.Replace(" ", "").Split(",").Select(v => int.Parse(v)).ToArray();
        protected float[] ToFloatArray(string s) => s.Replace(" ", "").Split(",").Select(v => float.Parse(v)).ToArray();

        protected Vector2 ToVector2(string s)
        {
            var v = ToFloatArray(s);
            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1));
        }

        protected Vector2Int ToVector2Int(string s)
        {
            var v = ToIntArray(s);
            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1));
        }

        protected Vector3 ToVector3(string s)
        {
            var v = ToFloatArray(s);
            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1), v.ElementAtOrDefault(2));
        }

        protected Vector3Int ToVector3Int(string s)
        {
            var v = ToIntArray(s);
            return new(v.ElementAtOrDefault(0), v.ElementAtOrDefault(1), v.ElementAtOrDefault(2));
        }

        protected T ToEnum<T>(string s)
        {
            return string.IsNullOrEmpty(s) ? (T)System.Enum.ToObject(typeof(T), 0) : (T)System.Enum.Parse(typeof(T), s);
        }
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
        public int Main_FrameRate; // Frame rate
        public string Main_Caution; // Caution message
        public Vector3 Main_Center1; // Center position 1
        public Vector3Int Main_Center2; // Center position 2
        public StrEnum Test_EnumTest; // Enum test value

        public MDClassResData(params string[] lines)
        {
            Main_FrameRate = int.Parse(lines[0]);
            Main_Caution = lines[1];
            Main_Center1 = ToVector3(lines[2]);
            Main_Center2 = ToVector3Int(lines[3]);
            Test_EnumTest = ToEnum<StrEnum>(lines[4]);
        }
    }

    [System.Serializable]
    public class MDClassStrData : MDClassBase
    {
        public StrEnum Name; // Name of data

        public MDClassStrData(params string[] lines)
        {
            Id = lines[0];
            Name = ToEnum<StrEnum>(lines[1]);
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
