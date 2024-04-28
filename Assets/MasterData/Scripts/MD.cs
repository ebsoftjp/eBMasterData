// Auto create by EbMasterData.ConvertBase
using UnityEngine;

public class MD
{
    private const string resourcePath = "EbMasterData/Data";

    public static MasterData.MDData _Tables;
    public static MasterData.MDData Tables { get { if (!_Tables) _Tables = Resources.Load<MasterData.MDData>(resourcePath); return _Tables; } }

    public static MasterData.MDClassAdrData[] AdrData => Tables.AdrData;
    public static MasterData.MDClassResData[] ResData => Tables.ResData;
    public static MasterData.MDClassStrData[] StrData => Tables.StrData;
    public static MasterData.MDClassSprData[] SprData => Tables.SprData;
    public static MasterData.MDClassSprSub1[] SprSub1 => Tables.SprSub1;
    public static MasterData.MDClassSprSub2[] SprSub2 => Tables.SprSub2;

    public static class Config
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
}
