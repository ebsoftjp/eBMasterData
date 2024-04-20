using UnityEngine;
using UnityEditor;

namespace EbMasterData.Editor
{
    class Window : EditorWindow
    {
        [MenuItem(Paths.OpenWindowPath)]
        public static void Open()
        {
            GetWindowWithRect<Window>(new Rect(0, 0, 320, 240), true, "EbMasterData - Settings", true);
        }

        private void OnGUI()
        {
            // header
            EditorGUILayout.BeginVertical();
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("EbMasterData control panel");
            var rect = new Rect(40.0f, 60.0f, 240.0f, 20.0f);

            if (GUI.Button(rect, "Init"))
            {
                Convert.InitData();
            }

            var addy = 32f;
            rect.y += addy;
            if (GUI.Button(rect, "Open Settings"))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Settings>(Paths.SettingsFullPath);
            }

            rect.y += addy;
            if (GUI.Button(rect, "Convert"))
            {
                Convert.ConvertDBClasses();
            }

            rect.y += addy;
            if (GUI.Button(rect, "Dump"))
            {
                Convert.DumpData();
            }

            rect.y += addy;
            if (GUI.Button(rect, "Close"))
            {
                Close();
            }
        }
    }
}
