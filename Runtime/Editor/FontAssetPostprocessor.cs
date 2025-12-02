// Editor/FontAssetPostprocessor.cs
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using UnityEngine.TextCore.LowLevel;

public class FontAssetPostprocessor : AssetPostprocessor
{
    static readonly string FontsFolder = "Runtime/Resources/SettingsFonts/";
    static readonly string GeneratedAssetsFolderName = "GeneratedAssets";
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            // Check if it's a font file in our package
            if (assetPath.Contains(FontsFolder) && 
                (assetPath.EndsWith(".ttf") || assetPath.EndsWith(".otf")))
            {
                GenerateSDFFontAsset(assetPath);
            }
        }
    }

    static void GenerateSDFFontAsset(string fontPath)
    {
        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (sourceFont == null) return;

        // Get the directory and filename
        string directory = Path.GetDirectoryName(fontPath);
        string fileName = Path.GetFileNameWithoutExtension(fontPath);
        
        // Create the GeneratedAssets subfolder path
        string generatedAssetsFolder = Path.Combine(directory, GeneratedAssetsFolderName);
        string generatedAssetsFolderPath = generatedAssetsFolder.Replace('\\', '/');
        
        // Ensure the GeneratedAssets directory exists
        if (!Directory.Exists(generatedAssetsFolderPath))
        {
            Directory.CreateDirectory(generatedAssetsFolderPath);
            AssetDatabase.Refresh();
        }
        
        // Create SDF font asset path in GeneratedAssets folder
        string fontAssetPath = Path.Combine(generatedAssetsFolderPath, fileName + " SDF.asset").Replace('\\', '/');
        
        // Check if already exists using Resources path (relative to Resources folder)
        // Extract the path relative to Resources folder
        string resourcesPath = fontAssetPath.Substring(fontAssetPath.IndexOf("Resources/") + "Resources/".Length);
        resourcesPath = resourcesPath.Replace(".asset", ""); // Remove .asset extension for Resources.Load
        
        // Check if the asset already exists
        TMP_FontAsset existingAsset = Resources.Load<TMP_FontAsset>(resourcesPath);
        if (existingAsset != null)
        {
            Debug.Log($"SDF Font Asset already exists: {fontAssetPath}");
            return;
        }

        // Get character set for atlas generation
        string characterSet = GetCharacterSet();
        
        // Generate SDF font asset with Dynamic mode (allows runtime addition)
        // We'll populate it immediately to generate the atlas
        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            90,   // Sampling point size
            4,    // Padding
            GlyphRenderMode.SDFAA,
            1024, // Atlas width
            1024, // Atlas height
            AtlasPopulationMode.Dynamic, // Dynamic mode allows runtime addition
            true  // enableMultiAtlasSupport
        );

        if (fontAsset != null)
        {
            fontAsset.name = sourceFont.name + " SDF";
            
            // Populate characters immediately to generate the atlas texture
            // This is crucial - without this, the atlas will be empty
            // Convert string to uint array (unicode values)
            uint[] unicodeCharacters = new uint[characterSet.Length];
            for (int i = 0; i < characterSet.Length; i++)
            {
                unicodeCharacters[i] = characterSet[i];
            }
            
            // TryAddCharacters will automatically generate the atlas when characters are added
            fontAsset.TryAddCharacters(unicodeCharacters);
            
            // Create material for the font asset
            Material fontMaterial = new Material(Shader.Find("TextMeshPro/Distance Field"));
            fontMaterial.name = sourceFont.name + " SDF Material";
            
            // Assign the atlas texture to the material's _MainTex property
            if (fontAsset.atlasTexture != null)
            {
                fontMaterial.SetTexture("_MainTex", fontAsset.atlasTexture);
            }
            
            // Assign material to font asset
            fontAsset.material = fontMaterial;
            
            // Save font asset first
            AssetDatabase.CreateAsset(fontAsset, fontAssetPath);
            
            // Save material as sub-asset
            AssetDatabase.AddObjectToAsset(fontMaterial, fontAssetPath);
            
            // Save atlas texture as sub-asset if it exists
            if (fontAsset.atlasTexture != null)
            {
                fontAsset.atlasTexture.name = sourceFont.name + " Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAssetPath);
            }
            
            // Mark font asset as dirty to ensure changes are saved
            EditorUtility.SetDirty(fontAsset);
            EditorUtility.SetDirty(fontMaterial);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Generated SDF Font Asset with Atlas: {fontAssetPath} (Atlas: {(fontAsset.atlasTexture != null ? "Generated" : "Missing")})");
        }
    }
    
    static string GetCharacterSet()
    {
        // Generate ASCII printable characters (32-126)
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 32; i <= 126; i++)
        {
            sb.Append((char)i);
        }
        
        // Add common extended characters for better language support
        sb.Append("ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿ");
        sb.Append("€£¥©®™•…—–‚„‹›«»\"\"''");
        
        return sb.ToString();
    }
}