using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using static System.IO.Directory;
using static UnityEditor.AssetDatabase;

namespace Jambav.Editor
{
    public class ToolsMenu : MonoBehaviour
    {
        /// <summary>
        /// Creating default folders for the initial project setup
        /// </summary>
        [MenuItem("Tools/Initial Setup/Create Default Folders")]
        public static void CreateDefaultFoldersInProject()
        {

            CreateDir("_Projects", "Scenes", "Sprites", "Scripts", "Prefabs", "Audios", "Fonts");
            if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode3D)
            {
                CreateDir("_Projects", "Models", "Textures", "Materials");
            }

            Refresh();

        }

        public static void CreateDir(string _rootPath, params string[] _dirNames)
        {
            string fullPath = Path.Combine(Application.dataPath, _rootPath);

            foreach (var dirName in _dirNames)
            {
                CreateDirectory(Path.Combine(fullPath, dirName));
            }

        }
    }
}
