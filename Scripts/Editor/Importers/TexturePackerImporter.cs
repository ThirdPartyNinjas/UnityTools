using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

using UnityEditor;
using UnityEngine;

// Usage:
// Add this script somewhere inside your Assets folder in your Unity project.
// (For example: Assets/Scripts/Importers is what I use.)
// In TexturePacker (http://www.codeandweb.com/texturepacker), use png and Generic XML
// output formats. Make sure you disable the "Allow rotation" option.

// Notes:
// 1) If you delete a texture from the atlas, when Unity reimports, it will fill the void
//    with a sprite names DELETED_SPRITE_# (with incrementing numbers for each missing sprite.)
//    This is because the sprite objects seem to use the sprite index, and if I shift down sprites
//    to fill the empty spot, all of your sprite objects will now target the wrong images.
//    When you add a new texture, it will attempt to fill the deleted slots first.
// 2) Setting TextureImporter.spritesheet works perfectly the first time, but on reimport
//    it seemed to hang on to the cached data. I'm not sure if this is a bug, or if I'm missing
//    a way to flush the cache. Calling TextureImporter.ClearPlatformTextureSettings("Standalone")
//    seems to work. So that's what I'm doing. I don't know what side effects that might have.

namespace ThirdPartyNinjas.UnityTools.Importers
{
    public class TexturePackerImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(Path.GetExtension(assetPath) == ".png")
            {
                // If the asset is a png file, look for a matching xml file
                string atlasPath = Path.ChangeExtension(assetPath, ".xml");
                
                if(File.Exists(atlasPath))
                {
                    TextureImporter importer = assetImporter as TextureImporter;

                    // if we're not in sprite mode, nothing to do here
                    if(importer.textureType != TextureImporterType.Sprite)
                        return;
                    
                    XDocument xmlDocument = XDocument.Load(atlasPath);
                    
                    // attempt to verify that we're really looking at a TexturePacker atlas
                    if(xmlDocument.Root.Name.LocalName != "TextureAtlas")
                        return;

                    int imageHeight = int.Parse(xmlDocument.Root.Attribute("height").Value, CultureInfo.InvariantCulture);
                    
                    var sprites = xmlDocument.Descendants("sprite");
                    
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    List<SpriteMetaData> metaData = new List<SpriteMetaData>();
                    
                    foreach(var sprite in sprites)
                    {
                        string n = sprite.Attribute("n").Value;
                        int x = int.Parse(sprite.Attribute("x").Value, CultureInfo.InvariantCulture);
                        int y = int.Parse(sprite.Attribute("y").Value, CultureInfo.InvariantCulture);
                        int w = int.Parse(sprite.Attribute("w").Value, CultureInfo.InvariantCulture);
                        int h = int.Parse(sprite.Attribute("h").Value, CultureInfo.InvariantCulture);
                        int oX = (sprite.Attribute("oX") == null) ? 0 : int.Parse(sprite.Attribute("oX").Value, CultureInfo.InvariantCulture);
                        int oY = (sprite.Attribute("oY") == null) ? 0 : int.Parse(sprite.Attribute("oY").Value, CultureInfo.InvariantCulture);
                        int oW = (sprite.Attribute("oW") == null) ? w : int.Parse(sprite.Attribute("oW").Value, CultureInfo.InvariantCulture);
                        int oH = (sprite.Attribute("oH") == null) ? h : int.Parse(sprite.Attribute("oH").Value, CultureInfo.InvariantCulture);
                        bool r = sprite.Attribute("r") != null;
                        bool trim = (sprite.Attribute("oX") != null) ||
                            (sprite.Attribute("oY") != null) ||
                            (sprite.Attribute("oW") != null) ||
                            (sprite.Attribute("oH") != null);
                        
                        if(r)
                            Debug.LogWarning("Rotated TexturePacker Sprites are not currently supported. File: " + assetPath + " Sprite: " + n);
                        
                        SpriteMetaData spriteMetaData = new SpriteMetaData();
                        spriteMetaData = new SpriteMetaData();
                        // According to Unity docs, Center = 0, Custom = 9
                        spriteMetaData.alignment = trim ? 9 : 0;
                        spriteMetaData.name = n;
                        spriteMetaData.pivot = new Vector2(((oW / 2.0f) - (oX)) / (float)w, 1.0f - ((oH / 2.0f) - (oY)) / (float)h);
                        spriteMetaData.rect = new Rect(x, imageHeight - y - h, w, h);

                        metaData.Add(spriteMetaData);
                    }

                    if(importer.spritesheet == null || importer.spritesheet.Length == 0)
                    {
                        importer.spritesheet = metaData.ToArray();
                    }
                    else
                    {
                        // merge importer.spritesheet and metaData
                        // We don't want to change the indices of existing sprites
                        List<SpriteMetaData> spriteData = new List<SpriteMetaData>();

                        int deletedCount = 0;
                        SpriteMetaData deletedData = new SpriteMetaData();
                        deletedData.name = "DELETED_SPRITE";
                        deletedData.rect = new Rect(0, 0, 1, 1);

                        // Add all of the data that matches the existing data
                        foreach(var sprite in importer.spritesheet)
                        {
                            int index = metaData.FindIndex(p => p.name == sprite.name);

                            if(index != -1)
                            {
                                spriteData.Add(metaData[index]);
                                SpriteMetaData temp = metaData[index];
                                temp.name = "IGNORE_USED_SPRITE_DATA";
                                metaData[index] = temp;
                            }
                            else
                            {
                                spriteData.Add(deletedData);
                                deletedCount++;
                            }
                        }

                        // Add all of the new data
                        foreach(var sprite in metaData)
                        {
                            if(sprite.name == "IGNORE_USED_SPRITE_DATA")
                                continue;

                            if(deletedCount > 0)
                            {
                                deletedCount--;
                                int index = spriteData.FindIndex(p => p.name == "DELETED_SPRITE");
                                spriteData[index] = sprite;
                            }
                            else
                            {
                                spriteData.Add(sprite);
                            }
                        }

                        // Rename any remaning deleted entries to DELETED_SPRITE_#
                        if(deletedCount > 0)
                        {
                            for(int i=0; i<deletedCount; i++)
                            {
                                int index = spriteData.FindIndex(p => p.name == "DELETED_SPRITE");
                                SpriteMetaData temp = spriteData[index];
                                temp.name = "DELETED_SPRITE_" + i;
                                spriteData[index] = temp;
                            }
                        }

                        // Calling ClearPlatformTextureSettings to stop the texture importer
                        // from holding on the a cached version of importer.spritesheet.
                        // This is a workaround, and might be the wrong thing to do.
                        importer.ClearPlatformTextureSettings("Standalone");
                        importer.spritesheet = spriteData.ToArray();
                    }
                }
            }
        }
    }
}
