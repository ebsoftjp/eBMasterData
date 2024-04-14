using System.Linq;
using UnityEngine;

namespace EbMasterData
{
    public class Settings : ScriptableObject
    {
        /// <summary>
        /// Output path
        /// </summary>
        public string OutputPath = "Assets/MasterData/Scripts/";

        /// <summary>
        /// Data file name
        /// </summary>
        public string DataFileName = "Data";

        /// <summary>
        /// Data file name
        /// </summary>
        public string DBClassesFileName = "DBClasses";

        /// <summary>
        /// Data sources
        /// </summary>
        public SettingsDataSource[] Sources;
    }

    [System.Serializable]
    public class SettingsDataSource
    {
        public enum DsDataType
        {
            CustomAPI,
            Resources,
            Addressables,
            StreamingAssets,
            GoogleSpreadSheet,
        }

        public enum DsFormat
        {
            CSV,
            JSON,
        }

        public string Path;
        public DsDataType DataType;
        public DsFormat Format;
    }
}
