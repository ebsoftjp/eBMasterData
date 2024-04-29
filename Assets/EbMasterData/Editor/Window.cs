using UnityEngine;
using UnityEditor;

namespace eBMasterData.Editor
{
    class Window : EditorWindow
    {
        [MenuItem(Paths.OpenWindowPath)]
        public static void Open()
        {
            GetWindowWithRect<Window>(new Rect(0, 0, 320, 240), true, "eB Master Data - Settings", true);
        }

        private void OnGUI()
        {
            // header
            EditorGUILayout.BeginVertical();
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("eB Master Data control panel");
            var rect = new Rect(40.0f, 60.0f, 240.0f, 20.0f);
            var cv = new Convert();

            if (GUI.Button(rect, "Init"))
            {
                cv.InitData();
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
                cv.ConvertDBClasses();
            }

            rect.y += addy;
            if (GUI.Button(rect, "Dump"))
            {
                cv.DumpData();
            }

            rect.y += addy;
            if (GUI.Button(rect, "Close"))
            {
                Close();
            }
        }
    }
}
