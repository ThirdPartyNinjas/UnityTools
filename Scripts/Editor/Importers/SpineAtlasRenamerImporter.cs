using System.IO;

using UnityEditor;

namespace ThirdPartyNinjas.UnityTools.Importers
{
    public class SpineAtlasRenamerImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets,
                                           string[] deletedAssets,
                                           string[] movedAssets,
                                           string[] movedFromAssetPaths)
        {
            bool refreshNeeded = false;
 
            foreach(var assetPath in importedAssets)
            {
                if(Path.GetExtension(assetPath) == ".atlas")
                {
                    string newAssetPath = Path.ChangeExtension(assetPath, ".atlas.txt");
 
                    if(File.Exists(newAssetPath))
                        AssetDatabase.DeleteAsset(newAssetPath);

                    FileUtil.MoveFileOrDirectory(assetPath, newAssetPath);
 
                    refreshNeeded = true;
                }
            }

            if(refreshNeeded)
                AssetDatabase.Refresh();
        }
    }
}
