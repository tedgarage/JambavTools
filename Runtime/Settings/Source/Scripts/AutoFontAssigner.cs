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
    // Resources path (relative to Resources folder)
    static readonly string ResourcesPath = "SettingsFonts/GeneratedAssets/{0} SDF";


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

        // Generate the Resources path for the SDF font asset
        string resourcePath = string.Format(ResourcesPath, sourceFont.name);

        // Load the SDF font asset from Resources
        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>(resourcePath);

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
            Debug.LogWarning($"[AutoFontAssigner] SDF font asset not found at Resources path: {resourcePath}\n" +
                           $"Please ensure the font '{sourceFont.name}' has a generated SDF asset in " +
                           $"Runtime/Resources/SettingsFonts/GeneratedAssets/");
        }
    }

    private void Reset()
    {
        // Initialize component reference when added
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
#endif
}

