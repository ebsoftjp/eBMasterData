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

            if (GUI.Button(new Rect(40.0f, 60.0f, 120.0f, 20.0f), "Init"))
            {
                Convert.InitData();
            }

            if (GUI.Button(new Rect(40.0f, 100.0f, 120.0f, 20.0f), "Convert"))
            {
                Convert.ConvertDBClasses();
            }

            if (GUI.Button(new Rect(40.0f, 140.0f, 120.0f, 20.0f), "Close"))
            {
                this.Close();
            }
        }
    }
}
