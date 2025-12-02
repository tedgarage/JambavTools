using UnityEngine;
using TMPro;
using UnityEditor;
using System.IO;

/// <summary>
/// Automatically assigns the generated SDF font asset to TextMeshProUGUI
/// when a TTF or OTF font file is assigned to this component.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class AutoFontAssigner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Assign the original font file (TTF/OTF), and the script will find and assign the generated SDF font asset")]
    private Font sourceFont;
    
    private TextMeshProUGUI textMeshPro;
    private Font lastSourceFont;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Get the TextMeshProUGUI component
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        // Check if the source font has changed
        if (sourceFont != lastSourceFont)
        {
            lastSourceFont = sourceFont;
            
            if (sourceFont != null)
            {
                AssignSDFFontAsset();
            }
        }
    }

    private void AssignSDFFontAsset()
    {
        if (sourceFont == null || textMeshPro == null)
            return;

        // Get the path of the source font
        string fontPath = AssetDatabase.GetAssetPath(sourceFont);
        
        if (string.IsNullOrEmpty(fontPath))
        {
            Debug.LogWarning($"[AutoFontAssigner] Could not find path for font: {sourceFont.name}");
            return;
        }

        // Get the directory and filename
        string directory = Path.GetDirectoryName(fontPath);
        string fileName = Path.GetFileNameWithoutExtension(fontPath);
        
        // Look for the SDF font asset in GeneratedAssets subfolder
        string generatedAssetsFolder = Path.Combine(directory, "GeneratedAssets");
        string sdfFontPath = Path.Combine(generatedAssetsFolder, fileName + " SDF.asset").Replace('\\', '/');

        // Check if the SDF font asset exists
        if (!File.Exists(sdfFontPath))
        {
            Debug.LogWarning($"[AutoFontAssigner] SDF font asset not found at: {sdfFontPath}\n" +
                           $"Please ensure the font is in the correct folder (Runtime/Resources/SettingsFonts/) " +
                           $"and the SDF font asset has been generated in the GeneratedAssets subfolder.");
            return;
        }

        // Load the SDF font asset
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(sdfFontPath);

        if (fontAsset != null)
        {
            // Assign the font asset to TextMeshProUGUI
            Undo.RecordObject(textMeshPro, "Assign SDF Font");
            textMeshPro.font = fontAsset;
            EditorUtility.SetDirty(textMeshPro);
            
            Debug.Log($"[AutoFontAssigner] Successfully assigned SDF font asset: {fontAsset.name} to {gameObject.name}");
        }
        else
        {
            Debug.LogError($"[AutoFontAssigner] Failed to load SDF font asset from: {sdfFontPath}");
        }
    }

    private void Reset()
    {
        // Initialize component reference when added
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
#endif
}

