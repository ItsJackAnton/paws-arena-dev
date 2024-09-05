using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class WebGlBuildPostProcess
{
    private const string ProjectName = "wasm";

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string destinationPath)
    {
        if (target != BuildTarget.WebGL)
        {
            return;
        }

        // Clean destination folder, excluding the Build folder
        CleanDestinationFolder(destinationPath);

        // Copy all files and folders from the template directory to the destination, excluding the Build folder
        CopyTemplateFiles(destinationPath);
    }

    private static void CleanDestinationFolder(string destinationPath)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(destinationPath);

        foreach (var file in dirInfo.GetFiles())
        {
            if (file.Name.Equals("Build"))
            {
                continue;
            }
            file.Delete();
        }

        foreach (var dir in dirInfo.GetDirectories())
        {
            if (dir.Name.Equals("Build"))
            {
                continue;
            }
            dir.Delete(true);
        }

        Debug.Log($"Cleaned up destination folder: {destinationPath}, excluding the Build folder");
    }

    private static void CopyTemplateFiles(string destinationPath)
    {
        string indexPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("WebTemplate/index"));
        string baseTemplatePath = Path.GetDirectoryName(indexPath);

        // Copy all files and directories from the template folder to the destination, except the Build folder
        foreach (var filePath in Directory.GetFiles(baseTemplatePath))
        {
            if (Path.GetExtension(filePath).Equals(".meta"))
            {
                continue;
            }

            string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(filePath));
            File.Copy(filePath, destinationFilePath, true);
        }

        foreach (var directoryPath in Directory.GetDirectories(baseTemplatePath))
        {
            string directoryName = Path.GetFileName(directoryPath);
            if (directoryName.Equals("Build"))
            {
                continue;
            }

            string destinationDirectoryPath = Path.Combine(destinationPath, directoryName);
            CopyFolderContents(directoryPath, destinationDirectoryPath);
        }

        Debug.Log($"Copied template files from {baseTemplatePath} to {destinationPath}");
    }

    private static void CopyFolderContents(string sourcePath, string destinationPath)
    {
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }
        Directory.CreateDirectory(destinationPath);

        DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
        if (!sourceDir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourcePath}");
        }

        foreach (FileInfo file in sourceDir.GetFiles())
        {
            if (file.Extension.Equals(".meta"))
            {
                continue;
            }
            string destinationFilePath = Path.Combine(destinationPath, file.Name);
            file.CopyTo(destinationFilePath, true);
        }

        foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
        {
            string destinationSubdirPath = Path.Combine(destinationPath, subdir.Name);
            CopyFolderContents(subdir.FullName, destinationSubdirPath);
        }
    }
}
