using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using UnityEditor;
using UnityEngine;

// Usage:
// In TexturePacker (http://www.codeandweb.com/texturepacker), use the Generic XML output format.
// Make sure you disable the "Allow rotation" option.

// Once you import/update the png and xml files into your project (they must be in the same folder),
// if the sprites don't look right, change one of the TextureAsset import settings,
// then change it back and hit apply. I haven't yet identified why the changes don't always take
// right away. (Right click your texture and choose reimport, if you added the xml file, or this
// script after the png had already been processed.)

namespace ThirdPartyNinjas.UnityTools.Importers
{
    public class TexturePackerImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets,
                                           string[] deletedAssets,
                                           string[] movedAssets,
                                           string[] movedFromAssetPaths)
        {
            foreach(string assetPath in importedAssets)
            {
                // If the asset being imported in a png file, look for an xml with the same name next to it
                if(Path.GetExtension(assetPath) == ".png")
                {
                    string atlasPath = Path.ChangeExtension(assetPath, ".xml");

                    if(File.Exists(atlasPath))
                    {
                        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);

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
                        SpriteMetaData[] metaData = new SpriteMetaData[sprites.Count()];

                        int currentSprite = 0;

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

                            metaData[currentSprite++] = spriteMetaData;
                        }

                        importer.spritesheet = metaData;
                    }
                }
            }
        }
    }
}
