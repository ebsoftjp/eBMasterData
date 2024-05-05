// Auto create by eBMasterData.ConvertBase
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MD
{
    private const string resourcePath = "eBMasterData/Data";

    public static MasterData.MDData _Tables;
    public static MasterData.MDData Tables { get { if (!_Tables) _Tables = Resources.Load<MasterData.MDData>(resourcePath); return _Tables; } }

    public static MasterData.MDClassAdrData[] AdrData => Tables.AdrData;

    public static class ResData
    {
        public static class Main
        {
            public static int FrameRate => Tables.ResData[0].Main_FrameRate;
            public static string Caution => Tables.ResData[0].Main_Caution;
            public static Vector3 Center1 => Tables.ResData[0].Main_Center1;
            public static Vector3Int Center2 => Tables.ResData[0].Main_Center2;
        }

        public static class Test
        {
            public static MasterData.StrEnum EnumTest => Tables.ResData[0].Test_EnumTest;
        }
    }
    public static MasterData.MDClassStrData[] StrData => Tables.StrData;
    public static MasterData.MDClassSprData[] SprData => Tables.SprData;
    public static MasterData.MDClassSprSub1[] SprSub1 => Tables.SprSub1;
    public static MasterData.MDClassSprSub2[] SprSub2 => Tables.SprSub2;

    public static T At<T>(this IList<T> self, string key) where T : MasterData.MDClassBase => self.FirstOrDefault(v => v.Id == key);
    public static T[] ArrayAt<T>(this IList<T> self, string key) where T : MasterData.MDClassBase => self.Where(v => v.Id == key).ToArray();
}
