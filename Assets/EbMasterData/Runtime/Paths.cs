namespace EbMasterData
{
    public static class Paths
    {
        public const string NameSpace = "MasterData";
        public static string BasePath = $"Assets/{NameSpace}";
        public const string OpenWindowPath = "Window/eB Master Data/DB class convert";

        public const string SettingsPath = "EbMasterData/Settings";
        public static string SettingsFullPath => $"{BasePath}/Resources/{SettingsPath}.asset";
        public static string DataFullPath => $"{BasePath}/Resources/EbMasterData/Data.asset";
        public static string AsmFullPath => $"{BasePath}/{NameSpace}.asmdef";
    }
}
