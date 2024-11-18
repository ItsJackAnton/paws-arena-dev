using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class WebGlBuildPostProcess
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget _target, string _destinationPath)
    {
        if (_target != BuildTarget.WebGL)
        {
            return;
        }

        CleanDestinationFolder(_destinationPath);
        CopyTemplateFiles(_destinationPath);
    }

    private static void CleanDestinationFolder(string _destinationPath)
    {
        DirectoryInfo _dirInfo = new DirectoryInfo(_destinationPath);

        foreach (var _file in _dirInfo.GetFiles())
        {
            if (_file.Name.Equals("Build"))
            {
                continue;
            }
            _file.Delete();
        }

        foreach (var _dir in _dirInfo.GetDirectories())
        {
            if (_dir.Name.Equals("Build"))
            {
                continue;
            }
            _dir.Delete(true);
        }

        Debug.Log($"Cleaned up destination folder: {_destinationPath}, excluding the Build folder");
    }

    private static void CopyTemplateFiles(string _destinationPath)
    {
        string _indexPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("WebTemplate/index"));
        string _baseTemplatePath = Path.GetDirectoryName(_indexPath);

        // Copy all files and directories from the template folder to the destination, except the Build folder
        foreach (var _filePath in Directory.GetFiles(_baseTemplatePath))
        {
            if (Path.GetExtension(_filePath).Equals(".meta"))
            {
                continue;
            }

            string _destinationFilePath = Path.Combine(_destinationPath, Path.GetFileName(_filePath));
            File.Copy(_filePath, _destinationFilePath, true);
        }

        foreach (var _directoryPath in Directory.GetDirectories(_baseTemplatePath))
        {
            string _directoryName = Path.GetFileName(_directoryPath);
            if (_directoryName.Equals("Build"))
            {
                continue;
            }

            string _destinationDirectoryPath = Path.Combine(_destinationPath, _directoryName);
            CopyFolderContents(_directoryPath, _destinationDirectoryPath);
        }

        Debug.Log($"Copied template files from {_baseTemplatePath} to {_destinationPath}");
    }

    private static void CopyFolderContents(string _sourcePath, string _destinationPath)
    {
        if (Directory.Exists(_destinationPath))
        {
            Directory.Delete(_destinationPath, true);
        }
        Directory.CreateDirectory(_destinationPath);

        DirectoryInfo _sourceDir = new DirectoryInfo(_sourcePath);
        if (!_sourceDir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {_sourcePath}");
        }

        foreach (FileInfo _file in _sourceDir.GetFiles())
        {
            if (_file.Extension.Equals(".meta"))
            {
                continue;
            }
            string _destinationFilePath = Path.Combine(_destinationPath, _file.Name);
            _file.CopyTo(_destinationFilePath, true);
        }

        foreach (DirectoryInfo _subdir in _sourceDir.GetDirectories())
        {
            string _destinationSubdirPath = Path.Combine(_destinationPath, _subdir.Name);
            CopyFolderContents(_subdir.FullName, _destinationSubdirPath);
        }
    }
}
