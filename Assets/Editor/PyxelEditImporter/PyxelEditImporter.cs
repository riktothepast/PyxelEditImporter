using UnityEngine;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using System.Linq;

namespace net.fiveotwo.pyxelImporter
{
    public class PyxelEditImporter : Editor
    {
        [MenuItem("502 Tools/PyxelEditImporter/Re Import")]
        private static void ForceReImport()
        {
            CreateInstance<PyxelEditImporter>().ImportFiles();
        }

        private void ImportFiles()
        {
            List<DirectoryInfo> directoryInfos = CheckDirectories(Application.dataPath);
            List<FileInfo> fileInfos = new List<FileInfo>();

            foreach (DirectoryInfo info in directoryInfos)
            {
                fileInfos.AddRange(FindFiles(info));
            }

            foreach (FileInfo info in fileInfos)
            {
                OpenPyxelFile(info);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private List<DirectoryInfo> CheckDirectories(string path)
        {
            List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>();
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                directoryInfos.Add(directory);
                directoryInfos.AddRange(CheckDirectories(directory.FullName));
            }

            return directoryInfos;
        }

        private List<FileInfo> FindFiles(DirectoryInfo info)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            foreach (FileInfo f in info.GetFiles("*.pyxel"))
            {
                fileInfos.Add(f);
            }
            return fileInfos;
        }

        private void OpenPyxelFile(FileInfo info)
        {
            string folderName = info.Name.Replace(".pyxel", "");
            string newPath = $"{info.DirectoryName}/temp_{folderName}";
            string finalPath = $"{info.DirectoryName}/{folderName}_animations";
            ZipFile.ExtractToDirectory(info.FullName, newPath, true);

            using (StreamReader r = new($"{newPath}/docData.json"))
            {
                string json = r.ReadToEnd();
                DocData docData = JsonConvert.DeserializeObject<DocData>(json);
                Texture2D texture2D = new CreateSprite().CreateImage(docData, newPath, info.DirectoryName);
                string assetPath = AssetDatabase.GetAssetPath(texture2D);
                List<Sprite> animationFrames = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToList();
                Directory.CreateDirectory($"{finalPath}");
                new CreateAnimation(docData, animationFrames, $"{finalPath}");
            }
            Directory.Delete(newPath, true);
            File.Delete($"{newPath}.meta");
        }
    }
}