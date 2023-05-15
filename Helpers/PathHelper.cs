using System.IO;

namespace Underall.Helpers;

public static class PathHelper
{
    private static readonly string _saveFolderName = "saves";
    // private static readonly string _levelInfoListName = "saves";
    private static readonly string _levelInfoFileName = "_level.json";
    private static readonly string _saveInfoFileName = "_save.json";
    private static readonly string _configFileName = "game_config.json";
    public static char PathSeparator => Path.DirectorySeparatorChar;
    
    private static readonly string ParentDirectory = new DirectoryInfo(".").FullName;
    private static readonly string SavesDirectory = Path.Combine(ParentDirectory, _saveFolderName);
    public static readonly string ConfigFullPath = Path.Combine(ParentDirectory, _configFileName);
    
    public static string GetFullPathTo(string fileName)
    {
        return Path.Combine(ParentDirectory, fileName);
    }

    private static string GetFullPathToSaveFolder(string folderName) => Path.Combine(SavesDirectory, folderName);

    public static string GetFullPathToSaveInfo(string saveFolderName) =>
        Path.Combine(GetFullPathToSaveFolder(saveFolderName), _saveInfoFileName);
    public static string GetFullPathToSavedLevelInfo(string saveFolderName) =>
        Path.Combine(GetFullPathToSaveFolder(saveFolderName), _levelInfoFileName);

    private static void CreateFolderIfNotExists(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static void CreateSaveFolderIfNotExists(string saveFolderName) => 
        CreateFolderIfNotExists(GetFullPathToSaveFolder(saveFolderName));

    public static void WriteToSaveInfo(string saveFolderName, string content) =>
        File.WriteAllText(GetFullPathToSaveInfo(saveFolderName), content);

    public static void WriteToLevelInfo(string saveFolderName, string content) =>
        File.WriteAllText(GetFullPathToSavedLevelInfo(saveFolderName), content);
    
}