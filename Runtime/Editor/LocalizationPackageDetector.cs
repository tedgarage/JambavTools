using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Build;
using System.Collections.Generic;

namespace Jambav.Editor
{
    [InitializeOnLoad]
    public class LocalizationPackageDetector
    {
        private const string LOCALIZATION_SYMBOL = "LOCALIZATION_AVAILABLE";

        static LocalizationPackageDetector()
        {
            UpdateScriptingDefineSymbols();
        }

        [MenuItem("Tools/Jambav/Update Localization Package Symbols")]
        public static void UpdateScriptingDefineSymbols()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			List<string> defines = ScriptingDefineUtility.GetDefines(buildTargetGroup);

			// Unity Localization lives in the Unity.Localization assembly.
			// Use well-known types to detect presence without adding a hard assembly reference.
			bool hasLocalization =
				ScriptingDefineUtility.TypeExists("UnityEngine.Localization.Settings.LocalizationSettings") ||
				ScriptingDefineUtility.TypeExists("UnityEngine.Localization.Locale"); // common runtime type

            // Debug.Log("Checking for Unity Localization package...");
            // Debug.Log("Unity Localization package found: " + hasLocalization);

            bool modified = false;

            // Handle Localization
            if (ScriptingDefineUtility.EnsureSymbol(LOCALIZATION_SYMBOL, hasLocalization, defines))
            {
                modified = true;
                Debug.Log("Unity Localization package detected. Added " + LOCALIZATION_SYMBOL + " symbol.");
            }
            else if (!hasLocalization && defines.Contains(LOCALIZATION_SYMBOL))
            {
                modified = true;
                Debug.Log("Unity Localization package not found. Removed " + LOCALIZATION_SYMBOL + " symbol.");
            }

            if (modified)
            {
				ScriptingDefineUtility.SetDefines(buildTargetGroup, defines);
                Debug.Log("Localization scripting define symbols updated successfully.");
            }
        }
    }
}