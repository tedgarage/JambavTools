using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.Collections.Generic;

namespace Jambav.Editor
{
	public static class ScriptingDefineUtility
	{
		public static List<string> GetDefines(BuildTargetGroup buildTargetGroup)
		{
#if UNITY_2021_2_OR_NEWER
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
			string definesString = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif
			if (string.IsNullOrEmpty(definesString))
			{
				return new List<string>();
			}
			return new List<string>(definesString.Split(';'));
		}

		public static void SetDefines(BuildTargetGroup buildTargetGroup, List<string> defines)
		{
			string joined = string.Join(";", defines.ToArray());
#if UNITY_2021_2_OR_NEWER
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, joined);
#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, joined);
#endif
		}

		public static bool EnsureSymbol(string symbol, bool shouldHave, List<string> defines)
		{
			bool modified = false;
			bool has = defines.Contains(symbol);
			if (shouldHave && !has)
			{
				defines.Add(symbol);
				modified = true;
			}
			else if (!shouldHave && has)
			{
				defines.Remove(symbol);
				modified = true;
			}
			return modified;
		}

		public static bool TypeExists(string typeName)
		{
			var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				try
				{
					var type = assembly.GetType(typeName);
					if (type != null)
						return true;
				}
				catch
				{
				}
			}
			return false;
		}
	}
}


