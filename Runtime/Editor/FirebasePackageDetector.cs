using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Build;
using System.Collections.Generic;

namespace Jambav.Editor
{
    [InitializeOnLoad]
    public class FirebasePackageDetector
    {
        private const string FIREBASE_AUTH_SYMBOL = "FIREBASE_AUTH_AVAILABLE";
        private const string FIREBASE_DATABASE_SYMBOL = "FIREBASE_DATABASE_AVAILABLE";
        private const string FIREBASE_FUNCTIONS_SYMBOL = "FIREBASE_FUNCTIONS_AVAILABLE";

        static FirebasePackageDetector()
        {
            UpdateScriptingDefineSymbols();
        }

        [MenuItem("Tools/Jambav/Update Firebase Package Symbols")]
        public static void UpdateScriptingDefineSymbols()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			List<string> defines = ScriptingDefineUtility.GetDefines(buildTargetGroup);

            
            bool hasAuth = ScriptingDefineUtility.TypeExists("Firebase.Auth.FirebaseAuth");
            bool hasDatabase = ScriptingDefineUtility.TypeExists("Firebase.Database.FirebaseDatabase");
            bool hasFunctions = ScriptingDefineUtility.TypeExists("Firebase.Functions.FirebaseFunctions");

            bool modified = false;

            // Handle Firebase Auth
            if (ScriptingDefineUtility.EnsureSymbol(FIREBASE_AUTH_SYMBOL, hasAuth, defines))
            {
				modified = true;
                Debug.Log("Firebase Auth package detected. Added " + FIREBASE_AUTH_SYMBOL + " symbol.");
            }
            else if (!hasAuth && defines.Contains(FIREBASE_AUTH_SYMBOL))
            {
				modified = true;
                Debug.Log("Firebase Auth package not found. Removed " + FIREBASE_AUTH_SYMBOL + " symbol.");
            }

            // Handle Firebase Database
            if (ScriptingDefineUtility.EnsureSymbol(FIREBASE_DATABASE_SYMBOL, hasDatabase, defines))
            {
				modified = true;
                Debug.Log("Firebase Database package detected. Added " + FIREBASE_DATABASE_SYMBOL + " symbol.");
            }
            else if (!hasDatabase && defines.Contains(FIREBASE_DATABASE_SYMBOL))
            {
				modified = true;
                Debug.Log("Firebase Database package not found. Removed " + FIREBASE_DATABASE_SYMBOL + " symbol.");
            }

            // Handle Firebase Functions
            if (ScriptingDefineUtility.EnsureSymbol(FIREBASE_FUNCTIONS_SYMBOL, hasFunctions, defines))
            {
				modified = true;
                Debug.Log("Firebase Functions package detected. Added " + FIREBASE_FUNCTIONS_SYMBOL + " symbol.");
            }
            else if (!hasFunctions && defines.Contains(FIREBASE_FUNCTIONS_SYMBOL))
            {
				modified = true;
                Debug.Log("Firebase Functions package not found. Removed " + FIREBASE_FUNCTIONS_SYMBOL + " symbol.");
            }

            if (modified)
            {
				ScriptingDefineUtility.SetDefines(buildTargetGroup, defines);
                Debug.Log("Firebase scripting define symbols updated successfully.");
            }
        }

    }
}


