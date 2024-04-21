using System.Linq;
using UnityEngine;

namespace EbMasterData
{
    public class Settings : ScriptableObject
    {
        [Header("Output path for Scripts")]
        public string OutputPath = "Assets/MasterData/Scripts/";

        [Header("Namespace name")]
        public string NamespaceName = "MasterData";

        [Header("Data file name")]
        public string DataFileName = "MDData";

        [Header("Classes file name")]
        public string ClassesFileName = "MDClasses";

        [Header("Data line split string")]
        public string LineSplitString = "\\r\\n";

        [Header("Data field split string")]
        public string FieldSplitString = ",";

        [Header("Class prefix name")]
        public string ClassNamePrefix = "MDClass";

        [Header("Class base name")]
        public string ClassNameBase = "Base";

        [Header("Class primary key")]
        public string ClassPrimaryKey = "Id";

        [Header("Data header line count")]
        public int HeaderLines = 3;

        [Header("Data sources")]
        public SettingsDataSource[] Sources;
    }

    [System.Serializable]
    public class SettingsDataSource
    {
        [Header("Data path")]
        public string Path;

        [Header("Label for Addressables")]
        public string AddressablesLabel;

        [Header("Data type")]
        public DsDataType DataType;

        [Header("Data format")]
        public DsFormat Format;
    }
}
