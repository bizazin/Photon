using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public class ClearConfigEditor
    {
        private static readonly string PersistantPath = Application.persistentDataPath;

#if UNITY_EDITOR
        [MenuItem("Tools/ClearAll #%r", false, -2)]
#endif
        public static void DeleteAllPrefs()
        {
            PlayerPrefs.DeleteAll();

            if (Directory.Exists(PersistantPath))
            {
                DeleteDirectory(PersistantPath);
                Debug.Log("ClearAll");
            }
            else
                Debug.Log("ClearOnlyPrefs");
        }

        private static void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                if (Path.GetFileName(file).Equals("Player.log", StringComparison.OrdinalIgnoreCase))
                    continue;


                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
                DeleteDirectory(dir);
            if (!Directory.EnumerateFileSystemEntries(targetDir).Any())
            {
                Directory.Delete(targetDir, false);
            }
        }

    }
}