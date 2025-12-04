using UnityEngine;
using TMPro;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class AutoFontAssigner : MonoBehaviour
{
//     [SerializeField]
//     [Tooltip("Assign the original font file (TTF/OTF), and the script will find and assign the generated SDF font asset")]
//     private Font sourceFont;
    
//     private TextMeshProUGUI textMeshPro;
//     private Font lastSourceFont;
//     // Project-specific folder path (outside the package)
//     static readonly string ProjectFontAssetsFolder = "Assets/JambavTools/Settings/FontAssets";


// #if UNITY_EDITOR
//     private void OnValidate()
//     {
//         // Get the TextMeshProUGUI component
//         if (textMeshPro == null)
//         {
//             textMeshPro = GetComponent<TextMeshProUGUI>();
//         }

//         // Check if the source font has changed
//         if (sourceFont != lastSourceFont)
//         {
//             lastSourceFont = sourceFont;
            
//             if (sourceFont != null)
//             {
//                 AssignSDFFontAsset();
//             }
//         }
//     }

//     private void AssignSDFFontAsset()
//     {
//         if (sourceFont == null || textMeshPro == null)
//             return;

//         // Generate the asset path for the SDF font asset
//         string fontAssetPath = $"{ProjectFontAssetsFolder}/{sourceFont.name} SDF.asset";

//         // Load the SDF font asset from project folder
//         TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);

//         if (fontAsset != null)
//         {
//             // Assign the font asset to TextMeshProUGUI
//             Undo.RecordObject(textMeshPro, "Assign SDF Font");
//             textMeshPro.font = fontAsset;
//             EditorUtility.SetDirty(textMeshPro);
            
//             // Debug.Log($"[AutoFontAssigner] Successfully assigned SDF font asset: {fontAsset.name} to {gameObject.name}");
//         }
//         else
//         {
//             // Debug.LogWarning($"[AutoFontAssigner] SDF font asset not found at: {fontAssetPath}\n" +
//             //                $"Please ensure the font '{sourceFont.name}' has a generated SDF asset in " +
//             //                $"{ProjectFontAssetsFolder}/\n" +
//             //                $"Import the font file from the package to generate it automatically.");
//         }
//     }

//     private void Reset()
//     {
//         // Initialize component reference when added
//         textMeshPro = GetComponent<TextMeshProUGUI>();
//     }
// #endif
}

