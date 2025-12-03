// Editor/FontAssetPostprocessor.cs
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using UnityEngine.TextCore.LowLevel;

public class FontAssetPostprocessor : AssetPostprocessor
{
    static readonly string FontsFolder = "Runtime/Resources/SettingsFonts/";
    // Project-specific folder outside the package (in the project's Assets root)
    static readonly string ProjectFontAssetsFolder = "Assets/JambavTools/Settings/FontAssets";
    
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

        // Get the filename
        string fileName = Path.GetFileNameWithoutExtension(fontPath);
        
        // Ensure the project-specific FontAssets directory exists
        // Create full directory structure if it doesn't exist
        if (!Directory.Exists(ProjectFontAssetsFolder))
        {
            // Create all parent directories if they don't exist
            Directory.CreateDirectory(ProjectFontAssetsFolder);
            
            // Create .gitkeep file to ensure the folder is tracked in git
            string gitkeepPath = Path.Combine(ProjectFontAssetsFolder, ".gitkeep");
            File.WriteAllText(gitkeepPath, "");
            
            AssetDatabase.Refresh();
            Debug.Log($"[FontAssetPostprocessor] Created project font assets folder: {ProjectFontAssetsFolder}");
        }
        
        // Create SDF font asset path in project folder
        string fontAssetPath = Path.Combine(ProjectFontAssetsFolder, fileName + " SDF.asset").Replace('\\', '/');
        
        // Check if the asset already exists
        TMP_FontAsset existingAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
        if (existingAsset != null)
        {
            Debug.Log($"[FontAssetPostprocessor] SDF Font Asset already exists: {fontAssetPath}");
            return;
        }

        // Get character set for atlas generation
        string characterSet = GetCharacterSet();
        
        // Generate SDF font asset with Dynamic mode (allows runtime addition)
        // We'll populate it immediately to generate the atlas
        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            120,   // Sampling point size
            9,    // Padding
            GlyphRenderMode.SDFAA,
            2048, // Atlas width
            2048, // Atlas height
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