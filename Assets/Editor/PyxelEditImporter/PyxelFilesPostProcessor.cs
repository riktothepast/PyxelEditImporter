using UnityEngine;
using UnityEditor;
using System.IO;

namespace net.fiveotwo.pyxelImporter
{
    class PyxelFilesPostProcessor : AssetPostprocessor
    {

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            PyxelEditImporter pyxelEditImporter = new ();
            foreach (string str in importedAssets)
            {
                string ext = Path.GetExtension(str);
                if (ext.Contains(".pyxel"))
                {
                    pyxelEditImporter.ImportFile(str);
                }
            }
        }
    }
}