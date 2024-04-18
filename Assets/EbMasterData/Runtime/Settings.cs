using System.Linq;
using UnityEngine;

namespace EbMasterData
{
    public class Settings : ScriptableObject
    {
        [Header("Output path for Scripts")]
        public string OutputPath = "Assets/MasterData/Scripts/";

        [Header("Data file name")]
        public string DataFileName = "MDData";

        [Header("Classes file name")]
        public string DBClassesFileName = "MDClasses";

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
